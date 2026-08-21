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
	}
}
