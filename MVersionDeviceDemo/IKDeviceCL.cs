using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using IKapC.NET;
using IKapBoardClassLibrary;
using System.Threading;

namespace MVersionDeviceDemo
{
    class IKDeviceCL : IKDevice
    {
        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        //  回调函数
        #region Callback
        delegate void IKapCallBackProc(IntPtr pParam);
        private IKapCallBackProc OnGrabStartProc;
        private IKapCallBackProc OnFrameLostProc;
        private IKapCallBackProc OnTimeoutProc;
        private IKapCallBackProc OnFrameReadyProc;
        private IKapCallBackProc OnGrabStopProc;
        #endregion

        public IKDeviceCL()
        {
            m_nType = 1;
        }

        public override bool openDevice(int nDevIndex, int nBoardIndex)
        {
            closeDevice();
            uint res = IKapCLib.ItkDevOpen((uint)nDevIndex
                , (int)(ItkDeviceAccessMode.ITKDEV_VAL_ACCESS_MODE_CONTROL)
                , ref m_pDev);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Camera error:Open camera failed");
                return false;
            }
            m_nDevIndex = nDevIndex;
            //打开采集卡
            m_pBoard = IKapBoard.IKapOpen((uint)BoardType.IKBoardPCIE, (uint)nBoardIndex);
            m_nBoardIndex = nBoardIndex;
            if (m_pBoard == new IntPtr(-1))
                return false;
            return true;
        }

        public override bool isOpen()
        {
            return m_pDev != new IntPtr(-1) && m_pBoard != new IntPtr(-1);
        }

        public override bool closeDevice()
        {
            if(isOpen())
            {
                IKapBoard.IKapClose(m_pBoard);
                IKapCLib.ItkDevClose(m_pDev);
            }
            return true;
        }

        public override bool loadConfiguration(string sFilePath)
        {
            int ret = IKapBoard.IKapLoadConfigurationFromFile(m_pBoard, sFilePath);
            return ret == (int)ErrorCode.IK_RTN_OK;
        }

        public override bool createBuffer()
        {
            int ret = (int)ErrorCode.IK_RTN_OK;
            int nImageType = 0;
            ret = IKapBoard.IKapGetInfo(m_pBoard, (uint)INFO_ID.IKP_IMAGE_WIDTH, ref m_nWidth);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;
            ret = IKapBoard.IKapGetInfo(m_pBoard, (uint)INFO_ID.IKP_IMAGE_HEIGHT, ref m_nHeight);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;
            ret = IKapBoard.IKapGetInfo(m_pBoard, (uint)INFO_ID.IKP_IMAGE_TYPE, ref nImageType);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;
            ret = IKapBoard.IKapGetInfo(m_pBoard, (uint)INFO_ID.IKP_DATA_FORMAT, ref m_nDepth);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;
            ret = IKapBoard.IKapGetInfo(m_pBoard, (uint)INFO_ID.IKP_FRAME_SIZE, ref m_nBufferSize);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;
            switch(nImageType)
            {
                case 0:
                    m_nChannels = 1;
                    break;
                case 1:
                case 3:
                    m_nChannels = 3;
                    break;
                case 2:
                case 4:
                    m_nChannels = 4;
                    break;
            }
            m_pUserBuffer = Marshal.AllocHGlobal(m_nBufferSize);
            return true;
        }

        public override void clearBuffer()
        {
            if (m_pUserBuffer == new IntPtr(-1))
                return;
            Marshal.FreeHGlobal(m_pUserBuffer);
            m_pUserBuffer = new IntPtr(-1);
        }

