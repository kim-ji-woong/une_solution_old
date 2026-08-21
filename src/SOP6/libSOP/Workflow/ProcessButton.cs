using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using UnE.SOP.Workstate;
using Sections;

using System.Drawing.Drawing2D;

namespace UnE
{
    namespace SOP
    {
        namespace Workstate
        {
            public class ProcessButton
            {
                public enum ButtonStatus { WAIT = 0, WAIT_MOUSE_OVER, CANCEL, DONE, NOT_USE, UNKNOWN };

                protected ButtonData m_data = new ButtonData();
                protected float m_fDiameter = 16.0f;
                protected float m_fDelta = 0.0f;

                protected ButtonStatus m_status = ButtonStatus.UNKNOWN;
                protected Arrow.ArrowPosition m_pos = Arrow.ArrowPosition.NONE;

                protected RectangleF m_rectArea = new RectangleF();

                protected ProcessButtonManager m_parent = null;

                protected bool m_isReadOnly = false;

                protected static Pen m_pen = new Pen(Color.Black);
                protected static SolidBrush m_initBrush = new SolidBrush(Color.FromArgb(210, 210, 210));
                protected static SolidBrush m_waitBrush = new SolidBrush(Color.FromArgb(255, 217, 255));
                protected static Color m_colorWaitMouseOverTop = Color.FromArgb(205, 206, 208);
                protected static Color m_colorWaitMouseOverBottom = Color.FromArgb(106, 103, 104);
                protected static SolidBrush m_cancelBrush = new SolidBrush(Color.Yellow);
                protected static SolidBrush m_doneBrush = new SolidBrush(Color.FromArgb(154, 213, 247));
                protected static SolidBrush m_textBrush = new SolidBrush(Color.Black);

                protected static SolidBrush m_rectBrush = new SolidBrush(Color.FromArgb(172, 157, 243));

                protected IWorkflowContainer m_procMain = null;
                public IWorkflowContainer ProcMain
                {
                    get { return m_procMain; }
                    //set { m_procMain = value; }
                }

                public ProcessButton(IWorkflowContainer procMain)
                {
                    m_procMain = procMain;
                    m_fDelta = (float)(m_fDiameter / 2 - m_fDiameter / 2 * System.Math.Cos(System.Math.PI / 4));
                }

                public virtual void Draw(System.Drawing.Graphics g, Section section)
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

                public virtual bool HitTest(float x, float y)
                {
                    if (m_isReadOnly)
                        return false;

                    if (x >= m_rectArea.Left && x <= m_rectArea.Right &&
                        y >= m_rectArea.Top && y <= m_rectArea.Bottom)
                        return true;

                    return false;
                }

                public static ProcessDirectionHistory ToProcessDirection(Arrow.ArrowPosition pos)
                {
                    if (pos == Arrow.ArrowPosition.TOP)
                        return ProcessDirectionHistory.TOP;
                    else if (pos == Arrow.ArrowPosition.LEFT)
                        return ProcessDirectionHistory.LEFT;
                    else if (pos == Arrow.ArrowPosition.RIGHT)
                        return ProcessDirectionHistory.RIGHT;
                    else if (pos == Arrow.ArrowPosition.BOTTOM)
                        return ProcessDirectionHistory.BOTTOM;

                    return ProcessDirectionHistory.NONE;
                }

                public Section GetNextSection(Section section)
                {
                    foreach (Arrow arrow in section.Arrows)
                    {
                        if (arrow.BeginLink == section && arrow.BeginPosition == this.m_pos)
                        {
                            return arrow.EndLink;
                        }
                    }

                    return null;
                }
                public virtual bool IsStartButton()
                {
                    return false;
                }
                public virtual bool EnableClick()
                {
                    if (ProcMain == null)
                        return false;

                    Section section = Parent.Section;
                    if (!ProcMain.IsWorkingMode(section))
                        return false;

                    Section.ComponentType type = section.GetComponentType();

                    if (type == Section.ComponentType.INTERNAL || type == Section.ComponentType.EXTERNAL
                        || type == Section.ComponentType.TRANSMISSION)
                    {
                        // 내부, 외부 상황전파는 실행중일 경우에만 완료 버튼을 누를 수 있다.
                        SOPScenario sopSC = ProcMain.GetCurrentSOPScenario();
                        if (sopSC != null)
                        {
                            WorkFlow workFlow = WorkFlowManager.Instance.Get(sopSC.ActionStepID, sopSC.RealMode);
                            SectionState state = workFlow.FindState(section);
                            if (state == null)
                                return false;

                            //if (state.State == State.RUN)
                                return true;
                            //else
                             //   return false;
                        }
                        else
                            return false;
                    }

                    return true;
                }

