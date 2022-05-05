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
        Vector2D vector2d;
        Vector3D vector3d;
        Camera cam;
        public Rendering()
        {
            cam = new Camera() {};
            vector2d = new Vector2D();
            vector3d = new Vector3D();
            cam.setDisplay("VGA");
            Debug.Print(cam.framex.ToString()+" "+cam.framey.ToString());

            List<Polygon> obj = new List<Polygon>()
            {
                new Polygon() { pos = new double[][] { new double[] {0,0,0 }, new double[] {1,1,1 }, new double[] {2,-2,2 }, } },
            };

            dImage = new DImage();
            dImage.makenew(cam.framex,cam.framey);

            //for (int i = 0; i<cam.framey; i++)
            //{
            //    for (int j = 0; j<cam.framex; j+=2)
            //    {
            //        dImage.setpixcel(j, i, 255, 255, 0, 0);
            //    }
            //}

            List<Polygon> polygons = obj;
            double[] vl = vector3d.normalize(vector3d.make(1,15,50));

            for (int i=0; i<polygons.Count; i++)
            {
                Polygon t = polygons[i];
                int[] p1 = pos3t2d(t.pos[0]);
                int[] p2 = pos3t2d(t.pos[1]);
                int[] p3 = pos3t2d(t.pos[2]);
                

                double[] v12 = vector3d.sub(t.pos[1],t.pos[0]);
                double[] v13 = vector3d.sub(t.pos[2], t.pos[0]);
                double[] normal = vector3d.normalize(vector3d.cross_product(v12, v13));
                double angl = vector3d.inner_product(vl, normal);
                double light = Math.Max(angl, angl*0.1)*0.9+0.3;

                int xmax = Math.Min(Maxi3(p1[0], p2[0], p3[0]), cam.framex);
                int xmin = Math.Max(Mini3(p1[0], p2[0], p3[0]), 0);
                int ymax = Math.Min(Maxi3(p1[1], p2[1], p3[1]), cam.framey);
                int ymin = Math.Max(Mini3(p1[1], p2[1], p3[1]), 0);

                int[][] dp = new int[][] { p1, p2, p3 };
                for (int iy = ymin; iy < ymax; iy++)
                {
                    for (int ix = xmin; ix < xmax; ix++)
                    {
                        if (inclusion(new int[] {ix, iy}, dp))
                        {
                            dImage.setpixcel(ix, iy, (int)(255), (int)(255*light), (int)(0*light), (int)(0*light));
                        }
                    }
                }
            }

        }
        public int Maxi3(int p1,int p2,int p3)
        {
            int r = p1;
            if (p2>r) { r = p2; }
            if (p3>r) { r = p3; }
            return r;
        }
        public int Mini3(int p1, int p2, int p3)
        {
            int r = p1;
            if (p2<r) { r = p2; }
            if (p3<r) { r = p3; }
            return r;
        }
        public Bitmap GetImage()
        {
            return dImage.frame;
        }
        public int[] pos3t2d(double[] v)
        {
            v = vector3d.sub(v, new double[3] { cam.posx, cam.posy, cam.posz });
            v = vector3d.rotate_z(v, cam.angh);
            v = vector3d.rotate_x(v, -cam.angv);
            double leng = Math.Abs(14/v[1]);
            double scale = cam.framex/10;
            return vector2d.toInt(vector2d.add(vector2d.scale(new double[2] {v[0],-v[2]},leng*scale),new double[2] {cam.framex/2,cam.framey/2}));
        }
        public bool inclusion(int[] v1,int[][] v2)
        {
            int[] a = vector2d.sub(v2[0], v1);
            int[] b = vector2d.sub(v2[1], v1);
            int[] c = vector2d.sub(v2[2], v1);
            double ab = a[0]*b[1]-a[1]*b[0];
            double bc = b[0]*c[1]-b[1]*c[0];
            double ca = c[0]*a[1]-c[1]*a[0];
            return ab<=0&&bc<=0&&ca<=0;
        }
        public double[] gcot(double[][] t) // 三角形の代表座標(3点の平均)
        {
            return new double[3] { (t[0][0]+t[1][0]+t[2][0])/3, (t[0][1]+t[1][1]+t[2][1])/3, (t[0][2]+t[1][2]+t[2][2])/3 };
        }
    }

    class Vector3D
    {
        public int[] toInt(double[] v)
        {
            return new int[3] { (int)v[0], (int)v[1], (int)v[2] };
        }
        public double[] make(double x, double y, double z)
        {
            return new double[3] { x, y, z };
        }
        public double[] add(double[] v1,double[] v2)
        {
            return new double[3] { v1[0]+v2[0], v1[1]+v2[1], v1[2]+v2[2] };
        }
        public double[] sub(double[] v1, double[] v2)
        {
            return new double[3] { v1[0]-v2[0], v1[1]-v2[1], v1[2]-v2[2] };
        }
        public double[] scale(double[] v1, double s)
        {
            return new double[3] { v1[0]*s, v1[1]*s, v1[2]*s };
        }
        public double[] rotate(double[] v1,double rx,double ry,double rz)
        {
            rx *= Math.PI/180;
            ry *= Math.PI/180;
            rz *= Math.PI/180;
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
            rx *= Math.PI/180;
            double x1 = v1[0]; double y1 = v1[1]; double z1 = v1[2];
            double y2 = y1*Math.Cos(rx)-z1*Math.Sin(rx);
            z1 = y1*Math.Sin(rx)+z1*Math.Cos(rx);
            return new double[3] { x1, y2, z1 };
        }
        public double[] rotate_z(double[] v1, double rz)
        {
            rz *= Math.PI/180;
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
        public int[] toInt(double[] v)
        {
            return new int[2] { (int)v[0], (int)v[1] };
        }
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
        public int[] sub(int[] v1, int[] v2)
        {
            return new int[2] { v1[0]-v2[0], v1[1]-v2[1] };
        }
        public double[] scale(double[] v1, double s)
        {
            return new double[2] { v1[0]*s, v1[1]*s };
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
            frame_x = 100;
            frame_y = 100;
            makenew(100,100);
            return;
        }
        public Bitmap makenew(int x, int y)
        {
            frame_x = x+1;
            frame_y = y+1;
            frame = new Bitmap(x+1, y+1);
            frame = fillRectangle(0, 0, x+1, y+1, 255, 255, 255, 255);
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
    public class Camera
    {
        Dictionary<string, int[]> display;
        public double posx { get; set; } // position x
        public double posy { get; set; } // position y
        public double posz { get; set; } // position z
        public double angh { get; set; } // angle h
        public double angv { get; set; } // angle v
        public int framex { get; set; } // frame x
        public int framey { get; set; } // frame y
        public Camera()
        {
            display = new Dictionary<string, int[]>()
            {
                {"QQVGA", new int[] { 160,120,4,3 }},
                {"VGA", new int[] { 640,480,4,3 }},
                {"SVGA", new int[] { 800,600,4,3 }},
                {"XGA", new int[] { 1024,768,4,3 }},
                {"HD", new int[] { 1280,720,16,9 }},
                {"UXGA", new int[] { 1600,1200,4,3 }},
                {"FHD", new int[] { 1920,1080,16,9 }},
                {"4K", new int[] { 3840,2160,16,9 }},
            };
            posx = 10;
            posy = 10;
            posz = 1.7;
            angh = 225;
            angv = 0;
            return;
        }
        public void setDisplay(string name)
        {
            int[] res = display[name];

            setResolution(res[0], res[1]);
            return;

        }
        public void setResolution(int x,int y)
        {
            Debug.Print(x.ToString()+" "+y.ToString()+" a");
            if (x>0)
            {
                framex = x;
            }
            if (y>0)
            {
                framey = y;
            }
            return;
        }
    }
    public class Polygon
    {
        public double[][] pos { get; set; } // position
    }
}
