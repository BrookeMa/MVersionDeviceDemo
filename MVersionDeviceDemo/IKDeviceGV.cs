using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using IKapC.NET;
namespace MVersionDeviceDemo
{
    public class IKDeviceGV : IKDevice
    {
        // 采集流句柄
        public IntPtr m_pStream = new IntPtr(-1);
        // 缓冲区列表
        public List<IntPtr> m_listBuffer = new List<IntPtr>();
        // 像素格式
        uint m_uPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB888;

        #region Callback Declare
        public IKapCLib.PITKSTREAMCALLBACK cbOnStartOfStreamProc = null;
        public IKapCLib.PITKSTREAMCALLBACK cbOnEndOfFrameProc = null;
        public IKapCLib.PITKSTREAMCALLBACK cbOnTimeOutProc = null;
        public IKapCLib.PITKSTREAMCALLBACK cbOnFrameLostProc = null;
        public IKapCLib.PITKSTREAMCALLBACK cbOnEndOfStreamProc = null;
        #endregion

        public IKDeviceGV()
        {
            m_nType = 0;
        }

        public override bool openDevice(int nDevIndex, int nBoardIndex)
        {
            closeDevice();
            uint res = IKapCLib.ItkDevOpen( (uint)nDevIndex
                , (int)(ItkDeviceAccessMode.ITKDEV_VAL_ACCESS_MODE_CONTROL)
                , ref m_pDev );
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Camera error:Open camera failed");
                return false;
            }
            m_nDevIndex = nDevIndex;
            return true;
        }

