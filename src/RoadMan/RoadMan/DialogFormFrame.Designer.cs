namespace RoadMan
{
	partial class DialogFormFrame
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
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.Size = new System.Drawing.Size(459, 20);
			// 
			// panelLeft
			// 
			this.panelLeft.Size = new System.Drawing.Size(5, 329);
			// 
			// panelRight
			// 
			this.panelRight.Location = new System.Drawing.Point(454, 20);
			this.panelRight.Size = new System.Drawing.Size(5, 329);
			// 
			// panelBottom
			// 
			this.panelBottom.Location = new System.Drawing.Point(5, 349);
			this.panelBottom.Size = new System.Drawing.Size(449, 5);
			// 
			// panelLB
			// 
			this.panelLB.Location = new System.Drawing.Point(0, 349);
			// 
			// panelRB
			// 
			this.panelRB.Location = new System.Drawing.Point(454, 349);
			// 
			// labelTitle
			// 
			this.labelTitle.Location = new System.Drawing.Point(10, 10);
			this.labelTitle.Size = new System.Drawing.Size(0, 15);
			this.labelTitle.Text = "";
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(437, 2);
			// 
			// btnMax
			// 
			this.btnMax.Location = new System.Drawing.Point(419, 2);
			// 
			// btnMin
			// 
			this.btnMin.Location = new System.Drawing.Point(401, 2);
			// 
			// DialogFormFrame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.ClientSize = new System.Drawing.Size(459, 354);
			this.DoubleBuffered = true;
			this.Name = "DialogFormFrame";
			this.ShowCloseButton = true;
			this.ShowMaxButton = true;
			this.ShowMinButton = true;
			this.ShowPictureBoxTitle = true;
			this.Text = "";
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
	}
}