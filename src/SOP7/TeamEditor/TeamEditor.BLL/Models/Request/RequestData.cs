using System;
using System.Collections.Generic;
using TeamEditor.BLL.Models.Data;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.Models.Request
{
    public class RequestData
    {
        private bool? m_requestTemporaryMembers = null;
        private bool? m_requestRegularMembers = null;
        private bool? m_requestRegulars = null;

        private RequestUpdateRegularMember m_RequestUpdateRegularMember = null;

        public bool? RequestTemporaryMembers
        {
            get { return m_requestTemporaryMembers; }
            set { m_requestTemporaryMembers = value; }
        }

        public bool? RequestRegularMembers
        {
            get { return m_requestRegularMembers; }
            set { m_requestRegularMembers = value; }
        }

        public bool? RequestRegulars
        {
            get { return m_requestRegulars; }
            set { m_requestRegulars = value; }
        }

        public RequestUpdateRegularMember RequestUpdateRegularMember
        {
            get { return m_RequestUpdateRegularMember; }
            set { m_RequestUpdateRegularMember = value; }
        }
    }

    public class RequestUpdateRegularMember
    {
        private RegularMember m_member = null;
        public RegularMember Member
        {
            get { return m_member; }
            set { m_member = value; }
        }
    }

    public class RequestRemoveRegularMember
    {
        private List<RegularMember> m_members = null;
        public List<RegularMember> Members
        {
            get { return m_members; }
            set { m_members = value; }
        }
    }

    public class RequestUpdateTemporaryMember
    {
        private TemporaryMemberInfo m_temporaryMemberInfo = null;

        public TemporaryMemberInfo TemporaryMemberInfo
        {
            get { return m_temporaryMemberInfo; }
            set { m_temporaryMemberInfo = value; }
        }
    }

    public class RequestRemoveTemporaryMember
    {
        private List<TemporaryMember> m_members = null;
        public List<TemporaryMember> Members
        {
            get { return m_members; }
            set { m_members = value; }
        }
    }

    public class RequestUpdateRegularTeam
    {
        private Regular m_regularTeam = null;

        public Regular RegularTeam
        {
            get { return m_regularTeam; }
            set { m_regularTeam = value; }
        }
    }

    public class RequestRemoveRegularTeam
    {
        private List<int> m_teamIDs = null;
        public List<int> TeamIDs
        {
            get { return m_teamIDs; }
            set { m_teamIDs = value; }
        }
    }

    public class RequestUpdateTemporaryTeam
    {
        private Temporary m_temporaryTeam = null;

        public Temporary TemporaryTeam
        {
            get { return m_temporaryTeam; }
            set { m_temporaryTeam = value; }
        }
    }

    public class RequestRemoveTemporaryTeam
    {
        private List<int> m_teamIDs = null;
        public List<int> TeamIDs
        {
            get { return m_teamIDs; }
            set { m_teamIDs = value; }
        }
    }
}
