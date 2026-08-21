using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVSingleViewer.Command
{
    public class CommandManager
    {
        private List<Command> m_commands = new List<Command>();
        private int m_nIndex = -1;

        private RibbonButtonGroup m_btnUndo = null;
        private RibbonButtonGroup m_btnRedo = null;

        public CommandManager()
        {
            m_btnUndo = new RibbonButtonGroup(OnClickUndo);
            m_btnRedo = new RibbonButtonGroup(OnClickRedo);
        }

        public void AddButton(UnE.GUI.RibbonButton btn, bool isUndo)
        {
            if (btn != null)
            {
                if (isUndo)
                    m_btnUndo.AddButton(btn);
                else
                    m_btnRedo.AddButton(btn);
            }
        }

        public void AddCommand(Command cmd)
        {
            int nIndex = m_nIndex + 1;

            if (nIndex >= 0 && nIndex < m_commands.Count)
            {
                m_commands.RemoveRange(nIndex, m_commands.Count - nIndex);
                GC.Collect();
            }

            m_commands.Add(cmd);
            m_nIndex = m_commands.Count - 1;

            if (m_btnUndo != null)
                m_btnUndo.Enabled = true;

            if (m_btnRedo != null)
                m_btnRedo.Enabled = false;
        }

        public void Clear()
        {
            m_commands.Clear();
            m_nIndex = -1;

            if (m_btnUndo != null)
                m_btnUndo.Enabled = false;

            if (m_btnRedo != null)
                m_btnRedo.Enabled = false;

            GC.Collect();
        }

        public void Undo()
        {
            if (m_nIndex < 0 || m_nIndex >= m_commands.Count)
                return;

            Command cmd = m_commands[m_nIndex--];
            cmd.RollBack();

            if (m_btnUndo != null)
                m_btnUndo.Enabled = m_nIndex >= 0;

            if (m_btnRedo != null)
                m_btnRedo.Enabled = true;
        }

        public void Redo()
        {
            int nIndex = m_nIndex + 1;
            if (nIndex < 0 || nIndex >= m_commands.Count)
                return;

            Command cmd = m_commands[nIndex];
            cmd.Do();
            m_nIndex++;

            if (m_btnUndo != null)
                m_btnUndo.Enabled = true;

            if (m_btnRedo != null)
                m_btnRedo.Enabled = m_nIndex + 1 < m_commands.Count;
        }

        private void OnClickUndo(object sender, EventArgs e)
        {
            Undo();

            FormMain.Instance.SetGroupInfo();
        }

        private void OnClickRedo(object sender, EventArgs e)
        {
            Redo();

            FormMain.Instance.SetGroupInfo();
        }
    }
}
