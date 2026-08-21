using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;


namespace section
{
    public class SectionTree 
    {
        private int m_nID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        private int m_nPrentID = -1;

        public int ParentID
        {
            get { return m_nPrentID; }
            set { m_nPrentID = value; }
        }

        private Rectangle r_Rect;
        public Rectangle Rect
        {
            get { return r_Rect; }
            set { r_Rect = value;}
        }

        //사각형 너비
        private int r_Width = 78;
        public int Width
        {
            get { return r_Width; }
            set { r_Width = value; }
        }


        //사각형 높이
        private int r_Height = 30;
        public int Height
        {
            get { return r_Height; }
            set { r_Height = value; }
        }

        //모서리에그릴 사각형1
        private Rectangle r_edge1;
        public Rectangle Edge1
        {
            get { return r_edge1; }
            set { r_edge1 = value; }
        }

        //모서리에그릴 사각형2
        private Rectangle r_edge2;
        public Rectangle Edge2
        {
            get { return r_edge2; }
            set { r_edge2 = value; }
        }

        //모서리에그릴 사각형3
        private Rectangle r_edge3;
        public Rectangle Edge3
        {
            get { return r_edge3; }
            set { r_edge3 = value; }
        }

        //모서리에그릴 사각형4
        private Rectangle r_edge4;
        public Rectangle Edge4
        {
            get { return r_edge4; }
            set { r_edge4 = value; }
        }

        //사이즈 조절 원
        private Rectangle r_circle1;
        public Rectangle Circle1
        {
            get { return r_circle1; }
            set { r_circle1 = value; }
        }

        //사이즈 조절 원2
        private Rectangle r_circle2;
        public Rectangle Circle2
        {
            get { return r_circle2; }
            set { r_circle2 = value; }
        }

        //사이즈 조절 원3
        private Rectangle r_circle3;
        public Rectangle Circle3
        {
            get { return r_circle3; }
            set { r_circle3 = value; }
        }

        //사이즈 조절 원4
        private Rectangle r_circle4;
        public Rectangle Circle4
        {
            get { return r_circle4; }
            set { r_circle4 = value; }
        }

        //사각형 너비
        private int e_Width = 8;
        public int E_Width
        {
            get { return e_Width; }
            set { e_Width = value; }
        }


        //사각형 높이
        private int e_Height = 8;
        public int E_Height
        {
            get { return e_Height; }
            set { e_Height = value; }
        }


        private string text="testText";
        public string Text
        {
            get { return text; }
            set { text = value; }
        }


        //부모노드
        private SectionTree r_Parent = null;
        public SectionTree Parent
        {
            get { return r_Parent; }
            set { r_Parent = value; }
        }
        
        //자식노드
        ArrayList r_child = new ArrayList();
        public ArrayList ChildList
        {
            get { return r_child; }
            set { r_child = value; }
        }

        //사각형번호
        private int number = 0;
        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        
        private System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
        public  System.Windows.Forms.TextBox textBox1
        {
            get { return textBox; }
            set { textBox = value; }
        }

        public SectionTree(TabPage panel) //생성자
        {
            Rect = new Rectangle(300, 200, r_Width, r_Height);

            textBox.SetBounds(300+5 , 200+5 , r_Width-10  , r_Height-10 );
            textBox.Multiline = true;
            textBox.Text = "test Text";
            textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            textBox.Enabled = false;
            //textBox.Visible = false;
            //textBox.ReadOnly = true;

            panel.Controls.Add(textBox);      
           

        }

        private bool m_bSelected = false;
        public bool BSelected
        {
            get { return m_bSelected; }
            set { m_bSelected = value; }
        }
        
