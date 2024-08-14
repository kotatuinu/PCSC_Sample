using System;
using System.Collections;
using System.Collections.Generic;

namespace PCSC_Sample
{
    abstract class IPCSCCardTest
    {
        public abstract void SCTest();

        protected Hashtable params_ = new Hashtable();
        public void setParam(string param, string value)
        {
            params_.Add(param, value);
        }
    }
}