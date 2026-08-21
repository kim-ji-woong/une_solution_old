namespace SOPManager.DAL
{
    using Model.Sop.Account;
    using Model.Sop.Category;
    using Model.Sop.Component;
    using Model.Sop.Config;
    using IDAL;
    using System;
    using dnsDBUtil;
    using System.Collections;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Linq;

    public class CreateManager : QueryManager, ICreate
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        private const int FindCountLimit = 100;

        public CreateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public Level CreateLevel(int? levelID, string strLevelName)
        {
            Dictionary<Level.Fields, object> dicFieldDatas = new Dictionary<Level.Fields, object>();
            dicFieldDatas[Level.Fields.LevelName] = strLevelName;

            string strSQL = "";

            if (levelID == null)
            {
                strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Level.TableName,
                GetFieldNames<Level.Fields>(),
                GetFieldValues(dicFieldDatas));
            }
            else
            {
                strSQL = string.Format("Insert into {0} ({1}) values ({2}, {3})",
                    Level.TableName,
                    GetFieldNames<Level.Fields>(),
                    (int)levelID,
                    GetFieldValues(dicFieldDatas));
            }

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                if (levelID != null)
                {
                    Level level = new Level();
                    level.ID = (int)levelID;
                    level.LevelName = strLevelName;

                    return level;
                }
                else
                {
                    bool isNullable;
                    string strPKFieldName = Level.GetFieldName(Level.Fields.ID, out isNullable);
                    string strCondition = string.Format("order by {0} desc", strPKFieldName);

                    string strErrorMessage;
                    // 가장 마지막에 삽입된 객체를 얻어온다.
                    List<Level> datas = m_dataManager.GetSelectManager().SelectLevels(null, strCondition, 1, out strErrorMessage);

                    if (datas == null || datas.Count == 0)
                    {
                        m_strErrorMessage = strErrorMessage;
                        return null;
                    }

                    if (IsSameLevel(datas[0], strLevelName))
                        return datas[0];

                    return GetLevel(strLevelName, Level.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
                }
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Level GetLevel(string strLevelName, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<Level> datas = m_dataManager.GetSelectManager().SelectLevels(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Level data in datas)
            {
                if (IsSameLevel(data, strLevelName))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetLevel(strLevelName, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameLevel(Level level, string strLevelName)
        {
            if (level.LevelName == strLevelName)
                return true;

            return false;
        }

        public User CreateUser(int? nMemberID, int nUserLevel, string strUserID, string strPassword, string strNickName, int nSiteID, string strPasswordCode = null)
        {
            Dictionary<User.Fields, object> dicFieldDatas = new Dictionary<User.Fields, object>();
            dicFieldDatas[User.Fields.MemberID] = nMemberID;
            dicFieldDatas[User.Fields.UserLevel] = nUserLevel;
            dicFieldDatas[User.Fields.UserID] = strUserID;
            dicFieldDatas[User.Fields.Password] = strPassword;
            dicFieldDatas[User.Fields.NickName] = strNickName;
            dicFieldDatas[User.Fields.SiteID] = nSiteID;
            dicFieldDatas[User.Fields.PasswordCode] = strPasswordCode;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                User.TableName,
                GetFieldNames<User.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = User.GetFieldName(User.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<User> datas = m_dataManager.GetSelectManager().SelectUsers(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameUser(datas[0], nMemberID, nUserLevel, strUserID, strPassword, strNickName, nSiteID, strPasswordCode))
                    return datas[0];

                return GetUser(nMemberID, nUserLevel, strUserID, strPassword, strNickName, nSiteID, strPasswordCode, User.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private User GetUser(int? nMemberID, int nUserLevel, string strUserID, string strPassword, string strNickName, int nSiteID, string strPasswordCode, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<User> datas = m_dataManager.GetSelectManager().SelectUsers(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (User data in datas)
            {
                if (IsSameUser(data, nMemberID, nUserLevel, strUserID, strPassword, strNickName, nSiteID, strPasswordCode))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetUser(nMemberID, nUserLevel, strUserID, strPassword, strNickName, nSiteID, strPasswordCode, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameUser(User user, int? nMemberID, int nUserLevel, string strUserID, string strPassword, string strNickName, int nSiteID, string strPasswordCode)
        {
            if (user.MemberID == nMemberID &&
                user.UserLevel == nUserLevel &&
                user.UserID == strUserID &&
                user.Password == strPassword &&
                user.NickName == strNickName &&
                user.SiteID == nSiteID &&
                user.PasswordCode == strPasswordCode)
                return true;

            return false;
        }

        public Option CreateOption(int nUserID, string strCategory, string strSubCategory, string strPropertyValue1, string strPropertyValue2, string strPropertyValue3, string strPropertyValue4)
        {
            Dictionary<Option.Fields, object> dicFieldDatas = new Dictionary<Option.Fields, object>();
            dicFieldDatas[Option.Fields.UserID] = nUserID;
            dicFieldDatas[Option.Fields.Category] = strCategory;
            dicFieldDatas[Option.Fields.SubCategory] = strSubCategory;
            dicFieldDatas[Option.Fields.PropertyValue1] = strPropertyValue1;
            dicFieldDatas[Option.Fields.PropertyValue2] = strPropertyValue2;
            dicFieldDatas[Option.Fields.PropertyValue3] = strPropertyValue3;
            dicFieldDatas[Option.Fields.PropertyValue4] = strPropertyValue4;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Option.TableName,
                GetFieldNames<Option.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = Option.GetFieldName(Option.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Option> datas = m_dataManager.GetSelectManager().SelectOptions(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameOption(datas[0], nUserID, strCategory, strSubCategory, strPropertyValue1, strPropertyValue2, strPropertyValue3, strPropertyValue4))
                    return datas[0];

                return GetOption(nUserID, strCategory, strSubCategory, strPropertyValue1, strPropertyValue2, strPropertyValue3, strPropertyValue4, Option.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Option GetOption(int nUserID, string strCategory, string strSubCategory, string strPropertyValue1, string strPropertyValue2, string strPropertyValue3, string strPropertyValue4, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<Option> datas = m_dataManager.GetSelectManager().SelectOptions(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Option data in datas)
            {
                if (IsSameOption(data, nUserID, strCategory, strSubCategory, strPropertyValue1, strPropertyValue2, strPropertyValue3, strPropertyValue4))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetOption(nUserID, strCategory, strSubCategory, strPropertyValue1, strPropertyValue2, strPropertyValue3, strPropertyValue4, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameOption(Option option, int nUserID, string strCategory, string strSubCategory, string strPropertyValue1, string strPropertyValue2, string strPropertyValue3, string strPropertyValue4)
        {
            if (option.UserID == nUserID &&
                option.Category == strCategory &&
                option.SubCategory == strSubCategory &&
                option.PropertyValue1 == strPropertyValue1 &&
                option.PropertyValue2 == strPropertyValue2 &&
                option.PropertyValue3 == strPropertyValue3 &&
                option.PropertyValue4 == strPropertyValue4)
                return true;

            return false;
        }

        public ActionStep CreateActionStep(string strStepName, int nDisasterID, int? nUserDefinedConfigID = null)
        {
            Dictionary<ActionStep.Fields, object> dicFieldDatas = new Dictionary<ActionStep.Fields, object>();
            dicFieldDatas[ActionStep.Fields.StepName] = strStepName;
            dicFieldDatas[ActionStep.Fields.DisasterID] = nDisasterID;
            dicFieldDatas[ActionStep.Fields.UserDefinedConfigID] = nUserDefinedConfigID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                ActionStep.TableName,
                GetFieldNames<ActionStep.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = ActionStep.GetFieldName(ActionStep.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<ActionStep> datas = m_dataManager.GetSelectManager().SelectActionSteps(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameActionStep(datas[0], strStepName, nDisasterID,  nUserDefinedConfigID))
                    return datas[0];

                return GetActionStep(strStepName, nDisasterID, nUserDefinedConfigID, ActionStep.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private ActionStep GetActionStep(string strStepName, int nDisasterID, int? nUserDefinedConfigID, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<ActionStep> datas = m_dataManager.GetSelectManager().SelectActionSteps(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (ActionStep data in datas)
            {
                if (IsSameActionStep(data, strStepName, nDisasterID, nUserDefinedConfigID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetActionStep(strStepName, nDisasterID, nUserDefinedConfigID, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameActionStep(ActionStep actionStep, string strStepName, int nDisasterID, int? nUserDefinedConfigID)
        {
            if (actionStep.StepName == strStepName &&
                actionStep.DisasterID == nDisasterID &&
                actionStep.UserDefinedConfigID == nUserDefinedConfigID)
                return true;

            return false;
        }

        public Annotation CreateAnnotation(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, int nStepMemberID, int? nSectionNumber = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null)
        {
            Dictionary<Annotation.Fields, object> dicFieldDatas = new Dictionary<Annotation.Fields, object>();
            dicFieldDatas[Annotation.Fields.GridID] = nGridID;
            dicFieldDatas[Annotation.Fields.GridRowIndex] = nGridRowIndex;
            dicFieldDatas[Annotation.Fields.GridColumnIndex] = nGridColumnIndex;
            dicFieldDatas[Annotation.Fields.Width] = fWidth;
            dicFieldDatas[Annotation.Fields.Height] = fHeight;
            dicFieldDatas[Annotation.Fields.Text] = strText;
            dicFieldDatas[Annotation.Fields.ComponentID] = strComponentID;
            dicFieldDatas[Annotation.Fields.StepMemberID] = nStepMemberID;
            dicFieldDatas[Annotation.Fields.VAlign] = nVAlign;
            dicFieldDatas[Annotation.Fields.HAlign] = nHAlign;
            dicFieldDatas[Annotation.Fields.FontName] = strFontName;
            dicFieldDatas[Annotation.Fields.FontStyle] = nFontStyle;
            dicFieldDatas[Annotation.Fields.FontSize] = fFontSize;
            dicFieldDatas[Annotation.Fields.LineSpace] = fLineSpace;
            dicFieldDatas[Annotation.Fields.FontColor] = nFontColor;
            dicFieldDatas[Annotation.Fields.SectionNumber] = nSectionNumber;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Annotation.TableName,
                GetFieldNames<Annotation.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = Annotation.GetFieldName(Annotation.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Annotation> datas = m_dataManager.GetSelectManager().SelectAnnotations(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameAnnotation(datas[0], nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor))
                    return datas[0];

                return GetAnnotation(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, Annotation.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Annotation GetAnnotation(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, int nStepMemberID, int? nSectionNumber, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<Annotation> datas = m_dataManager.GetSelectManager().SelectAnnotations(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Annotation data in datas)
            {
                if (IsSameAnnotation(data, nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetAnnotation(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameAnnotation(Annotation annotation, int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, int nStepMemberID, int? nSectionNumber, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor)
        {
            if (annotation.GridID == nGridID &&
                annotation.GridRowIndex == nGridRowIndex &&
                annotation.GridColumnIndex == nGridColumnIndex &&
                IsSameFloatData2(annotation.Width, fWidth) &&
                IsSameFloatData2(annotation.Height, fHeight) &&
                annotation.Text == strText &&
                annotation.ComponentID == strComponentID &&
                annotation.StepMemberID == nStepMemberID &&
                annotation.SectionNumber == nSectionNumber &&
                annotation.VAlign == nVAlign &&
                annotation.HAlign == nHAlign &&
                annotation.FontName == strFontName &&
                annotation.FontStyle == nFontStyle &&
                IsSameFloatData(annotation.FontSize, fFontSize) &&
                IsSameFloatData(annotation.LineSpace, fLineSpace) &&
                annotation.FontColor == nFontColor)
                return true;

            return false;
        }

        public Arrow CreateArrow(int nBeginComponentID, int nBeginComponentPosition, int nEndComponentID, int nEndComponentPosition, int nStepMemberID, string strText = null)
        {
            Dictionary<Arrow.Fields, object> dicFieldDatas = new Dictionary<Arrow.Fields, object>();
            dicFieldDatas[Arrow.Fields.BeginComponentID] = nBeginComponentID;
            dicFieldDatas[Arrow.Fields.BeginComponentPosition] = nBeginComponentPosition;
            dicFieldDatas[Arrow.Fields.EndComponentID] = nEndComponentID;
            dicFieldDatas[Arrow.Fields.EndComponentPosition] = nEndComponentPosition;
            dicFieldDatas[Arrow.Fields.StepMemberID] = nStepMemberID;
            dicFieldDatas[Arrow.Fields.Text] = strText;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Arrow.TableName,
                GetFieldNames<Arrow.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = Arrow.GetFieldName(Arrow.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Arrow> datas = m_dataManager.GetSelectManager().SelectArrows(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameArrow(datas[0], nBeginComponentID, nBeginComponentPosition, nEndComponentID, nEndComponentPosition, nStepMemberID, strText))
                    return datas[0];

                return GetArrow(nBeginComponentID, nBeginComponentPosition, nEndComponentID, nEndComponentPosition, nStepMemberID, strText, Arrow.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Arrow GetArrow(int nBeginComponentID, int nBeginComponentPosition, int nEndComponentID, int nEndComponentPosition, int nStepMemberID, string strText, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<Arrow> datas = m_dataManager.GetSelectManager().SelectArrows(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Arrow data in datas)
            {
                if (IsSameArrow(data, nBeginComponentID, nBeginComponentPosition, nEndComponentID, nEndComponentPosition, nStepMemberID, strText))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetArrow(nBeginComponentID, nBeginComponentPosition, nEndComponentID, nEndComponentPosition, nStepMemberID, strText, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameArrow(Arrow arrow, int nBeginComponentID, int nBeginComponentPosition, int nEndComponentID, int nEndComponentPosition, int nStepMemberID, string strText)
        {
            if (arrow.BeginComponentID == nBeginComponentID &&
                arrow.BeginComponentPosition == nBeginComponentPosition &&
                arrow.EndComponentID == nEndComponentID &&
                arrow.EndComponentPosition == nEndComponentPosition &&
                arrow.StepMemberID == nStepMemberID &&
                arrow.Text == strText)
                return true;

            return false;
        }

        public Decision CreateDecision(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, int nStepMemberID, int? nTeamID = null, int? nTeamType = null, int? nSectionNumber = null, string strDescription = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null, string strAutoRunScript = null, string strAutoRunScriptVariableTypes = null)
        {
            Dictionary<Decision.Fields, object> dicFieldDatas = new Dictionary<Decision.Fields, object>();
            dicFieldDatas[Decision.Fields.GridID] = nGridID;
            dicFieldDatas[Decision.Fields.GridRowIndex] = nGridRowIndex;
            dicFieldDatas[Decision.Fields.GridColumnIndex] = nGridColumnIndex;
            dicFieldDatas[Decision.Fields.Width] = fWidth;
            dicFieldDatas[Decision.Fields.Height] = fHeight;
            dicFieldDatas[Decision.Fields.Text] = strText;
            dicFieldDatas[Decision.Fields.ComponentID] = strComponentID;
            dicFieldDatas[Decision.Fields.StepMemberID] = nStepMemberID;
            dicFieldDatas[Decision.Fields.TeamID] = nTeamID;
            dicFieldDatas[Decision.Fields.TeamType] = nTeamType;
            dicFieldDatas[Decision.Fields.VAlign] = nVAlign;
            dicFieldDatas[Decision.Fields.HAlign] = nHAlign;
            dicFieldDatas[Decision.Fields.FontName] = strFontName;
            dicFieldDatas[Decision.Fields.FontStyle] = nFontStyle;
            dicFieldDatas[Decision.Fields.FontSize] = fFontSize;
            dicFieldDatas[Decision.Fields.LineSpace] = fLineSpace;
            dicFieldDatas[Decision.Fields.FontColor] = nFontColor;
            dicFieldDatas[Decision.Fields.AutoRunScript] = strAutoRunScript;
            dicFieldDatas[Decision.Fields.AutoRunScriptVariableTypes] = strAutoRunScriptVariableTypes;
            dicFieldDatas[Decision.Fields.SectionNumber] = nSectionNumber;
            dicFieldDatas[Decision.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Decision.TableName,
                GetFieldNames<Decision.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = Decision.GetFieldName(Decision.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Decision> datas = m_dataManager.GetSelectManager().SelectDecisions(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameDecision(datas[0], nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, nStepMemberID, nTeamID, nTeamType, nSectionNumber, strDescription, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, strAutoRunScript, strAutoRunScriptVariableTypes))
                    return datas[0];

                return GetDecision(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, nStepMemberID, nTeamID, nTeamType, nSectionNumber, strDescription, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, strAutoRunScript, strAutoRunScriptVariableTypes, Decision.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Decision GetDecision(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, int nStepMemberID, int? nTeamID, int? nTeamType, int? nSectionNumber, string strDescription, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor, string strAutoRunScript, string strAutoRunScriptVariableTypes, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<Decision> datas = m_dataManager.GetSelectManager().SelectDecisions(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Decision data in datas)
            {
                if (IsSameDecision(data, nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, nStepMemberID, nTeamID, nTeamType, nSectionNumber, strDescription, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, strAutoRunScript, strAutoRunScriptVariableTypes))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetDecision(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, nStepMemberID, nTeamID, nTeamType, nSectionNumber, strDescription, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, strAutoRunScript, strAutoRunScriptVariableTypes, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameDecision(Decision decision, int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, int nStepMemberID, int? nTeamID, int? nTeamType, int? nSectionNumber, string strDescription, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor, string strAutoRunScript, string strAutoRunScriptVariableTypes)
        {
            if (decision.GridID == nGridID &&
                decision.GridRowIndex == nGridRowIndex &&
                decision.GridColumnIndex == nGridColumnIndex &&
                IsSameFloatData2(decision.Width, fWidth) &&
                IsSameFloatData2(decision.Height, fHeight) &&
                decision.Text == strText &&
                decision.ComponentID == strComponentID &&
                decision.StepMemberID == nStepMemberID &&
                decision.TeamID == nTeamID &&
                decision.TeamType == nTeamType &&
                decision.SectionNumber == nSectionNumber &&
                decision.Description == strDescription &&
                decision.VAlign == nVAlign &&
                decision.HAlign == nHAlign &&
                decision.FontName == strFontName &&
                decision.FontStyle == nFontStyle &&
                IsSameFloatData(decision.FontSize, fFontSize) &&
                IsSameFloatData(decision.LineSpace, fLineSpace) &&
                decision.FontColor == nFontColor &&
                decision.AutoRunScript == strAutoRunScript &&
                decision.AutoRunScriptVariableTypes == strAutoRunScriptVariableTypes)
                return true;

            return false;
        }

        public Disaster CreateDisaster(string strDisasterName, int nSubDisasterCategoryID, int nVersionID, string strUserLevelIDs = null, string strDescription = null)
        {
            Dictionary<Disaster.Fields, object> dicFieldDatas = new Dictionary<Disaster.Fields, object>();
            dicFieldDatas[Disaster.Fields.DisasterName] = strDisasterName;
            dicFieldDatas[Disaster.Fields.SubDisasterCategoryID] = nSubDisasterCategoryID;
            dicFieldDatas[Disaster.Fields.VersionID] = nVersionID;
            dicFieldDatas[Disaster.Fields.UserLevelIDs] = strUserLevelIDs;
            dicFieldDatas[Disaster.Fields.Description] = strDescription;
            
            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Disaster.TableName,
                GetFieldNames<Disaster.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = Disaster.GetFieldName(Disaster.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Disaster> datas = m_dataManager.GetSelectManager().SelectDisasters(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameDisaster(datas[0], strDisasterName, nSubDisasterCategoryID, nVersionID, strUserLevelIDs, strDescription))
                    return datas[0];

                return GetDisaster(strDisasterName, nSubDisasterCategoryID, nVersionID, strUserLevelIDs, strDescription, Disaster.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Disaster GetDisaster(string strDisasterName, int nSubDisasterCategoryID, int nVersionID, string strUserLevelIDs, string strDescription, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<Disaster> datas = m_dataManager.GetSelectManager().SelectDisasters(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Disaster data in datas)
            {
                if (IsSameDisaster(data, strDisasterName, nSubDisasterCategoryID, nVersionID, strUserLevelIDs, strDescription))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetDisaster(strDisasterName, nSubDisasterCategoryID, nVersionID, strUserLevelIDs, strDescription, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameDisaster(Disaster disaster, string strDisasterName, int nSubDisasterCategoryID, int nVersionID, string strUserLevelIDs, string strDescription)
        {
            if (disaster.DisasterName == strDisasterName &&
                disaster.SubDisasterCategoryID == nSubDisasterCategoryID &&
                disaster.VersionID == nVersionID &&
                IsSameList<int>(disaster.UserLevelIDs, StringToIntList(strUserLevelIDs)) &&
                disaster.Description == strDescription)
                return true;

            return false;
        }

        public DisasterType CreateDisasterType(string strTypeName, int nSubDisasterCategoryID)
        {
            Dictionary<DisasterType.Fields, object> dicFieldDatas = new Dictionary<DisasterType.Fields, object>();
            dicFieldDatas[DisasterType.Fields.Name] = strTypeName;
            dicFieldDatas[DisasterType.Fields.SubDisasterID] = nSubDisasterCategoryID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                DisasterType.TableName,
                GetFieldNames<DisasterType.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = DisasterType.GetFieldName(DisasterType.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<DisasterType> datas = m_dataManager.GetSelectManager().SelectDisasterTypes(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameDisasterType(datas[0], strTypeName, nSubDisasterCategoryID))
                    return datas[0];

                return GetDisasterType(strTypeName, nSubDisasterCategoryID, DisasterType.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private DisasterType GetDisasterType(string strTypeName, int nSubDisasterCategoryID, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<DisasterType> datas = m_dataManager.GetSelectManager().SelectDisasterTypes(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (DisasterType data in datas)
            {
                if (IsSameDisasterType(data, strTypeName, nSubDisasterCategoryID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetDisasterType(strTypeName, nSubDisasterCategoryID, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameDisasterType(DisasterType disasterType, string strTypeName, int nSubDisasterCategoryID)
        {
            if (disasterType.Name == strTypeName &&
                disasterType.SubDisasterID == nSubDisasterCategoryID )
                return true;

            return false;
        }

        public DisasterCategory CreateDisasterCategory(string strCategoryName, int nSiteID)
        {
            Dictionary<DisasterCategory.Fields, object> dicFieldDatas = new Dictionary<DisasterCategory.Fields, object>();
            dicFieldDatas[DisasterCategory.Fields.CategoryName] = strCategoryName;
            dicFieldDatas[DisasterCategory.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                DisasterCategory.TableName,
                GetFieldNames<DisasterCategory.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = DisasterCategory.GetFieldName(DisasterCategory.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<DisasterCategory> datas = m_dataManager.GetSelectManager().SelectDisasterCategories(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameDisasterCategory(datas[0], strCategoryName, nSiteID))
                    return datas[0];

                return GetDisasterCategory(strCategoryName, nSiteID, DisasterCategory.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private DisasterCategory GetDisasterCategory(string strCategoryName, int nSiteID, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<DisasterCategory> datas = m_dataManager.GetSelectManager().SelectDisasterCategories(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (DisasterCategory data in datas)
            {
                if (IsSameDisasterCategory(data, strCategoryName, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetDisasterCategory(strCategoryName, nSiteID, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameDisasterCategory(DisasterCategory disasterCategory, string strCategoryName, int nSiteID)
        {
            if (disasterCategory.CategoryName == strCategoryName &&
                disasterCategory.SiteID == nSiteID)
                return true;

            return false;
        }

        public EndPoint CreateEndPoint(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, bool isBegin, int nStepMemberID, int? nSectionNumber = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null)
        {
            Dictionary<EndPoint.Fields, object> dicFieldDatas = new Dictionary<EndPoint.Fields, object>();
            dicFieldDatas[EndPoint.Fields.GridID] = nGridID;
            dicFieldDatas[EndPoint.Fields.GridRowIndex] = nGridRowIndex;
            dicFieldDatas[EndPoint.Fields.GridColumnIndex] = nGridColumnIndex;
            dicFieldDatas[EndPoint.Fields.Width] = fWidth;
            dicFieldDatas[EndPoint.Fields.Height] = fHeight;
            dicFieldDatas[EndPoint.Fields.Text] = strText;
            dicFieldDatas[EndPoint.Fields.ComponentID] = strComponentID;
            dicFieldDatas[EndPoint.Fields.IsBegin] = isBegin;
            dicFieldDatas[EndPoint.Fields.StepMemberID] = nStepMemberID;
            dicFieldDatas[EndPoint.Fields.VAlign] = nVAlign;
            dicFieldDatas[EndPoint.Fields.HAlign] = nHAlign;
            dicFieldDatas[EndPoint.Fields.FontName] = strFontName;
            dicFieldDatas[EndPoint.Fields.FontStyle] = nFontStyle;
            dicFieldDatas[EndPoint.Fields.FontSize] = fFontSize;
            dicFieldDatas[EndPoint.Fields.LineSpace] = fLineSpace;
            dicFieldDatas[EndPoint.Fields.FontColor] = nFontColor;
            dicFieldDatas[EndPoint.Fields.SectionNumber] = nSectionNumber;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                EndPoint.TableName,
                GetFieldNames<EndPoint.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = EndPoint.GetFieldName(EndPoint.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<EndPoint> datas = m_dataManager.GetSelectManager().SelectEndPoints(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameEndPoint(datas[0], nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, isBegin, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor))
                    return datas[0];

                return GetEndPoint(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, isBegin, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, EndPoint.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private EndPoint GetEndPoint(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, bool isBegin, int nStepMemberID, int? nSectionNumber, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<EndPoint> datas = m_dataManager.GetSelectManager().SelectEndPoints(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (EndPoint data in datas)
            {
                if (IsSameEndPoint(data, nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, isBegin, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetEndPoint(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, isBegin, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameEndPoint(EndPoint endpoint, int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, bool isBegin, int nStepMemberID, int? nSectionNumber, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor)
        {
            if (endpoint.GridID == nGridID &&
                endpoint.GridRowIndex == nGridRowIndex &&
                endpoint.GridColumnIndex == nGridColumnIndex &&
                IsSameFloatData2(endpoint.Width, fWidth) &&
                IsSameFloatData2(endpoint.Height, fHeight) &&
                endpoint.Text == strText &&
                endpoint.ComponentID == strComponentID &&
                endpoint.IsBegin == isBegin &&
                endpoint.StepMemberID == nStepMemberID &&
                endpoint.SectionNumber == nSectionNumber &&
                endpoint.VAlign == nVAlign &&
                endpoint.HAlign == nHAlign &&
                endpoint.FontName == strFontName &&
                endpoint.FontStyle == nFontStyle &&
                IsSameFloatData(endpoint.FontSize, fFontSize) &&
                IsSameFloatData(endpoint.LineSpace, fLineSpace) &&
                endpoint.FontColor == nFontColor)
                return true;

            return false;
        }

        public ExternalProgram CreateExternalProgram(string strExeName, string strDescription, string strInstallPath = null)
        {
            Dictionary<ExternalProgram.Fields, object> dicFieldDatas = new Dictionary<ExternalProgram.Fields, object>();
            dicFieldDatas[ExternalProgram.Fields.ExeName] = strExeName;
            dicFieldDatas[ExternalProgram.Fields.Description] = strDescription;
            dicFieldDatas[ExternalProgram.Fields.InstallPath] = strInstallPath;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                ExternalProgram.TableName,
                GetFieldNames<ExternalProgram.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = ExternalProgram.GetFieldName(ExternalProgram.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<ExternalProgram> datas = m_dataManager.GetSelectManager().SelectExternalPrograms(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameExternalProgram(datas[0], strExeName, strDescription, strInstallPath))
                    return datas[0];

                return GetExternalProgram(strExeName, strDescription, strInstallPath, ExternalProgram.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private ExternalProgram GetExternalProgram(string strExeName, string strDescription, string strInstallPath, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<ExternalProgram> datas = m_dataManager.GetSelectManager().SelectExternalPrograms(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (ExternalProgram data in datas)
            {
                if (IsSameExternalProgram(data, strExeName, strDescription, strInstallPath))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetExternalProgram(strExeName, strDescription, strInstallPath, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameExternalProgram(ExternalProgram externalProgram, string strExeName, string strDescription, string strInstallPath)
        {
            if (externalProgram.ExeName == strExeName &&
                externalProgram.Description == strDescription &&
                externalProgram.InstallPath == strInstallPath)
                return true;

            return false;
        }

        public ExternalProgramParameter CreateExternalProgramParameter(int nProgramID, int nParameterIndex, string strParameterName, int nValueType, bool isNullable)
        {
            Dictionary<ExternalProgramParameter.Fields, object> dicFieldDatas = new Dictionary<ExternalProgramParameter.Fields, object>();
            dicFieldDatas[ExternalProgramParameter.Fields.ProgramID] = nProgramID;
            dicFieldDatas[ExternalProgramParameter.Fields.ParameterIndex] = nParameterIndex;
            dicFieldDatas[ExternalProgramParameter.Fields.ParameterName] = strParameterName;
            dicFieldDatas[ExternalProgramParameter.Fields.ValueType] = nValueType;
            dicFieldDatas[ExternalProgramParameter.Fields.IsNullable] = isNullable;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                ExternalProgramParameter.TableName,
                GetFieldNames<ExternalProgramParameter.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                ExternalProgramParameter parameter = new ExternalProgramParameter();

                parameter.ProgramID = nProgramID;
                parameter.ParameterIndex = nParameterIndex;
                parameter.ParameterName = strParameterName;
                parameter.ValueType = nValueType;
                parameter.IsNullable = isNullable;

                return parameter;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public SectionGrid CreateGrid(int nStepMemberID)
        {
            Dictionary<SectionGrid.Fields, object> dicFieldDatas = new Dictionary<SectionGrid.Fields, object>();
            dicFieldDatas[SectionGrid.Fields.StepMemberID] = nStepMemberID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                SectionGrid.TableName,
                GetFieldNames<SectionGrid.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = SectionGrid.GetFieldName(SectionGrid.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<SectionGrid> datas = m_dataManager.GetSelectManager().SelectGrids(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSectionGrid(datas[0], nStepMemberID))
                    return datas[0];

                return GetSectionGrid(nStepMemberID, SectionGrid.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private SectionGrid GetSectionGrid(int nStepMemberID, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<SectionGrid> datas = m_dataManager.GetSelectManager().SelectGrids(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (SectionGrid data in datas)
            {
                if (IsSameSectionGrid(data, nStepMemberID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSectionGrid(nStepMemberID, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameSectionGrid(SectionGrid grid, int nStepMemberID)
        {
            if (grid.StepMemberID == nStepMemberID)
                return true;

            return false;
        }

        public SectionGridColumn CreateGridColumn(int nGridID, int nColumnIndex, int nWidth)
        {
            Dictionary<SectionGridColumn.Fields, object> dicFieldDatas = new Dictionary<SectionGridColumn.Fields, object>();
            dicFieldDatas[SectionGridColumn.Fields.GridID] = nGridID;
            dicFieldDatas[SectionGridColumn.Fields.ColumnIndex] = nColumnIndex;
            dicFieldDatas[SectionGridColumn.Fields.Width] = nWidth;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                SectionGridColumn.TableName,
                GetFieldNames<SectionGridColumn.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                SectionGridColumn column = new SectionGridColumn();

                column.GridID = nGridID;
                column.ColumnIndex = nColumnIndex;
                column.Width = nWidth;

                return column;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public SectionGridRow CreateGridRow(int nGridID, int nRowIndex, int nHeight)
        {
            Dictionary<SectionGridRow.Fields, object> dicFieldDatas = new Dictionary<SectionGridRow.Fields, object>();
            dicFieldDatas[SectionGridRow.Fields.GridID] = nGridID;
            dicFieldDatas[SectionGridRow.Fields.RowIndex] = nRowIndex;
            dicFieldDatas[SectionGridRow.Fields.Height] = nHeight;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                SectionGridRow.TableName,
                GetFieldNames<SectionGridRow.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                SectionGridRow row = new SectionGridRow();

                row.GridID = nGridID;
                row.RowIndex = nRowIndex;
                row.Height = nHeight;

                return row;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public InternalTransmission CreateInternalTransmission(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, bool useSMS, bool useBroadcast, bool useEmail, int nStepMemberID, bool autoRun, string strMessage = null, List<Receiver> teamList = null, bool? useSiren = null, bool? onlyTeamLeader = null, int? nSectionNumber = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null)
        {
            Dictionary<InternalTransmission.Fields, object> dicFieldDatas = new Dictionary<InternalTransmission.Fields, object>();
            dicFieldDatas[InternalTransmission.Fields.GridID] = nGridID;
            dicFieldDatas[InternalTransmission.Fields.GridRowIndex] = nGridRowIndex;
            dicFieldDatas[InternalTransmission.Fields.GridColumnIndex] = nGridColumnIndex;
            dicFieldDatas[InternalTransmission.Fields.Width] = fWidth;
            dicFieldDatas[InternalTransmission.Fields.Height] = fHeight;
            dicFieldDatas[InternalTransmission.Fields.Text] = strText;
            dicFieldDatas[InternalTransmission.Fields.ComponentID] = strComponentID;
            dicFieldDatas[InternalTransmission.Fields.UseSMS] = useSMS;
            dicFieldDatas[InternalTransmission.Fields.UseBroadcast] = useBroadcast;
            dicFieldDatas[InternalTransmission.Fields.UseEmail] = useEmail;
            dicFieldDatas[InternalTransmission.Fields.StepMemberID] = nStepMemberID;
            dicFieldDatas[InternalTransmission.Fields.AutoRun] = autoRun;
            dicFieldDatas[InternalTransmission.Fields.Message] = strMessage;
            dicFieldDatas[InternalTransmission.Fields.TeamList] = teamList == null ? null : MakeTeamListString(teamList);
            dicFieldDatas[InternalTransmission.Fields.UseSiren] = useSiren;
            dicFieldDatas[InternalTransmission.Fields.OnlyTeamLeader] = onlyTeamLeader;
            dicFieldDatas[InternalTransmission.Fields.VAlign] = nVAlign;
            dicFieldDatas[InternalTransmission.Fields.HAlign] = nHAlign;
            dicFieldDatas[InternalTransmission.Fields.FontName] = strFontName;
            dicFieldDatas[InternalTransmission.Fields.FontStyle] = nFontStyle;
            dicFieldDatas[InternalTransmission.Fields.FontSize] = fFontSize;
            dicFieldDatas[InternalTransmission.Fields.LineSpace] = fLineSpace;
            dicFieldDatas[InternalTransmission.Fields.FontColor] = nFontColor;
            dicFieldDatas[InternalTransmission.Fields.SectionNumber] = nSectionNumber;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                InternalTransmission.TableName,
                GetFieldNames<InternalTransmission.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = InternalTransmission.GetFieldName(InternalTransmission.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<InternalTransmission> datas = m_dataManager.GetSelectManager().SelectInternalTransmissions(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameInternalTransmission(datas[0], nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, useSMS, useBroadcast, useEmail, nStepMemberID, autoRun, strMessage, teamList, useSiren, onlyTeamLeader, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor))
                    return datas[0];

                return GetInternalTransmission(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, useSMS, useBroadcast, useEmail, nStepMemberID, autoRun, strMessage, teamList, useSiren, onlyTeamLeader, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, InternalTransmission.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private InternalTransmission GetInternalTransmission(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, bool useSMS, bool useBroadcast, bool useEmail, int nStepMemberID, bool autoRun, string strMessage, List<Receiver> teamList, bool? useSiren, bool? onlyTeamLeader, int? nSectionNumber, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<InternalTransmission> datas = m_dataManager.GetSelectManager().SelectInternalTransmissions(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (InternalTransmission data in datas)
            {
                if (IsSameInternalTransmission(data, nGridID, nGridRowIndex, nGridColumnIndex, fWidth,fHeight, strText, strComponentID, useSMS, useBroadcast, useEmail, nStepMemberID, autoRun, strMessage, teamList, useSiren, onlyTeamLeader, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetInternalTransmission(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, useSMS, useBroadcast, useEmail, nStepMemberID, autoRun, strMessage, teamList, useSiren, onlyTeamLeader, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameInternalTransmission(InternalTransmission internalTransmission, int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, bool useSMS, bool useBroadcast, bool useEmail, int nStepMemberID, bool autoRun, string strMessage, List<Receiver> teamList, bool? useSiren, bool? onlyTeamLeader, int? nSectionNumber, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor)
        {
            if (internalTransmission.GridID == nGridID &&
                internalTransmission.GridRowIndex == nGridRowIndex &&
                internalTransmission.GridColumnIndex == nGridColumnIndex &&
                IsSameFloatData2(internalTransmission.Width, fWidth) &&
                IsSameFloatData2(internalTransmission.Height, fHeight) &&
                internalTransmission.Text == strText &&
                internalTransmission.ComponentID == strComponentID &&
                internalTransmission.UseSMS == useSMS &&
                internalTransmission.UseBroadcast == useBroadcast &&
                internalTransmission.UseEmail == useEmail &&
                internalTransmission.StepMemberID == nStepMemberID &&
                internalTransmission.AutoRun == autoRun &&
                internalTransmission.Message == strMessage &&
                IsSameReceiverList(internalTransmission.TeamList, teamList) &&
                internalTransmission.UseSiren == useSiren &&
                internalTransmission.OnlyTeamLeader == onlyTeamLeader &&
                internalTransmission.SectionNumber == nSectionNumber &&
                internalTransmission.VAlign == nVAlign &&
                internalTransmission.HAlign == nHAlign &&
                internalTransmission.FontName == strFontName &&
                internalTransmission.FontStyle == nFontStyle &&
                IsSameFloatData(internalTransmission.FontSize, fFontSize) &&
                IsSameFloatData(internalTransmission.LineSpace, fLineSpace) &&
                internalTransmission.FontColor == nFontColor)
                return true;

            return false;
        }

        public Link CreateLink(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, string strLinkedComponentIDs, int nStepMemberID, int? nSectionNumber = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null)
        {
            Dictionary<Link.Fields, object> dicFieldDatas = new Dictionary<Link.Fields, object>();
            dicFieldDatas[Link.Fields.GridID] = nGridID;
            dicFieldDatas[Link.Fields.GridRowIndex] = nGridRowIndex;
            dicFieldDatas[Link.Fields.GridColumnIndex] = nGridColumnIndex;
            dicFieldDatas[Link.Fields.Width] = fWidth;
            dicFieldDatas[Link.Fields.Height] = fHeight;
            dicFieldDatas[Link.Fields.Text] = strText;
            dicFieldDatas[Link.Fields.ComponentID] = strComponentID;
            dicFieldDatas[Link.Fields.LinkedComponentIDList] = strLinkedComponentIDs;
            dicFieldDatas[Link.Fields.StepMemberID] = nStepMemberID;
            dicFieldDatas[Link.Fields.VAlign] = nVAlign;
            dicFieldDatas[Link.Fields.HAlign] = nHAlign;
            dicFieldDatas[Link.Fields.FontName] = strFontName;
            dicFieldDatas[Link.Fields.FontStyle] = nFontStyle;
            dicFieldDatas[Link.Fields.FontSize] = fFontSize;
            dicFieldDatas[Link.Fields.LineSpace] = fLineSpace;
            dicFieldDatas[Link.Fields.FontColor] = nFontColor;
            dicFieldDatas[Link.Fields.SectionNumber] = nSectionNumber;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Link.TableName,
                GetFieldNames<Link.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = Link.GetFieldName(Link.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Link> datas = m_dataManager.GetSelectManager().SelectLinks(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameLink(datas[0], nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, strLinkedComponentIDs, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor))
                    return datas[0];

                return GetLink(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, strLinkedComponentIDs, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, Link.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Link GetLink(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, string strLinkedComponentIDs, int nStepMemberID, int? nSectionNumber, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<Link> datas = m_dataManager.GetSelectManager().SelectLinks(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Link data in datas)
            {
                if (IsSameLink(data, nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, strLinkedComponentIDs, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetLink(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, strComponentID, strLinkedComponentIDs, nStepMemberID, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameLink(Link link, int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, string strLinkedComponentIDs, int nStepMemberID, int? nSectionNumber, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor)
        {
            if (link.GridID == nGridID &&
                link.GridRowIndex == nGridRowIndex &&
                link.GridColumnIndex == nGridColumnIndex &&
                IsSameFloatData2(link.Width, fWidth) &&
                IsSameFloatData2(link.Height, fHeight) &&
                link.Text == strText &&
                link.ComponentID == strComponentID &&
                IsSameList<string>(link.LinkedComponentIDList, StringToStringList(strLinkedComponentIDs)) &&
                link.StepMemberID == nStepMemberID &&
                link.SectionNumber == nSectionNumber &&
                link.VAlign == nVAlign &&
                link.HAlign == nHAlign &&
                link.FontName == strFontName &&
                link.FontStyle == nFontStyle &&
                IsSameFloatData(link.FontSize, fFontSize) &&
                IsSameFloatData(link.LineSpace, fLineSpace) &&
                link.FontColor == nFontColor)
                return true;

            return false;
        }

        public Process CreateProcess(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, List<Receiver> teamList, string strComponentID, int nStepMemberID, bool autoRun, bool? onlyTeamLeader, int? nSectionNumber = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null)
        {
            Dictionary<Process.Fields, object> dicFieldDatas = new Dictionary<Process.Fields, object>();
            dicFieldDatas[Process.Fields.GridID] = nGridID;
            dicFieldDatas[Process.Fields.GridRowIndex] = nGridRowIndex;
            dicFieldDatas[Process.Fields.GridColumnIndex] = nGridColumnIndex;
            dicFieldDatas[Process.Fields.Width] = fWidth;
            dicFieldDatas[Process.Fields.Height] = fHeight;
            dicFieldDatas[Process.Fields.Text] = strText;
            dicFieldDatas[Process.Fields.ComponentID] = strComponentID;
            dicFieldDatas[Process.Fields.StepMemberID] = nStepMemberID;
            dicFieldDatas[Process.Fields.AutoRun] = autoRun;
            dicFieldDatas[Process.Fields.TeamList] = MakeTeamListString(teamList);
            dicFieldDatas[Process.Fields.OnlyTeamLeader] = onlyTeamLeader;
            dicFieldDatas[Process.Fields.VAlign] = nVAlign;
            dicFieldDatas[Process.Fields.HAlign] = nHAlign;
            dicFieldDatas[Process.Fields.FontName] = strFontName;
            dicFieldDatas[Process.Fields.FontStyle] = nFontStyle;
            dicFieldDatas[Process.Fields.FontSize] = fFontSize;
            dicFieldDatas[Process.Fields.LineSpace] = fLineSpace;
            dicFieldDatas[Process.Fields.FontColor] = nFontColor;
            dicFieldDatas[Process.Fields.SectionNumber] = nSectionNumber;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Process.TableName,
                GetFieldNames<Process.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = Process.GetFieldName(Process.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Process> datas = m_dataManager.GetSelectManager().SelectProcesses(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameProcess(datas[0], nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, teamList, strComponentID, nStepMemberID, autoRun, onlyTeamLeader, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor))
                    return datas[0];

                return GetProcess(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, teamList, strComponentID, nStepMemberID, autoRun, onlyTeamLeader, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, Process.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Process GetProcess(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, List<Receiver> teamList, string strComponentID, int nStepMemberID, bool autoRun, bool? onlyTeamLeader, int? nSectionNumber, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<Process> datas = m_dataManager.GetSelectManager().SelectProcesses(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Process data in datas)
            {
                if (IsSameProcess(data, nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, teamList, strComponentID, nStepMemberID, autoRun, onlyTeamLeader, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetProcess(nGridID, nGridRowIndex, nGridColumnIndex, fWidth, fHeight, strText, teamList, strComponentID, nStepMemberID, autoRun, onlyTeamLeader, nSectionNumber, nVAlign, nHAlign, strFontName, nFontStyle, fFontSize, fLineSpace, nFontColor, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameProcess(Process process, int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, List<Receiver> teamList, string strComponentID, int nStepMemberID, bool autoRun, bool? onlyTeamLeader, int? nSectionNumber, int? nVAlign, int? nHAlign, string strFontName, int? nFontStyle, float? fFontSize, float? fLineSpace, int? nFontColor)
        {
            if (process.GridID == nGridID &&
                process.GridRowIndex == nGridRowIndex &&
                process.GridColumnIndex == nGridColumnIndex &&
                IsSameFloatData2(process.Width, fWidth) &&
                IsSameFloatData2(process.Height, fHeight) &&
                process.Text == strText &&
                IsSameReceiverList(process.TeamList, teamList) &&
                process.ComponentID == strComponentID &&
                process.StepMemberID == nStepMemberID &&
                process.AutoRun == autoRun &&
                process.OnlyTeamLeader == onlyTeamLeader &&
                process.SectionNumber == nSectionNumber &&
                process.VAlign == nVAlign &&
                process.HAlign == nHAlign &&
                process.FontName == strFontName &&
                process.FontStyle == nFontStyle &&
                IsSameFloatData(process.FontSize, fFontSize) &&
                IsSameFloatData(process.LineSpace, fLineSpace) &&
                process.FontColor == nFontColor)
                return true;

            return false;
        }

        public static string MakeTeamListString(List<Receiver> teamList)
        {
            string strTeamList = "";

            if (teamList != null)
            {
                foreach (Receiver receiver in teamList)
                {
                    if (strTeamList.Length == 0)
                        strTeamList = string.Format("{0}({1})", receiver.TeamID, receiver.TeamType);
                    else
                        strTeamList += string.Format(", {0}({1})", receiver.TeamID, receiver.TeamType);
                }
            }

            return strTeamList;
        }

        public ProcessMission CreateProcessMission(string strMissionText, int nProcessID)
        {
            Dictionary<ProcessMission.Fields, object> dicFieldDatas = new Dictionary<ProcessMission.Fields, object>();
            dicFieldDatas[ProcessMission.Fields.MissionText] = strMissionText;
            dicFieldDatas[ProcessMission.Fields.ProcessID] = nProcessID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                ProcessMission.TableName,
                GetFieldNames<ProcessMission.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = ProcessMission.GetFieldName(ProcessMission.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<ProcessMission> datas = m_dataManager.GetSelectManager().SelectProcessMissions(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameProcessMission(datas[0], strMissionText, nProcessID))
                    return datas[0];

                return GetProcessMission(strMissionText, nProcessID, ProcessMission.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private ProcessMission GetProcessMission(string strMissionText, int nProcessID, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<ProcessMission> datas = m_dataManager.GetSelectManager().SelectProcessMissions(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (ProcessMission data in datas)
            {
                if (IsSameProcessMission(data, strMissionText, nProcessID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetProcessMission(strMissionText, nProcessID, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameProcessMission(ProcessMission mission, string strMissionText, int nProcessID)
        {
            if (mission.MissionText == strMissionText &&
                mission.ProcessID == nProcessID)
                return true;

            return false;
        }

        public ProcessExternalMission CreateProcessExternalMission(int nProcessID, int nOrderIndex, int nProgramID, int nParameterIndex, string strValue = null)
        {
            Dictionary<ProcessExternalMission.Fields, object> dicFieldDatas = new Dictionary<ProcessExternalMission.Fields, object>();
            dicFieldDatas[ProcessExternalMission.Fields.ProcessID] = nProcessID;
            dicFieldDatas[ProcessExternalMission.Fields.OrderIndex] = nOrderIndex;
            dicFieldDatas[ProcessExternalMission.Fields.ProgramID] = nProgramID;
            dicFieldDatas[ProcessExternalMission.Fields.ParameterIndex] = nParameterIndex;
            dicFieldDatas[ProcessExternalMission.Fields.Value] = strValue;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                ProcessExternalMission.TableName,
                GetFieldNames<ProcessExternalMission.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                // Value를 빼면 모두 Primary Key 조건이 된다.
                ProcessExternalMission mission = new ProcessExternalMission();

                mission.ProcessID = nProcessID;
                mission.OrderIndex = nOrderIndex;
                mission.ProgramID = nProgramID;
                mission.ParameterIndex = nParameterIndex;
                mission.Value = strValue;

                return mission;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public StepMember CreateStepMember(int nTeamID, int nTeamType, int nActionStepID)
        {
            Dictionary<StepMember.Fields, object> dicFieldDatas = new Dictionary<StepMember.Fields, object>();
            dicFieldDatas[StepMember.Fields.TeamID] = nTeamID;
            dicFieldDatas[StepMember.Fields.TeamType] = nTeamType;
            dicFieldDatas[StepMember.Fields.ActionStepID] = nActionStepID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                StepMember.TableName,
                GetFieldNames<StepMember.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = StepMember.GetFieldName(StepMember.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<StepMember> datas = m_dataManager.GetSelectManager().SelectStepMembers(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameStepMember(datas[0], nTeamID, nTeamType, nActionStepID))
                    return datas[0];

                return GetStepMember(nTeamID, nTeamType, nActionStepID, StepMember.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private StepMember GetStepMember(int nTeamID, int nTeamType, int nActionStepID, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<StepMember> datas = m_dataManager.GetSelectManager().SelectStepMembers(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (StepMember data in datas)
            {
                if (IsSameStepMember(data, nTeamID, nTeamType, nActionStepID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetStepMember(nTeamID, nTeamType, nActionStepID, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameStepMember(StepMember stepMember, int nTeamID, int nTeamType, int nActionStepID)
        {
            if (stepMember.TeamID == nTeamID &&
                stepMember.TeamType == nTeamType &&
                stepMember.ActionStepID == nActionStepID)
                return true;

            return false;
        }

        public SubDisasterCategory CreateSubDisasterCategory(int nDisasterCategoryID, string strSubCategoryName)
        {
            Dictionary<SubDisasterCategory.Fields, object> dicFieldDatas = new Dictionary<SubDisasterCategory.Fields, object>();
            dicFieldDatas[SubDisasterCategory.Fields.DisasterCategoryID] = nDisasterCategoryID;
            dicFieldDatas[SubDisasterCategory.Fields.SubCategoryName] = strSubCategoryName;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                SubDisasterCategory.TableName,
                GetFieldNames<SubDisasterCategory.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = SubDisasterCategory.GetFieldName(SubDisasterCategory.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<SubDisasterCategory> datas = m_dataManager.GetSelectManager().SelectSubDisasterCategories(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSubDisasterCategory(datas[0], nDisasterCategoryID, strSubCategoryName))
                    return datas[0];

                return GetSubDisasterCategory(nDisasterCategoryID, strSubCategoryName, SubDisasterCategory.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private SubDisasterCategory GetSubDisasterCategory(int nDisasterCategoryID, string strSubCategoryName, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<SubDisasterCategory> datas = m_dataManager.GetSelectManager().SelectSubDisasterCategories(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (SubDisasterCategory data in datas)
            {
                if (IsSameSubDisasterCategory(data, nDisasterCategoryID, strSubCategoryName))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSubDisasterCategory(nDisasterCategoryID, strSubCategoryName, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameSubDisasterCategory(SubDisasterCategory sdc, int nDisasterCategoryID, string strSubCategoryName)
        {
            if (sdc.DisasterCategoryID == nDisasterCategoryID &&
                sdc.SubCategoryName == strSubCategoryName)
                return true;

            return false;
        }

        public Model.Sop.Category.Version CreateVersion(bool isNormal, DateTime dtCreate, DateTime dtLastAccess, string strVersionName, int nOwnerID, int nSiteID, string strDescription = null)
        {
            Dictionary<Model.Sop.Category.Version.Fields, object> dicFieldDatas = new Dictionary<Model.Sop.Category.Version.Fields, object>();
            dicFieldDatas[Model.Sop.Category.Version.Fields.IsNormal] = isNormal;
            dicFieldDatas[Model.Sop.Category.Version.Fields.CreateTime] = dtCreate;
            dicFieldDatas[Model.Sop.Category.Version.Fields.LastAccessTime] = dtLastAccess;
            dicFieldDatas[Model.Sop.Category.Version.Fields.VersionName] = strVersionName;
            dicFieldDatas[Model.Sop.Category.Version.Fields.OwnerID] = nOwnerID;
            dicFieldDatas[Model.Sop.Category.Version.Fields.SiteID] = nSiteID;
            dicFieldDatas[Model.Sop.Category.Version.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Model.Sop.Category.Version.TableName,
                GetFieldNames<Model.Sop.Category.Version.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Model.Sop.Category.Version> datas = m_dataManager.GetSelectManager().SelectVersions(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameVersion(datas[0], isNormal, dtCreate, dtLastAccess, strVersionName, nOwnerID, nSiteID, strDescription))
                    return datas[0];

                return GetVersion(isNormal, dtCreate, dtLastAccess, strVersionName, nOwnerID, nSiteID, strDescription, Model.Sop.Category.Version.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Model.Sop.Category.Version GetVersion(bool isNormal, DateTime dtCreate, DateTime dtLastAccess, string strVersionName, int nOwnerID, int nSiteID, string strDescription, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<Model.Sop.Category.Version> datas = m_dataManager.GetSelectManager().SelectVersions(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Model.Sop.Category.Version data in datas)
            {
                if (IsSameVersion(data, isNormal, dtCreate, dtLastAccess, strVersionName, nOwnerID, nSiteID, strDescription))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetVersion(isNormal, dtCreate, dtLastAccess, strVersionName, nOwnerID, nSiteID, strDescription, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameVersion(Model.Sop.Category.Version version, bool isNormal, DateTime dtCreate, DateTime dtLastAccess, string strVersionName, int nOwnerID, int nSiteID, string strDescription)
        {
            if (version.IsNormal == isNormal &&
                version.CreateTime.ToString("yyyyMMddHHmmss") == dtCreate.ToString("yyyyMMddHHmmss") &&
                version.LastAccessTime.ToString("yyyyMMddHHmmss") == dtLastAccess.ToString("yyyyMMddHHmmss") &&
                version.VersionName == strVersionName &&
                version.OwnerID == nOwnerID &&
                version.SiteID == nSiteID &&
                version.Description == strDescription)
                return true;

            return false;
        }

        public SpecialMessage CreateSpecialMessage(string strCategory, string strMessage, string strDescription = null)
        {
            Dictionary<SpecialMessage.Fields, object> dicFieldDatas = new Dictionary<SpecialMessage.Fields, object>();
            dicFieldDatas[SpecialMessage.Fields.Category] = strCategory;
            dicFieldDatas[SpecialMessage.Fields.Message] = strMessage;
            dicFieldDatas[SpecialMessage.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                SpecialMessage.TableName,
                GetFieldNames<SpecialMessage.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = SpecialMessage.GetFieldName(SpecialMessage.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<SpecialMessage> datas = m_dataManager.GetSelectManager().SelectSpecialMessages(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSpecialMessage(datas[0], strCategory, strMessage, strDescription))
                    return datas[0];

                return GetSpecialMessage(strCategory, strMessage, strDescription, SpecialMessage.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private SpecialMessage GetSpecialMessage(string strCategory, string strMessage, string strDescription, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<SpecialMessage> datas = m_dataManager.GetSelectManager().SelectSpecialMessages(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (SpecialMessage data in datas)
            {
                if (IsSameSpecialMessage(data, strCategory, strMessage, strDescription))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSpecialMessage(strCategory, strMessage, strDescription, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameSpecialMessage(SpecialMessage message, string strCategory, string strMessage, string strDescription)
        {
            if (message.Category == strCategory &&
                message.Message == strMessage &&
                message.Description == strDescription)
                return true;

            return false;
        }

        public Session CreateSession(int nAccountUserID, string strSessionKey, DateTime dtCreateDate, DateTime dtUpdateeDate, bool autoLogin)
        {
            Dictionary<Session.Fields, object> dicFieldDatas = new Dictionary<Session.Fields, object>();
            dicFieldDatas[Session.Fields.AccountUserID] = nAccountUserID;
            dicFieldDatas[Session.Fields.SessionKey] = strSessionKey;
            dicFieldDatas[Session.Fields.CreateDate] = dtCreateDate;
            dicFieldDatas[Session.Fields.UpdateDate] = dtUpdateeDate;
            dicFieldDatas[Session.Fields.IsAutoLogin] = autoLogin;


            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Session.TableName,
                GetFieldNames<Session.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = Session.GetFieldName(Session.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Session> datas = m_dataManager.GetSelectManager().SelectSessions(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSession(datas[0], nAccountUserID, strSessionKey, dtCreateDate, dtUpdateeDate, autoLogin))
                    return datas[0];

                return GetSession(nAccountUserID, strSessionKey, dtCreateDate, dtUpdateeDate, autoLogin, Session.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Session GetSession(int nAccountUserID, string strSessionKey, DateTime dtCreateDate, DateTime dtUpdateeDate, bool autoLogin, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<Session> datas = m_dataManager.GetSelectManager().SelectSessions(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Session data in datas)
            {
                if (IsSameSession(data, nAccountUserID, strSessionKey, dtCreateDate, dtUpdateeDate, autoLogin))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSession(nAccountUserID, strSessionKey, dtCreateDate, dtUpdateeDate, autoLogin, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameSession(Session session, int nAccountUserID, string strSessionKey, DateTime dtCreateDate, DateTime dtUpdateeDate, bool autoLogin)
        {
            if (session.AccountUserID == nAccountUserID &&
                session.SessionKey == strSessionKey &&
                session.CreateDate.ToString("yyyyMMddHHmmss") == dtCreateDate.ToString("yyyyMMddHHmmss") &&
                session.UpdateDate.ToString("yyyyMMddHHmmss") == dtUpdateeDate.ToString("yyyyMMddHHmmss") &&
                session.IsAutoLogin == autoLogin)
                return true;

            return false;
        }

        public LinkedSop CreateLinkedSop(int nFacilityTypeID, int nDisasterCategoryID, int nSubDisasterCategoryID, string strDisasterName, int? nLinkedBuildingID, int? nLinkedZoneID, string strDescription)
        {
            Dictionary<LinkedSop.Fields, object> dicFieldDatas = new Dictionary<LinkedSop.Fields, object>();
            dicFieldDatas[LinkedSop.Fields.FacilityTypeID] = nFacilityTypeID;
            dicFieldDatas[LinkedSop.Fields.DisasterCategoryID] = nDisasterCategoryID;
            dicFieldDatas[LinkedSop.Fields.SubDisasterCategoryID] = nSubDisasterCategoryID;
            dicFieldDatas[LinkedSop.Fields.DisasterName] = strDisasterName;
            dicFieldDatas[LinkedSop.Fields.LinkedBuildingID] = nLinkedBuildingID;
            dicFieldDatas[LinkedSop.Fields.LinkedZoneID] = nLinkedZoneID;
            dicFieldDatas[LinkedSop.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                LinkedSop.TableName,
                GetFieldNames<LinkedSop.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strPKFieldName = LinkedSop.GetFieldName(LinkedSop.Fields.ID, out isNullable);
                string strCondition = string.Format("order by {0} desc", strPKFieldName);

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<LinkedSop> datas = m_dataManager.GetSelectManager().SelectLinkedSops(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameLinkedSop(datas[0], nFacilityTypeID, nDisasterCategoryID, nSubDisasterCategoryID, strDisasterName, nLinkedBuildingID, nLinkedZoneID, strDescription))
                    return datas[0];

                return GetLinkedSop(nFacilityTypeID, nDisasterCategoryID, nSubDisasterCategoryID, strDisasterName, nLinkedBuildingID, nLinkedZoneID, strDescription, LinkedSop.TableName, strPKFieldName, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private LinkedSop GetLinkedSop(int nFacilityTypeID, int nDisasterCategoryID, int nSubDisasterCategoryID, string strDisasterName, int? nLinkedBuildingID, int? nLinkedZoneID, string strDescription, string tableName, string strPKFieldName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            string strCondition = string.Format("{0} < {1} order by {0} desc", strPKFieldName, id);

            List<LinkedSop> datas = m_dataManager.GetSelectManager().SelectLinkedSops(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (LinkedSop data in datas)
            {
                if (IsSameLinkedSop(data, nFacilityTypeID, nDisasterCategoryID, nSubDisasterCategoryID, strDisasterName, nLinkedBuildingID, nLinkedZoneID, strDescription))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetLinkedSop(nFacilityTypeID, nDisasterCategoryID, nSubDisasterCategoryID, strDisasterName, nLinkedBuildingID, nLinkedZoneID, strDescription, tableName, strPKFieldName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameLinkedSop(LinkedSop sop, int nFacilityTypeID, int nDisasterCategoryID, int nSubDisasterCategoryID, string strDisasterName, int? nLinkedBuildingID, int? nLinkedZoneID, string strDescription)
        {
            if (sop.FacilityTypeID == nFacilityTypeID &&
                sop.DisasterCategoryID == nDisasterCategoryID &&
                sop.SubDisasterCategoryID == nSubDisasterCategoryID &&
                sop.DisasterName == strDisasterName &&
                sop.LinkedBuildingID == nLinkedBuildingID &&
                sop.LinkedZoneID == nLinkedZoneID &&
                sop.Description == strDescription)
                return true;

            return false;
        }

        public string GetErrorMessage()
        {
            return m_strErrorMessage;
        }

        private string GetInsertErrorMessage(string tableName)
        {
            return string.Format("{0} 테이블의 데이터 삽입에 실패하였습니다.", tableName);
        }

        private bool IsSameFloatData(float? data1, float? data2)
        {
            if (data1 == null && data2 == null)
                return true;

            if (data1 != null && data2 != null)
            {
                return IsSameFloatData2((float)data1, (float)data2);
            }

            return false;
        }

        private bool IsSameFloatData2(float data1, float data2)
        {
            if (System.Math.Abs(data1 - data2) < UnE.Geometry.Math.HALF_TOLERANCE())
                return true;

            return false;
        }

        private bool IsSameList<DataType>(List<DataType> list1, List<DataType> list2)
        {
            if (list1 == null && list2 == null)
                return true;

            if (list1 != null && list2 != null)
            {
                int count1 = list1.Count;
                int count2 = list2.Count;

                if (count1 != count2)
                    return false;

                for (int i = 0; i < count1; i++)
                {
                    DataType data1 = list1[i];
                    DataType data2 = list2[i];

                    if (data1.Equals(data2) == false)
                        return false;
                }

                return true;
            }

            return false;
        }

        private bool IsSameReceiverList(List<Receiver> list1, List<Receiver> list2)
        {
            if (list1 == null && list2 == null)
                return true;

            if (list1 != null && list2 != null)
            {
                int count1 = list1.Count;
                int count2 = list2.Count;

                if (count1 != count2)
                    return false;

                for (int i = 0; i < count1; i++)
                {
                    Receiver data1 = list1[i];
                    Receiver data2 = list2[i];

                    if (data1.TeamID != data2.TeamID || data1.TeamType != data2.TeamType)
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}
