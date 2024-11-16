using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MVersionDeviceDemo {
    public class CameraManager
    {

        // 执行Grab后保存的图片
        public Image imageU3V;
        public Image imageCL;

        // 标志位，表示是否有单帧抓取请求
        public volatile bool m_bGrabOnceRequestedU3V = false;
        public volatile bool m_bGrabOnceRequestedCL = false;

        private static readonly CameraManager instance = new CameraManager();

        // 私有构造函数
        private CameraManager()
        {
            // 初始化相机资源
        }

        // 静态构造函数，CLR 确保类型只初始化一次
        static CameraManager()
        {
        }

        public static CameraManager Instance
        {
            get
            {
                return instance;
            }
        }

        // CameraManager 的其他方法
    }
}

