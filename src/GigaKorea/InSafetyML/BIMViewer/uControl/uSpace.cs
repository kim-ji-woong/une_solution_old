using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BIMViewer.BIM;

namespace BIMViewer.uControl
{
    public partial class uSpace : UserControl
    {
        public uSpace()
        {
            InitializeComponent();           
        }
        
        private Space m_uSpace = null;
        private bool m_bStairType = false;
        private string m_strSpaceName = "";     // 공간 이름 변경 체크
        private BIM.Level m_level = null;

        public void SetSpaceData(Space space, string strLevelName, BIM.Level level)
        {
            //층수, 객체번호, 객체명
            m_uSpace = space;
            m_level = level;
            txtObject.Text = " 공간";
            txtID.Text = m_uSpace.XMLID;
            lblFloor.Text = strLevelName;

            m_bStairType = false;
            txtRoomType.Text = GetRoomType(space.Properties);

            string strName = m_uSpace.Name;
            int i;

            i = strName.IndexOf("_");
            txtSpaceName.Text = " " + strName.Substring(0, i);
            m_strSpaceName = " " + strName.Substring(0, i);

            //방화구역 
            cmbYN.SelectedIndex = m_uSpace.SafetyFire ? 1 : 0;

            // 계단실 종류
            if (m_bStairType == true)
            {
                lbSafetyFire.Location = new Point(lbSafetyFire.Location.X, 21);
                cmbYN.Location = new Point(cmbYN.Location.X, 18);

                lbStairType.Visible = true;
                cmbStairType.Visible = true;


            }
            else    //if (m_bStairType == false)
            {
                lbSafetyFire.Location = new Point(lbSafetyFire.Location.X, 34);
                cmbYN.Location = new Point(cmbYN.Location.X, 31);

                lbStairType.Visible = false;
                cmbStairType.Visible = false;
            }
        }
        public void UpdateUserData()
        {
            bool bUnderChk = false;
            bool bChangeChk = false;

            // 방화구역유무 변경 체크
            bool bSafetyFire = (cmbYN.SelectedIndex == 1) ? true : false;

            string strUnder = "B";
            string strFloor = lblFloor.Text;

            bUnderChk = strFloor.Contains(strUnder);

            // 공간명, 방화구역
            int i;
            i = lblFloor.Text.IndexOf(" ");

            if (!bUnderChk)
                m_uSpace.Name = txtSpaceName.Text.Trim() + "_level" + lblFloor.Text.Substring(2, lblFloor.Text.Length - i - 1) + "F";
            else
                m_uSpace.Name = txtSpaceName.Text.Trim() + "_levelB" + lblFloor.Text.Substring(2, lblFloor.Text.Length - i - 1) + "F";

            // 공간이름 변경 유무 체크
            if (m_strSpaceName != txtSpaceName.Text)
                bChangeChk = true;

            // 방화구역유무 변경 체크
            if (m_uSpace.SafetyFire != bSafetyFire)
                bChangeChk = true;

            m_uSpace.SafetyFire = (cmbYN.SelectedIndex == 1) ? true : false;

            foreach (Property prop in m_uSpace.Properties)
            {
                if (prop.Name == "방화구역유무")
                    prop.Value = m_uSpace.SafetyFire ? "1" : "0";

                if (prop.Name == "계단실종류")
                {
                    string strOld = (string)prop.Value;
                    string strNew = (string)cmbStairType.SelectedItem;

                    prop.Value = (string)cmbStairType.SelectedItem;

                    if (strOld != strNew)
                        bChangeChk = true;
                }     
            }

            // 변경이 되었다면 Level XML ID 수정
            if (bChangeChk == true)
            {
                if (!bUnderChk)
                    m_level.XMLID = "level" + lblFloor.Text.Substring(2, lblFloor.Text.Length - i - 1) + "F";
                else
                    m_level.XMLID = "levelB" + lblFloor.Text.Substring(2, lblFloor.Text.Length - i - 1) + "F";
            }
        }

        private string GetRoomType(List<Property> properties)
        {
            string strRootType = "";

            foreach (Property pro in properties)
            {
                if (pro.Name == "실종류")
                    strRootType = pro.Value;

                if (pro.Name == "계단실종류")
                    cmbStairType.SelectedItem = pro.Value;
            }

            if (strRootType == "계단실")
                m_bStairType = true;

            return strRootType;
        }
    }
}
