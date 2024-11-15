﻿using IKapC.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public int Init(string[] paramFiles)
        {
            IKapCLib.ItkManInitialize();
            detectDevice();
            if (paramFiles != null)
            {
                devU3v.loadConfiguration(paramFiles[0]);
                devCL.loadConfiguration(paramFiles[1]);
            }
            return 0;
        }
        /// <summary>
        /// 上相机 Camere Link
        /// </summary>
        IKDevice devCL = new IKDeviceCL();
        /// <summary>
        /// 下相机 USB 3.0 相机
        /// </summary>
        IKDevice devU3v = new IKDeviceU3v();
    
        /// <summary>
        /// 上相机 Camera Link 相机显示窗口
        /// </summary>
        DeviceDisplayForm displayFormCL;
        /// <summary>
        /// 下相机USB 3.0 相机显示窗口
        /// </summary>
        DeviceDisplayForm displayFormU3v;

        // 设备信息列表
        private List<IKDeviceInfo> m_listDeviceInfo = new List<IKDeviceInfo>();
        // 显示窗口列表
        private List<DeviceDisplayForm> m_listDisplayForm = new List<DeviceDisplayForm>();
        public void detectDevice()
        {
            m_listDeviceInfo.Clear();
            uint nDevCount = 0;
            uint res = IKapCLib.ItkManGetDeviceCount(ref nDevCount);
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
            foreach (IKDeviceInfo pInfo in m_listDeviceInfo)
            {
                if (pInfo.nType == IKDeviceType.DEVICE_USB)
                {
                    devU3v.openDevice(pInfo.nDevIndex, pInfo.nBoardIndex);
                }

                if (pInfo.nType == IKDeviceType.DEVICE_CML)
                {
                    devCL.openDevice(pInfo.nDevIndex, pInfo.nBoardIndex);
                }
            }
            if (devU3v.isOpen())
            {
                Console.WriteLine("USB相机打开成功");
            }
            else
            {
                Console.WriteLine("USB相机打开失败");

            }
            if (devCL.isOpen())
            {
                Console.WriteLine("CL相机打开");
            }
            else
            {
                Console.WriteLine("CL相机失败");
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
            devU3v.clearBuffer();
            devCL.clearBuffer();
            bool isU3vClosed = devU3v.closeDevice();
            bool isCLClosed = devCL.closeDevice();

            if (isU3vClosed)
            {
                Console.WriteLine("关闭U3V相机成功。");
            } 
            else
            {
                Console.WriteLine("关闭U3V相机失败。");

            }

            if (isCLClosed)
            {
                Console.WriteLine("关闭CL相机成功。");
            }
            else
            {
                Console.WriteLine("关闭CL相机失败。");

            }
            if (isU3vClosed && isCLClosed)
            {
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
        public int Save(string camera, string filePath)
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
        public int Grab(out object img, string name, int index = 0)
        {
            img = null;
            if (name == "U3v")
            {
                this.devU3v.startGrab(1);
            }
            
            return 0;
        }

        /// <summary>
        /// 显示扫码相机实时画面
        /// </summary>
        /// <param name="index">相机编号</param>
        public void ShowVideo(int index = 0)
        {
            if (index == 0)
            {
                IKDeviceInfo pInfo = m_listDeviceInfo.Find(p => p.nType == IKDeviceType.DEVICE_USB);

                displayFormU3v = new DeviceDisplayForm(devU3v, pInfo.nBoardIndex)
                {
                    m_nMaxHeight = 400,
                    m_nMaxWidth = 400,
                    Text = pInfo.sDevName,
                    Height = 400,
                    Width = 400
                };

                displayFormU3v.TopLevel = true;
                displayFormU3v.StartPosition = FormStartPosition.CenterScreen;
                displayFormU3v.showImage(0);
                displayFormU3v.Show();
            }

            if (index == 1)
            {
                IKDeviceInfo pInfo = m_listDeviceInfo.Find(p => p.nType == IKDeviceType.DEVICE_CML);

                displayFormCL = new DeviceDisplayForm(devU3v, pInfo.nBoardIndex)
                {
                    m_nMaxHeight = 400,
                    m_nMaxWidth = 400,
                    Text = pInfo.sDevName,
                    Height = 400,
                    Width = 400
                };

                displayFormCL.TopLevel = true;
                displayFormCL.StartPosition = FormStartPosition.CenterScreen;
                displayFormCL.showImage(0);
                displayFormCL.Show();
            }
            
        }
        /// <summary>
        /// </summary>
        /// <param name="index">相机编号</param>
        public void CloseVideo(int index = 0)
        {
            if (displayFormU3v != null)
                displayFormU3v.Close();
            if (displayFormCL != null) 
                displayFormCL.Close();
        }
    }
}