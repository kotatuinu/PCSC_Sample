using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCSC_Sample
{
    internal class Class券面入力補助AP : CAPDUCommand
    {
        public Class券面入力補助AP()
        {
            DLLNAME = "Winscard.dll";
            PROCNAME = "g_rgSCardT1Pci";
            COMMAND_AP_INFO = new byte[] { 0x00, 0xa4, 0x04, 0x0c, 0x0a, 0xd3, 0x92, 0x10, 0x00, 0x31, 0x00, 0x01, 0x01, 0x04, 0x08 }; // ← 券面入力補助AP (DF)
        }
    }
}
