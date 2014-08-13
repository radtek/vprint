﻿//#define SPAIN

using System;
using System.Collections.Concurrent;
using System.Web;

namespace FintraxPTFImages.Common
{
    public class BarcodeDecoder
    {
        private static ConcurrentBag<BarcodeConfig> ms_bag;

        public BarcodeData Match(string barcode)
        {
            ConcurrentBag<BarcodeConfig> b;

            if (HttpContext.Current != null)
                b = (ConcurrentBag<BarcodeConfig>)HttpContext.Current.Application[Strings.LIST_OF_BARCODECONFIGS];
            else
                b = ms_bag;

            BarcodeData data = null;

            foreach (var item in b)
                if (item.ParseBarcode(barcode, ref data))
                    return data;
            return data;
        }

        public void Test()
        {
            ConcurrentBag<BarcodeConfig> b;

            if (HttpContext.Current != null)
                b = (ConcurrentBag<BarcodeConfig>)HttpContext.Current.Application[Strings.LIST_OF_BARCODECONFIGS];
            else
                b = ms_bag;

            foreach (var item in b)
                item.Test();
        }

        public static void Run()
        {
            ms_bag = new ConcurrentBag<BarcodeConfig>()
            {
                new BarcodeConfig()
                {
                    Name = "CCC-SS-RRRRRR-VVVVVVVVV",
                    Length = 20,
                    //iso, ty, br, voucher
                    Template = "{0:000}{1:00}{2:000000}{3:00000000}",
                    Sample = "012 01 012345 012345678",
                    CountryID = new Tuple<int,int>(0, 3),
                    BuzType = new Tuple<int,int>(3, 2),
                    RetailerID = new Tuple<int,int>(5, 6),
                    VoucherID = new Tuple<int,int>(11, 9),
                },
                new BarcodeConfig()
                {
                    Name = "CCC-RRRRRR-VVVVVVVVV",
                    Length = 18,
                    //iso, ty, br, voucher
                    Template = "{0:000}{2:000000}{3:00000000}",
                    Sample = "012 012345 012345678",
                    CountryID = new Tuple<int,int>(0, 3),
                    RetailerID = new Tuple<int,int>(3, 6),
                    VoucherID = new Tuple<int,int>(9, 9),
                },
                new BarcodeConfig()
                {
                    Name = "VVVVVVVVV-CCC-SS",
                    Length = 14,
                    //iso, ty, br, voucher
                    Template = "{3:000000000}{0:000}{1:00}",
                    Sample = "012345678 012 01",
                    VoucherID = new Tuple<int,int>(0, 9),
                    CountryID = new Tuple<int,int>(9, 3),
                    BuzType = new Tuple<int,int>(12,2),
                },
                new BarcodeConfig()
                {
                    Name = "CCC-SS-RRRRRR-VVVVVVVVV-AAAAAAAAAAA",
                    Length = 31,
                    //iso, ty, br, voucher
                    Template = "{0:000}{2:000000}{3:00000000}",
                    Sample = "012 01 012345 012345678 01234567890",
                    CountryID = new Tuple<int,int>(0, 3),
                    BuzType = new Tuple<int,int>(3, 2),
                    RetailerID = new Tuple<int,int>(5, 6),
                    VoucherID = new Tuple<int,int>(11, 9),
                },
            };

            if (HttpContext.Current != null)
                HttpContext.Current.Application.Add(Strings.LIST_OF_BARCODECONFIGS, ms_bag);
        }
    }
}