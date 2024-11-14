using IKapC.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVersionDeviceDemo
{
    public class CamereaViewModel
    {

        // 设备类型枚举
        enum IKDeviceType
        {
            DEVICE_NIL = 0,
            DEVICE_CML,
            DEVICE_CXP,
            DEVICE_USB,
            DEVICE_GIGEVISION
        }
        struct IKDeviceInfo
        {
            public IKDeviceType nType;
            public int nDevIndex;
            public int nBoardIndex;
            public string sDevName;
        }

        /// <summary>
        /// 初始化
        /// </summary >
        /// <param name="paramFile">配置文件</param>
        /// <neturns>返回操作代码，与具体的实现有关</returns>
        public int Init(string[] paramFiles = null)
        {
            IKapCLib.ItkManInitialize();
            detectDevice();
            if (paramFiles.Count() == 2)
            {
                devU3v.loadConfiguration(paramFiles[0]);
                devCL.loadConfiguration(paramFiles[1]);
            }
            
            return 0;
        }
        /// <summary>
        /// 上相机 Camere Link
        /// </summary>
        IKDevice devCL = new IKDevice();
        /// <summary>
        /// 下相机 USB 3.0 相机
        /// </summary>
        IKDevice devU3v = new IKDevice();


        // 设备信息列表
        private List<IKDeviceInfo> m_listDeviceInfo = new List<IKDeviceInfo>();
        // 显示窗口列表
        private List<DeviceDisplayForm> m_listDisplayForm = new List<DeviceDisplayForm>();
        public void detectDevice()
        {
            m_listDeviceInfo.Clear();
            uint nDevCount = 0;
            uint res = IKapCLib.ItkManGetDeviceCount(ref nDevCount);
            Console.WriteLine((uint)ItkStatusErrorId.ITKSTATUS_OK);
            if (res != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                return;
            IKapCLib.ITKDEV_INFO pDevInfo = new IKapCLib.ITKDEV_INFO();
            IKapCLib.ITK_CL_DEV_INFO pClDevInfo = new IKapCLib.ITK_CL_DEV_INFO();
            IKapCLib.ITK_CXP_DEV_INFO pCxpDevInfo = new IKapCLib.ITK_CXP_DEV_INFO();
            IKapCLib.ITKGIGEDEV_INFO pGvDevInfo = new IKapCLib.ITKGIGEDEV_INFO();
            IKapCLib.ITK_U3V_DEV_INFO pU3vDevInfo = new IKapCLib.ITK_U3V_DEV_INFO();
            for (uint i = 0; i < nDevCount; ++i)
            {
                IKDeviceInfo pInfo = new IKDeviceInfo();
                IKapCLib.ItkManGetDeviceInfo(i, ref pDevInfo);
                if (pDevInfo.DeviceClass.CompareTo("GigEVision") == 0)
                {
                    res = IKapCLib.ItkManGetGigEDeviceInfo(i, ref pGvDevInfo);
                    if (res != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                        return;
                    pInfo.nType = IKDeviceType.DEVICE_GIGEVISION;
                    pInfo.nDevIndex = (int)i;
                    pInfo.nBoardIndex = -1;
                    pInfo.sDevName = pDevInfo.FullName;
                }
                else if (pDevInfo.DeviceClass.CompareTo("USB3Vision") == 0)
                {
                    res = IKapCLib.ItkManGetU3VDeviceInfo(i, ref pU3vDevInfo);
                    if (res != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                        return;
                    pInfo.nType = IKDeviceType.DEVICE_USB;
                    pInfo.nDevIndex = (int)i;
                    pInfo.nBoardIndex = -1;
                    pInfo.sDevName = pDevInfo.FullName;
                    //continue;
                }
                else if (pDevInfo.DeviceClass.CompareTo("CoaXPress") == 0)
                {
                    res = IKapCLib.ItkManGetCXPDeviceInfo(i, ref pCxpDevInfo);
                    if (res != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                        return;
                    pInfo.nType = IKDeviceType.DEVICE_CXP;
                    pInfo.nDevIndex = (int)i;
                    pInfo.nBoardIndex = (int)pCxpDevInfo.BoardIndex;
                    pInfo.sDevName = pDevInfo.FullName;
                }
                else
                {
                    res = IKapCLib.ItkManGetCLDeviceInfo(i, ref pClDevInfo);
                    //if (res != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                    //    return;
                    pInfo.nType = IKDeviceType.DEVICE_CML;
                    pInfo.nDevIndex = (int)i;
                    pInfo.nBoardIndex = (int)pClDevInfo.BoardIndex;
                    pInfo.sDevName = pDevInfo.FullName;
                }
                m_listDeviceInfo.Add(pInfo);
            }
        }

        /// <summary>
        /// 打开设备
        /// </summary>
        /// sneturns>返回操作代码，与具体的实现有关</returns>
        public int Open()
        {

            for (int i = 0; i < m_listDeviceInfo.Count; i++)
            {
                IKDeviceInfo pInfo = m_listDeviceInfo[i];
                switch (pInfo.nType)
                {
                    case IKDeviceType.DEVICE_GIGEVISION:
                        // pDev = new IKDeviceGV();
                        break;
                    case IKDeviceType.DEVICE_USB:
                        devU3v = new IKDeviceU3v();
                        break;
                    case IKDeviceType.DEVICE_CML:
                        devCL = new IKDeviceCL();
                        break;
                    case IKDeviceType.DEVICE_CXP:
                        // pDev = new IKDeviceCXP();
                        break;
                }
            }
            for (int i = 0; i < m_listDeviceInfo.Count; i++)
            {
                IKDeviceInfo pInfo = m_listDeviceInfo[i];
                if (pInfo.nType == IKDeviceType.DEVICE_USB)
                {
                    devU3v.openDevice(pInfo.nDevIndex, pInfo.nBoardIndex);
                }

                if (pInfo.nType == IKDeviceType.DEVICE_CML)
                {
                    devCL.openDevice(pInfo.nDevIndex, pInfo.nBoardIndex);
                }
            }
            if (devCL.isOpen() && devU3v.isOpen())
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns>返回操作代码，与具体的实现有关</returns>
        public int Close()
        {
            if (devU3v.closeDevice() && devCL.closeDevice()) {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// 保存指定相机最近一次所取图像，BMP格式
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        int Save(string camera, string filePath)
        {

            return 0;
        }

        /// <summary>
        /// 通过相机名称进行获取图像操作
        /// </summary>
        /// <param name="img"></param>
        /// <param name="name">相机名称</param〉
        /// <param name="index">拍照序号</param>
        /// returns></returns>
        int Grab(out object img, string name, int index = 0)
        {
            img = null;
            if (index == 0)
            {
                this.devU3v.startGrab(1);
            }
            
            
            return 0;
        }

        /// <summary>
        /// 显示扫码相机实时画面
        /// </summary>
        /// <param name="index">相机编号</param>
        void ShowVideo(int index = 0)
        {

        }
        /// <summary>
        /// </summary>
        /// <param name="index">相机编号</param>
        void CloseVideo(int index = 0)
        {

        }
    }
}
