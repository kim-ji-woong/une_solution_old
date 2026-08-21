namespace SOPGen
{
    partial class FormProcess
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
            this.contextProcessMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddProcessMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.RenameProcessMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteProcessMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.AddGroupMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.contextGroupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RenameGroupMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowListMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteGroupMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.contextFormMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AutoAlignMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.label4Scroll = new System.Windows.Forms.Label();
            this.contextProcessMenu.SuspendLayout();
            this.contextGroupMenu.SuspendLayout();
            this.contextFormMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextProcessMenu
            // 
            this.contextProcessMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddProcessMenu,
            this.RenameProcessMenu,
            this.DeleteProcessMenu,
            this.AddGroupMenu});
            this.contextProcessMenu.Name = "contextProcessMenu";
            this.contextProcessMenu.Size = new System.Drawing.Size(151, 92);
            // 
            // AddProcessMenu
            // 
            this.AddProcessMenu.Name = "AddProcessMenu";
            this.AddProcessMenu.Size = new System.Drawing.Size(150, 22);
            this.AddProcessMenu.Text = "프로세스 추가";
            this.AddProcessMenu.Click += new System.EventHandler(this.OnMenuAddProcess);
            // 
            // RenameProcessMenu
            // 
            this.RenameProcessMenu.Name = "RenameProcessMenu";
            this.RenameProcessMenu.Size = new System.Drawing.Size(150, 22);
            this.RenameProcessMenu.Text = "프로세스 수정";
            this.RenameProcessMenu.Click += new System.EventHandler(this.OnMenuRenameProcess);
            // 
            // DeleteProcessMenu
            // 
            this.DeleteProcessMenu.Name = "DeleteProcessMenu";
            this.DeleteProcessMenu.Size = new System.Drawing.Size(150, 22);
            this.DeleteProcessMenu.Text = "프로세스 삭제";
            this.DeleteProcessMenu.Click += new System.EventHandler(this.OnMenuDeleteProcess);
            // 
            // AddGroupMenu
            // 
            this.AddGroupMenu.Name = "AddGroupMenu";
            this.AddGroupMenu.Size = new System.Drawing.Size(150, 22);
            this.AddGroupMenu.Text = "조직 등록";
            this.AddGroupMenu.Click += new System.EventHandler(this.OnMenuAddGroup);
            // 
            // contextGroupMenu
            // 
            this.contextGroupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RenameGroupMenu,
            this.ShowListMenu,
            this.DeleteGroupMenu});
            this.contextGroupMenu.Name = "contextGroupMenu";
            this.contextGroupMenu.Size = new System.Drawing.Size(191, 70);
            // 
            // RenameGroupMenu
            // 
            this.RenameGroupMenu.Name = "RenameGroupMenu";
            this.RenameGroupMenu.Size = new System.Drawing.Size(190, 22);
            this.RenameGroupMenu.Text = "조직 수정";
            this.RenameGroupMenu.Click += new System.EventHandler(this.OnMenuRenameGroup);
            // 
            // ShowListMenu
            // 
            this.ShowListMenu.Name = "ShowListMenu";
            this.ShowListMenu.Size = new System.Drawing.Size(190, 22);
            this.ShowListMenu.Text = "조직 리스트에서 선택";
            this.ShowListMenu.Click += new System.EventHandler(this.ShowListMenu_Click);
            // 
            // DeleteGroupMenu
            // 
            this.DeleteGroupMenu.Name = "DeleteGroupMenu";
            this.DeleteGroupMenu.Size = new System.Drawing.Size(190, 22);
            this.DeleteGroupMenu.Text = "조직 삭제";
            this.DeleteGroupMenu.Click += new System.EventHandler(this.OnMenuDeleteGroup);
            // 
            // contextFormMenu
            // 
            this.contextFormMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AutoAlignMenu});
            this.contextFormMenu.Name = "contextFormMenu";
            this.contextFormMenu.Size = new System.Drawing.Size(127, 26);
            // 
            // AutoAlignMenu
            // 
            this.AutoAlignMenu.Name = "AutoAlignMenu";
            this.AutoAlignMenu.Size = new System.Drawing.Size(126, 22);
            this.AutoAlignMenu.Text = "자동 정렬";
            this.AutoAlignMenu.Click += new System.EventHandler(this.OnMenuAutoAlign);
            // 
            // label4Scroll
            // 
            this.label4Scroll.AutoSize = true;
            this.label4Scroll.Location = new System.Drawing.Point(379, 237);
            this.label4Scroll.Name = "label4Scroll";
            this.label4Scroll.Size = new System.Drawing.Size(70, 12);
            this.label4Scroll.TabIndex = 3;
            this.label4Scroll.Text = "label4Scroll";
            // 
            // FormProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(500, 500);
            this.Controls.Add(this.label4Scroll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormProcess";
            this.ShowInTaskbar = false;
            this.Text = "FormProcess";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.contextProcessMenu.ResumeLayout(false);
            this.contextGroupMenu.ResumeLayout(false);
            this.contextFormMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextProcessMenu;
        private System.Windows.Forms.ToolStripMenuItem DeleteProcessMenu;
        private System.Windows.Forms.ToolStripMenuItem AddProcessMenu;
        private System.Windows.Forms.ToolStripMenuItem RenameProcessMenu;
        private System.Windows.Forms.ToolStripMenuItem AddGroupMenu;
        private System.Windows.Forms.ContextMenuStrip contextGroupMenu;
        private System.Windows.Forms.ToolStripMenuItem RenameGroupMenu;
        private System.Windows.Forms.ToolStripMenuItem DeleteGroupMenu;
        private System.Windows.Forms.ContextMenuStrip contextFormMenu;
        private System.Windows.Forms.ToolStripMenuItem AutoAlignMenu;
        private System.Windows.Forms.ToolStripMenuItem ShowListMenu;
        private System.Windows.Forms.Label label4Scroll;

    }
}