using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

ChatServer chatserver = new ChatServer();
chatserver.Start();
class ChatServer
{
    const short port = 4040;
    const string JOIN_CMD = "$<join>";
    HashSet<IPEndPoint> members = new HashSet<IPEndPoint>();
    UdpClient server = new UdpClient(port);
    IPEndPoint clientIPEndPoint = null;
    private void AddMember(IPEndPoint member)
    {
        members.Add(member);
        Console.WriteLine("Member was added!!");
        Console.WriteLine($@" {clientIPEndPoint} Member was added!!");
    }
    private void SendToAll(byte[] data)
    {
        foreach (IPEndPoint member in members)
        {
            server.SendAsync(data, data.Length, member);
        }
    }
    public void Start()
    {
        while (true)
        {
            byte[] data = server.Receive(ref clientIPEndPoint);
            string message = Encoding.Unicode.GetString(data);
            Console.WriteLine($"{message} at {DateTime.Now.ToShortTimeString()} from {clientIPEndPoint}");
            switch (message)
            {
                case JOIN_CMD:
                    AddMember(clientIPEndPoint);
                    break;
                default:
                    SendToAll(data);
                    break;
            }
        }
    }
}

