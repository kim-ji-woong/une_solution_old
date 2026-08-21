using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.Model.Sop.Team
{
    public class RegularMember
    {
        public enum Fields { ID, RegularID, MemberName, MemberID, OfficePhoneNumber, PhoneNumber, JobLevelID, JobPositionID, Email, StatusID };
        // 정상근무, 휴직, 퇴사, 기타
        public enum WorkStatus { Normal = 0, Absence, Resign, ETC}

        public int ID { get; set; }
        public int RegularID { get; set; }
        public string MemberName { get; set; }
        public string MemberID { get; set; }
        public string OfficePhoneNumber { get; set; }
        public string PhoneNumber { get; set; }
        public int? JobLevelID { get; set; }
        public int? JobPositionID { get; set; }
        public string Email { get; set; }
        // WorkStatus
        // SOPTeamOptions 테이블에서 PropertyName이 'Status'인 것에 해당
        public int StatusID { get; set; }

        public static string GetTableName()
        {
            return "SopTeamRegularMember";
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.PhoneNumber ||
                field == Fields.OfficePhoneNumber ||
                field == Fields.JobLevelID ||
                field == Fields.JobPositionID ||
                field == Fields.Email ||
                field == Fields.MemberID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
