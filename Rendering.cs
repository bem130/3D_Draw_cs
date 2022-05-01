using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace _3D_Draw_cs
{
    internal class Rendering
    {
        DImage dImage;
        public Rendering()
        {
            dImage = new DImage();
            dImage.makenew("VGA");


            for (int i=0; i < dImage.frame_y;i++)
            {
                for (int j=0;j<dImage.frame_x;j++)
                {
                    dImage.setpixcel(j, i, 255, 255, 0, 0);
                }
            }

        }
        public Bitmap GetImage()
        {
            return dImage.frame;
        }
    }

    class Vector3D
    {
        public double[] make(double x, double y, double z)
        {
            return new double[3] { x, y, z };
        }
        public double[] add(double[] v1,double[] v2)
        {
            return new double[3] { v1[0]+v2[0], v1[1]+v2[1], v1[2]+v2[2] };
        }
        public double[] scale(double[] v1, double s)
        {
            return new double[3] { v1[0]*s, v1[1]*s, v1[2]*s };
        }
        public double[] sub(double[] v1, double[] v2)
        {
            return new double[3] { v1[0]-v2[0], v1[1]-v2[1], v1[2]-v2[2] };
        }
        public double[] rotate(double[] v1,double rx,double ry,double rz)
        {
            double x1 = v1[0]; double y1 = v1[1]; double z1 = v1[2];
            double y2 = y1*Math.Cos(rx)-z1*Math.Sin(rx);
            z1 = y1*Math.Sin(rx)+z1*Math.Cos(rx);
            double x2 = x1*Math.Cos(ry)+z1*Math.Sin(ry);
            double z3 = -x1*Math.Sin(ry)+z1*Math.Cos(ry);
            double x3 = x2*Math.Cos(rz)-y2*Math.Sin(rz);
            double y3 = x2*Math.Sin(rz)+y2*Math.Cos(rz);
            return new double[3] { x3, y3,z3 };
        }
        public double[] rotate_x(double[] v1, double rx)
        {
            double x1 = v1[0]; double y1 = v1[1]; double z1 = v1[2];
            double y2 = y1*Math.Cos(rx)-z1*Math.Sin(rx);
            z1 = y1*Math.Sin(rx)+z1*Math.Cos(rx);
            return new double[3] { x1, y2, z1 };
        }
        public double[] rotate_z(double[] v1, double rz)
        {
            double x1 = v1[0]; double y1 = v1[1]; double z1 = v1[2];
            double x3 = x1*Math.Cos(rz)-y1*Math.Sin(rz);
            double y3 = x1*Math.Sin(rz)+y1*Math.Cos(rz);
            return new double[3] { x3, y3, z1 };
        }
        public double length(double[] v1, double[] v2)
        {
            return Math.Sqrt(square(v1[0]-v2[0])+square(v1[1]-v2[1])+square(v1[2]-v2[2]));
        }
        public double length(double[] v1)
        {
            return Math.Sqrt(square(v1[0])+square(v1[1])+square(v1[2]));
        }
        public double squared_length(double[] v1, double[] v2)
        {
            return square(v1[0]-v2[0])+square(v1[1]-v2[1])+square(v1[2]-v2[2]);
        }
        public double square(double x)
        {
            return x*x;
        }
        public double[] cross_product(double[] v1, double[] v2)
        {
            return new double[3] { v1[1]*v2[2]-v1[2]*v2[1], v1[2]*v2[0]-v1[0]*v2[2], v1[0]*v2[1]-v1[1]*v2[0] };
        }
        public double inner_product(double[] v1, double[] v2)
        { 
            return v1[0]*v2[0]+v1[1]*v2[1]+v1[2]*v2[2];
        }
        public double[] normalize(double[] v1)
        {
            return scale(v1,1/length(v1));
        }
    }

    class Vector2D
    {
        public double[] make(double x, double y)
        {
            return new double[2] { x, y };
        }
        public double[] add(double[] v1, double[] v2)
        {
            return new double[2] { v1[0]+v2[0], v1[1]+v2[1] };
        }
        public double[] sub(double[] v1, double[] v2)
        {
            return new double[2] { v1[0]-v2[0], v1[1]-v2[1] };
        }
        public double[] rotate(double[] v1, double r)
        {
            double x1 = v1[0]; double y1 = v1[1];
            var x = x1*Math.Cos(r)-y1*Math.Sin(r);
            var y = x1*Math.Sin(r)+y1*Math.Cos(r);
            return new double[2] {x,y};
        }
        public double square(double x)
        {
            return x*x;
        }
    }

    class DImage
    {
        public Bitmap frame;
        public Dictionary<string, int[]> display;
        public int frame_x;
        public int frame_y;
        public DImage()
        {
            display = new Dictionary<string, int[]>()
            {
                {"VGA", new int[] { 640,480,4,3 }},
            };
            frame_x = 100;
            frame_y = 100;
            makenew(100,100);
        }
        public Bitmap makenew(int x, int y)
        {
            frame_x = x+1;
            frame_y = y+1;
            frame = new Bitmap(x+1, y+1);
            frame = fillRectangle(0, 0, x+1, y+1, 255, 255, 255, 255);
            return frame;
        }
        public Bitmap makenew(string name)
        {
            int[] res = display[name];
            frame_x = res[0]+1;
            frame_y = res[1]+1;
            frame = new Bitmap(res[0]+1, res[1]+1);
            frame = fillRectangle(0, 0, res[0]+1, res[1]+1, 255, 25, 255, 255);
            return frame;
        }
        public void saveimage(string path)
        {
            try
            {
                frame.Save(path);
            }
            catch (Exception e) { Debug.Print(e.ToString()); };
            return;
        }
        public Bitmap setpixcel(int x, int y, int a, int r, int g, int b)
        {
            frame.SetPixel(x, y, System.Drawing.Color.FromArgb(a, r, g, b));
            return frame;
        }
        public Bitmap fillRectangle(int x1, int y1, int x2, int y2, int a, int r, int g, int b)
        {
            Graphics tmpedit = Graphics.FromImage(frame);
            tmpedit.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(a, r, g, b)), x1, y1, x2, y2);
            tmpedit.Dispose();
            return frame;
        }
    }
}
