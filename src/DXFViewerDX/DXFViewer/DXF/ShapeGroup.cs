using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;

namespace DXFViewer
{
    public class ShapeGroup : DXFDotNet.ShapeGroup, IDrawableShape
    {
        protected DXFViewer.IDrawableShape mDrawableShape = null;
        protected EditBox m_editBox = null;

        protected SharpDX.Direct2D1.Bitmap mDrawBitmap = null;
        protected SharpDX.RectangleF mImageRect = new SharpDX.RectangleF();

        public ShapeGroup() : base()
        {
        }

        public ShapeGroup(DXFDotNet.ShapeGroupOption option) : base(option)
        {            
        }

        public DXFDotNet.Shape GetShapeObject()
        {
            return this;
        }

        public override DXFDotNet.ShapeGroup CreateShapeGroup()
        {
            ShapeGroup arc = new ShapeGroup();
            return arc;
        }

        public bool CreateDXResource()
        {
            if (m_pOwnLayer == null || m_pOwnLayer.Owner == null)
                return false;
            DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)m_pOwnLayer.Owner;
            SharpDX.Direct2D1.RenderTarget g = ctrl.RenderTarget;
            
            if( this.DrawingType == DrawType.IMAGE)
            {
                mDrawBitmap = ShapeGroup.LoadFromImage(g, m_img);

            }

            m_editBox = new EditBox(ctrl);
            m_editBox.CreateDXResource();
            return true;
        }

        public bool DiscardDXResource()
        {
            if (m_editBox != null)
            {
                m_editBox.DiscardDXResource();
                m_editBox = null;
            }
            if (mDrawBitmap != null)
            {
                mDrawBitmap.Dispose();
                mDrawBitmap = null;
            }
            return true;
        }
        
        protected bool GetImageSize(ref float rWidth, ref float rHeight)
        {
            if (m_pOwnLayer == null || m_pOwnLayer.Owner == null)
                return false;

            DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)m_pOwnLayer.Owner;

	        if (m_img != null)
	        {
		        if (m_imgSize.Width < 0 || m_imgSize.Height < 0)
		        {
			        rWidth = m_img.Width * m_fImageScale;
			        rHeight = m_img.Height * m_fImageScale;
		        }
		        else
		        {
			        rWidth = m_imgSize.Width * m_fImageScale;
			        rHeight = m_imgSize.Height * m_fImageScale;
		        }

                if (ctrl.DownToTop())
			        rHeight = -rHeight;

		        return true;
	        }
	        return false;
        }

        public bool Draw(SharpDX.Direct2D1.RenderTarget g, bool bDrawText)
        {
            if (m_drawType == DrawType.IMAGE)
	        {
		        if (m_img != null)
		        {
			        float fWidth = 0, fHeight = 0;
			        GetImageSize(ref fWidth, ref fHeight);
                    
                    mImageRect.X = (float)m_vPos.x;
                    mImageRect.Y = (float)m_vPos.x;
                    mImageRect.Width = fWidth;
                    mImageRect.Height = fHeight;

                    g.DrawBitmap(mDrawBitmap, mImageRect, 1.0f, BitmapInterpolationMode.Linear);

			        if (Selectable && Selected)
				        m_editBox.Draw(g, (float)(m_vPos.x + fWidth / 2), (float)(m_vPos.y + fHeight / 2));

			        return true;
		        }
	        }
	        else if (m_drawType == DrawType.SHAPE)
	        {
                if (m_shape != null)
		        {
			        m_shape.Selectable = this.Selectable;
			        m_shape.Selected = this.Selected;
			        
                    mDrawableShape = (IDrawableShape)m_shape;
                    if(mDrawableShape != null)
                    {
                        mDrawableShape.Draw(g, bDrawText);
                    }
			        return true;
		        }
	        }
            return true;
        }

        public static Bitmap LoadFromImage(RenderTarget renderTarget, System.Drawing.Image img)
        {
            // Loads from file using System.Drawing.Image
            using (var bitmap = (System.Drawing.Bitmap)new System.Drawing.Bitmap(img))
            {
                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapProperties = new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                var size = new Size2(bitmap.Width, bitmap.Height);

                // Transform pixels from BGRA to RGBA
                int stride = bitmap.Width * sizeof(int);
                using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // Not optimized 
                            byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }

                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;
                    return new Bitmap(renderTarget, size, tempStream, stride, bitmapProperties);
                }
            }
        }

        public static Bitmap LoadFromFile(RenderTarget renderTarget, string file)
        {
            // Loads from file using System.Drawing.Image
            using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(file))
            {
                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapProperties = new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                var size = new Size2(bitmap.Width, bitmap.Height);

                // Transform pixels from BGRA to RGBA
                int stride = bitmap.Width * sizeof(int);
                using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // Not optimized 
                            byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }

                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;

                    return new Bitmap(renderTarget, size, tempStream, stride, bitmapProperties);
                }
            }
        }

        public override bool CheckClipBounds(UnE.Geometry.Vertex2D vClipTL, UnE.Geometry.Vertex2D vClipBR)
        {
            return true;
        }
        
    }
}
