﻿/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using VPrint.Common.Pdf;
using VPrinting.Common;

namespace VPrinting.ScaningProcessors
{
    internal class PDFFileHelper
    {
        public FileInfo Run(FileInfo info, StateManager.VoucherItem item)
        {
            string fullFilePath = null;

            var pdf = new PdfAManager();

            int count = 0;

            foreach (Bitmap bmp in pdf.ExtractImagesFromPDF(info.FullName))
            {
                if (count++ == 0)
                {
                    fullFilePath = Path.ChangeExtension(info.FullName, ".jpg");
                    Global.IgnoreList.Add(fullFilePath);
                    bmp.Save(fullFilePath, ImageFormat.Jpeg);
                }
                else
                {
                    var path = Path.ChangeExtension(info.FullName, ".jpg");
                    path = path.ChangeFilePath((name) => name.Replace(".", string.Concat("_ ", count, ".")));
                    Global.IgnoreList.Add(path);
                    bmp.Crop2().Save(path, ImageFormat.Jpeg);
                    item.FileInfoList.Add(new FileInfo(path)); // Scanned Image
                }
                bmp.DisposeSf();
            }

            info.DeleteSafe();

            return new FileInfo(fullFilePath);
        }

        //var resolutions = new int[] { 96, 200, 300, 600, 1200 };
        public FileInfo Run2(FileInfo info, StateManager.VoucherItem item, int dpi = 300)
        {
            var images = info.DrawToImage(dpi);
            item.FileInfoList.AddRange(images);
            return item.FileInfoList.FirstOrDefault();
        }        
    }
}
