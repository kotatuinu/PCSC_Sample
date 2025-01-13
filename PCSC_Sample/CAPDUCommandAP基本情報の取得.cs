using PCSC;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Text;

namespace PCSC_Sample
{
    class DATA_AP基本情報
    {
        public byte[] tag_; // TAG[2byte] FF40
        public byte[] tag {
            get => tag_;
            private set => tag_ = value;
        }
        public int len_;    // Len[1byte]
        public int len
        {
            get => len_;
            private set => len_ = value;
        }
        //  TAG[2byte] DF41
        //  Len[1byte]
        //  Value[4byte]
        //   AP仕様バージョン[1byte]
        //   拡張Lc/Le対応種別[1byte]
        //   ベンダ分類[1byte]
        //   ベンダ任意項目[1byte]
        //  TAG[2byte] DF42
        //  Len[1byte]
        //  Value 証明書検証用公開鍵（署名検証用）の鍵ID[1byte]
        //  Value その他データ
        public List<ClassTLV> data_ = new List<ClassTLV>();
        public List<ClassTLV> data
        {
            get => data_;
        }

        public static DATA_AP基本情報 bulde(byte[] data)
        {
            var obj = new DATA_AP基本情報();
            int offset = 0;
            var data1 = ClassTLV.create(data, 2, ref offset);
            obj.tag = data1.tag;
            obj.len = data1.len;

            offset = 0;
            var data1_1 = ClassTLV.create(data1.val, 2, ref offset);
            obj.data_.Add(data1_1);
            var data1_2 = ClassTLV.create(data1.val, 2, ref offset);
            obj.data_.Add(data1_2);
            var data1_3 = ClassTLV.create(data1.val, 2, ref offset);
            obj.data_.Add(data1_3);

            return obj;
        }

        public void dispData()
        {
            Console.Write("TAG:");
            ClassTLV.dispRowData(tag);
            Console.WriteLine("LEN:{0:X2}", len);
            foreach (var obj in data)
            {
                ClassTLV.dispData(obj);
            }
        }
    }

    // AP基本情報の取得
    class CAPDUCommandAP基本情報の取得 : IPCSCCardTest
    {
        public override void SCTest()
        {
            IntPtr hContext = IntPtr.Zero;
            var resp = new CAPDUResponse();

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
            var sendBuffer = new byte[] { 0x00, 0xa4, 0x04, 0x0c, 0x0a, 0xd3, 0x92, 0x10, 0x00, 0x31, 0x00, 0x01, 0x01, 0x04, 0x08 };  // ← 券面入力補助AP (DF)
            int pcbRecvLength = recvBuffer.Length;
            int cbSendLength = sendBuffer.Length;
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

            sendBuffer = new byte[] { 0x00, 0xb0, 0x85, 0x00, 0xff };  // ← AP基本情報バイト数取得
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

            var obj = DATA_AP基本情報.bulde(recvBuffer);
            obj.dispData();

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
