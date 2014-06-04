﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ReceivingServiceLib
{
    public static class DrawingEx
    {
        /// <summary>
        /// Bitmap bitmap = (Bitmap)Image.FromFile(file);
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static List<Image> GetAllPages(this Bitmap bitmap, ImageFormat useImageFormat)
        {
            var images = new List<Image>();
            int count = bitmap.GetFrameCount(FrameDimension.Page);

            for (int idx = 0; idx < count; idx++)
            {
                bitmap.SelectActiveFrame(FrameDimension.Page, idx);

                using (MemoryStream byteStream = new MemoryStream())
                {
                    bitmap.Save(byteStream, useImageFormat);
                    images.Add(Image.FromStream(byteStream));
                }
            }
            return images;
        }
    }
}
