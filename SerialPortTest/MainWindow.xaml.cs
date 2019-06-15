using System;
using System.Windows;
using System.IO.Ports;


namespace SerialPortTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort;
        TextBoxOutputter outputter;
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
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            Console.WriteLine("Start Reading PortName: " + portName + " BaudRate: " + baudRate);

            serialPort.Open();
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine("Data Received:");
            Console.Write(indata);
        }
    }
}