        public override bool startGrab(int nCount)
        {
            // 设置帧超时时间
            int timeout = -1;
            int ret = IKapBoard.IKapSetInfo(m_pBoard, (uint)INFO_ID.IKP_TIME_OUT, timeout);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;

            // 设置抓取模式，IKP_GRAB_NON_BLOCK为非阻塞模式
            int grab_mode = (int)GrabMode.IKP_GRAB_NON_BLOCK;
            ret = IKapBoard.IKapSetInfo(m_pBoard, (uint)INFO_ID.IKP_GRAB_MODE, grab_mode);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;

            // 设置帧传输模式，IKP_FRAME_TRANSFER_SYNCHRONOUS_NEXT_EMPTY_WITH_PROTECT为同步保存模式
            int transfer_mode = (int)FrameTransferMode.IKP_FRAME_TRANSFER_SYNCHRONOUS_NEXT_EMPTY_WITH_PROTECT;
            ret = IKapBoard.IKapSetInfo(m_pBoard, (uint)INFO_ID.IKP_FRAME_TRANSFER_MODE, transfer_mode);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;

            // 设置缓冲区格式
            ret = IKapBoard.IKapSetInfo(m_pBoard, (uint)INFO_ID.IKP_FRAME_COUNT, m_nFrameCount);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;
            
            // 注册回调函数
            IntPtr hPtr = new IntPtr(-1);
            OnGrabStartProc = new IKapCallBackProc(OnGrabStartFunc);
            ret = IKapBoard.IKapRegisterCallback(m_pBoard,(uint)CallBackEvents.IKEvent_GrabStart, Marshal.GetFunctionPointerForDelegate(OnGrabStartProc), hPtr);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;

            OnFrameReadyProc = new IKapCallBackProc(OnFrameReadyFunc);
            ret = IKapBoard.IKapRegisterCallback(m_pBoard, (uint)CallBackEvents.IKEvent_FrameReady, Marshal.GetFunctionPointerForDelegate(OnFrameReadyProc), hPtr);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;

            OnFrameLostProc = new IKapCallBackProc(OnFrameLostFunc);
            ret = IKapBoard.IKapRegisterCallback(m_pBoard, (uint)CallBackEvents.IKEvent_FrameLost, Marshal.GetFunctionPointerForDelegate(OnFrameLostProc), hPtr);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;

            OnTimeoutProc = new IKapCallBackProc(OnTimeoutFunc);
            ret = IKapBoard.IKapRegisterCallback(m_pBoard, (uint)CallBackEvents.IKEvent_TimeOut, Marshal.GetFunctionPointerForDelegate(OnTimeoutProc), hPtr);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;

            OnGrabStopProc = new IKapCallBackProc(OnGrabStopFunc);
            ret = IKapBoard.IKapRegisterCallback(m_pBoard, (uint)CallBackEvents.IKEvent_GrabStop, Marshal.GetFunctionPointerForDelegate(OnGrabStopProc), hPtr);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;

            m_bUpdateImage = false;
            m_nCurFrameIndex = 0;
            ret = IKapBoard.IKapStartGrab(m_pBoard, nCount);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;
            m_bGrabingImage = true;
            return true;
        }

        public override bool stopGrab()
        {
            IKapBoard.IKapStopGrab(m_pBoard);
            do
            {
            } while (m_bGrabingImage);
            return true;
        }

        #region Callback
        // 开始抓帧回调
        public void OnGrabStartFunc(IntPtr pParam)
        {
            Console.WriteLine("Start grabbing image");
        }
        // 丢帧回调
        public void OnFrameLostFunc(IntPtr pParam)
        {
            Console.WriteLine("Frame lost");
        }
        // 帧超时回调
        public void OnTimeoutFunc(IntPtr pParam)
        {
            Console.WriteLine("Grab image timeout");
        }
        // 一帧图像完成回调
        public void OnFrameReadyFunc(IntPtr pParam)
        {
            IntPtr hPtr = new IntPtr(-1);
            // 获取当前帧状态
            IKapBoard.IKAPBUFFERSTATUS status = new IKapBoard.IKAPBUFFERSTATUS();
            IKapBoard.IKapGetBufferStatus(m_pBoard, m_nCurFrameIndex, ref status);
            if(status.uFull == 1)
            {
                IKapBoard.IKapGetBufferAddress(m_pBoard, m_nCurFrameIndex, ref hPtr);
                lock(m_mutexImage)
                {
                    CopyMemory(m_pUserBuffer, hPtr, m_nBufferSize);
                    m_bUpdateImage = true;
                }
            }
            m_nCurFrameIndex++;
            m_nCurFrameIndex = m_nCurFrameIndex % m_nFrameCount;
        }
        // 停止抓取图像回调
        public void OnGrabStopFunc(IntPtr pParam)
        {
            Console.WriteLine("Stop grabbing image");  
            m_bGrabingImage = false;
        }
        #endregion
    }
}