        public override bool closeDevice()
        {
            uint res = IKapCLib.ItkDevClose(m_pDev);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Camera error:Close camera failed");
                return false;
            }
            return true;
        }

        public override bool isOpen()
        {
            if (m_pDev == new IntPtr(-1))
                return false;
            return true;
        }

        public override bool loadConfiguration(string sFilePath)
        {
            return base.loadConfiguration(sFilePath);
        }

        /*
         *@brief:获取相机图片格式
         *@param [in]:
         *@return:相机图片格式
         */
        public uint getPixelFormat()
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
            }
            else if (sPixelFormat.ToString() == "BayerRG8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG8;
                m_nDepth = 8;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGB8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB8;
                m_nDepth = 8;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerBG8")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG8;
                m_nDepth = 8;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGR10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR10;
                m_nDepth = 10;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerRG10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG10;
                m_nDepth = 10;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGB10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB10;
                m_nDepth = 10;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerBG10")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG10;
                m_nDepth = 10;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGR12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR12;
                m_nDepth = 12;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerRG12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG12;
                m_nDepth = 12;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGB12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB12;
                m_nDepth = 12;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerBG12")
            {
                nPixelFormat = (uint)ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG12;
                m_nDepth = 12;
                m_nChannels = 1;
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
         * @brief:申请相机采集流，相机缓冲区以及用户缓冲区
         * @return:是否申请成功
         */
        public bool createStreamAndBuffer()
        {
            bool bReturn = false;
            long nWidth = 0, nHeight = 0;
            uint res = IKapCLib.ItkDevGetInt64(m_pDev, "Width", ref nWidth);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Camera error:Get frame width failed");
                return false;
            }
            m_nWidth = (int)nWidth;
            res = IKapCLib.ItkDevGetInt64(m_pDev, "Height", ref nHeight);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Camera error:Get frame height failed");
                return false;
            }
            m_nHeight = (int)nHeight;
            m_uPixelFormat = getPixelFormat();
            if (m_uPixelFormat == 0)
                return bReturn;
            //创建第一个缓冲区，设置为数据流默认缓冲区
            IntPtr hBuffer = new IntPtr(-1);
            res = IKapCLib.ItkBufferNew(nWidth, nHeight, m_uPixelFormat, ref hBuffer);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Init stream error:Create first buffer failed");
                return false;
            }
            m_listBuffer.Add(hBuffer);
            res = IKapCLib.ItkDevAllocStream(m_pDev, 0, hBuffer, ref m_pStream);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Init stream error:Allocate buffer failed");
                return false;
            }
            //根据缓冲区个数创建剩余缓冲区并添加进数据流中
            for (int i = 1; i < m_nFrameCount; i++)
            {
                res = IKapCLib.ItkBufferNew(nWidth, nHeight, m_uPixelFormat, ref hBuffer);
                if (!Check(res))
                {
                    System.Diagnostics.Debug.WriteLine("Init stream error:Create new buffer failed");
                    return false;
                }
                res = IKapCLib.ItkStreamAddBuffer(m_pStream, hBuffer);
                if (!Check(res))
                {
                    System.Diagnostics.Debug.WriteLine("Init stream error:Add new buffer to stream failed");
                    return false;
                }
                m_listBuffer.Add(hBuffer);
            }
            //获取buffer的大小，以此创建用户缓冲区
            uint nBuffersz = 0;
            IntPtr pBuffersz = Marshal.AllocHGlobal(8);
            uint prm = (uint)ItkBufferPrm.ITKBUFFER_PRM_SIZE;
            res = IKapCLib.ItkBufferGetPrm(m_listBuffer[0], prm, pBuffersz);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Buffer size error:Get buffer size failed");
                return false;
            }
            nBuffersz = (uint)Marshal.ReadInt64(pBuffersz);
            Marshal.FreeHGlobal(pBuffersz);
            m_pUserBuffer = Marshal.AllocHGlobal((int)nBuffersz);
            m_nBufferSize = (int)nBuffersz;
            return true;
        }

        public override bool createBuffer()
        {
            return createStreamAndBuffer();
        }

        public override void clearBuffer()
        {
            clearStreamAndBuffer();
        }

        public override bool startGrab(int nCount)
        {
            uint res = (uint)ItkStatusErrorId.ITKSTATUS_OK;
            //相机传输模式
            IntPtr xferMode = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(xferMode, 0, (int)ItkStreamTransferMode.ITKSTREAM_VAL_TRANSFER_MODE_SYNCHRONOUS_WITH_PROTECT);
            //采集流采集模式
            IntPtr startMode = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(startMode, 0, (int)ItkStreamStartMode.ITKSTREAM_VAL_START_MODE_NON_BLOCK);
            //采集超时时间
            IntPtr timeOut = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(timeOut, 0, (int)IKapCLib.ITKSTREAM_CONTINUOUS);

            //设置采集模式
            res = IKapCLib.ItkStreamSetPrm(m_pStream, (uint)ItkStreamPrm.ITKSTREAM_PRM_START_MODE, startMode);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Configure stream error:Set start mode failed");
                Marshal.FreeHGlobal(startMode);
                return false;
            }
            //设置超时时间
            res = IKapCLib.ItkStreamSetPrm(m_pStream, (uint)ItkStreamPrm.ITKSTREAM_PRM_TIME_OUT, timeOut);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Configure stream error:Set time out failed");
                Marshal.FreeHGlobal(timeOut);
                return false;
            }
            //设置传输模式
            res = IKapCLib.ItkStreamSetPrm(m_pStream, (uint)ItkStreamPrm.ITKSTREAM_PRM_TRANSFER_MODE, xferMode);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Configure stream error:Set transfer mode failed");
                Marshal.FreeHGlobal(xferMode);
                return false;
            }
            Marshal.FreeHGlobal(xferMode);
            Marshal.FreeHGlobal(startMode);
            Marshal.FreeHGlobal(timeOut);

            //注册采集开始回调
            IntPtr hPtr = new IntPtr(-1);
            cbOnStartOfStreamProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnStartOfStreamFunc);
            res = IKapCLib.ItkStreamRegisterCallback(m_pStream,(uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_START_OF_STREAM, cbOnStartOfStreamProc, hPtr);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Configure stream error:Register callback failed");
                return false;
            }
            //注册采集超时回调
            cbOnTimeOutProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnTimeOutFunc);
            res = IKapCLib.ItkStreamRegisterCallback(m_pStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_TIME_OUT, cbOnTimeOutProc, hPtr);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Configure stream error:Register callback failed");
                return false;
            }
            //注册采集丢帧回调
            cbOnFrameLostProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnFrameLostFunc);
            res = IKapCLib.ItkStreamRegisterCallback(m_pStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_FRAME_LOST, cbOnFrameLostProc, hPtr);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Configure stream error:Register callback failed");
                return false;
            }
            //注册采集结束回调
            cbOnEndOfStreamProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnEndOfStreamFunc);
            res = IKapCLib.ItkStreamRegisterCallback(m_pStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_STREAM, cbOnEndOfStreamProc, hPtr);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Configure stream error:Register callback failed");
                return false;
            }
            //注册帧结束回调
            cbOnEndOfFrameProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnEndOfFrameFunc);
            res = IKapCLib.ItkStreamRegisterCallback(m_pStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_FRAME, cbOnEndOfFrameProc, hPtr);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Configure stream error:Register callback failed");
                return false;
            }

            //开始采集
            m_bUpdateImage = false;
            m_nCurFrameIndex = 0;
            uint nGrabCount = (uint)nCount;
            //连续采集
            if(nCount == 0)
                nGrabCount = (uint)IKapCLib.ITKSTREAM_CONTINUOUS;
            res = IKapCLib.ItkStreamStart(m_pStream, nGrabCount);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Start grab error:Start stream failed");
                return false;
            }
            m_bGrabingImage = true;
            return true;
        }

        public override bool stopGrab()
        {
            uint res = IKapCLib.ItkStreamStop(m_pStream);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Stop grab error:Stop stream failed");
                return false;
            }
            do
            {
            } while (m_bGrabingImage);

            //注销回调函数
            IKapCLib.ItkStreamUnregisterCallback(m_pStream,(uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_START_OF_STREAM);
            IKapCLib.ItkStreamUnregisterCallback(m_pStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_STREAM);
            IKapCLib.ItkStreamUnregisterCallback(m_pStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_FRAME_LOST);
            IKapCLib.ItkStreamUnregisterCallback(m_pStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_TIME_OUT);
            IKapCLib.ItkStreamUnregisterCallback(m_pStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_FRAME);
            return true;
        }

        /*
         * @brief: 清除申请的缓冲区和数据流
         * @return:
         */
        public void clearStreamAndBuffer()
        {
            if (m_pUserBuffer == new IntPtr(-1))
                return;
            for (int i = 0; i < m_nFrameCount; i++)
            {
                IKapCLib.ItkStreamRemoveBuffer(m_pStream, m_listBuffer[i]);
                IKapCLib.ItkBufferFree(m_listBuffer[i]);
            }
            IKapCLib.ItkDevFreeStream(m_pStream);
            m_listBuffer.Clear();
            Marshal.FreeHGlobal(m_pUserBuffer);
            m_pUserBuffer = new IntPtr(-1);
        }

        //回调函数定义
        #region Callback Define
        public void cbOnStartOfStreamFunc(uint eventType, IntPtr pContext)
        {
            System.Diagnostics.Debug.WriteLine("Stream start");
        }
        public void cbOnTimeOutFunc(uint eventType, IntPtr pContext)
        {
            System.Diagnostics.Debug.WriteLine("Timeout");
        }
        public void cbOnFrameLostFunc(uint eventType, IntPtr pContext)
        {
            System.Diagnostics.Debug.WriteLine("Frame lost");
        }
        public void cbOnEndOfStreamFunc(uint eventType, IntPtr pContext)
        {
            System.Diagnostics.Debug.WriteLine("Stream end");
            m_bGrabingImage = false;
        }
        public void cbOnEndOfFrameFunc(uint eventType, IntPtr pContext)
        {
            //获取buffer状态
            IntPtr bufferStatus = Marshal.AllocHGlobal(4);
            uint status = 0;
            uint res = IKapCLib.ItkBufferGetPrm(m_listBuffer[m_nCurFrameIndex], (uint)ItkBufferPrm.ITKBUFFER_PRM_STATE, bufferStatus);
            if (!Check(res))
            {
                System.Diagnostics.Debug.WriteLine("Read data error:Get buffer status failed");
                return;
            }
            //buffer状态为full时进行读取
            status = (uint)Marshal.ReadInt32(bufferStatus);
            if (status != (uint)ItkBufferState.ITKBUFFER_VAL_STATE_FULL)
            {
                System.Diagnostics.Debug.WriteLine("Read data error:Buffer is not full");
                return;
            }
            Marshal.FreeHGlobal(bufferStatus);
            lock (m_mutexImage)
            {
                IKapCLib.ItkBufferRead(m_listBuffer[m_nCurFrameIndex], 0, m_pUserBuffer, (uint)m_nBufferSize);
                m_bUpdateImage = true;
            }
            m_nCurFrameIndex++;
            m_nCurFrameIndex = m_nCurFrameIndex % m_nFrameCount;
        }
        #endregion
    }
}
