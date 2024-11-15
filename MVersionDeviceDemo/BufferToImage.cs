using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using IKapC.NET;

namespace MVersionDeviceDemo
{
    class BufferToImage
    {
        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

        // 缓冲区句柄
        public IntPtr m_pBuffer = new IntPtr(-1);
        // 缓冲区锁
        public object m_mutexBuffer = new object();
        // Bitmap锁
        public object m_mutexBitmap = new object();
        // 缓冲区大小
        public int m_nBufferSize = -1;
        // 位深
        public int m_nDepth = -1;
        // 图像深度
        public int m_nImageDepth = -1;
        // 图像通道数
        public int m_nChannels = -1;
        // 图像宽度
        public int m_nWidth = -1;
        // 图像高度
        public int m_nHeight = -1;
        //图像bayer格式：0:非bayer格式; 1:BGGR; 2:RGGB; 3:GBRG; 4:GRBG
        public int m_nBayerPattern = -1;
        // Bitmap
        public Bitmap m_bmp = null;


        /*
         * @brief: 构造函数
         * @param [in] nSize: 缓冲区大小
         * @param [in] nDepth: 位深
         * @param [in] nDepth: 位深
         * @param [in] nChannels: 图像通道数
         * @param [in] nWidth: 图像宽度
         * @param [in] nHeight: 图像高度
         * 
         */
        public BufferToImage(int nSize, int nDepth, int nImageDepth, int nChannels, int nWidth, int nHeight, int nBayerPattern)
        {
            if (nBayerPattern == 0)
            {
                m_pBuffer = Marshal.AllocHGlobal(nSize);
                m_nBufferSize = nSize;
                m_nChannels = nChannels;
                m_nImageDepth = nImageDepth;
            }
            else
            {
                //修改BufferToImage参数,适配bayer图像转换的RGB图像
                m_pBuffer = Marshal.AllocHGlobal(nSize * 3);
                m_nBufferSize = nSize * 3;
                m_nChannels = 3;
                m_nImageDepth = nImageDepth * 3;
            }
            m_nDepth = nDepth;
            m_nWidth = nWidth;
            m_nHeight = nHeight;
            //m_nBayerPattern = nBayerPattern;
            PixelFormat nPixelFormat = PixelFormat.Undefined;
            switch (m_nChannels)
            {
                case 1:
                    nPixelFormat = PixelFormat.Format8bppIndexed;
                    break;
                case 3:
                case 4:
                    if (m_nImageDepth == 24)
                    {
                        nPixelFormat = PixelFormat.Format24bppRgb;
                    }
                    else if (m_nImageDepth == 48)
                    {
                        nPixelFormat = PixelFormat.Format48bppRgb;
                    }
                    else
                    {
                        MessageBox.Show("Not supported imageDepth!", "Error", MessageBoxButtons.OK);
                    }

                    break;
            }
            m_bmp = new Bitmap(m_nWidth, m_nHeight, nPixelFormat);
            // 如果是灰度图像需要设置调色盘
            if (nPixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                ColorPalette cp = m_bmp.Palette;
                for (int i = 0; i < 256; i++)
                    cp.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                m_bmp.Palette = cp;
            }
        }

        /*
         * @brief: 释放缓冲区
         * @return:
         */
        public void freeBuffer()
        {
            if (m_pBuffer != new IntPtr(-1))
                Marshal.FreeHGlobal(m_pBuffer);
            if (m_bmp != null)
                m_bmp.Dispose();
            m_bmp = null;
        }

        /*
         * @brief:将源缓冲区的图像数据转为Image
         * @param [in] pSrc: 源缓冲区指针
         * @return: Image
         */
        public Image toImage(IntPtr pSrc)
        {
            Image im = null;
            lock (m_mutexBitmap)
            {
                lock (m_mutexBuffer)
                {
                    CopyMemory(m_pBuffer, pSrc, m_nBufferSize);
                    if (m_nChannels == 4)
                    {
                        readRGBC();
                        im = (Image)m_bmp.Clone();
                        return im;
                    }
                    Rectangle rect = new Rectangle(0, 0, m_bmp.Width, m_bmp.Height);
                    BitmapData bitmapData = m_bmp.LockBits(rect, ImageLockMode.ReadWrite, m_bmp.PixelFormat);
                    int nStride = m_nBufferSize / m_bmp.Height;
                    for (int i = 0; i < m_bmp.Height; i++)
                    {
                        IntPtr iptrDst = bitmapData.Scan0 + bitmapData.Stride * i;
                        IntPtr iptrSrc = m_pBuffer + nStride * i;
                        CopyMemory(iptrDst, iptrSrc, nStride);
                    }
                    m_bmp.UnlockBits(bitmapData);

                    im = (Image)m_bmp.Clone();
                }
            }
            return im;
        }

        /*
         * @brief: 转换四通道图像数据，仅保留RGB通道数据
         * @return:
         */
        public void readRGBC()
        {
            Rectangle rect = new Rectangle(0, 0, m_bmp.Width, m_bmp.Height);
            BitmapData bitmapData = m_bmp.LockBits(rect, ImageLockMode.ReadWrite, m_bmp.PixelFormat);

            int nShift = m_nDepth - 8;
            int nStride = bitmapData.Stride;
            int nCount = 0;
            byte[] pByteData = new byte[m_nBufferSize];
            byte[] pDstData = new byte[(m_nBufferSize * 3) / 4];
            Marshal.Copy(m_pBuffer, pByteData, 0, m_nBufferSize);
            if (m_nDepth == 8)
            {
                for (int i = 0; i < m_bmp.Height; i++)
                {
                    for (int j = 0; j < nStride; j = j + 3)
                    {
                        pDstData[i * nStride + j] = (byte)(pByteData[nCount]);
                        pDstData[i * nStride + j + 1] = (byte)(pByteData[nCount + 1]);
                        pDstData[i * nStride + j + 2] = (byte)(pByteData[nCount + 2]);
                        nCount += 4;
                    }
                }
                Marshal.Copy(pDstData, 0, bitmapData.Scan0, (m_nBufferSize * 3 / 4));
                m_bmp.UnlockBits(bitmapData);
                return;
            }
            short[] pShortData = new short[m_nBufferSize / 2];
            Marshal.Copy(m_pBuffer, pShortData, 0, m_nBufferSize / 2);
            for (int i = 0; i < bitmapData.Height; i++)
            {
                for (int j = 0; j < nStride; j = j + 3)
                {
                    pDstData[i * nStride + j] = (byte)(pShortData[nCount] >> nShift);
                    pDstData[i * nStride + j + 1] = (byte)(pShortData[nCount + 1] >> nShift);
                    pDstData[i * nStride + j + 2] = (byte)(pShortData[nCount + 2] >> nShift);
                    nCount += 4;
                }
            }
            Marshal.Copy(pDstData, 0, bitmapData.Scan0, (m_nBufferSize * 3 / 8));
            m_bmp.UnlockBits(bitmapData);
        }
    }
}