        public bool Selected(int x, int y)
        {
            if (m_bResizeMode == true)
            {
                bool b1 = r_edge1.Contains(x, y);
                bool b2 = r_edge2.Contains(x, y);
                bool b3 = r_edge3.Contains(x, y);
                bool b4 = r_edge4.Contains(x, y);
                bool b5 = r_circle1.Contains(x, y);
                bool b6 = r_circle2.Contains(x, y);
                bool b7 = r_circle3.Contains(x, y);
                bool b8 = r_circle4.Contains(x, y);
                if (b1 || b2 || b3 || b4 || b5 || b6 || b7 || b8)
                {
                    m_bSelected = true;
                    return true;
                }               
            }
            if (r_Rect.Contains(x, y))
            {
                m_bSelected = true;
                return true;
            }
            if (textBox.Focused == true)
            {
                return true;
            }
            m_bSelected = false;
            return false;
        }

        private bool m_bResizeMode = false;

        public bool ResizeMode
        {
            get { return m_bResizeMode; }
            set { m_bResizeMode = value; }
        }
        public bool IsResizeMode()
        {
            return m_bResizeMode;
        }

        private int flag = 0;
        public int Getflag()
        {
            return flag;
        }

        public int CheckResizeMode(int x, int y)
        {
            if (m_bSelected == false)
            {
                m_bResizeMode = false;
                return 0;
            }

            if (r_edge1.Contains(x, y))
            {
                flag = 1;
                m_bResizeMode = true;
                return 1;
            }
            else if (r_edge2.Contains(x, y))
            {
                flag = 2;
                m_bResizeMode = true;
                return 2;
            }
            else if (r_edge3.Contains(x, y))
            {
                flag = 3;
                m_bResizeMode = true;
                return 3;
            }
            else if (r_edge4.Contains(x, y))
            {
                flag = 4;
                m_bResizeMode = true;
                return 4;
            }
            else if (r_circle1.Contains(x, y))
            {
                flag = 5;
                m_bResizeMode = true;
                return 5;
            }
            else if (r_circle2.Contains(x, y))
            {
                flag = 6;
                m_bResizeMode = true;
                return 6;
            }
            else if (r_circle3.Contains(x, y))
            {
                flag = 7;
                m_bResizeMode = true;
                return 7;
            }
            else if (r_circle4.Contains(x, y))
            {
                flag = 8;
                m_bResizeMode = true;
                return 8;
            }
            else
            {
                flag = 0;
                m_bResizeMode = false;
                return 0;
            }
        }

        public void SetLocation(int x, int y, int width, int height)
        {
            r_Width = width;
            r_Height = height;
            r_Rect = new Rectangle(x, y, r_Width, r_Height);
            //Point text_xy = PointToScreen(new Point(x, y));
            textBox.SetBounds(x+5 , y+5 , r_Width - 10, r_Height - 10);
            Rect = r_Rect;

            EdgeLocation(x, y, r_Width, r_Height);
        }

        public Size GetSize()
        {
            return new Size(r_Width, r_Height);
        }

     

