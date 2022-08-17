using SuperSimpleTcp;
using System;
using System.Windows.Threading;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        SimpleTcpClient tcpClient;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var data = new byte[4];
            new Random().NextBytes(data);
            IPAddress ip = new IPAddress(data);
            ipTxt.Text = "127.0.0.1:4747";//ip.ToString() + $":{new Random().Next(1000, 9999)}";
            tcpClient = new SimpleTcpClient(ipTxt.Text);

            tcpClient.Events.Connected += Events_Connected;
            tcpClient.Events.Disconnected += Events_Disconnected;
            tcpClient.Events.DataReceived += Events_DataReceived;
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                chatMesHistory.Items.Add($"{Encoding.UTF8.GetString(e.Data)}");
            }));
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                chatMesHistory.Items.Add($"Disconnected");
            }));
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                chatMesHistory.Items.Add($"Connected");
            }));
        }

        private void nameConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                tcpClient.Connect();
                nameConfirmBtn.IsEnabled = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Cервер не запущен","Message",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void messageTxt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            else if (!string.IsNullOrEmpty(messageTxt.Text))
            {
                chatMesHistory.Items.Add($"{nameTxt.Text}: {messageTxt.Text}");
                tcpClient.Send(messageTxt.Text);
                messageTxt.Text = string.Empty;
            }
            e.Handled = true;
        }
    }
}
