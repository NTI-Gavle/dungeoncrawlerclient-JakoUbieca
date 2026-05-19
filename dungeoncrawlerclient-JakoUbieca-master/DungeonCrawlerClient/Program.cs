using System; 
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawlerClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 54321);
            TcpClient tcpClient = new TcpClient();

            try
            {
                tcpClient.Connect(iPEndPoint);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not connect to server.");
                return;
            }

            NetworkStream stream = tcpClient.GetStream();

            Task.Run(() =>
            {
                byte[] readBuffer = new byte[1024];
                int bytesRead;
                try
                {
                    while ((bytesRead = stream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    {
                        string message = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
                        Console.Write(message);
                    }
                }
                catch { }

                Console.WriteLine("\n\n[Disconnected from server.]");
                Environment.Exit(0);
            });
            try
            {
                while (tcpClient.Connected)
                {
                    string command = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(command)) continue;

                    byte[] writeBytes = Encoding.UTF8.GetBytes(command);
                    stream.Write(writeBytes, 0, writeBytes.Length);
                }
            }
            catch
            {

            }

            tcpClient.Close();
        }
    }
}