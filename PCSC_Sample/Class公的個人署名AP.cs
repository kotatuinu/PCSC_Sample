using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCSC_Sample
{
    internal class Class公的個人署名AP : CAPDUCommand
    {
        public Class公的個人署名AP()
        {
            DLLNAME = "Winscard.dll";
            PROCNAME = "g_rgSCardT1Pci";
            COMMAND_AP_INFO = new byte[] { 0x00, 0xa4, 0x04, 0x0c, 0x0a, 0xd3, 0x92, 0xf0, 0x00, 0x26, 0x01, 0x00, 0x00, 0x00, 0x01 };  // ← 公的個人署名AP
        }
    }
}
