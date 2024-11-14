namespace MVersionDeviceDemo
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxDevice = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonLoad = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveBmp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveTiff = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonProbe = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonGrabOnce = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonGrab = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStopGrab = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxDevice);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.Size = new System.Drawing.Size(851, 581);
            this.splitContainer1.SplitterDistance = 306;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBoxDevice
            // 
            this.listBoxDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxDevice.FormattingEnabled = true;
            this.listBoxDevice.ItemHeight = 15;
            this.listBoxDevice.Location = new System.Drawing.Point(0, 0);
            this.listBoxDevice.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxDevice.Name = "listBoxDevice";
            this.listBoxDevice.Size = new System.Drawing.Size(306, 581);
            this.listBoxDevice.TabIndex = 0;
            this.listBoxDevice.SelectedIndexChanged += new System.EventHandler(this.listBoxDevice_SelectedIndexChanged);
            this.listBoxDevice.DoubleClick += new System.EventHandler(this.listBoxDevice_DoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonLoad,
            this.toolStripButtonSaveBmp,
            this.toolStripButtonSaveTiff,
            this.toolStripButtonProbe,
            this.toolStripButtonGrabOnce,
            this.toolStripButtonGrab,
            this.toolStripButtonStopGrab});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(851, 31);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonLoad
            // 
            this.toolStripButtonLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLoad.Image = global::MVersionDeviceDemo.Properties.Resources.OpenFile;
            this.toolStripButtonLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLoad.Name = "toolStripButtonLoad";
            this.toolStripButtonLoad.Size = new System.Drawing.Size(29, 28);
            this.toolStripButtonLoad.Text = "toolStripButton1";
            this.toolStripButtonLoad.ToolTipText = "打开配置文件";
            this.toolStripButtonLoad.Click += new System.EventHandler(this.toolStripButtonLoad_Click);
            // 
            // toolStripButtonSaveBmp
            // 
            this.toolStripButtonSaveBmp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveBmp.Image = global::MVersionDeviceDemo.Properties.Resources.SaveB;
            this.toolStripButtonSaveBmp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveBmp.Name = "toolStripButtonSaveBmp";
            this.toolStripButtonSaveBmp.Size = new System.Drawing.Size(29, 28);
            this.toolStripButtonSaveBmp.Text = "toolStripButton2";
            this.toolStripButtonSaveBmp.ToolTipText = "保存bmp";
            this.toolStripButtonSaveBmp.Click += new System.EventHandler(this.toolStripButtonSaveBmp_Click);
            // 
            // toolStripButtonSaveTiff
            // 
            this.toolStripButtonSaveTiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveTiff.Image = global::MVersionDeviceDemo.Properties.Resources.SaveT;
            this.toolStripButtonSaveTiff.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveTiff.Name = "toolStripButtonSaveTiff";
            this.toolStripButtonSaveTiff.Size = new System.Drawing.Size(29, 28);
            this.toolStripButtonSaveTiff.Text = "toolStripButton3";
            this.toolStripButtonSaveTiff.ToolTipText = "保存tiff";
            this.toolStripButtonSaveTiff.Click += new System.EventHandler(this.toolStripButtonSaveTiff_Click);
            // 
            // toolStripButtonProbe
            // 
            this.toolStripButtonProbe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonProbe.Image = global::MVersionDeviceDemo.Properties.Resources.DetectDevice;
            this.toolStripButtonProbe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonProbe.Name = "toolStripButtonProbe";
            this.toolStripButtonProbe.Size = new System.Drawing.Size(29, 28);
            this.toolStripButtonProbe.Text = "toolStripButton4";
            this.toolStripButtonProbe.ToolTipText = "扫描设备";
            this.toolStripButtonProbe.Click += new System.EventHandler(this.toolStripButtonProbe_Click);
            // 
            // toolStripButtonGrabOnce
            // 
            this.toolStripButtonGrabOnce.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGrabOnce.Image = global::MVersionDeviceDemo.Properties.Resources.GrabOne;
            this.toolStripButtonGrabOnce.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGrabOnce.Name = "toolStripButtonGrabOnce";
            this.toolStripButtonGrabOnce.Size = new System.Drawing.Size(29, 28);
            this.toolStripButtonGrabOnce.Text = "toolStripButton5";
            this.toolStripButtonGrabOnce.ToolTipText = "单帧采集";
            this.toolStripButtonGrabOnce.Click += new System.EventHandler(this.toolStripButtonGrabOnce_Click);
            // 
            // toolStripButtonGrab
            // 
            this.toolStripButtonGrab.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGrab.Image = global::MVersionDeviceDemo.Properties.Resources.GrabContinuous;
            this.toolStripButtonGrab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGrab.Name = "toolStripButtonGrab";
            this.toolStripButtonGrab.Size = new System.Drawing.Size(29, 28);
            this.toolStripButtonGrab.Text = "toolStripButton6";
            this.toolStripButtonGrab.ToolTipText = "连续采集";
            this.toolStripButtonGrab.Click += new System.EventHandler(this.toolStripButtonGrab_Click);
            // 
            // toolStripButtonStopGrab
            // 
            this.toolStripButtonStopGrab.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStopGrab.Image = global::MVersionDeviceDemo.Properties.Resources.GrabStop;
            this.toolStripButtonStopGrab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStopGrab.Name = "toolStripButtonStopGrab";
            this.toolStripButtonStopGrab.Size = new System.Drawing.Size(29, 28);
            this.toolStripButtonStopGrab.Text = "toolStripButtonStopGrab";
            this.toolStripButtonStopGrab.ToolTipText = "停止采集";
            this.toolStripButtonStopGrab.Click += new System.EventHandler(this.toolStripButtonStopGrab_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 581);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "MVDemo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoad;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveBmp;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveTiff;
        private System.Windows.Forms.ToolStripButton toolStripButtonProbe;
        private System.Windows.Forms.ToolStripButton toolStripButtonGrabOnce;
        private System.Windows.Forms.ToolStripButton toolStripButtonGrab;
        private System.Windows.Forms.ToolStripButton toolStripButtonStopGrab;
        private System.Windows.Forms.ListBox listBoxDevice;
    }
}

