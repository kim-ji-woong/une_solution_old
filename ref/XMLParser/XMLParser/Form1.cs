using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreSafe
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private Variables<UserVariable> mUserVars = null;
        private Variables<Enums> mEnumVars = null;

        private void button1_Click(object sender, EventArgs e)
        {  
            if( openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string szXmlFile = openFileDialog1.FileName;

                XMLReader<UserVariable> reader = new XMLReader<UserVariable>();
                reader.ReadXML(szXmlFile);

                mUserVars = reader.Variables;               

                XMLReader<Enums> reader2 = new XMLReader<Enums>();
                reader2.ReadXML(szXmlFile);

                mEnumVars = reader2.Variables;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if( saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string szXmlFile = saveFileDialog1.FileName;
                XMLWriter<UserVariable> writer = new XMLWriter<UserVariable>();
                writer.Variables = mUserVars;
                writer.SaveXML(szXmlFile);
            }           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string szXmlFile = saveFileDialog1.FileName;
                XMLWriter<Enums> writer2 = new XMLWriter<Enums>();
                writer2.Variables = mEnumVars;
                writer2.SaveXML(szXmlFile);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }       
    }
}
