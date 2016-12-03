using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        System.Net.Sockets.UdpClient udp;

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //UdpClientを閉じる
            udp.Close();
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

            //バインドするローカルIPとポート番号
            string localIpString = "192.168.3.30";
            IPAddress localAddress = IPAddress.Parse(localIpString);
            int localPort = 810;


            //UdpClientを作成し、ローカルエンドポイントにバインドする
            IPEndPoint localEP = new IPEndPoint(localAddress, localPort);
            udp = new System.Net.Sockets.UdpClient(localEP);


            while (true)
            {
                //データを受信する
                IPEndPoint remoteEP = null;
                byte[] rcvBytes = udp.Receive(ref remoteEP);

                //データを文字列に変換する
                string rcvMsg = Encoding.UTF8.GetString(rcvBytes);

                //受信したデータと送信者の情報を表示する
                Console.WriteLine("受信したデータ:{0}", rcvMsg);
                Console.WriteLine("送信元アドレス:{0}/ポート番号:{1}", remoteEP.Address, remoteEP.Port);
            }
        }
    }
}
