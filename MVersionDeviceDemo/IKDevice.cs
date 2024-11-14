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
        // 相机类型，0为GV相机，1为CL相机，2为CXP相机
        public int m_nType = -1;
        // 图像宽度
        public int m_nWidth = -1;
        // 图像高度
        public int m_nHeight = -1;
        // 像素位数
        public int m_nDepth = 8;
        // 图像通道数
        public int m_nChannels = 1;
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

        /*
         *@brief:根据索引开启设备
         *@param [in] nDevIndex:相机索引
         *@param [in] nBoardIndex:采集卡索引
         *@return:是否开启成功
         */
        public virtual bool openDevice(int nDevIndex,int nBoardIndex)
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
        public bool setFeatureValue(string featureName,string featureValue)
        {
            IntPtr itkFeature = new IntPtr(-1);
            uint nType = 0;
            uint res = IKapCLib.ItkDevAllocFeature(m_pDev, featureName, ref itkFeature);
            if(!Check(res))
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
            switch(nType)
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
