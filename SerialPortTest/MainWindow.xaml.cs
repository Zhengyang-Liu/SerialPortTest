using System;
using System.Windows;
using System.IO.Ports;
using System.Windows.Forms;
using System.IO;
using System.Text;
using SerialPortTest.DataSource;
using System.Collections.Generic;

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
        string fileName;
        IDataSource dataSource;
        DatabaseManager databaseManager;

        public MainWindow()
        {
            InitializeComponent();
            InitUserData();
        }

        private void InitUserData()
        {
            BaudRateTextBox.Text = Properties.Settings.Default.BaudRate;
            FolderPathText.Text = Properties.Settings.Default.FolderPath;
            ServerNameTextBox.Text = Properties.Settings.Default.ServerName;
            DatabaseNameTextBox.Text = Properties.Settings.Default.DatabaseName;
            UserNameTextBox.Text = Properties.Settings.Default.UserName;
            PasswordTextBox.Text = Properties.Settings.Default.Password;

            PortNameComboBox.ItemsSource = SerialPort.GetPortNames();
        }

        private void StartReadingButton_Click(object sender, RoutedEventArgs e)
        {
            InitDataSource();
            InitDataWriteTimer();
            dataSource.Start();
            InitDatabase();
        }

        private void InitDataSource()
        {
            string portName = PortNameComboBox.SelectedIndex != -1 ? PortNameComboBox.SelectedValue.ToString() : "";
            int baudRate = int.Parse(BaudRateTextBox.Text);

            fileName = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            filePath = FolderPathText.Text + "\\" + fileName + ".txt";

            if (EnableFakeData.IsChecked == true)
            {
                dataSource = new FakeDataSource(sb, AppendData);
            }
            else
            {
                dataSource = new ArduinoDataSource(sb, AppendData, portName, baudRate);
            }
        }

        private void InitDataWriteTimer()
        {
            timer = new Timer();
            timer.Tick += WriteDataEvent;
            timer.Interval = 20000; // in miliseconds
            timer.Start();
        }

        private void InitDatabase()
        {
            if(UploadCheckBox.IsChecked == false)
                return;

            this.databaseManager = new DatabaseManager(ServerNameTextBox.Text, DatabaseNameTextBox.Text, UserNameTextBox.Text, PasswordTextBox.Text);
            this.databaseManager.CreateTable(this.fileName);
        }

        private void Browse_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FolderPathText.Text = dialog.SelectedPath;
            }
        }

        private void Backfill_Button_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(FolderPathText.Text);
            DirectoryInfo[] subDiArr = di.GetDirectories();

            foreach(FileInfo file in di.GetFiles())
            {
                databaseManager.CreateTable(file.Name);
                StreamReader sr = file.OpenText();
                List<DataStruct> DataList = StringHelper.GetDataList(sr);
                databaseManager.BulkInsert(file.Name, DataList);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.BaudRate = BaudRateTextBox.Text;
            Properties.Settings.Default.FolderPath = FolderPathText.Text;
            Properties.Settings.Default.ServerName = ServerNameTextBox.Text;
            Properties.Settings.Default.DatabaseName = DatabaseNameTextBox.Text;
            Properties.Settings.Default.UserName = UserNameTextBox.Text;
            Properties.Settings.Default.Password = PasswordTextBox.Text;

            Properties.Settings.Default.Save();

            base.OnClosing(e);
        }

        private void WriteDataEvent(object sender, EventArgs e)
        {
            File.AppendAllText(filePath, sb.ToString());

            if(UploadCheckBox.IsChecked == true)
            {
                string[] lines = sb.ToString().Split('\n');
                List<DataStruct> DataList = StringHelper.GetDataList(lines);
                databaseManager.BulkInsert(fileName, DataList);
            }

            sb.Clear();
        }

        private void AppendData(string data)
        {
            sb.Append(data);

            this.Dispatcher.Invoke(() =>
            {
                ConsoleTextBox.Text = data;
            });
        }
    }
}
