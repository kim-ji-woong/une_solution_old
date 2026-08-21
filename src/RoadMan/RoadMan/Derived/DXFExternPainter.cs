using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXFViewer;

namespace RoadMan
{
	public delegate void OnOverlayPostPaint(System.Windows.Forms.PaintEventArgs e);
	public delegate void OnSelectRectPostPaint(System.Windows.Forms.PaintEventArgs e);
	public delegate void OnUnderlayPrePaint(System.Windows.Forms.PaintEventArgs e);

    public class DXFExternPainter : ExternalPainter
    {
		public event OnOverlayPostPaint OverlayPostPainter;
		public event OnSelectRectPostPaint SelectRectPostPaint;
		public event OnUnderlayPrePaint UnderlayPrePainter;

        public enum LayerType { PROCESS_SCHEDULE = 0, PROCESS_RESULT, LAND_ADDRESS, TYPE_COUNT };

        private List<Layer> m_layers = new List<Layer>();
        private Dictionary<LayerType, Layer> m_dicLayer = new Dictionary<LayerType, Layer>();

        private Shape m_selectedShape = null;

        public DXFExternPainter(DXFControl ctrl)
        {
            Layer layerSchedule = new Layer(ctrl);
            layerSchedule.LayerName = "집행계획";
            m_layers.Add(layerSchedule);

            Layer layerResult = new Layer(ctrl);
            layerResult.LayerName = "집행진행상황";
            m_layers.Add(layerResult);

            Layer layerLandAddr = new Layer(ctrl);
            layerLandAddr.LayerName = "지번";
            m_layers.Add(layerLandAddr);

            layerLandAddr.LineColor = System.Drawing.Color.FromArgb(100, 0, 255, 0);

            m_dicLayer[LayerType.PROCESS_SCHEDULE] = layerSchedule;
            m_dicLayer[LayerType.PROCESS_RESULT] = layerResult;
            m_dicLayer[LayerType.LAND_ADDRESS] = layerLandAddr;
        }

        public override void OnPrevPaint(System.Drawing.Graphics g, bool m_bDrawText)
        //public override void OnPrevPaint(System.Windows.Forms.PaintEventArgs e)
        {
			if(UnderlayPrePainter!= null)
			{
                System.Drawing.Size size = FormMain.Instance.CurrentDXFControl.Size;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, size.Width, size.Height);
                System.Windows.Forms.PaintEventArgs e = new System.Windows.Forms.PaintEventArgs(g, rect);
				UnderlayPrePainter(e);
			}
        }

        public override void OnPostPaint(System.Drawing.Graphics g, bool m_bDrawText)
		//public override void OnPostPaint(System.Windows.Forms.PaintEventArgs e)
        {
            
        }

        public override void OnOverlayPaint(System.Drawing.Graphics g, bool bDrawText)
        {
            System.Drawing.Size size = FormMain.Instance.CurrentDXFControl.Size;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, size.Width, size.Height);
            System.Windows.Forms.PaintEventArgs e = new System.Windows.Forms.PaintEventArgs(g, rect);

            //System.Drawing.Graphics g = e.Graphics;
            foreach (Layer layer in m_layers)
            {
                layer.Draw(g, true);
            }

            if (OverlayPostPainter != null)
            {
                OverlayPostPainter(e);
            }

            if (SelectRectPostPaint != null)
            {
                SelectRectPostPaint(e);
            }
        }

		// OnPrintPage() 호출되기 직전에 호출된다.
        public override void OnPrevPrint(System.Drawing.Graphics g)
		//public override void OnPrevPrint(System.Windows.Forms.PaintEventArgs e)
		{
			//if (UnderlayPrePainter != null)
			//{
			//	UnderlayPrePainter(e);
			//}
		}
		// OnPrintPage() 호출된 직후에 호출된다.
        public override void OnPostPrint(System.Drawing.Graphics g)
		//public override void OnPostPrint(System.Windows.Forms.PaintEventArgs e)
		{
            System.Drawing.Size size = FormMain.Instance.CurrentDXFControl.Size;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, size.Width, size.Height);
            System.Windows.Forms.PaintEventArgs e = new System.Windows.Forms.PaintEventArgs(g, rect);

			//System.Drawing.Graphics g = e.Graphics;
			foreach (Layer layer in m_layers)
			{
				layer.Draw(g, true);
			}

			if (OverlayPostPainter != null)
			{
				OverlayPostPainter(e);
			}
		}

        public Layer GetLayer(LayerType type)
        {
            if (m_dicLayer.ContainsKey(type))
                return m_dicLayer[type];

            return null;
        }

        public void Clear()
        {
            foreach (Layer layer in m_layers)
            {
                layer.RemoveAll();
            }
        }

        private Shape SelectObject(Layer layer, UnE.Geometry.Vertex2D vertex)
        {
            foreach (Shape shape in layer.Shapes)
            {
                shape.Selectable = true;

                if (shape.HitTest(vertex.x, vertex.y))
                    return shape;
            }

            return null;
        }

        public void ClearSelection()
        {
            if (m_selectedShape != null)
            {
                m_selectedShape.Visible = false;
                m_selectedShape = null;
            }
        }

        public Shape SelectShape(DXFControl ctrl, int x, int y, bool refresh)
        {
            Layer layer = GetLayer(DXFExternPainter.LayerType.PROCESS_SCHEDULE);

            if (layer == null)
                return null;

            bool isChanged = FormEditSection.Instance.ClearSelection();

            UnE.Geometry.Vertex2D vertex = ctrl.ScreenToGlobal(x, y);
            Shape shape = SelectObject(layer, vertex);

            if (m_selectedShape == shape)
            {
                if (isChanged && refresh)
                    ctrl.Refresh();

                return shape;
            }
            else
            {
                if (shape == null)
                    m_selectedShape.Visible = false;
                else
                {
                    if (m_selectedShape != null)
                        m_selectedShape.Visible = false;

                    shape.Visible = true;

                    if (shape.GetType() == typeof(EditBoxHatch))
                        FormMain.Instance.ShowEditBoxHatchProperty((EditBoxHatch)shape);
                }

                m_selectedShape = shape;

                if (refresh)
                    ctrl.Refresh();
            }

            return shape;
        }
    }
}