                public virtual void OnClick()
                {
                    if (m_isReadOnly)
                        return;

                    if (m_status == ButtonStatus.WAIT || m_status == ButtonStatus.WAIT_MOUSE_OVER)
                    {
                        if (!EnableClick())
                            return;

                        // Section이 이미 완료 상태일 경우는 다시 완료 상태를 만들지 않고 ProcessDirections 정보만 수정한다.
                        PanelSection panel = (PanelSection)m_parent.Section.GetParent();

                        bool isDecision = m_parent.Section.GetComponentType() == Section.ComponentType.DECISION;
                        Section sectionNext = null;

                        if (isDecision)
                            sectionNext = GetNextSection(m_parent.Section);

                        if (panel.CompleteSection(m_parent.Section, (int)ToProcessDirection(m_pos), false, sectionNext))
                        {
                            Status = ButtonStatus.DONE;

                            // 판단의 경우 판단 Component 완료와 동시에 다음 Section을 실행시킨다.
                            if (m_parent.Section.GetComponentType() == Section.ComponentType.DECISION)
                            {
                                RunNextSection(m_parent.Section, panel);

                                // 자신 이외의 나머지 버튼들은 모두 사용 안함으로 만든다.
                                m_parent.SetAllButtonsStatus(ButtonStatus.NOT_USE, this);
                            }
                            sectionNext = GetNextSection(m_parent.Section);
                            ProcMain.FocusSection(sectionNext);
                            panel.Refresh();
                        }
                    }
                }

                // 현재 Section의 다음 Section을 실행시킨다.
                protected virtual void RunNextSection(Section sectionCurrent, PanelSection panel)
                {
                    foreach (Arrow arrow in sectionCurrent.Arrows)
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

                    foreach (Arrow arrow in m_data.Arrows)
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

                public Arrow.ArrowPosition Position
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

            public class ProcessButtonRect : ProcessButton
            {
                private int m_nWidth = 80;
                private int m_nHeight = 25;
                private bool m_bProcessSection = false;
                private string m_szBtnText = "임무실행";
                protected static Font TEXT_FONT = new Font("맑은고딕", 9, FontStyle.Bold);
                public ProcessButtonRect(IWorkflowContainer procMain, bool bPrcess)
                    : base(procMain)
                {
                    m_bProcessSection = bPrcess;
                    if (m_bProcessSection != true)
                    {
                        m_szBtnText = "전송";
                    }
                }

                private bool m_bStartEnd = false;

                public override bool IsStartButton()
                {
                    if (m_bStartEnd == true && m_szBtnText == "시작")
                        return true;
                    return false;
                }
                
                public void SetStartButtn(bool bStart)
                {
                    if (bStart == true)
                        m_szBtnText = "시작";
                    else
                        m_szBtnText = "종료";

                    m_bStartEnd = true;
                }
                
                public override void Draw(System.Drawing.Graphics g, Section section)
                {
                    PointF pt = section.Position;
                    SizeF size = section.RectSize;

                    PointF rectLoc = new PointF(pt.X + size.Width, pt.Y);
                    PanelSection pane = section.GetParent();

                    Point ptScr = pane.GlobalToScreen2(rectLoc);
                    Point newTL = new Point(ptScr.X + 3, ptScr.Y);
                    Point newBR = new Point(ptScr.X + 3, ptScr.Y);
                    PointF btnLoc = pane.ScreenToGlobal(newTL);
                    PointF btnBR = new PointF(btnLoc.X + m_nWidth, btnLoc.Y + m_nHeight);
                    
                    btnLoc.X += 10;

                    if (m_bStartEnd == false)
                        btnLoc.Y += m_nHeight + 10;
                    m_rectArea = new RectangleF(btnLoc.X, btnLoc.Y, m_nWidth, m_nHeight);

                    //pane
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.StartFigure();

                        path.AddLine(btnLoc.X, btnLoc.Y, btnLoc.X + m_nWidth, btnLoc.Y);
                        path.AddLine(btnLoc.X + m_nWidth, btnLoc.Y, btnLoc.X + m_nWidth, btnLoc.Y + m_nHeight);
                        path.AddLine(btnLoc.X + m_nWidth, btnLoc.Y + m_nHeight, btnLoc.X, btnLoc.Y + m_nHeight);
                        path.AddLine(btnLoc.X, btnLoc.Y + m_nHeight, btnLoc.X, btnLoc.Y);

                        path.CloseFigure();

                        DrawShadow(g, path);

                        if (m_status == ButtonStatus.WAIT)
                        {
                            Color color = section.GetColor(Section.ColorTarget.FILL);
                            m_waitBrush.Color = color;
                            g.FillPath(m_waitBrush, path);

                        }
                        else if (m_status == ButtonStatus.WAIT_MOUSE_OVER)
                        {
                            System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
                            brush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;
                            System.Drawing.Drawing2D.ColorBlend clrBlend = new System.Drawing.Drawing2D.ColorBlend(3);
                            clrBlend.Colors = new Color[] { Color.Transparent, m_colorWaitMouseOverTop, m_colorWaitMouseOverBottom };
                            clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };
                            brush.InterpolationColors = clrBlend;

                            g.FillPath(brush, path);
                            brush.Dispose();
                            m_textBrush.Color = Color.White;
                        }
                        else if (m_status == ButtonStatus.CANCEL)
                            g.FillPath(m_cancelBrush, path);
                        else if (m_status == ButtonStatus.DONE)
                            g.FillPath(m_doneBrush, path);
                        else if (m_status == ButtonStatus.NOT_USE)
                        {
                            g.FillPath(m_rectBrush, path);
                        }
                        else
                            return;
                        // 항상 두께 1로 처리
                        m_pen.Width = 0;
                        g.DrawPath(m_pen, path);


                        Point ptStrLoc = new Point((int)m_rectArea.X + 15, (int)m_rectArea.Y + 7);
                        if( m_szBtnText == "임무실행")
                        {
                            ptStrLoc.X -= 4;
                        }
                        else if (m_szBtnText == "전송")
                        {
                            ptStrLoc.X += 12;
                        }
                        else if (m_szBtnText == "시작")
                        {
                            ptStrLoc.X += 12;
                        }
                        else if (m_szBtnText == "종료")
                        {
                            ptStrLoc.X += 12;
                        }
                        g.DrawString(m_szBtnText, TEXT_FONT, m_textBrush, ptStrLoc);

                        if (m_status == ButtonStatus.WAIT_MOUSE_OVER)
                        {
                            m_textBrush.Color = Color.Black;
                        }
                    }
                }

