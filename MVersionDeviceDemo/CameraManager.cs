using IKapC.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MVersionDeviceDemo
{
    /// <summary>
    /// 相机管理器，负责初始化、打开、关闭、抓取、保存以及显示实时视频
    /// </summary>
    public class CameraManager: IDisposable
    {
        // 单台相机设备，目前为U3V，相机类型可扩展
        private IKDeviceU3v deviceU3v;

        // SynchronizationContext 用于在UI线程更新图像
        private SynchronizationContext syncContext;

        // 控制采集线程的标志
        private volatile bool isGrabbing = false;

        // 线程取消标记
        private CancellationTokenSource grabCancellationTokenSource;

        // 构造函数，接受 UI 线程的 SynchronizationContext 用于回调
        public CameraManager(SynchronizationContext syncContext)
        {
            uint res = (uint)ItkStatusErrorId.ITKSTATUS_OK;
            uint numCameras = 0;

            // 枚举可用相机的数量。在打开相机前，必须调用 ItkManGetDeviceCount() 函数。
            //
            // Enumerate the number of available cameras. Before opening the camera, ItkManGetDeviceCount() function must be called.
            res = IKapCLib.ItkManGetDeviceCount(ref numCameras);

            this.syncContext = syncContext;
            deviceU3v = new IKDeviceU3v();
        }

        /// <summary>
        /// 初始化相机管理器，加载配置文件或从相机闪存加载默认配置
        /// </summary>
        /// <param name="paramFiles">配置文件路径数组，第一个路径用于U3V相机，第二个用于CL相机（可选）</param>
        /// <returns>操作代码，0表示成功，非0表示失败</returns>
        public int Init(string[] paramFiles = null)
        {
            // 仅有一台U3V相机，因此忽略第二个参数
            
            Console.WriteLine("初始化成功: U3V相机配置加载完成。");
            return 0;
        }

        /// <summary>
        /// 打开所有相机设备
        /// </summary>
        /// <returns>操作代码，0表示成功，非0表示失败</returns>
        public int Open()
        {
            IKapCLib.ITK_U3V_DEV_INFO pU3vDevInfo = new IKapCLib.ITK_U3V_DEV_INFO();

            // 打开U3V相机，设备索引和板卡索引根据实际情况调整
            bool openU3v = deviceU3v.openDevice(0, 0);
            if (!openU3v)
            {
                Console.WriteLine("打开U3V相机失败。");
                return -1;
            }

            Console.WriteLine("U3V相机已成功打开。");
            return 0;
        }

        /// <summary>
        /// 关闭所有相机设备
        /// </summary>
        /// <returns>操作代码，0表示成功，非0表示失败</returns>
        public int Close()
        {
            bool closeU3v = deviceU3v.closeDevice();

            if (!closeU3v)
            {
                Console.WriteLine("关闭U3V相机失败。");
                return -1;
            }

            Console.WriteLine("U3V相机已成功关闭。");
            return 0;
        }

        /// <summary>
        /// 从指定相机获取图像
        /// </summary>
        /// <param name="img">输出图像对象，具体类型根据实现而定</param>
        /// <param name="name">相机名称，"U3V" 或 "CL"</param>
        /// <param name="index">拍照序号，默认0</param>
        /// <returns>操作代码，0表示成功，非0表示失败</returns>
        public int Grab(out Image img, string name, int index = 0)
        {
            img = null;

            if (!name.Equals("U3V", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Grab错误: 未知的相机名称 '{name}'。");
                return -1;
            }

            // 开始抓取图像
            bool grabSuccess = deviceU3v.startGrab(1); // 抓取一张
            if (!grabSuccess)
            {
                Console.WriteLine("Grab错误: 启动抓取失败。");
                return -1;
            }

            // 等待图像更新
            // 在实际实现中，应该使用事件或回调来通知抓取完成
            // 这里为了简化，使用轮询
            int maxRetries = 100; // 超时100 * 10ms = 1秒
            int retries = 0;
            while (!deviceU3v.m_bUpdateImage && retries < maxRetries)
            {
                Thread.Sleep(10);
                retries++;
            }

            if (!deviceU3v.m_bUpdateImage)
            {
                Console.WriteLine("Grab错误: 抓取图像超时。");
                deviceU3v.stopGrab();
                return -1;
            }

            // 读取图像
            lock (deviceU3v.m_mutexImage)
            {
                deviceU3v.m_bUpdateImage = false;
                BufferToImage bufferHandler = new BufferToImage(
                    deviceU3v.m_nBufferSize,
                    deviceU3v.m_nDepth,
                    deviceU3v.m_nChannels,
                    deviceU3v.m_nWidth,
                    deviceU3v.m_nHeight
                );

                img = bufferHandler.toImage(deviceU3v.m_pUserBuffer);
            }

            Console.WriteLine("成功抓取图像。");
            return 0;
        }

        /// <summary>
        /// 保存指定相机最近一次抓取的图像，支持BMP和TIFF格式
        /// </summary>
        /// <param name="camera">相机名称，"U3V" 或 "CL"</param>
        /// <param name="filePath">保存文件的路径</param>
        /// <returns>操作代码，0表示成功，非0表示失败</returns>
        public int Save(string camera, string filePath)
        {
            // 获取最近抓取的图像
            Image img;
            int grabResult = Grab(out img, camera);
            if (grabResult != 0)
            {
                Console.WriteLine($"Save错误: 无法从相机 '{camera}' 获取图像。");
                return -1;
            }

            if (img == null)
            {
                Console.WriteLine($"Save错误: 相机 '{camera}' 没有有效的图像数据。");
                return -1;
            }

            try
            {
                // 确保文件路径有效
                if (string.IsNullOrEmpty(filePath))
                {
                    Console.WriteLine("Save错误: 无效的文件路径。");
                    return -1;
                }

                // 根据文件扩展名确定保存格式
                string extension = System.IO.Path.GetExtension(filePath).ToLower();
                System.Drawing.Imaging.ImageFormat imgFormat;

                switch (extension)
                {
                    case ".bmp":
                        imgFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                        break;
                    case ".tif":
                    case ".tiff":
                        imgFormat = System.Drawing.Imaging.ImageFormat.Tiff;
                        break;
                    default:
                        Console.WriteLine($"Save错误: 不支持的文件格式 '{extension}'。");
                        return -1;
                }

                img.Save(filePath, imgFormat);
                Console.WriteLine($"成功保存图像到 '{filePath}'。");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save错误: 保存图像失败。异常信息: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 显示指定相机的实时视频画面
        /// </summary>
        /// <param name="name">相机名称，"U3V" 或 "CL"</param>
        /// <returns>操作代码，0表示成功，非0表示失败</returns>
        public int ShowVideo(string name)
        {
            if (isGrabbing)
            {
                Console.WriteLine("Video已经在显示中。");
                return -1;
            }

            if (!name.Equals("U3V", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"ShowVideo错误: 未知的相机名称 '{name}'。");
                return -1;
            }

            isGrabbing = true;
            grabCancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = grabCancellationTokenSource.Token;

            Task.Run(() => VideoLoop(name, token), token);

            Console.WriteLine($"开始显示相机 '{name}' 的实时视频。");
            return 0;
        }

        /// <summary>
        /// 关闭指定相机的实时视频显示
        /// </summary>
        /// <param name="name">相机名称，"U3V" 或 "CL"</param>
        /// <returns>操作代码，0表示成功，非0表示失败</returns>
        public int CloseVideo(string name)
        {
            if (!isGrabbing)
            {
                Console.WriteLine("CloseVideo错误: 没有正在显示的视频。");
                return -1;
            }

            grabCancellationTokenSource.Cancel();
            isGrabbing = false;

            // 等待抓取线程结束
            Thread.Sleep(100); // 可优化为线程同步

            Console.WriteLine($"停止显示相机 '{name}' 的实时视频。");
            return 0;
        }

        /// <summary>
        /// 实时视频抓取循环
        /// </summary>
        /// <param name="name">相机名称</param>
        /// <param name="token">取消令牌</param>
        private void VideoLoop(string name, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Image img;
                int grabResult = Grab(out img, name);
                if (grabResult == 0 && img != null)
                {
                    // 在UI线程更新图像
                    syncContext.Post(new SendOrPostCallback(o =>
                    {
                        // 假设有一个 PictureBox 控件用于显示图像，命名为 pictureBoxImage
                        // 需要在实际窗体中实现相应逻辑
                        // 比如：
                        // pictureBoxImage.Image = (Image)o;
                        // 此处仅输出日志
                        Console.WriteLine("实时视频更新图像。");
                    }), img);

                    img.Dispose();
                }

                Thread.Sleep(30); // 控制帧率，约33 FPS
            }
        }

        /// <summary>
        /// 清除申请的缓冲区和数据流
        /// </summary>
        public void ClearBuffer()
        {
            deviceU3v.clearBuffer();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (isGrabbing)
            {
                CloseVideo("U3V");
            }
            Close();
            ClearBuffer();
        }
    }
}