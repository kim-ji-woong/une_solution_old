using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace UnE.Overlay
{
	public class OverlayElement
	{
		private float m_ObjectScale = 1.0f;
		public float ObjectScale
		{
			get { return m_ObjectScale; }
			set { m_ObjectScale = value; }
		}

		protected RectangleF m_OffsetRect;
		protected float m_DistGravity = 3.0f;

		protected float m_Thick = 1.0f;
		protected bool m_bHighLight = false;

		public float LineThick
		{
			get { return m_Thick; }
			set { m_Thick = value; }
		}

		protected Color m_HighLightColor;
		protected Color m_LineColor = Color.Black;
		public Color LineColor
		{
			get { return m_LineColor; }
			set
			{
				m_LineColor = value;

				m_HighLightColor = ColorExtensions.GetContrast(m_LineColor, true);
			}
		}

		public virtual void DrawElement(PaintEventArgs g)
		{
		}

		public virtual void TempDrawElement(PaintEventArgs e)
		{
		}

		private PointF m_TranslatePoint;
		public PointF BasePoint
		{
			get { return m_TranslatePoint; }
			set
			{
				m_TranslatePoint = value;
			}
		}


		protected void SetScale(Graphics g)
		{
			//float scale = 1 / m_ObjectScale;
			//g.TranslateTransform(-m_TranslatePoint.X, -m_TranslatePoint.Y);
			//g.ScaleTransform(scale, scale);
		}

		protected void UnsetScale(Graphics g)
		{
			//float scale =  m_ObjectScale;

			//g.ScaleTransform(scale, scale);
			//g.TranslateTransform(m_TranslatePoint.X, m_TranslatePoint.Y);
		}

		public virtual bool IsPicked(PointF pt)
		{
			return false;
		}

		public virtual void HighLight()
		{
			m_bHighLight = true;
		}

		public virtual void Reset()
		{
			m_bHighLight = false;
		}
	}
}
