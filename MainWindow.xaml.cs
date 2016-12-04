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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ServerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            Application.Current.Exit += Current_Exit;
        }

        UdpClient client;
        HttpListener httpListener;
        List<Player> playerList = new List<Player>();

        DispatcherTimer sendClock = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 16) };

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            client.Close();
            httpListener.Close();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            client = new UdpClient(new IPEndPoint(IPAddress.Any, 810));

            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://*:810/");

            ListenMessage();
            HTTPServer();

            sendClock.Tick += SendClock_Tick;
            sendClock.Start();
        }

        private void SendClock_Tick(object sender, EventArgs e)
        {
            foreach(Player p in playerList)
            {
                string data = null;
                foreach(Player otherPlayers in playerList)
                {
                    string x = otherPlayers.X >= 0.0 ? otherPlayers.X.ToString("f3") : otherPlayers.X.ToString("f2");
                    string y = otherPlayers.Y >= 0.0 ? otherPlayers.Y.ToString("f3") : otherPlayers.Y.ToString("f2");

                    data += "X" + x + "Y" + y;
                }
                Send(p.ipAddress, data);
            }
        }

        public async void HTTPServer()
        {
            httpListener.Start();

            while (true)
            {
                var ctx = await httpListener.GetContextAsync();
                ctx.Response.ContentType = "text/plain; charset=UTF-8";
                using (var writer = new StreamWriter(ctx.Response.OutputStream))
                {
                    writer.Write("I'm Reiji. Hello...");
                }
            }
        }

        public async void ListenMessage()
        {
            while (true)
            {
                // データ受信待機
                var result = await client.ReceiveAsync();

                // 受信したデータを変換
                var data = Encoding.UTF8.GetString(result.Buffer);

                // 受信データを返信
                //var returnMessage = Encoding.UTF8.GetBytes(data + "...recieved!" + " yourPort:" + result.RemoteEndPoint.Port);
                //await client.SendAsync(returnMessage, returnMessage.Length, result.RemoteEndPoint);


                // Receive イベント を実行
                OnRecieve(result.RemoteEndPoint, data);
            }
        }

        private void OnRecieve(IPEndPoint remote, string data)
        {
            //ConsoleTb.Text += data + "\n";

            if (data == "Goodbye!")
            {
                Player removePlayer = null;
                foreach(Player p in playerList)
                {
                    if (p.Equals(remote))
                    {
                        removePlayer = p;
                        break;
                    }
                }
                if (removePlayer != null)
                    playerList.Remove(removePlayer);

                Send(remote, "SeeYou!");
            }
            else if(data.Length == 12)
            {
                bool exist = false;
                foreach (Player p in playerList)
                {
                    if (p.Equals(remote))
                    {
                        p.SetData(data);
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    Player newPlayer = new Player(remote);
                    newPlayer.SetData(data);
                    playerList.Add(newPlayer);
                }
            }
        }

        private async void Send(IPEndPoint remote, string data)
        {
            var command = Encoding.UTF8.GetBytes(data);
            await client.SendAsync(command, command.Length, remote);
        }
    }

    public class Player
    {
        public Player(IPEndPoint remote)
        {
            ipAddress = remote;
        }

        public IPEndPoint ipAddress;

        public double X;
        public double Y;

        public bool Equals(IPEndPoint p)
        {
            return p.Address.Equals(ipAddress.Address) && p.Port == ipAddress.Port;
        }

        public void SetData(string data)
        {
            if (data.Length != 12 && data[0] != 'X')
                return;

            string x = data[1].ToString() + data[2].ToString() + data[3].ToString() + data[4].ToString() + data[5].ToString();
            string y = data[7].ToString() + data[8].ToString() + data[9].ToString() + data[10].ToString() + data[11].ToString();

            double tempX, tempY;
            if (double.TryParse(x, out tempX))
                X = tempX;
            if (double.TryParse(y, out tempY))
                Y = tempY;
        }
    }
}
