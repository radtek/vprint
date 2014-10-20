﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using CPrint2.Data;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CPrint2.Controls
{
    public partial class CameraControl : UserControl
    {
        public delegate void ImageTakenDelegate(object sender, EventArgs e);
        public event ImageTakenDelegate ImageTaken;

        private Capture m_Cap = default(Capture);

        public FileInfo Cap_Back_Info { get; set; }
        public FileInfo Cap_Fore_Info { get; set; }
        public int CameraIndex { get; set; }

        private volatile bool m_InitCommand, m_RunCommand;

        public CameraControl()
        {
            InitializeComponent();
        }

        public CameraControl(int index)
        {
            InitializeComponent();
            CameraIndex = index;

            m_Cap = new Capture(index);
            m_Cap.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_COUNT, Config.FRAME_COUNT);//
            m_Cap.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, Config.FRAME_WIDTH);//5168, 1280, 2304, 
            m_Cap.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, Config.FRAME_HEIGHT);//2907, 720, 1536 
            m_Cap.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FOURCC, CvInvoke.CV_FOURCC('U', '2', '6', '3')); //622,3730
            m_Cap.ImageGrabbed += cap_ImageGrabbed;
        }

        public void Start()
        {
            m_Cap.Start();
        }

        public void Init()
        {
            m_InitCommand = true;
        }

        public void Run()
        {
            m_RunCommand = true;
        }

        public void Stop()
        {
            m_Cap.Stop();
        }

        private void cap_ImageGrabbed(object sender, EventArgs e)
        {
            using (var img = m_Cap.RetrieveBgrFrame())
            {
                if (m_InitCommand)
                {
                    m_InitCommand = false;
                    Cap_Back_Info = Cap_Back_Info.DeleteSave().Temp().IfDebug(string.Format("C:\\test{0}_back.jpg", CameraIndex));
                    img.ToBitmap().Save(Cap_Back_Info.FullName, ImageFormat.Jpeg);
                }
                else if (m_RunCommand && Cap_Back_Info != null)
                {
                    m_RunCommand = false;

                    Cap_Fore_Info = ((FileInfo)null).Temp().IfDebug(string.Format("C:\\test{0}_fore.jpg", CameraIndex));

                    using (Image<Bgr, byte> back = new Image<Bgr, byte>(Cap_Back_Info.FullName))
                    {
                        Subtract filter = new Subtract(back.ToBitmap());
                        img.DrawBorder(5, Color.Black);
                        using (Bitmap sourceImage = img.ToBitmap())
                        using (Bitmap image = filter.Apply(sourceImage))
                        {
                            Rectangle rect = Rectangle.Empty;
                            using (Bitmap biggestBlobsImage = image.ExtractBiggestBlob(ref rect))
                            using (Image<Bgr, byte> rimg = img.Copy(rect))
                                rimg.ToBitmap().Save(Cap_Fore_Info.FullName, ImageFormat.Jpeg);
                        }
                    }
                    StopStartGrabbers();
                }
                captureBox.Image = img;
            }
        }

        private void StopStartGrabbers()
        {
            var t = Task.Factory.StartNew(() =>
            {
                m_Cap.ImageGrabbed -= cap_ImageGrabbed;

                if (Cap_Fore_Info != null && ImageTaken != null)
                {
                    captureBox.Image = new Image<Bgr, byte>(Cap_Fore_Info.FullName);
                    ImageTaken(this, EventArgs.Empty);
                }

                var tt = Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(Config.FRAME_SHOWN_INSEC));
                    m_Cap.ImageGrabbed += cap_ImageGrabbed;
                });
            });
        }
    }
}