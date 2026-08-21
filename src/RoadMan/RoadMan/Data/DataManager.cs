using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXFViewer;
using System.Drawing;

namespace RoadMan
{
    public class DataManager
    {
        // 개설
        private List<LayerData> m_layersComplete = new List<LayerData>();
        private List<LayerData> m_layersCompleteBackup = new List<LayerData>();
        // 미개설
        private List<LayerData> m_layersInComplete = new List<LayerData>();
        private List<LayerData> m_layersInCompleteBackup = new List<LayerData>();
        // 폭원미개설
        private List<LayerData> m_layersPartial = new List<LayerData>();
        private List<LayerData> m_layersPartialBackup = new List<LayerData>();

        private Color m_colorProcessScheduleLayer = Color.Yellow;
        private Color m_colorProcessResultLayer = Color.Yellow;

        // Key : 노선이름
        private Dictionary<string, List<Shape>> m_dicStreetShapes = new Dictionary<string, List<Shape>>();
        private Dictionary<string, StreetCenterLine2> m_dicStreetCenterLines = new Dictionary<string, StreetCenterLine2>();
        // Shape과 연결된 노선이름
        private Dictionary<Shape, string> m_dicShapeStreets = new Dictionary<Shape, string>();

        // Key : 지번 이름 
        private Dictionary<string, LandAddressData2> m_dicLandAddressData = new Dictionary<string, LandAddressData2>();

        // 개설
        public List<LayerData> CompleteLayers
        {
            get { return m_layersComplete; }
        }

        // 미개설
        public List<LayerData> IncompleteLayers
        {
            get { return m_layersInComplete; }
        }

        // 폭원 미개설
        public List<LayerData> PartialLayers
        {
            get { return m_layersPartial; }
        }
			
        public Dictionary<string, List<Shape>> StreetShapes
        {
            get { return m_dicStreetShapes; }
        }

        public Dictionary<string, StreetCenterLine2> StreetCenterLines
        {
            get { return m_dicStreetCenterLines; }
        }

        public Dictionary<Shape, string> ShapeStreets
        {
            get { return m_dicShapeStreets; }
            set { m_dicShapeStreets = value; }
        }

        public Dictionary<string, LandAddressData2> LandAddressDatas
        {
            get { return m_dicLandAddressData; }
        }

        // 쓰레드를 사용하지 않는 버전
		public List<LayerData> OpenDXF(string strPath, PanelDXFViewer panel)
		{
			DXFControl ctrl = panel.DXFControl;
			List<LayerData> arrLayerDatas = null;

			if (ctrl.OpenDXF(strPath))
			{
				arrLayerDatas = new List<LayerData>();
				int nLayerIndex = ctrl.Layers.Count;

				foreach (Layer layer in ctrl.Layers)
				{
					LayerData data = new LayerData();

					data.Visible = !layer.Hidden && !layer.Frozen;
					data.LayerName = layer.LayerName;
					data.Color = Color.FromArgb(255, layer.LineColor.R, layer.LineColor.G, layer.LineColor.B);
					data.Alpha = layer.LineColor.A;
					data.LayerIndex = nLayerIndex--;
					data.LinkedLayer = layer;

					//arrLayerDatas.Add(data);
					arrLayerDatas.Insert(0, data);
				}

				NewExternLayer(ctrl);
				panel.DXFFilePath = strPath;
			}

			return arrLayerDatas;
		}

        // 쓰레드를 사용하는 버전
        public List<LayerData> PostOpenDXF(string strPath, PanelDXFViewer panel)
        {
            DXFControl ctrl = panel.DXFControl;
            List<LayerData> arrLayerDatas = null;

            arrLayerDatas = new List<LayerData>();
            int nLayerIndex = ctrl.Layers.Count;

            foreach (Layer layer in ctrl.Layers)
            {
                LayerData data = new LayerData();

                data.Visible = !layer.Hidden && !layer.Frozen;
                data.LayerName = layer.LayerName;
                data.Color = Color.FromArgb(255, layer.LineColor.R, layer.LineColor.G, layer.LineColor.B);
                data.Alpha = layer.LineColor.A;
                data.LayerIndex = nLayerIndex--;
                data.LinkedLayer = layer;

                arrLayerDatas.Insert(0, data);
            }

            NewExternLayer(ctrl);
            panel.DXFFilePath = strPath;
            
            return arrLayerDatas;
        }

