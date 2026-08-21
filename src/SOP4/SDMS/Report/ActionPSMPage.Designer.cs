namespace SDMS
{
    partial class ActionPSMPage
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
            this.gvMain = new System.Windows.Forms.DataGridView();
            this.lblReportTitle = new System.Windows.Forms.Label();
            this.lblAlarmType = new System.Windows.Forms.Label();
            this.lblSearchLocation = new System.Windows.Forms.Label();
            this.lblActorName = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblEquipZone = new System.Windows.Forms.Label();
            this.lblMinDate = new System.Windows.Forms.Label();
            this.lblActionTime = new System.Windows.Forms.Label();
            this.lblSplit = new System.Windows.Forms.Label();
            this.lblStatementEnd = new System.Windows.Forms.Label();
            this.lblSelectDate = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDefault = new System.Windows.Forms.Label();
            this.btnSaveHWP = new System.Windows.Forms.Button();
            this.reactionPSMLogBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.textBoxMemo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gvMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactionPSMLogBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // gvMain
            // 
            this.gvMain.AllowUserToAddRows = false;
            this.gvMain.AllowUserToDeleteRows = false;
            this.gvMain.AllowUserToResizeRows = false;
            this.gvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gvMain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvMain.BackgroundColor = System.Drawing.SystemColors.Control;
            this.gvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvMain.Location = new System.Drawing.Point(25, 142);
            this.gvMain.Name = "gvMain";
            this.gvMain.ReadOnly = true;
            this.gvMain.RowHeadersVisible = false;
            this.gvMain.RowTemplate.Height = 30;
            this.gvMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvMain.Size = new System.Drawing.Size(858, 285);
            this.gvMain.TabIndex = 22;
            // 
            // lblReportTitle
            // 
            this.lblReportTitle.AutoSize = true;
            this.lblReportTitle.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblReportTitle.Location = new System.Drawing.Point(23, 20);
            this.lblReportTitle.Name = "lblReportTitle";
            this.lblReportTitle.Size = new System.Drawing.Size(170, 24);
            this.lblReportTitle.TabIndex = 19;
            this.lblReportTitle.Text = "누출 대응 이력";
            // 
            // lblAlarmType
            // 
            this.lblAlarmType.AutoSize = true;
            this.lblAlarmType.Location = new System.Drawing.Point(294, 73);
            this.lblAlarmType.Name = "lblAlarmType";
            this.lblAlarmType.Size = new System.Drawing.Size(13, 12);
            this.lblAlarmType.TabIndex = 30;
            this.lblAlarmType.Text = "  ";
            // 
            // lblSearchLocation
            // 
            this.lblSearchLocation.AutoSize = true;
            this.lblSearchLocation.Location = new System.Drawing.Point(580, 31);
            this.lblSearchLocation.Name = "lblSearchLocation";
            this.lblSearchLocation.Size = new System.Drawing.Size(53, 12);
            this.lblSearchLocation.TabIndex = 29;
            this.lblSearchLocation.Text = "모든건물";
            // 
            // lblActorName
            // 
            this.lblActorName.AutoSize = true;
            this.lblActorName.ForeColor = System.Drawing.Color.Red;
            this.lblActorName.Location = new System.Drawing.Point(92, 104);
            this.lblActorName.Name = "lblActorName";
            this.lblActorName.Size = new System.Drawing.Size(34, 12);
            this.lblActorName.TabIndex = 28;
            this.lblActorName.Text = "Actor";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.ForeColor = System.Drawing.Color.Red;
            this.lblResult.Location = new System.Drawing.Point(28, 104);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(53, 12);
            this.lblResult.TabIndex = 27;
            this.lblResult.Text = "누출신고";
            // 
            // lblEquipZone
            // 
            this.lblEquipZone.AutoSize = true;
            this.lblEquipZone.Location = new System.Drawing.Point(138, 73);
            this.lblEquipZone.Name = "lblEquipZone";
            this.lblEquipZone.Size = new System.Drawing.Size(94, 12);
            this.lblEquipZone.TabIndex = 26;
            this.lblEquipZone.Text = "EquipmentZone";
            // 
            // lblMinDate
            // 
            this.lblMinDate.AutoSize = true;
            this.lblMinDate.Location = new System.Drawing.Point(265, 31);
            this.lblMinDate.Name = "lblMinDate";
            this.lblMinDate.Size = new System.Drawing.Size(73, 12);
            this.lblMinDate.TabIndex = 25;
            this.lblMinDate.Text = " 데이터 없음";
            // 
            // lblActionTime
            // 
            this.lblActionTime.AutoSize = true;
            this.lblActionTime.Location = new System.Drawing.Point(201, 31);
            this.lblActionTime.Name = "lblActionTime";
            this.lblActionTime.Size = new System.Drawing.Size(57, 12);
            this.lblActionTime.TabIndex = 24;
            this.lblActionTime.Text = "발생 시간";
            // 
            // lblSplit
            // 
            this.lblSplit.AutoSize = true;
            this.lblSplit.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSplit.Location = new System.Drawing.Point(254, 27);
            this.lblSplit.Name = "lblSplit";
            this.lblSplit.Size = new System.Drawing.Size(12, 19);
            this.lblSplit.TabIndex = 23;
            this.lblSplit.Text = "l";
            // 
            // lblStatementEnd
            // 
            this.lblStatementEnd.AutoSize = true;
            this.lblStatementEnd.Location = new System.Drawing.Point(420, 73);
            this.lblStatementEnd.Name = "lblStatementEnd";
            this.lblStatementEnd.Size = new System.Drawing.Size(121, 12);
            this.lblStatementEnd.TabIndex = 21;
            this.lblStatementEnd.Text = "대한 대응이력입니다.";
            // 
            // lblSelectDate
            // 
            this.lblSelectDate.AutoSize = true;
            this.lblSelectDate.Location = new System.Drawing.Point(28, 73);
            this.lblSelectDate.Name = "lblSelectDate";
            this.lblSelectDate.Size = new System.Drawing.Size(34, 12);
            this.lblSelectDate.TabIndex = 20;
            this.lblSelectDate.Text = "Time";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(12, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(884, 5);
            this.panel1.TabIndex = 31;
            // 
            // lblDefault
            // 
            this.lblDefault.AutoSize = true;
            this.lblDefault.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDefault.Location = new System.Drawing.Point(23, 92);
            this.lblDefault.Name = "lblDefault";
            this.lblDefault.Size = new System.Drawing.Size(290, 19);
            this.lblDefault.TabIndex = 32;
            this.lblDefault.Text = "조회할 누출상황을 선택하세요.";
            // 
            // btnSaveHWP
            // 
            this.btnSaveHWP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveHWP.Location = new System.Drawing.Point(806, 12);
            this.btnSaveHWP.Name = "btnSaveHWP";
            this.btnSaveHWP.Size = new System.Drawing.Size(90, 39);
            this.btnSaveHWP.TabIndex = 35;
            this.btnSaveHWP.Text = "한글파일저장";
            this.btnSaveHWP.UseVisualStyleBackColor = true;
            this.btnSaveHWP.Click += new System.EventHandler(this.btnSaveHWP_Click);
            // 
            // reactionPSMLogBindingSource
            // 
            this.reactionPSMLogBindingSource.DataSource = typeof(SDMS.Report.ReactionPSMLog);
            // 
            // textBoxMemo
            // 
            this.textBoxMemo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMemo.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemo.Location = new System.Drawing.Point(25, 471);
            this.textBoxMemo.Margin = new System.Windows.Forms.Padding(5);
            this.textBoxMemo.Multiline = true;
            this.textBoxMemo.Name = "textBoxMemo";
            this.textBoxMemo.ReadOnly = true;
            this.textBoxMemo.Size = new System.Drawing.Size(858, 58);
            this.textBoxMemo.TabIndex = 37;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(23, 446);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 19);
            this.label7.TabIndex = 36;
            this.label7.Text = "메모";
            // 
            // ActionPSMPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 554);
            this.Controls.Add(this.textBoxMemo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnSaveHWP);
            this.Controls.Add(this.lblDefault);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gvMain);
            this.Controls.Add(this.lblReportTitle);
            this.Controls.Add(this.lblAlarmType);
            this.Controls.Add(this.lblSearchLocation);
            this.Controls.Add(this.lblActorName);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblEquipZone);
            this.Controls.Add(this.lblMinDate);
            this.Controls.Add(this.lblActionTime);
            this.Controls.Add(this.lblSplit);
            this.Controls.Add(this.lblStatementEnd);
            this.Controls.Add(this.lblSelectDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ActionPSMPage";
            this.Text = "ActionPSMPage";
            this.Load += new System.EventHandler(this.ActionPSMPage_Load);
            this.Resize += new System.EventHandler(this.ActionPSMPage_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.gvMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactionPSMLogBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gvMain;
        private System.Windows.Forms.Label lblReportTitle;
        private System.Windows.Forms.Label lblAlarmType;
        private System.Windows.Forms.Label lblSearchLocation;
        private System.Windows.Forms.Label lblActorName;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblEquipZone;
        private System.Windows.Forms.Label lblMinDate;
        private System.Windows.Forms.Label lblActionTime;
        private System.Windows.Forms.Label lblSplit;
        private System.Windows.Forms.Label lblStatementEnd;
        private System.Windows.Forms.Label lblSelectDate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblDefault;
        private System.Windows.Forms.Button btnSaveHWP;
        private System.Windows.Forms.BindingSource reactionPSMLogBindingSource;
        private System.Windows.Forms.TextBox textBoxMemo;
        private System.Windows.Forms.Label label7;

    }
}