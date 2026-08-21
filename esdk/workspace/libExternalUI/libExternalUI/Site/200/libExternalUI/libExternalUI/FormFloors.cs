using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using DBUtility2;

namespace libExternalUI
{
    public partial class FormFloors : Form
    {
        private Mutex checking = new Mutex(false);
        private Control m_invokeCtrl = null;
        private UIManager m_uiManager = null;

        private static VariousData<int> m_selectedFloorIndex = null;

        public FormFloors(Control ctrlInvoke, UIManager uiManager)
        {
            InitializeComponent();
            m_invokeCtrl = ctrlInvoke;
            m_uiManager = uiManager;
        }

        private void FormFloors_Load(object sender, EventArgs e)
        {
            this.Size = new Size(86, 432);

            if (m_selectedFloorIndex != null)
            {
                ImageButtonEx btn = null;

                if (m_selectedFloorIndex.Data == 6)
                    btn = btn7F;
                else if (m_selectedFloorIndex.Data == 5)
                    btn = btn6F;
                else if (m_selectedFloorIndex.Data == 4)
                    btn = btn5F;
                else if (m_selectedFloorIndex.Data == 3)
                    btn = btn4F;
                else if (m_selectedFloorIndex.Data == 2)
                    btn = btn3F;
                else if (m_selectedFloorIndex.Data == 1)
                    btn = btn2F;
                else if (m_selectedFloorIndex.Data == 0)
                    btn = btn1F;
                else if (m_selectedFloorIndex.Data == -1)
                    btn = btnB1F;
                else if (m_selectedFloorIndex.Data == -2)
                    btn = btnB2F;
                else if (m_selectedFloorIndex.Data == -3)
                    btn = btnB3F;
                else if (m_selectedFloorIndex.Data == -4)
                    btn = btnB4F;
                else if (m_selectedFloorIndex.Data == -5)
                    btn = btnB5F;

                if (btn != null)
                    btn.ImageNormal = btn.ImageClicked;
            }
        }

        private void FormFloors_Leave(object sender, EventArgs e)
        {
            StartWaitingForClickFromOutside();
        }

        private void FormFloors_MouseLeave(object sender, EventArgs e)
        {
            StartWaitingForClickFromOutside();
        }

        private void FormFloors_Deactivate(object sender, EventArgs e)
        {
            StartWaitingForClickFromOutside();
        }

        private void FormFloors_VisibleChanged(object sender, EventArgs e)
        {
            StartWaitingForClickFromOutside();
        }

        private void StartWaitingForClickFromOutside()
        {
            try
            {
                if (checking.WaitOne(10))
                {
                    var ctx = new SynchronizationContext();

                    Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            Thread.Sleep(1);

                            if (MouseButtons == MouseButtons.Left)
                            {
                                if (IsFormInside() == false)
                                {
                                    ctx.Send(ClickFromOutside, null);
                                    break;
                                }
                            }
                        }

                        checking.ReleaseMutex();
                    });
                }
            }
            catch (Exception)
            {
            }
        }

        private bool IsFormInside()
        {
            if (btn7F.IsMouseOver || btn6F.IsMouseOver || btn5F.IsMouseOver || btn4F.IsMouseOver ||
                btn3F.IsMouseOver || btn2F.IsMouseOver || btn1F.IsMouseOver || btnB1F.IsMouseOver ||
                btnB2F.IsMouseOver || btnB3F.IsMouseOver || btnB4F.IsMouseOver || btnB5F.IsMouseOver)
                return true;

            return false;
        }

        private void ClickFromOutside(object state)
        {
            m_invokeCtrl.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }

        private void btn_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_uiManager != null)
            {
                VariousData<int> floorIndex = null;

                if (btn7F == sender)
                {
                    floorIndex = new VariousData<int>(6);
                    //m_uiManager.OnFloorClick(6);
                }
                else if (btn6F == sender)
                {
                    floorIndex = new VariousData<int>(5);
                    //m_uiManager.OnFloorClick(5);
                }
                else if (btn5F == sender)
                {
                    floorIndex = new VariousData<int>(4);
                    //m_uiManager.OnFloorClick(4);
                }
                else if (btn4F == sender)
                {
                    floorIndex = new VariousData<int>(3);
                    //m_uiManager.OnFloorClick(3);
                }
                else if (btn3F == sender)
                {
                    floorIndex = new VariousData<int>(2);
                    //m_uiManager.OnFloorClick(2);
                }
                else if (btn2F == sender)
                {
                    floorIndex = new VariousData<int>(1);
                    //m_uiManager.OnFloorClick(1);
                }
                else if (btn1F == sender)
                {
                    floorIndex = new VariousData<int>(0);
                    //m_uiManager.OnFloorClick(0);
                }
                else if (btnB1F == sender)
                {
                    floorIndex = new VariousData<int>(-1);
                    //m_uiManager.OnFloorClick(-1);
                }
                else if (btnB2F == sender)
                {
                    floorIndex = new VariousData<int>(-2);
                    //m_uiManager.OnFloorClick(-2);
                }
                else if (btnB3F == sender)
                {
                    floorIndex = new VariousData<int>(-3);
                    //m_uiManager.OnFloorClick(-3);
                }
                else if (btnB4F == sender)
                {
                    floorIndex = new VariousData<int>(-4);
                    //m_uiManager.OnFloorClick(-4);
                }
                else if (btnB5F == sender)
                {
                    floorIndex = new VariousData<int>(-5);
                    //m_uiManager.OnFloorClick(-5);
                }

                if (floorIndex != null)
                {
                    m_uiManager.OnFloorClick(floorIndex.Data);
                    m_selectedFloorIndex = floorIndex;
                }
            }

            this.Close();
        }

        public static void RemoveSelection()
        {
            m_selectedFloorIndex = null;
        }
    }

    public class ImageButtonEx : UnE.GUI.ImageButton
    {
        public bool IsMouseOver
        {
            get { return m_isMouseOver; }
        }
    }
}
