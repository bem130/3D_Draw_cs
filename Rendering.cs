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
        public Dictionary<string, int[]> display;
        public Rendering()
        {
            dImage = new DImage();
            dImage.setpixcel(10, 10, 255, 255, 0, 0);
            dImage.setpixcel(0, 10, 255, 255, 0, 0);
            dImage.setpixcel(100, 10, 255, 255, 0, 0);

            display = new Dictionary<string, int[]>()
            {
                {"VGA", new int[] { 640,480,4,3 }},
            };
        }
        public Bitmap GetImage()
        {
            return dImage.frame;
        }
    }
    class DImage
    {
        public Bitmap frame;
        public DImage()
        {
            makenew(100,100);
        }
        public Bitmap makenew(int x, int y)
        {
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
}
