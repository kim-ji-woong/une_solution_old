using System;
using System.Collections.Generic;
using System.Text;

namespace Common.BLL.Models.Response
{
    public class ResponseAccountSettings : MessageResult
    {
        private ShortcutKey m_shortcutKey = null;
        private string m_strIdleTime = null;
        private Request.PopupState m_popupState = null;
        private string m_strTurnStart = null;
        private string m_strUseAlarmTurn = null;

        public ShortcutKey ShortcutKey
        {
            get { return m_shortcutKey; }
            set { m_shortcutKey = value; }
        }

        public string IdleTime
        {
            get { return m_strIdleTime; }
            set { m_strIdleTime = value; }
        }

        public Request.PopupState PopupState
        {
            get { return m_popupState; }
            set { m_popupState = value; }
        }

        public string TurnStart
        {
            get { return m_strTurnStart; }
            set { m_strTurnStart = value; }
        }

        public string UseAlarmTurn
        {
            get { return m_strUseAlarmTurn; }
            set { m_strUseAlarmTurn = value; }
        }
    }
}