        private void NewExternLayer(DXFControl ctrl)
        {
            DXFExternPainter externPainter = (DXFExternPainter)ctrl.ExternalPainter;
            externPainter.Clear();

            Layer layerSchedule = externPainter.GetLayer(DXFExternPainter.LayerType.PROCESS_SCHEDULE);
            Layer layerResult = externPainter.GetLayer(DXFExternPainter.LayerType.PROCESS_RESULT);

            if (layerSchedule != null)
                layerSchedule.LineColor = m_colorProcessScheduleLayer;

            if (layerResult != null)
                layerResult.LineColor = m_colorProcessResultLayer;
        }

        public void SetSelectableLayers()
        {
            SetSelectableLayers(m_layersComplete, true);
            SetSelectableLayers(m_layersInComplete, true);
            SetSelectableLayers(m_layersPartial, true);
        }

        private void SetSelectableLayers(List<LayerData> layers, bool selectable)
        {
            foreach (LayerData data in layers)
            {
                if (data.LinkedLayer != null)
                    SetSelectableObjects(data.LinkedLayer, selectable);
            }
        }

        private void SetSelectableObjects(DXFViewer.Layer layer, bool selectable)
        {
            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape.Visible)
                {
                    shape.Selectable = selectable;
                    shape.SelectedShowing = Shape.SelectedShowingType.BRIGHT_EFFECT;
                }
            }
        }

        public void Clear()
        {
            m_layersCompleteBackup.Clear();
            m_layersInCompleteBackup.Clear();
            m_layersPartialBackup.Clear();

            foreach (LayerData data in m_layersComplete)
            {
                m_layersCompleteBackup.Add(data);
            }

            foreach (LayerData data in m_layersInComplete)
            {
                m_layersInCompleteBackup.Add(data);
            }

            foreach (LayerData data in m_layersPartial)
            {
                m_layersPartialBackup.Add(data);
            }

            m_layersComplete.Clear();
            m_layersInComplete.Clear();
            m_layersPartial.Clear();
        }

        public void Backup()
        {
            m_layersCompleteBackup.Clear();
            m_layersInCompleteBackup.Clear();
            m_layersPartialBackup.Clear();

            foreach (LayerData data in m_layersComplete)
            {
                m_layersCompleteBackup.Add(data);
            }

            foreach (LayerData data in m_layersInComplete)
            {
                m_layersInCompleteBackup.Add(data);
            }

            foreach (LayerData data in m_layersPartial)
            {
                m_layersPartialBackup.Add(data);
            }
        }

        public void Restore()
        {
            m_layersComplete.Clear();
            m_layersInComplete.Clear();
            m_layersPartial.Clear();

            foreach (LayerData data in m_layersCompleteBackup)
            {
                m_layersComplete.Add(data);
            }

            foreach (LayerData data in m_layersInCompleteBackup)
            {
                m_layersInComplete.Add(data);
            }

            foreach (LayerData data in m_layersPartialBackup)
            {
                m_layersPartial.Add(data);
            }
        }

        public void SetLayerList(List<LayerData> layersTrg, List<LayerData> layersSrc)
        {
            List<LayerData> layersBackup = null;

            if (layersTrg == m_layersComplete)
                layersBackup = m_layersCompleteBackup;
            else if (layersTrg == m_layersInComplete)
                layersBackup = m_layersInCompleteBackup;
            else if (layersTrg == m_layersPartial)
                layersBackup = m_layersPartialBackup;
            else
                return;

            SetSelectableLayers(layersBackup, false);

            layersTrg.Clear();
            layersBackup.Clear();

            foreach (LayerData data in layersSrc)
            {
                layersTrg.Add(data);
                layersBackup.Add(data);
            }

            SetSelectableLayers(layersTrg, true);
        }

        public void SetLinkedLayers(List<LayerData> layers)
        {
            Dictionary<string, LayerData> dicLayers = new Dictionary<string, LayerData>();

            foreach (LayerData data in layers)
            {
                dicLayers[data.LayerName] = data;
            }

            SetLinkedLayers(m_layersComplete, m_layersCompleteBackup, dicLayers);
            SetLinkedLayers(m_layersInComplete, m_layersInCompleteBackup, dicLayers);
            SetLinkedLayers(m_layersPartial, m_layersPartialBackup, dicLayers);
        }

        private void SetLinkedLayers(List<LayerData> layers, List<LayerData> backupLayers, Dictionary<string, LayerData> dicLayers)
        {
            List<LayerData> arrAdd = new List<LayerData>();

            foreach (LayerData data in layers)
            {
                if (dicLayers.ContainsKey(data.LayerName))
                    arrAdd.Add(dicLayers[data.LayerName]);
            }

            layers.Clear();
            backupLayers.Clear();

            foreach (LayerData data in arrAdd)
            {
                layers.Add(data);
                backupLayers.Add(data);
            }

            SetSelectableLayers(layers, true);
        }

        public Dictionary<int, Shape> SetStreetShapes(System.Collections.ArrayList layers, Dictionary<string, List<int>> dicStreetShapeIDs)
        {
            m_dicStreetShapes.Clear();

            Dictionary<int, Shape> dicShapeIDs = new Dictionary<int, Shape>();

            // 모든 Shape ID를 읽어 dicShapeIDs에 집어넣기
            foreach (Layer layer in layers)
            {
                foreach (Shape shape in layer.Shapes)
                {
                    dicShapeIDs[shape.ID] = shape;
                }
            }

            if (dicStreetShapeIDs != null && dicStreetShapeIDs.Count > 0)
            {
                Shape _shape;

                // dicStreetShapeIDs를 읽어 m_dicStreetShapes에 할당하기
                foreach (KeyValuePair<string, List<int>> pair in dicStreetShapeIDs)
                {
                    List<Shape> shapes = new List<Shape>();
                    m_dicStreetShapes[pair.Key] = shapes;

                    foreach (int nShapeID in pair.Value)
                    {
                        if (dicShapeIDs.TryGetValue(nShapeID, out _shape))
                        {
                            shapes.Add(_shape);
                            m_dicShapeStreets[_shape] = pair.Key;
                        }
                    }
                }
            }

            return dicShapeIDs;
        }

        public void SetStreetCenterLines(Dictionary<int, Shape> dicShapeIDs, Dictionary<string, StreetCenterLine> dicStreetCenterLines)
        {
            m_dicStreetCenterLines.Clear();

            if (dicStreetCenterLines != null)
            {
                foreach (KeyValuePair<string, StreetCenterLine> pair in dicStreetCenterLines)
                {
                    StreetCenterLine2 line2 = ToStreetCenterLine2(pair.Value, dicShapeIDs);
                    m_dicStreetCenterLines[pair.Key] = line2;
                }
            }
        }

        private StreetCenterLine2 ToStreetCenterLine2(StreetCenterLine centerLine, Dictionary<int, Shape> dicShapeIDs)
        {
            Shape shape;
            StreetCenterLine2 line2 = new StreetCenterLine2();

            foreach (KeyValuePair<int, PolyLineEx> pair in centerLine.PolyLines)
            {
                if (dicShapeIDs.TryGetValue(pair.Key, out shape))
                {
                    line2.PolyLines[shape] = pair.Value;
                }
            }

            line2.StreetName = centerLine.StreetName;
            return line2;
        }

        public string GetStreetName(Shape shape)
        {
            if (shape != null)
            {
                string strStreetName = "";

                if (m_dicShapeStreets.TryGetValue(shape, out strStreetName))
                    return strStreetName;
            }

            return "";
        }

        public void SelectShape(PanelDXFViewer panel, int x, int y, bool refresh)
        {
            UnE.Geometry.Vertex2D vertex = panel.DXFControl.ScreenToGlobal(x, y);
            Shape shape = panel.DXFControl.SelectObject(vertex.x, vertex.y);

            if (shape != null)
            {
                string strStreetName = "";

                if (m_dicShapeStreets.TryGetValue(shape, out strStreetName))
                {
                    ScheduleProperty prop = panel.FindScheduleProperty(strStreetName);

                    if (prop != null)
                    {
                        FormSettingStreetName.SelectShape(panel, shape, true);

                        if (panel.ScheduleDetailForm != null)
                        {
                            // 이미 같은 창이 떠있으면 다시 띄우지 않는다.
                            if (panel.ScheduleDetailForm.ScheduleProperty != prop)
                            {
                                panel.ScheduleDetailForm.Close();
                                ShowScheduleDetail(prop, panel);
                            }
                        }
                        else
                            ShowScheduleDetail(prop, panel);

                        FormSettingStreetName.SelectShape(panel, shape, false);
                    }
                }
            }
        }

        public static void ShowScheduleDetail(ScheduleProperty prop, PanelDXFViewer panel)
        {
            FormScheduleDetail frm = new FormScheduleDetail(prop, panel);
			DialogFormFrame frameOption = new DialogFormFrame(frm);
            //frm.Show();
			frameOption.Show();
        }

		public List<Shape> FindShapes(string szStreetName)
		{
			if (szStreetName == null || szStreetName == "")
				return null;

			List<Shape> arList = null;
			if (m_dicStreetShapes.TryGetValue(szStreetName, out arList))
			{
				return arList;
			}	
			return null;
		}

		private RectangleF CalcShapeBound(List<Shape> shapes)
		{
			RectangleF rectResult = new RectangleF();

			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			foreach(Shape shape in shapes)
			{
				UnE.Geometry.Vertex2D ptTl = shape.BoundaryTL;
				UnE.Geometry.Vertex2D ptBr = shape.BoundaryBR;

				if (minX > (float)ptTl.x)
				{
					minX = (float)ptTl.x;
				}
				
				if( maxX < (float)ptTl.x)
				{
					maxX = (float)ptTl.x;
				}

				if (minX > (float)ptBr.x)
				{
					minX = (float)ptBr.x;
				}

				if (maxX < (float)ptBr.x)
				{
					maxX = (float)ptBr.x;
				}


				if (minY > (float)ptTl.y)
				{
					minY = (float)ptTl.y;
				}

				if (maxY < (float)ptTl.y)
				{
					maxY = (float)ptTl.y;
				}

				if (minY > (float)ptBr.y)
				{
					minY = (float)ptBr.y;
				}

				if (maxY < (float)ptBr.y)
				{
					maxY = (float)ptBr.y;
				}

				
			}

			rectResult.X = minX;
			rectResult.Y = minY;
			rectResult.Width = maxX - minX;
			rectResult.Height = maxY - minY;

			return rectResult;
		}

		public List<Shape> ObjectZoom(List<Shape> shapes, PanelDXFViewer panel, bool noSelection = false)
		{			
			if (shapes == null || shapes.Count == 0)
				return null;

			RectangleF rect = CalcShapeBound(shapes);

            if (!noSelection)
            {
                panel.ClearFixedSelection();

                FormSettingStreetName.SelectShapes(panel, shapes, true, false);
            }


			float xCenter = rect.X + rect.Width * 0.5f;
			float yCenter = rect.Y + rect.Height * 0.5f;

			UnE.Geometry.Vertex2D tl = panel.DXFControl.ScreenToGlobal(0, 0);
			UnE.Geometry.Vertex2D br = panel.DXFControl.ScreenToGlobal(panel.DXFControl.Width, panel.DXFControl.Height);

            if (tl == null || br == null)
                return null;

			float vDX = (float)Math.Abs(tl.x - br.x);
			float vDY = (float)Math.Abs(tl.y - br.y);

			float widthRate = rect.Width / vDX;
			float heightRate = rect.Height / vDY;

			float targetRate = (widthRate > heightRate ? widthRate : heightRate);
			UnE.Geometry.Vertex2D ptCenter = new UnE.Geometry.Vertex2D(xCenter, yCenter);

			UnE.Geometry.Vertex2D ppp = panel.DXFControl.GetViewportCenter();
			Point pt2 = new Point(panel.DXFControl.Width / 2, panel.DXFControl.Height / 2);
			Point pt1 = panel.DXFControl.GlobalToScreen(ptCenter);

			//UnE.Geometry.Vertex2D ppp = panel.DXFControl.ScreenToGlobal(pt2.X, pt2.Y);

			//UnE.Overlay.OverlayRect overlayrect = new UnE.Overlay.OverlayRect();
			//overlayrect.Point1 = new PointF(rect.X, rect.Y);
			//overlayrect.Point2 = new PointF(rect.X + rect.Width, rect.Y + rect.Height);

			//panel.OverlayPanel.EntityList.Add(overlayrect);

			int dx = pt2.X - pt1.X;
			int dy = pt2.Y - pt1.Y;

			UnE.Geometry.Vertex2D vCenter = new UnE.Geometry.Vertex2D(ppp.x - dx, ppp.y - dy);
			panel.DXFControl.SetViewportCenter(vCenter);

			float fZoomValue = (float)panel.DXFControl.GetViewportWeight() / targetRate * 0.5f;
			if (fZoomValue > 7.0f)
				fZoomValue = 7.0f;

			UnE.Geometry.Vertex2D ptZoom = panel.DXFControl.ScreenToGlobal(pt2.X, pt2.Y);
			panel.DXFControl.Zoom(fZoomValue, ptCenter, true);

			panel.DXFControl.Refresh();

			return shapes;

		}

		public List<Shape> ObjectZoom(string szStreetName, PanelDXFViewer panel, bool noSelection = false)
		{
			List<Shape> shapes = FindShapes(szStreetName);
			return ObjectZoom(shapes, panel, noSelection);
		}
    }
}
