namespace SDMS
{
	partial class FormDataBackup
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lstFileList = new System.Windows.Forms.ListBox();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.btnRestore = new UnE.GUI.ImageButton();
            this.btnDelete = new UnE.GUI.ImageButton();
            this.btnBackup = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.btnOK = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnRestore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBackup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font(Program.prgFont, 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 22);
            this.label3.TabIndex = 1;
            this.label3.Text = "백업 / 복원";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(13, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 18);
            this.label2.TabIndex = 9;
            this.label2.Text = "저장된 백업 파일";
            // 
            // lstFileList
            // 
            this.lstFileList.BackColor = System.Drawing.Color.White;
            this.lstFileList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstFileList.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lstFileList.FormattingEnabled = true;
            this.lstFileList.ItemHeight = 18;
            this.lstFileList.Location = new System.Drawing.Point(16, 79);
            this.lstFileList.Name = "lstFileList";
            this.lstFileList.Size = new System.Drawing.Size(329, 146);
            this.lstFileList.TabIndex = 13;
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Size = new System.Drawing.Size(414, 289);
            this.shapeContainer1.TabIndex = 3;
            this.shapeContainer1.TabStop = false;
            // 
            // btnRestore
            // 
            this.btnRestore.ButtonText = "";
            this.btnRestore.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRestore.ImageClicked = global::SDMS.Properties.Resources.BtnRestore_Click;
            this.btnRestore.ImageDisabled = null;
            this.btnRestore.ImageMouseOver = global::SDMS.Properties.Resources.BtnRestore_Click;
            this.btnRestore.ImageNormal = global::SDMS.Properties.Resources.BtnRestore_Default;
            this.btnRestore.Location = new System.Drawing.Point(351, 147);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Owner = null;
            this.btnRestore.Size = new System.Drawing.Size(77, 36);
            this.btnRestore.TabIndex = 18;
            this.btnRestore.TabStop = false;
            this.btnRestore.TextColor = System.Drawing.Color.Black;
            this.btnRestore.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRestore.ToolTipText = "";
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.ButtonText = "";
            this.btnDelete.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDelete.ImageClicked = global::SDMS.Properties.Resources.BtnDelete_Click;
            this.btnDelete.ImageDisabled = null;
            this.btnDelete.ImageMouseOver = global::SDMS.Properties.Resources.BtnDelete_Click;
            this.btnDelete.ImageNormal = global::SDMS.Properties.Resources.BtnDelete_Default;
            this.btnDelete.Location = new System.Drawing.Point(351, 189);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Owner = null;
            this.btnDelete.Size = new System.Drawing.Size(77, 36);
            this.btnDelete.TabIndex = 17;
            this.btnDelete.TabStop = false;
            this.btnDelete.TextColor = System.Drawing.Color.Black;
            this.btnDelete.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDelete.ToolTipText = "";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnBackup
            // 
            this.btnBackup.ButtonText = "";
            this.btnBackup.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnBackup.ImageClicked = global::SDMS.Properties.Resources.BtnBackup_Click;
            this.btnBackup.ImageDisabled = null;
            this.btnBackup.ImageMouseOver = global::SDMS.Properties.Resources.BtnBackup_Click;
            this.btnBackup.ImageNormal = global::SDMS.Properties.Resources.BtnBackup_Default;
            this.btnBackup.Location = new System.Drawing.Point(351, 79);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Owner = null;
            this.btnBackup.Size = new System.Drawing.Size(77, 36);
            this.btnBackup.TabIndex = 16;
            this.btnBackup.TabStop = false;
            this.btnBackup.TextColor = System.Drawing.Color.Black;
            this.btnBackup.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnBackup.ToolTipText = "";
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonText = "";
            this.btnCancel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ImageClicked = global::SDMS.Properties.Resources.Cancel_77_36_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SDMS.Properties.Resources.Cancel_77_36_Click;
            this.btnCancel.ImageNormal = global::SDMS.Properties.Resources.Cancel_77_36_Default;
            this.btnCancel.Location = new System.Drawing.Point(351, 231);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(77, 36);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.ButtonText = "";
            this.btnOK.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ImageClicked = global::SDMS.Properties.Resources.Ok_77_36_Click;
            this.btnOK.ImageDisabled = null;
            this.btnOK.ImageMouseOver = global::SDMS.Properties.Resources.Ok_77_36_Click;
            this.btnOK.ImageNormal = global::SDMS.Properties.Resources.Ok_77_36_Default;
            this.btnOK.Location = new System.Drawing.Point(268, 231);
            this.btnOK.Name = "btnOK";
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(77, 36);
            this.btnOK.TabIndex = 6;
            this.btnOK.TabStop = false;
            this.btnOK.TextColor = System.Drawing.Color.Black;
            this.btnOK.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ToolTipText = "";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FormDataBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(440, 277);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.lstFileList);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDataBackup";
            this.ShowInTaskbar = false;
            this.Text = "FormDataBackup";
            ((System.ComponentModel.ISupportInitialize)(this.btnRestore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBackup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private UnE.GUI.ImageButton btnOK;
        private UnE.GUI.ImageButton btnCancel;
        private System.Windows.Forms.ListBox lstFileList;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private UnE.GUI.ImageButton btnBackup;
        private UnE.GUI.ImageButton btnDelete;
        private UnE.GUI.ImageButton btnRestore;
	}
}