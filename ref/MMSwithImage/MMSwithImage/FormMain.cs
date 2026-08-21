using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace MMSwithImage
{
    public partial class FormMain : Form
    {
        //private string m_strImageType = "";
        private string m_strImagePath = "";

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Image files (*.bmp, *.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.bmp; *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            dlg.Title = "전송할 이미지를 선택하세요.";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Image img = Image.FromFile(dlg.FileName);
                pbImage.Image = img;
                pbImage.Size = img.Size;

                /*int nIndex = dlg.FileName.LastIndexOf('.');

                if (nIndex < 0)
                    m_strImageType = "";
                else
                    m_strImageType = dlg.FileName.Substring(nIndex + 1).ToLower();*/

                m_strImagePath = dlg.FileName;
            }
        }

        private void btnDeleteImage_Click(object sender, EventArgs e)
        {
            pbImage.Image = null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string strPhoneNumber = textBoxPhoneNumber.Text.Trim();

            if (strPhoneNumber.Length == 0)
            {
                textBoxPhoneNumber.Focus();
                MessageBox.Show("수신할 전화번호를 입력하세요");
            }
            else
            {
                string str = "";

                for (int i=0;i<strPhoneNumber.Length;i++)
                {
                    char ch = strPhoneNumber.ElementAt(i);

                    if (ch != '-' && ch != ' ')
                    {
                        if (ch < '0' || ch > '9')
                        {
                            textBoxPhoneNumber.Focus();
                            MessageBox.Show("전화번호는 숫자만 입력 가능합니다.");
                            break;
                        }

                        str += ch;
                    }
                }

                if (pbImage.Image == null)
                {
                    if (textBoxBody.Text.Length == 0)
                    {
                        textBoxBody.Focus();
                        MessageBox.Show("이미지나 텍스트 둘중 하나는 입력해야 합니다.");
                    }
                    else
                        SendMMS(str);
                }
                else
                    SendMMS(str);
            }
        }

        private void SendMMS(string strPhoneNumber)
        {
            string strImage = "";

            if (pbImage.Image != null)
            {
                //string strImageName = "aaa.jpg";
                //pbImage.Image.Save(strImageName, ImageFormat.Jpeg);

                int nIndex = m_strImagePath.LastIndexOf('\\');

                if (nIndex < 0)
                    strImage = m_strImagePath;
                else
                    strImage = m_strImagePath.Substring(nIndex + 1);
                //strImage = strImageName;

                UploadFile(m_strImagePath);
            }

            libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(3, "127.0.0.1");

            if (client != null)
            {
                if (strImage.Length > 0)
                    client.SendMMS("027144133", textBoxPhoneNumber.Text.Trim(), textBoxBody.Text.Trim(), textBoxTitle.Text.Trim(), libSMS.MessageContentMMS.ContentType.Image, strImage);
                else
                    client.SendMMS("027144133", textBoxPhoneNumber.Text.Trim(), textBoxBody.Text.Trim(), textBoxTitle.Text.Trim());
            }
        }

        private void UploadFile(string strFilePath)
        {
            int nIndex = strFilePath.LastIndexOf('\\');
            string strFileName = nIndex < 0 ? strFilePath : strFilePath.Substring(nIndex + 1);

            // Get the object used to communicate with the server.
            System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create("ftp://192.168.0.195:1021/" + strFileName);
            request.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new System.Net.NetworkCredential("anonymous", "");

            // Copy the contents of the file to the request stream.
            System.IO.FileStream stream = System.IO.File.Open(strFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
            byte[] fileContents = reader.ReadBytes((int)stream.Length);
            stream.Close();
            reader.Close();

            request.ContentLength = fileContents.Length;

            System.IO.Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse();
            response.Close();
        }
    }
}
