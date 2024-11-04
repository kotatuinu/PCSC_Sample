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

            Assert.IsFalse(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_正常系002()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x90, 0x00, };

            Assert.IsFalse(obj.isError(data));
        }

        [TestMethod()]
        public void isErrorTest_エラー0x62_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x62, 0x81, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x62_002()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x62, 0x89, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x62_003()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x62, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }

        [TestMethod()]
        public void isErrorTest_エラー0x63_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x63, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }

        [TestMethod()]
        public void isErrorTest_エラー0x64_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x64, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x65_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x65, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x67_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x67, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x68_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x68, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x69_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x69, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x6a_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x6a, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x6b_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x6b, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x6d_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x6d, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x6e_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x6e, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }
        [TestMethod()]
        public void isErrorTest_エラー0x6f_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x6f, 0x00, };

            Assert.IsTrue(obj.isError(data));
        }


        [TestMethod()]
        public void GetErrorInfoTest_正常系001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x90, 0x00, };

            var rslt = obj.GetErrorInfo(data);
            Assert.IsTrue(rslt.msg.Equals( "正常終了。"));
            Assert.IsFalse(rslt.isError);
        }
        [TestMethod()]
        public void GetErrorInfoTest_正常系002()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x90, 0x00, };

            var rslt = obj.GetErrorInfo(data);
            Assert.IsTrue(rslt.msg.Equals("正常終了。"));
            Assert.IsFalse(rslt.isError);
        }
 
        [TestMethod()]
        public void GetErrorInfoTest_エラー0x62_001()
        {
            var obj = new CAPDUResponse();
            byte[] data = { 0x00, 0x62, 0x81, };

            var rslt = obj.GetErrorInfo(data);
            Assert.IsTrue(rslt.msg.Equals("出力データに異常がある。"));
            Assert.IsTrue(rslt.isError);
            Assert.IsTrue(rslt.sw1 == 0x62);
            Assert.IsTrue(rslt.sw2 == 0x81);
        }
    }
}