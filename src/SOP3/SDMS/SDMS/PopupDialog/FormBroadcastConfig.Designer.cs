namespace SDMS
{
    partial class FormBroadcastConfig
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.radioRepeatTwice = new System.Windows.Forms.RadioButton();
            this.radioRepeatOnce = new System.Windows.Forms.RadioButton();
            this.radioNoRepeat = new System.Windows.Forms.RadioButton();
            this.checkBoxUseSiren = new System.Windows.Forms.CheckBox();
            this.checkBoxUseBroadcast = new System.Windows.Forms.CheckBox();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.checkBoxUseBroadcast2 = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(10, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(442, 47);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(20, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "방송 관리";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Controls.Add(this.radioRepeatTwice);
            this.panel2.Controls.Add(this.radioRepeatOnce);
            this.panel2.Controls.Add(this.radioNoRepeat);
            this.panel2.Controls.Add(this.checkBoxUseSiren);
            this.panel2.Controls.Add(this.checkBoxUseBroadcast2);
            this.panel2.Controls.Add(this.checkBoxUseBroadcast);
            this.panel2.Controls.Add(this.richTextBox);
            this.panel2.Location = new System.Drawing.Point(10, 70);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(442, 353);
            this.panel2.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(370, 314);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(51, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(236, 253);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(201, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "<< >> 내의 메시지는 반복되지 않음";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(236, 230);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(161, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "● : 방송시 화재 위치로 표현";
            // 
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(310, 314);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(51, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // radioRepeatTwice
            // 
            this.radioRepeatTwice.AutoSize = true;
            this.radioRepeatTwice.Location = new System.Drawing.Point(261, 172);
            this.radioRepeatTwice.Name = "radioRepeatTwice";
            this.radioRepeatTwice.Size = new System.Drawing.Size(69, 16);
            this.radioRepeatTwice.TabIndex = 2;
            this.radioRepeatTwice.TabStop = true;
            this.radioRepeatTwice.Text = "2회 반복";
            this.radioRepeatTwice.UseVisualStyleBackColor = true;
            // 
            // radioRepeatOnce
            // 
            this.radioRepeatOnce.AutoSize = true;
            this.radioRepeatOnce.Location = new System.Drawing.Point(261, 139);
            this.radioRepeatOnce.Name = "radioRepeatOnce";
            this.radioRepeatOnce.Size = new System.Drawing.Size(69, 16);
            this.radioRepeatOnce.TabIndex = 2;
            this.radioRepeatOnce.TabStop = true;
            this.radioRepeatOnce.Text = "1회 반복";
            this.radioRepeatOnce.UseVisualStyleBackColor = true;
            // 
            // radioNoRepeat
            // 
            this.radioNoRepeat.AutoSize = true;
            this.radioNoRepeat.Location = new System.Drawing.Point(261, 108);
            this.radioNoRepeat.Name = "radioNoRepeat";
            this.radioNoRepeat.Size = new System.Drawing.Size(71, 16);
            this.radioNoRepeat.TabIndex = 2;
            this.radioNoRepeat.TabStop = true;
            this.radioNoRepeat.Text = "반복없음";
            this.radioNoRepeat.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseSiren
            // 
            this.checkBoxUseSiren.AutoSize = true;
            this.checkBoxUseSiren.Location = new System.Drawing.Point(261, 63);
            this.checkBoxUseSiren.Name = "checkBoxUseSiren";
            this.checkBoxUseSiren.Size = new System.Drawing.Size(156, 16);
            this.checkBoxUseSiren.TabIndex = 1;
            this.checkBoxUseSiren.Text = "방송 시작시 사이렌 사용";
            this.checkBoxUseSiren.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseBroadcast
            // 
            this.checkBoxUseBroadcast.AutoSize = true;
            this.checkBoxUseBroadcast.Location = new System.Drawing.Point(261, 18);
            this.checkBoxUseBroadcast.Name = "checkBoxUseBroadcast";
            this.checkBoxUseBroadcast.Size = new System.Drawing.Size(168, 16);
            this.checkBoxUseBroadcast.TabIndex = 1;
            this.checkBoxUseBroadcast.Text = "화재 탐지시 사내방송 실시";
            this.checkBoxUseBroadcast.UseVisualStyleBackColor = true;
            // 
            // richTextBox
            // 
            this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.richTextBox.Location = new System.Drawing.Point(14, 14);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(215, 323);
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "<<재난안전팀에서 알려드립니다.>>\n●에서 화재가 탐지되었습니다.\n소방 담당자들은 현장 확인하셔 주시고, 나머지 직원들은 비상 방송 및 무전기를 " +
    "이용하여 전파되는 임무메시지에 따라 행동해 주시기 바랍니다.";
            // 
            // checkBoxUseBroadcast2
            // 
            this.checkBoxUseBroadcast2.AutoSize = true;
            this.checkBoxUseBroadcast2.Location = new System.Drawing.Point(261, 40);
            this.checkBoxUseBroadcast2.Name = "checkBoxUseBroadcast2";
            this.checkBoxUseBroadcast2.Size = new System.Drawing.Size(168, 16);
            this.checkBoxUseBroadcast2.TabIndex = 1;
            this.checkBoxUseBroadcast2.Text = "화재 신고시 사내방송 실시";
            this.checkBoxUseBroadcast2.UseVisualStyleBackColor = true;
            // 
            // FormBroadcastConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(462, 433);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormBroadcastConfig";
            this.Text = "FormBroadcastConfig";
            this.Load += new System.EventHandler(this.FormBroadcastConfig_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioRepeatTwice;
        private System.Windows.Forms.RadioButton radioRepeatOnce;
        private System.Windows.Forms.RadioButton radioNoRepeat;
        private System.Windows.Forms.CheckBox checkBoxUseSiren;
        private System.Windows.Forms.CheckBox checkBoxUseBroadcast;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxUseBroadcast2;
    }
}