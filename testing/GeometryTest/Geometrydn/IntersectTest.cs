using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnE.Geometry;

namespace Geometrydn
{
    /// <summary>
    /// IntersectTest의 요약 설명
    /// </summary>
    [TestClass]
    public class IntersectTest
    {
        public IntersectTest()
        {
            //
            // TODO: 여기에 생성자 논리를 추가합니다.
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///현재 테스트 실행에 대한 정보 및 기능을
        ///제공하는 테스트 컨텍스트를 가져오거나 설정합니다.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 추가 테스트 특성
        //
        // 테스트를 작성할 때 다음 추가 특성을 사용할 수 있습니다.
        //
        // ClassInitialize를 사용하여 클래스의 첫 번째 테스트를 실행하기 전에 코드를 실행합니다.
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // ClassCleanup을 사용하여 클래스의 테스트를 모두 실행한 후에 코드를 실행합니다.
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // TestInitialize를 사용하여 각 테스트를 실행하기 전에 코드를 실행합니다.
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // TestCleanup을 사용하여 각 테스트를 실행하기 전에 코드를 실행합니다.
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestIntesect()
        {
            //
            // TODO: 테스트 논리를 여기에 추가합니다.
            //
//             if (!TestLineToLine())
//                 return;
//             if (!TestLineToEArc())
//                 return;
//             if (!TestEArcToEArc())
//                 return;
//             if (!TestEArcToArc())
//                 return;
//             if (!TestArcToArc())
//                 return;
        }

        bool ErrorMessage(String strMessage)
		{
			Assert.Inconclusive(strMessage);
			return false;
		}

        bool ErrorIntersectMessage0(string strTag, int nVertexCount, int nResultCount)
        {
            string strMsg = String.Format("{0} 결과 : VertexCount({1})이어야 하나, ResultCount({2})이다.",
                strTag, nResultCount, nVertexCount);
            return ErrorMessage(strMsg);
        }

        bool ErrorIntersectMessage1(string strTag, Vertex2D v1, Line2D.LineType type, Vertex2D vResult1, Line2D.LineType typeResult)
        {
            string strMsg = String.Format("{0} 결과 : v1({1}, {2}), LineType({3})이어야 하나, vResult1({4}, {5})), LineType({6})이다.",
                 strTag, vResult1.x, vResult1.y, typeResult, v1.x, v1.y, type);
            return ErrorMessage(strMsg);
        }

        bool ErrorIntersectMessage2(string strTag, Vertex2D v1, Vertex2D v2, Line2D.LineType type, Vertex2D vResult1, Vertex2D vResult2, Line2D.LineType typeResult)
        {
            string strMsg = String.Format("{0} 결과 : v1({1}, {2}), v2({3}, {4}), LineType({5})이어야 하나, vResult1({6}, {7}), vResult2({8}, {9}), LineType({10})이다.",
                 strTag, vResult1.x, vResult1.y, vResult2.x, vResult2.y, typeResult, v1.x, v1.y, v2.x, v2.y, type);
            return ErrorMessage(strMsg);
        }

        bool CheckIntersect(string strTag, Vertex2D v1, Vertex2D v2, Line2D.LineType type, int nVertexCount, Vertex2D vResult1, Vertex2D vResult2, Line2D.LineType typeResult, int nResultCount)
        {
            if (nVertexCount != nResultCount)
            {
                return ErrorIntersectMessage0(strTag, nVertexCount, nResultCount);
            }

            if (nVertexCount == 0)
                return true;

            if (nVertexCount == 1)
            {
                if (v1 != vResult1 && type == typeResult)
                {
                    return ErrorIntersectMessage1(strTag, v1, type, vResult1, typeResult);
                }
                else
                    return true;
            }

            if (type != typeResult)
                return ErrorIntersectMessage2(strTag, v1, v2, type, vResult1, vResult2, typeResult);

            if (type == Line2D.LineType.LINE)
                return true;

            if (v1 != vResult1 || v2 != vResult2)
            {
                if (v1 != vResult2 || v2 != vResult1)
                {
                    return ErrorIntersectMessage2(strTag, v1, v2, type, vResult1, vResult2, typeResult);
                }
            }

            return true;
        }

        bool TestLineToLine()
        {
            Vertex2D v1 = new Vertex2D(0.0, 0.0);
            Vertex2D v2 = new Vertex2D(100.0, 0.0);

            Line2D line1 = new Line2D(v1, v2, Line2D.LineType.LINE);
            Line2D halfLine1 = new Line2D(v1, v2, Line2D.LineType.HALF_LINE_BEGIN_2_END);
            Line2D seg1 = new Line2D(v1, v2, Line2D.LineType.SEGMENT);

            Vertex2D v3 = new Vertex2D(50.0, 0.0);
            Vertex2D v4 = new Vertex2D(150.0, 0.0);

            Line2D line2 = new Line2D(v3, v4, Line2D.LineType.LINE);
            Line2D halfLine2 = new Line2D(v3, v4, Line2D.LineType.HALF_LINE_BEGIN_2_END);
            Line2D seg2 = new Line2D(v3, v4, Line2D.LineType.SEGMENT);

            Vertex2D v5 = new Vertex2D(50.0, 50.0);
            Vertex2D v6 = new Vertex2D(50.0, -50.0);

            Line2D line3 = new Line2D(v5, v6, Line2D.LineType.LINE);
            Line2D halfLine3 = new Line2D(v5, v6, Line2D.LineType.HALF_LINE_BEGIN_2_END);
            Line2D seg3 = new Line2D(v5, v6, Line2D.LineType.SEGMENT);

            Vertex2D v7 = new Vertex2D(50.0, 150.0);
            Vertex2D v8 = new Vertex2D(50.0, 100.0);
            Vertex2D v9 = new Vertex2D(50.0, 50.0);

            Line2D line4 = new Line2D(v7, v8, Line2D.LineType.LINE);
            Line2D halfLine4 = new Line2D(v7, v8, Line2D.LineType.HALF_LINE_BEGIN_2_END);
            Line2D seg4 = new Line2D(v7, v8, Line2D.LineType.SEGMENT);

            Line2D seg5 = new Line2D(v7, v9, Line2D.LineType.SEGMENT);

            Vertex2D v12_1 = new Vertex2D(50.0, 0.0);
            Vertex2D v12_2 = new Vertex2D(100.0, 0.0);
            Line2D.LineType type12 = Line2D.LineType.LINE;
            int nResult12 = 2;

            Vertex2D v13_1 = new Vertex2D(50.0, 0.0);
            Line2D.LineType type13 = Line2D.LineType.NO_LINE;
            int nResult13 = 1;

            Line2D.LineType type34 = Line2D.LineType.NO_LINE;
            int nResult34 = 0;

            Vertex2D v35_1 = new Vertex2D(50.0, 50.0);
            Line2D.LineType type35 = Line2D.LineType.NO_LINE;
            int nResult35 = 1;

            Vertex2D vNull = new Vertex2D();

            Vertex2D vertex1, vertex2;
            Line2D.LineType lineType;

            int nResult = line1.IntersectLine(line2, out vertex1, out vertex2, out lineType);
            if (!CheckIntersect("line1 & line2 Intersect", vertex1, vertex2, lineType, nResult, v12_1, v12_2, type12, nResult12))
                return false;

            nResult = line1.IntersectLine(line3, out vertex1, out vertex2, out lineType);
            if (!CheckIntersect("line1 & line3 Intersect", vertex1, vertex2, lineType, nResult, v13_1, vNull, type13, nResult13))
                return false;

            nResult = seg3.IntersectLine(seg4, out vertex1, out vertex2, out lineType);
            if (!CheckIntersect("seg3 & seg4 Intersect", vertex1, vertex2, lineType, nResult, vNull, vNull, type34, nResult34))
                return false;

            nResult = halfLine3.IntersectLine(seg5, out vertex1, out vertex2, out lineType);
            if (!CheckIntersect("halfLine3 & seg5 Intersect", vertex1, vertex2, lineType, nResult, v35_1, vNull, type35, nResult35))
                return false;

            return true;
        }

        bool ErrorIntersectMessage1(string strTag, Vertex2D v1, Vertex2D vResult1)
        {
            string strMsg = String.Format("{0} 결과 : v1({1}, {2})이어야 하나, vResult1({3}, {4})이다.",
                 strTag, vResult1.x, vResult1.y, v1.x, v1.y);
            return ErrorMessage(strMsg);
        }

        bool ErrorIntersectMessage2(string strTag, Vertex2D v1, Vertex2D v2, Vertex2D vResult1, Vertex2D vResult2)
        {
            string strMsg = String.Format("{0} 결과 : v1({1}, {2}), v2({3}, {4})이어야 하나, vResult1({5}, {6}), vResult2({7}, {8})이다.",
                 strTag, vResult1.x, vResult1.y, vResult2.x, vResult2.y, v1.x, v1.y, v2.x, v2.y);
            return ErrorMessage(strMsg);
        }

        bool CheckIntersect(string strTag, Vertex2D v1, Vertex2D v2, int nVertexCount, Vertex2D vResult1, Vertex2D vResult2, int nResultCount)
        {
            if (nVertexCount != nResultCount)
            {
                return ErrorIntersectMessage0(strTag, nVertexCount, nResultCount);
            }

            if (nVertexCount == 0)
                return true;

            if (nVertexCount == 1)
            {
                if (v1 != vResult1)
                {
                    return ErrorIntersectMessage1(strTag, v1, vResult1);
                }
                else
                    return true;
            }

            if (v1 != vResult1 || v2 != vResult2)
            {
                if (v1 != vResult2 || v2 != vResult1)
                {
                    return ErrorIntersectMessage2(strTag, v1, v2, vResult1, vResult2);
                }
            }

            return true;
        }

        bool TestLineToEArc()
        {
            Vertex2D v1 = new Vertex2D(0.0, 0.0);
            Vertex2D v2 = new Vertex2D(100.0, 0.0);

            Line2D line1 = new Line2D(v1, v2, Line2D.LineType.LINE);
            Line2D halfLine1 = new Line2D(v1, v2, Line2D.LineType.HALF_LINE_BEGIN_2_END);
            Line2D seg1 = new Line2D(v1, v2, Line2D.LineType.SEGMENT);

            Vertex2D vTL = new Vertex2D(20.0, 20.0);
            Vertex2D vBL = new Vertex2D(20.0, -20.0);
            Vertex2D vBR = new Vertex2D(80.0, -20.0);

            EArc2D earc1 = new EArc2D(vTL, vBL, vBR, 6.0, 3.5, false);
            EArc2D earc2 = new EArc2D(vTL, vBL, vBR, 6.0, 3.5, true);

            Vertex2D vNull = new Vertex2D();

            Vertex2D v11_1 = new Vertex2D(80.0, 0.0);
            Vertex2D v11_2 = new Vertex2D(20.0, 0.0);
            int nResult11 = 2;

            Vertex2D v12_1 = new Vertex2D(20.0, 0.0);
            int nResult12 = 1;

            Vertex2D vertex1, vertex2;
            int nResult = earc1.IntersectLine(line1, out vertex1, out vertex2);

            if (!CheckIntersect("earc1 & line1 Intersect", vertex1, vertex2, nResult, v11_1, v11_2, nResult11))
                return false;

            nResult = earc2.IntersectLine(line1, out vertex1, out vertex2);

            if (!CheckIntersect("earc2 & line1 Intersect", vertex1, vertex2, nResult, v12_1, vNull, nResult12))
                return false;

            return true;
        }

        bool ContainVertex(Vertex2D vertex, System.Collections.ArrayList arrListVertex)
        {
            foreach (Vertex2D v in arrListVertex)
            {
                if (vertex == v)
                    return true;
            }

            return false;
        }

        bool CheckIntersect(string strTag, System.Collections.ArrayList arrListVertex, System.Collections.ArrayList arrListEArc, int nIntersectCount, System.Collections.ArrayList arrListVertexResult, System.Collections.ArrayList arrListEArcResult, int nResultCount)
        {
            if (nIntersectCount != nResultCount)
            {
                return ErrorIntersectMessage0(strTag, nIntersectCount, nResultCount);
            }

            if (nIntersectCount == 0)
                return true;

            int nVertexCount = nIntersectCount % 100;
            int nEArcCount = nIntersectCount / 100;
            
            if (nVertexCount > 0)
            {
                if (arrListVertex == null || arrListVertexResult == null)
                {
                    string strError = strTag + "arrListVertex 또는 arrListVertexResult가 null입니다.";
                    return ErrorMessage(strError);
                }

                int nCount1 = arrListVertex.Count;
                int nCount2 = arrListVertexResult.Count;

                if (nVertexCount != nCount1 || nCount1 != nCount2)
                {
                    string strError = strTag + String.Format("nVertexCount = {0}, arrListVertex의 개수 = {1}, arrListVertexResult의 개수 = {2}로 서로 일치하지 않습니다.",
                        nVertexCount, nCount1, nCount2);
                    return ErrorMessage(strError);
                }

                for (int i = 0; i < nVertexCount; i++)
                {
                    Vertex2D v1 = (Vertex2D)arrListVertex[i];

                    if (!ContainVertex(v1, arrListVertexResult))
                    {
                        Vertex2D v2 = (Vertex2D)arrListVertexResult[i];
                        return ErrorIntersectMessage1(strTag, v1, v2);
                    }
                    //Vertex2D v2 = (Vertex2D)arrListVertexResult[i];

                    //if (v1 != v2)
                    //    return ErrorIntersectMessage1(strTag, v1, v2);
                }
            }

            if (nEArcCount > 0)
            {
                if (arrListEArc == null || arrListEArcResult == null)
                {
                    string strError = strTag + "arrListEArc 또는 arrListEArcResult가 null입니다.";
                    return ErrorMessage(strError);
                }

                int nCount1 = arrListEArc.Count;
                int nCount2 = arrListEArcResult.Count / 2;

                if (nEArcCount != nCount1 || nCount1 != nCount2)
                {
                    string strError = strTag + String.Format("nEArcCount = {0}, arrListEArc의 개수 = {1}, arrListEArcResult의 개수 = {2}로 서로 일치하지 않습니다.",
                        nVertexCount, nCount1, nCount2);
                    return ErrorMessage(strError);
                }

                for (int i = 0; i < nEArcCount; i++)
                {
                    EArc2D arc1 = (EArc2D)arrListEArc[i];

                    //Vertex2D v1 = (Vertex2D)arrListEArcResult[i * 2 + 0];
                    //Vertex2D v2 = (Vertex2D)arrListEArcResult[i * 2 + 1];

                    Vertex2D vBegin = arc1.GetBeginVertex();
                    Vertex2D vEnd = arc1.GetEndVertex();

                    if (!ContainVertex(vBegin, arrListEArcResult) || !ContainVertex(vEnd, arrListEArcResult))
                    {
                        Vertex2D v1 = (Vertex2D)arrListEArcResult[i * 2 + 0];
                        Vertex2D v2 = (Vertex2D)arrListEArcResult[i * 2 + 1];
                        return ErrorIntersectMessage2(strTag, vBegin, vEnd, v1, v2);
                    }

                    /*if (v1 != vBegin || v2 != vEnd)
                        return ErrorIntersectMessage2(strTag, vBegin, vEnd, v1, v2);*/
                }
            }

            return true;
        }

        bool TestEArcToEArc()
        {
            Vertex2D vTL1 = new Vertex2D(20.0, 20.0);
            Vertex2D vBL1 = new Vertex2D(20.0, -20.0);
            Vertex2D vBR1 = new Vertex2D(80.0, -20.0);

            Vertex2D vTL2 = new Vertex2D(30.0, 30.0);
            Vertex2D vBL2 = new Vertex2D(30.0, -30.0);
            Vertex2D vBR2 = new Vertex2D(70.0, -30.0);

            EArc2D earc1 = new EArc2D(vTL1, vBL1, vBR1, 6.0, 3.5, false);
            EArc2D earc2 = new EArc2D(vTL2, vBL2, vBR2, 5.0, 3.5, true);

            System.Collections.ArrayList arrVertex, arrEArc;
            int nResult = earc1.IntersectEArc(earc2, out arrVertex, out arrEArc);

            Vertex2D vNull = new Vertex2D();

            System.Collections.ArrayList arrVertex11 = new System.Collections.ArrayList();
            Vertex2D v11_1 = new Vertex2D(33.358994113243149, 16.641005886756883);
            arrVertex11.Add(v11_1);

            int nResult11 = 1;

            if (!CheckIntersect("earc1 & earc2 Intersect", arrVertex, arrEArc, nResult, arrVertex11, null, nResult11))
                return false;

            System.Collections.ArrayList arrVertex3, arrEArc3;
            EArc2D earc3 = new EArc2D(vTL1, vBL1, vBR1, 6.2, 4.0, true);
            nResult = earc1.IntersectEArc(earc3, out arrVertex3, out arrEArc3);

            System.Collections.ArrayList arrEArc13 = new System.Collections.ArrayList();

            Vertex2D v13_1 = new Vertex2D(77.494713190712559, -8.0011317688452870);
            Vertex2D v13_2 = new Vertex2D(79.768092332187791, -2.4819955145757739);
            Vertex2D v13_3 = new Vertex2D(36.902718602104358, 17.993347165418133);
            Vertex2D v13_4 = new Vertex2D(20.189874241688958, -2.2466174350372952);
            arrEArc13.Add(v13_1);
            arrEArc13.Add(v13_2);
            arrEArc13.Add(v13_3);
            arrEArc13.Add(v13_4);

            int nResult13 = 200;

            if (!CheckIntersect("earc1 & earc3 Intersect", arrVertex3, arrEArc3, nResult, null, arrEArc13, nResult13))
                return false;

            return true;
        }

        bool TestEArcToArc()
        {
            Vertex2D vTL1 = new Vertex2D(20.0, 20.0);
            Vertex2D vBL1 = new Vertex2D(20.0, -20.0);
            Vertex2D vBR1 = new Vertex2D(80.0, -20.0);

            Vertex2D v1 = new Vertex2D(50.0, 25.0);
            Vertex2D v2 = new Vertex2D(75.0, 0.0);
            Vertex2D v3 = new Vertex2D(50.0, -25.0);

            EArc2D earc = new EArc2D(vTL1, vBL1, vBR1, 6.0, 3.5, false);
            Arc2D arc = new Arc2D(v1, v2, v3);
            
            System.Collections.ArrayList arrVertex, arrArc;
            int nResult = earc.IntersectEArc(arc, out arrVertex, out arrArc);

            Vertex2D vNull = new Vertex2D();

            System.Collections.ArrayList arrVertex11 = new System.Collections.ArrayList();
            Vertex2D v11_1 = new Vertex2D(70.124611797498119, 14.832396974191322);
            arrVertex11.Add(v11_1);

            int nResult11 = 1;

            if (!CheckIntersect("earc & arc Intersect", arrVertex, arrArc, nResult, arrVertex11, null, nResult11))
                return false;

            return true;
        }

        bool TestArcToArc()
        {
            Vertex2D v1 = new Vertex2D(50.0, 25.0);
            Vertex2D v2 = new Vertex2D(25.0, 0.0);
            Vertex2D v3 = new Vertex2D(50.0, -25.0);

            Vertex2D v4 = new Vertex2D(50.0, 25.0);
            Vertex2D v5 = new Vertex2D(75.0, 0.0);
            Vertex2D v6 = new Vertex2D(50.0, -25.0);

            Vertex2D vCenter = new Vertex2D(50.0, 0.0);

            Arc2D arc1 = new Arc2D(v1, v2, v3);
            Arc2D arc2 = new Arc2D(v4, v5, v6);
            Arc2D arc3 = new Arc2D(vCenter, 25.0, 1.0, UnE.Geometry.Math._2PI() - 2, false);

            System.Collections.ArrayList arrVertex, arrArc;
            int nResult = arc1.IntersectEArc(arc2, out arrVertex, out arrArc);

            Vertex2D vNull = new Vertex2D();

            System.Collections.ArrayList arrVertex12 = new System.Collections.ArrayList();
            Vertex2D v12_1 = new Vertex2D(50.0, 25.0);
            Vertex2D v12_2 = new Vertex2D(50.0, -25.0);
            arrVertex12.Add(v12_1);
            arrVertex12.Add(v12_2);

            int nResult12 = 2;

            if (!CheckIntersect("arc1 & arc2 Intersect", arrVertex, arrArc, nResult, arrVertex12, null, nResult12))
                return false;

            System.Collections.ArrayList arrVertex2, arrArc2;

            int nResult2 = arc2.IntersectEArc(arc3, out arrVertex2, out arrArc2);

            System.Collections.ArrayList arrArc13 = new System.Collections.ArrayList();
            Vertex2D v23_1 = new Vertex2D(63.507557646703496, 21.036774620197413);
            Vertex2D v23_2 = new Vertex2D(50.0, 25.0);
            Vertex2D v23_3 = new Vertex2D(50.0, -25.0);
            Vertex2D v23_4 = new Vertex2D(63.507557646703496, -21.036774620197413);
            arrArc13.Add(v23_1);
            arrArc13.Add(v23_2);
            arrArc13.Add(v23_3);
            arrArc13.Add(v23_4);

            int nResult23 = 200;

            if (!CheckIntersect("arc2 & arc3 Intersect", arrVertex2, arrArc2, nResult2, null, arrArc13, nResult23))
                return false;

            return true;
        }
    }
}
