using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortTest
{
    public struct DataStruct
    {
        public string dateTime;
        public string data;
        public DataStruct(string dateTime, string data)
        {
            this.dateTime = dateTime;
            this.data = data;
        }
    }
}
