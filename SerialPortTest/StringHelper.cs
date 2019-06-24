using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Documents;

namespace SerialPortTest
{
    public static class StringHelper
    {
        public static List<DataStruct> GetDataList(StreamReader streamReader)
        {
            List<DataStruct> dataList = new List<DataStruct>();

            string s;
            while ((s = streamReader.ReadLine()) != null)
            {
                string[] split = s.Split(new Char[] { '|' });
                dataList.Add(new DataStruct(DateTime.Parse(split[0]), split[1]));
            }

            return dataList;
        }

        public static List<DataStruct> GetDataList(string[] lineArray)
        {
            List<DataStruct> dataList = new List<DataStruct>();

            foreach (string line in lineArray)
            {
                if (line.Contains('|'))
                {
                    try
                    {
                        string[] split = line.Split(new Char[] { '|' });
                        dataList.Add(new DataStruct(DateTime.ParseExact(split[0], "yyyy-M-d H.m.s.fffff", null), split[1]));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Corrupted line: " + line);
                    }
                }
            }

            return dataList;
        }
    }
}
