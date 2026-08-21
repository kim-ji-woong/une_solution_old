using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using Sections;

namespace SOPManager
{
	public class PropertiesDecision : SectionPropertiesBase
	{	
        [Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("내용")]
		[Description("컴포넌트의 내용을 표시합니다.")]
		[Editor(typeof(TextEditor), typeof(UITypeEditor))]
		public string Text
		{
			get
			{
				if (mSection != null)
				{
					m_szText = mSection.Title;
				}
				return m_szText;
			}
			set
			{
				m_szText = value;
				if (mSection != null)
				{
                    if (m_bEnabledSnapshot == true)
					    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");

					mSection.Title = m_szText;
					if (mSection.GetParent() != null)
						mSection.GetParent().Refresh();
				}
			}
		}

        [Category("\t수식")]
        [Browsable(true)]
        [DisplayName("내용")]
        [Description("수식의 내용을 표시합니다.")]
        [Editor(typeof(TextEditor), typeof(UITypeEditor))]
        public string Expression
        {
            get
            {
                if (mSection != null)
                {
                    m_szExpr = mSection.Data.Expression;
                }
                return m_szExpr;
            }
            set
            {
                m_szExpr = value;
                if (mSection != null)
                {
                    if (m_bEnabledSnapshot == true)
                        UndoRedoManager.Instance.SaveSnapshot("컴포넌트 수식 편집");

                    mSection.Data.Expression = m_szExpr;
                    if (mSection.GetParent() != null)
                        mSection.GetParent().Refresh();
                }
            }
        }

        [Category("\t수식")]
        [Browsable(true)]
        [DisplayName("변수타입")]
        [Description("변수들의 타입을 표시합니다.")]
        [Editor(typeof(TextEditor), typeof(UITypeEditor))]
        public string VariableType
        {
            get
            {
                string strTypes = "";

                if (mSection != null && mSection is SectionDecision)
                {
                    SectionDataDecision data = (SectionDataDecision)mSection.Data;
                    strTypes = ToVariableTypeString(data);
                }

                return strTypes;
            }
            set
            {
                if (mSection != null && mSection is SectionDecision)
                {
                    SectionDataDecision data = (SectionDataDecision)mSection.Data;
                    SetVariableTypes(data, value);
                    
                    if (m_bEnabledSnapshot == true)
                        UndoRedoManager.Instance.SaveSnapshot("컴포넌트 변수타입 편집");

                    if (mSection.GetParent() != null)
                        mSection.GetParent().Refresh();
                }
            }
        }

        public static string ToVariableTypeString(SectionDataDecision data)
        {
            string strTypes = "";

            foreach (KeyValuePair<string, SectionDataDecision.VariableType> pair in data.VariableTypes)
            {
                if (strTypes.Length > 0)
                    strTypes += ";";

                strTypes += pair.Key + "(" + pair.Value.ToString() + ")";
            }
        
            return strTypes;
        }

        public static void SetVariableTypes(SectionDataDecision data, string strVariableTypes)
        {
            data.VariableTypes.Clear();
            string[] tokens = strVariableTypes.Split(';');

            foreach (string strToken in tokens)
            {
                int nIndex1 = strToken.IndexOf('(');
                int nIndex2 = strToken.IndexOf(')');

                if (nIndex1 < 0 || nIndex2 < nIndex1)
                    continue;

                string strVariable = strToken.Substring(0, nIndex1);
                string strType = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                data.VariableTypes[strVariable] = SectionDataDecision.ToVariableType(strType);
            }
        }
	}
}
