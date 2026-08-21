using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PreSafe
{

    internal class XMLReader<T> : XMLBase where T : Variable
    {
        protected Variables<T> m_Variables = null;
        internal Variables<T> Variables
        {
            get { return m_Variables; }
        }

        internal XMLReader()
        {
        }

        internal bool ReadXML(string szPath)
        {
            XmlDocument m_xDoc = new XmlDocument();
            m_xDoc.Load(szPath);

            XmlNode mSopNode = FindSOPNode(m_xDoc);
            if (mSopNode != null)
            {
                XmlNode varsNode = FindNode2(mSopNode, "Variables");
                if (varsNode != null)
                {
                    m_Variables = GetVariables<T>(m_xDoc, varsNode);
                }
            }
            else
                return false;

            return true;
        }

        protected static bool ReadVariable(XmlNode node, Variable mVar)
        {
            if (node == null)
                return false;

            string strName = null;
            if (node.Attributes["name"].Value != null)
                strName = node.Attributes["name"].Value;

            string szType = null;
            if (node.Attributes["type"].Value != null)
                szType = node.Attributes["type"].Value;

            string szDesc = null;
            if (node.Attributes["description"].Value != null)
                szDesc = node.Attributes["description"].Value;

            string szUnit = null;
            if (node.Attributes["unit"].Value != null)
                szUnit = node.Attributes["unit"].Value;

            if (szUnit == null)
                szUnit = "";

            if (strName == null || szType == null || szDesc == null)
                return false;

            szType = szType.ToUpper();
            if (szType != "정수" && szType != "실수" && szType != "문자열" && szType != "ENUM" && szType != "BOOLEAN")
            {
                szType = "정수";
            }

            mVar.Name = strName;
            mVar.Type = szType;
            mVar.Description = szDesc;
            mVar.Unit = szUnit;

            return true;
        }

        protected static bool ReadVariable(XmlNode node, UserVariable var)
        {
            UserVariable mVar = (UserVariable)var;

            bool result = ReadVariable(node, (Variable)mVar);
            if (result == false)
                return false;

            string szValue = null;
            if (node.Attributes["value"].Value != null)
                szValue = node.Attributes["value"].Value;

            object obj = null;
            if (!ObjectUtil.StringToObject(szValue, mVar.Type, out obj))
            {
                obj = null;
            }

            string szMinVal = null;
            if (node.Attributes["minValue"].Value != null)
                szMinVal = node.Attributes["minValue"].Value;
            else
                szMinVal = "";

            string szMaxValue = null;
            if (node.Attributes["maxValue"].Value != null)
                szMaxValue = node.Attributes["maxValue"].Value;
            else
                szMaxValue = "";

            string szDefValue = null;
            if (node.Attributes["defaultValue"].Value != null)
                szDefValue = node.Attributes["defaultValue"].Value;
            else
                szDefValue = "";

            object objMin, objMax, objDef;
            if (!ObjectUtil.StringToObject(szMinVal, mVar.Type, null, out objMin))
            {
                objMin = null;
            }
            if (!ObjectUtil.StringToObject(szMinVal, mVar.Type, null, out objMax))
            {
                objMax = null;
            }
            if (!ObjectUtil.StringToObject(szMinVal, mVar.Type, null, out objDef))
            {
                objDef = null;
            }

            mVar.Value = obj;
            mVar.MinValue = objMin;
            mVar.MaxValue = objMax;
            mVar.DefaultValue = objDef;

            return true;
        }

        protected static bool ReadVariable(XmlNode node, Enums mVar)
        {
            bool bResult = ReadVariable(node, (Variable)mVar);
            if (bResult == false)
                return false;

            string szValue = null;
            if (node.Attributes["value"].Value != null)
                szValue = node.Attributes["value"].Value;

            object obj = null;
            if (!ObjectUtil.StringToObject(szValue, mVar.Type, out obj))
            {
                obj = null;
            }

            mVar.Value = obj;

            return true;
        }

        internal static Variables<T> GetVariables<T>(XmlDocument xDoc, XmlNode nodeParent) where T : Variable
        {
            Variables<T> mResult = new Variables<T>();

            XmlNodeList childList = nodeParent.ChildNodes;
            if (childList == null || childList.Count == 0)
                return mResult;

            string szNodeName = "SystemVariable";

            foreach (XmlNode node in childList)
            {               
                if (typeof(T) == typeof(UserVariable))
                {
                    szNodeName = "UserVariable";
                    if (node.Name == szNodeName)
                    {
                        UserVariable item = new UserVariable("", "", "");
                        if (ReadVariable(node, item))
                        {
                            mResult.AddVariable((T)(object)item);
                        }
                    }
                }
                else if (typeof(T) == typeof(Enums))
                {
                    szNodeName = "UserEnum";
                    if (node.Name == szNodeName)
                    {
                        Enums item = new Enums("", "", null, "");
                        if (ReadVariable(node, item))
                        {
                            mResult.AddVariable((T)(object)item);
                        }
                    }
                }
                else
                {
                    szNodeName = "SystemVariable";
                    if (node.Name == szNodeName)
                    {
                        Variable item = new Variable("", "", "");
                        if (ReadVariable(node, item))
                        {
                            mResult.AddVariable((T)(object)item);
                        }
                    }
                }
            }
            return mResult;
        }
    }

    internal class XMLWriter<T> : XMLBase where T : Variable
    {
        protected Variables<T> m_Variables = null;
        internal Variables<T> Variables
        {
            set { m_Variables = value; }
        }

        internal XMLWriter()
        {
        }

        internal bool SaveXML(string szPath)
        {
            if (m_Variables == null)
                return false;

            XmlDocument m_xDoc = new XmlDocument();

            XmlNode mSopNode = m_xDoc.CreateElement("SOP");
            if (mSopNode != null)
            {
                m_xDoc.AppendChild(mSopNode);
                XmlNode varsNode = m_xDoc.CreateElement("Variables");
                if (varsNode != null)
                {
                    mSopNode.AppendChild(varsNode);

                    IEnumerable<T> varList = m_Variables.VarList;
                    foreach (T item in varList)
                    {                       
                        AddVariable(m_xDoc, varsNode, item);
                    }

                }
                else
                    return false;
            }
            else
                return false;

            if (m_xDoc != null)
                m_xDoc.Save(szPath);

            return true;
        }


        protected static bool AddAttribute(XmlElement node, UserVariable var)
        {
            bool bResult = AddAttribute(node, (Variable)var);
            if (bResult == false)
                return false;

            string szValue = (var.Value == null ? "" : var.Value.ToString());
            string szMinValue = (var.MinValue == null ? "" : var.MinValue.ToString());
            string szMaxValue = (var.MaxValue == null ? "" : var.MaxValue.ToString());
            string szDefaultValue = (var.DefaultValue == null ? "" : var.DefaultValue.ToString());

            node.SetAttribute("value", szValue);
            node.SetAttribute("minValue", szMinValue);
            node.SetAttribute("maxValue", szMaxValue);
            node.SetAttribute("defaultValue", szDefaultValue);
            
            return true;
        }

        protected static bool AddAttribute(XmlElement node, Variable var)
        {
            if (node == null || var == null)
                return false;

            string szDesc = (var.Description == null ? "" : var.Description);
            string szUnit = (var.Unit == null ? "" : var.Unit);
            string szType = (var.Type == null ? "" : var.Type);
            string szName = (var.Name == null ? "" : var.Name);

            if (szType == "" || szName == "")
                return false;
            
            node.SetAttribute("name", szName);
            node.SetAttribute("type", szType);
            node.SetAttribute("unit", szUnit);
            node.SetAttribute("description", szDesc);

            return true;
        }
        protected static bool AddAttribute(XmlElement node, Enums var)
        {
            bool bResult = AddAttribute(node, (Variable)var);
            if (bResult == false)
                return false;

            string szValue = (var.Value == null ? "" : var.Value.ToString());
            node.SetAttribute("value", szValue);

            return true;
        }

        internal static bool DeleteVariable<T>(XmlDocument xDoc, XmlNode nodeParent, string szVarName) where T : Variable
        {
            XmlNodeList childList = nodeParent.ChildNodes;
            if (childList == null || childList.Count == 0)
                return false;

            string szNodeName = "SystemVariable";
            if (typeof(T) == typeof(UserVariable))
            {
                szNodeName = "UserVariable";                
            }
            else if (typeof(T) == typeof(Enums))
            {
                szNodeName = "UserEnum";               
            }
            foreach (XmlNode node in childList)
            {
                if( node.Name == szNodeName )
                {
                    if( node.Attributes["name"] != null)
                    {
                        string szAttrName = node.Attributes["name"].Value;
                        if (szAttrName == szVarName)
                        {
                            nodeParent.RemoveChild(node);
                            break;
                        }
                    }
                }
            }
            return true;
        }


        internal static bool AddVariable<T>(XmlWriter writer, T item) where T : Variable
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(writer.ToString());

            string szNodeName = item.GetTypeString();
            if (typeof(T) == typeof(UserVariable))
            {
                UserVariable var = (UserVariable)(object)item;
                szNodeName = var.GetTypeString();
                XmlElement newNode = xDoc.CreateElement(szNodeName);
                if (AddAttribute(newNode, var))
                {
                    newNode.WriteTo(writer);
                    return true;
                }
            }
            else if (typeof(T) == typeof(Enums))
            {
                Enums var = (Enums)(object)item;
                szNodeName = var.GetTypeString();
                XmlElement newNode = xDoc.CreateElement(szNodeName);
                if (AddAttribute(newNode, var))
                {
                    newNode.WriteTo(writer);
                    return true;
                }

            }
            else
            {
                XmlElement newNode = xDoc.CreateElement(szNodeName);
                if (AddAttribute(newNode, item))
                {
                    newNode.WriteTo(writer);
                    return true;
                }

            }
            return false;
        }

        internal static bool AddVariable<T>(XmlDocument xDoc, XmlNode nodeParent, T item) where T : Variable
        {
            string szNodeName = item.GetTypeString();
            if (typeof(T) == typeof(UserVariable))
            {
                UserVariable var = (UserVariable)(object)item;
                szNodeName = var.GetTypeString();
                XmlElement newNode = xDoc.CreateElement(szNodeName);
                if (AddAttribute(newNode, var))
                {
                    nodeParent.AppendChild(newNode);
                    return true;
                }

            }
            else if (typeof(T) == typeof(Enums))
            {
                Enums var = (Enums)(object)item;
                szNodeName = var.GetTypeString();
                XmlElement newNode = xDoc.CreateElement(szNodeName);
                if (AddAttribute(newNode, var))
                {
                    nodeParent.AppendChild(newNode);
                    return true;
                }

            }
            else
            {
                XmlElement newNode = xDoc.CreateElement(szNodeName);
                if (AddAttribute(newNode, item))
                {
                    nodeParent.AppendChild(newNode);
                    return true;
                }

            }
            return false;
        }
    }

    internal class XMLBase
    {
        protected XMLBase()
        {
        }

        protected virtual XmlNode FindSOPNode(XmlDocument doc)
        {
            XmlNode root = doc.DocumentElement;
            if (root != null && root.Name.ToUpper() == "SOP")
            {
                return root;
            }
            return null;
        }

        protected virtual XmlNode FindNode2(XmlNode parent, string szName)
        {
            XmlNode varsNode = parent.SelectSingleNode(szName);
            return varsNode;
        }

        protected virtual XmlNode FindNode(XmlNode parent, string szName)
        {
            if (parent.HasChildNodes)
            {
                foreach (XmlNode node in parent.ChildNodes)
                {
                    if (node.Name.ToUpper() == szName.ToUpper())
                    {
                        return node;
                    }
                }
            }
            return null;
        }
    }
}
