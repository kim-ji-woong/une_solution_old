namespace WindowsApplication1
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.b_open = new System.Windows.Forms.Button();
            this.b_close = new System.Windows.Forms.Button();
            this.b_exit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.port1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ip1 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.port2 = new System.Windows.Forms.TextBox();
            this.ip2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tp1 = new System.Windows.Forms.TextBox();
            this.tp2 = new System.Windows.Forms.TextBox();
            this.rp1 = new System.Windows.Forms.TextBox();
            this.rp2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // b_open
            // 
            this.b_open.Location = new System.Drawing.Point(52, 393);
            this.b_open.Name = "b_open";
            this.b_open.Size = new System.Drawing.Size(94, 25);
            this.b_open.TabIndex = 0;
            this.b_open.Text = "Open";
            this.b_open.UseVisualStyleBackColor = true;
            this.b_open.Click += new System.EventHandler(this.b_open_Click);
            // 
            // b_close
            // 
            this.b_close.Enabled = false;
            this.b_close.Location = new System.Drawing.Point(233, 393);
            this.b_close.Name = "b_close";
            this.b_close.Size = new System.Drawing.Size(94, 23);
            this.b_close.TabIndex = 1;
            this.b_close.Text = "Close";
            this.b_close.UseVisualStyleBackColor = true;
            this.b_close.Click += new System.EventHandler(this.b_close_Click);
            // 
            // b_exit
            // 
            this.b_exit.Location = new System.Drawing.Point(411, 393);
            this.b_exit.Name = "b_exit";
            this.b_exit.Size = new System.Drawing.Size(100, 21);
            this.b_exit.TabIndex = 2;
            this.b_exit.Text = "Exit";
            this.b_exit.UseVisualStyleBackColor = true;
            this.b_exit.Click += new System.EventHandler(this.b_exit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.port1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ip1);
            this.groupBox1.Location = new System.Drawing.Point(24, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(233, 88);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "One";
            // 
            // port1
            // 
            this.port1.Location = new System.Drawing.Point(103, 54);
            this.port1.Name = "port1";
            this.port1.Size = new System.Drawing.Size(40, 21);
            this.port1.TabIndex = 3;
            this.port1.Text = "1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port Number:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server IP:";
            // 
            // ip1
            // 
            this.ip1.Location = new System.Drawing.Point(101, 21);
            this.ip1.Name = "ip1";
            this.ip1.Size = new System.Drawing.Size(110, 21);
            this.ip1.TabIndex = 0;
            this.ip1.Text = "172.18.101.111";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.port2);
            this.groupBox2.Controls.Add(this.ip2);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(279, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(233, 88);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Two";
            // 
            // port2
            // 
            this.port2.Location = new System.Drawing.Point(105, 52);
            this.port2.Name = "port2";
            this.port2.Size = new System.Drawing.Size(40, 21);
            this.port2.TabIndex = 3;
            this.port2.Text = "2";
            // 
            // ip2
            // 
            this.ip2.Location = new System.Drawing.Point(106, 18);
            this.ip2.Name = "ip2";
            this.ip2.Size = new System.Drawing.Size(110, 21);
            this.ip2.TabIndex = 2;
            this.ip2.Text = "172.18.101.112";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Port Number:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Server IP:";
            // 
            // tp1
            // 
            this.tp1.Enabled = false;
            this.tp1.Location = new System.Drawing.Point(27, 147);
            this.tp1.Multiline = true;
            this.tp1.Name = "tp1";
            this.tp1.Size = new System.Drawing.Size(229, 100);
            this.tp1.TabIndex = 5;
            this.tp1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tp1_KeyPress);
            // 
            // tp2
            // 
            this.tp2.Enabled = false;
            this.tp2.Location = new System.Drawing.Point(282, 147);
            this.tp2.Multiline = true;
            this.tp2.Name = "tp2";
            this.tp2.Size = new System.Drawing.Size(228, 100);
            this.tp2.TabIndex = 6;
            this.tp2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tp2_KeyPress);
            // 
            // rp1
            // 
            this.rp1.Enabled = false;
            this.rp1.Location = new System.Drawing.Point(27, 276);
            this.rp1.Multiline = true;
            this.rp1.Name = "rp1";
            this.rp1.Size = new System.Drawing.Size(233, 102);
            this.rp1.TabIndex = 7;
            // 
            // rp2
            // 
            this.rp2.Enabled = false;
            this.rp2.Location = new System.Drawing.Point(285, 277);
            this.rp2.Multiline = true;
            this.rp2.Name = "rp2";
            this.rp2.Size = new System.Drawing.Size(226, 101);
            this.rp2.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "One Transmitter";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(285, 132);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "Two Transmitter";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(29, 259);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "One Receiver";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(283, 259);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "Two Receiver";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 430);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.rp2);
            this.Controls.Add(this.rp1);
            this.Controls.Add(this.tp2);
            this.Controls.Add(this.tp1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.b_exit);
            this.Controls.Add(this.b_close);
            this.Controls.Add(this.b_open);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multi-port test";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button b_open;
        private System.Windows.Forms.Button b_close;
        private System.Windows.Forms.Button b_exit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox ip1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox port1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox port2;
        private System.Windows.Forms.TextBox ip2;
        private System.Windows.Forms.TextBox tp1;
        private System.Windows.Forms.TextBox tp2;
        private System.Windows.Forms.TextBox rp1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox rp2;
    }
}