                public override bool HitTest(float x, float y)
                {
                    if (m_isReadOnly)
                        return false;

                    if (x >= m_rectArea.Left && x <= m_rectArea.Right &&
                        y >= m_rectArea.Top && y <= m_rectArea.Bottom)
                        return true;

                    return false;
                }

                protected virtual void DrawShadow(Graphics g, System.Drawing.Drawing2D.GraphicsPath path)
                {
                    if (path == null)
                        return;
                    //float fMoveX = 10.0f, fMoveY = 10.0f;
                    float fMoveX = 5.0f, fMoveY = 4.0f;
                    g.TranslateTransform(fMoveX, fMoveY);

                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
                    brush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;

                    System.Drawing.Drawing2D.ColorBlend clrBlend = new System.Drawing.Drawing2D.ColorBlend(3);
                    clrBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(180, Color.DimGray), Color.FromArgb(180, Color.DimGray) };

                    clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };

                    brush.InterpolationColors = clrBlend;
                    g.FillPath(brush, path);

                    g.TranslateTransform(-fMoveX, -fMoveY);
                }

                protected ArrayList GetNextSections(Section section)
                {
                    ArrayList arList = new ArrayList();
                    foreach (Arrow arrow in section.Arrows)
                    {
                        if (arrow.BeginLink == section && arrow.EndLink != null)
                        {
                            if (!arList.Contains(arrow.EndLink))
                                arList.Add(arrow.EndLink);
                        }
                    }
                    return arList;
                }

                private bool IsWorkingActionStep(Section section)
                {
                    bool bRunningSOP = false;

                    PanelSection pane = section.GetParent();
                    if( pane != null)
                    {
                        UnE.SOP.Sections.SectionTabPage page = (UnE.SOP.Sections.SectionTabPage)pane.Parent;
                        if (page != null)
                        {
                            bRunningSOP = ProcMain.IsWorkingMode(page.ActionStepID, !page.VirtualMode);
                            
                        }
                        return bRunningSOP;
                    }
                    return bRunningSOP;
                }

