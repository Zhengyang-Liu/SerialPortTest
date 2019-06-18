using System;
using System.Windows;
using System.IO.Ports;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace SerialPortTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort;
        TextBoxOutputter outputter;
        StringBuilder sb = new StringBuilder();
        private Timer timer;
        string filePath;

        public MainWindow()
        {
            InitializeComponent();

            outputter = new TextBoxOutputter(ConsoleTextBox);
            Console.SetOut(outputter);
            Console.WriteLine("Started");

            PortNameComboBox.ItemsSource = SerialPort.GetPortNames();
            BaudRateTextBox.Text = "38400";
        }

        private void StartReadingButton_Click(object sender, RoutedEventArgs e)
        {
            string portName = PortNameComboBox.SelectedValue.ToString();
            int baudRate = int.Parse(BaudRateTextBox.Text);

            serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;
            serialPort.DataReceived += DataReceivedHandler;

            filePath = FolderPathText.Text + "\\" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss")+".txt";

            Console.WriteLine("Start Reading PortName: " + portName + " BaudRate: " + baudRate);

            InitTimer();
            //InitTimer2();
            serialPort.Open();
        }

        public void InitTimer()
        {
            timer = new Timer();
            timer.Tick += WriteDataEvent;
            timer.Interval = 20000; // in miliseconds
            timer.Start();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.Write(indata);
            sb.Append(indata);

            ConsoleTextBox.ScrollToEnd();
            while (ConsoleTextBox.LineCount > 20)
            {
                ConsoleTextBox.Text = ConsoleTextBox.Text.Remove(0, ConsoleTextBox.GetLineLength(0));
            }
        }

        private void Browse_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FolderPathText.Text = dialog.SelectedPath;
            }
        }

        private void WriteDataEvent(object sender, EventArgs e)
        {
            File.AppendAllText(filePath, sb.ToString());
            sb.Clear();
        }

        //public void InitTimer2()
        //{
        //    timer = new Timer();
        //    timer.Tick += WriteDate;
        //    timer.Interval = 30; // in miliseconds
        //    timer.Start();
        //    ConsoleTextBox.ScrollToEnd();
        //}

        //private void WriteDate(object sender, EventArgs e)
        //{
            
        //    string data = DateTime.Now.ToString();
        //    sb.AppendLine(data);
        //    Console.WriteLine(data);
        //    ConsoleTextBox.ScrollToEnd();

        //    while (ConsoleTextBox.LineCount > 10)
        //    {
        //        ConsoleTextBox.Text = ConsoleTextBox.Text.Remove(0, ConsoleTextBox.GetLineLength(0));
        //    }
        //}
    }
}
