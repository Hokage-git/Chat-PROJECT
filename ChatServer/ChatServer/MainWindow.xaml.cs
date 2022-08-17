using SuperSimpleTcp;
using System;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;

namespace ChatServer
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

        SimpleTcpServer tcpServer;

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            tcpServer.Start();
            connectButton.IsEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tcpServer = new SimpleTcpServer(ipTxt.Text);
            tcpServer.Events.ClientConnected += Events_ClientConnected;
            tcpServer.Events.ClientDisconnected += Events_ClientDisconnected;
            tcpServer.Events.DataReceived += Events_DataReceived;
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                messageBlock.Items.Add($"{e.IpPort}: {Encoding.UTF8.GetString(e.Data)}");
            }));
        }

        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                messageBlock.Items.Add($"{e.IpPort} has Disconnected");
                userList.Items.Remove(e.IpPort);
            }));
        }

        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                messageBlock.Items.Add($"{e.IpPort} Connected");
                userList.Items.Add(e.IpPort);
            }));
        }

        private void messageBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            else if(!string.IsNullOrEmpty(messageBox.Text) && userList.SelectedItem != null)
            {
                messageBlock.Items.Add($"Server: {messageBox.Text}");
                tcpServer.Send(userList.SelectedItem.ToString(), messageBox.Text);
                messageBox.Text = string.Empty;
            }
            e.Handled = true;
        }
    }
}
