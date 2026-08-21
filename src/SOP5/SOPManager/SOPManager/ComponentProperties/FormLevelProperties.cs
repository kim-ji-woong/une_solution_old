using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;
using System.Reflection;


namespace SOPManager
{
	public partial class FormLevelProperties : Form
	{
		private PropertiesLevel levelProperties = new PropertiesLevel();
		public PropertiesLevel LevelProperties
		{
			get { return levelProperties; }
		}

		public FormLevelProperties()
		{
			InitializeComponent();
			mTitle.Text = "";
			levelProperties.SetData(null);
		}

		private string m_szCategory = "";
		private string m_szSubCategory = "";
		private string m_szDisaster = "";
		public void SetTitleText(string szText)
		{
			
			string[] sepList = { "/" };
			string[] textList = szText.Split(sepList, StringSplitOptions.RemoveEmptyEntries);
			if (textList.Length < 3)
			{
				mTitle.Text = "  " + szText;
				return;
			}
            mTitle.Text = "  " + textList[2];

			m_szCategory = textList[0];
			m_szSubCategory = textList[1];
			m_szDisaster = textList[2];
		}

        public void event_WinRateChanged()
        {
            double fLabelFontSize = mTitle.Font.Size * FormMain.Instance.WindowWidthRate;

            mTitle.Font = new Font(Program.prgFont, (float)fLabelFontSize, FontStyle.Bold);

            panel1.Size = new Size(panel1.Size.Width, (int)((float)panel1.Size.Height * FormMain.Instance.WindowHeightRate));

            fLabelFontSize = mPropertyGrid.Font.Size * FormMain.Instance.WindowWidthRate;
            mPropertyGrid.Font = new Font(Program.prgFont, (float)fLabelFontSize, FontStyle.Bold);
        }
		
		public void ClearSelection()
		{
			mPropertyGrid.SelectedObject = null;
			mTitle.Text = "";
		}       

		public bool SetEnabled
		{
			get
			{
				return mPropertyGrid.Enabled;
			}
			set
			{
				mPropertyGrid.Enabled = value;
			}
		}

		public void SetActionStep(Data_ActionStep actionStep)
		{
			if (actionStep == null)
			{
				m_szCategory = "";
				m_szSubCategory = "";
				m_szDisaster = "";
				mTitle.Text = "";
				mPropertyGrid.SelectedObject = null;
				return;
			}

			levelProperties.CategoryName = m_szCategory;
			levelProperties.SubCategoryName = m_szSubCategory;
			levelProperties.DisasterName = m_szDisaster;
			levelProperties.SetData(actionStep);
			mPropertyGrid.SelectedObject = levelProperties;

			FormMain.Instance.SetStatusText("단계 선택 : " + actionStep.StepName);
		}

		private void FormProperties_Load(object sender, EventArgs e)
		{

			int nSplitterPosition = PropertyGridExtensions.GetInternalLabelWidth(mPropertyGrid);
			PropertyGridExtensions.MoveSplitterTo(mPropertyGrid, nSplitterPosition - 30);
		}

		public override void Refresh()
		{
			base.Refresh();
			mPropertyGrid.Refresh();
		}
	}
}
