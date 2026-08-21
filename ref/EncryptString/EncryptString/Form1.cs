using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBUtility2;

namespace EncryptString
{
    public partial class Form1 : Form
    {
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public Form1()
        {
            string strEnc = "AwVB0IrUXAghp5PlaWuqWg==";
            string strDec = DBUtility2.AES256Cipher.AES_decrypt(strEnc, key);
            System.Diagnostics.Trace.WriteLine(strDec);
            InitializeComponent();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (textBoxOrigin.Text.Length == 0)
                MessageBox.Show("원본 문자열을 입력해 주세요");
            else
            {
                string strEncrypt = DBUtility2.AES256Cipher.AES_encrypt(textBoxOrigin.Text, key);
                textBoxEncrypt.Text = strEncrypt;
            }
        }
    }
}
