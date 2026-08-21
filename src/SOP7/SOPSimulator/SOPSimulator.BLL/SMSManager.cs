using dnsDBUtil;
using dnsEmail;
using dnsSMS;
using SOPManager.Model.Sop.Component;
using SOPSimulator.BLL.Models.Request;
using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.Model.Sop.Team;

namespace SOPSimulator.BLL
{
    public class SMSManager
    {
        private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
        private ProcessManager m_processManager = null;
        public SMSManager(ProcessManager processManager)
        {
            this.m_processManager = processManager;
        }

        public bool ProgressInternalSpread(List<Receiver> receivers, string message)
        {
            if (m_processManager.SopSimulatorDataManager.SiteID == 12)
            {
                List<string> emails = GetEmail(receivers);
                if (emails != null && emails.Count > 0)
                    return SendSMS("", emails, message);
            }
            else
            {
                List<string> phoneNumbers = GetPhoneNumber(receivers);
                if (phoneNumbers != null && phoneNumbers.Count > 0)
                    return SendSMS("", phoneNumbers, message);
            }

            return false;
        }

        private List<string> GetPhoneNumber(List<Receiver> receivers)
        {
            List<string> phoneNumbers = new List<string>();

            if (receivers == null)
                return phoneNumbers;

            foreach (Receiver receiver in receivers)
            {
                if (receiver.TeamType == 2)
                {
                    string strErrorMessage = null;
                    List<RegularMember> members = m_processManager.TeamDataManager.GetSelectManager().SelectRegularMembers("RegularID=" + receiver.TeamID, out strErrorMessage);
                    if (members != null)
                    {
                        foreach (RegularMember member in members)
                        {
                            if (member.PhoneNumber.Length > 0)
                            {
                                string phoneNumber = DecryptString(member.PhoneNumber);
                                phoneNumbers.Add(phoneNumber);
                            }
                        }
                    }                    
                }

            }

            return phoneNumbers;
        }

        private List<string> GetEmail(List<Receiver> receivers)
        {
            List<string> emails = new List<string>();

            if (receivers == null)
                return emails;

            foreach (Receiver receiver in receivers)
            {
                if (receiver.TeamType == 2)
                {
                    string strErrorMessage = null;
                    List<RegularMember> members = m_processManager.TeamDataManager.GetSelectManager().SelectRegularMembers("RegularID=" + receiver.TeamID, out strErrorMessage);
                    if (members != null)
                    {
                        foreach (RegularMember member in members)
                        {
                            if (member.Email.Length > 0)
                            {                                
                                emails.Add(member.Email);
                            }
                        }
                    }
                }

            }

            return emails;
        }

        public bool SendSMS(string strCaller, List<string> strPhoneNumbers, string message)
        {
            bool returnValue = false;

            IMessageClient client = MessageClientFactory.CreateMessageClient(m_processManager.CommonDataManager, m_processManager.SdmsManager);
            if (client != null)
            {
                MessageContent content = new MessageContent();
                content.Caller = strCaller;
                if (m_processManager.SopSimulatorDataManager.SiteID == 12)
                    content.EMails.AddRange(strPhoneNumbers);
                else
                    content.PhoneNumbers.AddRange(strPhoneNumbers);
                content.Message = message;
                //content.SensorReactionHistoryID = 7; // 카톡 test

                //if (client.SendSMS(content))
                //    return true;

                returnValue = client.SendSMS(content);
            }            

            return returnValue;
        }

        public string EncryptString(string str)
        {
            return AES256Cipher.AES_encrypt(str, key);
        }

        public string DecryptString(string str)
        {
            return AES256Cipher.AES_decrypt(str, key);
        }
    }
}
