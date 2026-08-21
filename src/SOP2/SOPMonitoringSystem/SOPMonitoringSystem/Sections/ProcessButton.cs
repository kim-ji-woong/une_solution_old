using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace SOPMonitoringSystem
{
    public class ProcessButton
    {
        public enum ButtonStatus { WAIT = 0, WAIT_MOUSE_OVER, CANCEL, DONE, NOT_USE, UNKNOWN };

        private ButtonData m_data = new ButtonData();
        private float m_fDiameter = 16.0f;
        private float m_fDelta = 0.0f;

        private ButtonStatus m_status = ButtonStatus.UNKNOWN;
        private Sections.Arrow.ArrowPosition m_pos = Sections.Arrow.ArrowPosition.NONE;

        private RectangleF m_rectArea = new RectangleF();

        private ProcessButtonManager m_parent = null;

        private bool m_isReadOnly = false;

        private static Pen m_pen = new Pen(Color.Black);
        private static SolidBrush m_waitBrush = new SolidBrush(Color.White);
        private static Color m_colorWaitMouseOverTop = Color.FromArgb(205, 206, 208);
        private static Color m_colorWaitMouseOverBottom = Color.FromArgb(106, 103, 104);
        private static SolidBrush m_cancelBrush = new SolidBrush(Color.Yellow);
        private static SolidBrush m_doneBrush = new SolidBrush(Color.Black);

        public ProcessButton()
        {
            m_fDelta = (float)(m_fDiameter / 2 - m_fDiameter / 2 * System.Math.Cos(System.Math.PI / 4));
        }

        public void Draw(System.Drawing.Graphics g, Sections.Section section)
        {
            bool isSuccess;
            PointF ptPos = section.GetArrowPoint(m_pos, out isSuccess);

            ptPos.X -= m_fDiameter / 2;
            ptPos.Y -= m_fDiameter / 2;

            if (!isSuccess)
                return;

            m_rectArea.Location = ptPos;
            m_rectArea.Size = new SizeF(m_fDiameter, m_fDiameter);

            if (m_status == ButtonStatus.WAIT)
                g.FillEllipse(m_waitBrush, ptPos.X, ptPos.Y, m_fDiameter, m_fDiameter);
            else if (m_status == ButtonStatus.WAIT_MOUSE_OVER)
            {
                System.Drawing.Drawing2D.LinearGradientBrush waitMouseOverBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new PointF(ptPos.X + m_fDiameter / 2, ptPos.Y), new PointF(ptPos.X + m_fDiameter / 2, ptPos.Y + m_fDiameter), m_colorWaitMouseOverTop, m_colorWaitMouseOverBottom);
                g.FillEllipse(waitMouseOverBrush, ptPos.X, ptPos.Y, m_fDiameter, m_fDiameter);
                waitMouseOverBrush.Dispose();
            }
            else if (m_status == ButtonStatus.CANCEL)
                g.FillEllipse(m_cancelBrush, ptPos.X, ptPos.Y, m_fDiameter, m_fDiameter);
            else if (m_status == ButtonStatus.DONE)
                g.FillEllipse(m_doneBrush, ptPos.X, ptPos.Y, m_fDiameter, m_fDiameter);
            else if (m_status == ButtonStatus.NOT_USE)
            {
                g.FillEllipse(m_waitBrush, ptPos.X, ptPos.Y, m_fDiameter, m_fDiameter);

                g.DrawLine(m_pen, ptPos.X + m_fDelta, ptPos.Y + m_fDelta, ptPos.X + m_fDiameter - m_fDelta, ptPos.Y + m_fDiameter - m_fDelta);
                g.DrawLine(m_pen, ptPos.X + m_fDiameter - m_fDelta, ptPos.Y + m_fDelta, ptPos.X + m_fDelta, ptPos.Y + m_fDiameter - m_fDelta);
            }
            else
                return;

            g.DrawEllipse(m_pen, ptPos.X, ptPos.Y, m_fDiameter, m_fDiameter);
        }

        public bool HitTest(float x, float y)
        {
            if (m_isReadOnly)
                return false;

            if (x >= m_rectArea.Left && x <= m_rectArea.Right &&
                y >= m_rectArea.Top && y <= m_rectArea.Bottom)
                return true;

            return false;
        }

        public static Sections.ProcessDirection ToProcessDirection(Sections.Arrow.ArrowPosition pos)
        {
            if (pos == Sections.Arrow.ArrowPosition.TOP)
                return Sections.ProcessDirection.TOP;
            else if (pos == Sections.Arrow.ArrowPosition.LEFT)
                return Sections.ProcessDirection.LEFT;
            else if (pos == Sections.Arrow.ArrowPosition.RIGHT)
                return Sections.ProcessDirection.RIGHT;
            else if (pos == Sections.Arrow.ArrowPosition.BOTTOM)
                return Sections.ProcessDirection.BOTTOM;

            return Sections.ProcessDirection.NONE;
        }

        private Sections.Section GetNextSection(Sections.Section section)
        {
            foreach (Sections.Arrow arrow in section.Arrows)
            {
                if (arrow.BeginLink == section && arrow.BeginPosition == this.m_pos)
                {
                    return arrow.EndLink;
                }
            }

            return null;
        }

        public bool EnableClick()
        {
            Sections.Section section = Parent.Section;
            if (!FormMain.Instance.GetPageHome().IsWorkingMode(section))
                return false;

            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.INTERNAL || type == Sections.Section.ComponentType.EXTERNAL
                || type == Sections.Section.ComponentType.TRANSMISSION)
            {
                // 내부, 외부 상황전파는 실행중일 경우에만 완료 버튼을 누를 수 있다.
                int nActionStepID;
                bool isReal, isRegular, isNormal;
                string strSOPPath = SOPMonitoringSystem.FormMain.Instance.GetPageHome().GetDockScenario().GetCurrentSOPInfo(out nActionStepID, out isReal, out isRegular, out isNormal);

                if (nActionStepID > 0)
                {
                    Sections.WorkFlow workFlow = Sections.WorkFlowManager.Instance.Get(nActionStepID, isReal);
                    Sections.SectionState state = workFlow.FindState(section);

                    if (state == null)
                        return false;

                    if (state.State == Sections.State.RUN)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }

            return true;
        }

        public void OnClick()
        {
            if (m_isReadOnly)
                return;

            if (m_status == ButtonStatus.WAIT || m_status == ButtonStatus.WAIT_MOUSE_OVER)
            {
                if (!EnableClick())
                    return;

                // Section이 이미 완료 상태일 경우는 다시 완료 상태를 만들지 않고 ProcessDirections 정보만 수정한다.
                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)m_parent.Section.GetParent();

                bool isDecision = m_parent.Section.GetComponentType() == Sections.Section.ComponentType.DECISION;
                Sections.Section sectionNext = null;

                if (isDecision)
                    sectionNext = GetNextSection(m_parent.Section);

                if (panel.CompleteSection(m_parent.Section, (int)ToProcessDirection(m_pos), false, sectionNext))
                {
                    Status = ButtonStatus.DONE;

                    // 판단의 경우 판단 Component 완료와 동시에 다음 Section을 실행시킨다.
                    if (m_parent.Section.GetComponentType() == Sections.Section.ComponentType.DECISION)
                    {
                        RunNextSection(m_parent.Section, panel);

                        // 자신 이외의 나머지 버튼들은 모두 사용 안함으로 만든다.
                        m_parent.SetAllButtonsStatus(ButtonStatus.NOT_USE, this);
                    }
                    sectionNext = GetNextSection(m_parent.Section);
                    FormMain.Instance.FocusSection(sectionNext);
                    panel.Refresh();
                }
            }
        }

        // 현재 Section의 다음 Section을 실행시킨다.
        protected void RunNextSection(Sections.Section sectionCurrent, Sections.PanelSectionEx panel)
        {
            foreach (Sections.Arrow arrow in sectionCurrent.Arrows)
            {
                if (arrow.BeginLink == sectionCurrent && arrow.BeginPosition == this.m_pos)
                {
                    panel.RunSection(arrow.EndLink, false);
                    break;
                }
            }
        }

        protected void SetArrowLine(bool solidLine)
        {
            if (m_data == null)
                return;

            foreach (Sections.Arrow arrow in m_data.Arrows)
            {
                if (solidLine)
                {
                    //arrow.LineColor = Color.Black;
                    //arrow.LineThick = 3;
                    arrow.LineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    //arrow.FillColor = Color.Black;
                }
                else
                {
                    arrow.LineColor = Color.Gray;
                    //arrow.LineThick = 2;
                    arrow.LineStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    //arrow.FillColor = Color.Gray;
                }
            }
        }

        public ButtonData Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public Sections.Arrow.ArrowPosition Position
        {
            get { return m_pos; }
            set { m_pos = value; }
        }

        public ButtonStatus Status
        {
            get { return m_status; }
            set
            {
                m_status = value;

                if (m_status == ButtonStatus.DONE)
                    SetArrowLine(true);
                else
                    SetArrowLine(false);
            }
        }

        public float Diameter
        {
            get { return m_fDiameter; }
            set
            {
                m_fDiameter = value;
                m_fDelta = (float)(m_fDiameter / 2 - m_fDiameter / 2 * System.Math.Cos(System.Math.PI / 4));
            }
        }

        public ProcessButtonManager Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public bool ReadOnly
        {
            get { return m_isReadOnly; }
            set { m_isReadOnly = value; }
        }
    }

    public class ButtonData
    {
        private Sections.Section m_section = null;
        private ArrayList m_arrArrows = new ArrayList();
        private Sections.Arrow.ArrowPosition m_position = Sections.Arrow.ArrowPosition.NONE;

        public Sections.Section Section
        {
            get { return m_section; }
            set { m_section = value; }
        }

        public ArrayList Arrows
        {
            get { return m_arrArrows; }
        }

        public Sections.Arrow.ArrowPosition Position
        {
            get { return m_position; }
            set { m_position = value; }
        }
    }
    
    public class ProcessButtonManager : Sections.ISectionPainter
    {
        private Dictionary<Sections.Arrow.ArrowPosition, ProcessButton> m_dicButtons = new Dictionary<Sections.Arrow.ArrowPosition, ProcessButton>();
        private Sections.Section m_section = null;
        private bool m_isPrepared = false;

        public ProcessButton FindButton(Sections.Arrow.ArrowPosition pos)
        {
            if (m_dicButtons.ContainsKey(pos))
                return m_dicButtons[pos];

            return null;
        }

        public void SetButton(Sections.Arrow.ArrowPosition pos, ProcessButton btn)
        {
            m_dicButtons[pos] = btn;
            btn.Parent = this;
        }

        public void Draw(Graphics g)
        {
            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)m_section.GetParent();

            if (panel.DrawingProcessOption == 1)
            {
                foreach (KeyValuePair<Sections.Arrow.ArrowPosition, ProcessButton> pair in m_dicButtons)
                {
                    pair.Value.Draw(g, m_section);
                }
            }
            else if (m_isPrepared == false)
            {
                // 나중에 그리기 위하여 Panel에 임시 저장한다.
                panel.ProcessManagers.Add(this);
                m_isPrepared = true;
            }
        }

        public SOPMonitoringSystem.ProcessButton GetProcessButton(float x, float y)
        {
            foreach (KeyValuePair<Sections.Arrow.ArrowPosition, ProcessButton> pair in m_dicButtons)
            {
                if (pair.Value.HitTest(x, y))
                    return pair.Value;
            }

            return null;
        }

        public void SetAllButtonsStatus(ProcessButton.ButtonStatus btnStatus, ProcessButton except = null, Sections.SectionState state = null)
        {
            foreach (KeyValuePair<Sections.Arrow.ArrowPosition, ProcessButton> pair in m_dicButtons)
            {
                if (pair.Value != except)
                    pair.Value.Status = btnStatus;
            }

            if (state != null)
            {
                if (btnStatus == ProcessButton.ButtonStatus.DONE)
                {
                    ProcessButton btn = FindButton(Sections.Arrow.ArrowPosition.BOTTOM);
                    if (btn != null && btn != except)
                        state.ProcessDirections |= (int)Sections.ProcessDirection.BOTTOM;

                    btn = FindButton(Sections.Arrow.ArrowPosition.LEFT);
                    if (btn != null && btn != except)
                        state.ProcessDirections |= (int)Sections.ProcessDirection.LEFT;

                    btn = FindButton(Sections.Arrow.ArrowPosition.RIGHT);
                    if (btn != null && btn != except)
                        state.ProcessDirections |= (int)Sections.ProcessDirection.RIGHT;

                    btn = FindButton(Sections.Arrow.ArrowPosition.TOP);
                    if (btn != null && btn != except)
                        state.ProcessDirections |= (int)Sections.ProcessDirection.TOP;
                }
                else
                {
                    ProcessButton btn = FindButton(Sections.Arrow.ArrowPosition.BOTTOM);
                    if (btn != null && btn != except)
                        state.ProcessDirections &= ~((int)Sections.ProcessDirection.BOTTOM);

                    btn = FindButton(Sections.Arrow.ArrowPosition.LEFT);
                    if (btn != null && btn != except)
                        state.ProcessDirections &= ~((int)Sections.ProcessDirection.LEFT);

                    btn = FindButton(Sections.Arrow.ArrowPosition.RIGHT);
                    if (btn != null && btn != except)
                        state.ProcessDirections &= ~((int)Sections.ProcessDirection.RIGHT);

                    btn = FindButton(Sections.Arrow.ArrowPosition.TOP);
                    if (btn != null && btn != except)
                        state.ProcessDirections &= ~((int)Sections.ProcessDirection.TOP);
                }
            }
        }

        public Sections.Section Section
        {
            get { return m_section; }
            set { m_section = value; }
        }
    }
}
