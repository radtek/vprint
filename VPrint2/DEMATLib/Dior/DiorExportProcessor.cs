﻿/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using DEMATLib.Data;

namespace DEMATLib.Dior
{
    public class DiorExportProcessor
    {
        public static event ThreadExceptionEventHandler Error;

        public static string ExportDirectory { get; set; }

        private readonly IList<HeadOffice> m_HeadOffices;

        public DiorExportProcessor(IList<HeadOffice> hoList)
        {
            m_HeadOffices = hoList;
        }

        public void Run()
        {
            try
            {
                if (m_HeadOffices == null)
                    throw new ArgumentNullException("headOffices");

                if (string.IsNullOrWhiteSpace(ExportDirectory))
                    throw new ArgumentException("ExportDirectory");

                if (!Directory.Exists(ExportDirectory))
                    Directory.CreateDirectory(ExportDirectory);

                foreach (var ho in m_HeadOffices)
                {
                    string name = DiorDataAccess.SelectTradingName(ho.IsoId, ho.HoId);

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        FireError(new Exception(string.Format("Cannot find trading name for iso: {0} ho: {1}", ho.IsoId, ho.HoId)));
                        ho.Name = "NA";
                    }
                    else if (name.IndexOf("dior", StringComparison.InvariantCultureIgnoreCase) != -1)
                        ho.Name = "DIOR";
                    else if (name.IndexOf("chanel", StringComparison.InvariantCultureIgnoreCase) != -1)
                        ho.Name = "CHANEL";
                    else if (name.IndexOf("printemps", StringComparison.InvariantCultureIgnoreCase) != -1)
                        ho.Name = "PRINTEMPS";
                    else
                        ho.Name = name.Replace(' ', '_');
                }

                foreach (var ho in m_HeadOffices)
                {
                    var retailers = DiorDataAccess.SelectAllDiorRetailes(ho.IsoId, ho.HoId);

                    foreach (var br in retailers)
                    {
                        try
                        {
                            var b = new DiorXmlBuilder();
                            b.AddHeader((DiorXmlBuilder.VoucherHeader)br);

                            var vouchersInVoucherTable = DiorDataAccess.SelectVouchersPerRetailer(br.IsoId, br.BrId);
                            var vouchersInCacheTable = DiorObjDataAccess.SelectVouchersPerRetailer(br.IsoId, br.BrId);

                            foreach (var v1 in vouchersInVoucherTable)
                            {
                                Debug.Assert(v1 != null);

                                var v2 = vouchersInCacheTable.FirstOrDefault(_v => _v.IsoId == v1.IsoId && _v.VId == v1.VId);

                                if (v2 == null || !v1.Equals(v2))
                                {
                                    //Not exist or changed
                                    try
                                    {
                                        using (var tran = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {
                                            DiorObjDataAccess.DeleteVoucher(v1);
                                            DiorObjDataAccess.InsertVoucher(v1);
                                            tran.Complete();
                                            b.AddStatus((DiorXmlBuilder.VoucherStatus)v1);
                                        }
                                    }
                                    catch
                                    {
                                        //if (Error != null)
                                        //    Error(this, new ThreadExceptionEventArgs(ex));
                                    }
                                }
                            }

                            if (!b.IsEmpty)
                            {
                                b.Close();

                                var xml = b.ToString();
                                string fileName = string.Format("{0}_Export_{1}_{2}_{3:yyyy-MM-dd}.xml", ho.Name, br.BrId, br.IsoId, DateTime.Today);
                                string path = Path.Combine(ExportDirectory, fileName);
                                File.WriteAllText(path, xml, Encoding.UTF8);
                            }
                        }
                        finally
                        {
                            Thread.Yield();
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                FireError(ex2);
            }
        }

        private void FireError(Exception ex2)
        {
            if (Error != null)
                Error(this, new ThreadExceptionEventArgs(ex2));
        }
    }
}
