using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using OpenCvSharp.Extensions;
using OpenCvSharp;

namespace OpenCV_TCP
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket =
                new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Connecting..");
            EndPoint serverEP = new IPEndPoint(IPAddress.Parse("192.168.111.131"), 9999);

            socket.Connect(serverEP);

            Mat Img = RecvImage(socket);

            Cv2.ImShow("From Server", Img);

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();

            socket.Close();
        }

        static private int RecvInt(Socket sock)
        {
            int RecvSize = 0;
            List<byte> lstData = new List<byte>();

            while (true)
            {
                byte[] data = new byte[1];
                RecvSize = sock.Receive(data);
                if (data[0] == 0)
                    break;
                lstData.Add(data[0]);
            }

            return int.Parse(UTF8Encoding.UTF8.GetString(lstData.ToArray()));
        }

        static private Mat RecvImage(Socket sock)
        {
            List<byte> lstData = new List<byte>();
            int ImgSize = RecvInt(sock);


            while (ImgSize > lstData.Count())
            {
                byte[] bData = new byte[ImgSize - lstData.Count()];

                int RecvSize = sock.Receive(bData);
                Array.Resize(ref bData, RecvSize);
                lstData.AddRange(bData);
            }

            Mat Img = Mat.ImDecode(lstData.ToArray());

            return Img;
        }
    }
}
