using PCSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCSC_Sample
{
    internal class CAPDUCommand :Class1
    {
        private IntPtr hContext = IntPtr.Zero;
        private readonly CAPDUResponse resp = new CAPDUResponse();
        protected IntPtr hCard = IntPtr.Zero;
        protected IntPtr activeProtocol = IntPtr.Zero;
        protected IntPtr pci = IntPtr.Zero;
        protected Api.SCARD_IO_REQUEST ioRecv = new Api.SCARD_IO_REQUEST();

        protected readonly uint maxRecvDataLen = 256;
        protected byte[] recvBuffer = new byte[257];

        public void init()
        {

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

        }

        public List<string> getCardList()
        {
            uint pcchReaders = 0;

            // NFCリーダの文字列バッファのサイズを取得
            var ret = Api.SCardListReaders(hContext, null, null, ref pcchReaders);
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

            var readerList = new List<string>();
            int offset = 0;
            // 認識したNDCリーダの最初の1台を使用
            int nullindex = readerNameMultiString.IndexOf((char)0);
            while (nullindex > 0)
            {
                var readerName = readerNameMultiString.Substring(offset, nullindex - offset);
                if (readerName.Length > 0)
                {
                    readerList.Add(readerName);
                }
                offset = nullindex + 1;
                nullindex = readerNameMultiString.IndexOf((char)0, offset);
            }
            return readerList;
        }

        public void connectCard(string cardName)
        {
            IntPtr hCard = IntPtr.Zero;
            IntPtr activeProtocol = IntPtr.Zero;
            var ret = Api.SCardConnect(hContext, cardName, Constant.SCARD_SHARE_SHARED, Constant.SCARD_PROTOCOL_T1, ref hCard, ref activeProtocol);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("カードに接続できません。code = " + ret);
            }
            Console.WriteLine("　カードに接続しました。");


            IntPtr handle = Api.LoadLibrary(DLLNAME);
            pci = Api.GetProcAddress(handle, PROCNAME);
            Api.FreeLibrary(handle);

            ioRecv.cbPciLength = 255;
        }

        public void transmit() 
        {
            uint maxRecvDataLen = 256;
            var recvBuffer = new byte[maxRecvDataLen + 2];
            var sendBuffer = new byte[] { 0xff, 0xca, 0x00, 0x00, 0x00 };  // ← IDmを取得するコマンド
            int pcbRecvLength = recvBuffer.Length;
            int cbSendLength = sendBuffer.Length;
            var ret = Api.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
            }
        }

        public void transmitPassword(string password)
        {
            uint maxRecvDataLen = 256;
            var recvBuffer = new byte[maxRecvDataLen + 2];

            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            var sendBuffer = new byte[data.Length + 5];// ← 認証用PINパスワード
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
            var pcbRecvLength = recvBuffer.Length;
            var cbSendLength = sendBuffer.Length;
            var ret = Api.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
            }
            if (resp.isError(recvBuffer, pcbRecvLength))
            {
                Console.WriteLine("ERROR");
                return;
            }
        }

        public void transmitResult()
        {
            uint maxRecvDataLen = 256;
            var recvBuffer = new byte[maxRecvDataLen + 2];

            var sendBuffer = new byte[] { 0x00, 0xb0, 0x00, 0x02, 0x01 };  // ← 基本4情報の読み取り（3バイト目のデータ長のみ）
            var pcbRecvLength = recvBuffer.Length;
            var cbSendLength = sendBuffer.Length;
            var ret = Api.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
            }
            if (resp.isError(recvBuffer, pcbRecvLength))
            {
                Console.WriteLine("ERROR");
                return;
            }

            // 基本4情報の読み取り（データ長：3 + 0x68）
            sendBuffer = new byte[4 + pcbRecvLength - 2];
            sendBuffer[0] = 0x00;
            sendBuffer[1] = 0xb0;
            sendBuffer[2] = 0x00;
            sendBuffer[3] = 0x00;
            sendBuffer[4] = (byte)(recvBuffer[0] + 3);

            recvBuffer = new byte[recvBuffer[0] + 2 + 3];
            pcbRecvLength = recvBuffer.Length;
            cbSendLength = sendBuffer.Length;
            ret = Api.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
            }
            if (resp.isError(recvBuffer, pcbRecvLength))
            {
                Console.WriteLine("ERROR");
                return;
            }

        }

        public void release()
        {
            if (hCard == IntPtr.Zero) {
                return;
            }

            uint ret = Api.SCardDisconnect(hCard, Constant.SCARD_LEAVE_CARD);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードとの切断に失敗しました。code = " + ret);
            }

        }
    }
}
