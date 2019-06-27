using System;
using System.Collections.Generic;
using System.Windows;
using System.IO.Ports;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace SerialPortTest.DataSource
{
    class ArduinoDataSource : IDataSource
    {
        public string PortName
        {
            get;
            set;
        }

        public int BaudRate
        {
            get;
            set;
        }

        public SerialPort serialPort
        {
            get;
            set;
        }

        Action<string> appendDataFunction;

        public ArduinoDataSource(Action<string> appendDataFunction, string portName = "", int baudRate = 0, string folderPath = "")
        {
            this.PortName = portName;
            this.BaudRate = baudRate;
            this.appendDataFunction = appendDataFunction;
        }

        public void Start()
        {
            serialPort = new SerialPort();
            serialPort.PortName = this.PortName;
            serialPort.BaudRate = this.BaudRate;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;
            serialPort.DataReceived += DataReceivedHandler;

            Console.WriteLine("Start Reading PortName: " + this.PortName + " BaudRate: " + this.BaudRate);

            serialPort.Open();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            string[] stringArray = indata.Split('\n');
            foreach (string str in stringArray)
            {
                appendDataFunction(str +"\n");
            }
        }
    }
}
