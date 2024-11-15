using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using IKapC.NET;
using IKapBoardClassLibrary;

namespace MVersionDeviceDemo
{
    public class IKDevice
    {
        // 相机句柄
        public IntPtr m_pDev = new IntPtr(-1);
        // 采集卡句柄
        public IntPtr m_pBoard = new IntPtr(-1);
        // 用户缓冲区，用于图像数据转换
        public IntPtr m_pUserBuffer = new IntPtr(-1);
        // 是否正在采集
        public volatile bool m_bGrabingImage = false;
        // 是否已更新用户缓冲区
        public volatile bool m_bUpdateImage = false;
        // 相机类型，0为GV/XGV相机+网卡，1为CL相机，2为CXP相机，3为GV/XGV相机+采集卡
        public int m_nType = -1;
        // 图像宽度
        public int m_nWidth = -1;
        // 图像高度
        public int m_nHeight = -1;
        // 通道位深
        public int m_nDepth = 8;
        // 图像通道数
        public int m_nChannels = 1;
        // 图像深度
        public int m_nImageDepth = 8;
        // BayerPattern: 0:非bayer格式; 1:BGGR; 2:RGGB; 3:GBRG; 4:GRBG
        public int m_nBayerPattern = 0;
        // 相机索引
        public int m_nDevIndex = -1;
        // 采集卡索引
        public int m_nBoardIndex = -1;
        // 相机缓冲区个数
        public int m_nFrameCount = 2;
        // 当前帧索引
        public int m_nCurFrameIndex = 0;
        // 相机缓冲区大小
        public int m_nBufferSize = 0;
        // 用户缓冲区锁
        public object m_mutexImage = new object();
        // 相机像素格式
        public uint m_uCameraPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB888;
        public IntPtr m_color = new IntPtr(-1);
        /*
         *@brief:根据索引开启设备
         *@param [in] nDevIndex:相机索引
         *@param [in] nBoardIndex:采集卡索引
         *@return:是否开启成功
         */
        public virtual bool openDevice(int nDevIndex, int nBoardIndex)
        {
            return false;
        }

        /*
         *@brief:关闭相机
         *@param [in]:
         *@return:是否关闭成功
         */
        public virtual bool closeDevice()
        {
            return false;
        }

        /*
         *@brief:查询设备连接状态
         *@param [in]:
         *@return:是否连接
         */
        public virtual bool isOpen()
        {
            return false;
        }

        /*
         *@brief:加载采集卡配置文件
         *@param [in] sFilePath:配置文件路径
         *@return:是否加载成功
         */
        public virtual bool loadConfiguration(string sFilePath)
        {
            return false;
        }

        /*
         * @brief:申请缓冲区资源
         * @return:是否申请成功
         */
        public virtual bool createBuffer()
        {
            return false;
        }

        /*
         * @brief:清除已申请缓冲区资源
         * @return:
         */
        public virtual void clearBuffer()
        {
        }

        /*
         *@brief:开始采集
         *@param [in] nCount:采集帧数
         *@return:是否开始采集
         */
        public virtual bool startGrab(int nCount)
        {
            return false;
        }

        /*
         *@brief:停止采集
         *@param [in]:
         *@return:是否停止采集
         */
        public virtual bool stopGrab()
        {
            return false;
        }

