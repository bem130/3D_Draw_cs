﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Drawing;
using System.Windows.Shapes;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace _3D_Draw_cs
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        Rendering render;
        Stopwatch sw;
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public MainWindow()
        {
            sw = new Stopwatch();
            InitializeComponent();
            render = new Rendering();
            renderloop();
            DisplayResolution.ItemsSource = render.getDisplayRes();
            DisplayResolution.SelectedValue = "VGA";
        }
        public void show()
        {
            Bitmap showimage = render.GetImage();
            IntPtr hbitmap = showimage.GetHbitmap();
            ImageView.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hbitmap);
            showimage.Dispose();
        }
        async public void renderloop()
        {
            await Task.Delay(100);
            sw.Start();
            while (true)
            {
                render.render();
                show();
                sw.Stop();
                Time.Text = (((double)sw.ElapsedMilliseconds)/1000).ToString()+" "+(1/(((double)sw.ElapsedMilliseconds)/1000)).ToString();
                sw.Reset();
                sw.Start();
                await Task.Delay(10);
            }
        }
        public void changeDisplayRes()
        {
            Debug.Print("display setting changed");
            Debug.Print(DisplayResolution.SelectedValue.ToString());
            render.setDisplay(DisplayResolution.SelectedValue.ToString());
        }
        public void changeDisplayRes(object sender, SelectionChangedEventArgs e){ changeDisplayRes();}
    }

}
