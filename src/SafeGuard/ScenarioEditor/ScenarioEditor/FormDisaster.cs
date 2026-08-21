using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScenarioEditor
{
    public partial class FormDisaster : Form
    {
        private List<Item> m_items = new List<Item>();
        private CheckBox m_checkBox = null;
        private string m_strComponentID = "";

        public string ComponentID
        {
            get { return m_strComponentID; }
            set { m_strComponentID = value; }
        }

        public List<string> Items
        {
            get
            {
                List<string> items = new List<string>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    items.Add(row.Cells[0].Value.ToString());
                }

                return items;
            }
        }

        public CheckBox CheckBox
        {
            get { return m_checkBox; }
            set
            {
                m_checkBox = value;
                m_checkBox.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            }
        }

        public FormDisaster()
        {
            InitializeComponent();
        }

        private void FormDisaster_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void SetGrid()
        {
            foreach (Item item in m_items)
            {
                if (item.ActionList != null)
                    AddRows(item.ActionList);
                else if (item.PatientItems != null)
                    AddRows(item.PatientItems);
                else
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();

                    cell.Value = item.ToString();

                    row.Cells.Add(cell);
                    dataGridView1.Rows.Add(row);
                }
            }
        }

        private void AddRows(List<string> items)
        {
            foreach (string strItem in items)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();

                cell.Value = strItem;

                row.Cells.Add(cell);
                dataGridView1.Rows.Add(row);
            }
        }

        public void Reload()
        {
            dataGridView1.Rows.Clear();
            SetGrid();
        }

        public void Reset(IVariable frm)
        {
            foreach (Item item in m_items)
            {
                item.SetItemText(Item.ItemType.MaterialName, frm.MaterialName);
                item.SetItemText(Item.ItemType.Place, frm.Place);
                item.SetItemText(Item.ItemType.Reason, frm.Reason);
                item.SetItemText(Item.ItemType.Weather, frm.Weather);
                item.SetItemText(Item.ItemType.Material, frm.Material);
                item.SetItemText(Item.ItemType.CountOfDeath, frm.CountOfDeath);
                item.SetItemText(Item.ItemType.CountOfBuilding, frm.CountOfBuilding);
                item.SetItemText(Item.ItemType.InitialDistance, frm.InitialDistance);
                item.SetItemText(Item.ItemType.Control, frm.Control);
                item.SetItemText(Item.ItemType.Distance, frm.Distance);
                item.SetItemText(Item.ItemType.MixedFactor, frm.MixedFactor);
                item.SetItemText(Item.ItemType.ActionList, frm.Actions);
                item.SetItemText(Item.ItemType.PatientItems, frm.PatientItems);
            }

            Reload();
        }

        public void AddItem(string strItem)
        {
            Item item = new Item(strItem);
            m_items.Add(item);
        }

        private void FormDisaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormMain.Instance.CloseApplication)
            {
                e.Cancel = true;
                m_checkBox.Checked = false;
                this.Hide();
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_checkBox.Checked)
                this.Show();
            else
                this.Hide();
        }
    }

    public class Item
    {
        public enum ItemType
        {
            MaterialName = 0,
            Place,
            Reason,
            Weather,
            Material,
            CountOfDeath,
            CountOfBuilding,
            InitialDistance,
            Control,
            Distance,
            MixedFactor,
            ActionList,
            PatientItems,
            STRING
        }

        private class Data
        {
            private ItemType m_type = ItemType.STRING;
            private string m_strData = "";

            public ItemType Type
            {
                get { return m_type; }
                set { m_type = value; }
            }

            public string Text
            {
                get { return m_strData; }
                set { m_strData = value; }
            }

            public Data()
            {
            }

            public Data(ItemType type, string strText)
            {
                m_type = type;
                m_strData = strText;
            }
        }

        private List<Data> m_datas = new List<Data>();
        private List<string> m_actionList = null;
        private List<string> m_patientItems = null;

        public List<string> ActionList
        {
            get { return m_actionList; }
        }

        public List<string> PatientItems
        {
            get { return m_patientItems; }
        }

        public Item(string strItem)
        {
            int len = strItem.Length;
            int nBeginIndex = 0;

            while (nBeginIndex < len)
            {
                Data data = null;
                int nIndex1 = strItem.IndexOf('(', nBeginIndex);

                if (nIndex1 < 0)
                {
                    string str = strItem.Substring(nBeginIndex);
                    data = new Data(ItemType.STRING, str);
                    m_datas.Add(data);
                    break;
                }

                int nIndex2 = strItem.IndexOf(')', nIndex1 + 1);

                if (nIndex2 < 0)
                {
                    string str = strItem.Substring(nBeginIndex);
                    data = new Data(ItemType.STRING, str);
                    m_datas.Add(data);
                    break;
                }
 
                string strData = strItem.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();

                if (strData == "물질명")
                    data = new Data(ItemType.MaterialName, "(" + strData + ")");
                else if (strData == "장소")
                    data = new Data(ItemType.Place, "(" + strData + ")");
                else if (strData == "사고 원인" || strData == "사고원인")
                    data = new Data(ItemType.Reason, "(" + strData + ")");
                else if (strData == "기상")
                    data = new Data(ItemType.Weather, "(" + strData + ")");
                else if (strData == "발생 물질" || strData == "발생물질")
                    data = new Data(ItemType.Material, "(" + strData + ")");
                else if (strData == "사상자 인원" || strData == "사상자인원")
                    data = new Data(ItemType.CountOfDeath, "(" + strData + ")");
                else if (strData == "건물 숫자" || strData == "건물숫자")
                    data = new Data(ItemType.CountOfBuilding, "(" + strData + ")");
                else if (strData == "초기 이격거리" || strData == "초기 이격 거리" || strData == "초기이격거리")
                    data = new Data(ItemType.InitialDistance, "(" + strData + ")");
                else if (strData == "지휘 체계" || strData == "지휘체계")
                    data = new Data(ItemType.Control, "(" + strData + ")");
                else if (strData == "대피 거리" || strData == "대피거리")
                    data = new Data(ItemType.Distance, "(" + strData + ")");
                else if (strData == "반응 물질" || strData == "반응물질")
                    data = new Data(ItemType.MixedFactor, "(" + strData + ")");
                else if (strData == "대응 내용" || strData == "대응내용")
                    data = new Data(ItemType.ActionList, "(" + strData + ")");
                else if (strData == "환자 응급조치" || strData == "환자 응급 조치" || strData == "환자응급조치")
                    data = new Data(ItemType.PatientItems, "(" + strData + ")");
                else
                {
                    string str = strItem.Substring(nBeginIndex);
                    data = new Data(ItemType.STRING, str);
                    m_datas.Add(data);
                    break;
                }

                if (nIndex1 > nBeginIndex)
                {
                    string strPrev = strItem.Substring(nBeginIndex, nIndex1 - nBeginIndex);
                    Data prev = new Data(ItemType.STRING, strPrev);
                    m_datas.Add(prev);
                }

                m_datas.Add(data);
                nBeginIndex = nIndex2 + 1;
            }
        }

        public void SetItemText(ItemType type, string strText)
        {
            foreach (Data data in m_datas)
            {
                if (data.Type == type)
                    data.Text = strText;
            }
        }

        public void SetItemText(ItemType type, List<string> textList)
        {
            foreach (Data data in m_datas)
            {
                if (data.Type == type)
                {
                    if (type == ItemType.ActionList)
                    {
                        m_patientItems = null;
                        m_actionList = new List<string>();

                        foreach (string strItem in textList)
                        {
                            m_actionList.Add(strItem);
                        }
                    }
                    else if (type == ItemType.PatientItems)
                    {
                        m_patientItems = new List<string>();
                        m_actionList = null;

                        foreach (string strItem in textList)
                        {
                            m_patientItems.Add(strItem);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            string strItem = "";

            foreach (Data data in m_datas)
            {
                strItem += data.Text;
            }

            return strItem;
        }
    }
}
