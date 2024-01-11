using System.Net;
using System.Net.Sockets;

namespace Server {

    internal class Program
    {       

        static async Task Main() => await new Program().Run();

        private IList<TcpClient> clients = new List<TcpClient>();

        private async Task Run()
        {
            string hostname = Dns.GetHostName();
            string miIP = Dns.GetHostByName(hostname).AddressList[3].ToString();
            TcpListener listener = new TcpListener(IPAddress.Parse(miIP), 2024);
            listener.Start();

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                lock (clients)
                    clients.Add(client);
                ListenToClient(client);
            }
        }

        private async void ListenToClient(TcpClient from)
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                int read;
                try
                {
                    read = await from.GetStream().ReadAsync(buffer, 0, buffer.Length);
                }
                catch (Exception)
                {
                    break;
                }

                IReadOnlyList<TcpClient> copy;
                lock (clients)
                    copy = clients.ToList();

                foreach (TcpClient to in copy)
                    await to.GetStream().WriteAsync(buffer, 0, read);
            }

            from.Dispose();
            lock (clients)
                clients.Remove(from);
        }
    }

}