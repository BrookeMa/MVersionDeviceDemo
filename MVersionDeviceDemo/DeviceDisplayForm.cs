using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using IKapBoardClassLibrary;
using IKapC.NET;

namespace MVersionDeviceDemo
{
    public partial class DeviceDisplayForm : Form
    {
        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        // 设备句柄
        public IKDevice m_pDev = new IKDevice();
        // 图像更新上下文
        public SynchronizationContext m_imageUpdateSync;
        // 采集结束上下文
        public SynchronizationContext m_grabStopSync;
        // 是否正在显示
        public volatile bool m_bShowing = false;
        // 相机是否为连续采集模式
        public bool m_bIsContinousGrab = false;
        // 窗体绑定的设备索引，在窗体创建时传入
        public int m_nIndex = -1;
        // 窗体最大高度
        public int m_nMaxHeight = -1;
        // 窗体最大宽度
        public int m_nMaxWidth = -1;

        /*
         * @brief: 构造函数
         * @param [in] pDev: 设备句柄
         * @param [in] nIndex: 设备索引
         * 
         */
        public DeviceDisplayForm(IKDevice pDev, int nIndex)
        {
            InitializeComponent();
            m_imageUpdateSync = SynchronizationContext.Current;
            m_grabStopSync = SynchronizationContext.Current;
            m_pDev = pDev;
            m_nIndex = nIndex;
            ControlBox = false;
        }

        /*
         * @brief: 显示图像
         * @param [in] nCount: 显示帧数，0为连续显示
         * @return:
         */
        public void showImage(int nCount)
        {
            if (!m_pDev.createBuffer())
                return;
            //m_bGrabing = true;
            Thread thread = new Thread(new ParameterizedThreadStart(workThread));
            thread.Start(this);
            if (nCount != 0)
                m_bIsContinousGrab = false;
            else
            {
                m_bIsContinousGrab = true;
            }
            if (!m_pDev.startGrab(nCount))
                return;
        }

        /*
         * brief: 加载采集卡配置文件
         * @param [in] sFilePath: 文件路径
         * @return:
         */
        public void loadConfigure(string sFilePath)
        {
            m_pDev.loadConfiguration(sFilePath);
        }

        public void stopGrab()
        {
            if (m_pDev.m_bGrabingImage) m_pDev.stopGrab();
            while (m_bShowing) { Thread.Sleep(1); }
            m_pDev.clearBuffer();
        }

        /*
         * @brief: 保存图片
         * @param [in]: 保存格式，目前可选bmp或tiff
         * @return:
         */
        public void saveImage(string format)
        {
            if (m_bShowing || this.pictureBoxImage.Image == null)
                return;
            SaveFileDialog saveImg = new SaveFileDialog();
            saveImg.Title = "图片保存";
            saveImg.Filter = "BMP(*.bmp)|*.bmp|TIFF(*.tif)|*.tif";
            int formatIndex = 0;
            if (String.Equals(format, "bmp"))
            {
                saveImg.FilterIndex = 1;
                formatIndex = 0;
            }
            else if (String.Equals(format, "tiff"))
            {
                saveImg.FilterIndex = 2;
                formatIndex = 1;
            }
            if (saveImg.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveImg.FileName.ToString();
                if (fileName != "" && fileName != null)
                {
                    System.Drawing.Imaging.ImageFormat imgFormat = System.Drawing.Imaging.ImageFormat.Png;
                    switch (formatIndex)
                    {
                        case 0:
                            imgFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                            break;
                        case 1:
                            imgFormat = System.Drawing.Imaging.ImageFormat.Tiff;
                            break;
                    }
                    try
                    {
                        this.pictureBoxImage.Image.Save(fileName, imgFormat);
                    }
                    catch
                    {
                        MessageBox.Show("Save image failure", "Warning", MessageBoxButtons.OK);
                    }
                }
            }
        }