                public override bool EnableClick()
                {
                    if (ProcMain == null)
                        return false;

                    Section section = Parent.Section;
                    if (!ProcMain.IsWorkingMode(section) && m_bStartEnd == false)
                        return false;

                    if(IsWorkingActionStep(section) == false && m_bStartEnd == true && m_szBtnText == "시작")
                        return true;      
                    if(IsWorkingActionStep(section) == true && m_bStartEnd == true && m_szBtnText == "시작")
                        return false;

                    if (IsWorkingActionStep(section) == true && m_bStartEnd == true && m_szBtnText == "종료")
                        return true;
                    if (IsWorkingActionStep(section) == false && m_bStartEnd == true && m_szBtnText == "종료")
                        return false;

                    Section.ComponentType type = section.GetComponentType();

                    if (type == Section.ComponentType.INTERNAL || type == Section.ComponentType.EXTERNAL
                        || type == Section.ComponentType.TRANSMISSION)
                    {
                        // 내부, 외부 상황전파는 실행중일 경우에만 완료 버튼을 누를 수 있다.
                        SOPScenario sopSC = ProcMain.GetCurrentSOPScenario();
                        if (sopSC != null)
                        {
                            WorkFlow workFlow = WorkFlowManager.Instance.Get(sopSC.ActionStepID, sopSC.RealMode);
                            SectionState state = workFlow.FindState(section);
                            if (state == null)
                                return false;

                            //if (state.State == State.RUN)
                                return true;
                            //else
                            //    return false;
                        }
                        else
                            return false;
                    }

                    return true;
                }

                public void OnStartClick()
                {
                    Section section = Parent.Section;
                    if( IsWorkingActionStep(section) == false)
                    {
                        if (ProcMain != null)
                        {
                            ProcMain.RunWorkflowWithEvent();
                        }
                    }                    
                }

                public override void OnClick()
                {
                    if (m_isReadOnly)
                        return;

                    if (m_status == ButtonStatus.WAIT || m_status == ButtonStatus.WAIT_MOUSE_OVER)
                    {
                        if (m_bStartEnd == true && m_szBtnText == "시작")
                        {
                            OnStartClick();
                            return;
                        }

                        if (!EnableClick())
                            return;

                        
                        if( m_parent.Section.GetComponentType() == Section.ComponentType.ENDPOINT)
                        {
                            base.OnClick();
                            return;
                        }

                        // Section이 이미 완료 상태일 경우는 다시 완료 상태를 만들지 않고 ProcessDirections 정보만 수정한다.
                        PanelSection panel = (PanelSection)m_parent.Section.GetParent();
                        bool isDecision = m_parent.Section.GetComponentType() == Section.ComponentType.DECISION;

                        ArrayList arNextSections = null;
                        //if (isDecision)
                        arNextSections = GetNextSections(m_parent.Section);

                        if (arNextSections == null || arNextSections.Count == 0)
                        {
                            // 다음 Section이 존재하지 않을 경우
                            if (panel.CompleteSection(m_parent.Section))
                                DoneSection(panel);
                        }
                        else
                        {
                            foreach (Section sectionNext in arNextSections)
                            {
                               
                                if (panel.CompleteSection(m_parent.Section, (int)ToProcessDirection(m_pos), false, sectionNext))
                                {
                                     if (sectionNext.GetComponentType() != Section.ComponentType.ENDPOINT)
                                         DoneSection(panel, sectionNext);
                                }
                                    
                            }
                        }
                    }
                }

                private void DoneSection(PanelSection panel, Section sectionNext = null)
                {
                    Status = ButtonStatus.DONE;
                    // 판단의 경우 판단 Component 완료와 동시에 다음 Section을 실행시킨다.
                    if (m_parent.Section.GetComponentType() == Section.ComponentType.DECISION)
                    {
                        RunNextSection(m_parent.Section, panel);

                        // 자신 이외의 나머지 버튼들은 모두 사용 안함으로 만든다.
                        m_parent.SetAllButtonsStatus(ButtonStatus.NOT_USE, this);
                    }

                    if (sectionNext != null)
                        ProcMain.FocusSection(sectionNext);

                    panel.Refresh();
                }

                // 현재 Section의 다음 Section을 실행시킨다.
                protected override void RunNextSection(Section sectionCurrent, PanelSection panel)
                {
                    foreach (Arrow arrow in sectionCurrent.Arrows)
                    {
                        if (arrow.BeginLink == sectionCurrent && arrow.BeginPosition == this.m_pos)
                        {
                            panel.RunSection(arrow.EndLink, false);
                            break;
                        }
                    }
                }
            }

            public class ButtonData
            {
                private Section m_section = null;
                private ArrayList m_arrArrows = new ArrayList();
                private Arrow.ArrowPosition m_position = Arrow.ArrowPosition.NONE;

