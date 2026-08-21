using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;
using CrisisAlertManager.Popup_Dialog;
using CrisisAlertManager.Data;
using CrisisAlertManager.Popup_Dialog.Message;

namespace CrisisAlertManager.Manual
{
    public partial class uFormManual : UserControl
    {
        private int m_nButtonWidth = 269;
        private int m_nButtonHeight = 85;

        private FacilityType m_facilityType = FacilityType.NONE;
        private string m_strRiskLevel = CommonString.RiskLevel_Normal;

        public uFormManual()
        {
            InitializeComponent();

            rbtnFire_Click(null, null);
        }

        private void rbtnFire_Click(object sender, EventArgs e)
        {
            if (rbtnFire.IsChecked)
                return;

            m_facilityType = FacilityType.FIRE_SENSOR;

            rbtnFire.IsChecked = true;
            rbtnFlood.IsChecked = false;
            rbtnHeat.IsChecked = false;
            rbtnCollapse.IsChecked = false;

            rbtnFire.Refresh();
            rbtnFlood.Refresh();
            rbtnHeat.Refresh();
            rbtnCollapse.Refresh();

            plFire.Visible = true;
            plFlood.Visible = false;
            plHeat.Visible = false;
            plCollapse.Visible = false;

            // 관심 메뉴얼 보기
            if (rbtnFireAttention.IsChecked)
            {
                m_strRiskLevel = CommonString.RiskLevel_Attention;
                ShowManual();
            }
            else
                rbtnFireAttention_Click(null, null);
        }

        private void rbtnFlood_Click(object sender, EventArgs e)
        {
            if (rbtnFlood.IsChecked)
                return;

            m_facilityType = FacilityType.FLOOD_SENSOR;

            rbtnFire.IsChecked = false;
            rbtnFlood.IsChecked = true;
            rbtnHeat.IsChecked = false;
            rbtnCollapse.IsChecked = false;

            rbtnFire.Refresh();
            rbtnFlood.Refresh();
            rbtnHeat.Refresh();
            rbtnCollapse.Refresh();

            plFire.Visible = false;
            plFlood.Visible = true;
            plHeat.Visible = false;
            plCollapse.Visible = false;

            if (rbtnFloodAttention.IsChecked)
            {
                m_strRiskLevel = CommonString.RiskLevel_Attention;
                ShowManual();
            }
            else
                rbtnFloodAttention_Click(null, null);
        }

        private void rbtnHeat_Click(object sender, EventArgs e)
        {
            if (rbtnHeat.IsChecked)
                return;

            m_facilityType = FacilityType.HEAT_SENSOR;

            rbtnFire.IsChecked = false;
            rbtnFlood.IsChecked = false;
            rbtnHeat.IsChecked = true;
            rbtnCollapse.IsChecked = false;

            rbtnFire.Refresh();
            rbtnFlood.Refresh();
            rbtnHeat.Refresh();
            rbtnCollapse.Refresh();

            plFire.Visible = false;
            plFlood.Visible = false;
            plHeat.Visible = true;
            plCollapse.Visible = false;

            if (rbtnHeatAttention.IsChecked)
            {
                m_strRiskLevel = CommonString.RiskLevel_Attention;
                ShowManual();
            }
            else
                rbtnHeatAttention_Click(null, null);


        }

        private void rbtnCollapse_Click(object sender, EventArgs e)
        {
            if (rbtnCollapse.IsChecked)
                return;

            m_facilityType = FacilityType.COLLAPSE_SENSOR;

            rbtnFire.IsChecked = false;
            rbtnFlood.IsChecked = false;
            rbtnHeat.IsChecked = false;
            rbtnCollapse.IsChecked = true;

            rbtnFire.Refresh();
            rbtnFlood.Refresh();
            rbtnHeat.Refresh();
            rbtnCollapse.Refresh();

            plFire.Visible = false;
            plFlood.Visible = false;
            plHeat.Visible = false;
            plCollapse.Visible = true;

            if (rbtnCollapseAttention.IsChecked)
            {
                m_strRiskLevel = CommonString.RiskLevel_Attention;
                ShowManual();
            }
            else
                rbtnCollapseAttention_Click(null, null);
        }

