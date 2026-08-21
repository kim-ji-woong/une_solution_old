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
    internal partial class FormProperties : Form
    {
        //시작/끝
		protected PropertiesEndPoint mEndPoint = new PropertiesEndPoint();
        //프로세스
		protected PropertiesProcess mProcess = new PropertiesProcess();
        //설명
		protected PropertiesAnnotation mAnnotation = new PropertiesAnnotation();
		// 판단
		protected PropertiesDecision mDecision = new PropertiesDecision();
		// 그룹
		protected PropertiesGroup mGroup = new PropertiesGroup();
		// 외부 상황전파
		protected PropertiesExternal mExternal = new PropertiesExternal();
		// 내부 상황전파
		protected PropertiesInternal mInternal = new PropertiesInternal();
		// 상황전파
		protected PropertiesTransmission mTransmission = new PropertiesTransmission();
		// 프로세스 링크
		protected PropertiesLink mLink = new PropertiesLink();
		// 다른 SOP로 전환
		protected PropertiesTransSOP mTransSOP = new PropertiesTransSOP();		

        public FormProperties()
        {
            InitializeComponent();

            mAnnotation.SetParent(mPropertyGrid);

            mProcess.SetParent(mPropertyGrid);
            mDecision.SetParent(mPropertyGrid);
            mGroup.SetParent(mPropertyGrid);
            mExternal.SetParent(mPropertyGrid);

            mEndPoint.SetParent(mPropertyGrid);
            mTransmission.SetParent(mPropertyGrid);
            mLink.SetParent(mPropertyGrid);
            mInternal.SetParent(mPropertyGrid);
            mTransSOP.SetParent(mPropertyGrid);
            
        }

        public void ClearSelection()
        {
            mPropertyGrid.SelectedObject = null;          
			mAnnotation.SetData(null);
            
			mProcess.SetData(null);
			mDecision.SetData(null);
			//mGroup.SetData(null);
			//mExternal.SetData(null);

			//mInternal.SetData(null);
			mEndPoint.SetData(null);
			//mTransmission.SetData(null);
			//mLink.SetData(null);

			//mInternal.SetData(null);
			mTransSOP.SetData(null);
        }

        public SectionPropertiesBase GetCurrentProperty()
        {
            return (SectionPropertiesBase)mPropertyGrid.SelectedObject;
        }

        public void SetComponent(Sections.Section section)
        {

			ClearSelection();
			mTitle.Text = "컴포넌트 속성";

            if (section == null)
            {
                FormMain.Instance.SetStatusText("컴포넌트 선택 취소");
                ClearSelection();
                return;
            }                

            Section.ComponentType nType = section.GetComponentType();
            SectionData data = section.Data;
            if (data == null)
            {		
                return;
            }
            
            string szType = "";
            switch(nType)
            {
                case Section.ComponentType.ANNOTATION:                    
                    mAnnotation.SetData(section); 
                    mPropertyGrid.SelectedObject = mAnnotation;
                    szType = "설명";
                    break;

                case Section.ComponentType.DECISION:
					mDecision.SetData(section);
					mPropertyGrid.SelectedObject = mDecision;
                    szType = "비교/판단";
                    break;

                case Section.ComponentType.ENDPOINT:
                    mEndPoint.SetData(section);
                    mPropertyGrid.SelectedObject = mEndPoint;
                    szType = "시작/종료";
                    break;

                case Section.ComponentType.PROCESS:
                    mProcess.SetData(section);
                    mPropertyGrid.SelectedObject = mProcess;
					szType = "프로세스";
                    break;
				case Section.ComponentType.EXTERNAL:
					mExternal.SetData(section);
					mPropertyGrid.SelectedObject = mExternal;
					szType = "외부상황전파";
					break;
				case Section.ComponentType.INTERNAL:
					mInternal.SetData(section);
					mPropertyGrid.SelectedObject = mInternal;
					szType = "내부상황전파";
					break;
				case Section.ComponentType.LINK:
					mLink.SetData(section);
					mPropertyGrid.SelectedObject = mLink;
					szType  = "프로세스 연결";
					break;
				case Sections.Section.ComponentType.TRANSMISSION:
					this.mTransmission.SetData(section);
					mPropertyGrid.SelectedObject = mTransmission;
					szType = "상황전파";
					break;
				case Sections.Section.ComponentType.TRANSSOP:
					this.mTransSOP.SetData(section);
					mPropertyGrid.SelectedObject = mTransSOP;
					szType = "시나리오 전환";
					break;
				case Sections.Section.ComponentType.GROUP:
					this.mGroup.SetData(section);
					mPropertyGrid.SelectedObject = mGroup;
					szType = "컴포턴트 그룹";
					break;
				
            }

            //GridItem item = mPropertyGrid.SelectedGridItem;
            //if( item != null)
            //{
            //    while (item.Parent != null)
            //    {
            //        item = item.Parent;
            //    }
            //    int nGrid = item.GridItems.Count;
            //    if( nGrid > 0)
            //    {
            //        item.GridItems[nGrid - 1].Expanded = false;
            //    }               
            //}
         
			if (szType != "")
			{
				mTitle.Text = "컴포넌트 속성 - " + szType;
				FormMain.Instance.SetStatusText(szType + " 컴포넌트 선택");
			}
			
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

        private void FormProperties_Load(object sender, EventArgs e)
        {

            int nSplitterPosition = PropertyGridExtensions.GetInternalLabelWidth(mPropertyGrid);
            PropertyGridExtensions.MoveSplitterTo(mPropertyGrid, nSplitterPosition - 30);
        }
    }
}
