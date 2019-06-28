using System;
using System.Windows;
using System.IO.Ports;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace SerialPortTest.DataSource
{
    class FakeDataSource : IDataSource
    {
        Timer timer;
        Action<string> appendDataFunction;

        public FakeDataSource(Action<string> appendDataFunction)
        {
            this.appendDataFunction = appendDataFunction;
        }

        public void Start()
        {
            InitDataGeneratorTimer();
        }

        public void InitDataGeneratorTimer()
        {
            timer = new Timer();
            timer.Tick += WriteDate;
            timer.Interval = 30;
            timer.Start();
        }

        private void WriteDate(object sender, EventArgs e)
        {
            string data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "|0.003\n";
            appendDataFunction(data);
        }
    }
}
