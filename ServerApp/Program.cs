using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

ChatServer chatserver = new ChatServer();

try
{

    chatserver.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

class ChatServer
{
    const short port = 4040;
    const string address = "127.0.0.1";

    TcpListener listener = null;

    public ChatServer()
    {
        listener = new TcpListener(IPAddress.Parse(address),port);

    }
    public void Start()
    {
        listener.Start();
        Console.WriteLine("Waiting for connection ......");
        TcpClient client = listener.AcceptTcpClient();
        Console.WriteLine("Connceted");
        var NetStream = client.GetStream();
        StreamReader sr = new StreamReader(NetStream);
        StreamWriter sw = new StreamWriter(NetStream);

        while (true)
        { 
            string? message = sr.ReadLine();
            if(message == "exit") {listener.Start(); break;}
            Console.WriteLine($"{message} at {DateTime.Now.ToShortTimeString()} from {client.Client.LocalEndPoint}");
            sw.WriteLine(message);
            sw.Flush();
        }
    }
}

