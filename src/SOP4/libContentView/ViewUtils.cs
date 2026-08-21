using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnE.View.Content
{
    public class ViewUtils
    {
        private static IFormContent m_formContent = null;
        public static void RegisterContentView(IFormContent formContent)
        {
            m_formContent = formContent;
        }

        public static IFormContent GetContentView()
        {
            if (m_formContent == null)
                throw new Exception("FormContent가 지정되지 않았습니다. RegisterContentView를 수행하십시요");
            return m_formContent;
        }

        private static IFormContentOwner m_formContentOwner = null;
        public static void RegisterContentViewOwner(IFormContentOwner formContent)
        {
            m_formContentOwner = formContent;
        }

        public static IFormContentOwner GetContentViewOwner()
        {
            if (m_formContentOwner == null)
                throw new Exception("FormContentOwner가 지정되지 않았습니다. RegisterContentViewOwnwer를 수행하십시요");
            return m_formContentOwner;
        }

        public static System.Windows.Forms.Form InvokeForm
        {
            get 
            {
                if (m_formContentOwner == null)
                    throw new Exception("FormContentOwner가 지정되지 않았습니다. RegisterContentViewOwnwer를 수행하십시요");

                return m_formContentOwner.InvokeForm; 
            }
        }


    }
}
