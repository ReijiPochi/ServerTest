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
            Closing += MainWindow_Closing;

            
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //var httpListener = new HttpListener();
            //httpListener.Prefixes.Add("http://*:810/");
            //httpListener.Start();

            //while (true)
            //{
            //    var ctx = httpListener.GetContext();
            //    ctx.Response.ContentType = "text/plain; charset=UTF-8";
            //    using (var writer = new StreamWriter(ctx.Response.OutputStream))
            //    {
            //        writer.Write("I'm Reiji. Hello...");
            //    }
            //}

            ListenMessage();
        }

        public async void ListenMessage()
        {
            // 接続ソケットの準備
            var local = new IPEndPoint(IPAddress.Any, 810);
            var client = new UdpClient(local);

            while (true)
            {
                // データ受信待機
                var result = await client.ReceiveAsync();

                // 受信したデータを変換
                var data = Encoding.UTF8.GetString(result.Buffer);

                // Receive イベント を実行
                OnRecieve(data);
            }

            //// 接続ソケットの準備
            //var local = new IPEndPoint(IPAddress.Any, 810);
            //var remote = new IPEndPoint(IPAddress.Any, 810);
            //var client = new UdpClient(local);

            //while (true)
            //{
            //    // データ受信待機
            //    var buffer = client.Receive(ref remote);

            //    // 受信したデータを変換
            //    var data = Encoding.UTF8.GetString(buffer);

            //    // Receive イベント を実行
            //    this.OnRecieve(data);
            //}

        }

        private void OnRecieve(string data)
        {
            ConsoleTb.Text += data;
        }
    }
}
