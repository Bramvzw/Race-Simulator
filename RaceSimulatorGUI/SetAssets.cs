using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RaceSimulatorGUI
{
    public static class SetAssets
    {
        private static Dictionary<string, Bitmap> Image;

        public static void Initialise()
        {
            Image = new Dictionary<string, Bitmap>();
        }

        

        public static Bitmap CreateEmptyBitmap(int width, int height)
        {
            Bitmap returnValue;
            if (Image.ContainsKey("empty")) return (Bitmap)LoadImg("empty").Clone();
            returnValue = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(returnValue);
            SolidBrush solidBrush = new SolidBrush(System.Drawing.Color.Green);
            graphics.FillRectangle(solidBrush, 0, 0, width, height);
            Image.Add("empty", returnValue);
            return (Bitmap)returnValue.Clone();
        }

        public static Dictionary<string, Bitmap> GetImg()
        {
            return Image;
        }


       
        public static Bitmap LoadImg(string imgUrl)
        {
            if (Image.ContainsKey(imgUrl))
            {
                return Image[imgUrl];
            }
            Bitmap image = new Bitmap(imgUrl);
            Image.Add(imgUrl, image);
            return image;
        }

        public static void ClearCache()
        {
            Image?.Clear();
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("Bitmap doesn't exist");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
