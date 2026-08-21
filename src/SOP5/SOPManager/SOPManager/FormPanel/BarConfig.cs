using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace SOPManager.FormPanel
{
    public partial class BarConfig : Form, Popup.IEditItemOwner
    {
        private const string FOLDER_NAME = "UserDefined";
        private const string FILE_PATTERN = ".ini";

        private const int NOT_USE_INDEX = 0;
        private const int FIRST_FILE_INDEX = 1;

        private int m_nPrevSelectedIndex = -1;
        private bool m_systemCall = false;
        private bool m_useDB = true;

        public BarConfig()
        {
            InitializeComponent();            
        }

        public void event_WinRateChanged()
        {
            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            //Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            //double WindowRateWidth = dWindowRate[0];
            //double WindowRateHeight = dWindowRate[1];
            double WindowRateWidth = FormMain.Instance.WindowWidthRate;
            double WindowRateHeight = FormMain.Instance.WindowHeightRate;

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            FormMain.Instance.UpdateWindowRate(panelTop, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label2, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(cboConfig, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picEdit, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblEdit, WindowRateWidth, WindowRateHeight);
            
            FormMain.Instance.UpdateWindowRate(dataGridView, WindowRateWidth, WindowRateHeight);
            dataGridView.Size = new Size(this.Size.Width, dataGridView.Height);
        }

        private void BarConfig_Load(object sender, EventArgs e)
        {
            BindingComboBox();

            dataGridView.Columns[1].DefaultCellStyle.NullValue = "정수";
        }

        private ConfigData ReadConfig(string strFilePath)
        {
            int nIndex1 = strFilePath.LastIndexOf('\\');
            string strFileName = nIndex1 >= 0 ? strFilePath.Substring(nIndex1 + 1) : strFilePath;

            int nIndex2 = strFileName.LastIndexOf('.');

            if (nIndex2 >= 0)
                strFileName = strFileName.Substring(0, nIndex2);

            ConfigData data = new ConfigData(ConfigData.ConfigType.FILE, strFileName);

            StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.Default);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                int nIndex = strLine.IndexOf('#');

                if (nIndex == 0)
                    continue;

                if (nIndex > 0)
                    strLine = strLine.Substring(0, nIndex).Trim();

                if (strLine.Length == 0)
                    continue;

                char first = strLine.ElementAt(0);

                if (first == '*')
                {
                    ReadDescription(data, strLine);
                }
                else if (first == '{')
                {
                    ReadVariable(data, strLine);
                }
            }

            reader.Close();

            return data;
        }

        private void ReadVariable(ConfigData data, string strLine)
        {
            int nIndex = strLine.IndexOf('}');

            if (nIndex < 0)
                return;

            string strVariableName = strLine.Substring(1, nIndex - 1).Trim();

            if (strVariableName.Length == 0)
                return;

            // 변수 중간에 빈칸이 있으면 안된다.
            if (strVariableName.IndexOf(' ') >= 0)
                return;

            int nIndex2 = strLine.IndexOf(',', nIndex + 1);

            if (nIndex2 < 0)
                return;

            int nIndex3 = strLine.IndexOf(',', nIndex2 + 1);

            string strVariableType = strLine.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1).Trim();
            string strDescription = strLine.Substring(nIndex3 + 1).Trim();

            Sections.SectionDataDecision.VariableType type = Sections.SectionDataDecision.ToVariableType(strVariableType);

            if (type == Sections.SectionDataDecision.VariableType.UNKNOWN)
                return;

            SOPParameter param = new SOPParameter();
            param.VariableName = strVariableName;
            param.Type = type;
            param.Description = strDescription;

            data.Variables.Add(param);
        }

        private void ReadDescription(ConfigData data, string strLine)
        {
            string strDescription = strLine.Substring(1);

            if (data.Description.Length == 0)
                data.Description = strDescription;
            else
                data.Description += "\r\n" + strDescription;
        }

        private List<ConfigData> GetConfigDB()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Select ID, ConfigName, Description from UserDefinedConfig";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            List<ConfigData> datas = new List<ConfigData>();

            if (arrResult == null)
                return datas;

            int nResultCount = arrResult.Count;

            Dictionary<int, ConfigData> dicConfigs = new Dictionary<int, ConfigData>();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                DBUtility.VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strConfigName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 2]);

                if (id == null || strConfigName == null)
                    continue;

                ConfigData data = new ConfigData(ConfigData.ConfigType.FILE);
                data.Text = strConfigName;
                data.ID = id.Data;

                if (strDescription != null)
                    data.Description = strDescription;

                datas.Add(data);
                dicConfigs[id.Data] = data;
            }

            if (datas.Count == 0)
                return datas;

            strSQL = "Select ConfigID, No, VariableName, VariableType, Description from UserDefinedConfigVariable";
            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return datas;

            nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                DBUtility.VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                DBUtility.VariousData<int> no = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strVariableName = WebDBManager.GetStringField(arrResult[i + 2]);
                DBUtility.VariousData<int> type = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strVariableDescription = WebDBManager.GetStringField(arrResult[i + 4]);

                if (id == null || no == null || strVariableName == null || type == null || strVariableDescription == null)
                    continue;

                ConfigData data = null;

                if (dicConfigs.TryGetValue(id.Data, out data) == false)
                    continue;

                SOPParameter param = new SOPParameter();
                param.VariableName = strVariableName;
                param.Type = Sections.SectionDataDecision.ToVariableType(type.Data);
                param.Description = strVariableDescription;
                param.No = no.Data;

                data.Variables.Add(param);
            }

            return datas;

            /*string strSQL = "Select config.ID, config.ConfigName, config.Description, variable.No, variable.VariableName, variable.VariableType, variable.Description ";
            strSQL += "from UserDefinedConfig as config, UserDefinedConfigVariable as variable ";
            strSQL += "where config.ID = variable.ConfigID order by config.ID, variable.No";

            List<ConfigData> datas = new List<ConfigData>();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return datas;

            int nResultCount = arrResult.Count;

            Dictionary<int, ConfigData> dicConfigs = new Dictionary<int, ConfigData>();
            ConfigData data = null;

            for (int i=0;i<nResultCount-6;i+=7)
            {
                DBUtility.VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strConfigName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 2]);
                DBUtility.VariousData<int> no = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strVariableName = WebDBManager.GetStringField(arrResult[i + 4]);
                DBUtility.VariousData<int> type = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strVariableDescription = WebDBManager.GetStringField(arrResult[i + 6]);

                if (id == null || strConfigName == null || no == null || strVariableName == null || type == null || strVariableDescription == null)
                    continue;

                if (dicConfigs.TryGetValue(id.Data, out data) == false)
                {
                    data = new ConfigData(ConfigData.ConfigType.FILE);
                    dicConfigs[id.Data] = data;

                    data.Text = strConfigName;

                    if (strDescription != null)
                        data.Description = strDescription;
                }

                SOPParameter param = new SOPParameter();
                param.VariableName = strVariableName;
                param.Type = Sections.SectionDataDecision.ToVariableType(type.Data);
                param.Description = strVariableDescription;

                data.Variables.Add(param);
            }

            return dicConfigs.Values.ToList();*/
        }

        private string[] GetConfigFiles()
        {
            string[] files = null;

            string strPath = Application.ExecutablePath;
            string strFolderPath = System.IO.Path.GetDirectoryName(strPath) + "\\" + FOLDER_NAME;

            if (Directory.Exists(strFolderPath) == false)
                return files;

            files = Directory.GetFiles(strFolderPath, "*" + FILE_PATTERN);
            return files;
        }

        private void cboConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboConfig.SelectedIndex < 0)
                return;

            if (m_systemCall)
                return;

            ConfigData data = (ConfigData)cboConfig.Items[cboConfig.SelectedIndex];

            if (data.Type == ConfigData.ConfigType.FILE)
            {
                SetGrid(data);
                m_nPrevSelectedIndex = cboConfig.SelectedIndex;
                checkBoxEdit.Enabled = dataGridView.Enabled = lblEdit.Enabled = picEdit.Enabled = true;
            }
            else if (data.Type == ConfigData.ConfigType.NEW)
            {
                // New Form
                if (m_useDB)
                    NewConfigDB();
                else
                    NewConfigFile();

                m_systemCall = true;
                cboConfig.SelectedIndex = m_nPrevSelectedIndex;
                m_systemCall = false;

                checkBoxEdit.Enabled = dataGridView.Enabled = lblEdit.Enabled = picEdit.Enabled = true;
            }
            else if (data.Type == ConfigData.ConfigType.EDIT)
            {
                // Edit Form
                EditConfigFile();

                m_systemCall = true;
                cboConfig.SelectedIndex = m_nPrevSelectedIndex;
                m_systemCall = false;

                checkBoxEdit.Enabled = dataGridView.Enabled = lblEdit.Enabled = picEdit.Enabled = true;
            }
            else if (data.Type == ConfigData.ConfigType.NOT_USE)
            {
                SetGrid(null);
                checkBoxEdit.Enabled = dataGridView.Enabled = lblEdit.Enabled = picEdit.Enabled = false;                
                m_nPrevSelectedIndex = 0;

                if (checkBoxEdit.Checked)
                    checkBoxEdit.Checked = false;
            }
        }

        private void EditConfigFile()
        {
            Popup.FormEditUserDefinedParameterConfig frm = new Popup.FormEditUserDefinedParameterConfig("설정 편집", GetConfigNames(), this);
            UnE.GUI.DialogFormFrameRibbon editUserDefined = new UnE.GUI.DialogFormFrameRibbon(frm);

            editUserDefined.ShowIcon = false;
            editUserDefined.StartPosition = FormStartPosition.CenterScreen;
            editUserDefined.Text = "설정 편집";
            editUserDefined.TitleTextFont = new Font("나눔스퀘어", 12.5f, FontStyle.Bold);
            editUserDefined.ForeColor = Color.Black;
            editUserDefined.ShowMaxButton = false;
            editUserDefined.ShowMinButton = false;
            editUserDefined.Sizable = false;
            editUserDefined.TopMost = true;
            editUserDefined.ShowDialog(this);    
        }

        private void NewConfigDB()
        {
            Popup.FormNewUserDefinedParameterConfig frm = new Popup.FormNewUserDefinedParameterConfig(GetConfigNames(), this);
            UnE.GUI.DialogFormFrameRibbon editUserDefined = new UnE.GUI.DialogFormFrameRibbon(frm);

            editUserDefined.ShowIcon = false;
            editUserDefined.StartPosition = FormStartPosition.CenterScreen;
            editUserDefined.Text = "새 설정구성";
            editUserDefined.TitleTextFont = new Font("나눔스퀘어", 12.5f, FontStyle.Bold);
            editUserDefined.ForeColor = Color.Black;
            editUserDefined.ShowMaxButton = false;
            editUserDefined.ShowMinButton = false;
            editUserDefined.TopMost = true;
            editUserDefined.Sizable = false;
            
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            
            if (editUserDefined.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)            
            {
                if (frm.CopyFrom != null)
                {
                    ConfigData data = GetConfig(frm.CopyFrom);

                    if (data == null)
                        return;

                    try
                    {
                        if (data.ID > 0)
                        {
                            ConfigData newData = data.Clone(frm.NewConfigName);
                            cboConfig.Items.Insert(FIRST_FILE_INDEX, newData);

                            SetGrid(newData);
                            m_nPrevSelectedIndex = FIRST_FILE_INDEX;

                            string strDescription = data.Description == null ? "NULL" : "'" + data.Description + "'";
                            int nID = FormMain.Instance.GetMaxTableID("UserDefinedConfig", 0) + 1;

                            string strSQL = string.Format("Insert into UserDefinedConfig (ID, ConfigName, Description) values ({0}, '{1}', {2})",
                                nID, frm.NewConfigName, strDescription);

                            if (dbMgr.GetResultData(strSQL, 0) == null)
                                return;

                            newData.ID = nID;
                            SetConfigVariables(dbMgr, nID, newData.Variables);
                        }
                        /*string strSQL = "Select ID, ConfigName from UserDefinedConfig where lower(ConfigName) = '" + data.Text.ToLower() + "'";
                        ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                        string strDescription = data.Description == null ? "NULL" : "'" + data.Description + "'";

                        if (arrResult != null && arrResult.Count >= 2)
                        {
                            DBUtility.VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                            if (id == null)
                                return;

                            strSQL = "Update UserDefinedConfig set ConfigName = '" + frm.NewConfigName + "', Description = " + strDescription + " where ID = " + id.Data.ToString();

                            if (dbMgr.GetResultData(strSQL, 0) == null)
                                return;

                            data.ID = id.Data;
                            SetConfigVariables(dbMgr, id.Data, data.Variables);
                        }
                        else
                        {
                            int nID = FormMain.Instance.GetMaxTableID("UserDefinedConfig", 0) + 1;

                            strSQL = string.Format("Insert into UserDefinedConfig (ID, ConfigName, Description) values ({0}, '{1}', {2})",
                                nID, frm.NewConfigName, strDescription);

                            if (dbMgr.GetResultData(strSQL, 0) == null)
                                return;

                            data.ID = nID;
                            SetConfigVariables(dbMgr, nID, data.Variables);
                        }

                        ConfigData newData = data.Clone(frm.NewConfigName);
                        cboConfig.Items.Insert(FIRST_FILE_INDEX, newData);

                        SetGrid(newData);
                        m_nPrevSelectedIndex = FIRST_FILE_INDEX;*/
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }
                }
                else
                {
                    dataGridView.Rows.Clear();

                    try
                    {
                        int nID = FormMain.Instance.GetMaxTableID("UserDefinedConfig", 0) + 1;

                        string strSQL = string.Format("Insert into UserDefinedConfig (ID, ConfigName, Description) values ({0}, '{1}', NULL)",
                            nID, frm.NewConfigName);

                        if (dbMgr.GetResultData(strSQL, 0) == null)
                            return;

                        ConfigData newData = new ConfigData(ConfigData.ConfigType.FILE, frm.NewConfigName);
                        newData.ID = nID;
                        cboConfig.Items.Insert(FIRST_FILE_INDEX, newData);

                        SetGrid(newData);
                        m_nPrevSelectedIndex = FIRST_FILE_INDEX;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }
                }
            }
        }

        private bool SetConfigVariables(WebDBManager dbMgr, int nConfigID, List<SOPParameter> parameters, bool batchCommit = true)
        {
            if (batchCommit)
                dbMgr.BeginBatch();

            string strSQL = "Delete from UserDefinedConfigVariable where ConfigID = " + nConfigID.ToString();
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            Dictionary<SOPParameter, int> dicParamIDs = new Dictionary<SOPParameter, int>();

            for (int i=0;i<parameters.Count;i++)
            {
                SOPParameter param = parameters[i];
                string strDescription = param.Description == null ? "" : param.Description;

                strSQL = string.Format("Insert into UserDefinedConfigVariable (ConfigID, No, VariableName, VariableType, Description) values ({0}, {1}, '{2}', {3}, '{4}')",
                    nConfigID, i + FIRST_FILE_INDEX, param.VariableName, (int)param.Type, strDescription);

                if (dbMgr.GetBatchData(strSQL) == null)
                {
                    dbMgr.BatchRollback();
                    return false;
                }

                dicParamIDs[param] = i + FIRST_FILE_INDEX;
            }

            if (batchCommit)
                dbMgr.BatchCommit();

            foreach (KeyValuePair<SOPParameter, int> pair in dicParamIDs)
            {
                pair.Key.No = pair.Value;
            }

            return true;
        }

        private void NewConfigFile()
        {
            Popup.FormNewUserDefinedParameterConfig frm = new Popup.FormNewUserDefinedParameterConfig(GetConfigNames(), this);
            UnE.GUI.DialogFormFrameRibbon mEditUserDefined = new UnE.GUI.DialogFormFrameRibbon(frm);

            mEditUserDefined.ShowIcon = false;
            mEditUserDefined.StartPosition = FormStartPosition.CenterScreen;
            mEditUserDefined.Text = "새 설정구성";
            mEditUserDefined.ForeColor = Color.Black;
            mEditUserDefined.ShowMaxButton = false;
            mEditUserDefined.ShowMinButton = false;
            mEditUserDefined.Sizable = false;                        

            string strPath = Application.ExecutablePath;
            string strFolderPath = System.IO.Path.GetDirectoryName(strPath) + "\\" + FOLDER_NAME;
            
            if (mEditUserDefined.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (frm.CopyFrom != null)
                {
                    ConfigData data = GetConfig(frm.CopyFrom);

                    if (data == null)
                        return;

                    try
                    {
                        File.Copy(strFolderPath + "\\" + data.ToString() + FILE_PATTERN, strFolderPath + "\\" + frm.NewConfigName + FILE_PATTERN);
                        ConfigData newData = data.Clone(frm.NewConfigName);
                        cboConfig.Items.Insert(FIRST_FILE_INDEX, newData);

                        SetGrid(newData);
                        m_nPrevSelectedIndex = FIRST_FILE_INDEX;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }
                }
                else
                {
                    try
                    {
                        StreamWriter writer = new StreamWriter(strFolderPath + "\\" + frm.NewConfigName + FILE_PATTERN, false, Encoding.Default);
                        writer.Close();

                        ConfigData newData = new ConfigData(ConfigData.ConfigType.FILE, frm.NewConfigName);
                        cboConfig.Items.Insert(FIRST_FILE_INDEX, newData);

                        SetGrid(newData);
                        m_nPrevSelectedIndex = FIRST_FILE_INDEX;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }
                }
            }
        }

        private ConfigData GetConfig(string strConfigName)
        {
            foreach (ConfigData data in cboConfig.Items)
            {
                if (data.ToString() == strConfigName)
                    return data;
            }

            return null;
        }

        private List<string> GetConfigNames()
        {
            List<string> configNames = new List<string>();

            foreach (ConfigData data in cboConfig.Items)
            {
                if (data.Type == ConfigData.ConfigType.FILE)
                    configNames.Add(data.ToString());
            }

            return configNames;
        }

        private void SetGrid(ConfigData data)
        {
            CheckChangedData();
            ConfigToGrid(data);

            if (data != null)
                Popup.PopupNoteDecision2.ChangeUserDefinedVariables(data.Variables);
            else
                Popup.PopupNoteDecision2.ChangeUserDefinedVariables(null);
        }

        public void SetConfig(ConfigData data)
        {
            if (data == null)
            {
                if (cboConfig.SelectedIndex != NOT_USE_INDEX)
                    cboConfig.SelectedIndex = NOT_USE_INDEX;
            }
            else
            {                
                int nLastFileIndex = 0;

                for (int i = FIRST_FILE_INDEX; i < cboConfig.Items.Count; i++)
                {
                    ConfigData config = (ConfigData)cboConfig.Items[i];

                    if (config.Type == ConfigData.ConfigType.FILE)
                    {
                        nLastFileIndex = i;

                        if (string.Compare(config.Text, data.Text, true) == 0)
                        {
                            config.Text = data.Text;
                            config.Variables.Clear();
                            config.Variables.AddRange(data.Variables);

                            if (cboConfig.SelectedIndex != i)
                                cboConfig.SelectedIndex = i;
                            else
                                SetGrid(config);

                            return;
                        }
                    }
                }

                cboConfig.Items.Insert(nLastFileIndex + 1, data);
                cboConfig.SelectedIndex = nLastFileIndex + 1;
            }
        }

        public void CheckChangedData()
        {
            /*ConfigData currentData = m_nPrevSelectedIndex >= FIRST_FILE_INDEX ? (ConfigData)cboConfig.Items[m_nPrevSelectedIndex] : null;

            if (currentData == null)
                return;

            int nOriginVariableCount = currentData.Variables.Count;
            int nGridVariableCount = dataGridView.Rows.Count;

            if (nGridVariableCount > 0)
            {
                int nLastRowIndex = -1;

                for (int i = nGridVariableCount - 1; i >= 0; i--)
                {
                    DataGridViewRow row = dataGridView.Rows[i];

                    if (row.IsNewRow)
                        continue;
                    else
                    {
                        nLastRowIndex = i;
                        break;
                    }
                }

                nGridVariableCount = nLastRowIndex + 1;
            }

            if (nOriginVariableCount != nGridVariableCount)
                GridToConfig(currentData);
            else
            {
                for (int i=0;i<nOriginVariableCount;i++)
                {
                    SOPParameter param = currentData.Variables[i];
                    
                    if (IsSame(param, dataGridView.Rows[i]) == false)
                    {
                        GridToConfig(currentData);
                        break;
                    }
                }
            }*/
        }

        private bool IsSame(SOPParameter param, DataGridViewRow row)
        {
            string strVariableName = row.Cells[0].Value == null ? "" : row.Cells[0].Value.ToString().Trim();
            string strVariableType = row.Cells[1].Value == null ? "" : row.Cells[1].Value.ToString();
            string strDescription = row.Cells[2].Value == null ? "" : row.Cells[2].Value.ToString().Trim();

            if (strVariableName.Length == 0 || strVariableType.Length == 0)
                return false;

            if (strVariableName.Contains(' '))
                return false;

            if (strVariableName.Contains('{') || strVariableName.Contains('}'))
                return false;

            if (param.VariableName != strVariableName)
                return false;

            if (param.Type != Sections.SectionDataDecision.ToVariableType(strVariableType))
                return false;

            if (param.Description != strDescription)
                return false;

            return true;
        }

        private void ConfigToGrid(ConfigData data)
        {
            m_systemCall = true;
            dataGridView.Rows.Clear();
            dataGridView.AllowUserToAddRows = false;

            if (data != null)
            {
                foreach (SOPParameter param in data.Variables)
                {
                    int nRowIndex = dataGridView.Rows.Add();
                    DataGridViewRow row = dataGridView.Rows[nRowIndex];

                    row.Cells[0].Value = param.VariableName;
                    row.Cells[1].Value = Sections.SectionDataDecision.GetVariableTypeName(param.Type);
                    row.Cells[2].Value = param.Description;
                    row.Tag = param;
                }
            }

            dataGridView.AllowUserToAddRows = checkBoxEdit.Checked;
            m_systemCall = false;
        }

        /*private void GridToConfig(ConfigData data)
        {
            data.Variables.Clear();
            data.IsChanged = true;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string strVariableName = row.Cells[0].Value == null ? "" : row.Cells[0].Value.ToString().Trim();
                string strVariableType = row.Cells[1].Value == null ? "정수" : row.Cells[1].Value.ToString();
                string strDescription = row.Cells[2].Value == null ? "" : row.Cells[2].Value.ToString().Trim();

                if (strVariableName.Length == 0 || strVariableType.Length == 0)
                    continue;

                if (strVariableName.Contains(' '))
                    continue;

                if (strVariableName.Contains('{') || strVariableName.Contains('}'))
                    continue;
                
                SOPParameter param = new SOPParameter();
                param.VariableName = strVariableName;
                param.Type = Sections.SectionDataDecision.ToVariableType(strVariableType);
                param.Description = strDescription;

                data.Variables.Add(param);
                row.Tag = param;
            }

            if (m_useDB)
            {
                WebDBManager dbMgr = FormMain.Instance.DBManager;

                dbMgr.BeginBatch();

                string strSQL = "Select ID from UserDefinedConfig where lower(ConfigName) = '" + data.Text + "'";
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (strSQL == null)
                {
                    dbMgr.BatchRollback();
                    return;
                }

                if (arrResult.Count == 0)
                {
                    int nID = FormMain.Instance.GetMaxTableID("UserDefinedConfig", 1) + 1;

                    strSQL = string.Format("Insert into UserDefinedConfig (ID, ConfigName, Description) values ({0}, '{1}', NULL)", nID, data.Text);
                    
                    if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        dbMgr.BatchRollback();
                        return;
                    }
                    else
                        data.ID = nID;

                    if (SetConfigVariables(dbMgr, nID, data.Variables, false) == false)
                        return;
                }
                else
                {
                    foreach (object result in arrResult)
                    {
                        DBUtility.VariousData<int> id = WebDBManager.GetIntField(result.ToString());

                        if (id == null)
                        {
                            dbMgr.BatchRollback();
                            return;
                        }

                        if (SetConfigVariables(dbMgr, id.Data, data.Variables, false) == false)
                            return;
                    }
                }

                dbMgr.BatchCommit();
            }
            else
            {
                try
                {
                    StreamWriter writer = new StreamWriter(data.Text, false, Encoding.Default);

                    if (data.Description != null && data.Description.Length > 0)
                        writer.WriteLine("* " + data.Description);

                    foreach (SOPParameter param in data.Variables)
                    {
                        string strDescription = param.Description == null ? "" : param.Description;
                        writer.WriteLine("{" + param.VariableName + "}, " + Sections.SectionDataDecision.GetVariableTypeName(param.Type, false) + ", " + strDescription);
                    }

                    writer.Close();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }
            }
        }*/

        public bool OnRemoveItem(string strItemName)
        {
            ConfigData data = GetConfig(strItemName);

            if (m_useDB)
            {
                WebDBManager dbMgr = FormMain.Instance.DBManager;

                dbMgr.BeginBatch();

                string strSQL = "Select ID from UserDefinedConfig where lower(ConfigName) = '" + strItemName.ToLower() + "'";
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.BatchRollback();
                    return false;
                }

                foreach (object result in arrResult)
                {
                    DBUtility.VariousData<int> id = WebDBManager.GetIntField(result.ToString());

                    if (id == null)
                        continue;

                    strSQL = "Update ActionStep set UserDefinedConfigID = NULL where UserDefinedConfigID = " + id.Data.ToString();

                    if (dbMgr.GetBatchData(strSQL, 0) == null)
                    {
                        dbMgr.BatchRollback();
                        return false;
                    }

                    strSQL = "Delete from UserDefinedConfigVariable where ConfigID = " + id.Data.ToString();

                    if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        dbMgr.BatchRollback();
                        return false;
                    }

                    strSQL = "Delete from UserDefinedConfig where ID = " + id.Data.ToString();

                    if (dbMgr.GetBatchData(strSQL, 0) == null)
                    {
                        dbMgr.BatchRollback();
                        return false;
                    }
                }

                dbMgr.BatchCommit();

                cboConfig.Items.Remove(data);

                if (cboConfig.SelectedIndex < 0)
                {
                    if (cboConfig.Items.Count > 0)
                        cboConfig.SelectedIndex = NOT_USE_INDEX;
                }
                else
                {
                    ConfigData dataSelected = (ConfigData)cboConfig.Items[cboConfig.SelectedIndex];

                    if (dataSelected.Type != ConfigData.ConfigType.FILE)
                        cboConfig.SelectedIndex = NOT_USE_INDEX;
                }
            }
            else
            {
                string strPath = Application.ExecutablePath;
                string strFolderPath = System.IO.Path.GetDirectoryName(strPath) + "\\" + FOLDER_NAME;
                string strFilePath = strFolderPath + "\\" + data.ToString() + FILE_PATTERN;

                try
                {
                    File.Delete(strFilePath);
                    cboConfig.Items.Remove(data);

                    if (cboConfig.SelectedIndex < 0)
                    {
                        if (cboConfig.Items.Count > 0)
                            cboConfig.SelectedIndex = NOT_USE_INDEX;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    return false;
                }
            }

            return true;
        }

        public bool OnRenameItem(string strOriginItemName, string strNewItemName)
        {
            ConfigData data = GetConfig(strOriginItemName);
            
            foreach (ConfigData config in cboConfig.Items)
            {
                if (config == data)
                    continue;

                // 이미 존재하는 이름은 사용할 수 없다.
                if (string.Compare(config.Text, strNewItemName, true) == 0)
                    return false;
            }

            if (m_useDB)
            {
                WebDBManager dbMgr = FormMain.Instance.DBManager;

                string strSQL = "Select ID from UserDefinedConfig where lower(ConfigName) = '" + strOriginItemName.ToLower() + "'";
                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count == 0)
                    return false;

                DBUtility.VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return false;

                strSQL = string.Format("Update UserDefinedConfig set ConfigName = '{0}' where ID = {1}", strNewItemName, id.Data);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return false;

                data.ID = id.Data;

                foreach (object item in cboConfig.Items)
                {
                    ConfigData tempCfgData = item as ConfigData;
                    if (item != null && data == tempCfgData)
                    {
                        tempCfgData.Text = strNewItemName;
                        // combobox reload
                        BindingComboBox();
                        break;
                    }
                }
            }
            else
            {
                string strPath = Application.ExecutablePath;
                string strFolderPath = System.IO.Path.GetDirectoryName(strPath) + "\\" + FOLDER_NAME;
                string strFilePath = strFolderPath + "\\" + data.ToString() + FILE_PATTERN;
                string strTargetPath = strFolderPath + "\\" + strNewItemName + FILE_PATTERN;

                try
                {
                    File.Move(strFilePath, strTargetPath);
                    data.Text = strNewItemName;

                    if (cboConfig.Items[cboConfig.SelectedIndex] == data)
                        cboConfig.Refresh();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    return false;
                }
            }

            return true;
        }

        public bool IsValidName(string strItemName)
        {
            foreach (ConfigData config in cboConfig.Items)
            {
                if (config.Type == ConfigData.ConfigType.FILE)
                    continue;

                // 이미 존재하는 이름은 사용할 수 없다.
                if (string.Compare(config.Text, strItemName, true) == 0)
                    return false;
            }

            return true;
        }

        private void CheckBox_Click(object sender, EventArgs e)
        {
            checkBoxEdit.Checked = !checkBoxEdit.Checked;
        }

        private void checkBoxEdit_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEdit.Checked)
            {
                picEdit.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_enable;
                dataGridView.AllowUserToAddRows = true;

                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.ReadOnly = false;
                }

                DataGridViewComboBoxColumn comboColumn = (DataGridViewComboBoxColumn)dataGridView.Columns[1];
                comboColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            }
            else
            {
                //CheckChangedData();
                ConfigData data = (ConfigData)cboConfig.Items[cboConfig.SelectedIndex];

                if (data.Type == ConfigData.ConfigType.FILE)
                {
                    SetGrid(data);
                    if (data != null)
                        Popup.PopupNoteDecision2.ChangeUserDefinedVariables(data.Variables);
                    else
                        Popup.PopupNoteDecision2.ChangeUserDefinedVariables(null);
                }

                picEdit.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_disable;
                dataGridView.AllowUserToAddRows = false;

                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.ReadOnly = true;
                }

                DataGridViewComboBoxColumn comboColumn = (DataGridViewComboBoxColumn)dataGridView.Columns[1];
                comboColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            }
        }

        public List<SOPParameter> GetCurrentVariables(out ConfigData data)
        //public List<SOPParameter> GetCurrentVariables(out string strConfigName)
        {
            data = null;
            //strConfigName = "";

            if (cboConfig.SelectedIndex < 0 || cboConfig.Items.Count <= 0)
                return null;

            /*ConfigData */data = (ConfigData)cboConfig.Items[cboConfig.SelectedIndex];

            if (data.Type == ConfigData.ConfigType.FILE)
            {
                List<SOPParameter> parameters = new List<SOPParameter>();

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    if (row.Cells[0].Value == null)
                        continue;

                    if (row.Cells[1].Value == null)
                    {
                        if (row.Cells[1].FormattedValue != null)
                            row.Cells[1].Value = row.Cells[1].FormattedValue.ToString();
                        else
                            continue;
                    }

                    string strVariableName = row.Cells[0].Value.ToString().Trim();
                    Sections.SectionDataDecision.VariableType type = Sections.SectionDataDecision.ToVariableType(row.Cells[1].Value.ToString());
                    string strDescription = row.Cells[2].Value == null ? "" : row.Cells[2].Value.ToString().Trim();

                    if (type == Sections.SectionDataDecision.VariableType.UNKNOWN)
                        continue;

                    SOPParameter param = new SOPParameter();

                    param.VariableName = strVariableName;
                    param.Type = type;
                    param.Description = strDescription;

                    parameters.Add(param);
                }

                //strConfigName = data.Text;
                return parameters;
            }

            data = null;
            return null;
        }

        private void tsMenuDelete_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.Tag != null && contextMenuStrip1.Tag is int)
            {
                int nRowIndex = (int)contextMenuStrip1.Tag;
                dataGridView.Rows.RemoveAt(nRowIndex);
                contextMenuStrip1.Tag = null;
            }
        }

        private void dataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            if (dataGridView.AllowUserToAddRows == false)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitInfo = dataGridView.HitTest(e.X, e.Y);

                if (hitInfo.RowIndex >= 0 && hitInfo.ColumnIndex >= 0)
                {
                    DataGridViewRow row = dataGridView.Rows[hitInfo.RowIndex];

                    if (row.IsNewRow)
                        return;

                    row.Cells[hitInfo.ColumnIndex].Selected = true;

                    contextMenuStrip1.Tag = hitInfo.RowIndex;
                    contextMenuStrip1.Show(dataGridView, e.Location);
                }
            }
        }

        private void BindingComboBox()
        {
            cboConfig.Items.Clear();
            cboConfig.Items.Add(new ConfigData(ConfigData.ConfigType.NOT_USE));

            if (m_useDB)
            {
                List<ConfigData> configs = GetConfigDB();

                foreach (ConfigData data in configs)
                {
                    cboConfig.Items.Add(data);
                }
            }
            else
            {
                string[] files = GetConfigFiles();

                if (files != null)
                {
                    foreach (string strFilePath in files)
                    {
                        ConfigData data = ReadConfig(strFilePath);

                        if (data != null)
                            cboConfig.Items.Add(data);
                    }
                }
            }

            cboConfig.Items.Add(new ConfigData(ConfigData.ConfigType.NEW));
            cboConfig.Items.Add(new ConfigData(ConfigData.ConfigType.EDIT));

            if (cboConfig.Items.Count > 2)
                cboConfig.SelectedIndex = NOT_USE_INDEX;
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (m_systemCall || checkBoxEdit.Checked == false || cboConfig.SelectedIndex < 0)
                return;

            ConfigData data = (ConfigData)cboConfig.Items[cboConfig.SelectedIndex];

            if (data.Type == ConfigData.ConfigType.FILE)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                SOPParameter param = (SOPParameter)row.Tag;
                object rollbackData = null;

                if (param == null)
                {
                    param = new SOPParameter();

                    string strType = "";

                    if (row.Cells[1].Value != null)
                        strType = row.Cells[1].Value.ToString();
                    else if (row.Cells[1].FormattedValue != null)
                        strType = row.Cells[1].FormattedValue.ToString();

                    param.Type = Sections.SectionDataDecision.ToVariableType(strType);
                    data.Variables.Add(param);
                    row.Tag = param;
                }

                if (e.ColumnIndex == 0)
                {
                    rollbackData = param.VariableName;
                    DataGridViewCell cell = row.Cells[e.ColumnIndex];

                    if (cell == null)
                        return;

                    if (cell.Value == null)
                        cell.Value = "";

                    string strValue = cell.Value.ToString().Trim();

                    if (strValue.Length == 0)
                    {
                        MessageBox.Show("변수이름을 비워둘 수 없습니다.");
                        cell.Value = param.VariableName;
                        return;
                    }

                    for (int i = 0; i < dataGridView.Rows.Count; i++)
                    {
                        if (i == e.RowIndex)
                            continue;

                        DataGridViewRow _row = dataGridView.Rows[i];

                        if (_row.IsNewRow || _row.Cells[0].Value == null)
                            continue;

                        if (_row.Cells[0].Value.ToString().Trim() == strValue)
                        {
                            MessageBox.Show("이미 같은 이름의 변수가 존재합니다.");
                            cell.Value = param.VariableName;
                            return;
                        }
                    }

                    param.VariableName = strValue;
                }
                else if (e.ColumnIndex == 1)
                {
                    rollbackData = param.Type;
                    param.Type = Sections.SectionDataDecision.ToVariableType(row.Cells[1].Value.ToString());
                }
                else if (e.ColumnIndex == 2)
                {
                    rollbackData = param.Description;
                    DataGridViewCell cell = row.Cells[e.ColumnIndex];

                    if (cell == null)
                        return;

                    if (cell.Value == null)
                        cell.Value = "";

                    param.Description = cell.Value.ToString().Trim();
                }
                else
                    return;

                if (param.No < 0)
                {
                    if (AddConfigVariable(data, param) == false)
                    {
                        dataGridView.Rows.Remove(row);
                        data.Variables.Remove(param);
                    }
                }
                else
                {
                    if (UpdateConfigVariable(data, param) == false)
                    {
                        if (e.ColumnIndex == 0)
                        {
                            if (rollbackData != null)
                                param.VariableName = (string)rollbackData;
                            else
                                param.VariableName = "";

                            row.Cells[e.ColumnIndex].Value = param.VariableName;
                        }
                        else if (e.ColumnIndex == 1)
                        {
                            if (rollbackData != null)
                                param.Type = (Sections.SectionDataDecision.VariableType)rollbackData;
                            else
                                param.Type = Sections.SectionDataDecision.VariableType.UNKNOWN;

                            row.Cells[1].Value = Sections.SectionDataDecision.GetVariableTypeName(param.Type);
                        }
                        else if (e.ColumnIndex == 2)
                        {
                            if (rollbackData != null)
                                param.Description = (string)rollbackData;
                            else
                                param.Description = "";

                            row.Cells[e.ColumnIndex].Value = param.Description;
                        }
                    }
                }
            }
        }

        private bool UpdateConfigVariable(ConfigData data, SOPParameter param)
        {
            string strVariableName = param.VariableName.Trim();

            if (strVariableName.Length == 0)
                return false;

            string strSQL = string.Format("Update UserDefinedConfigVariable set VariableName = '{0}', VariableType = {1}, Description = '{2}' where ConfigID = {3} and No = {4}",
                strVariableName, (int)param.Type, param.Description, data.ID, param.No);

            return FormMain.Instance.DBManager.GetResultData(strSQL, 0) != null;
        }

        private bool AddConfigVariable(ConfigData data, SOPParameter param)
        {
            string strVariableName = param.VariableName.Trim();

            if (strVariableName.Length == 0)
                return false;

            int no = GetMaxVariableNo(data, false) + 1;

            string strSQL = string.Format("Insert into UserDefinedConfigVariable (ConfigID, No, VariableName, VariableType, Description) values ({0}, {1}, '{2}', {3}, '{4}')",
                data.ID, no, strVariableName, (int)param.Type, param.Description);

            if (FormMain.Instance.DBManager.GetResultData(strSQL, 0) != null)
            {
                param.No = no;
                return true;
            }

            return false;
        }

        public int GetMaxVariableNo(ConfigData data, bool transaction)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Select max(No) from UserDefinedConfigVariable where ConfigID = " + data.ID.ToString();
            ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (m_systemCall || checkBoxEdit.Checked == false || cboConfig.SelectedIndex < 0)
                return;

            List<SOPParameter> removeParams = new List<SOPParameter>();
            string strRemoveVariableNames = "";

            ConfigData data = (ConfigData)cboConfig.Items[cboConfig.SelectedIndex];

            foreach (SOPParameter param in data.Variables)
            {
                bool find = false;
                string strParamTypeName = Sections.SectionDataDecision.GetVariableTypeName(param.Type);

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    if (row.Cells[0].Value == null)
                        continue;

                    if (row.Cells[1].Value == null)
                    {
                        if (row.Cells[1].FormattedValue != null)
                            row.Cells[1].Value = row.Cells[1].FormattedValue.ToString();
                        else
                            continue;
                    }

                    string strVariableName = row.Cells[0].Value.ToString();
                    string strVariableType = row.Cells[1].Value.ToString();

                    if (strVariableName == param.VariableName && strVariableType == strParamTypeName)
                    {
                        find = true;
                        break;
                    }
                }

                if (find == false)
                {
                    removeParams.Add(param);

                    if (strRemoveVariableNames.Length == 0)
                        strRemoveVariableNames = "'" + param.VariableName + "'";
                    else
                        strRemoveVariableNames += ", '" + param.VariableName + "'";
                }
            }

            foreach (SOPParameter param in removeParams)
            {
                data.Variables.Remove(param);
            }

            if (strRemoveVariableNames.Length > 0)
            {
                if (m_useDB)
                {
                    string strSQL = string.Format("Delete from UserDefinedConfigVariable where ConfigID = (Select ID from UserDefinedConfig where ConfigName = '{0}') and VariableName in ({1})",
                        data.Text, strRemoveVariableNames);

                    FormMain.Instance.DBManager.GetResultData(strSQL, 0);
                }
            }
        }
    }
}
