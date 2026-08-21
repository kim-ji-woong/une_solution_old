using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace libShapeFile
{
    public class FileLoader
    {
        public enum ErrorType { NONE = 0, NO_SHP_TYPE, NO_SHX_FILE, NO_DBF_FILE, NOT_SUPPORTED_SHAPE_TYPE };

        private ErrorType m_errorType = ErrorType.NONE;
        private UnE.Geometry.Vertex2D m_vTL = new UnE.Geometry.Vertex2D();
        private UnE.Geometry.Vertex2D m_vBR = new UnE.Geometry.Vertex2D();

        public UnE.Geometry.Vertex2D TopLeft
        {
            get { return m_vTL; }
        }

        public UnE.Geometry.Vertex2D BottomRight
        {
            get { return m_vBR; }
        }

        public bool SingleThread
        {
            get { return Shape.SingleThreaded; }
            set { Shape.SingleThreaded = false; }
        }
        
        public ErrorType ErrorMessage
        {
            get { return m_errorType; }
        }

        public List<Shape> LoadFile(string strShapeFilePath, out ShapeInfo shapeInfo)
        {
            shapeInfo = null;

            m_errorType = ErrorType.NONE;
            string strPath = "";

            if (strShapeFilePath.EndsWith(".shp", StringComparison.OrdinalIgnoreCase))
            {
                strPath = strShapeFilePath.Substring(0, strShapeFilePath.Length - 4);
            }
            else
            {
                m_errorType = ErrorType.NO_SHP_TYPE;
                return null;
            }

            string strIndexFilePath = strPath + ".shx";

            if (!File.Exists(strIndexFilePath))
            {
                m_errorType = ErrorType.NO_SHP_TYPE;
                return null;
            }

            string strDBFFilePath = strPath + ".dbf";
            
            if (File.Exists(strDBFFilePath))
            {
                shapeInfo = ShapeInfo.Load(strDBFFilePath);
            }

            List<Shape> shapes = new List<Shape>();

            ShapeFileMainHeader fileHeader;
            RecordHeader[] recordHeaders = LoadIndexfile(strIndexFilePath, out fileHeader);

            m_vTL.SetVertex(fileHeader.Xmin, fileHeader.Ymax);
            m_vBR.SetVertex(fileHeader.Xmax, fileHeader.Ymin);

            //open the main file and adjust the mainheader file length
            FileStream shapeFileStream = new FileStream(strShapeFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            fileHeader.FileLength = (int)shapeFileStream.Length;

            switch (fileHeader.ShapeType)
            {
                case ShapeType.Point:
                    if (!Point.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.Polygon:
                    if (!Polygon.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.PolyLine:
                    if (!PolyLine.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.MultiPoint:
                    if (!MultiPoint.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.PointM:
                    if (!PointM.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.PolygonM:
                    if (!PolygonM.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.PolyLineM:
                    if (!PolyLineM.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.MultiPointM:
                    if (!MultiPointM.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.PointZ:
                    if (!PointZ.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.PolygonZ:
                    if (!PolygonZ.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.PolyLineZ:
                    if (!PolyLineZ.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                case ShapeType.MultiPointZ:
                    if (!MultiPointZ.Load(shapeFileStream, recordHeaders, shapes))
                        goto RETURN_FALSE;
                    break;
                default:
                    shapeFileStream.Close();
                    m_errorType = ErrorType.NOT_SUPPORTED_SHAPE_TYPE;
                    return null;
            }

            shapeFileStream.Close();
            return shapes;

            RETURN_FALSE:
            shapeFileStream.Close();
            return null;
        }

        private RecordHeader[] LoadIndexfile(string path, out ShapeFileMainHeader fileHeader)
        {
            //read record headers from the index file
            RecordHeader[] recordHeaders = null;
            BinaryReader bReader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
            
            try
            {
                fileHeader = new ShapeFileMainHeader(bReader.ReadBytes(100));
                int totalRecords = (fileHeader.FileLength - 100) >> 3;
                recordHeaders = new RecordHeader[totalRecords];
                int numRecs = 0;
                //now read the record headers
                byte[] data = new byte[fileHeader.FileLength - 100];
                bReader.Read(data, 0, data.Length);
                while (numRecs < totalRecords)
                {
                    RecordHeader recHead = new RecordHeader(numRecs + 1);
                    recHead.readFromIndexFile(data, numRecs << 3);
                    recordHeaders[numRecs++] = recHead;
                }
                data = null;
#if SinglePrecision             
                this.recordHeaders = recordHeaders;
#endif
            }
            finally
            {
                bReader.Close();
                bReader = null;
            }
            return recordHeaders;
        }
    }
}
