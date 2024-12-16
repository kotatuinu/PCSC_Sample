using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCSC_Sample
{
    public class CAPDUResponse
    {
        // ステータスワードSW1、SW2
        public class RESP_SW2
        {
            public RESP_SW2()
            {
                isError = true;
                definedflg = true;
            }
            public bool isError;
            public byte sw2;
            public bool definedflg;
            public string msg;
        };
        private Dictionary<byte, List<RESP_SW2>> STATUS_WORD = new Dictionary<byte, List<RESP_SW2>>()
        {
            {  0x90, new List<RESP_SW2> {new RESP_SW2{ isError= false, sw2 = 0x00, msg= "正常終了。" }, } },
            {  0x62, new List<RESP_SW2> {
                new RESP_SW2{ sw2=0x81, msg="出力データに異常がある。"},
                new RESP_SW2{ sw2=0x83, msg="DFが閉塞している。"},
                new RESP_SW2{ sw2=0x00, definedflg=false,  msg="警告終了：不揮発性メモリの状態が変化していない。"},
            } },
            {  0x63, new List<RESP_SW2> {
                new RESP_SW2{ sw2=0x00, msg="照合不一致。"},
                new RESP_SW2{ sw2=0x81, msg="ファイルが今回の書き込みによって一杯になった。"},
                new RESP_SW2{ sw2=0xc1, msg="照合不一致（残り再試行回数1）。"},
                new RESP_SW2{ sw2=0xc2, msg="照合不一致（残り再試行回数2）。"},
                new RESP_SW2{ sw2=0xc3, msg="照合不一致（残り再試行回数3）。"},
                new RESP_SW2{ sw2=0xc4, msg="照合不一致（残り再試行回数4）。"},
                new RESP_SW2{ sw2=0xc5, msg="照合不一致（残り再試行回数5）。"},
                new RESP_SW2{ sw2=0xc6, msg="照合不一致（残り再試行回数6）。"},
                new RESP_SW2{ sw2=0xc7, msg="照合不一致（残り再試行回数7）。"},
                new RESP_SW2{ sw2=0xc8, msg="照合不一致（残り再試行回数8）。"},
                new RESP_SW2{ sw2=0xc9, msg="照合不一致（残り再試行回数9）。"},
                new RESP_SW2{ sw2=0xca, msg="照合不一致（残り再試行回数10）。"},
                new RESP_SW2{ sw2=0xcb, msg="照合不一致（残り再試行回数11）。"},
                new RESP_SW2{ sw2=0xcc, msg="照合不一致（残り再試行回数12）。"},
                new RESP_SW2{ sw2=0xcd, msg="照合不一致（残り再試行回数13）。"},
                new RESP_SW2{ sw2=0xce, msg="照合不一致（残り再試行回数14）。"},
                new RESP_SW2{ sw2=0xcf, msg="照合不一致（残り再試行回数15）。"},
                new RESP_SW2{ sw2=0x00, definedflg=false,  msg="警告終了：不揮発性メモリの状態が変化している。"},
            } },
            {  0x64, new List<RESP_SW2> {
                new RESP_SW2{ sw2=0x00, msg="ファイル制御情報に異常がある。"},
                new RESP_SW2{ sw2=0x00, definedflg=false,  msg="実行誤り：不揮発性メモリの状態が変化していない。"},
            } },
            {  0x65, new List<RESP_SW2> {
                new RESP_SW2{ sw2=0x00, msg="メモリへの書き込みが失敗した。"},
                new RESP_SW2{ sw2=0x00, definedflg=false,  msg="実行誤り。不揮発性メモリの状態が変化している。"},
            } },
            {  0x67, new List<RESP_SW2> {new RESP_SW2{ sw2=0x00, msg= "Lc/Leフィールドが間違っている。" }, } },
            {  0x68, new List<RESP_SW2> {
                new RESP_SW2{ sw2=0x81,  msg="指定された論理チャンネル番号によるアクセス機能を提供しない。"},
                new RESP_SW2{ sw2=0x82,  msg="CLAバイトで指定されたセキュアメッセージング機能を提供しない。"},
                new RESP_SW2{ sw2=0x00, definedflg=false,  msg="検査誤り。CLAの機能が提供されない。"},
            } },
            {  0x69, new List<RESP_SW2> {
                new RESP_SW2{ sw2=0x81,  msg="ファイル構造と矛盾したコマンドである。"},
                new RESP_SW2{ sw2=0x82,  msg="セキュリティステータスが満足されない。"},
                new RESP_SW2{ sw2=0x83,  msg="認証方法を受け付けない。"},
                new RESP_SW2{ sw2=0x84,  msg="参照されたIEFが閉塞している。"},
                new RESP_SW2{ sw2=0x85,  msg="コマンドの使用条件が満足されない。"},
                new RESP_SW2{ sw2=0x86,  msg="ファイルが存在しない。"},
                new RESP_SW2{ sw2=0x87,  msg="セキュアメッセージングに必要なデータオブジェクトが存在しない。"},
                new RESP_SW2{ sw2=0x88,  msg="セキュアメッセージング関連エラー。"},
                new RESP_SW2{ sw2=0x00, definedflg=false,  msg="検査誤り。コマンドは許されない。"},
            } },
            {  0x6a, new List<RESP_SW2> {
                new RESP_SW2{ sw2=0x80,  msg="データフィールドのタグが正しくない。"},
                new RESP_SW2{ sw2=0x81,  msg="機能が提供されていない。"},
                new RESP_SW2{ sw2=0x82,  msg="ファイルが存在しない。"},
                new RESP_SW2{ sw2=0x83,  msg="アクセス対象のレコードがない。"},
                new RESP_SW2{ sw2=0x84,  msg="ファイル内に十分なメモリ容量がない。"},
                new RESP_SW2{ sw2=0x85,  msg="Lcの値がTLV構造に矛盾している。"},
                new RESP_SW2{ sw2=0x86,  msg="P1-P2の値が正しくない。"},
                new RESP_SW2{ sw2=0x87,  msg="Lcの値がP1-P2に矛盾している。"},
                new RESP_SW2{ sw2=0x88,  msg="参照された鍵が正しく設定されていない。"},
                new RESP_SW2{ sw2=0x00, definedflg=false,  msg="検査誤り。間違ったパラメータP1,P2。"},
            } },
            {  0x6b, new List<RESP_SW2> {new RESP_SW2{ sw2 = 0x00, msg= "EF範囲外にオフセット指定した(検査誤り)。" }, } },
            {  0x6d, new List<RESP_SW2> {new RESP_SW2{ sw2 = 0x00, msg= "INSが提供されていない(検査誤り)。" }, } },
            {  0x6e, new List<RESP_SW2> {new RESP_SW2{ sw2 = 0x00, msg= "CLAが提供されていない(検査誤り)。" }, } },
            {  0x6f, new List<RESP_SW2> {new RESP_SW2{ sw2 = 0x00, msg= "自己診断異常（検査誤り）。" }, } },
        };

        public bool isError(byte[] respData, int RecvBuffLen)
        {
            RESP_STATUS status = GetErrorInfo(respData, RecvBuffLen);
            return status.isError;
        }

        public struct RESP_STATUS
        {
            public bool isError;
            public byte sw1;
            public byte sw2;
            public string msg;
        };
        public RESP_STATUS GetErrorInfo(byte[] respData, int RecvBuffLen)
        {
            if (!STATUS_WORD.ContainsKey(respData[RecvBuffLen - 2]))
            {
                // 存在しないエラー
                return new RESP_STATUS { isError = true };

            }

            var sw2List = STATUS_WORD[respData[RecvBuffLen - 2]];
            foreach (var item in sw2List)
            {
                // SW2が定型値でありその値に一致する場合、
                // または定型値を全て参照した後に定型値以外の定義がある場合、
                // その結果を返す
                if ((item.definedflg && item.sw2 == respData[RecvBuffLen-1]) || !item.definedflg)
                {
                    return new RESP_STATUS { isError = item.isError, sw1 = respData[RecvBuffLen-2], sw2 = item.sw2, msg = item.msg };
                }
            }

            return new RESP_STATUS { isError = true };
        }
    }
}