        /*
         * @brief:设置相机特征值
         * @param [in] featureName:特征名
         * @param [in] featureValue:特征值
         * @return: 是否设置成功
         */
        public bool setFeatureValue(string featureName, string featureValue)
        {
            IntPtr itkFeature = new IntPtr(-1);
            uint nType = 0;
            uint res = IKapCLib.ItkDevAllocFeature(m_pDev, featureName, ref itkFeature);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Camera error:Allocate feature failed");
                return false;
            }
            res = IKapCLib.ItkFeatureGetType(itkFeature, ref nType);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Camera error:Get feature type failed");
                return false;
            }
            switch (nType)
            {
                case (uint)ItkFeatureType.ITKFEATURE_VAL_TYPE_INT32:
                    res = IKapCLib.ItkFeatureSetInt32(itkFeature, Convert.ToInt32(featureValue));
                    break;
                case (uint)ItkFeatureType.ITKFEATURE_VAL_TYPE_INT64:
                    res = IKapCLib.ItkFeatureSetInt64(itkFeature, Convert.ToInt64(featureValue));
                    break;
                case (uint)ItkFeatureType.ITKFEATURE_VAL_TYPE_FLOAT:
                case (uint)ItkFeatureType.ITKFEATURE_VAL_TYPE_DOUBLE:
                    res = IKapCLib.ItkFeatureSetDouble(itkFeature, Convert.ToDouble(featureValue));
                    break;
                case (uint)ItkFeatureType.ITKFEATURE_VAL_TYPE_ENUM:
                case (uint)ItkFeatureType.ITKFEATURE_VAL_TYPE_STRING:
                    res = IKapCLib.ItkFeatureFromString(itkFeature, featureValue);
                    break;
                case (uint)ItkFeatureType.ITKFEATURE_VAL_TYPE_COMMAND:
                    res = IKapCLib.ItkFeatureExecuteCommand(itkFeature);
                    break;
            }
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Camera error:Set feature failed");
                return false;
            }
            return true;
        }

        /*
 *@brief:获取相机图片格式
 *@param [in]:
 *@return:相机图片格式
 */
        public virtual uint getPixelFormat()
        {
            uint nPixelFormat = 0;
            StringBuilder sPixelFormat = new StringBuilder(64);
            uint nFormatLen = 64;
            uint res = IKapCLib.ItkDevToString(m_pDev, "PixelFormat", sPixelFormat, ref nFormatLen);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Pixel format error:Get pixel format failed");
                return 0;
            }
            if (sPixelFormat.ToString() == "Mono8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO8;
                m_nDepth = 8;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "Mono10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO10;
                m_nDepth = 10;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "Mono12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO12;
                m_nDepth = 12;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGR8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR8;
                m_nDepth = 8;
                m_nChannels = 1;
                m_nBayerPattern = 4;
            }
            else if (sPixelFormat.ToString() == "BayerRG8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG8;
                m_nDepth = 8;
                m_nChannels = 1;
                m_nBayerPattern = 2;
            }
            else if (sPixelFormat.ToString() == "BayerGB8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB8;
                m_nDepth = 8;
                m_nChannels = 1;
                m_nBayerPattern = 3;
            }
            else if (sPixelFormat.ToString() == "BayerBG8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG8;
                m_nDepth = 8;
                m_nChannels = 1;
                m_nBayerPattern = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGR10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR10;
                m_nDepth = 10;
                m_nChannels = 1;
                m_nBayerPattern = 4;
            }
            else if (sPixelFormat.ToString() == "BayerRG10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG10;
                m_nDepth = 10;
                m_nChannels = 1;
                m_nBayerPattern = 2;
            }
            else if (sPixelFormat.ToString() == "BayerGB10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB10;
                m_nDepth = 10;
                m_nChannels = 1;
                m_nBayerPattern = 3;
            }
            else if (sPixelFormat.ToString() == "BayerBG10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG10;
                m_nDepth = 10;
                m_nChannels = 1;
                m_nBayerPattern = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGR12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR12;
                m_nDepth = 12;
                m_nChannels = 1;
                m_nBayerPattern = 4;
            }
            else if (sPixelFormat.ToString() == "BayerRG12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG12;
                m_nDepth = 12;
                m_nChannels = 1;
                m_nBayerPattern = 2;
            }
            else if (sPixelFormat.ToString() == "BayerGB12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB12;
                m_nDepth = 12;
                m_nChannels = 1;
                m_nBayerPattern = 3;
            }
            else if (sPixelFormat.ToString() == "BayerBG12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG12;
                m_nDepth = 12;
                m_nChannels = 1;
                m_nBayerPattern = 1;
            }
            else if (sPixelFormat.ToString() == "RGB8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB888;
                m_nDepth = 8;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "RGB10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB101010;
                m_nDepth = 10;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "RGB12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB121212;
                m_nDepth = 12;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "BGR8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR888;
                m_nDepth = 8;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "BGR10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR101010;
                m_nDepth = 10;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "BGR12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR121212;
                m_nDepth = 12;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "YUV422_8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_YUV422_8_UYUV;
                m_nDepth = 8;
                m_nChannels = 3;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Pixel format error:Undefined format type");
                return 0;
            }
            return nPixelFormat;
        }

        /*
         *@brief:Bayer image transform to RGB image
         *@param [in]：pDataSrc: the buffer of Bayer image 
         *@param [in]：nDataSrcFormat: the pixelformat of Bayer image defiend in IKapCLib 
         *@param [out]：pDataDst: the buffer of RGB image
         *@param [in]：nDataSrcFormat: the pixelformat of RGB image defiend in IKapCLib 
         *@param [in]：nWidth: the width of image
         *@param [in]：nHeight: the height of image
         *@param [in]：nBayerEncodeType: the encoding type of Bayer image，optional valid params: 1[BGGR]; 2[RGGB]; 3[GBRG]; 4[GRBG]
         *@return:是否错误
         */
        public static bool Bayer2RGB(IntPtr pDataSrc, uint nDataSrcFormat, ref IntPtr pDataDst, uint nDataDstFormat, int nWidth, int nHeight, int nBayerEncodeType)
        {
            uint res = (uint)ItkStatusErrorId.ITKSTATUS_OK;
            IntPtr itkBufferBayer = IntPtr.Zero;
            IntPtr itkBufferRGB = IntPtr.Zero;

            res = IKapCLib.ItkBufferNewWithPtr(nWidth, nHeight, nDataSrcFormat, pDataSrc, ref itkBufferBayer);
            if (!IKDevice.Check(res))
            {
                return false;
            }
            res = IKapCLib.ItkBufferNewWithPtr(nWidth, nHeight, nDataDstFormat, pDataDst, ref itkBufferRGB);
            if (!IKDevice.Check(res))
            {
                return false;
            }

            res = IKapCLib.ItkBufferBayerConvert(itkBufferBayer, itkBufferRGB, (uint)Math.Pow(2, (nBayerEncodeType - 1)));
            if (!IKDevice.Check(res))
            {
                return false;
            }

            res = IKapCLib.ItkBufferFree(itkBufferBayer);
            if (!IKDevice.Check(res))
            {
                return false;
            }
            res = IKapCLib.ItkBufferFree(itkBufferRGB);
            if (!IKDevice.Check(res))
            {
                return false;
            }

            return true;
        }

        /*
         *@brief:检查错误码
         *@param [in] err:错误码
         *@return:是否错误
         */
        public static bool Check(uint err)
        {
            if (err != (uint)ItkStatusErrorId.ITKSTATUS_OK)
            {
                System.Diagnostics.Debug.WriteLine("Error code: {0}.\n", err.ToString("x8"));
                return false;
            }
            return true;
        }
    }
}
