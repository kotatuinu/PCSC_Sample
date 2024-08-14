using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCSC_Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCSC_Sample.Tests
{
    [TestClass()]
    public class CAPDUResponseTests
    {
        [TestMethod()]
        public void isErrorTest_正常系001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x90, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_正常系002()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x90, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }

        [TestMethod()]
        public void GetErrorInfoTest_正常系001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x90, 0x00, };

            var rslt = obj.GetErrorInfo(data);
            Assert.IsTrue(rslt.msg.Equals( "正常終了。"));
            Assert.IsTrue(rslt.isError);
        }
        [TestMethod()]
        public void GetErrorInfoTest_正常系002()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x90, 0x00, };

            var rslt = obj.GetErrorInfo(data);
            Assert.IsTrue(rslt.msg.Equals("正常終了。"));
            Assert.IsTrue(rslt.isError);
        }
    }
}