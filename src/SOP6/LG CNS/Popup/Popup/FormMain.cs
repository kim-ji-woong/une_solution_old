using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace Popup
{
    public partial class FormMain : Form
    {
        private string m_strImageFolder = "Image";
        private string m_strImagePath = "";

        public FormMain(string strReceivers, string strMessage, string strImage, string strTitle)
        {
            InitializeComponent();

            SetImage(strImage);

            labelTitle.Text = strTitle;
            textBoxMessage.Text = strMessage;

        }

        private void SetImage(string strImage)
        {
            if (DownloadImage(strImage))
            {
                Image img = Image.FromFile(m_strImagePath);

                if (img != null)
                {
                    pbImage.Image = img;

                    int width = 0, height = 0;

                    if (pbImage.Size.Width < img.Size.Width)
                    {
                        int diff = this.Size.Width - pbImage.Size.Width;
                        width = img.Size.Width + diff;
                    }

                    if (pbImage.Size.Height < img.Size.Height)
                    {
                        int diff = this.Size.Height - pbImage.Size.Height;
                        height = img.Size.Height + diff;
                    }

                    if (width > 0 || height > 0)
                    {
                        if (width == 0)
                            width = this.Size.Width;

                        if (height == 0)
                            height = this.Size.Height;

                        this.Size = new Size(width, height);
                    }
                }
            }
        }

        private bool DownloadImage(string strImage)
        {
            if (Directory.Exists(m_strImageFolder) == false)
                Directory.CreateDirectory(m_strImageFolder);

            string strURL = System.Configuration.ConfigurationManager.AppSettings.Get("imgURL");

            if (strURL == null || strURL.Length == 0)
                return false;

            WebClient client = new WebClient();
            m_strImagePath = m_strImageFolder + "/" + strImage;
            client.DownloadFile(strURL + strImage, m_strImagePath);
            return true;
        }
    }
}
