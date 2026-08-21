using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalUnE.Data
{
    public class LinkData
    {
        public enum _IconType { None = 0, Calendar = 1, Factory1, Factory2, Siren, Signal };
        public enum _TextType { None = 0, Type1 = 1, Type4 = 4, Type5 = 5, Type6 = 6 };

        private string m_strUrl = "";
        // 1(달력), 2(공장1), 3(공장2), 4(비상등), 5(통신주파수)
        private int m_nIconType = 0;
        // 1(16px), 4(18px), 5(18px)
        private int m_nTextType = 0;
        private string m_strText = "";

        public string Url
        {
            get { return m_strUrl; }
            set { m_strUrl = value; }
        }

        public int IconType
        {
            get { return m_nIconType; }
            set { m_nIconType = value; }
        }

        public int TextType
        {
            get { return m_nTextType; }
            set { m_nTextType = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public static LinkData ParseData(string strUrl, string strIconType, string strTextType, string strText)
        {
            if (strUrl.Length == 0 || strIconType.Length == 0 || strTextType.Length == 0 || strText.Length == 0)
                return null;

            int nIconType, nTextType;

            if (int.TryParse(strIconType, out nIconType) == false || int.TryParse(strTextType, out nTextType) == false)
                return null;

            _IconType iconType = _IconType.None;
            _TextType textType = _TextType.None;

            foreach (_IconType type in Enum.GetValues(typeof(_IconType)))
            {
                if ((int)type == nIconType)
                {
                    iconType = type;
                    break;
                }
            }

            if (iconType == _IconType.None)
                return null;

            foreach (_TextType type in Enum.GetValues(typeof(_TextType)))
            {
                if ((int)type == nTextType)
                {
                    textType = type;
                    break;
                }
            }

            if (textType == _TextType.None)
                return null;

            LinkData linkData = new LinkData();

            linkData.Url = strUrl;
            linkData.IconType = (int)iconType;
            linkData.TextType = (int)textType;
            linkData.Text = strText;

            return linkData;
        }
    }
}
