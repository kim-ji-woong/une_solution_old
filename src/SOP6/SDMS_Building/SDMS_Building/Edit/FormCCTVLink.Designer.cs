namespace SDMS_Building.Edit
{
    partial class FormCCTVLink
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCCTVLink));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.treeViewSensor = new Aga.Controls.Tree.TreeViewAdv();
            this.nodeStateIconSensor = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
            this.nodeTextBoxSensor = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.treeViewCCTV = new Aga.Controls.Tree.TreeViewAdv();
            this.nodeStateIconCCTV = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
            this.nodeTextBoxCCTV = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.ImageButton();
            this.btnSave = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnSearch = new UnE.GUI.ImageButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "building.png");
            this.imageList1.Images.SetKeyName(1, "floor.png");
            this.imageList1.Images.SetKeyName(2, "fire.png");
            this.imageList1.Images.SetKeyName(3, "cctv.png");
            // 
            // treeViewSensor
            // 
            this.treeViewSensor.AllowDrop = true;
            this.treeViewSensor.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewSensor.DefaultToolTipProvider = null;
            this.treeViewSensor.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewSensor.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewSensor.Location = new System.Drawing.Point(8, 50);
            this.treeViewSensor.Model = null;
            this.treeViewSensor.Name = "treeViewSensor";
            this.treeViewSensor.NodeControls.Add(this.nodeStateIconSensor);
            this.treeViewSensor.NodeControls.Add(this.nodeTextBoxSensor);
            this.treeViewSensor.SelectedNode = null;
            this.treeViewSensor.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.treeViewSensor.Size = new System.Drawing.Size(383, 428);
            this.treeViewSensor.TabIndex = 0;
            this.treeViewSensor.Text = "treeViewAdv1";
            this.treeViewSensor.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeViewSensor.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeViewSensor.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            this.treeViewSensor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyDown);
            // 
            // nodeStateIconSensor
            // 
            this.nodeStateIconSensor.LeftMargin = 1;
            this.nodeStateIconSensor.ParentColumn = null;
            this.nodeStateIconSensor.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            // 
            // nodeTextBoxSensor
            // 
            this.nodeTextBoxSensor.DataPropertyName = "Text";
            this.nodeTextBoxSensor.EditEnabled = true;
            this.nodeTextBoxSensor.IncrementalSearchEnabled = true;
            this.nodeTextBoxSensor.LeftMargin = 3;
            this.nodeTextBoxSensor.ParentColumn = null;
            // 
            // treeViewCCTV
            // 
            this.treeViewCCTV.AllowDrop = true;
            this.treeViewCCTV.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewCCTV.DefaultToolTipProvider = null;
            this.treeViewCCTV.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewCCTV.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewCCTV.Location = new System.Drawing.Point(397, 50);
            this.treeViewCCTV.Model = null;
            this.treeViewCCTV.Name = "treeViewCCTV";
            this.treeViewCCTV.NodeControls.Add(this.nodeStateIconCCTV);
            this.treeViewCCTV.NodeControls.Add(this.nodeTextBoxCCTV);
            this.treeViewCCTV.SelectedNode = null;
            this.treeViewCCTV.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.treeViewCCTV.Size = new System.Drawing.Size(413, 428);
            this.treeViewCCTV.TabIndex = 1;
            this.treeViewCCTV.Text = "treeViewAdv2";
            this.treeViewCCTV.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeViewCCTV.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeViewCCTV.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            this.treeViewCCTV.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyDown);
            // 
            // nodeStateIconCCTV
            // 
            this.nodeStateIconCCTV.LeftMargin = 1;
            this.nodeStateIconCCTV.ParentColumn = null;
            this.nodeStateIconCCTV.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            // 
            // nodeTextBoxCCTV
            // 
            this.nodeTextBoxCCTV.DataPropertyName = "Text";
            this.nodeTextBoxCCTV.EditEnabled = true;
            this.nodeTextBoxCCTV.IncrementalSearchEnabled = true;
            this.nodeTextBoxCCTV.LeftMargin = 3;
            this.nodeTextBoxCCTV.ParentColumn = null;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(822, 42);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(52, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 20);
            this.label1.TabIndex = 17;
            this.label1.Text = "CCTV 연결";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.panel3.Location = new System.Drawing.Point(28, 17);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(7, 7);
            this.panel3.TabIndex = 16;
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::SDMS_Building.Properties.Resources.close_Click;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SDMS_Building.Properties.Resources.close_Hover;
            this.btnClose.ImageNormal = global::SDMS_Building.Properties.Resources.close_Normal;
            this.btnClose.Location = new System.Drawing.Point(783, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(20, 20);
            this.btnClose.TabIndex = 15;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.ButtonText = "";
            this.btnSave.ImageClicked = global::SDMS_Building.Properties.Resources.editSave_click;
            this.btnSave.ImageDisabled = global::SDMS_Building.Properties.Resources.editSave_click;
            this.btnSave.ImageMouseOver = global::SDMS_Building.Properties.Resources.editSave_hover;
            this.btnSave.ImageNormal = global::SDMS_Building.Properties.Resources.editSave_normal;
            this.btnSave.Location = new System.Drawing.Point(730, 483);
            this.btnSave.Name = "btnSave";
            this.btnSave.Owner = null;
            this.btnSave.Size = new System.Drawing.Size(80, 26);
            this.btnSave.TabIndex = 62;
            this.btnSave.TabStop = false;
            this.btnSave.TextColor = System.Drawing.Color.White;
            this.btnSave.TextFont = new System.Drawing.Font("나눔바른고딕", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ToolTipText = "";
            this.btnSave.UseToolTip = false;
            this.btnSave.WindowRateWidth = 1F;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonText = "";
            this.btnCancel.ImageClicked = global::SDMS_Building.Properties.Resources.cancel_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SDMS_Building.Properties.Resources.cancel_Hover;
            this.btnCancel.ImageNormal = global::SDMS_Building.Properties.Resources.cancel_Normal;
            this.btnCancel.Location = new System.Drawing.Point(828, 482);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(80, 26);
            this.btnCancel.TabIndex = 63;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(4, 7);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(193, 14);
            this.textBox1.TabIndex = 64;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.ButtonText = "";
            this.btnSearch.ImageClicked = global::SDMS_Building.Properties.Resources.search2_click;
            this.btnSearch.ImageDisabled = global::SDMS_Building.Properties.Resources.search2_click;
            this.btnSearch.ImageMouseOver = global::SDMS_Building.Properties.Resources.search2_hover;
            this.btnSearch.ImageNormal = global::SDMS_Building.Properties.Resources.search2_normal;
            this.btnSearch.Location = new System.Drawing.Point(214, 484);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Owner = null;
            this.btnSearch.Size = new System.Drawing.Size(36, 26);
            this.btnSearch.TabIndex = 66;
            this.btnSearch.TabStop = false;
            this.btnSearch.TextColor = System.Drawing.Color.White;
            this.btnSearch.TextFont = new System.Drawing.Font("나눔바른고딕", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.ToolTipText = "";
            this.btnSearch.UseToolTip = false;
            this.btnSearch.WindowRateWidth = 1F;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Location = new System.Drawing.Point(8, 483);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 28);
            this.panel2.TabIndex = 67;
            // 
            // FormCCTVLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(822, 524);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.treeViewCCTV);
            this.Controls.Add(this.treeViewSensor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCCTVLink";
            this.Text = "FormCCTVLink";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private Aga.Controls.Tree.TreeViewAdv treeViewSensor;
        private Aga.Controls.Tree.NodeControls.NodeStateIcon nodeStateIconSensor;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBoxSensor;
        private Aga.Controls.Tree.TreeViewAdv treeViewCCTV;
        private Aga.Controls.Tree.NodeControls.NodeStateIcon nodeStateIconCCTV;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBoxCCTV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private UnE.GUI.ImageButton btnClose;
        private UnE.GUI.ImageButton btnSave;
        private UnE.GUI.ImageButton btnCancel;
        private System.Windows.Forms.TextBox textBox1;
        private UnE.GUI.ImageButton btnSearch;
        private System.Windows.Forms.Panel panel2;
    }
}