        /*
         * @brief: 图像转换线程，将缓冲区中的图像数据转换为Image
         */
        static void workThread(object obj)
        {
            DeviceDisplayForm hDisplay = obj as DeviceDisplayForm;
            BufferToImage hBuffer = new BufferToImage(hDisplay.m_pDev.m_nBufferSize
                , hDisplay.m_pDev.m_nDepth, hDisplay.m_pDev.m_nImageDepth, hDisplay.m_pDev.m_nChannels
                , hDisplay.m_pDev.m_nWidth, hDisplay.m_pDev.m_nHeight, hDisplay.m_pDev.m_nBayerPattern);
            Image im = null;
            while (!hDisplay.m_pDev.m_bGrabingImage)
            {
                Thread.Sleep(1);
            }
            hDisplay.m_bShowing = true;
            while (hDisplay.m_pDev.m_bGrabingImage)
            {
                if (hDisplay.m_pDev.m_bUpdateImage)
                {
                    lock (hDisplay.m_pDev.m_mutexImage)
                    {
                        hDisplay.m_pDev.m_bUpdateImage = false;
                        //对于Bayer图像，转换为RGB格式
                        if (hDisplay.m_pDev.m_nBayerPattern != 0)
                        {
                            IntPtr pConvertedData = Marshal.AllocHGlobal(hBuffer.m_nBufferSize);
                            //获取相机像素格式
                            hDisplay.m_pDev.m_uCameraPixelFormat = hDisplay.m_pDev.getPixelFormat();
                            if (0 == hDisplay.m_pDev.m_uCameraPixelFormat)
                            {
                                System.Diagnostics.Debug.WriteLine("getPixelFormat excute failed!");
                                Marshal.FreeHGlobal(pConvertedData);

                                break;
                            }
                            //根据bayer图像位深确定转换后的RGB图像的像素格式
                            uint uDstPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB888;
                            if (hDisplay.m_pDev.m_nDepth == 8)
                            {
                                ;
                            }
                            else if (hDisplay.m_pDev.m_nDepth == 12)
                            {
                                uDstPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB121212;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("The bit_Depth of bayer image is not supported!");
                                Marshal.FreeHGlobal(pConvertedData);

                                break;
                            }
                            //图像转换
                            if (!IKDevice.Bayer2RGB(hDisplay.m_pDev.m_pUserBuffer, hDisplay.m_pDev.m_uCameraPixelFormat, ref pConvertedData, uDstPixelFormat, hBuffer.m_nWidth, hBuffer.m_nHeight, hDisplay.m_pDev.m_nBayerPattern))
                            {
                                System.Diagnostics.Debug.WriteLine("Bayer transform to RGB failed!");
                                Marshal.FreeHGlobal(pConvertedData);

                                break;
                            }

                            im = hBuffer.toImage(pConvertedData);
                            Marshal.FreeHGlobal(pConvertedData);
                        }
                        else
                        {
                            im = hBuffer.toImage(hDisplay.m_pDev.m_pUserBuffer);
                        }
                    }
                    hDisplay.m_imageUpdateSync.Post(hDisplay.ImageUpdateSyncContext, im.Clone());

                    // 将图片保存到Manager里面，保证连续取图的时候，可以存图片
                    if (hDisplay.m_pDev is IKDeviceU3V && CameraManager.Instance.m_bGrabOnceRequestedU3V)
                    {
                        // 观察一下
                        CameraManager.Instance.imageU3V = (Image)im.Clone();
                        CameraManager.Instance.m_bGrabOnceRequestedU3V = false;
                    }
                    if (hDisplay.m_pDev is IKDeviceCL && CameraManager.Instance.m_bGrabOnceRequestedU3V)
                    {
                        // 观察一下
                        CameraManager.Instance.imageCL = (Image)im.Clone();
                        CameraManager.Instance.m_bGrabOnceRequestedCL = false;
                    }

                    im.Dispose();
                }
                if (!hDisplay.m_pDev.m_bGrabingImage)
                    break;
                //Thread.Sleep(10);
            }
            hDisplay.m_bShowing = false;
            hBuffer.freeBuffer();
            hDisplay.m_grabStopSync.Post(hDisplay.GrabStopSyncContext, null);

        }

        // 停止采集
        private void GrabStopSyncContext(object obj)
        {
            stopGrab();
        }

        // 刷新图像
        private void ImageUpdateSyncContext(object obj)
        {
            if (this.pictureBoxImage.Image != null)
                this.pictureBoxImage.Image.Dispose();
            this.pictureBoxImage.Image = obj as Image;
        }

        private void DeviceDisplayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_bShowing)
            {
                m_pDev.stopGrab();
                do
                {
                } while (m_pDev.m_bGrabingImage); //等待
            }
            m_pDev.clearBuffer();
            m_pDev.closeDevice();
        }

        private void pictureBoxImage_Resize(object sender, EventArgs e)
        {
            int nHeight = this.Height > m_nMaxHeight ? m_nMaxHeight : this.Height;
            int nWidth = this.Width > m_nMaxWidth ? m_nMaxWidth : this.Width;
            this.Width = nWidth;
            this.Height = nHeight;
        }

        private void pictureBoxImage_Click(object sender, EventArgs e)
        {

        }
    }
}
