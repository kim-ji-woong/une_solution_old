namespace SOPManager
{
    partial class PopupSelectTeam3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupSelectTeam3));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelTeamType = new System.Windows.Forms.Label();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ribbonButton1 = new UnE.GUI.RibbonButton();
            this.ribbonButton2 = new UnE.GUI.RibbonButton();
            this.btnChangeTeam = new UnE.GUI.RibbonButton();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelTeamType);
            this.groupBox1.Controls.Add(this.treeViewTeam);
            this.groupBox1.Location = new System.Drawing.Point(9, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(345, 379);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // labelTeamType
            // 
            this.labelTeamType.AutoSize = true;
            this.labelTeamType.BackColor = System.Drawing.Color.Transparent;
            this.labelTeamType.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTeamType.ForeColor = System.Drawing.Color.White;
            this.labelTeamType.Location = new System.Drawing.Point(10, 14);
            this.labelTeamType.Name = "labelTeamType";
            this.labelTeamType.Size = new System.Drawing.Size(54, 17);
            this.labelTeamType.TabIndex = 6;
            this.labelTeamType.Text = "전체 팀";
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewTeam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            this.treeViewTeam.Location = new System.Drawing.Point(13, 37);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(317, 331);
            this.treeViewTeam.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(370, 457);
            this.panel2.TabIndex = 25;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.ribbonButton1);
            this.panel3.Controls.Add(this.ribbonButton2);
            this.panel3.Controls.Add(this.btnChangeTeam);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(364, 451);
            this.panel3.TabIndex = 0;
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.CheckButton = false;
            this.ribbonButton1.CheckedBkgndImage = null;
            this.ribbonButton1.CheckedImage = null;
            this.ribbonButton1.ClickedBackgroundImage = null;
            this.ribbonButton1.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.ribbonButton1.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.ribbonButton1.DisabledBkgndImage = null;
            this.ribbonButton1.DisabledImage = null;
            this.ribbonButton1.ID = -1;
            this.ribbonButton1.InitButtonWidth = 100;
            this.ribbonButton1.IsChecked = false;
            this.ribbonButton1.Location = new System.Drawing.Point(288, 405);
            this.ribbonButton1.MouseOverBkgndImage = null;
            this.ribbonButton1.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.ribbonButton1.Owner = null;
            this.ribbonButton1.Size = new System.Drawing.Size(69, 37);
            this.ribbonButton1.TabIndex = 99;
            this.ribbonButton1.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton1.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton1.ToolTipText = "";
            this.ribbonButton1.UseCustomImageRect = true;
            this.ribbonButton1.UseTextLocation = false;
            this.ribbonButton1.UseVisualStyleBackColor = true;
            this.ribbonButton1.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ribbonButton2
            // 
            this.ribbonButton2.CheckButton = false;
            this.ribbonButton2.CheckedBkgndImage = null;
            this.ribbonButton2.CheckedImage = null;
            this.ribbonButton2.ClickedBackgroundImage = null;
            this.ribbonButton2.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.ribbonButton2.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.ribbonButton2.DisabledBkgndImage = null;
            this.ribbonButton2.DisabledImage = null;
            this.ribbonButton2.ID = -1;
            this.ribbonButton2.InitButtonWidth = 100;
            this.ribbonButton2.IsChecked = false;
            this.ribbonButton2.Location = new System.Drawing.Point(217, 405);
            this.ribbonButton2.MouseOverBkgndImage = null;
            this.ribbonButton2.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.ribbonButton2.Name = "ribbonButton2";
            this.ribbonButton2.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.ribbonButton2.Owner = null;
            this.ribbonButton2.Size = new System.Drawing.Size(69, 37);
            this.ribbonButton2.TabIndex = 98;
            this.ribbonButton2.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton2.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton2.ToolTipText = "";
            this.ribbonButton2.UseCustomImageRect = true;
            this.ribbonButton2.UseTextLocation = false;
            this.ribbonButton2.UseVisualStyleBackColor = true;
            this.ribbonButton2.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnChangeTeam
            // 
            this.btnChangeTeam.CheckButton = false;
            this.btnChangeTeam.CheckedBkgndImage = null;
            this.btnChangeTeam.CheckedImage = null;
            this.btnChangeTeam.ClickedBackgroundImage = null;
            this.btnChangeTeam.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.btnChangeTeam.CustomImageRect = new System.Drawing.Rectangle(0, 0, 100, 55);
            this.btnChangeTeam.DisabledBkgndImage = null;
            this.btnChangeTeam.DisabledImage = null;
            this.btnChangeTeam.ID = -1;
            this.btnChangeTeam.InitButtonWidth = 100;
            this.btnChangeTeam.IsChecked = false;
            this.btnChangeTeam.Location = new System.Drawing.Point(8, 396);
            this.btnChangeTeam.MouseOverBkgndImage = null;
            this.btnChangeTeam.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.btnChangeTeam.Name = "btnChangeTeam";
            this.btnChangeTeam.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBack;
            this.btnChangeTeam.Owner = null;
            this.btnChangeTeam.Size = new System.Drawing.Size(100, 55);
            this.btnChangeTeam.TabIndex = 97;
            this.btnChangeTeam.Text = "조직변경";
            this.btnChangeTeam.TextLocation = new System.Drawing.Point(-2, 17);
            this.btnChangeTeam.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnChangeTeam.ToolTipText = "조직변경";
            this.btnChangeTeam.UseCustomImageRect = true;
            this.btnChangeTeam.UseTextLocation = true;
            this.btnChangeTeam.UseVisualStyleBackColor = true;
            this.btnChangeTeam.Click += new System.EventHandler(this.btnChangeTeam_Click);
            // 
            // PopupSelectTeam3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(370, 457);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PopupSelectTeam3";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "조직 선택";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupSelectTeam3_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupSelectTeam3_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupSelectTeam3_MouseUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelTeamType;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private UnE.GUI.RibbonButton btnChangeTeam;
        private UnE.GUI.RibbonButton ribbonButton1;
        private UnE.GUI.RibbonButton ribbonButton2;


    }
}