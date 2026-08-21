using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPGen
{
    public partial class FlowMap : Form
    {
        private ArrayList m_arrSection = new ArrayList();
        private Section m_sectionSelected = null;
        private bool m_clickedLButton = false;
        private Point m_ptClicked = new Point();
        private Point m_ptSelected = new Point();
        
        public FlowMap()
        {
            InitializeComponent();

            m_arrSection.Add(new Section(this, 300, 300));

            this.DoubleBuffered = true;
        }

        private void FlowMap_Paint(object sender, PaintEventArgs e)
        {
            foreach (Section section in m_arrSection)
            {
                section.Draw(e.Graphics);
            }
        }

        private bool SelectSection(int x, int y)
        {
            foreach (Section section in m_arrSection)
            {
                Section secsionSelected = section.Select(x, y);

                if (secsionSelected != null)
                {
                    if (m_sectionSelected != null)
                    {
                        if (m_sectionSelected != secsionSelected)
                        {
                            Invalidate(m_sectionSelected.InvalidateRectArea);
                        }
                        else
                        {
                            // 선택된 상태에서 다시 선택되었음을 알린다.
                            // 텍스트 편집이나 기타 기능을 수행할 수 있다.
                            m_sectionSelected.DoubleSelect(true);
                        }
                    }

                    secsionSelected.Select(true, m_arrSection);
                    m_sectionSelected = secsionSelected;
                    Invalidate(secsionSelected.InvalidateRectArea);
                    return true;
                }
            }

            if (m_sectionSelected != null)
            {
                m_sectionSelected.Select(false, m_arrSection);
                Invalidate(m_sectionSelected.InvalidateRectArea);
                m_sectionSelected = null;
            }

            return false;
        }

        private void FlowMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_clickedLButton)
            {
                if (m_sectionSelected != null)
                {
                    if (m_sectionSelected.GetChangeSizeOption() != EditBox.BoxPosition.NO_SELECT)
                    {
                        m_sectionSelected.ChangeSize(e.X, e.Y);
                    }
                    else
                    {
                        int xMove = e.X - m_ptClicked.X;
                        int yMove = e.Y - m_ptClicked.Y;

                        m_sectionSelected.Position = new Point(m_ptSelected.X + xMove, m_ptSelected.Y + yMove);
                        Rectangle rectOrigin = m_sectionSelected.InvalidateRectArea;

                        int nWidth = rectOrigin.Width;
                        int nHeight = rectOrigin.Height;
                        Rectangle rect = new Rectangle(rectOrigin.Left - nWidth, rectOrigin.Top - nHeight, nWidth * 3, nHeight * 3);

                        Invalidate();//rect);
                    }
                }
            }
            else
            {
                if (m_sectionSelected != null)
                {
                    m_sectionSelected.CheckMouse(e.X, e.Y);
                }
                else
                    this.Cursor = Cursors.Arrow;
            }
        }

        private void FlowMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_clickedLButton = true;
                m_ptClicked.X = e.X;
                m_ptClicked.Y = e.Y;

                if (m_sectionSelected != null && m_sectionSelected.GetChangeSizeOption() != EditBox.BoxPosition.NO_SELECT)
                {
                    m_sectionSelected.SetChangeSizeOriginPoint(e.X, e.Y);
                }
                else
                {
                    if (SelectSection(e.X, e.Y))
                        m_ptSelected = m_sectionSelected.Position;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (SelectSection(e.X, e.Y))
                {
                    m_ptSelected = m_sectionSelected.Position;
                    contextSectionMenu.Show(this, new Point(e.X, e.Y));
                }
            }
        }

        private void FlowMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_clickedLButton = false;

                if (m_sectionSelected != null)
                    m_sectionSelected.DoubleSelect(false);
            }
        }

        private void sectionAdd_Click(object sender, EventArgs e)
        {
            if (m_sectionSelected != null)
            {
                Section newSection = new Section(this);

                Point ptSelected = m_sectionSelected.Position;
                Size sizeSelected = m_sectionSelected.RectSize;

                newSection.Position = new Point(ptSelected.X, ptSelected.Y + sizeSelected.Height + newSection.RectSize.Height);
                m_sectionSelected.AddChild(newSection);

                m_sectionSelected.Select(false, m_arrSection);
                m_sectionSelected = null;

                Invalidate();
                //m_arrSection.Add(newSection);

                //Invalidate(newSection.InvalidateRectArea);
            }
        }

        private void FlowMap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (m_sectionSelected != null)
                {
                    if (m_arrSection.Contains(m_sectionSelected))
                    {
                        // Root는 못지움.
                        return;
                        //m_sectionSelected.GetTextBox().Hide();
                        //m_arrSection.Remove(m_sectionSelected);
                    }
                    
                    Section sectionParent = m_sectionSelected.GetParentSection();

                    if (sectionParent != null)
                        sectionParent.RemoveChild(m_sectionSelected);

                    m_sectionSelected = null;
                    Invalidate();
                }
            }
        }
    }
}
