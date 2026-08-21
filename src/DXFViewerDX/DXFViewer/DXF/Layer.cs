using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharpDX.Direct2D1;

namespace DXFViewer
{
    public class Layer : DXFDotNet.Layer
    {
        protected DXFViewer.DXFControl m_Ctrl = null;

        public Layer(DXFDotNet.IShapeOwner owner) : base(owner)
        {
            m_Ctrl = (DXFViewer.DXFControl)owner;
           
        }

        public Layer(DXFDotNet.IShapeOwner owner, DXFDotNet.LineType lineType)
            : base(owner, lineType)
        {
            m_Ctrl = (DXFViewer.DXFControl)owner;
           
        }

        public override void Add(DXFDotNet.Shape obj)
        {
            base.Add(obj);

            if (obj.GetType().GetInterfaces().Contains(typeof(IDrawableShape)))
            {
                IDrawableShape shape = (IDrawableShape)obj;
                m_Ctrl.AddDXEntity(shape);
            }                       
        }

		// pObject가 Layer에 존재하면 pObject를 삭제하고 true를 리턴한다.
		public override  bool Remove(DXFDotNet.Shape obj)
        {
            bool bResult = base.Remove(obj);
            if (obj.GetType().GetInterfaces().Contains(typeof(IDrawableShape)))
            {
                IDrawableShape shape = (IDrawableShape)obj;
                m_Ctrl.RemoveDXEntity(shape);
            }  
            return bResult;
        }

		// 모든 Object를 삭제하면 true,
		// 삭제하지 못한 Object가 존재하면 false를 리턴한다.
		public override  bool RemoveAll()
        {
            foreach(DXFDotNet.Shape shape in this.m_listObject)
            {
                if (shape.GetType().GetInterfaces().Contains(typeof(IDrawableShape)))
                {
                    IDrawableShape obj = (IDrawableShape)shape;
                    m_Ctrl.RemoveDXEntity(obj);
                }     
            }
            return base.RemoveAll();
        }

		// pObject가 Layer에 존재하면 true를 리턴한다.
		public override bool Find(DXFDotNet.Shape obj)
        {
            return base.Find(obj);
        }

		public override void Reset()
        {
            base.Reset();
        }
        
        public bool Draw(SharpDX.Direct2D1.RenderTarget g, bool bDrawText)
        {
	        if (m_isHidden || m_isFrozen)
                return false;

	        foreach (DXFDotNet.Shape obj in m_listObject)
	        {
		        Block block = (Block)obj.GetBlock();
		        if (block != null)
		        {
			        if (block.Hidden)
				        continue;
		        }
                if (obj.Visible)
                {
                    DrawObject(obj, g, bDrawText);                    
                }
	        }
	        return true;
        }

        bool DrawObject(DXFDotNet.Shape shape, SharpDX.Direct2D1.RenderTarget g, bool bDrawText)
        {
            if(shape.GetType().GetInterfaces().Contains(typeof(IDrawableShape)))
            {
                IDrawableShape obj = (IDrawableShape)shape;
                DrawObject((IDrawableShape)obj, g, bDrawText);
            }
            return true;
        }

        bool DrawObject(IDrawableShape obj, SharpDX.Direct2D1.RenderTarget g, bool bDrawText)
        {
            if (m_isHidden || m_isFrozen || obj == null || m_owner == null)
                return false;

            DXFDotNet.Shape shape = obj.GetShapeObject();            
            if (shape != null)
            {
                DXFViewer.DXFControl control = (DXFViewer.DXFControl)m_owner;
                UnE.Geometry.Vertex2D vCenter = control.GetViewportCenter();
                double dViewportWeight = control.GetViewportWeight();
                int nScreenWidth = control.GetScreenWidth();
                int nScreenHeight = control.GetScreenHeight();

                UnE.Geometry.Vertex2D vScreenTL = m_owner.ScreenToGlobal(0, 0);
                UnE.Geometry.Vertex2D vScreenBR = m_owner.ScreenToGlobal(nScreenWidth, nScreenHeight);

                //if (obj.CheckClipBounds(g, vScreenTL, vScreenBR))
                {
                
                    if (shape.GetBlock() != null)
                    {
                        //---glPushMatrix();
                        UnE.Geometry.Vertex2D vOrigin = shape.GetBlock().OriginVertex;

                        SharpDX.Matrix3x2 mat = g.Transform;
                        mat.M31 += (float)vOrigin.x;
                        mat.M32 += (float)vOrigin.y;

                        obj.Draw(g, bDrawText);

                        mat.M31 -= (float)vOrigin.x;
                        mat.M32 -= (float)vOrigin.y;
                    }
                    else
                    {
                        obj.Draw(g, bDrawText);
                    }
                } 
            }
            return true;
        }
          
        public bool DrawShapeByType(SharpDX.Direct2D1.RenderTarget g, bool bDrawText, DXFDotNet.Shape.ShapeType type)
        {
            if (m_isHidden || m_isFrozen)
                return false;

            IEnumerable<DXFDotNet.Shape> nonHatchList =
                from DXFDotNet.Shape shape in m_listObject
                where (shape.GetShapeType() == type)
                select shape;

            foreach (DXFDotNet.Shape obj in nonHatchList)
            {
                Block block = (Block)obj.GetBlock();

                if (block != null)
                {
                    if (block.Hidden)
                        continue;
                }

                if (obj.GetShapeType() == type)
                {
                    if (obj.Visible)
                    {
                        DrawObject((IDrawableShape)obj, g, bDrawText);
                    }
                }                
            }
            return true;
        }

        public bool DrawShapeExcludeByType(SharpDX.Direct2D1.RenderTarget g, bool bDrawText, DXFDotNet.Shape.ShapeType type)
        {
            if (m_isHidden || m_isFrozen)
                return false;

            IEnumerable<DXFDotNet.Shape> nonHatchList =
                from DXFDotNet.Shape shape in m_listObject
                where (shape.GetShapeType() != type)
                select shape;

            foreach (DXFDotNet.Shape obj in nonHatchList)
            {
                Block block = (Block)obj.GetBlock();

                if (block != null)
                {
                    if (block.Hidden)
                        continue;
                }

                if (obj.GetShapeType() != type)
                {
                    if (obj.Visible)
                    {
                        DrawObject((IDrawableShape)obj, g, bDrawText);
                    }
                }
            }
            return true;
        }

        public bool DrawExceptHatchNText(SharpDX.Direct2D1.RenderTarget g, bool bDrawText)
        {
	        if (m_isHidden || m_isFrozen)
                return false;

            IEnumerable<DXFDotNet.Shape> hatchList =
                from DXFDotNet.Shape shape in m_listObject
                where (shape.GetShapeType() != DXFDotNet.Shape.ShapeType.HATCH)
                && (shape.GetShapeType() != DXFDotNet.Shape.ShapeType.TEXT)
                select shape;

            foreach (DXFDotNet.Shape obj in hatchList)
	        {
		        Block block = (Block)obj.GetBlock();

		        if (block != null)
		        {
			        if (block.Hidden)
				        continue;
		        }

                if ((obj.GetShapeType() != DXFDotNet.Shape.ShapeType.HATCH)
                && (obj.GetShapeType() != DXFDotNet.Shape.ShapeType.TEXT))
                {
                    if (obj.Visible)
                    {
                        DrawObject((IDrawableShape)obj, g, bDrawText);
                    }	
                }                	       
	        }	
	        return true;
        }
    }
}
