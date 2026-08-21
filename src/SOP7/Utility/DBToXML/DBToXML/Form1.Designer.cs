
namespace DBToXML
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtWebServerURL = new System.Windows.Forms.TextBox();
            this.cbDBType = new System.Windows.Forms.ComboBox();
            this.btnConnet = new System.Windows.Forms.Button();
            this.lblConnectDB = new System.Windows.Forms.Label();
            this.txtDBName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSiteID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnDBToXML = new System.Windows.Forms.Button();
            this.btnReadXML = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtWebServerURL
            // 
            this.txtWebServerURL.Location = new System.Drawing.Point(82, 42);
            this.txtWebServerURL.Name = "txtWebServerURL";
            this.txtWebServerURL.Size = new System.Drawing.Size(190, 23);
            this.txtWebServerURL.TabIndex = 0;
            this.txtWebServerURL.Text = "http://127.0.0.1:81";
            // 
            // cbDBType
            // 
            this.cbDBType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDBType.FormattingEnabled = true;
            this.cbDBType.Items.AddRange(new object[] {
            "MSSQL",
            "MySQL"});
            this.cbDBType.Location = new System.Drawing.Point(82, 13);
            this.cbDBType.Name = "cbDBType";
            this.cbDBType.Size = new System.Drawing.Size(190, 23);
            this.cbDBType.TabIndex = 1;
            // 
            // btnConnet
            // 
            this.btnConnet.Location = new System.Drawing.Point(197, 136);
            this.btnConnet.Name = "btnConnet";
            this.btnConnet.Size = new System.Drawing.Size(75, 23);
            this.btnConnet.TabIndex = 2;
            this.btnConnet.Text = "접속";
            this.btnConnet.UseVisualStyleBackColor = true;
            this.btnConnet.Click += new System.EventHandler(this.btnConnet_Click);
            // 
            // lblConnectDB
            // 
            this.lblConnectDB.AutoSize = true;
            this.lblConnectDB.Location = new System.Drawing.Point(12, 140);
            this.lblConnectDB.Name = "lblConnectDB";
            this.lblConnectDB.Size = new System.Drawing.Size(12, 15);
            this.lblConnectDB.TabIndex = 3;
            this.lblConnectDB.Text = "-";
            // 
            // txtDBName
            // 
            this.txtDBName.Location = new System.Drawing.Point(82, 72);
            this.txtDBName.Name = "txtDBName";
            this.txtDBName.Size = new System.Drawing.Size(190, 23);
            this.txtDBName.TabIndex = 4;
            this.txtDBName.Text = "WSOP_10";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "DB Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "DB URL";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Site ID";
            // 
            // txtSiteID
            // 
            this.txtSiteID.Location = new System.Drawing.Point(82, 103);
            this.txtSiteID.Name = "txtSiteID";
            this.txtSiteID.Size = new System.Drawing.Size(190, 23);
            this.txtSiteID.TabIndex = 7;
            this.txtSiteID.Text = "10";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "DB Type";
            // 
            // btnDBToXML
            // 
            this.btnDBToXML.Location = new System.Drawing.Point(175, 165);
            this.btnDBToXML.Name = "btnDBToXML";
            this.btnDBToXML.Size = new System.Drawing.Size(97, 23);
            this.btnDBToXML.TabIndex = 10;
            this.btnDBToXML.Text = "DB To XML";
            this.btnDBToXML.UseVisualStyleBackColor = true;
            this.btnDBToXML.Click += new System.EventHandler(this.btnDBToXML_Click);
            // 
            // btnReadXML
            // 
            this.btnReadXML.Location = new System.Drawing.Point(12, 165);
            this.btnReadXML.Name = "btnReadXML";
            this.btnReadXML.Size = new System.Drawing.Size(75, 23);
            this.btnReadXML.TabIndex = 11;
            this.btnReadXML.Text = "XML읽기";
            this.btnReadXML.UseVisualStyleBackColor = true;
            this.btnReadXML.Click += new System.EventHandler(this.btnReadXML_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 198);
            this.Controls.Add(this.btnReadXML);
            this.Controls.Add(this.btnDBToXML);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSiteID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDBName);
            this.Controls.Add(this.lblConnectDB);
            this.Controls.Add(this.btnConnet);
            this.Controls.Add(this.cbDBType);
            this.Controls.Add(this.txtWebServerURL);
            this.Name = "Form1";
            this.Text = "XML읽기";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtWebServerURL;
        private System.Windows.Forms.ComboBox cbDBType;
        private System.Windows.Forms.Button btnConnet;
        private System.Windows.Forms.Label lblConnectDB;
        private System.Windows.Forms.TextBox txtDBName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSiteID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnDBToXML;
        private System.Windows.Forms.Button btnReadXML;
    }
}

