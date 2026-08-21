namespace GDK_tester
{
    partial class form_text_in_search
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Root Device");
            this.STC_FROM = new System.Windows.Forms.Label();
            this.STC_TO = new System.Windows.Forms.Label();
            this.CHK_FIRST = new System.Windows.Forms.CheckBox();
            this.CHK_LAST = new System.Windows.Forms.CheckBox();
            this.DTP_FROM = new System.Windows.Forms.DateTimePicker();
            this.DTP_TO = new System.Windows.Forms.DateTimePicker();
            this.TRV_DEVICES = new System.Windows.Forms.TreeView();
            this.STC_NAME = new System.Windows.Forms.Label();
            this.STC_COMP = new System.Windows.Forms.Label();
            this.STC_VALUE = new System.Windows.Forms.Label();
            this.STC_COLUMN = new System.Windows.Forms.Label();
            this.STC_LINE = new System.Windows.Forms.Label();
            this.OPT_0_EDT_NAME = new System.Windows.Forms.TextBox();
            this.OPT_0_EDT_VALUE = new System.Windows.Forms.TextBox();
            this.OPT_0_EDT_COLUMN = new System.Windows.Forms.TextBox();
            this.OPT_0_EDT_LINE = new System.Windows.Forms.TextBox();
            this.OPT_0_CBX_COMP = new System.Windows.Forms.ComboBox();
            this.OPT_1_EDT_NAME = new System.Windows.Forms.TextBox();
            this.OPT_1_EDT_VALUE = new System.Windows.Forms.TextBox();
            this.OPT_1_EDT_COLUMN = new System.Windows.Forms.TextBox();
            this.OPT_1_EDT_LINE = new System.Windows.Forms.TextBox();
            this.OPT_1_CBX_TYPE = new System.Windows.Forms.ComboBox();
            this.OPT_1_CBX_COMP = new System.Windows.Forms.ComboBox();
            this.OPT_2_EDT_NAME = new System.Windows.Forms.TextBox();
            this.OPT_2_EDT_VALUE = new System.Windows.Forms.TextBox();
            this.OPT_2_EDT_COLUMN = new System.Windows.Forms.TextBox();
            this.OPT_2_EDT_LINE = new System.Windows.Forms.TextBox();
            this.OPT_2_CBX_TYPE = new System.Windows.Forms.ComboBox();
            this.OPT_2_CBX_COMP = new System.Windows.Forms.ComboBox();
            this.OPT_3_EDT_NAME = new System.Windows.Forms.TextBox();
            this.OPT_3_EDT_VALUE = new System.Windows.Forms.TextBox();
            this.OPT_3_EDT_COLUMN = new System.Windows.Forms.TextBox();
            this.OPT_3_EDT_LINE = new System.Windows.Forms.TextBox();
            this.OPT_3_CBX_TYPE = new System.Windows.Forms.ComboBox();
            this.OPT_3_CBX_COMP = new System.Windows.Forms.ComboBox();
            this.OPT_4_EDT_NAME = new System.Windows.Forms.TextBox();
            this.OPT_4_EDT_VALUE = new System.Windows.Forms.TextBox();
            this.OPT_4_EDT_COLUMN = new System.Windows.Forms.TextBox();
            this.OPT_4_EDT_LINE = new System.Windows.Forms.TextBox();
            this.OPT_4_CBX_TYPE = new System.Windows.Forms.ComboBox();
            this.OPT_4_CBX_COMP = new System.Windows.Forms.ComboBox();
            this.CHK_CASE_SENSITIVE = new System.Windows.Forms.CheckBox();
            this.CHK_EXACT_WORD = new System.Windows.Forms.CheckBox();
            this.CHK_TRANSACTION_WISE = new System.Windows.Forms.CheckBox();
            this.BTN_OK = new System.Windows.Forms.Button();
            this.BTN_CANCEL = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // STC_FROM
            // 
            this.STC_FROM.Location = new System.Drawing.Point(12, 11);
            this.STC_FROM.Name = "STC_FROM";
            this.STC_FROM.Size = new System.Drawing.Size(55, 17);
            this.STC_FROM.TabIndex = 25;
            this.STC_FROM.Text = "From :";
            this.STC_FROM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_TO
            // 
            this.STC_TO.Location = new System.Drawing.Point(12, 34);
            this.STC_TO.Name = "STC_TO";
            this.STC_TO.Size = new System.Drawing.Size(55, 17);
            this.STC_TO.TabIndex = 26;
            this.STC_TO.Text = "To :";
            this.STC_TO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CHK_FIRST
            // 
            this.CHK_FIRST.AutoSize = true;
            this.CHK_FIRST.Location = new System.Drawing.Point(73, 12);
            this.CHK_FIRST.Name = "CHK_FIRST";
            this.CHK_FIRST.Size = new System.Drawing.Size(47, 17);
            this.CHK_FIRST.TabIndex = 0;
            this.CHK_FIRST.Text = "First";
            this.CHK_FIRST.UseVisualStyleBackColor = true;
            this.CHK_FIRST.CheckedChanged += new System.EventHandler(this.on_chk_first);
            // 
            // CHK_LAST
            // 
            this.CHK_LAST.AutoSize = true;
            this.CHK_LAST.Location = new System.Drawing.Point(73, 35);
            this.CHK_LAST.Name = "CHK_LAST";
            this.CHK_LAST.Size = new System.Drawing.Size(46, 17);
            this.CHK_LAST.TabIndex = 1;
            this.CHK_LAST.Text = "Last";
            this.CHK_LAST.UseVisualStyleBackColor = true;
            this.CHK_LAST.CheckedChanged += new System.EventHandler(this.on_chk_last);
            // 
            // DTP_FROM
            // 
            this.DTP_FROM.CustomFormat = " yyyy-MM-dd HH:mm:ss";
            this.DTP_FROM.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_FROM.Location = new System.Drawing.Point(126, 9);
            this.DTP_FROM.Name = "DTP_FROM";
            this.DTP_FROM.Size = new System.Drawing.Size(148, 20);
            this.DTP_FROM.TabIndex = 2;
            this.DTP_FROM.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.on_dtp_mouse_wheel);
            // 
            // DTP_TO
            // 
            this.DTP_TO.CustomFormat = " yyyy-MM-dd HH:mm:ss";
            this.DTP_TO.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_TO.Location = new System.Drawing.Point(125, 35);
            this.DTP_TO.Name = "DTP_TO";
            this.DTP_TO.Size = new System.Drawing.Size(149, 20);
            this.DTP_TO.TabIndex = 3;
            this.DTP_TO.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.on_dtp_mouse_wheel);
            // 
            // TRV_DEVICES
            // 
            this.TRV_DEVICES.CheckBoxes = true;
            this.TRV_DEVICES.Location = new System.Drawing.Point(12, 61);
            this.TRV_DEVICES.Name = "TRV_DEVICES";
            treeNode1.Name = "ROOT_DEVICE";
            treeNode1.Text = "Root Device";
            this.TRV_DEVICES.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.TRV_DEVICES.Size = new System.Drawing.Size(390, 200);
            this.TRV_DEVICES.TabIndex = 4;
            // 
            // STC_NAME
            // 
            this.STC_NAME.AutoSize = true;
            this.STC_NAME.Location = new System.Drawing.Point(67, 271);
            this.STC_NAME.Name = "STC_NAME";
            this.STC_NAME.Size = new System.Drawing.Size(34, 13);
            this.STC_NAME.TabIndex = 27;
            this.STC_NAME.Text = "Name";
            // 
            // STC_COMP
            // 
            this.STC_COMP.AutoSize = true;
            this.STC_COMP.Location = new System.Drawing.Point(163, 270);
            this.STC_COMP.Name = "STC_COMP";
            this.STC_COMP.Size = new System.Drawing.Size(38, 13);
            this.STC_COMP.TabIndex = 28;
            this.STC_COMP.Text = "Comp.";
            // 
            // STC_VALUE
            // 
            this.STC_VALUE.AutoSize = true;
            this.STC_VALUE.Location = new System.Drawing.Point(219, 270);
            this.STC_VALUE.Name = "STC_VALUE";
            this.STC_VALUE.Size = new System.Drawing.Size(33, 13);
            this.STC_VALUE.TabIndex = 29;
            this.STC_VALUE.Text = "Value";
            // 
            // STC_COLUMN
            // 
            this.STC_COLUMN.AutoSize = true;
            this.STC_COLUMN.Location = new System.Drawing.Point(315, 271);
            this.STC_COLUMN.Name = "STC_COLUMN";
            this.STC_COLUMN.Size = new System.Drawing.Size(42, 13);
            this.STC_COLUMN.TabIndex = 30;
            this.STC_COLUMN.Text = "Column";
            // 
            // STC_LINE
            // 
            this.STC_LINE.AutoSize = true;
            this.STC_LINE.Location = new System.Drawing.Point(361, 270);
            this.STC_LINE.Name = "STC_LINE";
            this.STC_LINE.Size = new System.Drawing.Size(26, 13);
            this.STC_LINE.TabIndex = 31;
            this.STC_LINE.Text = "Line";
            // 
            // OPT_0_EDT_NAME
            // 
            this.OPT_0_EDT_NAME.Location = new System.Drawing.Point(68, 287);
            this.OPT_0_EDT_NAME.Name = "OPT_0_EDT_NAME";
            this.OPT_0_EDT_NAME.Size = new System.Drawing.Size(90, 20);
            this.OPT_0_EDT_NAME.TabIndex = 5;
            // 
            // OPT_0_EDT_VALUE
            // 
            this.OPT_0_EDT_VALUE.Location = new System.Drawing.Point(220, 286);
            this.OPT_0_EDT_VALUE.Name = "OPT_0_EDT_VALUE";
            this.OPT_0_EDT_VALUE.Size = new System.Drawing.Size(90, 20);
            this.OPT_0_EDT_VALUE.TabIndex = 7;
            // 
            // OPT_0_EDT_COLUMN
            // 
            this.OPT_0_EDT_COLUMN.Location = new System.Drawing.Point(316, 287);
            this.OPT_0_EDT_COLUMN.Name = "OPT_0_EDT_COLUMN";
            this.OPT_0_EDT_COLUMN.Size = new System.Drawing.Size(40, 20);
            this.OPT_0_EDT_COLUMN.TabIndex = 8;
            // 
            // OPT_0_EDT_LINE
            // 
            this.OPT_0_EDT_LINE.Location = new System.Drawing.Point(362, 287);
            this.OPT_0_EDT_LINE.Name = "OPT_0_EDT_LINE";
            this.OPT_0_EDT_LINE.Size = new System.Drawing.Size(40, 20);
            this.OPT_0_EDT_LINE.TabIndex = 9;
            // 
            // OPT_0_CBX_COMP
            // 
            this.OPT_0_CBX_COMP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPT_0_CBX_COMP.FormattingEnabled = true;
            this.OPT_0_CBX_COMP.Items.AddRange(new object[] {
            "",
            "<",
            "<=",
            "=",
            ">=",
            ">"});
            this.OPT_0_CBX_COMP.Location = new System.Drawing.Point(164, 286);
            this.OPT_0_CBX_COMP.Name = "OPT_0_CBX_COMP";
            this.OPT_0_CBX_COMP.Size = new System.Drawing.Size(50, 21);
            this.OPT_0_CBX_COMP.TabIndex = 6;
            // 
            // OPT_1_EDT_NAME
            // 
            this.OPT_1_EDT_NAME.Location = new System.Drawing.Point(68, 313);
            this.OPT_1_EDT_NAME.Name = "OPT_1_EDT_NAME";
            this.OPT_1_EDT_NAME.Size = new System.Drawing.Size(90, 20);
            this.OPT_1_EDT_NAME.TabIndex = 11;
            // 
            // OPT_1_EDT_VALUE
            // 
            this.OPT_1_EDT_VALUE.Location = new System.Drawing.Point(220, 312);
            this.OPT_1_EDT_VALUE.Name = "OPT_1_EDT_VALUE";
            this.OPT_1_EDT_VALUE.Size = new System.Drawing.Size(90, 20);
            this.OPT_1_EDT_VALUE.TabIndex = 13;
            // 
            // OPT_1_EDT_COLUMN
            // 
            this.OPT_1_EDT_COLUMN.Location = new System.Drawing.Point(316, 313);
            this.OPT_1_EDT_COLUMN.Name = "OPT_1_EDT_COLUMN";
            this.OPT_1_EDT_COLUMN.Size = new System.Drawing.Size(40, 20);
            this.OPT_1_EDT_COLUMN.TabIndex = 14;
            // 
            // OPT_1_EDT_LINE
            // 
            this.OPT_1_EDT_LINE.Location = new System.Drawing.Point(362, 313);
            this.OPT_1_EDT_LINE.Name = "OPT_1_EDT_LINE";
            this.OPT_1_EDT_LINE.Size = new System.Drawing.Size(40, 20);
            this.OPT_1_EDT_LINE.TabIndex = 15;
            // 
            // OPT_1_CBX_TYPE
            // 
            this.OPT_1_CBX_TYPE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPT_1_CBX_TYPE.FormattingEnabled = true;
            this.OPT_1_CBX_TYPE.Items.AddRange(new object[] {
            "",
            "AND",
            "OR"});
            this.OPT_1_CBX_TYPE.Location = new System.Drawing.Point(12, 313);
            this.OPT_1_CBX_TYPE.Name = "OPT_1_CBX_TYPE";
            this.OPT_1_CBX_TYPE.Size = new System.Drawing.Size(50, 21);
            this.OPT_1_CBX_TYPE.TabIndex = 10;
            this.OPT_1_CBX_TYPE.SelectedIndexChanged += new System.EventHandler(this.on_opt_type_changed);
            // 
            // OPT_1_CBX_COMP
            // 
            this.OPT_1_CBX_COMP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPT_1_CBX_COMP.FormattingEnabled = true;
            this.OPT_1_CBX_COMP.Items.AddRange(new object[] {
            "",
            "<",
            "<=",
            "=",
            ">=",
            ">"});
            this.OPT_1_CBX_COMP.Location = new System.Drawing.Point(164, 312);
            this.OPT_1_CBX_COMP.Name = "OPT_1_CBX_COMP";
            this.OPT_1_CBX_COMP.Size = new System.Drawing.Size(50, 21);
            this.OPT_1_CBX_COMP.TabIndex = 12;
            // 
            // OPT_2_EDT_NAME
            // 
            this.OPT_2_EDT_NAME.Location = new System.Drawing.Point(68, 339);
            this.OPT_2_EDT_NAME.Name = "OPT_2_EDT_NAME";
            this.OPT_2_EDT_NAME.Size = new System.Drawing.Size(90, 20);
            this.OPT_2_EDT_NAME.TabIndex = 17;
            // 
            // OPT_2_EDT_VALUE
            // 
            this.OPT_2_EDT_VALUE.Location = new System.Drawing.Point(220, 338);
            this.OPT_2_EDT_VALUE.Name = "OPT_2_EDT_VALUE";
            this.OPT_2_EDT_VALUE.Size = new System.Drawing.Size(90, 20);
            this.OPT_2_EDT_VALUE.TabIndex = 19;
            // 
            // OPT_2_EDT_COLUMN
            // 
            this.OPT_2_EDT_COLUMN.Location = new System.Drawing.Point(316, 339);
            this.OPT_2_EDT_COLUMN.Name = "OPT_2_EDT_COLUMN";
            this.OPT_2_EDT_COLUMN.Size = new System.Drawing.Size(40, 20);
            this.OPT_2_EDT_COLUMN.TabIndex = 20;
            // 
            // OPT_2_EDT_LINE
            // 
            this.OPT_2_EDT_LINE.Location = new System.Drawing.Point(362, 339);
            this.OPT_2_EDT_LINE.Name = "OPT_2_EDT_LINE";
            this.OPT_2_EDT_LINE.Size = new System.Drawing.Size(40, 20);
            this.OPT_2_EDT_LINE.TabIndex = 21;
            // 
            // OPT_2_CBX_TYPE
            // 
            this.OPT_2_CBX_TYPE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPT_2_CBX_TYPE.FormattingEnabled = true;
            this.OPT_2_CBX_TYPE.Items.AddRange(new object[] {
            "",
            "AND",
            "OR"});
            this.OPT_2_CBX_TYPE.Location = new System.Drawing.Point(12, 338);
            this.OPT_2_CBX_TYPE.Name = "OPT_2_CBX_TYPE";
            this.OPT_2_CBX_TYPE.Size = new System.Drawing.Size(50, 21);
            this.OPT_2_CBX_TYPE.TabIndex = 16;
            this.OPT_2_CBX_TYPE.SelectedIndexChanged += new System.EventHandler(this.on_opt_type_changed);
            // 
            // OPT_2_CBX_COMP
            // 
            this.OPT_2_CBX_COMP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPT_2_CBX_COMP.FormattingEnabled = true;
            this.OPT_2_CBX_COMP.Items.AddRange(new object[] {
            "",
            "<",
            "<=",
            "=",
            ">=",
            ">"});
            this.OPT_2_CBX_COMP.Location = new System.Drawing.Point(164, 338);
            this.OPT_2_CBX_COMP.Name = "OPT_2_CBX_COMP";
            this.OPT_2_CBX_COMP.Size = new System.Drawing.Size(50, 21);
            this.OPT_2_CBX_COMP.TabIndex = 18;
            // 
            // OPT_3_EDT_NAME
            // 
            this.OPT_3_EDT_NAME.Location = new System.Drawing.Point(68, 365);
            this.OPT_3_EDT_NAME.Name = "OPT_3_EDT_NAME";
            this.OPT_3_EDT_NAME.Size = new System.Drawing.Size(90, 20);
            this.OPT_3_EDT_NAME.TabIndex = 23;
            // 
            // OPT_3_EDT_VALUE
            // 
            this.OPT_3_EDT_VALUE.Location = new System.Drawing.Point(220, 364);
            this.OPT_3_EDT_VALUE.Name = "OPT_3_EDT_VALUE";
            this.OPT_3_EDT_VALUE.Size = new System.Drawing.Size(90, 20);
            this.OPT_3_EDT_VALUE.TabIndex = 25;
            // 
            // OPT_3_EDT_COLUMN
            // 
            this.OPT_3_EDT_COLUMN.Location = new System.Drawing.Point(316, 365);
            this.OPT_3_EDT_COLUMN.Name = "OPT_3_EDT_COLUMN";
            this.OPT_3_EDT_COLUMN.Size = new System.Drawing.Size(40, 20);
            this.OPT_3_EDT_COLUMN.TabIndex = 26;
            // 
            // OPT_3_EDT_LINE
            // 
            this.OPT_3_EDT_LINE.Location = new System.Drawing.Point(362, 365);
            this.OPT_3_EDT_LINE.Name = "OPT_3_EDT_LINE";
            this.OPT_3_EDT_LINE.Size = new System.Drawing.Size(40, 20);
            this.OPT_3_EDT_LINE.TabIndex = 27;
            // 
            // OPT_3_CBX_TYPE
            // 
            this.OPT_3_CBX_TYPE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPT_3_CBX_TYPE.FormattingEnabled = true;
            this.OPT_3_CBX_TYPE.Items.AddRange(new object[] {
            "",
            "AND",
            "OR"});
            this.OPT_3_CBX_TYPE.Location = new System.Drawing.Point(12, 365);
            this.OPT_3_CBX_TYPE.Name = "OPT_3_CBX_TYPE";
            this.OPT_3_CBX_TYPE.Size = new System.Drawing.Size(50, 21);
            this.OPT_3_CBX_TYPE.TabIndex = 22;
            this.OPT_3_CBX_TYPE.SelectedIndexChanged += new System.EventHandler(this.on_opt_type_changed);
            // 
            // OPT_3_CBX_COMP
            // 
            this.OPT_3_CBX_COMP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPT_3_CBX_COMP.FormattingEnabled = true;
            this.OPT_3_CBX_COMP.Items.AddRange(new object[] {
            "",
            "<",
            "<=",
            "=",
            ">=",
            ">"});
            this.OPT_3_CBX_COMP.Location = new System.Drawing.Point(164, 364);
            this.OPT_3_CBX_COMP.Name = "OPT_3_CBX_COMP";
            this.OPT_3_CBX_COMP.Size = new System.Drawing.Size(50, 21);
            this.OPT_3_CBX_COMP.TabIndex = 24;
            // 
            // OPT_4_EDT_NAME
            // 
            this.OPT_4_EDT_NAME.Location = new System.Drawing.Point(68, 391);
            this.OPT_4_EDT_NAME.Name = "OPT_4_EDT_NAME";
            this.OPT_4_EDT_NAME.Size = new System.Drawing.Size(90, 20);
            this.OPT_4_EDT_NAME.TabIndex = 29;
            // 
            // OPT_4_EDT_VALUE
            // 
            this.OPT_4_EDT_VALUE.Location = new System.Drawing.Point(220, 390);
            this.OPT_4_EDT_VALUE.Name = "OPT_4_EDT_VALUE";
            this.OPT_4_EDT_VALUE.Size = new System.Drawing.Size(90, 20);
            this.OPT_4_EDT_VALUE.TabIndex = 31;
            // 
            // OPT_4_EDT_COLUMN
            // 
            this.OPT_4_EDT_COLUMN.Location = new System.Drawing.Point(316, 391);
            this.OPT_4_EDT_COLUMN.Name = "OPT_4_EDT_COLUMN";
            this.OPT_4_EDT_COLUMN.Size = new System.Drawing.Size(40, 20);
            this.OPT_4_EDT_COLUMN.TabIndex = 32;
            // 
            // OPT_4_EDT_LINE
            // 
            this.OPT_4_EDT_LINE.Location = new System.Drawing.Point(362, 391);
            this.OPT_4_EDT_LINE.Name = "OPT_4_EDT_LINE";
            this.OPT_4_EDT_LINE.Size = new System.Drawing.Size(40, 20);
            this.OPT_4_EDT_LINE.TabIndex = 33;
            // 
            // OPT_4_CBX_TYPE
            // 
            this.OPT_4_CBX_TYPE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPT_4_CBX_TYPE.FormattingEnabled = true;
            this.OPT_4_CBX_TYPE.Items.AddRange(new object[] {
            "",
            "AND",
            "OR"});
            this.OPT_4_CBX_TYPE.Location = new System.Drawing.Point(12, 390);
            this.OPT_4_CBX_TYPE.Name = "OPT_4_CBX_TYPE";
            this.OPT_4_CBX_TYPE.Size = new System.Drawing.Size(50, 21);
            this.OPT_4_CBX_TYPE.TabIndex = 28;
            this.OPT_4_CBX_TYPE.SelectedIndexChanged += new System.EventHandler(this.on_opt_type_changed);
            // 
            // OPT_4_CBX_COMP
            // 
            this.OPT_4_CBX_COMP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPT_4_CBX_COMP.FormattingEnabled = true;
            this.OPT_4_CBX_COMP.Items.AddRange(new object[] {
            "",
            "<",
            "<=",
            "=",
            ">=",
            ">"});
            this.OPT_4_CBX_COMP.Location = new System.Drawing.Point(164, 390);
            this.OPT_4_CBX_COMP.Name = "OPT_4_CBX_COMP";
            this.OPT_4_CBX_COMP.Size = new System.Drawing.Size(50, 21);
            this.OPT_4_CBX_COMP.TabIndex = 30;
            // 
            // CHK_CASE_SENSITIVE
            // 
            this.CHK_CASE_SENSITIVE.AutoSize = true;
            this.CHK_CASE_SENSITIVE.Location = new System.Drawing.Point(68, 427);
            this.CHK_CASE_SENSITIVE.Name = "CHK_CASE_SENSITIVE";
            this.CHK_CASE_SENSITIVE.Size = new System.Drawing.Size(96, 17);
            this.CHK_CASE_SENSITIVE.TabIndex = 34;
            this.CHK_CASE_SENSITIVE.Text = "Case Sensitive";
            this.CHK_CASE_SENSITIVE.UseVisualStyleBackColor = true;
            // 
            // CHK_EXACT_WORD
            // 
            this.CHK_EXACT_WORD.AutoSize = true;
            this.CHK_EXACT_WORD.Location = new System.Drawing.Point(68, 447);
            this.CHK_EXACT_WORD.Name = "CHK_EXACT_WORD";
            this.CHK_EXACT_WORD.Size = new System.Drawing.Size(82, 17);
            this.CHK_EXACT_WORD.TabIndex = 35;
            this.CHK_EXACT_WORD.Text = "Exact Word";
            this.CHK_EXACT_WORD.UseVisualStyleBackColor = true;
            // 
            // CHK_TRANSACTION_WISE
            // 
            this.CHK_TRANSACTION_WISE.AutoSize = true;
            this.CHK_TRANSACTION_WISE.Location = new System.Drawing.Point(68, 467);
            this.CHK_TRANSACTION_WISE.Name = "CHK_TRANSACTION_WISE";
            this.CHK_TRANSACTION_WISE.Size = new System.Drawing.Size(222, 17);
            this.CHK_TRANSACTION_WISE.TabIndex = 36;
            this.CHK_TRANSACTION_WISE.Text = "Eliminate Duplication within a Transaction";
            this.CHK_TRANSACTION_WISE.UseVisualStyleBackColor = true;
            // 
            // BTN_OK
            // 
            this.BTN_OK.Location = new System.Drawing.Point(246, 500);
            this.BTN_OK.Name = "BTN_OK";
            this.BTN_OK.Size = new System.Drawing.Size(75, 23);
            this.BTN_OK.TabIndex = 37;
            this.BTN_OK.Text = "OK";
            this.BTN_OK.UseVisualStyleBackColor = true;
            this.BTN_OK.Click += new System.EventHandler(this.on_btn_OK);
            // 
            // BTN_CANCEL
            // 
            this.BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BTN_CANCEL.Location = new System.Drawing.Point(327, 500);
            this.BTN_CANCEL.Name = "BTN_CANCEL";
            this.BTN_CANCEL.Size = new System.Drawing.Size(75, 23);
            this.BTN_CANCEL.TabIndex = 38;
            this.BTN_CANCEL.Text = "Cancel";
            this.BTN_CANCEL.UseVisualStyleBackColor = true;
            // 
            // form_text_in_search
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 535);
            this.Controls.Add(this.STC_FROM);
            this.Controls.Add(this.STC_TO);
            this.Controls.Add(this.CHK_FIRST);
            this.Controls.Add(this.CHK_LAST);
            this.Controls.Add(this.DTP_FROM);
            this.Controls.Add(this.DTP_TO);
            this.Controls.Add(this.TRV_DEVICES);
            this.Controls.Add(this.STC_NAME);
            this.Controls.Add(this.STC_COMP);
            this.Controls.Add(this.STC_VALUE);
            this.Controls.Add(this.STC_COLUMN);
            this.Controls.Add(this.STC_LINE);
            this.Controls.Add(this.OPT_0_EDT_NAME);
            this.Controls.Add(this.OPT_0_EDT_VALUE);
            this.Controls.Add(this.OPT_0_EDT_COLUMN);
            this.Controls.Add(this.OPT_0_EDT_LINE);
            this.Controls.Add(this.OPT_0_CBX_COMP);
            this.Controls.Add(this.OPT_1_EDT_NAME);
            this.Controls.Add(this.OPT_1_EDT_VALUE);
            this.Controls.Add(this.OPT_1_EDT_COLUMN);
            this.Controls.Add(this.OPT_1_EDT_LINE);
            this.Controls.Add(this.OPT_1_CBX_TYPE);
            this.Controls.Add(this.OPT_1_CBX_COMP);
            this.Controls.Add(this.OPT_2_EDT_LINE);
            this.Controls.Add(this.OPT_2_EDT_COLUMN);
            this.Controls.Add(this.OPT_2_EDT_VALUE);
            this.Controls.Add(this.OPT_2_EDT_NAME);
            this.Controls.Add(this.OPT_2_CBX_TYPE);
            this.Controls.Add(this.OPT_2_CBX_COMP);
            this.Controls.Add(this.OPT_3_EDT_NAME);
            this.Controls.Add(this.OPT_3_EDT_VALUE);
            this.Controls.Add(this.OPT_3_EDT_COLUMN);
            this.Controls.Add(this.OPT_3_EDT_LINE);
            this.Controls.Add(this.OPT_3_CBX_TYPE);
            this.Controls.Add(this.OPT_3_CBX_COMP);
            this.Controls.Add(this.OPT_4_EDT_NAME);
            this.Controls.Add(this.OPT_4_EDT_VALUE);
            this.Controls.Add(this.OPT_4_EDT_COLUMN);
            this.Controls.Add(this.OPT_4_EDT_LINE);
            this.Controls.Add(this.OPT_4_CBX_TYPE);
            this.Controls.Add(this.OPT_4_CBX_COMP);
            this.Controls.Add(this.CHK_CASE_SENSITIVE);
            this.Controls.Add(this.CHK_EXACT_WORD);
            this.Controls.Add(this.CHK_TRANSACTION_WISE);
            this.Controls.Add(this.BTN_OK);
            this.Controls.Add(this.BTN_CANCEL);
            this.AcceptButton = this.BTN_OK;
            this.CancelButton = this.BTN_CANCEL;
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_text_in_search";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TextIn Search";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label STC_FROM;
        private System.Windows.Forms.Label STC_TO;
        private System.Windows.Forms.CheckBox CHK_FIRST;
        private System.Windows.Forms.CheckBox CHK_LAST;
        private System.Windows.Forms.DateTimePicker DTP_FROM;
        private System.Windows.Forms.DateTimePicker DTP_TO;
        private System.Windows.Forms.TreeView TRV_DEVICES;
        private System.Windows.Forms.Label STC_NAME;
        private System.Windows.Forms.Label STC_COMP;
        private System.Windows.Forms.Label STC_VALUE;
        private System.Windows.Forms.Label STC_COLUMN;
        private System.Windows.Forms.Label STC_LINE;
        private System.Windows.Forms.TextBox OPT_0_EDT_NAME;
        private System.Windows.Forms.TextBox OPT_0_EDT_VALUE;
        private System.Windows.Forms.TextBox OPT_0_EDT_COLUMN;
        private System.Windows.Forms.TextBox OPT_0_EDT_LINE;
        private System.Windows.Forms.ComboBox OPT_0_CBX_COMP;
        private System.Windows.Forms.TextBox OPT_1_EDT_NAME;
        private System.Windows.Forms.TextBox OPT_1_EDT_VALUE;
        private System.Windows.Forms.TextBox OPT_1_EDT_COLUMN;
        private System.Windows.Forms.TextBox OPT_1_EDT_LINE;
        private System.Windows.Forms.ComboBox OPT_1_CBX_TYPE;
        private System.Windows.Forms.ComboBox OPT_1_CBX_COMP;
        private System.Windows.Forms.TextBox OPT_2_EDT_NAME;
        private System.Windows.Forms.TextBox OPT_2_EDT_VALUE;
        private System.Windows.Forms.TextBox OPT_2_EDT_COLUMN;
        private System.Windows.Forms.TextBox OPT_2_EDT_LINE;
        private System.Windows.Forms.ComboBox OPT_2_CBX_TYPE;
        private System.Windows.Forms.ComboBox OPT_2_CBX_COMP;
        private System.Windows.Forms.TextBox OPT_3_EDT_NAME;
        private System.Windows.Forms.TextBox OPT_3_EDT_VALUE;
        private System.Windows.Forms.TextBox OPT_3_EDT_COLUMN;
        private System.Windows.Forms.TextBox OPT_3_EDT_LINE;
        private System.Windows.Forms.ComboBox OPT_3_CBX_TYPE;
        private System.Windows.Forms.ComboBox OPT_3_CBX_COMP;
        private System.Windows.Forms.TextBox OPT_4_EDT_NAME;
        private System.Windows.Forms.TextBox OPT_4_EDT_VALUE;
        private System.Windows.Forms.TextBox OPT_4_EDT_COLUMN;
        private System.Windows.Forms.TextBox OPT_4_EDT_LINE;
        private System.Windows.Forms.ComboBox OPT_4_CBX_TYPE;
        private System.Windows.Forms.ComboBox OPT_4_CBX_COMP;
        private System.Windows.Forms.CheckBox CHK_CASE_SENSITIVE;
        private System.Windows.Forms.CheckBox CHK_EXACT_WORD;
        private System.Windows.Forms.CheckBox CHK_TRANSACTION_WISE;
        private System.Windows.Forms.Button BTN_OK;
        private System.Windows.Forms.Button BTN_CANCEL;
    }
}