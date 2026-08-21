using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX.Mathematics;
using SharpDX.DirectWrite;

namespace DXFViewer
{
    public class Text : DXFDotNet.Text, IDrawableShape
    {
        protected SharpDX.Vector2 mVecCenter = new SharpDX.Vector2();
        protected SharpDX.DirectWrite.TextLayout mTextLayout = null;
        protected SharpDX.DirectWrite.TextFormat mTextFormat = null;
        protected SharpDX.RectangleF mTextRect;
        public Text()
            : base()
        {
        }

        public DXFDotNet.Shape GetShapeObject()
        {
            return this;
        }

        public override DXFDotNet.Text CreateText()
        {
            Text arc = new Text();
            return arc;
        }

        public bool CreateDXResource()
        {
            if (m_pOwnLayer == null || m_pOwnLayer.Owner == null)
                return false;

            DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)m_pOwnLayer.Owner;
            SharpDX.DirectWrite.Factory factory = ctrl.DWriteFactory;

            if (factory == null)
                return false;

            // Set Center Vector
            mVecCenter.X = m_ptPos.X;
            mVecCenter.Y = m_ptPos.Y;

            // Set Text Format / Layout
            string szFontName = m_font.Name;
            float size = m_font.Size;

            TextAlignment vAlign = TextAlignment.Leading;
            if (VerticalAlignment == System.Drawing.StringAlignment.Center)
                vAlign = TextAlignment.Center;
            else if (VerticalAlignment == System.Drawing.StringAlignment.Near)
                vAlign = TextAlignment.Trailing;

            ParagraphAlignment hAlign = ParagraphAlignment.Near;
            if (HorizontalAlignment == System.Drawing.StringAlignment.Center)
                hAlign = ParagraphAlignment.Center;
            else if (HorizontalAlignment == System.Drawing.StringAlignment.Far)
                hAlign = ParagraphAlignment.Far;

            // Create Text Format
            mTextFormat = new TextFormat(factory, szFontName, FontWeight.Regular, FontStyle.Normal, size)
            {
                TextAlignment = vAlign,
                ParagraphAlignment = hAlign
            };

            System.Drawing.Graphics g2 = ctrl.CreateGraphics();
            System.Drawing.SizeF sf_font = g2.MeasureString(Title, Font);
            g2.Dispose();
            // Create Text Layout
            mTextLayout = new TextLayout(factory, m_strText, mTextFormat, sf_font.Width, sf_font.Height);
            mTextRect = new SharpDX.RectangleF(m_ptPos.X, m_ptPos.Y, sf_font.Width, sf_font.Height);

            return true;
        }

        public bool DiscardDXResource()
        {
            mTextFormat.Dispose();
            mTextLayout.Dispose();
            mTextFormat = null;
            mTextLayout = null;
            return true;
        }
        
        public bool Draw(SharpDX.Direct2D1.RenderTarget g, bool bDrawText)
        {
            if (m_pOwnLayer == null || m_pOwnLayer.Owner == null)
                return false;

            System.Drawing.Color orColor = GetColor();
            SharpDX.Color color = new SharpDX.Color(orColor.R, orColor.G, orColor.B, orColor.A);

            SharpDX.Matrix3x2 orgMatrix = g.Transform;
            SharpDX.Matrix3x2 textMatrix = SharpDX.Matrix3x2.Identity;
            textMatrix.M11 = g.Transform.M11;
            textMatrix.M12 = g.Transform.M12;
            textMatrix.M21 = g.Transform.M21;
            textMatrix.M22 = g.Transform.M22;
            textMatrix.M31 = g.Transform.M31;
            textMatrix.M32 = g.Transform.M32;

            if (m_dTextAngle != 0.0)
            {
                float rad = SharpDX.MathUtil.DegreesToRadians((float)m_dTextAngle);
                textMatrix = (SharpDX.Matrix3x2.Rotation(rad, mVecCenter) * textMatrix);
            }

            using (SolidColorBrush solidColorBrush = new SolidColorBrush(g, color))
            {
                DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)m_pOwnLayer.Owner;
                if (ctrl.DownToTop())
                {
                    // 윈도우 좌표계와 AutoCAD 좌표계는 세로 방향이 반대이므로 그대로 그리면 글자 모양이 뒤집힌다.
                    // 이를 방지하기 위하여 세로축 방향을 다시 뒤집은 다음, Y좌표를 음수로 두어 글자가 뒤집히지 않은채
                    // 원래 위치에서 표시되도록 한다.
                    textMatrix = (SharpDX.Matrix3x2.Scaling(1.0f, -1.0f) * textMatrix);
                    g.Transform = textMatrix;

                    mVecCenter.Y = -m_ptPos.Y;

                    float x1 = g.Transform.M22;
                    // 폰트의 길이와 Y축의 곱이 실제 픽셀당 거리
                    float h = x1 * m_font.Height;
                    // 1 픽셀미만이면 의미없으므로 Cutoff를 1로 한다.
                    // 자간이 좁아지면 Graphics에서 예외가 발생하므로 작은값은 피한다.
                    if (h > 1.0f || h < -1.0f)
                    {
                        //mTextRect.Y = -m_ptPos.Y;
                        try
                        {
                            g.DrawTextLayout(mVecCenter, mTextLayout, solidColorBrush, DrawTextOptions.None);
                        }
                        catch (System.InvalidOperationException)
                        {
                        }
                        catch (System.Runtime.InteropServices.ExternalException)
                        {
                        }
                    }

                    //mTextRect.Y = m_ptPos.Y;                   
                    mVecCenter.Y = m_ptPos.Y;
                }
                else
                {
                    g.Transform = textMatrix;
                    try
                    {
                        
                        g.DrawTextLayout(mVecCenter, mTextLayout, solidColorBrush, DrawTextOptions.None);
                    }
                    catch (System.Runtime.InteropServices.ExternalException)
                    {
                    }
                }
            }
            g.Transform = orgMatrix;
            return true;
        }

        public override bool CheckClipBounds(UnE.Geometry.Vertex2D vClipTL, UnE.Geometry.Vertex2D vClipBR)
        {
            return true;
        }
    }
}
