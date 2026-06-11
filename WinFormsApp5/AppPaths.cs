using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WinFormsApp5
{
    internal static class AppPaths
    {
        public static string ImagesFolder =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        public static void EnsureImagesFolder()
        {
            Directory.CreateDirectory(ImagesFolder);

            string picture = Path.Combine(ImagesFolder, "picture.png");
            if (File.Exists(picture))
            {
                return;
            }

            using (Bitmap bmp = new Bitmap(200, 150))
            using (Graphics g = Graphics.FromImage(bmp))
            using (Font font = new Font("Segoe UI", 12))
            {
                g.Clear(Color.LightGray);
                g.DrawString("Нет фото", font, Brushes.DimGray, 55, 60);
                bmp.Save(picture, ImageFormat.Png);
            }
        }
    }
}
