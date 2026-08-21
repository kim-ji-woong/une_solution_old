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
using System.Xml;
using System.Xml.Serialization;
using System.IO;

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
		public RectangleF OffsetRect
		{
			get { return m_OffsetRect; }
			set { m_OffsetRect = value; }
		}

		protected float m_DistGravity = 30.0f;
		public float DistanceGravity
		{
			get { return m_DistGravity; }
			set { m_DistGravity = value; }
		}

		protected float m_Thick = 1.0f;
		public float LineThick
		{
			get { return m_Thick; }
			set { m_Thick = value; }
		}

		protected bool m_bHighLight = false;
		public bool Selected
		{
			get { return m_bHighLight; }
			set { m_bHighLight = value; }
		}

		protected Color m_HighLightColor;
		protected Color m_LineColor = Color.Black;

		[XmlIgnore]
		public Color LineColor
		{
			get { return m_LineColor; }
			set
			{
				m_LineColor = value;

				m_HighLightColor = ColorExtensions.GetContrast(m_LineColor, true);
			}
		}

		public int LineColorInt
		{
			get { return m_LineColor.ToArgb(); }
			set
			{
				m_LineColor = Color.FromArgb(value);
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
			
		}

		protected void UnsetScale(Graphics g)
		{
			
		}

		public virtual bool IsPicked(RectangleF rect)
		{
			return false;
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

			
		public virtual void SaveXML(XmlTextWriter witer)
		{
			try
			{
				XmlSerializer sz = new XmlSerializer(this.GetType());
				sz.Serialize(witer, this);
			}
			catch(Exception)
			{

			}			
		}
		
        public virtual void OnPostXMLRead()
        {
        }
	}

	public class OverlayFactory
	{
		public static UnE.Overlay.OverlayElement Deserialize(string srXML)
		{
			try
			{
				string szText = srXML.Substring(2);
				szText = szText.Trim();
				OverlayElement result = null;
				
				//reader.Read();
				//reader.
				Type type = typeof(OverlayElement);
				if (szText.IndexOf("OverlayLine") != -1)
				{
					type = typeof(OverlayLine);
				}
				else if (szText.IndexOf("OverlayRect") != -1)
				{
					type = typeof(OverlayRect);
				}
				else if (szText.IndexOf("OverlayOval") != -1)
				{
					type = typeof(OverlayOval);
				}
				else if (szText.IndexOf("OverlayFreeHands") != -1)
				{
					type = typeof(OverlayFreeHands);
				}
				else if (szText.IndexOf("OverlayText") != -1)
				{
					type = typeof(OverlayText);
				}


				XmlReader reader = XmlReader.Create(new StringReader(szText));
				XmlSerializer sz = new XmlSerializer(type);
				result = (OverlayElement)sz.Deserialize(reader);
				return result;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.StackTrace);
			}
			return null;
			
		}
	}
}
