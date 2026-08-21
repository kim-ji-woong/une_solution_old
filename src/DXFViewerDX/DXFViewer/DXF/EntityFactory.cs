using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFViewer
{
    public class EntityFactory : DXFDotNet.EntityFactory
    {
        protected DXFViewer.DXFControl mTargetContrl = null;

        protected EntityFactory() 
            : base()
        {
        }

        public EntityFactory(DXFViewer.DXFControl shapeOwner)
            : base()
        {
            mTargetContrl = shapeOwner;
        }

        public override DXFDotNet.Arc CreateArc()
        {
            DXFViewer.Arc arc = new DXFViewer.Arc();
            return arc;
        }

        public override DXFDotNet.Block CreateBlock(DXFDotNet.DXFControl ctrl)
        {
            DXFViewer.Block block = new DXFViewer.Block((DXFViewer.DXFControl)ctrl);
            return block;
        }

        public override DXFDotNet.EArc CreateEArc()
        {
            DXFViewer.EArc arc = new DXFViewer.EArc();
            return arc;
        }

        public override DXFDotNet.Hatch CreateHatch()
        {
            DXFViewer.Hatch hatch = new DXFViewer.Hatch();
            return hatch;
        }

        public override DXFDotNet.Layer CreateLayer(DXFDotNet.IShapeOwner owner)
        {
            DXFViewer.Layer layer = new DXFViewer.Layer(owner);
            return layer;
        }

        public override DXFDotNet.Layer CreateLayer(DXFDotNet.IShapeOwner owner, DXFDotNet.LineType lineType)
        {
            DXFViewer.Layer layer = new DXFViewer.Layer(owner, lineType);
            return layer;
        }

        public override DXFDotNet.Line CreateLine()
        {
            DXFViewer.Line line = new DXFViewer.Line();
            return line;
        }

        public override DXFDotNet.Line CreateLine(DXFDotNet.Line rhs)
        {
            DXFViewer.Line line = new DXFViewer.Line((DXFViewer.Line)rhs);
            return line;
        }

        public override DXFDotNet.Line CreateLine(UnE.Geometry.Vertex2D vBegin, UnE.Geometry.Vertex2D vEnd)
        {
            DXFViewer.Line line = new DXFViewer.Line(vBegin, vEnd);
            return line;
        }

        public override DXFDotNet.PolyLine CreatePolyLine()
        {
            DXFViewer.PolyLine line = new DXFViewer.PolyLine();
            return line;
        }

        public override DXFDotNet.ShapeGroup CreateShapeGroup()
        {
            DXFViewer.ShapeGroup group = new DXFViewer.ShapeGroup();
            return group;
        }

        public override DXFDotNet.ShapeGroup CreateShapeGroup(DXFDotNet.ShapeGroupOption option)
        {
            DXFViewer.ShapeGroup group = new DXFViewer.ShapeGroup(option);
            return group;
        }

        public override DXFDotNet.Text CreateText()
        {
            DXFViewer.Text text = new DXFViewer.Text();
            return text;
        }   

    }
}
