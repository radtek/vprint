﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceivingServiceLib;
using System.IO;
using System.Drawing;
using VPrint.Common.Pdf;
using VPrinting;
using VPrinting.Pdf;
using System.Net;
using System.Diagnostics;

namespace ReceivingServiceTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class PdfTest
    {
        public PdfTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void create_pdf_sign()
        {
            string[] files = Directory.GetFiles(@"C:\Users\Rosen.rusev\Pictures\Presenter");

            PdfAManager manager = new PdfAManager();

            string fileName = "C:\\test\\SES724-320377-0013491924-5.pdf";
            string signedFileName = "C:\\test\\SES724-320377-0013491924-5_Signed.pdf";

            using (var bmp = (Bitmap)Bitmap.FromFile("C:\\test\\SES724-320377-0013491924-5.jpg"))
            {
                manager.CreatePdf(fileName, new Bitmap[] { bmp },
                    new PdfCreationInfo()
                {
                    Title = "Voucher SES724-320377-0013491924-5",
                    Subject = "Retailer 320377",
                    Author = "PTF Spain",
                    Creator = "PTF Spain"
                });
            }

            manager.SignPdfFile(
                fileName,
                signedFileName,
            new PdfSignInfo()
            {
                pfxFilePath = @"C:\PROJECTS\VPrint2\ReceivingServiceLib.Common\PTF.pfx",
                pfxKeyPass = "",
                DocPass = null,
                SignImagePath = @"C:\PROJECTS\VPrint2\ReceivingServiceLib.Common\Resources\PTFLogo.jpg",
                ReasonForSigning = "Voucher SES724-320377-0013491924-5",
                Location = "Madrid, Spain"
            });
        }

        [TestMethod]
        public void tiff_GetAllPages()
        {
            string fileName = @"C:\Users\Rosen.rusev\Pictures\Presenter\New folder (3)\826_152948_380202463.tif";
            using (Bitmap bitmap = (Bitmap)Image.FromFile(fileName))
            {
                var list = bitmap.GetAllPages(System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        const string NAME = "rosen.rusev";
        const string PASS = "RGR3245971!!";
        const string DOMAIN = "fintrax.com";

        [TestMethod]
        public void call_ReportingServer()
        {
            try
            {
                /// GOOD
                ///http://192.168.53.144/Reportserver/Pages/ReportViewer.aspx?%2fNota+Debito%2fNota+Debito+0032&rs:Command=Render&rs:format=PDF&iso_id=724&Office=167150&in_date=02/12/2013&invoicenumber=42538
                ///

                string serverUrl0 = "http://192.168.53.144/";
                string serverUrl = @"http://192.168.53.144/Reportserver/Pages/ReportViewer.aspx?%2fNota+Debito%2fNota+Debito+0032&rs:Command=Render&rs:format=PDF&iso_id=724&Office=167150&in_date=02/12/2013&invoicenumber=42538";

                try
                {
                    var u = new Uri(serverUrl);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(u);
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                    request.PreAuthenticate = true;
                    request.UseDefaultCredentials = false;
                    request.Proxy = WebRequest.GetSystemWebProxy();

                    var u0 = request.Proxy.GetProxy(u);
                    CredentialCache cache = new CredentialCache();
                    cache.Add(new Uri(serverUrl0), "Basic", new NetworkCredential(NAME, PASS));
                    cache.Add(u0, "Negotiate", new NetworkCredential(NAME, PASS, DOMAIN));
                    cache.Add(new Uri(serverUrl0), "Digest", new NetworkCredential(NAME, PASS, DOMAIN));
                    request.Credentials = cache;

                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        byte[] file = stream.ReadAllBytes();
                        response.Close();
                        File.WriteAllBytes("C:\\test.pdf", file);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                }



                ////CookieContainer cookies = new CookieContainer();
                ////// build authentication token
                ////string authToken = "15869be800840d58ed4ec1a9e5e32235:api_token";

                ////// encode to Base64
                ////byte[] bytes = Encoding.ASCII.GetBytes(authToken);
                ////authToken = Convert.ToBase64String(bytes);

                ////// write the "Authorization" header
                ////request.Headers.Add("Authorization", "Basic " + authToken);

                //CredentialCache myCache = new CredentialCache();
                //myCache.Add(new Uri("http://192.168.53.144/"), "Basic", new NetworkCredential("rosen.rusev", ""));
                //myCache.Add(new Uri("http://192.168.53.144/"), "Digest", new NetworkCredential("rosen.rusev", "", "fintrax.com"));

                //client.Credentials = myCache;PASS
                //client.Proxy = WebRequest.DefaultWebProxy;
                //client.Proxy.Credentials = myCache;

                
                var client = new WebClient();
                client.UseDefaultCredentials = false;
                client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)");
                client.Headers.Add("X-Parse-Application-Id", "12345678800");
                client.Headers.Add("X-Parse-REST-API-Key", "12345667890");
                var cache2 = new CredentialCache();
                cache2.Add(new Uri(serverUrl0), "Basic", new NetworkCredential("rosen.rusev", PASS));
                client.Credentials = cache2;
                var buffer = client.DownloadData(serverUrl);
                File.WriteAllBytes("C:\\test.pdf", buffer);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
    }
}
