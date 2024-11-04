using System;
using System.IO;
using System.Collections.Generic;

namespace PCSC_Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPCSCCardTest obj;
            List<string> paramLiist;
            switch (args[0])
            {
                case "1":
                    obj = new CAPDUCommandカード認識();
                    break;
                case "2":
                    obj = new CAPDUCommand証明書の取得();
                    break;
                case "3":
                    paramLiist = readParamFile(args[1]);
                    obj = new CAPDUCommandマイナンバー個人番号の取得();
                    obj.setParam("password", paramLiist[Int32.Parse(args[2])]);
                    break;
                case "4":
                    paramLiist = readParamFile(args[1]);
                    obj = new CAPDUCommand認証用秘密鍵による署名();
                    obj.setParam("password", paramLiist[Int32.Parse(args[2])]);
                    break;
                case "5":
                    paramLiist = readParamFile(args[1]);
                    obj = new CAPDUCommand基本4情報の取得();
                    obj.setParam("password", paramLiist[Int32.Parse(args[2])]);
                    break;
                default:
                    return;
            }
            obj.SCTest();

        }

        // どこかにパスワードを保存したファイルを読み込む。
        // 住民基本台帳・券面事項・利用者（4桁数字）や署名（）を大量に入れておいて、どこかに紛れ込ます
        static List<string> readParamFile(string path)
        {
            var result = new List<string>();

            StreamReader sr = new StreamReader(path);
            string line;
            do
            {
                line = sr.ReadLine();
                if(line != null) { 
                    result.Add(line);
                }

            } while (line != null);
            sr.Close();

            return result;
        }
    }
}
