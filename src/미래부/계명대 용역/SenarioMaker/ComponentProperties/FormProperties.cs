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


namespace UnE.SenarioMaker
{
    internal partial class FormProperties : Form
    {

        //시작/끝
        private PropertyEndpoint mEndPoint = new PropertyEndpoint();

        //프로세스
        private PropertyDecisionProcess mProcess = new PropertyDecisionProcess();

        //설명
        private PropertyAnnotation mAnnotation = new PropertyAnnotation();

        public FormProperties()
        {
            InitializeComponent();



        }

        public void ClearSelection()
        {
            mPropertyGrid.SelectedObject = null;

            mAnnotation.SetData(null);

        }
 
        public void SetComponent(Sections.Section section)
        {
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
                ClearSelection();
                return;
            }

            ClearSelection();
            string szType = "프로세스";
            switch(nType)
            {
                case Section.ComponentType.ANNOTATION:                    
                    mAnnotation.SetData(section); 
                    mPropertyGrid.SelectedObject = mAnnotation;
                    szType = "주석";
                    break;

                case Section.ComponentType.DECISION:
                    mProcess.SetData(section);
                    mPropertyGrid.SelectedObject = mProcess;
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
                    break;
            }

            FormMain.Instance.SetStatusText(szType +" 컴포넌트 선택");
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
            PropertyGridExtensions.MoveSplitterTo(mPropertyGrid, nSplitterPosition - 40);
        }
    }


    public static class PropertyGridExtensions
    {
        /// <summary>
        /// Gets the (private) PropertyGridView instance.
        /// </summary>
        /// <param name="propertyGrid">The property grid.</param>
        /// <returns>The PropertyGridView instance.</returns>
        private static object GetPropertyGridView(PropertyGrid propertyGrid)
        { 
            //private PropertyGridView GetPropertyGridView();
            //PropertyGridView is an internal class...
            MethodInfo methodInfo = typeof(PropertyGrid).GetMethod("GetPropertyGridView", BindingFlags.NonPublic | BindingFlags.Instance);
            return methodInfo.Invoke(propertyGrid, new object[] {});
        }

        /// <summary>
        /// Gets the width of the left column.
        /// </summary>
        /// <param name="propertyGrid">The property grid.</param>
        /// <returns>
        /// The width of the left column.
        /// </returns>
        public static int GetInternalLabelWidth(this PropertyGrid propertyGrid)
        {
            //System.Windows.Forms.PropertyGridInternal.PropertyGridView
            object gridView = GetPropertyGridView(propertyGrid);

            //protected int InternalLabelWidth
            PropertyInfo propInfo = gridView.GetType().GetProperty("InternalLabelWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)propInfo.GetValue(gridView);
        }

        /// <summary>
        /// Moves the splitter to the supplied horizontal position.
        /// </summary>
        /// <param name="propertyGrid">The property grid.</param>
        /// <param name="xpos">The horizontal position.</param>
        public static void MoveSplitterTo(this PropertyGrid propertyGrid, int xpos)
        {
            //System.Windows.Forms.PropertyGridInternal.PropertyGridView
            object gridView = GetPropertyGridView(propertyGrid);

            //private void MoveSplitterTo(int xpos);
            MethodInfo methodInfo = gridView.GetType().GetMethod("MoveSplitterTo", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(gridView, new object[] { xpos });
        }
    }

}
