namespace SOPMonitoringSystem.Popup
{
    partial class PopupSelectSOP
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
            this.panel7 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblSenario = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.treeSOP = new SOPMonitoringSystem.Popup.SOPTreeSim();
            this.rdoEmergency = new System.Windows.Forms.RadioButton();
            this.rdoNormal = new System.Windows.Forms.RadioButton();
            this.btnSelect = new System.Windows.Forms.Button();
            this.panel7.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel7
            // 
            this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.Controls.Add(this.label15);
            this.panel7.Location = new System.Drawing.Point(12, 12);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(491, 47);
            this.panel7.TabIndex = 4;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label15.Location = new System.Drawing.Point(20, 15);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(157, 16);
            this.label15.TabIndex = 1;
            this.label15.Text = "SOP 시나리오 선택";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblSenario);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.rdoEmergency);
            this.panel1.Controls.Add(this.rdoNormal);
            this.panel1.Controls.Add(this.btnSelect);
            this.panel1.Location = new System.Drawing.Point(12, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(491, 524);
            this.panel1.TabIndex = 5;
            // 
            // lblSenario
            // 
            this.lblSenario.AutoSize = true;
            this.lblSenario.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.lblSenario.Location = new System.Drawing.Point(20, 13);
            this.lblSenario.Name = "lblSenario";
            this.lblSenario.Size = new System.Drawing.Size(88, 12);
            this.lblSenario.TabIndex = 65;
            this.lblSenario.Text = "평일 시나리오";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(403, 489);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 64;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.treeSOP);
            this.panel2.Location = new System.Drawing.Point(13, 34);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(465, 449);
            this.panel2.TabIndex = 63;
            // 
            // treeSOP
            // 
            this.treeSOP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeSOP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeSOP.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.treeSOP.IgnoreLoadSOP = false;
            this.treeSOP.IgnoreSelect = false;
            this.treeSOP.ImageIndex = 0;
            this.treeSOP.Location = new System.Drawing.Point(0, 0);
            this.treeSOP.Name = "treeSOP";
            this.treeSOP.PrevSelectedDisasterID = -1;
            this.treeSOP.PrevSelectedNode = null;
            this.treeSOP.SelectedImageIndex = 0;
            this.treeSOP.Size = new System.Drawing.Size(465, 449);
            this.treeSOP.TabIndex = 60;
            this.treeSOP.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSOP_AfterSelect);
            // 
            // rdoEmergency
            // 
            this.rdoEmergency.AutoSize = true;
            this.rdoEmergency.Location = new System.Drawing.Point(94, 496);
            this.rdoEmergency.Name = "rdoEmergency";
            this.rdoEmergency.Size = new System.Drawing.Size(119, 16);
            this.rdoEmergency.TabIndex = 61;
            this.rdoEmergency.TabStop = true;
            this.rdoEmergency.Text = "야간 및 휴일 모드";
            this.rdoEmergency.UseVisualStyleBackColor = true;
            this.rdoEmergency.Visible = false;
            // 
            // rdoNormal
            // 
            this.rdoNormal.AutoSize = true;
            this.rdoNormal.Location = new System.Drawing.Point(13, 496);
            this.rdoNormal.Name = "rdoNormal";
            this.rdoNormal.Size = new System.Drawing.Size(75, 16);
            this.rdoNormal.TabIndex = 60;
            this.rdoNormal.TabStop = true;
            this.rdoNormal.Text = "평일 모드";
            this.rdoNormal.UseVisualStyleBackColor = true;
            this.rdoNormal.Visible = false;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Enabled = false;
            this.btnSelect.Location = new System.Drawing.Point(322, 489);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 58;
            this.btnSelect.Text = "선택";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // PopupSelectSOP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(515, 613);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupSelectSOP";
            this.Text = "PopupSelectSOP";
            this.Load += new System.EventHandler(this.PopupSelectSOP_Load);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.RadioButton rdoEmergency;
        private System.Windows.Forms.RadioButton rdoNormal;
        private System.Windows.Forms.Panel panel2;
        private SOPTreeSim treeSOP;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblSenario;
    }
}