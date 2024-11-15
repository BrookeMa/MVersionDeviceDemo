using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IKapC.NET;

namespace MVersionDeviceDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            IKapCLib.ItkManInitialize();
            //this.listBoxDevice.Height = this.splitContainer1.Panel1.Height - this.toolStrip1.Height - 1;
            this.toolStrip1.SendToBack();
        }

        // 设备类型枚举
        enum IKDeviceType
        {
            DEVICE_NIL = 0,
            DEVICE_CML,
            DEVICE_CXP,
            DEVICE_USB,
            DEVICE_GIGEVISION
        }

        // 设备信息结构体
        struct IKDeviceInfo
        {
            public IKDeviceType nType;
            public int nDevIndex;
            public int nBoardIndex;
            public string sDevName;
        }

        // 设备信息列表
        private List<IKDeviceInfo> m_listDeviceInfo = new List<IKDeviceInfo>();
        // 显示窗口列表
        private List<DeviceDisplayForm> m_listDisplayForm = new List<DeviceDisplayForm>();

        private void detectDevice()
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

        private void toolStripButtonLoad_Click(object sender, EventArgs e)
        {
            string vlcfFileName = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "vlcf文件(*.vlcf)|*.vlcf|所有文件(*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Title = "选择打开文件";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            vlcfFileName = ofd.FileName;
            int nIndex = this.listBoxDevice.SelectedIndex;
            for (int i = 0; i < m_listDisplayForm.Count; ++i)
            {
                if (m_listDisplayForm[i].m_nIndex == nIndex)
                    m_listDisplayForm[i].loadConfigure(vlcfFileName);
            }
        }

        private void toolStripButtonSaveBmp_Click(object sender, EventArgs e)
        {
            int nIndex = this.listBoxDevice.SelectedIndex;
            for (int i = 0; i < m_listDisplayForm.Count; ++i)
            {
                if (m_listDisplayForm[i].m_nIndex == nIndex)
                    m_listDisplayForm[i].saveImage("bmp");
            }
        }

        private void toolStripButtonSaveTiff_Click(object sender, EventArgs e)
        {
            int nIndex = this.listBoxDevice.SelectedIndex;
            for (int i = 0; i < m_listDisplayForm.Count; ++i)
            {
                if (m_listDisplayForm[i].m_nIndex == nIndex)
                    m_listDisplayForm[i].saveImage("tiff");
            }
        }

        private void toolStripButtonProbe_Click(object sender, EventArgs e)
        {
            detectDevice();
            this.listBoxDevice.Items.Clear();
            for(int i = 0; i < m_listDeviceInfo.Count; ++i)
            {
                this.listBoxDevice.Items.Add(m_listDeviceInfo[i].sDevName);
            }
        }

        private void toolStripButtonGrabOnce_Click(object sender, EventArgs e)
        {
            int nIndex = this.listBoxDevice.SelectedIndex;
            for (int i = 0; i < m_listDisplayForm.Count; ++i)
            {
                if (m_listDisplayForm[i].m_nIndex == nIndex)
                    m_listDisplayForm[i].showImage(1);
            }
        }

        private void toolStripButtonGrab_Click(object sender, EventArgs e)
        {
            int nIndex = this.listBoxDevice.SelectedIndex;
            for(int i = 0; i < m_listDisplayForm.Count; ++i)
            {
                if (m_listDisplayForm[i].m_nIndex == nIndex)
                    m_listDisplayForm[i].showImage(0);
            }
        }

        private void toolStripButtonStopGrab_Click(object sender, EventArgs e)
        {
            int nIndex = this.listBoxDevice.SelectedIndex;
            for (int i = 0; i < m_listDisplayForm.Count; ++i)
            {
                if (m_listDisplayForm[i].m_nIndex == nIndex)
                    m_listDisplayForm[i].stopGrab();
            }
        }

        private void listBoxDevice_DoubleClick(object sender, EventArgs e)
        {
            int nIndex = this.listBoxDevice.SelectedIndex;
            DeviceDisplayForm temp = null;
            int nAlreadyOpenedDeviceIdx = -1;
            for(int i = 0; i < m_listDisplayForm.Count; ++i)
            {
                if (m_listDisplayForm[i].m_nIndex == nIndex)
                {
                    temp = m_listDisplayForm[i];
                    nAlreadyOpenedDeviceIdx = i;
                }
            }
            IKDeviceInfo pInfo = m_listDeviceInfo[nIndex];
            IKDevice pDev = new IKDevice();
            switch(pInfo.nType)
            {
                case IKDeviceType.DEVICE_GIGEVISION:
                    pDev = new IKDeviceGV();
                    break;
                case IKDeviceType.DEVICE_USB:
                    pDev = new IKDeviceU3V();
                    break;
                case IKDeviceType.DEVICE_CML:
                    pDev = new IKDeviceCL();
                    break;
                case IKDeviceType.DEVICE_CXP:
                    pDev = new IKDeviceCXP();
                    break;
            }
            pDev.openDevice(pInfo.nDevIndex, pInfo.nBoardIndex);
            if (!pDev.isOpen())
                return;
            DeviceDisplayForm displayForm = new DeviceDisplayForm(pDev,nIndex);
            displayForm.TopLevel = false;
            this.splitContainer1.Panel2.Controls.Add(displayForm);
            int nCount = this.splitContainer1.Panel2.Controls.Count;
            if(nCount == 1)
            {
                displayForm.Location = new System.Drawing.Point(0, this.toolStrip1.Height);
            }
            else
            {
                Control c = this.splitContainer1.Panel2.Controls[nCount - 2];
                displayForm.Location = new System.Drawing.Point(0, c.Height + c.Location.Y + 1);
            }
            displayForm.m_nMaxHeight = this.splitContainer1.Panel2.Height - this.toolStrip1.Height-5;
            displayForm.m_nMaxWidth = this.splitContainer1.Panel2.Width;
            displayForm.Text = pInfo.sDevName;
            displayForm.Show();
            if(nAlreadyOpenedDeviceIdx != -1)
            {
                m_listDisplayForm[nAlreadyOpenedDeviceIdx] = displayForm;
                temp.Dispose();
            }
            else
            {
                m_listDisplayForm.Add(displayForm);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //this.listBoxDevice.Height = this.splitContainer1.Panel1.Height - this.toolStrip1.Height-1;
            for(int i = 0; i < m_listDisplayForm.Count; ++i)
            {
                m_listDisplayForm[i].m_nMaxHeight = this.splitContainer1.Panel2.Height - this.toolStrip1.Height-5;
                m_listDisplayForm[i].m_nMaxWidth = this.splitContainer1.Panel2.Width;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            for(int i = 0; i < m_listDisplayForm.Count; ++i)
            {
                m_listDisplayForm[i].Close();
            }
        }

        private void listBoxDevice_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
