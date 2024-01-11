using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NP_hw_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileModel fileModel = new();
        private TcpClient server;
        public MainWindow()
        {
            InitializeComponent();

            string hostname = Dns.GetHostName();

            server = new TcpClient(Dns.GetHostByName(hostname).AddressList[3].ToString(), 2024);

            //SendFileContent(server, fileModel.FileName);

            RecieveFile(server, fileModel.FileName);

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string path;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                DialogResult result = openFileDialog.ShowDialog();
                filePath.Text = openFileDialog.FileName;
            }

        }

        private async void Button_Click_Send(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(fileModel.UserName))
            {
                System.Windows.MessageBox.Show("Введите имя пользователя, чтобы общаться: ");
                return;
            }
            if (string.IsNullOrEmpty(fileModel.FileName.ToString()))
            {
                System.Windows.MessageBox.Show("Выберите файл: ");
                return;
            }
            SendFileContent(server, fileModel.FileName);

            string concat = $"{fileModel.UserName}: {fileModel.FileName}";
            byte[] bytes = Encoding.UTF8.GetBytes(concat);
            await server.GetStream().WriteAsync(bytes);
        }

        private static async Task SendFileContent(TcpClient client, Stream file)
        {
            int length = (int)file.Length;
            await SendInt32(client, length);

            byte[] buffer = new byte[1024];
            int pos = 0;
            while (pos < length)
            {
                int read = await file.ReadAsync(buffer, 0, Math.Min(buffer.Length, length - pos));
                await client.GetStream().WriteAsync(buffer, 0, read);
                pos += read;
            }

        }

        private static async Task RecieveFile(TcpClient client, Stream file)
        {
            int length = await RecieveInt32(client);
        }

        private static async Task SendInt32(TcpClient client, int number) =>
            await client.GetStream().WriteAsync(BitConverter.GetBytes(number));

        private static async Task<int> RecieveInt32(TcpClient client)
        {
            byte[] lengthBuffer = await RecieveFixed(client, sizeof(int));
            return BitConverter.ToInt32(lengthBuffer);
        }

        private static async Task<byte[]> RecieveFixed(TcpClient client, int length)
        {
            byte[] buffer = new byte[length];
            await client.GetStream().ReadAsync(buffer, 0, length);
            return buffer;
        }


    }
}
