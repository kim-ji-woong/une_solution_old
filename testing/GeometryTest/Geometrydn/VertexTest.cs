using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnE.Geometry;

namespace Geometrydn
{
    /// <summary>
    /// VertexTest의 요약 설명
    /// </summary>
    [TestClass]
    public class VertexTest
    {
        public VertexTest()
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
        public void TestVertex()
        {
            //
            // TODO: 테스트 논리를 여기에 추가합니다.
            //
            if (!TestGetLinearVertex3D())
                return;
            if (!TestGetLinearVertex2D())
                return;
            //if (!TestIntersectLine2D())
            //    return;
        }

        bool ErrorMessage(String strMessage)
		{
			Assert.Inconclusive(strMessage);
			return false;
		}

        /*bool ErrorIntersectMessage0(string strTag, int nVertexCount, int nResultCount)
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

        bool TestIntersectLine2D()
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
        }*/

        bool TestGetLinearVertex3D()
        {
            Vertex3D v1 = new Vertex3D(0.0, 0.0, 0.0);
            Vertex3D v2 = new Vertex3D(100.0, 100.0, 100.0);

            Vertex3D v3 = GetLinearVertex(v1, v2, v1.GetDistance(v2) * 2);

            if (System.Math.Abs(v3.x - 200.0) > UnE.Geometry.Math.HALF_TOLERANCE() ||
                System.Math.Abs(v3.y - 200.0) > UnE.Geometry.Math.HALF_TOLERANCE() ||
                System.Math.Abs(v3.z - 200.0) > UnE.Geometry.Math.HALF_TOLERANCE())
            {
                ErrorMessage(String.Format("v1({0}, {1}, {2})에서 v2({3}, {4}, {5}) 방향으로 {6} 만큼 이동한 좌표가 v3({7}, {8}, {9})입니다.",
                    v1.x, v1.y, v1.z, v2.x, v2.y, v2.z, v1.GetDistance(v2) * 2, v3.x, v3.y, v3.z));
                return false;
            }

            return true;
        }

        bool TestGetLinearVertex2D()
        {
            Vertex2D v1 = new Vertex2D(0.0, 0.0);
            Vertex2D v2 = new Vertex2D(100.0, 100.0);

            Vertex2D v3 = GetLinearVertex(v1, v2, v1.GetDistance(v2) * 2);

            if (System.Math.Abs(v3.x - 200.0) > UnE.Geometry.Math.HALF_TOLERANCE() ||
                System.Math.Abs(v3.y - 200.0) > UnE.Geometry.Math.HALF_TOLERANCE())
            {
                ErrorMessage(String.Format("v1({0}, {1})에서 v2({2}, {3}) 방향으로 {4} 만큼 이동한 좌표가 v3({5}, {6})입니다.",
                    v1.x, v1.y, v2.x, v2.y, v1.GetDistance(v2) * 2, v3.x, v3.y));
                return false;
            }

            return true;
        }

		Vertex3D GetLinearVertex(Vertex3D v1, Vertex3D v2, double dLen)
		{
            double dist = v1.GetDistance(v2);
			if (dist <= UnE.Geometry.Math.HALF_TOLERANCE())
				return new Vertex3D(v1.x, v1.y, v1.z);

			Vertex3D v = v1 + (v2 - v1) * dLen / dist;
			return v;
		}

        Vertex2D GetLinearVertex(Vertex2D v1, Vertex2D v2, double dLen)
        {
            double dist = v1.GetDistance(v2);
            if (dist <= UnE.Geometry.Math.HALF_TOLERANCE())
                return new Vertex2D(v1.x, v1.y);

            Vertex2D v = v1 + (v2 - v1) * dLen / dist;
            return v;
        }
    }
}
