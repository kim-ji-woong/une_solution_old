using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace UnE.SenarioMaker
{
    internal class ObjectUtil
    {
        internal static object Clone(object obj)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, obj);
                    stream.Seek(0, SeekOrigin.Begin);
                    return formatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        internal static bool GetValue(object value, out int retValue)
        {
           
            retValue = -1;
            if (value == null)
                return false;

            if (int.TryParse(value.ToString(), out retValue))
                return true;
            return false;
        }

        internal static bool GetValue(object value, out float retValue)
        {
            retValue = -1;
            if (value == null)
                return false;
            if (float.TryParse(value.ToString(), out retValue))
                return true;
            return false;
        }

        internal static bool GetValue(object value, out string retValue)
        {
            retValue = "";
            if (value == null)
                return false;
            retValue = value.ToString();
            return true;
        }

        internal static bool GetValue(object value, out bool retValue)
        {
            retValue = false;
            if (value == null)
                return false;
            if (bool.TryParse(value.ToString(), out retValue))
                return true;
            return false;
        }

        internal static bool StringToObject(string szValue, string szType, object def, out object obj)
        {
            obj = def;

            if (szType == "정수" || szType == "ENUM")
            {
                int nValue;
                if (!ObjectUtil.GetValue(szValue, out nValue))
                {
                    obj = def;
                }
            }
            else if (szType == "실수")
            {
                float nValue;
                if (!ObjectUtil.GetValue(szValue, out nValue))
                {
                    obj = def;
                }
            }
            else if (szType == "BOOLEAN")
            {
                bool nValue;
                if (!ObjectUtil.GetValue(szValue, out nValue))
                {
                    obj = def;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        internal static bool StringToObject(string szValue, string szType, out object obj)
        {
            obj = null;

            if (szType == "정수" || szType == "ENUM")
            {
                int nValue;
                if (!ObjectUtil.GetValue(szValue, out nValue))
                {
                    obj = 0;
                }
            }
            else if (szType == "실수")
            {
                float nValue;
                if (!ObjectUtil.GetValue(szValue, out nValue))
                {
                    obj = 0.0f;
                }
            }
            else if (szType == "BOOLEAN")
            {
                bool nValue;
                if (!ObjectUtil.GetValue(szValue, out nValue))
                {
                    obj = false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
    }

    internal class Variable
    {
        public Variable(string szName, string szType, string szDesc)
        {
            m_szName = szName;
            m_szType = szType;
            m_szDesc = szDesc;
        }

        protected string m_szName = "";
        public virtual string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        protected string m_szType = "";
        public virtual string Type
        {
            get
            {
                return m_szType;
            }
            set { m_szType = value; }
        }

        protected string m_szDesc = "";
        public virtual string Description
        {
            get { return m_szDesc; }
            set { m_szDesc = value; }
        }

        protected object m_value = null;
        public virtual object Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        protected string m_szUnit = "";
        public virtual string Unit
        {
            get { return m_szUnit; }
            set { m_szUnit = value; }
        }


        public override string ToString()
        {
            return base.ToString();
        }

        public virtual string ToStringValue()
        {
            if (m_value == null)
                return "";

            if (m_szType == null || m_szType == "")
                return m_value.ToString();
            try
            {
                string szValue = m_value.ToString();
                if (m_szType == "정수" || m_szType == "ENUM")
                {
                    int nValue;
                    if (int.TryParse(szValue, out nValue))
                        return string.Format("{0}", nValue);

                }
                else if (m_szType == "실수")
                {
                    float fValue;
                    if (float.TryParse(szValue, out fValue))
                        return string.Format("{0,:F2}", fValue);
                }
                else if (m_szType == "문자열")
                {
                    return szValue;
                }
                else if (m_szType == "BOOLEAN")
                {
                    bool bValue;
                    if (bool.TryParse(szValue, out bValue))
                        return (bValue == true ? bool.TrueString : bool.FalseString);
                }
            }
            catch (Exception)
            {
            }
            return m_value.ToString();
        }

        public virtual string GetTypeString()
        {
            return "SystemVariable";
        }
    }

    internal class UserVariable : Variable
    {
        public UserVariable(string szName, string szType, string szDesc)
            : base(szName, szType, szDesc)
        {
        }

        public override object Value
        {
            get { return m_value; }
            set
            {
                m_defaultValue = ObjectUtil.Clone(value);
                m_value = value;
            }
        }

        protected object m_defaultValue = null;
        public object DefaultValue
        {
            get { return m_defaultValue; }
            set { m_defaultValue = value; }
        }

        protected object m_minValue = null;
        public object MinValue
        {
            get { return m_minValue; }
            set { m_minValue = value; }
        }

        protected object m_maxValue = null;
        public object MaxValue
        {
            get { return m_maxValue; }
            set { m_maxValue = value; }
        }

        public override string ToStringValue()
        {
            return base.ToStringValue();
        }

        public override string GetTypeString()
        {
            return "UserVariable";
        }
    }

    internal class Enums : Variable
    {
        public Enums(string szName, string szType, object value, string szDesc)
            : base(szName, szType, szDesc)
        {
            m_value = value;
        }

        public override string GetTypeString()
        {
            return "UserEnum";
        }
    }


    internal class Variables<T> where T : Variable
    {
        private Dictionary<string, T> m_vars = new Dictionary<string, T>();

        public IEnumerable<T> VarList
        {
            get
            {
                return m_vars.Values;
            }
        }

        public bool ContainsKey(string szKey)
        {
            return m_vars.ContainsKey(szKey);
        }

        public bool ContainsValue(T var)
        {
            return m_vars.ContainsValue(var);
        }

        public bool AddVariable(T var)
        {
            if (var == null)
                return false;

            if (m_vars.ContainsKey(var.Name))
                return false;

            m_vars.Add(var.Name, var);

            return true;
        }

        public object RemoveVariable(string szName)
        {
            if (szName == null)
                return null;

            if (!m_vars.ContainsKey(szName))
                return null;

            object obj = m_vars[szName];

            m_vars.Remove(szName);

            return obj;
        }
    }
}