        private void rbtnFireAttention_Click(object sender, EventArgs e)
        {
            if (rbtnFireAttention.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Attention;

            // 버튼 상태변화
            rbtnFireAttention.IsChecked = true;
            rbtnFireCaution.IsChecked = false;
            rbtnFireAlert.IsChecked = false;
            rbtnFireSerious.IsChecked = false;

            rbtnFireAttention.Refresh();
            rbtnFireCaution.Refresh();
            rbtnFireAlert.Refresh();
            rbtnFireSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void ShowManual()
        {
            Panel panel = null;

            if (m_facilityType == FacilityType.FIRE_SENSOR)
                panel = plFireManual;
            else if (m_facilityType == FacilityType.FLOOD_SENSOR)
                panel = plFloodManual;
            else if (m_facilityType == FacilityType.HEAT_SENSOR)
                panel = plHeatManual;
            else
                panel = plCollapseManual;

            panel.Controls.Clear();

            // 메뉴얼 목록 불러오기
            Dictionary<int, FacilityManual> dicFacilityManual = FormMain.Instance.DataManager.LoadFacilityRiskLevelManuals(m_facilityType, m_strRiskLevel);

            foreach (KeyValuePair<int, FacilityManual> pair in dicFacilityManual)
            {
                FacilityManual manual = pair.Value;

                TreeButton btn = new TreeButton();
                btn.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnManualNew_Normal;
                btn.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnManualNew_Normal;
                btn.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnManualNew_Click;
                btn.Size = new Size(m_nButtonWidth, m_nButtonHeight);
                btn.Location = new Point(60 + (panel.Controls.Count % 4) * (m_nButtonWidth + 40), 30 + (panel.Controls.Count / 4) * (m_nButtonHeight + 30));
                btn.ButtonText = manual.Number + ". " + manual.Title;
                btn.MouseClick += btnManual_Click;
                btn.MouseDoubleClick += btnManual_DoubleClick;
                btn.Tag = manual;

                panel.Controls.Add(btn);
            }


        }

        private void btnManual_Click(object sender, EventArgs e)
        {
            TreeButton btn = sender as TreeButton;
            if (btn == null)
                return;

            btn.IsChecked = !btn.IsChecked;

            // 체크할 경우
            if (btn.IsChecked)
            {
                btn.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnManualNew_Click;
                btn.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnManualNew_Click;
                btn.Refresh();
            }
            else // 체크해제
            {
                btn.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnManualNew_Normal;
                btn.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnManualNew_Normal;
                btn.Refresh();
            }
        }

        private void btnManual_DoubleClick(object sender, EventArgs e)
        {
            TreeButton btn = sender as TreeButton;
            if (btn == null)
                return;

            btn.IsChecked = !btn.IsChecked;

            // 체크할 경우
            if (btn.IsChecked)
            {
                btn.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnManualNew_Click;
                btn.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnManualNew_Click;
                btn.Refresh();
            }
            else // 체크해제
            {
                btn.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnManualNew_Normal;
                btn.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnManualNew_Normal;
                btn.Refresh();
            }


            // 메뉴얼 수정창 띄우기
            if (btn.Tag == null)
                return;

            FacilityManual manual = (FacilityManual)btn.Tag;

            FormManualInfo manualInfo = new FormManualInfo(manual);
            manualInfo.StartPosition = FormStartPosition.CenterParent;

            if (manualInfo.ShowDialog() == DialogResult.Yes)
            {
                // 데이터 재 불러오기(추가된 데이터 포함)
                FormMain.Instance.DataManager.LoadManual();
                ShowManual();
            }
        }

        private void rbtnFireCaution_Click(object sender, EventArgs e)
        {
            if (rbtnFireCaution.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Caution;

            // 버튼 상태변화
            rbtnFireAttention.IsChecked = false;
            rbtnFireCaution.IsChecked = true;
            rbtnFireAlert.IsChecked = false;
            rbtnFireSerious.IsChecked = false;

            rbtnFireAttention.Refresh();
            rbtnFireCaution.Refresh();
            rbtnFireAlert.Refresh();
            rbtnFireSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnFireAlert_Click(object sender, EventArgs e)
        {
            if (rbtnFireAlert.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Alert;

            // 버튼 상태변화
            rbtnFireAttention.IsChecked = false;
            rbtnFireCaution.IsChecked = false;
            rbtnFireAlert.IsChecked = true;
            rbtnFireSerious.IsChecked = false;

            rbtnFireAttention.Refresh();
            rbtnFireCaution.Refresh();
            rbtnFireAlert.Refresh();
            rbtnFireSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnFireSerious_Click(object sender, EventArgs e)
        {
            if (rbtnFireSerious.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Serious;

            // 버튼 상태변화
            rbtnFireAttention.IsChecked = false;
            rbtnFireCaution.IsChecked = false;
            rbtnFireAlert.IsChecked = false;
            rbtnFireSerious.IsChecked = true;

            rbtnFireAttention.Refresh();
            rbtnFireCaution.Refresh();
            rbtnFireAlert.Refresh();
            rbtnFireSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnFloodAttention_Click(object sender, EventArgs e)
        {
            if (rbtnFloodAttention.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Attention;

            rbtnFloodAttention.IsChecked = true;
            rbtnFloodCaution.IsChecked = false;
            rbtnFloodAlert.IsChecked = false;
            rbtnFloodSerious.IsChecked = false;

            rbtnFloodAttention.Refresh();
            rbtnFloodCaution.Refresh();
            rbtnFloodAlert.Refresh();
            rbtnFloodSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnFloodCaution_Click(object sender, EventArgs e)
        {
            if (rbtnFloodCaution.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Caution;

            rbtnFloodAttention.IsChecked = false;
            rbtnFloodCaution.IsChecked = true;
            rbtnFloodAlert.IsChecked = false;
            rbtnFloodSerious.IsChecked = false;

            rbtnFloodAttention.Refresh();
            rbtnFloodCaution.Refresh();
            rbtnFloodAlert.Refresh();
            rbtnFloodSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnFloodAlert_Click(object sender, EventArgs e)
        {
            if (rbtnFloodAlert.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Alert;

            rbtnFloodAttention.IsChecked = false;
            rbtnFloodCaution.IsChecked = false;
            rbtnFloodAlert.IsChecked = true;
            rbtnFloodSerious.IsChecked = false;

            rbtnFloodAttention.Refresh();
            rbtnFloodCaution.Refresh();
            rbtnFloodAlert.Refresh();
            rbtnFloodSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnFloodSerious_Click(object sender, EventArgs e)
        {
            if (rbtnFloodSerious.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Serious;

            rbtnFloodAttention.IsChecked = false;
            rbtnFloodCaution.IsChecked = false;
            rbtnFloodAlert.IsChecked = false;
            rbtnFloodSerious.IsChecked = true;

            rbtnFloodAttention.Refresh();
            rbtnFloodCaution.Refresh();
            rbtnFloodAlert.Refresh();
            rbtnFloodSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnHeatAttention_Click(object sender, EventArgs e)
        {
            if (rbtnHeatAttention.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Attention;

            rbtnHeatAttention.IsChecked = true;
            rbtnHeatCaution.IsChecked = false;
            rbtnHeatAlert.IsChecked = false;
            rbtnHeatSerious.IsChecked = false;

            rbtnHeatAttention.Refresh();
            rbtnHeatCaution.Refresh();
            rbtnHeatAlert.Refresh();
            rbtnHeatSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnHeatCaution_Click(object sender, EventArgs e)
        {
            if (rbtnHeatCaution.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Caution;

            rbtnHeatAttention.IsChecked = false;
            rbtnHeatCaution.IsChecked = true;
            rbtnHeatAlert.IsChecked = false;
            rbtnHeatSerious.IsChecked = false;

            rbtnHeatAttention.Refresh();
            rbtnHeatCaution.Refresh();
            rbtnHeatAlert.Refresh();
            rbtnHeatSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnHeatAlert_Click(object sender, EventArgs e)
        {
            if (rbtnHeatAlert.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Alert;

            rbtnHeatAttention.IsChecked = false;
            rbtnHeatCaution.IsChecked = false;
            rbtnHeatAlert.IsChecked = true;
            rbtnHeatSerious.IsChecked = false;

            rbtnHeatAttention.Refresh();
            rbtnHeatCaution.Refresh();
            rbtnHeatAlert.Refresh();
            rbtnHeatSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnHeatSerious_Click(object sender, EventArgs e)
        {
            if (rbtnHeatSerious.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Serious;

            rbtnHeatAttention.IsChecked = false;
            rbtnHeatCaution.IsChecked = false;
            rbtnHeatAlert.IsChecked = false;
            rbtnHeatSerious.IsChecked = true;

            rbtnHeatAttention.Refresh();
            rbtnHeatCaution.Refresh();
            rbtnHeatAlert.Refresh();
            rbtnHeatSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnCollapseAttention_Click(object sender, EventArgs e)
        {
            if (rbtnCollapseAttention.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Attention;

            rbtnCollapseAttention.IsChecked = true;
            rbtnCollapseCaution.IsChecked = false;
            rbtnCollapseAlert.IsChecked = false;
            rbtnCollapseSerious.IsChecked = false;

            rbtnCollapseAttention.Refresh();
            rbtnCollapseCaution.Refresh();
            rbtnCollapseAlert.Refresh();
            rbtnCollapseSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnCollapseCaution_Click(object sender, EventArgs e)
        {
            if (rbtnCollapseCaution.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Caution;

            rbtnCollapseAttention.IsChecked = false;
            rbtnCollapseCaution.IsChecked = true;
            rbtnCollapseAlert.IsChecked = false;
            rbtnCollapseSerious.IsChecked = false;

            rbtnCollapseAttention.Refresh();
            rbtnCollapseCaution.Refresh();
            rbtnCollapseAlert.Refresh();
            rbtnCollapseSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnCollapseAlert_Click(object sender, EventArgs e)
        {
            if (rbtnCollapseAlert.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Alert;

            rbtnCollapseAttention.IsChecked = false;
            rbtnCollapseCaution.IsChecked = false;
            rbtnCollapseAlert.IsChecked = true;
            rbtnCollapseSerious.IsChecked = false;

            rbtnCollapseAttention.Refresh();
            rbtnCollapseCaution.Refresh();
            rbtnCollapseAlert.Refresh();
            rbtnCollapseSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void rbtnCollapseSerious_Click(object sender, EventArgs e)
        {
            if (rbtnCollapseSerious.IsChecked)
                return;

            m_strRiskLevel = CommonString.RiskLevel_Serious;

            rbtnCollapseAttention.IsChecked = false;
            rbtnCollapseCaution.IsChecked = false;
            rbtnCollapseAlert.IsChecked = false;
            rbtnCollapseSerious.IsChecked = true;

            rbtnCollapseAttention.Refresh();
            rbtnCollapseCaution.Refresh();
            rbtnCollapseAlert.Refresh();
            rbtnCollapseSerious.Refresh();

            // 메뉴얼 버튼 표시
            ShowManual();
        }

        private void btnAddManual_Click(object sender, EventArgs e)
        {
            FormManualInfo manualInfo = new FormManualInfo(m_facilityType, m_strRiskLevel);
            manualInfo.StartPosition = FormStartPosition.CenterParent;

            if (manualInfo.ShowDialog() == DialogResult.Yes)
            {
                // 데이터 재 불러오기(추가된 데이터 포함)
                FormMain.Instance.DataManager.LoadManual();
                ShowManual();
            }
        }

        public class TreeButton : ImageButton
        {
            public bool IsChecked = false;
        }

        private void btnRemoveManual_Click(object sender, EventArgs e)
        {
            bool bChk = true;
            List<FacilityManual> listRemoveManual = new List<FacilityManual>();

            // 체크된 버튼 찾기
            Panel panel = null;

            if (m_facilityType == FacilityType.FIRE_SENSOR)
                panel = plFireManual;
            else if (m_facilityType == FacilityType.FLOOD_SENSOR)
                panel = plFloodManual;
            else if (m_facilityType == FacilityType.HEAT_SENSOR)
                panel = plHeatManual;
            else
                panel = plCollapseManual;

            for (int i = 0; i < panel.Controls.Count; i++)
            {
                object control = panel.Controls[i];

                TreeButton btn = control as TreeButton;
                if (btn == null || btn.Tag == null)
                    continue;

                FacilityManual manual = (FacilityManual)btn.Tag;

                if (btn.IsChecked == true)
                {
                    int nID = manual.ID;
                    listRemoveManual.Add(manual);
                }
            }

            if (listRemoveManual.Count == 0)
                return;

            // 경고문
            FormMessageBox msg = new FormMessageBox("행동요령 삭제 확인", "행동요령 삭제 하시겠습니까?", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;

            if (msg.ShowDialog() == DialogResult.No)
                return;

            // id 값으로 삭제
            foreach (FacilityManual manual in listRemoveManual)
            {
                bool bResult = false;

                bResult = FormMain.Instance.DataManager.DeleteFacilityManual(manual.ID);


                if (bResult)
                    CheckSubNumberManual(manual.Number + 1);
                else
                    bChk = false;
            }

            if (!bChk)
            {
                msg = new FormMessageBox("실패", "메뉴얼 DB 삭제가 실패하였습니다. \n관리자에게 문의 해주세요.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
            }

            // 데이터 재 불러오기
            FormMain.Instance.DataManager.LoadManual();
            ShowManual();
        }

        private void CheckSubNumberManual(int nNum)
        {
            // 해당 순서 메뉴얼을 찾아 있다면 -1을 한다.
            FacilityManual manual = null;

            manual = FormMain.Instance.DataManager.CheckNumberManuals(m_facilityType, m_strRiskLevel, nNum);

            if (manual == null)
                return;

            CheckSubNumberManual(nNum + 1);

            manual.Number = manual.Number - 1;
            FormMain.Instance.DataManager.UpdateFacilityManual(manual.ID, manual.Title, manual.Members, manual.Number, manual.Manual, manual.RiskLevel);
        }

        private void plHeatManual_Click(object sender, EventArgs e)
        {
            Panel panel = plHeatManual;

            ResetManualButton(panel);
        }

        private void plCollapseManual_Click(object sender, EventArgs e)
        {
            Panel panel = plCollapseManual;

            ResetManualButton(panel);
        }

        private void plFireManual_Click(object sender, EventArgs e)
        {
            Panel panel = plFireManual;

            ResetManualButton(panel);
        }

        private void ResetManualButton(Panel panel)
        {
            foreach (Control control in panel.Controls)
            {
                if (control.GetType() == typeof(TreeButton))
                {
                    TreeButton button = control as TreeButton;
                    button.IsChecked = false;

                    button.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnManualNew_Normal;
                    button.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnManualNew_Normal;
                    button.Refresh();
                }
            }
        }

        private void plFloodManual_Click(object sender, EventArgs e)
        {
            Panel panel = plFloodManual;

            ResetManualButton(panel);
        }
    }
}
