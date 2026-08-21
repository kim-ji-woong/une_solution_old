using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SOPManager
{
    internal class CustomMenuHelper
    {
        private IMenuCommandOwner mOwner = null;
       
        private Font m_MenuFont = null;
        public Font MenuFont
        {
            get { return m_MenuFont; }
            set { m_MenuFont = value; }
        }
        public CustomMenuHelper(IMenuCommandOwner owner)
        {
            mOwner = owner;
        }

        public void MakeCustomLookMenu(MenuStrip mainMenu, Color backColor, Color textColor)
        {
            // 커스텀 메뉴 렌더러를 생성
            CustomLookMenuRenderer renderer = new CustomLookMenuRenderer();
            renderer.BackColor = backColor;
            mainMenu.Renderer = renderer;
            if( m_MenuFont == null)
                m_MenuFont = new System.Drawing.Font("맑은 고딕", 9F);

            foreach (ToolStripItem menu in mainMenu.Items)
            {
                if ((typeof(ToolStripMenuItem)).IsAssignableFrom(menu.GetType()))
                {
                    ToolStripMenuItem item = (ToolStripMenuItem)menu;
                    item.BackColor = backColor;
                    item.ForeColor = textColor;
                    item.Size = new System.Drawing.Size(214, 30);
                    item.Font = m_MenuFont;
                    item.MouseEnter += MenuHint_Enter;
                    item.MouseLeave += MenuHint_Leave;


                    foreach (ToolStripItem menuItem in item.DropDownItems)
                    {
                        if ((typeof(ToolStripMenuItem)).IsAssignableFrom(menuItem.GetType()))
                        {
                            ToolStripMenuItem item2 = (ToolStripMenuItem)menuItem;
                            item2.BackColor = backColor;
                            item2.ForeColor = textColor;
                            item2.Size = new System.Drawing.Size(214, 30);
                            item2.Font = m_MenuFont;
                            item2.Click += new System.EventHandler(MenuItemClicked);
                            item2.CheckedChanged += new System.EventHandler(MenuItemChecked);
                            item2.MouseEnter += MenuHint_Enter;
                            item2.MouseLeave += MenuHint_Leave;

                        }
                    }
                }
            }
        }

        private void MenuHint_Enter(object sender, EventArgs e)
        {
            if (mOwner != null)
            {
                ToolStripStatusLabel label = mOwner.GetStatusLabel();
                if (label != null)
                {
                    label.Text = (sender as ToolStripMenuItem).ToolTipText;
                    label.ToolTipText = label.Text;
                }
                    
            }            
        }

        private void MenuHint_Leave(object sender, EventArgs e)
        {
            if (mOwner != null)
            {
                ToolStripStatusLabel label = mOwner.GetStatusLabel();
                if (label != null)
                {
                    label.Text = "";
                    label.ToolTipText = label.Text;
                }
            }  
        }


        private void MenuItemChecked(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(IDToolStripMenuItem))
            {
                IDToolStripMenuItem item = (IDToolStripMenuItem)sender;
                int nCommand = item.CommandID;
                if (nCommand > 0)
                {
                    if (mOwner != null)
                    {
                        bool bChecked = item.Checked;
                        mOwner.CheckedChanged(nCommand, bChecked);
                    }
                }
            }
        }

        private void MenuItemClicked(object sender, EventArgs e)
        {
            if( sender.GetType() == typeof(IDToolStripMenuItem))
            {
                IDToolStripMenuItem item = (IDToolStripMenuItem)sender;
                int nCommand = item.CommandID;
                if( nCommand > 0)
                {
                    if( mOwner != null)
                    {
                        if(item.CheckOnClick == false)
                            mOwner.RunCommand(nCommand);
                    }
                }
            }            
        }
    }
}
