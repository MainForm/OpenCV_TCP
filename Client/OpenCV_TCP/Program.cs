using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            EndPoint serverEP = new IPEndPoint(IPAddress.Parse("192.168.40.131"), 9999);

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
            int width = RecvInt(sock);
            int height = RecvInt(sock);

            Mat Img = new Mat(height, width, 16);

            for (int iy = 0; iy < height; iy++)
            {
                for (int ix = 0; ix < width; ix++)
                {
                    byte[] tmp = new byte[4];
                    sock.Receive(tmp, 3, 0);
                    Img.At<int>(iy, ix) = BitConverter.ToInt32(tmp, 0);
                }
            }

            return Img;
        }
    }
}
