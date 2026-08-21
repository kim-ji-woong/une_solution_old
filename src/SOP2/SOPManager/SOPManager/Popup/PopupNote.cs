using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPManager
{
    public partial class PopupNote : Form
    {
        PropertiesProcess m_propertiesProcess = null;
        PropertiesDecision m_propertiesDecision = null;
        PropertiesAnnotation m_propertiesAnnotation = null;
        PropertiesEndPoint m_propertiesEndPoint = null;
        PropertiesLink m_propertiesLink = null;
        PropertiesTransSOP m_propertiesTransSOP = null;
        PropertiesInternal m_propertiesInternal = null;
        PropertiesExternal m_propertiesExternal = null;
        PropertiesTransmission m_propertiesTransmission = null;
		PropertiesGroup m_propertiesGroup = null;

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        private int m_itemID = 0;
        public int ItemID
        {
            get { return m_itemID; }
            set { m_itemID = value; }
        }


        public PopupNote()
        {
            InitializeComponent();

            m_propertiesProcess = FormMain.Instance.GetPageLevel().GetPropertiesProcess();
            m_propertiesDecision = FormMain.Instance.GetPageLevel().GetPropertiesDecision();
            m_propertiesAnnotation = FormMain.Instance.GetPageLevel().GetPropertiesAnnotation();
            m_propertiesEndPoint = FormMain.Instance.GetPageLevel().GetPropertiesEndPoint();
            m_propertiesLink = FormMain.Instance.GetPageLevel().GetPropertiesLink();
            m_propertiesTransSOP = FormMain.Instance.GetPageLevel().GetPropertiesTransSOP();
            m_propertiesInternal = FormMain.Instance.GetPageLevel().GetPropertiesInternal();
            m_propertiesExternal = FormMain.Instance.GetPageLevel().GetPropertiesExternal();
            m_propertiesTransmission = FormMain.Instance.GetPageLevel().GetPropertiesTransmission();

			m_propertiesGroup = FormMain.Instance.GetPageLevel().GetPropertiesGrouup();
            //InitText(ID.ID_ITEM_MISSION);
        }

        public void InitText(int nItemID)
        {
            bool showWarning = false;

            switch (nItemID)
            {
                case ID.ID_ITEM_MISSION:
                    this.Text = "임무 내용 작성";
                    labelNote.Text = "임무 내용";
                    textBox.Text = m_propertiesProcess.Mission.Title;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    showWarning = true;
                    break;
                case ID.ID_ITEM_TASK:
                    this.Text = "업무 내용 작성";
                    labelNote.Text = "업무 내용";
                    textBox.Text = m_propertiesDecision.Task;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    break;
                case ID.ID_ITEM_ANNOTATION_DESC:
                    this.Text = "내용 작성";
                    labelNote.Text = "내용";
                    textBox.Text = m_propertiesAnnotation.Description;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    break;
                case ID.ID_ITEM_ENDPOINT_DESC:
                    this.Text = "내용 작성";
                    labelNote.Text = "내용";
                    textBox.Text = m_propertiesEndPoint.Description;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    break;
                case ID.ID_ITEM_LINK_DESC:
                    this.Text = "내용 작성";
                    labelNote.Text = "내용";
                    textBox.Text = m_propertiesLink.Description;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    break;
                case ID.ID_ITEM_CONTENT:
                    this.Text = "내용 작성";
                    labelNote.Text = "내용";
                    textBox.Text = m_propertiesExternal.Memo;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    showWarning = true;
                    break;
                case ID.ID_ITEM_TRANSMISSION_CONTENT:
                    this.Text = "내용 작성";
                    labelNote.Text = "내용";
                    //textBox.Text = m_propertiesExternal.Memo;
                    textBox.Text = m_propertiesTransmission.ExternalSMSMessage;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    showWarning = true;
                    break;
                case ID.ID_ITEM_TRANSSOP_DESC:
                    this.Text = "설명 작성";
                    labelNote.Text = "설명";
                    textBox.Text = m_propertiesTransSOP.Description;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    break;
                case ID.ID_ITEM_INTERNAL_DESC:
                    this.Text = "내용 작성";
                    labelNote.Text = "내용";
                    textBox.Text = m_propertiesInternal.Description;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    break;
                case ID.ID_ITEM_TRANSMISSION_DESC:
                    this.Text = "내용 작성";
                    labelNote.Text = "내용";
                    textBox.Text = m_propertiesTransmission.Title;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    break;
                case ID.ID_ITEM_EXTERNAL_DESC:
                    this.Text = "내용 작성";
                    labelNote.Text = "내용";
                    textBox.Text = m_propertiesExternal.Description;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    break;

				case ID.ID_ITEM_GROUP_DESC:
					this.Text = "내용 작성";
                    labelNote.Text = "내용";
                    textBox.Text = m_propertiesGroup.Description;
                    btnStandard.Visible = false;
                    m_itemID = nItemID;
                    break;
            }

            if (!showWarning)
            {
                labelWarning.Visible = labelWarning2.Visible = labelWarning3.Visible = false;

                int nNewHeight = 230;
                int nGap = this.Size.Height - nNewHeight;
                this.Size = new Size(this.Size.Width, nNewHeight);

                textBox.Location = new Point(textBox.Location.X, textBox.Location.Y - nGap);
                btnOK.Location = new Point(btnOK.Location.X, btnOK.Location.Y - nGap);
                btnCancel.Location = new Point(btnCancel.Location.X, btnCancel.Location.Y - nGap);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            switch (m_itemID)
            {
                case ID.ID_ITEM_MISSION:
                    m_propertiesProcess.Mission.Title = textBox.Text;
                    m_propertiesProcess.SetSectionUpText();
                    break;
                case ID.ID_ITEM_TASK:
                    m_propertiesDecision.Task = textBox.Text;
                    m_propertiesDecision.SetSectionText();
                    break;
                case ID.ID_ITEM_ANNOTATION_DESC:
                    m_propertiesAnnotation.Description = textBox.Text;
                    m_propertiesAnnotation.SetSectionText();
                    break;
                case ID.ID_ITEM_ENDPOINT_DESC:
                    m_propertiesEndPoint.Description = textBox.Text;
                    m_propertiesEndPoint.SetSectionText();
                    break;
                case ID.ID_ITEM_LINK_DESC:
                    m_propertiesLink.Description = textBox.Text;
                    m_propertiesLink.SetSectionText();
                    break;
                case ID.ID_ITEM_CONTENT:
                    m_propertiesExternal.Memo = textBox.Text;
                    m_propertiesExternal.SetSectionMessage();
                    break;
                case ID.ID_ITEM_TRANSMISSION_CONTENT:
                    m_propertiesTransmission.ExternalSMSMessage = textBox.Text;
                    m_propertiesTransmission.SetSectionMessage();
                    break;
                case ID.ID_ITEM_TRANSSOP_DESC:
                    m_propertiesTransSOP.Description = textBox.Text;
                    m_propertiesTransSOP.SetSectionDescription();
                    break;
                case ID.ID_ITEM_INTERNAL_DESC:
                    m_propertiesInternal.Description = textBox.Text;
                    m_propertiesInternal.SetSectionText();
                    break;
                case ID.ID_ITEM_EXTERNAL_DESC:
                    m_propertiesExternal.Description = textBox.Text;
                    m_propertiesExternal.SetSectionText();
                    break;
                case ID.ID_ITEM_TRANSMISSION_DESC:
                    m_propertiesTransmission.Title = textBox.Text;
                    m_propertiesTransmission.SetSectionText();
                    break;
				case ID.ID_ITEM_GROUP_DESC:
					m_propertiesGroup.Description = textBox.Text;
					m_propertiesGroup.SetSectionText();
					break;

            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void PopupNote_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void PopupNote_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void PopupNote_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

    }
}
