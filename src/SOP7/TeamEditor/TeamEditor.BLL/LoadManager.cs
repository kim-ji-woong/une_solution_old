using dnsDBUtil;
using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.BLL.Models.Data;
using TeamEditor.BLL.Models.Response;
using TeamEditor.IDAL;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL
{
    public class LoadManager
    {
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public IDataManager m_dataManager = null;
        public LoadManager(IDataManager dataManager)
        {
            m_dataManager = dataManager;
        }

        public List<RegularMember> LoadRegularMember()
        {
            string strErrorMessage;
            List<RegularMember> regularMembers = m_dataManager.GetSelectManager().SelectRegularMembers(out strErrorMessage);
            foreach (RegularMember item in regularMembers)
            {
                if (item.PhoneNumber != null)
                    item.PhoneNumber = DecryptString(item.PhoneNumber);
            }

            return regularMembers;
        }

        public ResponseRegularMembers LoadRegularMembers()
        {
            string strErrorMessage;
            ResponseRegularMembers response = new ResponseRegularMembers();

            //Dictionary<RegularMember.Fields, object> dicConditions = new Dictionary<RegularMember.Fields, object>();
            //List<RegularMember> regularMembers = m_dataManager.GetSelectManager().GetRegularMember(dicConditions, out strErrorMessage);//SelectRegularMembers
            string strCondition = "";
            List<RegularMember> regularMembers = m_dataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);

            if (regularMembers == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            foreach (RegularMember item in regularMembers)
            {
                if (item.PhoneNumber != null)
                    item.PhoneNumber = DecryptString(item.PhoneNumber);
            }

            response.Success = true;
            response.RegularMembers = regularMembers;

            return response;
        }

        public ResponseRegulars LoadRegulars()
        {
            string strErrorMessage;
            ResponseRegulars response = new ResponseRegulars();

            Dictionary<Regular.Fields, object> dicConditions = new Dictionary<Regular.Fields, object>();
            List<Regular> regulars = m_dataManager.GetSelectManager().SelectRegulars(dicConditions, out strErrorMessage);

            if (regulars == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            response.Success = true;
            response.Regulars = regulars;

            return response;
        }

        public ResponseTemporaryMembers LoadTemporaryMembers()
        {
            string strErrorMessage;
            ResponseTemporaryMembers response = new ResponseTemporaryMembers();

            Dictionary<TemporaryMember.Fields, object> dicConditions = new Dictionary<TemporaryMember.Fields, object>();
            List<TemporaryMember> temporaryMembers = m_dataManager.GetSelectManager().SelectTemporaryMembers(dicConditions, out strErrorMessage);
            if (temporaryMembers == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            List<TemporaryMemberInfo> temporaryMemberInfos = new List<TemporaryMemberInfo>();

            foreach (TemporaryMember member in temporaryMembers)
            {
                TemporaryMemberInfo temporaryMemberInfo = new TemporaryMemberInfo();
                temporaryMemberInfo.ID = member.ID;
                temporaryMemberInfo.DisplaySOPName = member.DisplaySOPName;
                temporaryMemberInfo.IsNormal = member.IsNormal;
                temporaryMemberInfo.Role = member.Role;
                
                Temporary temporary = m_dataManager.GetSelectManager().SelectTemporary(member.TeamID, out strErrorMessage);
                if (temporary == null)
                {
                    response.Success = false;
                    response.Message = strErrorMessage;
                    return response;
                }

                temporaryMemberInfo.Temporary = temporary;

                if (member.RegularID != null && member.RegularID != -1)
                {
                    Regular regular = m_dataManager.GetSelectManager().SelectRegular((int)member.RegularID, out strErrorMessage);
                    if (regular == null)
                    {
                        response.Success = false;
                        response.Message = strErrorMessage;
                        return response;
                    }

                    temporaryMemberInfo.Regular = regular;
                }

                if (member.RegularMemberID != null && member.RegularMemberID != -1)
                {
                    RegularMember regularMember = m_dataManager.GetSelectManager().SelectRegularMember((int)member.RegularMemberID, out strErrorMessage);
                    if (regularMember == null)
                    {
                        response.Success = false;
                        response.Message = strErrorMessage;
                        return response;
                    }

                    temporaryMemberInfo.RegularMember = regularMember;
                }

                temporaryMemberInfos.Add(temporaryMemberInfo);
            }

            response.Success = true;
            response.TemporaryMemberInfos = temporaryMemberInfos;

            return response;
        }

        public List<Options> LoadJobLevel()
        {
            string strErrorMessage;
            string strSQL = " PropertyName = 'JobLevel'";
            List<Options> options = m_dataManager.GetSelectManager().SelectOptions(strSQL, out strErrorMessage);

            return options;
        }

        public List<Options> LoadJobPosition()
        {
            string strErrorMessage;
            string strSQL = " PropertyName = 'JobPosition'";
            List<Options> options = m_dataManager.GetSelectManager().SelectOptions(strSQL, out strErrorMessage);

            return options;
        }

        public static string EncryptString(string str)
        {
            return AES256Cipher.AES_encrypt(str, key);
        }

        public static string DecryptString(string str)
        {
            return AES256Cipher.AES_decrypt(str, key);
        }
    }
}
