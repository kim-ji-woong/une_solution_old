using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using mshtml;


namespace PreSafe
{
    public partial class PopupFormHelp : Form
    {
        public PopupFormHelp()
        {
            InitializeComponent();

        }

        private string m_szText = "";
        public void SetPageLoad(string szPageText)
        {
            if(string.Compare(m_szText, szPageText) != 0)
            {
                m_szText = szPageText;
                webBrowser1.DocumentText = szPageText;
                
            }
            
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {                
                base.Text = value;
                if (this.Parent != null)
                    Parent.Text = Text;
            }
        }
        

        private void PopupFormHelp_Load(object sender, EventArgs e)
        {
            SetPageLoad(Properties.Resources.Drawing);
        
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Document != null)
            {
                for (int i = 0; i < webBrowser1.Document.Links.Count; i++)
                {
                    webBrowser1.Document.Links[i].Click += new HtmlElementEventHandler(this.LinkClick);
                }
                this.Text = "도움말 - " + webBrowser1.DocumentTitle;
            }            
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (webBrowser1.Document != null)
            {
                for (int i = 0; i < webBrowser1.Document.Links.Count; i++)
                {
                    webBrowser1.Document.Links[i].Click -= new HtmlElementEventHandler(this.LinkClick);
                }
            }            
        }

        private void LinkClick(object sender, System.EventArgs e)
        {
            HtmlElement element = (HtmlElement)sender;
            HTMLAnchorElementClass ae = (HTMLAnchorElementClass)(element.DomElement);
            
            string szPageName = ae.href.Replace("about:", "");
            szPageName = szPageName.Replace(".html", "");

            try
            {
                ResourceManager rm = global::PreSafe.Properties.Resources.ResourceManager;
                string szPageText = (string)rm.GetObject(szPageName);
                SetPageLoad(szPageText);
            }
            catch(Exception ex)
            {
            }
        }
    }
}
