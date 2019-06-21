using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortTest
{
    public struct DataStruct
    {
        public DateTime dateTime;
        public string data;
        public DataStruct(DateTime dateTime, string data)
        {
            this.dateTime = dateTime;
            this.data = data;
        }
    }
}
