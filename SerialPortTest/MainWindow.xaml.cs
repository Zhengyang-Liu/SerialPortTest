using System;
using System.Windows;
using System.IO.Ports;
using System.Windows.Forms;
using System.IO;
using System.Text;
using SerialPortTest.DataSource;

namespace SerialPortTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBoxOutputter outputter;
        StringBuilder sb = new StringBuilder();
        Timer timer;
        string filePath;
        IDataSource dataSource;

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
            string portName = PortNameComboBox.SelectedIndex != -1 ? PortNameComboBox.SelectedValue.ToString() : "";
            int baudRate = int.Parse(BaudRateTextBox.Text);

            filePath = FolderPathText.Text + "\\" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + ".txt";

            if(EnableFakeData.IsChecked == true)
            {
                dataSource = new FakeDataSource(sb, AppendData);
            }else
            {
                dataSource = new ArduinoDataSource(sb, AppendData, portName, baudRate);
            }

            InitDataWriteTimer();
            dataSource.Start();
        }

        public void InitDataWriteTimer()
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

        private void AppendData(string data)
        {
            sb.Append(data);
            Console.Write(data);
            ConsoleTextBox.ScrollToEnd();

            this.Dispatcher.Invoke(() =>
            {
                while (ConsoleTextBox.LineCount > 20)
                {
                    ConsoleTextBox.Text = ConsoleTextBox.Text.Remove(0, ConsoleTextBox.GetLineLength(0));
                }
            });
        }
    }
}
