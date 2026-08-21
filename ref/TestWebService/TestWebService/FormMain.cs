using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace TestWebService
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            ReadRemember();
        }

        private void ReadRemember()
        {
            if (File.Exists("remember.dat") == false)
                return;

            StreamReader reader = new StreamReader("remember.dat", Encoding.UTF8);

            string strURL = reader.ReadLine().Trim();

            if (strURL.Length == 0)
            {
                reader.Close();
                return;
            }

            string strRequest = "";

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();
                strRequest += strLine;
            }

            textBoxURL.Text = strURL;
            textBoxRequest.Text = strRequest;
            reader.Close();
        }

        private void Remember(string strRequest, string strURL)
        {
            StreamWriter writer = new StreamWriter("remember.dat", false, Encoding.UTF8);
            writer.WriteLine(strURL);
            writer.Write(strRequest);
            writer.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string strRequest = textBoxRequest.Text.Trim();
            string strURL = textBoxURL.Text.Trim();

            if (strURL.Length == 0)
            {
                textBoxURL.Focus();
                MessageBox.Show("URL을 입력하세요.");
                return;
            }

            if (strRequest.Length == 0)
            {
                textBoxRequest.Focus();
                MessageBox.Show("요청 내용을 입력하세요.");
                return;
            }

            Remember(strRequest, strURL);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] bytes = Encoding.ASCII.GetBytes(strRequest);
            request.ContentLength = bytes.Length; // 바이트수 지정

            string strResponse = "";

            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream);

                strResponse = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

            }
            catch (WebException ex)
            {
                string strErrorMessage = ex.Message;
                textBoxResponse.Text = strErrorMessage;
                return;
            }

            if (strResponse == null)
            {
                textBoxResponse.Text = "No Response";
                return;
            }

            textBoxResponse.Text = strResponse;
        }
    }
}