        public void SetTempSize(int dx, int dy, int current_x, int current_y)
        {
            int width = r_Width + dx;
            int height = r_Height + dy;
            if (width < 10)
                width = 10;
            if (height < 10)
                height = 10;

            int x = Rect.Location.X;
            int y = Rect.Location.Y;
            int tmp_Width = 0;
            int tmp_Height = 0;
           
                
            if (flag == 1)
            {
                tmp_Width = width;
                tmp_Height = height;                
                r_Rect = new Rectangle(current_x, current_y, tmp_Width, tmp_Height);
                textBox.SetBounds(current_x + 5, current_y + 5, tmp_Width - 10, tmp_Height - 10);
                EdgeLocation(current_x, current_y, tmp_Width, tmp_Height);
            }
            else if (flag == 2)
            {
                tmp_Width = width;
                tmp_Height = height;
                r_Rect = new Rectangle(x, current_y, tmp_Width, tmp_Height);
                textBox.SetBounds(x + 5, current_y + 5, tmp_Width - 10, tmp_Height - 10);
                EdgeLocation(x, current_y, tmp_Width, tmp_Height);
            }
            else if (flag == 3)
            {
                tmp_Width = width;
                tmp_Height = height;
                r_Rect = new Rectangle(current_x, y, tmp_Width, tmp_Height);
                textBox.SetBounds(current_x + 5, y + 5, tmp_Width - 10, tmp_Height - 10);
                EdgeLocation(current_x, y, tmp_Width, tmp_Height);
            }
            else if (flag == 4)
            {
                tmp_Width = width;
                tmp_Height = height;
                r_Rect = new Rectangle(x, y, tmp_Width, tmp_Height);
                textBox.SetBounds(x + 5, y + 5, tmp_Width - 10, tmp_Height - 10);
                EdgeLocation(x, y, tmp_Width, tmp_Height);
            }
            else if (flag == 5)
            {
                tmp_Width = r_Width;
                tmp_Height = height;
                r_Rect = new Rectangle(x, current_y, tmp_Width, tmp_Height);
                textBox.SetBounds(x + 5, y + 5, tmp_Width - 10, tmp_Height - 10);
                EdgeLocation(x, current_y, tmp_Width, tmp_Height);
            }
            else if (flag == 6)
            {
                tmp_Width = width;
                tmp_Height = r_Height;
                r_Rect = new Rectangle(current_x, y, tmp_Width, tmp_Height);
                textBox.SetBounds(current_x + 5, y + 5, tmp_Width - 10, tmp_Height - 10);
                EdgeLocation(current_x, y, tmp_Width, tmp_Height);
            }
            else if (flag == 7)
            {
                tmp_Width = width;
                tmp_Height = r_Height;
                r_Rect = new Rectangle(x, y, tmp_Width, tmp_Height);
                textBox.SetBounds(x + 5, y + 5, tmp_Width - 10, tmp_Height - 10);
                EdgeLocation(x, y, tmp_Width, tmp_Height);
            }
            else if (flag == 8)
            {
                tmp_Width = r_Width;
                tmp_Height = height;
                r_Rect = new Rectangle(x, y, tmp_Width, tmp_Height);
                textBox.SetBounds(x + 5, y + 5, tmp_Width - 10, tmp_Height - 10);
                EdgeLocation(x, y, tmp_Width, tmp_Height);
            }
        }
        
        public void SetSize(int width, int height ,int current_x, int current_y)
        {
            if (width < 10)
                width = 10;
            if (height < 10)
                height = 10;

            int x = Rect.Location.X;
            int y = Rect.Location.Y;
            if (flag == 1)
            {
                r_Width = width;
                r_Height = height;
                r_Rect = new Rectangle(current_x, current_y, r_Width, r_Height);
                textBox.SetBounds(current_x + 5, current_y + 5, r_Width - 10, r_Height - 10);
                EdgeLocation(current_x, current_y, r_Width, r_Height);
            }
            else if (flag == 2)
            {
                r_Width = width;
                r_Height = height;
                r_Rect = new Rectangle(x, current_y, r_Width, r_Height);
                textBox.SetBounds(x + 5, current_y + 5, r_Width - 10, r_Height - 10);
                EdgeLocation(x, current_y, r_Width, r_Height);
            }
            else if (flag == 3)
            {
                r_Width = width;
                r_Height = height;
                r_Rect = new Rectangle(current_x, y, r_Width, r_Height);
                textBox.SetBounds(current_x + 5, y + 5, r_Width - 10, r_Height - 10);
                EdgeLocation(current_x, y, r_Width, r_Height);
            }
            else if (flag == 4)
            {
                r_Width = width;
                r_Height = height;
                r_Rect = new Rectangle(x, y, r_Width, r_Height);
                textBox.SetBounds(x + 5, y + 5, r_Width - 10, r_Height - 10);
                EdgeLocation(x, y, r_Width, r_Height);
            }
            else if (flag == 5)
            {
                //r_Width = width;


                r_Height = height;

                r_Rect = new Rectangle(x , current_y, r_Width, r_Height);
                textBox.SetBounds(x + 5, current_y + 5, r_Width - 10, r_Height - 10);
                EdgeLocation(x, current_y, r_Width, r_Height);
            }
            else if (flag == 6)
            {
                r_Width = width;
                //r_Height = height;
                r_Rect = new Rectangle(current_x, y, r_Width, r_Height);
                textBox.SetBounds(current_x + 5, y + 5, r_Width - 10, r_Height - 10);
                EdgeLocation(current_x, y, r_Width, r_Height);
            }
            else if (flag == 7)
            {
                r_Width = width;
                //r_Height = height;
                r_Rect = new Rectangle(x, y, r_Width, r_Height);
                textBox.SetBounds(x + 5, y + 5, r_Width - 10, r_Height - 10);
                EdgeLocation(x, y, r_Width, r_Height);
            }
            else if (flag == 8)
            {
                //r_Width = width;
                r_Height = height;
                r_Rect = new Rectangle(x, y, r_Width, r_Height);
                textBox.SetBounds(x + 5, y + 5, r_Width - 10, r_Height - 10);
                EdgeLocation(x, y, r_Width, r_Height);
            }

            
        }

