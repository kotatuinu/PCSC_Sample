using PCSC;
using System;
using System.Text;

namespace PCSC_Sample
{
    // 認証用秘密鍵による署名
    class CAPDUCommand認証用秘密鍵による署名 : IPCSCCardTest
    {
        public override void SCTest()
        {
            IntPtr hContext = IntPtr.Zero;

            // ##################################################
            // 1. SCardEstablishContext
            // ##################################################
            Console.WriteLine("***** 1. SCardEstablishContext *****");
            uint ret = Api.SCardEstablishContext(Constant.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out hContext);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                string message;
                switch (ret)
                {
                    case Constant.SCARD_E_NO_SERVICE:
                        message = "サービスが起動されていません。";
                        break;
                    default:
                        message = "サービスに接続できません。code = " + ret;
                        break;
                }
                throw new ApplicationException(message);
            }

            if (hContext == IntPtr.Zero)
            {
                throw new ApplicationException("コンテキストの取得に失敗しました。");
            }
            Console.WriteLine("　サービスに接続しました。");


            // ##################################################
            // 2. SCardListReaders
            // ##################################################
            Console.WriteLine("***** 2. SCardListReaders *****");
            uint pcchReaders = 0;

            // NFCリーダの文字列バッファのサイズを取得
            ret = Api.SCardListReaders(hContext, null, null, ref pcchReaders);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                // 検出失敗
                throw new ApplicationException("NFCリーダを確認できません。");
            }

            // NFCリーダの文字列を取得
            byte[] mszReaders = new byte[pcchReaders * 2]; // 1文字2byte
            ret = Api.SCardListReaders(hContext, null, mszReaders, ref pcchReaders);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                // 検出失敗
                throw new ApplicationException("NFCリーダの取得に失敗しました。");
            }


            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            string readerNameMultiString = unicodeEncoding.GetString(mszReaders);

            // 認識したNDCリーダの最初の1台を使用
            int nullindex = readerNameMultiString.IndexOf((char)0);
            var readerName = readerNameMultiString.Substring(0, nullindex);
            Console.WriteLine("　NFCリーダを検出しました。 " + readerName);



            // ##################################################
            // 3. SCardConnect
            // ##################################################
            Console.WriteLine("***** 3. SCardConnect *****");
            IntPtr hCard = IntPtr.Zero;
            IntPtr activeProtocol = IntPtr.Zero;
            ret = Api.SCardConnect(hContext, readerName, Constant.SCARD_SHARE_SHARED, Constant.SCARD_PROTOCOL_T1, ref hCard, ref activeProtocol);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("カードに接続できません。code = " + ret);
            }
            Console.WriteLine("　カードに接続しました。");



            // ##################################################
            // 4. SCardTransmit
            // ##################################################
            Console.WriteLine("***** 4. SCardTransmit *****");

            Api.SCARD_IO_REQUEST ioRecv = new Api.SCARD_IO_REQUEST();
            ioRecv.cbPciLength = 255;

            IntPtr handle = Api.LoadLibrary("Winscard.dll");
            IntPtr pci = Api.GetProcAddress(handle, "g_rgSCardT1Pci");
            Api.FreeLibrary(handle);

            uint maxRecvDataLen = 256;
            var recvBuffer = new byte[maxRecvDataLen + 2];
            byte[] sendBuffer;
            sendBuffer = new byte[] { 0x00, 0xa4, 0x04, 0x0c, 0x0a, 0xd3, 0x92, 0xf0, 0x00, 0x26, 0x01, 0x00, 0x00, 0x00, 0x01 };  // ← 公的個人認証AP
            int pcbRecvLength = recvBuffer.Length;
            int cbSendLength = sendBuffer.Length;
            ret = Api.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
            }

            sendBuffer = new byte[] { 0x00, 0xa4, 0x02, 0x0c, 0x02, 0x00, 0x18 };  // ← 認証用PIN
            pcbRecvLength = recvBuffer.Length;
            cbSendLength = sendBuffer.Length;
            ret = Api.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
            }

            byte[] data = System.Text.Encoding.ASCII.GetBytes(params_["password"].ToString());
            sendBuffer = new byte[data.Length + 5];// ← 認証用PINパスワード
            sendBuffer[0] = 0x00;
            sendBuffer[1] = 0x20;
            sendBuffer[2] = 0x00;
            sendBuffer[3] = 0x80;
            sendBuffer[4] = 0x04;
            int idx = 0;
            foreach (var b in data)
            {
                sendBuffer[idx + 5] = data[idx];
                idx++;
            }
            pcbRecvLength = recvBuffer.Length;
            cbSendLength = sendBuffer.Length;
            ret = Api.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
            }

            sendBuffer = new byte[] { 0x00, 0xa4, 0x02, 0x0c, 0x02, 0x00, 0x17 };  // ← 認証用秘密鍵
            pcbRecvLength = recvBuffer.Length;
            cbSendLength = sendBuffer.Length;
            ret = Api.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
            }

            sendBuffer = new byte[] { 0x80, 0x2a, 0x00, 0x80, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00 };  // ← 署名付与
            data = System.Text.Encoding.ASCII.GetBytes("test");
            sendBuffer[5] = data[0];
            sendBuffer[6] = data[1];
            sendBuffer[7] = data[2];
            sendBuffer[8] = data[3];
            pcbRecvLength = recvBuffer.Length;
            cbSendLength = sendBuffer.Length;
            ret = Api.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
            }

            // ##################################################
            // 5. SCardDisconnect
            // ##################################################
            Console.WriteLine("***** 5. SCardDisconnect *****");
            ret = Api.SCardDisconnect(hCard, Constant.SCARD_LEAVE_CARD);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードとの切断に失敗しました。code = " + ret);
            }
            Console.WriteLine("　カードを切断しました。");
        }
    }
}
