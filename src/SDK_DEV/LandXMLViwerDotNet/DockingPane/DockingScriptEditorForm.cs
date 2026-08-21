using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UBMLViewer
{
    public partial class FormScriptEditor : Form
    {
        public FormScriptEditor()
        {
            InitializeComponent();
            textBox1.AcceptsReturn = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ScriptProxy.Instance.RunPythonScript(textBox1.Text);
        }
    }
}
