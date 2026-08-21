using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.Model.Sop.Team
{
    /// <summary>
    /// SopTeamRegular
    /// SopTeamRegularMember
    /// SopTeamTemporary
    /// SopTeamTemporaryMember
    /// </summary>
    public class RegularmemberTemporarymember
    {
        public int TemporaryMemberID { get; set; }
        public int TemporaryID { get; set; }
        public string TemporaryName { get; set; }
        public int? Role { get; set; }
        public bool IsNormal { get; set; }
        public string DisplaySOPName { get; set; }
        public int RegularID { get; set; }
        public string RegularName { get; set; }
        public int RegularMemberID { get; set; }
        public string RegularMemberName { get; set; }
    }
}
