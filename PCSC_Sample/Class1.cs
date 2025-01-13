using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PCSC.Api;
using static PCSC_Sample.Class1;

namespace PCSC_Sample
{
    internal class Class1
    {

        protected string DLLNAME;
        protected string PROCNAME;

        public enum COMMAND_KIND
        {
            READ_BINARY,    //0xb0
            WRITE_BINARY,   //0xd0
            UPDATE_BINARY,  //0xd6
            READ_RECORDS,   //0xb2
            WRITE_RECORD,   //0xd2
            APPEND_RECORD,  //0xe2
            UPDATE_RECORD,  //0xdc
            GET_DATA,       //0xca
            PUT_DATA,       //0xda
            SELECT_FILE,    //0xa4
            VERIFY,         //0x20
            INTERNAL_AUTHENTICATE,  //0x88
            EXTERNAL_AUTHENTICATE,  //0x82
            GET_CHALLENGE,  //00xb2
            LOCK_DF,        //
            UNLOCK_DF,
            UNLOCK_KEY,
            CHANGE_KEY,
            ERASE_ALL_RECORDS,
        }


        // カード認識
        public readonly struct COMMAND_INFO
        {
            public COMMAND_INFO(byte[] APDUCommand_, COMMAND_KIND kind_)
            {
                APDUcommand = APDUCommand_;
                kind = kind_;
            }
            public byte[] APDUcommand { get; }
            public COMMAND_KIND kind { get; }
        }

        public enum COMMAND_AP
        {
            券面入力補助AP,
            公的個人認証AP,
        }
        protected byte[] COMMAND_AP_INFO;

        private List<COMMAND_INFO> COMMAND_カード認識 = new List<COMMAND_INFO> {
            new COMMAND_INFO( new byte[]{ 0xff, 0xca, 0x00, 0x00, 0x00 }, COMMAND_KIND.GET_DATA ),  // ← IDmを取得するコマンド
        };

        // 証明書の取得
        private List<COMMAND_INFO> COMMAND_証明書の取得 = new List<COMMAND_INFO> {
            new COMMAND_INFO( new byte[]{ 0x00, 0xa4, 0x04, 0x0c, 0x0a, 0xd3, 0x92, 0xf0, 0x00, 0x26, 0x01, 0x00, 0x00, 0x00, 0x01 }, COMMAND_KIND.SELECT_FILE ),  // ← 公的個人認証AP
            new COMMAND_INFO( new byte[]{ 0x00, 0xa4, 0x02, 0x0c, 0x02, 0x00, 0x0a }, COMMAND_KIND.SELECT_FILE ),  // ← 認証用証明書
            new COMMAND_INFO( new byte[]{ 0x00, 0xb0, 0x00, 0x00, 0x04 }, COMMAND_KIND.READ_BINARY ),  // ← 証明書バイト数取得
            new COMMAND_INFO( new byte[]{ 0x00, 0xb0, 0x00, 0x04, 0x00, 0x00, 0x00 }, COMMAND_KIND.READ_BINARY ),  // ← 人商用証明書（データ取得；バイト数設定）
        };
    }
}
