using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace SOPManager
{
	internal class ComponentDropDownList
	{
		internal static ArrayList ComponentList = new ArrayList();
		internal static ArrayList OrginalList = new ArrayList();
		
		internal static void UpdateList()
		{
			ComponentList.Clear();
			OrginalList.Clear();
			ArrayList arComp = FormMain.Instance.GetPageLevel().AllComponentList();
			foreach (Sections.Section section in arComp)
			{

				ComponentList.Add(section.Data.ComponentID);
				OrginalList.Add(section);
			}
		}

		internal static Sections.Section FindSection(string szCompID)
		{
			foreach (Sections.Section section in OrginalList)
			{
				if (section.Data.ComponentID == szCompID)
				{
					return section;
				}
			}
			return null;
		}
	}

	public class ComponentConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override System.ComponentModel.TypeConverter.StandardValuesCollection
			   GetStandardValues(ITypeDescriptorContext context)
		{
			ComponentDropDownList.UpdateList();
			return new StandardValuesCollection(ComponentDropDownList.ComponentList);
		}
	}


	public class PropertiesLink: SectionPropertiesBase
	{    
		[Category("일반")]
		[Browsable(true)]
		[DisplayName("표시내용")]
		[Description("링크의 표시내용을 입력합니다.")]
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

		private string m_szLinkComponentID = "";
		[Category("일반")]
		[Browsable(true)]
		[DisplayName("ProcessLink")]
		[Description("연결될 Process Section을 지정합니다.")]
		[TypeConverter(typeof(ComponentConverter))]
		public string LinkSection
		{
			get
			{
				m_szLinkComponentID = "";
				if (mSection != null)
				{
					Sections.SectionDataLink data = (Sections.SectionDataLink)mSection.Data;
					if (data != null)
					{
						if (data.LinkedSection != null)
						{
							m_szLinkComponentID = data.LinkedSection.Data.ComponentID;	
						}											
					}
				}
				return m_szLinkComponentID;
			}
			set
			{
				if (m_szLinkComponentID != value)
				{
					if (mSection != null)
					{
						Sections.SectionDataLink data = (Sections.SectionDataLink)mSection.Data;
						if (data != null)
						{
                            if (m_bEnabledSnapshot == true)
							    UndoRedoManager.Instance.SaveSnapshot("연결 컴포넌트 변경");

							data.LinkedSection = ComponentDropDownList.FindSection(value);
						}
					}
				}
				m_szLinkComponentID = value;
			}
		}

		
	}


	
}
