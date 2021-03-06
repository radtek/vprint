﻿/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ReceivingServiceLib.Data;
using VPrinting;

namespace ReceivingServiceLib.FileWorkers
{
    public class ImportFileWorker : FileWorkerBase
    {
        /// <summary>
        /// 4 MINUTES
        /// </summary>
        const int WAIT_BEFORE_DELETE_MINUTES = 4;
        protected static ImportFileWorker ms_instance;
        public static ImportFileWorker Default
        {
            get
            {
                if (ms_instance == null)
                    ms_instance = new ImportFileWorker();
                return ms_instance;
            }
        }
 
        protected override void WorkerThreadFunction()
        {
            Debug.Assert(Global.Strings.IsValid());

            while (true)
            {
                try
                {
                    var errorRoot = new DirectoryInfo(Global.Strings.UPLOADERRORS);
                    errorRoot.CreateIfNotExist();

                    var uploadRoot = new DirectoryInfo(Global.Strings.UPLOADROOT);
                    uploadRoot.CreateIfNotExist();

                    var dirs = uploadRoot.GetDirectories();                    

                    if (dirs.Length != 0)
                    {
                        int jobId = 0, countryId = 0, retailerId = 0, voucherId = 0, userId, locationId;
                        int? folderId, v_type;
                        string siteCode = null, barCode = null, sessionId;
                        string message = null;

                        foreach (var fromDir in dirs)
                        {
                            try
                            {
#if DEBUGGER
                                Trace.WriteLine("Trying ".concat(fromDir.Name), Strings.APPNAME);
#endif

                                if (!fromDir.Exists)
                                    continue;

                                if (ScanService.FileLocks.Contains(fromDir.Name, StringComparer.InvariantCultureIgnoreCase))
                                    continue;

                                bool isVoucher = false;

                                var fac = new ZipFileAccess();

                                string xmlPath = Path.Combine(uploadRoot.FullName, fromDir.Name, "data.xml");
                                if (File.Exists(xmlPath))
                                {
                                    isVoucher = true;

                                    fac.ReadVoucherXml(xmlPath, out jobId, out countryId, out retailerId, out voucherId, out folderId,
                                        out siteCode, out barCode, out userId, out locationId, out sessionId, out v_type);

                                    message = string.Concat("Job ID: ", jobId, Environment.NewLine, "Country ID: ",
                                        countryId, Environment.NewLine, "Retailer ID: ", retailerId, Environment.NewLine,
                                       "Voucher ID: ", voucherId, Environment.NewLine, "Folder ID: ", folderId, Environment.NewLine,
                                       "Site code: ", siteCode, Environment.NewLine,
                                       "User ID: ", userId, Environment.NewLine, "Location ID: ", locationId, Environment.NewLine,
                                       "Session ID: ", sessionId, Environment.NewLine,
                                       string.Format("Archive created: {0:G}", DateTime.Now));
                                }
                                else
                                {
                                    isVoucher = false;

                                    xmlPath = Path.Combine(uploadRoot.FullName, fromDir.Name, "cover.xml");
                                    if (File.Exists(xmlPath))
                                    {
                                        fac.ReadCoversheetXml(xmlPath, out folderId, out userId, out locationId, out sessionId);

                                        message = string.Concat("User ID: ", userId, Environment.NewLine, "Location ID: ", locationId,
                                            Environment.NewLine, "Session ID: ", sessionId, Environment.NewLine,
                                            string.Format("Archive created: {0:G}", DateTime.Now));
                                    }
                                    else
                                    {
                                        if (fromDir.CreationTime.AddMinutes(WAIT_BEFORE_DELETE_MINUTES) > DateTime.Now)
                                            continue;

                                        throw new Exception("Cannot find: " + xmlPath);
                                    }
                                }
                                                                 #if DEBUGGER
                                Trace.WriteLine("Create zip ".concat(fromDir.Name), Strings.APPNAME);
#endif
                                string zipPath = Path.Combine(uploadRoot.FullName, string.Concat(fromDir.Name, ".zip"));
                                var zipFile = new FileInfo(zipPath);
                                zipFile.DeleteSafe();

                                fac.CreateZip(zipPath, fromDir.FullName, message);

                                string binPath = Path.Combine(uploadRoot.FullName, string.Concat(fromDir.Name, ".bin"));
                                var binFile = new FileInfo(binPath);
                                zipFile.EncriptFile(binFile);
#if DEBUGGER
                                Trace.WriteLine("Read xml ".concat(fromDir.Name), Strings.APPNAME);
                                Trace.WriteLine("Sql Insert ".concat(fromDir.Name), Strings.APPNAME);
#endif
                                var fileShare = fac.CreateDirectoryHerarchy(Global.Strings.FILESERVERFOLDER, countryId, retailerId, voucherId);
                                var saveFile = fileShare.CombineFileName(binFile.Name);

                                binFile.CopyTo(saveFile);

                                if (isVoucher)
                                {
                                    VoucherDataAccess.Instance.AddVoucher(jobId, countryId, retailerId, voucherId, folderId,
                                        siteCode, barCode, locationId, userId, null, 0, sessionId, true, 2);
                                }
                                else
                                {
                                    VoucherDataAccess.Instance.AddCoversheet(folderId, locationId, userId, null, 0, sessionId, true);
                                }

#if DEBUGGER
                                Trace.WriteLine("Save to history ".concat(fromDir.Name), Strings.APPNAME);
#endif
                                var voucherDirectory = fac.CreateDirectoryHerarchy(Global.Strings.VOCUHERSFOLDER, countryId, retailerId, voucherId);
                                fromDir.CopyFiles(voucherDirectory, true);

#if DEBUGGER
                                Trace.WriteLine("Clean up ".concat(fromDir.Name), Strings.APPNAME);
#endif
                                zipFile.DeleteSafe();
                                binFile.DeleteSafe();
                                fromDir.DeleteSafe(true);
                                Array.Clear(m_Buffer50MB, 0, m_Buffer50MB.Length);
                            }
                            catch (Exception ex)
                            {
                                FireError(ex);
                                try
                                {
                                    var errDir = errorRoot.Combine(fromDir.Name);
                                    errDir.CreateIfNotExist();
                                    fromDir.MoveTo(errDir);
                                }
                                catch (Exception ex2)
                                {
                                    FireError(ex2);
                                }
                            }
                            finally
                            {                                
#if DEBUGGER
                                Trace.WriteLine("===================================", Strings.APPNAME);
#endif
                                Thread.Yield();
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(EMPTYTIMEOUT);
                    }
                }
                catch (Exception ex)
                {
                    FireError(ex);
                }
                finally
                {
                    Thread.Sleep(TIMEOUT);
                }
            }
        }
    }
}