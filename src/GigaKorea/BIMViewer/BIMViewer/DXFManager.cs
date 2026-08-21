using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXFViewer;
using System.Drawing;

namespace BIMViewer
{
    public class DXFManager
    {
        public Layer Load(string strPath, DXFViewer.IPainter painter)
        {
            DXFControl ctrl = new DXFControl();

            if (ctrl.OpenDXF(strPath))
            {
                Layer newLayer = new Layer(painter);

                foreach (Layer layer in ctrl.Layers)
                {
                    if (layer.Hidden)
                        continue;

                    foreach (Shape shape in layer.Shapes)
                    {
                        Color color = shape.GetColor();
                        shape.SetColorOption(Shape.ControlType.BYOWN);
                        shape.SetOwnColor(color);
                        newLayer.Add(shape);
                    }
                }

                return newLayer;
            }

            return null;
        }
    }
}
