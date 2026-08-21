using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace section
{
    public class SectionTabPage : System.Windows.Forms.TabPage
    {

        private ArrayList m_arrSectionTree = new ArrayList();
        public System.Collections.ArrayList SectionTreeList
        {
            get { return m_arrSectionTree; }
            set { m_arrSectionTree = value; }
        }
        private int dx = 0;
        private int dy = 0;

        private int s_dx = 0;
        private int s_dy = 0;

        private Point ptMouseDown = new Point(); //MouseDown 좌표
        private Point ptMouseUp = new Point(); //MouseUp 좌표

        private bool bSelected = false; //선택영역 확인

        private SectionTree m_treeCurrent = null;
        public SectionTree TreeCurrent
        {
            get { return m_treeCurrent; }
            set { m_treeCurrent = value; }
        }

        private SectionTree m_treeRoot = null;
        public SectionTree TreeRoot
        {
            get { return m_treeRoot; }
            set { m_treeRoot = value; }
        }
       

        public SectionTabPage()
        {
            this.DoubleBuffered = true;
            m_treeRoot = new SectionTree(this);
            m_arrSectionTree.Add(m_treeRoot); //사각형들 저장하는 배열
            m_treeCurrent = m_treeRoot;
            m_treeCurrent.Parent = null;

            this.Paint += DrawSection;
            this.MouseMove += FormMain_MouseMove;
            this.MouseUp += FormMain_MouseUP;
            this.MouseDoubleClick += FormMain_MouseDoubleClick;
            this.MouseDown += FormMain_MouseDown;
        }

        private void DrawSection(object sender, PaintEventArgs e)
        {
            foreach (SectionTree n in m_arrSectionTree)
            {
                n.DrawRect(e.Graphics);
                n.DrawLine(e.Graphics, m_treeRoot);
            }
        }

        public void AddSection(object sender, EventArgs e)
        {
            SectionTree tempTree = new SectionTree(this);

            tempTree.SetLocation(m_treeCurrent.Rect.Location.X, m_treeCurrent.Rect.Location.Y + m_treeCurrent.Rect.Size.Height + 30, tempTree.Rect.Size.Width, tempTree.Rect.Size.Height); //사각형
            tempTree.Parent = m_treeCurrent;

            m_treeCurrent.ChildList.Add(tempTree); //자식은 여러개므로 배열로  

            m_arrSectionTree.Add(tempTree);
        }

        private void FormMain_MouseMove(object sender, MouseEventArgs e) //마우스 이동시
        {
            //마우스 커서 변경
            if (e.Button == MouseButtons.None) //
            {
                //  if (m_treeCurrent != null)
                {
                    int nResult = m_treeCurrent.CheckResizeMode(e.X, e.Y);
                    switch (nResult)
                    {
                        case 1:
                            this.Cursor = Cursors.SizeNWSE;
                            break;
                        case 2:
                            this.Cursor = Cursors.SizeNESW;
                            break;
                        case 3:
                            this.Cursor = Cursors.SizeNESW;
                            break;
                        case 4:
                            this.Cursor = Cursors.SizeNWSE;
                            break;
                        case 5:
                            this.Cursor = Cursors.SizeNS;
                            break;
                        case 6:
                            this.Cursor = Cursors.SizeWE;
                            break;
                        case 7:
                            this.Cursor = Cursors.SizeWE;
                            break;
                        case 8:
                            this.Cursor = Cursors.SizeNS;
                            break;
                        default:
                            this.Cursor = Cursors.Arrow;
                            break;
                    }
                }
            }

            if (e.Button == MouseButtons.Left) //드래그상태면
            {
                // if (m_treeCurrent != null)
                {
                    if (bSelected == true)
                    {
                        if (m_treeCurrent.IsResizeMode()) //모서리에있는 도형 클릭했을경우 크기조절
                        {
                            s_dx = e.X - ptMouseDown.X;
                            s_dy = e.Y - ptMouseDown.Y;

                            Size currentSize = m_treeCurrent.GetSize();

                            int flag = m_treeCurrent.Getflag();

                            //리턴된 크기값으로 사이즈 조절, 사각형 크기조절을 실시간으로 보여줄 수 있게
                            switch (flag)
                            {
                                case 1:
                                    m_treeCurrent.SetTempSize(-s_dx, -s_dy, e.X, e.Y);
                                    break;
                                case 2:
                                    m_treeCurrent.SetTempSize(s_dx, -s_dy, e.X, e.Y);
                                    break;
                                case 3:
                                    m_treeCurrent.SetTempSize(-s_dx, s_dy, e.X, e.Y);
                                    break;
                                case 4:
                                    m_treeCurrent.SetTempSize(s_dx, s_dy, e.X, e.Y);
                                    break;
                                case 5:
                                    m_treeCurrent.SetTempSize(-s_dx, -s_dy, e.X, e.Y);
                                    break;
                                case 6:
                                    m_treeCurrent.SetTempSize(-s_dx, s_dy, e.X, e.Y);
                                    break;
                                case 7:
                                    m_treeCurrent.SetTempSize(s_dx, s_dy, e.X, e.Y);
                                    break;
                                case 8:
                                    m_treeCurrent.SetTempSize(s_dx, s_dy, e.X, e.Y);
                                    break;
                                default:
                                    m_treeCurrent.SetTempSize(s_dx, s_dy, e.X, e.Y);
                                    break;
                            }

                            PageHome.Instance.RedrawPanel();
                        }
                        //크기조절이 아니면 드래그 가능
                        else
                        {
                            //선택영역이면
                            ptMouseUp.X = (e.X + dx);
                            ptMouseUp.Y = (e.Y + dy);

                            m_treeCurrent.SetLocation(ptMouseUp.X, ptMouseUp.Y, m_treeCurrent.Rect.Size.Width, m_treeCurrent.Rect.Size.Height);
                            //m_treeCurrent.EdgeLocation(n.Rect.X, n.Rect.Y);
                            Invalidate();
                        }
                    }
                }
            }
        }

        public void FormMain_MouseDoubleClick(object sender, MouseEventArgs e) //마우스 더블클릭시
        {
            if (m_treeCurrent.Selected(e.X, e.Y) && e.Button == MouseButtons.Left)
                m_treeCurrent.textBox1.Enabled = true;
        }

        public void FormMain_MouseDown(object sender, MouseEventArgs e) //마우스로 컨트롤 클릭시
        {
            //처음 좌표를 기억
            ptMouseDown.X = e.X;
            ptMouseDown.Y = e.Y;

            m_arrSectionTree.Reverse(); //나중에 만들어진 사각형부터 드래그 가능하게

            bSelected = false;

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                foreach (SectionTree n in m_arrSectionTree)
                {
                    if (n.Selected(e.X, e.Y))
                    {
                        dx = n.Rect.Location.X - ptMouseDown.X;
                        dy = n.Rect.Location.Y - ptMouseDown.Y;

                        bSelected = true;
                        m_treeCurrent = n;
                        break;
                    }
                }
            }
            m_treeCurrent.textBox1.Enabled = false;
            m_arrSectionTree.Reverse(); //배열 순서 원래대로

            Refresh();
        }

        private void FormMain_MouseUP(object sender, MouseEventArgs e) //컨트롤에서 마우스를 놓았을떄
        {
            if (m_treeCurrent != null && m_treeCurrent.IsResizeMode() == true)
            {
                s_dx = e.X - ptMouseDown.X;
                s_dy = e.Y - ptMouseDown.Y;

                Size currentSize = m_treeCurrent.GetSize(); //현재 사각형의 크기값을 리턴

                int flag = m_treeCurrent.Getflag();//flag값 받아옴

                //리턴된 크기값으로 사이즈 조절
                switch (flag)
                {
                    case 1:
                        m_treeCurrent.SetSize(currentSize.Width - s_dx, currentSize.Height - s_dy, e.X, e.Y);
                        break;
                    case 2:
                        m_treeCurrent.SetSize(currentSize.Width + s_dx, currentSize.Height - s_dy, e.X, e.Y);
                        break;
                    case 3:
                        m_treeCurrent.SetSize(currentSize.Width - s_dx, currentSize.Height + s_dy, e.X, e.Y);
                        break;
                    case 4:
                        m_treeCurrent.SetSize(currentSize.Width + s_dx, currentSize.Height + s_dy, e.X, e.Y);
                        break;
                    case 5:
                        m_treeCurrent.SetSize(currentSize.Width - s_dx, currentSize.Height - s_dy, e.X, e.Y);
                        break;
                    case 6:
                        m_treeCurrent.SetSize(currentSize.Width - s_dx, currentSize.Height + s_dy, e.X, e.Y);
                        break;
                    case 7:
                        m_treeCurrent.SetSize(currentSize.Width + s_dx, currentSize.Height + s_dy, e.X, e.Y);
                        break;
                    case 8:
                        m_treeCurrent.SetSize(currentSize.Width + s_dx, currentSize.Height + s_dy, e.X, e.Y);
                        break;
                    default:
                        m_treeCurrent.SetSize(currentSize.Width + s_dx, currentSize.Height + s_dy, e.X, e.Y);
                        break;
                }
                m_treeCurrent.ResizeMode = false;
            }

            if (bSelected == true)
            {                
                foreach (SectionTree n in m_arrSectionTree)
                {
                    if (n.Selected(e.X, e.Y))
                    {
                        Point ptClick = PointToScreen(new Point(e.X, e.Y)); //특정 화면 지점의 위치를 클라이언트 좌표로 계산
                        m_treeCurrent = n;

                        if (e.Button == MouseButtons.Right)
                            PageHome.Instance.ContextMenu.Show(ptClick);
                        break;
                    }
                }                
            }

            if (bSelected == true && m_treeCurrent != null)
            {
                PageHome.Instance.PropertyForm.SetValue(m_treeCurrent);
            }
            Invalidate();
        }

        public SectionTree FindParent(int nId)
        {
            foreach (SectionTree n in m_arrSectionTree)
            {
                if (n.ID == nId)
                    return n;
            }
            return null;
        }
    }
}
