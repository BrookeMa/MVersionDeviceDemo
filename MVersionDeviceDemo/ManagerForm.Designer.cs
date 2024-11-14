namespace MVersionDeviceDemo
{
    partial class ManagerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.Close = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.Dectect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(74, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(154, 77);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Close
            // 
            this.Close.Location = new System.Drawing.Point(267, 31);
            this.Close.Name = "Close";
            this.Close.Size = new System.Drawing.Size(145, 77);
            this.Close.TabIndex = 1;
            this.Close.Text = "Close";
            this.Close.UseVisualStyleBackColor = true;
            this.Close.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(74, 126);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(154, 71);
            this.button3.TabIndex = 2;
            this.button3.Text = "Grab";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(267, 126);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(145, 71);
            this.button4.TabIndex = 3;
            this.button4.Text = "Save";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(74, 220);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(154, 78);
            this.button5.TabIndex = 4;
            this.button5.Text = "Show Video";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(267, 220);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(145, 78);
            this.button6.TabIndex = 5;
            this.button6.Text = "Close Video";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(117, 328);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(18, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(74, 318);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(154, 74);
            this.button7.TabIndex = 7;
            this.button7.Text = "Show Video 2";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(267, 321);
            this.button8.Name = "button8";
            this.button8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button8.Size = new System.Drawing.Size(145, 71);
            this.button8.TabIndex = 8;
            this.button8.Text = "Close Video 2";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // Dectect
            // 
            this.Dectect.Location = new System.Drawing.Point(474, 31);
            this.Dectect.Name = "Dectect";
            this.Dectect.Size = new System.Drawing.Size(173, 109);
            this.Dectect.TabIndex = 9;
            this.Dectect.Text = "button9";
            this.Dectect.UseVisualStyleBackColor = true;
            this.Dectect.Click += new System.EventHandler(this.Dectect_Click);
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 404);
            this.Controls.Add(this.Dectect);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.Close);
            this.Controls.Add(this.button1);
            this.Name = "ManagerForm";
            this.Text = "ManagerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Close;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button Dectect;
    }
}