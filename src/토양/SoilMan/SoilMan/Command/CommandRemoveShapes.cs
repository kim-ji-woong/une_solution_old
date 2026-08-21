using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilMan.Command
{
    public class CommandRemoveShapes : UnE.Command.Command
    {
        private libShapeFile.ShapeInfo m_shapeInfo = null;
        private Drawing.ShapeLayer m_layer = null;
        private List<Drawing.Polygon> m_removeShapes = null;
        private Popup.FormDetailAttrib m_frmDetailAttrib = null;
        private DXFViewer.DXFControl m_ctrl = null;

        public List<Drawing.Polygon> RemoveShapes
        {
            get { return m_removeShapes; }
            set { m_removeShapes = value; }
        }

        public CommandRemoveShapes(DXFViewer.DXFControl ctrl, libShapeFile.ShapeInfo shapeInfo, Drawing.ShapeLayer layer, Popup.FormDetailAttrib frm)
        {
            m_shapeInfo = shapeInfo;
            m_layer = layer;
            m_frmDetailAttrib = frm;
            m_ctrl = ctrl;
        }

        public CommandRemoveShapes(DXFViewer.DXFControl ctrl, libShapeFile.ShapeInfo shapeInfo, Drawing.ShapeLayer layer, Popup.FormDetailAttrib frm, List<Drawing.Polygon> removeShapes)
        {
            m_shapeInfo = shapeInfo;
            m_layer = layer;
            m_removeShapes = removeShapes;
            m_frmDetailAttrib = frm;
            m_ctrl = ctrl;
        }

        public override void RollBack()
        {
            if (m_removeShapes == null)
                return;

            foreach (DXFViewer.Shape shape in m_layer.Shapes)
            {
                if (shape is Drawing.PolygonList)
                {
                    Drawing.PolygonList polygonList = (Drawing.PolygonList)shape;
                    polygonList.AddPolygons(m_removeShapes);
                    PostChange(polygonList, m_layer);
                    return;
                }
            }

            m_layer.Shapes.AddRange(m_removeShapes);
            PostChange(null, m_layer);
        }

        public override void Do()
        {
            if (m_removeShapes == null)
                return;

            foreach (DXFViewer.Shape shape in m_layer.Shapes)
            {
                if (shape is Drawing.PolygonList)
                {
                    Drawing.PolygonList polygonList = (Drawing.PolygonList)shape;
                    polygonList.RemovePolygons(m_removeShapes);
                    PostChange(polygonList, m_layer);
                    return;
                }
            }

            foreach (Drawing.Polygon polygon in m_removeShapes)
            {
                m_layer.Shapes.Remove(polygon);
            }

            PostChange(null, m_layer);
        }

        private void PostChange(Drawing.PolygonList polygonList, Drawing.ShapeLayer layer)
        {
            if (m_frmDetailAttrib != null)
            {
                List<Drawing.Polygon> selectedShapes = m_frmDetailAttrib.SelectedShapes;
                List<Drawing.Polygon> extraShapes = new List<Drawing.Polygon>();

                if (selectedShapes != null)
                {
                    foreach (Drawing.Polygon selectedShape in selectedShapes)
                    {
                        if (selectedShape != null)
                        {
                            bool removedShaped = false;

                            if (polygonList != null)
                            {
                                if (polygonList.GetPolygonFromID(selectedShape.ID) == null)
                                    removedShaped = true;
                            }
                            else if (!layer.Shapes.Contains(selectedShape))
                                removedShaped = true;

                            if (removedShaped)
                            {
                                layer.SelectedList.Remove(selectedShape);
                            }
                            else
                            {
                                extraShapes.Add(selectedShape);
                            }
                        }
                    }
                }

                m_frmDetailAttrib.SetShapeInfo(m_shapeInfo, m_layer, true);

                bool bUpdate = false;
                foreach (Drawing.Polygon selectedShape in extraShapes)
                {
                    if (m_frmDetailAttrib.Visible && selectedShape != null)
                    {
                        bUpdate = true;
                        m_frmDetailAttrib.Select(selectedShape);
                    }
                }    
                if( bUpdate == false)
                {
                    m_frmDetailAttrib.Unselect();
                }
            }
            m_ctrl._Refresh();
        }
    }
}