        public void EdgeLocation(int x, int y, int width, int height)
        {
            r_edge1 = new Rectangle(x-3, y-3, e_Width, e_Height);
            Edge1 = r_edge1;

            r_edge2 = new Rectangle(x + width-3, y-3 , e_Width, e_Height);
            Edge2 = r_edge2;

            r_edge3 = new Rectangle(x - 3, y - 3 + height, e_Width, e_Height);
            Edge3 = r_edge3;

            r_edge4 = new Rectangle(x + width - 3, y + height - 3, e_Width, e_Height);
            Edge4 = r_edge4;

            r_circle1 = new Rectangle(x + (width / 2) - 3, y - 3, 8, 8);
            Edge4 = r_edge4;

            r_circle2 = new Rectangle(x - 3, y + (height / 2) - 3, 8, 8);
            Edge4 = r_edge4;

            r_circle3 = new Rectangle(x + width - 3, y + (height / 2) - 3, 8, 8);
            Edge4 = r_edge4;

            r_circle4 = new Rectangle(x + (width / 2) - 3, y + height - 3, 8, 8);
            Edge4 = r_edge4;

        }

        //Font F1 = new Font("굴림", 10);

        public void DrawRect(Graphics g)
        { 
            g.DrawRectangle(Pens.Black, r_Rect);

            if (m_bSelected == true)
                DrawEdge(g);

            //StringFormat stringFormat = new StringFormat();
            //stringFormat.Alignment = StringAlignment.Center; //가로 가운데정렬
            //stringFormat.LineAlignment = StringAlignment.Center; //세로 가운데정렬

            //g.DrawString(text, F1 , Brushes.Black, r_Rect, stringFormat); 
        }

        //private bool isDraw = true;

        //public bool IsDraw
        //{
        //    get { return isDraw; }
        //    set { isDraw = value; }
        //}


        //사각형, 선 그려주는 메소드
        public void DrawLine(Graphics g, SectionTree Parent)
        {
            foreach (SectionTree Child in Parent.ChildList)
            {
                    g.DrawLine(Pens.Black, Parent.GetPoint1(), Child.GetPoint2());
                    DrawLine(g, Child);
            }
        }


        public void DrawEdge(Graphics g)
        {
            g.FillRectangle(Brushes.Blue, r_edge1);
            g.FillRectangle(Brushes.Blue, r_edge2);
            g.FillRectangle(Brushes.Blue, r_edge3);
            g.FillRectangle(Brushes.Blue, r_edge4);

            g.FillEllipse(Brushes.Blue, r_circle1);
            g.FillEllipse(Brushes.Blue, r_circle2);
            g.FillEllipse(Brushes.Blue, r_circle3);
            g.FillEllipse(Brushes.Blue, r_circle4);
        }

        public void AddChild(SectionTree child)
        {
            r_child.Add(child);
        }

        public Point GetPoint1()
        {
            return new Point(Rect.Location.X + (r_Width/2) , Rect.Location.Y+r_Height);
        }
        public Point GetPoint2()
        {
            return new Point(Rect.Location.X + (r_Width / 2), Rect.Location.Y);
        }
     }
}