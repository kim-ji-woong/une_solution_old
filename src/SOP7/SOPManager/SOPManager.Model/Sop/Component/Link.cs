using System.Collections.Generic;

namespace SOPManager.Model.Sop.Component
{
    public class Link : Section
    {
        public enum Fields { ID, GridID, GridRowIndex, GridColumnIndex, Width, Height, Text, ComponentID, LinkedComponentIDList, StepMemberID, VAlign, HAlign, FontName, FontStyle, FontSize, LineSpace, FontColor, SectionNumber };

        private string m_strText = "";
        private List<string> m_linkedComponentIDList = new List<string>();

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public List<string> LinkedComponentIDList
        {
            get { return m_linkedComponentIDList; }
        }

        public static string TableName
        {
            get { return "SopComponentLink"; }
        }

        public override int ComponentType
        {
            get { return (int)SectionType.Link; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.VAlign ||
                field == Fields.HAlign ||
                field == Fields.FontName ||
                field == Fields.FontStyle ||
                field == Fields.FontSize ||
                field == Fields.LineSpace ||
                field == Fields.FontColor ||
                field == Fields.SectionNumber)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
