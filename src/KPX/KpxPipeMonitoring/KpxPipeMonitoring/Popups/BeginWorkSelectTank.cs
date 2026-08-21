using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpxPipeMonitoring.Popups
{ 
    public partial class BeginWorkSelectTank : Form
    { 
        private int nPipeID { get; set; }
        public int nTankID = -1;
        private int nStableBeginWorkM { get; set; } // 작업 시작 후 ?분 기준

        private int m_nHorzButtonCount = 4;
        private int m_nVertButtonCount = 3;
        private Color m_btnForeColor = Color.Black;
         
        /// <summary>
        /// 배관에서 작업시작할때
        /// </summary>
        /// <param name="selectedPipeId">배관ID</param>
        public BeginWorkSelectTank(int selectedPipeId)
        {
            InitializeComponent();

            this.nPipeID = selectedPipeId; 
            foreach (CommonFunction.PipeInfo item in MainForm.Instance.pipeInfo)
            {
                if (item.nPipeID == this.nPipeID)
                {
                    label_pipeName.Text = item.strPipeName + item.strPipeType;
                    break;
                }
            }

            label_pipeName.Parent = pictureBox1;
            pictureBox_begin.Parent = pictureBox1;
            pictureBox_cancel.Parent = pictureBox1;

            ReadButtonCount(panelTankButtonArea, ref m_nHorzButtonCount, ref m_nVertButtonCount);
            Display();
        }

        public static void ReadButtonCount(Panel panelButtonArea, ref int nHorzButtonCount, ref int nVertButtonCount)
        {
            if (panelButtonArea.Tag == null)
                return;

            if (panelButtonArea.Tag is string)
            {
                string[] tokens = panelButtonArea.Tag.ToString().Trim().Split(',');

                if (tokens.Count() != 2)
                    return;

                int horz, vert;

                if (int.TryParse(tokens[0].Trim(), out horz) && int.TryParse(tokens[1].Trim(), out vert))
                {
                    nHorzButtonCount = horz;
                    nVertButtonCount = vert;
                }
            }
        }

        Image unSelectImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Normal;
        Image selectImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Click;

        public static void GetDisplayInfo(Panel panelButtonArea, Size originImageSize, int nHorzButtonCount, int nVertButtonCount, out int nImageWidth, out int nImageHeight, out int nSpaceHorz, out int nSpaceVert)
        {
            int left = panelButtonArea.Location.X;
            int top = panelButtonArea.Location.Y;
            int right = panelButtonArea.Location.X + panelButtonArea.Size.Width;
            int bottom = panelButtonArea.Location.Y + panelButtonArea.Size.Height;
            int areaWidth = right - left, areaHeight = bottom - top;

            int nOriginImageWidth = originImageSize.Width;
            int nOriginImageHeight = originImageSize.Height;

            nSpaceHorz = nSpaceVert = 0;
            nImageWidth = nOriginImageWidth;
            nImageHeight = nOriginImageHeight;

            if (nOriginImageWidth * nHorzButtonCount <= areaWidth)
                nSpaceHorz = (areaWidth - nOriginImageWidth * nHorzButtonCount) / (nHorzButtonCount - 1);
            else
                nImageWidth = areaWidth / nHorzButtonCount;

            if (nOriginImageHeight * nVertButtonCount <= areaHeight)
                nSpaceVert = (areaHeight - nOriginImageHeight * nVertButtonCount) / (nVertButtonCount - 1);
            else
                nImageHeight = areaHeight / nVertButtonCount;
        }

        private void Display()
        {
            int recentWorkId = -1;
            ArrayList arrList = MainForm.Instance.dbMgr.GetResultData("select tankid from lastworkhistory where pipeid = " + this.nPipeID + " order by begintime desc limit 1", 0);
            if (arrList != null && arrList.Count == 1)
            {
                recentWorkId = DBUtility.WebDBManager.GetIntField(arrList[0].ToString(), -1);
            }

            int nSpaceHorz, nSpaceVert;
            int nImageWidth, nImageHeight;
            GetDisplayInfo(panelTankButtonArea, unSelectImg.Size, m_nHorzButtonCount, m_nVertButtonCount, out nImageWidth, out nImageHeight, out nSpaceHorz, out nSpaceVert);

            int index = 0;

            for (int i=0;i<MainForm.Instance.tankInfo.Count;i++)
            {
                CommonFunction.TankInfo item = MainForm.Instance.tankInfo[i];

                if (item.bDisconnected)
                    continue;

                int x = index % m_nHorzButtonCount;
                int y = index / m_nHorzButtonCount;

                int x2 = panelTankButtonArea.Location.X + x * (nImageWidth + nSpaceHorz);
                int y2 = panelTankButtonArea.Location.Y + y * (nImageHeight + nSpaceVert);
                SettingBtn(item, x2, y2, nImageWidth, nImageHeight, (item.nTankID == recentWorkId));

                index++;
            }
        }
        /*private void Display()
        {
            int btnXEmpty = 34;
            int btnYEmpty = 7;
            int btnWidth = 188;
            int btnHeight = 82;
            int btnX = 73;
            int btnY = 230;
            int btnCnt = 1;
            int btnWidth = 188;
            int btnHeight = 82;

            foreach (CommonFunction.TankOptionInfo item in MainForm.tankOptionInfo)
            {
                if (item.bDisconnected) continue;
                SettingBtn(item, btnX, btnY, btnWidth, btnHeight);

                btnX += btnWidth + btnXEmpty;
                if (btnCnt % 4 == 0)
                {
                    btnY += btnHeight + btnYEmpty;
                    btnX = 73;
                }
                btnCnt++;
            }

            //선택안함
            //SettingBtn(null, btnX, btnY, btnWidth, btnHeight);   
        }*/

        private void SettingBtn(CommonFunction.TankInfo item, int btnX, int btnY, int btnWidth, int btnHeight, bool isSelectedBtn)
        {
            //int btnWidth = 188;
            //int btnHeight = 82;

            PictureBox pic = new PictureBox();
            pic.Size = new Size(btnWidth, btnHeight);
            pic.Location = new Point(btnX, btnY);
            pic.BackColor = Color.Transparent;
            pic.Parent = pictureBox1;
            if (isSelectedBtn)
            {
                pic.Image = selectImg;
                selectedBtn = pic;
            }
            else
                pic.Image = unSelectImg;
            pic.MouseClick += btn_MouseClick;
            pic.Tag = item;
            pic.Cursor = Cursors.Hand;
            pic.SizeMode = PictureBoxSizeMode.StretchImage;

            Label label = new Label();
            label.AutoSize = false;
            label.Font = new System.Drawing.Font("나눔바른고딕", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            label.Size = new Size(btnWidth, btnHeight);
            label.Location = new Point(0, 0);
            label.BackColor = Color.Transparent;
            label.Parent = pictureBox1;
            if (isSelectedBtn)
                label.ForeColor = Color.White;//Color.FromArgb(243, 116, 33);
            else
                label.ForeColor = m_btnForeColor;
            if (item == null)
                label.Text = "선택안함";
            else
                label.Text = item.strTankName;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Tag = item;
            label.Cursor = Cursors.Hand;
            label.MouseClick += btn_MouseClick;

            pic.Controls.Add(label);
            pictureBox1.Controls.Add(pic);
        }

        PictureBox selectedBtn = null;
        void btn_MouseClick(object sender, MouseEventArgs e)
        {
            PictureBox btn = null;
            if (sender is PictureBox)
            {
                btn = sender as PictureBox;
            }
            else if (sender is Label)
            {
                Label label = sender as Label;
                if (label.Parent is PictureBox)
                {
                    btn = label.Parent as PictureBox;
                    label.ForeColor = Color.White;
                }
            }

            if (btn == null) return;
            if (selectedBtn != null)
            {
                selectedBtn.Image = unSelectImg;
                if (selectedBtn.Controls != null && selectedBtn.Controls.Count == 1)
                {
                    //Child Label 글자 색상 바꾸기 
                    Label label = selectedBtn.Controls[0] as Label;
                    label.ForeColor = m_btnForeColor;// Color.FromArgb(243, 116, 33);
                }
            }

            if (btn == selectedBtn)
            {
                btn.Image = unSelectImg;
                selectedBtn = null;
            }
            else
            {
                btn.Image = selectImg;
                selectedBtn = btn;
            }
        } 

        private void pictureBox_begin_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedBtn == null || selectedBtn.Tag == null)
                    throw new ApplicationException("연결할 탱크를 선택하세요.");

                CommonFunction.TankInfo selectedInfo = null;
                
                selectedInfo = (CommonFunction.TankInfo)selectedBtn.Tag; 
                this.nTankID = selectedInfo.nTankID;

                if (MainForm.Instance.commonFunction.ReturnConnectPipeIDs(selectedInfo.nTankID).Count == 2)
                    throw new ApplicationException("이미 다른 배관과 작업중입니다.\r최대 연결할 수 있는 배관의 수는 2개입니다.");

                //// •작업시 시작될 때 압력 또는 유량의 안정범위는 시작될 당시의 값을 기준으로 작성되는데, 이 옵션이 비율(%)일 경우는 값이 0이면 문제가 됨
                //// •안정범위가 0~0이 되기 때문에 항상 알람이 발생하기 때문에 이에 대한 회피 방법이 필요함
                //// •압력 또는 유량이 0이면서 옵션이 비율(%)일 경우에 작업시작 버튼을 누르면 경고문구 메시지 박스를 띄우고 작업은 시작시키지 않는다.
                //StringBuilder sb = new StringBuilder();
                //sb.Append("SELECT p.Pressure, Flow, PipeStableType, TankStableType ");
                //sb.Append("  FROM Pipe as p, Tank as t INNER JOIN AlarmOptions as ao ON t.id=ao.tankid ");
                //sb.Append(" WHERE p.id= " + this.nPipeID + " AND t.id=" + this.nTankID);
                //ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                //if (arrResult == null || arrResult.Count < 4) return;

                //double nPressure = (arrResult[0].ToString() == "null") ? 0 : Convert.ToDouble(arrResult[0]);
                //double nFlow = (arrResult[1].ToString() == "null") ? 0 : Convert.ToDouble(arrResult[1]);
                //double nPipeStableType = DBUtility.WebDBManager.GetIntField(arrResult[2].ToString(), -1);
                //double nTankStableType = DBUtility.WebDBManager.GetIntField(arrResult[3].ToString(), -1);
                 
                //string msg = "";
                //bool isPipeMsg = false;
                //bool isTankMsg = false;
                //if ((nPressure == 0 && nPipeStableType == 0) && (nFlow == 0 && nTankStableType == 0))
                //{
                //    isPipeMsg = true;
                //    isTankMsg = true;
                //    msg = "현재 압력과 유량값이 0입니다.\r이 탱크의 압력, 유량에 관한 정상범위는 비율(%)로 판단하도록 설정되어있어 안정범위를 산정할 수 없습니다.\r압력과 유량값이 0보다 큰 값으로 나타날때까지 기다리거나 비율(%)대신 절대값으로 변경해야합니다.";
                //}
                //else if (nPressure == 0 && nPipeStableType == 0)
                //{
                //    isPipeMsg = true;
                //    msg = "현재 압력값이 0입니다.\r이 탱크의 압력에 관한 정상범위는 비율(%)로 판단하도록 설정되어있어 안정범위를 산정할 수 없습니다.\r압력값이 0보다 큰 값으로 나타날때까지 기다리거나 비율(%)대신 절대값으로 변경해야합니다.";
                //}
                //else if (nFlow == 0 && nTankStableType == 0)
                //{
                //    isTankMsg = true;
                //    if (isPipeMsg)
                //        msg += "\r";
                //    msg += "현재 유량값이 0입니다.\r이 탱크의 유량에 관한 정상범위는 비율(%)로 판단하도록 설정되어있어 안정범위를 산정할 수 없습니다.\r유량이 0보다 큰 값으로 나타날때까지 기다리거나 비율(%)대신 절대값으로 변경해야합니다.";
                //}
                 
                //if (isPipeMsg || isTankMsg)
                //{
                //    msg += "\r절대값으로 변경하시겠습니까?";
                //    if (UnE.Utility.UMessageBox.Show(msg, "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                //        return;

                //    int nCommandID = commonFunction.GetMaxTableID("command") + 1;
                //    int nCommandHistoryID = commonFunction.GetMaxTableID("commandHistory") + 1;
                //    if (isPipeMsg)
                //    {
                //        new EnvironmentPop().SaveOptionSql(nCommandID, nCommandHistoryID, 6, this.nTankID, "PipeStableType", 1);
                //        nCommandID++;
                //        nCommandHistoryID++;
                //    }
                //    if (isTankMsg)
                //    {
                //        new EnvironmentPop().SaveOptionSql(nCommandID, nCommandHistoryID, 6, this.nTankID, "TankStableType", 1); 
                //    }
                //}

                foreach (CommonFunction.AlarmTankOptionInfo item in MainForm.Instance.alarmTankOptionInfo)
                {
                    if (item.nTankID == this.nTankID)
                    {
                        this.nStableBeginWorkM = item.nStableBeginWorkM;
                        break;
                    }
                }

                if (UnE.Utility.UMessageBox.Show(this, "작업을 시작하시겠습니까?\r" + "작업 시작 후 " + this.nStableBeginWorkM + "분동안은 알람이 발생하지 않습니다.", "작업 시작", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    this.DialogResult = System.Windows.Forms.DialogResult.No;
                else
                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            }
            catch (Exception ex)
            {
                UnE.Utility.UMessageBox.Show(ex.Message, "");
            }
        }

        private void pictureBox_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
        }

        Image beginNormalImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Begin_Normal;
        Image beginClickImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Begin_Click;
        Image cancelNormalImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Cancel_Normal;
        Image CancelClickImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Cancel_Click;

        private void pictureBox_begin_MouseEnter(object sender, EventArgs e)
        {
            pictureBox_begin.Image = beginClickImg;
        }

        private void pictureBox_begin_MouseLeave(object sender, EventArgs e)
        {
            pictureBox_begin.Image = beginNormalImg;
        }

        private void pictureBox_cancel_MouseEnter(object sender, EventArgs e)
        {
            pictureBox_cancel.Image = CancelClickImg;
        }

        private void pictureBox_cancel_MouseLeave(object sender, EventArgs e)
        {
            pictureBox_cancel.Image = cancelNormalImg;
        }
    }
}
