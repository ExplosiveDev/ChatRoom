using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatRoom
{
    public partial class Chat : Window
    {
        ObservableCollection<MessageInfo> messages = new ObservableCollection<MessageInfo>();

        IPEndPoint serverEndPoint;
        UdpClient client;
        public Chat(string name)
        {
            InitializeComponent();
            Nickname.Content = name;
            client = new UdpClient();
            string serverAddress = ConfigurationManager.AppSettings["ServerAddress"]!;
            short serverPort = short.Parse(ConfigurationManager.AppSettings["ServerPort"]!);
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            DataContext = messages;
        }
        async void Listen()
        {
            while (true)
            {
                var result = await client.ReceiveAsync();
                string message = Encoding.Unicode.GetString(result.Buffer);
                messages.Add(new MessageInfo(message));
            }
        }
        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageTextBox.Text != "")
            {

                string message = MessageTextBox.Text;
                SendMessage(message);
                MessageTextBox.Text = "";
            }
            else MessageBox.Show("Please< enter ypur messages");
        }

        private void Join_Button_Click(object sender, RoutedEventArgs e)
        {
            string message = "$<join>";
            SendMessage(message);
            Listen();
        }
        private async void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            await client.SendAsync(data, data.Length, serverEndPoint);
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    class MessageInfo
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public MessageInfo(string message)
        {
            Message = message;
            Time = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{Message} : {Time}";
        }
    }
}
