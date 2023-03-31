using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
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
        NetworkStream NetStream = null;
        StreamReader sr = null;
        StreamWriter sw = null;
        TcpClient client;

        static string ClientName = null;
        public Chat(string name)
        {
            InitializeComponent();
            ClientName = name;
            Nickname.Content = ClientName;
            client = new TcpClient();
            string serverAddress = ConfigurationManager.AppSettings["ServerAddress"]!;
            short serverPort = short.Parse(ConfigurationManager.AppSettings["ServerPort"]!);
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            DataContext = messages;
        }
        async void Listen()
        {
            try
            {
                while (true)
                {
                    string? message = await sr.ReadLineAsync();
                    messages.Add(new MessageInfo(message));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageTextBox.Text != "")
                {
                    string message = ClientName + " : " + MessageTextBox.Text;
                    sw.WriteLine(message);
                    sw.Flush();
                    MessageTextBox.Text = "";
                }
                else MessageBox.Show("Please< enter ypur messages");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Join_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Connect(serverEndPoint);
                NetStream = client.GetStream();
                sr = new StreamReader(NetStream);
                sw = new StreamWriter(NetStream);
                Listen();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            NetStream.Close();
            client.Close();
        }

        private void Disconnect_Button_Click(object sender, RoutedEventArgs e)
        {
            sw.WriteLine("exit");
            sw.Flush();
            NetStream.Close();
            client.Close();
        }
    }
    class MessageInfo
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public MessageInfo(string? message)
        {
            Message = message ?? "";
            Time = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{Message} : {Time}";
        }
    }
}
