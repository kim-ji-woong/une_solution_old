using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;

namespace SOPManager
{

	public class PropertiesEndPoint : SectionPropertiesBase
	{
		private string m_szEndType;

        [Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("구분")]
		[Description("시작 또는 종료를 선택합니다.")]
		[TypeConverter(typeof(EndTypeConverter))]
		public string Type
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)mSection.Data;
					if (data != null)
					{
						bool bBeginPoint = data.IsBegin;
						if (bBeginPoint)
							m_szEndType = EndTypes.TypeList[0];
						else
							m_szEndType = EndTypes.TypeList[1];
					}
				}
				return m_szEndType;
			}
			set
			{
				if (m_szEndType != value)
				{
					if (mSection != null)
					{
						Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)mSection.Data;
						if (data != null)
						{
                            if (m_bEnabledSnapshot == true)
							    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 타입 변경");
							data.IsBegin = !data.IsBegin;
						}
					}
				}
				m_szEndType = value;
			}
		}

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
				if (mSection != null)
				{
                    if (m_bEnabledSnapshot == true)
					    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");
					mSection.Title = value;
					if (mSection.GetParent() != null)
						mSection.GetParent().Refresh();
				}
				m_szText = value;
			}
		}
	}
}