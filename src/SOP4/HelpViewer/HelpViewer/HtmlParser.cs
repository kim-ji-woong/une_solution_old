using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpViewer
{
    public class HtmlParser
    {
        private const string END_COMMENT = "-->";
        private const string BEGIN_COMMENT = "<!--";

        private bool m_isComment = false;

        private HtmlElement m_rootElement = null;
        private HtmlElement m_currentElement = null;

        private string m_strUnprocessedLine = "";

        // 주석이 끝났는가?
        private static bool EndComment(ref string strLine, ref bool onceMore, int nBeginIndex = 0)
        {
            int nIndex = strLine.IndexOf(END_COMMENT, nBeginIndex);

            if (nIndex >= 0)
            {
                onceMore = true;
                strLine = strLine.Substring(nBeginIndex + END_COMMENT.Length);
                return true;
                /*int nIndex2 = strLine.IndexOf(BEGIN_COMMENT, nIndex + END_COMMENT.Length);

                if (nIndex2 >= 0)
                {
                    if (nBeginIndex > 0)
                    {
                        nIndex2 = (nIndex + END_COMMENT.Length) - ((nIndex + END_COMMENT.Length) - nBeginIndex);
                        strLine = strLine.Substring(0, nBeginIndex) + strLine.Substring(nIndex + END_COMMENT.Length);
                    }
                    else
                    {
                        nIndex2 = nIndex2 - (nIndex + END_COMMENT.Length);
                        strLine = strLine.Substring(nIndex + END_COMMENT.Length);
                    }

                    return EndComment(ref strLine, nIndex2 + BEGIN_COMMENT.Length);
                }
                else
                {
                    strLine = strLine.Substring(nIndex + END_COMMENT.Length);
                    return true;
                }*/
            }
            else
            {
                if (nBeginIndex == 0)
                    strLine = "";
                else
                    strLine = strLine.Substring(0, nBeginIndex);
            }

            return false;
        }

        private static bool BeginComment(ref string strLine)
        {
            int nIndex = strLine.IndexOf(BEGIN_COMMENT);

            if (nIndex >= 0)
            {
                int nIndex2 = strLine.IndexOf(END_COMMENT, nIndex + BEGIN_COMMENT.Length);

                if (nIndex2 >= 0)
                {
                    strLine = strLine.Substring(0, nIndex) + strLine.Substring(nIndex2 + END_COMMENT.Length);
                    return BeginComment(ref strLine);
                }
                else
                {
                    strLine = strLine.Substring(0, nIndex);
                    return true;
                }
            }

            return false;
        }

        public bool ReadLine(string strLine)
        {
            bool onceMore = false;

            do
            {
                onceMore = false;

                // 주석인가?
                if (m_isComment)
                {
                    if (HtmlParser.EndComment(ref strLine, ref onceMore))
                        m_isComment = false;
                }
                else
                {
                    if (HtmlParser.BeginComment(ref strLine))
                        m_isComment = true;
                }
            }
            while (onceMore);

            if (strLine.Length == 0)
                return true;

            if (!ParseLine(strLine))
                return false;

            return true;
        }

        private bool ParseLine(string strLine)
        {
            if (m_strUnprocessedLine.Length > 0)
                strLine = m_strUnprocessedLine + strLine;

            m_strUnprocessedLine = "";

            int nIndex = strLine.IndexOf('<');

            if (nIndex < 0 || nIndex == strLine.Length - 1)
            {
                m_strUnprocessedLine = strLine;
                return true;
            }

            string strText = nIndex > 0 ? strLine.Substring(0, nIndex) : "";

            char ch = strLine.ElementAt(nIndex + 1);

            if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n' || ch == '!')
                return true;

            int nIndex2 = strLine.IndexOf('>', nIndex + 1);

            if (nIndex2 >= 0)
            {
                if (strText.Length > 0)
                {
                    HtmlElement element = GetUncompletedElement(m_currentElement);

                    if (element != null)
                        element.Text += strText;
                }

                if (MakeElement(strLine.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim()) == null)
                    return false;

                ParseLine(strLine.Substring(nIndex2 + 1));
            }
            else
                m_strUnprocessedLine = strLine.Substring(nIndex).Trim() + " ";

            return true;
        }

        private HtmlElement MakeElement(string strLine)
        {
            int len = strLine.Length;
            int nIndex = len;

            for (int i=1;i<len;i++)
            {
                char ch = strLine.ElementAt(i);

                if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
                {
                    nIndex = i;
                    break;
                }
            }

            string strElementName = strLine.Substring(0, nIndex);

            // 닫힘 태그
            if (strElementName.StartsWith("/"))
            {
                HtmlElement element = GetUncompletedElement(m_currentElement, strElementName.Substring(1));

                if (element == null)
                    return null;

                element.IsCompleted = true;
                return element;
            }
            
            if (m_currentElement == null)
            {
                if (m_rootElement == null && string.Compare(strElementName, "html", true) == 0)
                {
                    m_rootElement = m_currentElement = new HtmlElement();
                }
                else
                    return null;
            }
            else if (m_currentElement.IsCompleted)
            {
                HtmlElement parent = GetUncompletedElement(m_currentElement.Parent);

                if (parent == null)
                    return null;

                m_currentElement = new HtmlElement();
                parent.AddChild(m_currentElement);
            }
            else
            {
                HtmlElement element = new HtmlElement();
                m_currentElement.AddChild(element);
                m_currentElement = element;
            }

            m_currentElement.Name = strElementName;

            int nBeginIndex = nIndex + 1;

            for (int i = nIndex + 1;i<len;i++)
            {
                char ch = strLine.ElementAt(i);

                if (ch == '=')
                {
                    HtmlAttribute attr = MakeAttribute(strLine, nBeginIndex, i, ref i);

                    if (attr == null)
                        return null;

                    m_currentElement.AddAttrib(attr);
                    nBeginIndex = i + 1;
                }
            }

            if (strLine.ElementAt(len - 1) == '/')
                m_currentElement.IsCompleted = true;

            return m_currentElement;
        }

        private HtmlAttribute MakeAttribute(string strLine, int nBeginIndex, int nMiddleIndex, ref int nEndIndex)
        {
            string strAttrName = strLine.Substring(nBeginIndex, nMiddleIndex - nBeginIndex).Trim();

            if (strAttrName.Length == 0)
                return null;

            int len = strLine.Length;
            int nIndex1 = -1, nIndex2 = len;
            bool quotationBegin = true, quotationEnd = false;

            for (int i=nMiddleIndex + 1;i<len;i++)
            {
                char ch = strLine.ElementAt(i);

                if (ch == '\"')
                {
                    if (quotationBegin == false)
                        return null;

                    if (nIndex1 < 0)
                        nIndex1 = i + 1;
                    else
                    {
                        quotationEnd = true;
                        nIndex2 = i;
                        break;
                    }
                }
                else if (nIndex1 < 0)
                {
                    if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n')
                    {
                        quotationBegin = false;
                        nIndex1 = i;
                    }
                }
                else
                {
                    if (!quotationBegin)
                    {
                        if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
                        {
                            nIndex2 = i;
                            break;
                        }
                    }
                }
            }

            if (quotationBegin && !quotationEnd)
                return null;

            string strAttrValue = strLine.Substring(nIndex1, nIndex2 - nIndex1);

            HtmlAttribute attr = new HtmlAttribute(strAttrName, strAttrValue);
            
            nEndIndex = nIndex2;
            return attr;
        }

        private HtmlElement GetUncompletedElement(HtmlElement element, string strElementName)
        {
            if (element == null)
                return null;

            if (element.Name == strElementName && element.IsCompleted == false)
                return element;

            return GetUncompletedElement(element.Parent, strElementName);
        }

        private HtmlElement GetUncompletedElement(HtmlElement element)
        {
            if (element == null)
                return null;

            if (element.IsCompleted == false)
                return element;

            return GetUncompletedElement(element.Parent);
        }

        public PageData Calc()
        {
            if (m_rootElement == null)
                return null;

            HtmlElement head = FindElement(m_rootElement, "head");

            if (head == null)
                return null;

            PageData dataRoot = GetPageDataFromMetaElements(head, "meta");

            if (dataRoot == null)
                return null;

            HtmlElement body = FindElement(m_rootElement, "body");

            if (body == null)
                return dataRoot;

            GetALinkDatas(body, dataRoot, dataRoot);
            SetPageDataText(dataRoot, body);
            return dataRoot;
        }

        private void SetPageDataText(PageData data, HtmlElement element)
        {
            string strText = "";

            if (data.LinkName.Length == 0)
            {
                // 전체 html
                SearchElement(ref strText, element);
            }
            else
            {
                bool findLink = false;
                HtmlElement _element = FindElementFromLink(data.LinkName, element, ref findLink);

                if (_element != null)
                {
                    SearchElement(ref strText, _element);
                }
            }

            data.DisplayText = strText;

            foreach (PageData childData in data.ChildDatas)
            {
                SetPageDataText(childData, element);
            }
        }

        private HtmlElement FindElementFromLink(string strLinkName, HtmlElement element, ref bool findLink)
        {
            foreach (HtmlElement child in element.Elements)
            {
                if (findLink == false && string.Compare(child.Name, "a", true) == 0)
                {
                    HtmlAttribute attr = FindAttrib(child, "name");

                    if (attr != null)
                    {
                        if (string.Compare(attr.Value, strLinkName, true) == 0)
                        {
                            findLink = true;
                            return null;
                        }
                    }
                }
                else if (findLink && string.Compare(child.Name, "div", true) == 0)
                {
                    HtmlAttribute attr = FindAttrib(child, "class");

                    if (attr != null)
                    {
                        string strClassName = attr.Value.ToLower();

                        if (strClassName.StartsWith("section"))
                            return child;
                    }
                }

                HtmlElement _element = FindElementFromLink(strLinkName, child, ref findLink);

                if (_element != null)
                    return _element;
            }

            return null;
        }

        private void SearchElement(ref string strText, HtmlElement element)
        {
            foreach (HtmlElement child in element.Elements)
            {
                if (string.Compare(child.Name, "span", true) == 0)
                    continue;
                else if (string.Compare(child.Name, "dl", true) == 0)
                    continue;
                else if (string.Compare(child.Name, "img", true) == 0)
                    continue;
                else if (string.Compare(child.Name, "div", true) == 0)
                {
                    HtmlAttribute attr = FindAttrib(child, "class");

                    if (attr != null)
                    {
                        if (string.Compare(attr.Value, "footer", true) == 0)
                            continue;

                        string strClassName = attr.Value.ToLower();

                        if (strClassName.StartsWith("section"))
                            continue;
                    }
                }

                strText += child.Text;
                SearchElement(ref strText, child);
            }
        }

        private HtmlAttribute FindAttrib(HtmlElement element, string strAttrib)
        {
            foreach (HtmlAttribute attr in element.Attribs)
            {
                if (string.Compare(attr.Name, strAttrib, true) == 0)
                    return attr;
            }

            return null;
        }

        private void GetALinkDatas(HtmlElement parent, PageData dataParent, PageData dataRoot)
        {
            List<HtmlElement> aLinkElements = FindElements(parent, "a");

            foreach (HtmlElement element in aLinkElements)
            {
                bool href = false;
                string strName = "", strTreeItem = "", strTag = "", strID = "", strOrderIndex = "", strParent = "", strChildFolder = "";

                foreach (HtmlAttribute attr in element.Attribs)
                {
                    if (string.Compare(attr.Name, "href", true) == 0)
                    {
                        href = true;
                        break;
                    }
                    else if (string.Compare(attr.Name, "name", true) == 0)
                        strName = attr.Value;
                    else if (string.Compare(attr.Name, "treeitem", true) == 0)
                        strTreeItem = attr.Value;
                    else if (string.Compare(attr.Name, "tag", true) == 0)
                        strTag = attr.Value;
                    else if (string.Compare(attr.Name, "id", true) == 0)
                        strID = attr.Value;
                    else if (string.Compare(attr.Name, "orderindex", true) == 0)
                        strOrderIndex = attr.Value;
                    else if (string.Compare(attr.Name, "parent", true) == 0)
                        strParent = attr.Value;
                    else if (string.Compare(attr.Name, "childFolder", true) == 0)
                        strChildFolder = attr.Value;
                }

                if (href || strName.Length == 0)
                    continue;

                PageData data = new PageData();

                data.LinkName = strName;
                data.ID = strName;
                data.TreeItem = strName;

                if (strChildFolder.Length > 0)
                    data.AddChildFolder(strChildFolder);

                if (strID.Length > 0)
                    data.ID = strID;

                if (strTreeItem.Length > 0)
                    data.TreeItem = strTreeItem;

                if (strTag.Length > 0)
                    data.AddTag(strTag);

                if (strOrderIndex.Length > 0)
                {
                    int nOrderIndex;

                    if (int.TryParse(strOrderIndex, out nOrderIndex))
                        data.OrderIndex = nOrderIndex;
                }

                PageData parent2 = dataParent;

                if (strParent.Length > 0)
                {
                    parent2 = FindPageData(strParent, dataRoot);

                    if (parent2 == null)
                        parent2 = dataParent;
                }

                parent2.ChildDatas.Add(data);
            }
        }

        private PageData FindPageData(string strID, PageData dataParent)
        {
            if (dataParent == null)
                return null;

            if (string.Compare(dataParent.ID, strID, true) == 0)
                return dataParent;

            foreach (PageData data in dataParent.ChildDatas)
            {
                if (string.Compare(data.ID, strID, true) == 0)
                    return data;

                PageData child = FindPageData(strID, data);

                if (child != null)
                    return child;
            }

            return null;
        }

        private List<HtmlElement> FindElements(HtmlElement parent, string strElementName, List<HtmlElement> elements = null)
        {
            if (elements == null)
                elements = new List<HtmlElement>();

            foreach (HtmlElement element in parent.Elements)
            {
                if (string.Compare(element.Name, strElementName, true) == 0)
                    elements.Add(element);

                FindElements(element, strElementName, elements);
            }

            return elements;
        }

        private PageData GetPageDataFromMetaElements(HtmlElement parent, string strElementName)
        {
            PageData data = null;

            foreach (HtmlElement element in parent.Elements)
            {
                if (string.Compare(element.Name, strElementName, true) == 0)
                {
                    string strName = "", strContent = "";

                    foreach (HtmlAttribute attr in element.Attribs)
                    {
                        if (string.Compare(attr.Name, "name", true) == 0)
                            strName = attr.Value;
                        else if (string.Compare(attr.Name, "content", true) == 0)
                            strContent = attr.Value;
                    }

                    if (strName.Length > 0 && strContent.Length > 0)
                    {
                        if (string.Compare(strName, "id", true) == 0)
                        {
                            if (data == null)
                                data = new PageData();

                            data.ID = strContent;

                            if (data.TreeItem.Length == 0)
                                data.TreeItem = strContent;
                        }
                        else if (string.Compare(strName, "treeitem", true) == 0)
                        {
                            if (data == null)
                                data = new PageData();

                            data.TreeItem = strContent;
                        }
                        else if (string.Compare(strName, "tag", true) == 0)
                        {
                            if (data == null)
                                data = new PageData();

                            data.AddTag(strContent);
                        }
                        else if (string.Compare(strName, "childFolder", true) == 0)
                        {
                            if (data == null)
                                data = new PageData();

                            data.AddChildFolder(strContent);
                        }
                        else if (string.Compare(strName, "orderindex", true) == 0)
                        {
                            if (data == null)
                                data = new PageData();

                            int nOrderIndex;

                            if (int.TryParse(strContent, out nOrderIndex))
                                data.OrderIndex = nOrderIndex;
                        }
                    }
                }
            }

            return data;
        }

        private HtmlElement FindElement(HtmlElement parent, string strElementName)
        {
            foreach (HtmlElement element in parent.Elements)
            {
                if (string.Compare(element.Name, strElementName, true) == 0)
                    return element;

                HtmlElement child = FindElement(element, strElementName);

                if (child != null)
                    return child;
            }

            return null;
        }
    }

    public class PageData
    {
        private string m_strTreeItem = "";
        private string m_strID = "";
        private string m_strLinkName = "";
        private List<string> m_tags = new List<string>();
        private List<PageData> m_childPageDatas = new List<PageData>();
        private string m_strURL = "";
        private List<string> m_childFolderNames = new List<string>();
        private int m_nOrderIndex = 0;
        private string m_strDisplay = "";

        public string TreeItem
        {
            get { return m_strTreeItem; }
            set { m_strTreeItem = value; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string LinkName
        {
            get { return m_strLinkName; }
            set { m_strLinkName = value; }
        }

        public List<string> Tags
        {
            get { return m_tags; }
        }

        public List<PageData> ChildDatas
        {
            get { return m_childPageDatas; }
        }

        // 이 값이 true이면 Page 전체를 대표한다.
        // false이면 Page내의 특정 링크를 의미한다.
        public bool IsPageTitle
        {
            get { return m_strLinkName.Length == 0; }
        }

        public string PageURL
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }

        public int OrderIndex
        {
            get { return m_nOrderIndex; }
            set { m_nOrderIndex = value; }
        }

        // 하위 Section의 Text는 제외한다.
        public string DisplayText
        {
            get { return m_strDisplay; }
            set { m_strDisplay = value; }
        }

        public void AddTag(string strTagLine)
        {
            string[] tags = strTagLine.Split(';');

            foreach (string strTag in tags)
            {
                m_tags.Add(strTag.Trim());
            }
        }

        public void AddChildFolder(string strFolderLine)
        {
            string[] folders = strFolderLine.Split(';');

            foreach (string strFolder in folders)
            {
                m_childFolderNames.Add(strFolder.Trim());
            }
        }

        public bool ContainsChildFolder(string strFolderName)
        {
            foreach (string strFolder in m_childFolderNames)
            {
                if (string.Compare(strFolder, strFolderName, true) == 0)
                    return true;
            }

            return false;
        }
    }

    public class HtmlElement
    {
        private string m_strElementName = "";
        private List<HtmlAttribute> m_attribs = new List<HtmlAttribute>();
        private List<HtmlElement> m_elements = new List<HtmlElement>();
        private HtmlElement m_parent = null;
        private string m_strText = "";
        private bool m_isCompleted = false;

        public string Name
        {
            get { return m_strElementName; }
            set { m_strElementName = value; }
        }

        public List<HtmlAttribute> Attribs
        {
            get { return m_attribs; }
        }

        public List<HtmlElement> Elements
        {
            get { return m_elements; }
        }

        public HtmlElement Parent
        {
            get { return m_parent; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public bool IsCompleted
        {
            get { return m_isCompleted; }
            set { m_isCompleted = value; }
        }

        public void AddChild(HtmlElement element)
        {
            if (element.Parent == null)
            {
                m_elements.Add(element);
                element.m_parent = this;
            }
            else
            {
                element.Parent.Elements.Remove(element);
                m_elements.Add(element);
                element.m_parent = this;
            }
        }

        public void AddAttrib(HtmlAttribute attr)
        {
            m_attribs.Add(attr);
        }
    }

    public class HtmlAttribute
    {
        private string m_strAttrName = "";
        private string m_strAttrValue = "";

        public string Name
        {
            get { return m_strAttrName; }
            set { m_strAttrName = value; }
        }

        public string Value
        {
            get { return m_strAttrValue; }
            set { m_strAttrValue = value; }
        }

        public HtmlAttribute()
        {
        }

        public HtmlAttribute(string strName, string strValue)
        {
            m_strAttrName = strName;
            m_strAttrValue = strValue;
        }
    }
}