                public Section Section
                {
                    get { return m_section; }
                    set { m_section = value; }
                }

                public ArrayList Arrows
                {
                    get { return m_arrArrows; }
                }

                public Arrow.ArrowPosition Position
                {
                    get { return m_position; }
                    set { m_position = value; }
                }
            }

            public class ProcessButtonManager : ISectionPainter
            {
                private Dictionary<Arrow.ArrowPosition, ProcessButton> m_dicButtons = new Dictionary<Arrow.ArrowPosition, ProcessButton>();
                private Section m_section = null;
                private bool m_isPrepared = false;

                public virtual ProcessButton FindButton(Arrow.ArrowPosition pos)
                {
                    if (m_dicButtons.ContainsKey(pos))
                        return m_dicButtons[pos];

                    return null;
                }

                public void SetButton(Arrow.ArrowPosition pos, ProcessButton btn)
                {
                    m_dicButtons[pos] = btn;

                    btn.Parent = this;
                }

                public virtual void Draw(Graphics g)
                {
                    PanelSection panel = (PanelSection)m_section.GetParent();

                    if (panel.DrawingProcessOption == 1)
                    {

                        foreach (KeyValuePair<Arrow.ArrowPosition, ProcessButton> pair in m_dicButtons)
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

                public virtual ProcessButton GetProcessButton(float x, float y)
                {
                    foreach (KeyValuePair<Arrow.ArrowPosition, ProcessButton> pair in m_dicButtons)
                    {
                        if (pair.Value.HitTest(x, y))
                            return pair.Value;
                    }
                    return null;
                }

                public virtual void SetAllButtonsStatus(ProcessButton.ButtonStatus btnStatus, ProcessButton except = null, SectionState state = null)
                {
                    foreach (KeyValuePair<Arrow.ArrowPosition, ProcessButton> pair in m_dicButtons)
                    {
                        if (pair.Value != except)
                            pair.Value.Status = btnStatus;
                    }

                    if (state != null)
                    {
                        if (btnStatus == ProcessButton.ButtonStatus.DONE)
                        {
                            ProcessButton btn = FindButton(Arrow.ArrowPosition.BOTTOM);
                            if (btn != null && btn != except)
                                state.ProcessDirections |= (int)ProcessDirectionHistory.BOTTOM;

                            btn = FindButton(Arrow.ArrowPosition.LEFT);
                            if (btn != null && btn != except)
                                state.ProcessDirections |= (int)ProcessDirectionHistory.LEFT;

                            btn = FindButton(Arrow.ArrowPosition.RIGHT);
                            if (btn != null && btn != except)
                                state.ProcessDirections |= (int)ProcessDirectionHistory.RIGHT;

                            btn = FindButton(Arrow.ArrowPosition.TOP);
                            if (btn != null && btn != except)
                                state.ProcessDirections |= (int)ProcessDirectionHistory.TOP;

                        }
                        else
                        {                            
                            ProcessButton btn = FindButton(Arrow.ArrowPosition.BOTTOM);
                            if (btn != null && btn != except)
                                state.ProcessDirections &= ~((int)ProcessDirectionHistory.BOTTOM);

                            btn = FindButton(Arrow.ArrowPosition.LEFT);
                            if (btn != null && btn != except)
                                state.ProcessDirections &= ~((int)ProcessDirectionHistory.LEFT);

                            btn = FindButton(Arrow.ArrowPosition.RIGHT);
                            if (btn != null && btn != except)
                                state.ProcessDirections &= ~((int)ProcessDirectionHistory.RIGHT);

                            btn = FindButton(Arrow.ArrowPosition.TOP);
                            if (btn != null && btn != except)
                                state.ProcessDirections &= ~((int)ProcessDirectionHistory.TOP);
                        }
                    }
                }

                public virtual Section Section
                {
                    get { return m_section; }
                    set { m_section = value; }
                }
            }

            public class ProcessRectButtonManager : ProcessButtonManager
            {                
                private Section m_section = null;
                private bool m_isPrepared = false;
                private ProcessButton m_Button = null;

                public override ProcessButton FindButton(Arrow.ArrowPosition pos)
                {
                    return FindButton();
                }

                public  ProcessButton FindButton()
                {
                    return m_Button;
                }

                public void SetButton(ProcessButton btn)
                {
                      m_Button = btn;
                    btn.Parent = this;
                }

                public override void Draw(Graphics g)
                {
                    PanelSection panel = (PanelSection)m_section.GetParent();

                    if (panel.DrawingProcessOption == 1)
                    {
                        m_Button.Draw(g, m_section);
                    }
                    else if (m_isPrepared == false)
                    {
                        // 나중에 그리기 위하여 Panel에 임시 저장한다.
                        panel.ProcessManagers.Add(this);
                        m_isPrepared = true;
                    }
                }

                public override ProcessButton GetProcessButton(float x, float y)
                {
                    if (m_Button.HitTest(x, y))
                    {
                        return m_Button;
                    }
                    return null;
                }

                public override void SetAllButtonsStatus(ProcessButton.ButtonStatus btnStatus, ProcessButton except = null, SectionState state = null)
                {
                    m_Button.Status = btnStatus;

                    if (state != null)
                    {
                        if (btnStatus == ProcessButton.ButtonStatus.DONE)
                        {
                            foreach (Arrow arrow in m_section.Arrows)
                            {
                                if (arrow.BeginLink == m_section)
                                {
                                    if (arrow.BeginPosition == Arrow.ArrowPosition.BOTTOM)
                                        state.ProcessDirections |= (int)ProcessDirectionHistory.BOTTOM;

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.LEFT)
                                        state.ProcessDirections |= (int)ProcessDirectionHistory.LEFT;

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.RIGHT)
                                        state.ProcessDirections |= (int)ProcessDirectionHistory.RIGHT;

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.TOP)
                                        state.ProcessDirections |= (int)ProcessDirectionHistory.TOP;
                                }
                            }
                        }
                        else
                        {
                            foreach (Arrow arrow in m_section.Arrows)
                            {
                                if (arrow.BeginLink == m_section)
                                {
                                    if (arrow.BeginPosition == Arrow.ArrowPosition.BOTTOM)
                                        state.ProcessDirections &= ~((int)ProcessDirectionHistory.BOTTOM);

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.LEFT)
                                        state.ProcessDirections &= ~((int)ProcessDirectionHistory.LEFT);

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.RIGHT)
                                        state.ProcessDirections &= ~((int)ProcessDirectionHistory.RIGHT);

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.TOP)
                                        state.ProcessDirections &= ~((int)ProcessDirectionHistory.TOP);
                                }
                            }
                        }
                    }
                }

                public override Section Section
                {
                    get { return m_section; }
                    set { m_section = value; }
                }
            }


            public class ConfirmButton
            {
                public enum ButtonStatus { WAIT = 0, WAIT_MOUSE_OVER, CANCEL, DONE, NOT_USE, UNKNOWN };

                protected ButtonData m_data = new ButtonData();
           
                protected ButtonStatus m_status = ButtonStatus.UNKNOWN;

                protected RectangleF m_rectArea = new RectangleF();

                protected ConfirmButtonManager m_parent = null;

                protected bool m_isReadOnly = false;

                protected static Pen m_pen = new Pen(Color.Black);
                protected static SolidBrush m_initBrush = new SolidBrush(Color.FromArgb(210, 210, 210));
                protected static SolidBrush m_waitBrush = new SolidBrush(Color.FromArgb(255, 217, 255));
                protected static Color m_colorWaitMouseOverTop = Color.FromArgb(205, 206, 208);
                protected static Color m_colorWaitMouseOverBottom = Color.FromArgb(106, 103, 104);
                protected static SolidBrush m_cancelBrush = new SolidBrush(Color.Yellow);
                protected static SolidBrush m_doneBrush = new SolidBrush(Color.FromArgb(154, 213, 247));
                protected static SolidBrush m_textBrush = new SolidBrush(Color.Black);
                protected static SolidBrush m_rectBrush = new SolidBrush(Color.FromArgb(172, 157, 243));

                protected IWorkflowContainer m_procMain = null;
                public IWorkflowContainer ProcMain
                {
                    get { return m_procMain; }
                }

                protected int m_nWidth = 80;
                protected int m_nHeight = 25;
                protected bool m_bProcessSection = false;
                protected string m_szBtnText = "임무확인";
                protected static Font TEXT_FONT = new Font("맑은고딕", 9, FontStyle.Bold);
                
                public ConfirmButton(IWorkflowContainer procMain, bool bPrcess)
                {
                    m_procMain = procMain;
                    m_bProcessSection = bPrcess;
                    if (m_bProcessSection != true)
                    {
                        m_szBtnText = "내용확인";
                    }
                }
                
                public virtual void Draw(System.Drawing.Graphics g, Section section)
                {
                    PointF pt = section.Position;
                    SizeF size = section.RectSize;

                    PointF rectLoc = new PointF(pt.X + size.Width, pt.Y);
                    PanelSection pane = section.GetParent();

                    Point ptScr = pane.GlobalToScreen2(rectLoc);
                    Point newTL = new Point(ptScr.X + 3, ptScr.Y);
                    Point newBR = new Point(ptScr.X + 3, ptScr.Y);
                    PointF btnLoc = pane.ScreenToGlobal(newTL);
                    PointF btnBR = new PointF(btnLoc.X + m_nWidth, btnLoc.Y + m_nHeight);
                    
                    btnLoc.X += 10;
                    m_rectArea = new RectangleF(btnLoc.X, btnLoc.Y, m_nWidth, m_nHeight);

                    //pane
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.StartFigure();

                        path.AddLine(btnLoc.X, btnLoc.Y, btnLoc.X + m_nWidth, btnLoc.Y);
                        path.AddLine(btnLoc.X + m_nWidth, btnLoc.Y, btnLoc.X + m_nWidth, btnLoc.Y + m_nHeight);
                        path.AddLine(btnLoc.X + m_nWidth, btnLoc.Y + m_nHeight, btnLoc.X, btnLoc.Y + m_nHeight);
                        path.AddLine(btnLoc.X, btnLoc.Y + m_nHeight, btnLoc.X, btnLoc.Y);

                        path.CloseFigure();

                        DrawShadow(g, path);

                        if (m_status == ButtonStatus.WAIT)
                        {
                            Color color = section.GetColor(Section.ColorTarget.FILL);
                            m_waitBrush.Color = color;
                            g.FillPath(m_waitBrush, path);

                        }
                        else if (m_status == ButtonStatus.WAIT_MOUSE_OVER)
                        {
                            System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
                            brush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;
                            System.Drawing.Drawing2D.ColorBlend clrBlend = new System.Drawing.Drawing2D.ColorBlend(3);
                            clrBlend.Colors = new Color[] { Color.Transparent, m_colorWaitMouseOverTop, m_colorWaitMouseOverBottom };
                            clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };
                            brush.InterpolationColors = clrBlend;

                            g.FillPath(brush, path);
                            brush.Dispose();
                            m_textBrush.Color = Color.White;
                        }
                        else if (m_status == ButtonStatus.CANCEL)
                            g.FillPath(m_cancelBrush, path);
                        else if (m_status == ButtonStatus.DONE)
                            g.FillPath(m_doneBrush, path);
                        else if (m_status == ButtonStatus.NOT_USE)
                        {
                            g.FillPath(m_rectBrush, path);
                        }
                        else
                            return;
                        // 항상 두께 1로 처리
                        m_pen.Width = 0;
                        g.DrawPath(m_pen, path);


                        Point ptStrLoc = new Point((int)m_rectArea.X + 15, (int)m_rectArea.Y + 7);
                        if( m_szBtnText == "임무실행")
                        {
                            ptStrLoc.X -= 4;
                        }
                        else if (m_szBtnText == "전송")
                        {
                            ptStrLoc.X += 14;
                        }
                        g.DrawString(m_szBtnText, TEXT_FONT, m_textBrush, ptStrLoc);

                        if (m_status == ButtonStatus.WAIT_MOUSE_OVER)
                        {
                            m_textBrush.Color = Color.Black;
                        }
                    }
                }

                public virtual bool HitTest(float x, float y)
                {
                    if (m_isReadOnly)
                        return false;

                    if (x >= m_rectArea.Left && x <= m_rectArea.Right &&
                        y >= m_rectArea.Top && y <= m_rectArea.Bottom)
                        return true;

                    return false;
                }

                protected virtual void DrawShadow(Graphics g, System.Drawing.Drawing2D.GraphicsPath path)
                {
                    if (path == null)
                        return;
                    //float fMoveX = 10.0f, fMoveY = 10.0f;
                    float fMoveX = 5.0f, fMoveY = 4.0f;
                    g.TranslateTransform(fMoveX, fMoveY);

                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
                    brush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;

                    System.Drawing.Drawing2D.ColorBlend clrBlend = new System.Drawing.Drawing2D.ColorBlend(3);
                    clrBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(180, Color.DimGray), Color.FromArgb(180, Color.DimGray) };

                    clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };

                    brush.InterpolationColors = clrBlend;
                    g.FillPath(brush, path);

                    g.TranslateTransform(-fMoveX, -fMoveY);
                }

                public virtual bool EnableClick()
                {
                    if (ProcMain == null)
                        return false;

                    Section section = Parent.Section;
                    if (!ProcMain.IsWorkingMode(section))
                        return false;

                    Section.ComponentType type = section.GetComponentType();
                    SOPScenario sopSC = ProcMain.GetCurrentSOPScenario();
                    if (sopSC != null)
                    {
                        WorkFlow workFlow = WorkFlowManager.Instance.Get(sopSC.ActionStepID, sopSC.RealMode);
                        SectionState state = workFlow.FindState(section);
                        if (state == null)
                            return false;

                        if (state.State != State.RUN)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;                    
                }

                public virtual void OnClick()
                {
                    if (m_isReadOnly)
                        return;

                    if (m_status == ButtonStatus.WAIT || m_status == ButtonStatus.WAIT_MOUSE_OVER)
                    {
                        if (!EnableClick())
                            return;

                        // Section이 이미 완료 상태일 경우는 다시 완료 상태를 만들지 않고 ProcessDirections 정보만 수정한다.
                        
                        PanelSection panel = (PanelSection)m_parent.Section.GetParent();
                        Status = ButtonStatus.DONE;
                        panel.RunSection(m_parent.Section, true);

                        ProcMain.FocusSection(m_parent.Section);
                        panel.Refresh();                          
                    }
                }

                public ButtonData Data
                {
                    get { return m_data; }
                    set { m_data = value; }
                }

                public ButtonStatus Status
                {
                    get { return m_status; }
                    set { m_status = value; }
                }

                public ConfirmButtonManager Parent
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

            public class ConfirmButtonManager : ISectionPainter
            {
                private Section m_section = null;
                private bool m_isPrepared = false;
                private ConfirmButton m_Button = null;

                public virtual ConfirmButton FindButton(Arrow.ArrowPosition pos)
                {
                    return FindButton();
                }

                public ConfirmButton FindButton()
                {
                    return m_Button;
                }

                public void SetButton(ConfirmButton btn)
                {
                    m_Button = btn;
                    btn.Parent = this;
                }

                public virtual void Draw(Graphics g)
                {
                    PanelSection panel = (PanelSection)m_section.GetParent();

                    if (panel.DrawingProcessOption == 1)
                    {
                        m_Button.Draw(g, m_section);
                    }
                    else if (m_isPrepared == false)
                    {
                        // 나중에 그리기 위하여 Panel에 임시 저장한다.
                        panel.ProcessManagers.Add(this);
                        m_isPrepared = true;
                    }
                }

                public virtual ConfirmButton GetProcessButton(float x, float y)
                {
                    if (m_Button.HitTest(x, y))
                    {
                        return m_Button;
                    }
                    return null;
                }

                public virtual void SetAllButtonsStatus(ConfirmButton.ButtonStatus btnStatus, ConfirmButton except = null, SectionState state = null)
                {
                    m_Button.Status = btnStatus;

                    if (state != null)
                    {
                        if (btnStatus == ConfirmButton.ButtonStatus.DONE)
                        {
                            foreach (Arrow arrow in m_section.Arrows)
                            {
                                if (arrow.BeginLink == m_section)
                                {
                                    if (arrow.BeginPosition == Arrow.ArrowPosition.BOTTOM)
                                        state.ProcessDirections |= (int)ProcessDirectionHistory.BOTTOM;

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.LEFT)
                                        state.ProcessDirections |= (int)ProcessDirectionHistory.LEFT;

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.RIGHT)
                                        state.ProcessDirections |= (int)ProcessDirectionHistory.RIGHT;

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.TOP)
                                        state.ProcessDirections |= (int)ProcessDirectionHistory.TOP;
                                }
                            }
                        }
                        else
                        {
                            foreach (Arrow arrow in m_section.Arrows)
                            {
                                if (arrow.BeginLink == m_section)
                                {
                                    if (arrow.BeginPosition == Arrow.ArrowPosition.BOTTOM)
                                        state.ProcessDirections &= ~((int)ProcessDirectionHistory.BOTTOM);

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.LEFT)
                                        state.ProcessDirections &= ~((int)ProcessDirectionHistory.LEFT);

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.RIGHT)
                                        state.ProcessDirections &= ~((int)ProcessDirectionHistory.RIGHT);

                                    if (arrow.BeginPosition == Arrow.ArrowPosition.TOP)
                                        state.ProcessDirections &= ~((int)ProcessDirectionHistory.TOP);
                                }
                            }
                        }
                    }
                }

                public virtual Section Section
                {
                    get { return m_section; }
                    set { m_section = value; }
                }
            }
        }
    }

}

