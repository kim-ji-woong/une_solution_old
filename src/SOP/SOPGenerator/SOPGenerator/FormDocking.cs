using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using XtremeShortcutBar;

namespace SOPGen
{
    public partial class FormDocking : Form
    {
        private FormMain m_Main = null;
        private FormDockingCircumstances m_frmCircum = null;
        private FormStandard m_frmStandard = null;

        private ArrayList m_arrForm = new ArrayList();

        //private ProcessSection m_currentProcess = null;

        private string m_strValue = "상황전파";
        private string m_strTitle = "";
        private int m_nShortcutID = 0;
        private int m_nItemID = 0;
        private bool m_isAdd = false;

        public bool AddCheck
        {
            get { return m_isAdd; }
            set { m_isAdd = value; }
        }

        public FormDocking(FormMain main)
        {
            InitializeComponent();

            m_Main = main;

            Initialize();
            CreateShortcutBar();
        }

        private void Initialize()
        {
            m_frmCircum = new FormDockingCircumstances(this);
            m_frmStandard = new FormStandard(this);
        }

        public FormMain GetMain()
        {
            return m_Main;
        }

        public FormDockingCircumstances GetCircumstances()
        {
            FormDockingCircumstances frmCircum = (FormDockingCircumstances)m_arrForm[m_nItemID];
            return frmCircum;
        }

        public FormStandard GetStandard()
        {
            return m_frmStandard;
        }

        private void CreateCircum()
        {
            m_frmCircum = new FormDockingCircumstances(this);
            m_arrForm.Add(m_frmCircum);
        }

        public void CreateShortcutBar()
        {
            string strValue = m_strValue + Convert.ToInt32(m_nShortcutID + 1);
            CreateCircum();
            ShortcutBarItem ItemCircum = axShortcutBar.AddItem(m_nShortcutID, strValue, m_frmCircum.Handle.ToInt32());

            axShortcutBar.Selected = ItemCircum;
            axShortcutBar.ExpandedLinesCount = m_nShortcutID;
        }

        private void tsbtnAdd_Click(object sender, EventArgs e)
        {
            AddCheck = true;
            m_nShortcutID++;
            CreateCircum();
            ShowEditTitle();
            ShortcutBarItem ItemCircum = axShortcutBar.AddItem(m_nShortcutID, m_strTitle, m_frmCircum.Handle.ToInt32());

            axShortcutBar.Selected = ItemCircum;
            axShortcutBar.ExpandedLinesCount = m_nShortcutID;
        }

        private void tsbtnDel_Click(object sender, EventArgs e)
        {
            if (axShortcutBar.ItemCount > 2)
            {
                string strValue = "선택한 " + m_strTitle + "을(를) 삭제하시겠습니까?";
                DialogResult result = MessageBox.Show(strValue, "상황전파 삭제", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    axShortcutBar.RemoveItem(m_nItemID);
                    
                    ShortcutBarItem ItemCircum = axShortcutBar.FindItem(0);
                    axShortcutBar.Selected = ItemCircum;
                }
            }
            else
            {
                m_frmCircum.NewSOP();
            }
        }

        private void tsbtnEdit_Click(object sender, EventArgs e)
        {
            ShowEditTitle();
        }

        private void axShortcutBar_SelectedChanged(object sender, AxXtremeShortcutBar._DShortcutBarEvents_SelectedChangedEvent e)
        {
            AddCheck = false;
            m_nItemID = e.item.Id;
            m_strTitle = e.item.Caption;
        }

        private void ShowEditTitle()
        {
            FormEditTitle frmEdit = new FormEditTitle(this);
            frmEdit.ShowDialog();
        }

        public string GetTitle()
        {
            if (AddCheck)
                m_strTitle = m_strValue + Convert.ToInt32(m_nShortcutID + 1);
            else
                m_strTitle = axShortcutBar.Selected.Caption;

            return m_strTitle;
        }

        public void SetEditTitle(string strTitle)
        {
            if(AddCheck)
            {
                m_strTitle = strTitle;
            }
            else
            {
                ShortcutBarItem ItemCircum = null;
                ItemCircum = axShortcutBar.FindItem(m_nItemID);
                ItemCircum.Caption = strTitle;
            }
        }

        public void NewSOP()
        {
            m_frmCircum.NewSOP();
        }

    }

    public class ProcessSection
    {
        string m_strCellphone1;
        string m_strCellphone2;
        string m_strCellphone3;
        string m_strMessage;
        string m_strFAX1;
        string m_strFAX2;
        string m_strFAX3;
        string m_strFAXFile;
        string m_strBroadcast;
        SectionTimeText m_linkedSection = null;


        public string CellPhone1
        {
            get { return m_strCellphone1; }
            set { m_strCellphone1 = value; }
        }

        public string CellPhone2
        {
            get { return m_strCellphone2; }
            set { m_strCellphone2 = value; }
        }

        public string CellPhone3
        {
            get { return m_strCellphone3; }
            set { m_strCellphone3 = value; }
        }

        public string CircumMessage
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public string FAX1
        {
            get { return m_strFAX1; }
            set { m_strFAX1 = value; }
        }

        public string FAX2
        {
            get { return m_strFAX2; }
            set { m_strFAX2 = value; }
        }

        public string FAX3
        {
            get { return m_strFAX3; }
            set { m_strFAX3 = value; }
        }

        public string FAXFile
        {
            get { return m_strFAXFile; }
            set { m_strFAXFile = value; }
        }

        public string Broadcast
        {
            get { return m_strBroadcast; }
            set { m_strBroadcast = value; }
        }

        public SectionTimeText LinkedSection
        {
            get { return m_linkedSection; }
            set
            {
                m_linkedSection = value;
                if (m_linkedSection != null)
                {
                    if (m_linkedSection.ProcessData != this)
                        m_linkedSection.ProcessData = this;
                }
            }
        }
    }
}
