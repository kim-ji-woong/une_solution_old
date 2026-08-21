using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnE.Geometry;

namespace Geometrydn
{
    /// <summary>
    /// OffsetTest의 요약 설명
    /// </summary>
    [TestClass]
    public class PlaneTest
    {
        public PlaneTest()
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
        public void TestPlane()
        {
            //
            // TODO: 테스트 논리를 여기에 추가합니다.
            //
            if (!TestMakePlane())
                return;
            if (!TestNearestVertex())
                return;
        }

        bool ErrorMessage(String strMessage)
		{
			Assert.Inconclusive(strMessage);
			return false;
		}

        bool CheckPlane(String strTag, double a, double b, double c, double d, double aResult, double bResult, double cResult, double dResult)
        {
            if (System.Math.Abs(a - aResult) > UnE.Geometry.Math.HALF_TOLERANCE())
            {
                String strMsg = String.Format("{0} 평면의 방정식 계수 a가 {1}이어야 하나 {2}이다.",
                    strTag, aResult, a);
                return ErrorMessage(strMsg);
            }

            if (System.Math.Abs(b - bResult) > UnE.Geometry.Math.HALF_TOLERANCE())
            {
                String strMsg = String.Format("{0} 평면의 방정식 계수 b가 {1}이어야 하나 {2}이다.",
                    strTag, bResult, b);
                return ErrorMessage(strMsg);
            }

            if (System.Math.Abs(c - cResult) > UnE.Geometry.Math.HALF_TOLERANCE())
            {
                String strMsg = String.Format("{0} 평면의 방정식 계수 c가 {1}이어야 하나 {2}이다.",
                    strTag, cResult, c);
                return ErrorMessage(strMsg);
            }

            if (System.Math.Abs(d - dResult) > UnE.Geometry.Math.HALF_TOLERANCE())
            {
                String strMsg = String.Format("{0} 평면의 방정식 계수 d가 {1}이어야 하나 {2}이다.",
                    strTag, dResult, d);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckVertex(String strTag, Vertex3D v, Vertex3D vResult)
        {
            if (v != vResult)
            {
                String strMsg = String.Format("{0} vTarget이 ({1}, {2}, {3})이어야 하나 ({4}, {5}, {6})이다.",
                    strTag, v.x, v.y, v.z, vResult.x, vResult.y, vResult.z);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool TestMakePlane()
        {
            Vertex3D v1 = new Vertex3D(0.0, 0.0, 0.0);
            Vertex3D v2 = new Vertex3D(0.0, 100.0, 0.0);
            Vertex3D v3 = new Vertex3D(100.0, 0.0, 0.0);
            
            double a, b, c, d;
            if (!UnE.Geometry.Math.MakePlane(v1, v2, v3, out a, out b, out c, out d))
                return false;

            if (!CheckPlane("TestMakePlane 첫번째 Test", a, b, c, d, 0.0, 0.0, -10000.0, 0.0))
                return false;

            v1.z = v2.z = v3.z = 1.0;
            if (!UnE.Geometry.Math.MakePlane(v1, v2, v3, out a, out b, out c, out d))
                return false;

            if (!CheckPlane("TestMakePlane 두번째 Test", a, b, c, d, 0.0, 0.0, -10000.0, 10000.0))
                return false;

            v1.SetVertex(1.0, 0.0, 0.0);
            v2.SetVertex(1.0, 0.0, 100.0);
            v3.SetVertex(1.0, 100.0, 0.0);

            if (!UnE.Geometry.Math.MakePlane(v1, v2, v3, out a, out b, out c, out d))
                return false;

            if (!CheckPlane("TestMakePlane 세번째 Test", a, b, c, d, -10000.0, 0.0, 0.0, 10000.0))
                return false;

            v1.SetVertex(0.0, 1.0, 0.0);
            v2.SetVertex(100.0, 1.0, 0.0);
            v3.SetVertex(0.0, 1.0, 100.0);

            if (!UnE.Geometry.Math.MakePlane(v1, v2, v3, out a, out b, out c, out d))
                return false;

            if (!CheckPlane("TestMakePlane 네번째 Test", a, b, c, d, 0.0, -10000.0, 0.0, 10000.0))
                return false;

            return true;
        }

        bool TestNearestVertex()
        {
            Vertex3D v1 = new Vertex3D(0.0, 0.0, 0.0);
            Vertex3D v2 = new Vertex3D(0.0, 100.0, 0.0);
            Vertex3D v3 = new Vertex3D(100.0, 0.0, 0.0);

            double a, b, c, d;
            if (!UnE.Geometry.Math.MakePlane(v1, v2, v3, out a, out b, out c, out d))
                return false;

            Vertex3D vertex = new Vertex3D(50.0, 50.0, 50.0);
            Vertex3D vTarget = UnE.Geometry.Math.GetNearestVertex(vertex, a, b, c, d);

            if (!CheckVertex("TestNearestVertex 첫번째 Test", vTarget, new Vertex3D(50.0, 50.0, 0.0)))
                return false;

            v1.z = v2.z = v3.z = 1.0;
            if (!UnE.Geometry.Math.MakePlane(v1, v2, v3, out a, out b, out c, out d))
                return false;

            vTarget = UnE.Geometry.Math.GetNearestVertex(vertex, a, b, c, d);

            if (!CheckVertex("TestNearestVertex 두번째 Test", vTarget, new Vertex3D(50.0, 50.0, 1.0)))
                return false;

            v1.SetVertex(1.0, 0.0, 0.0);
            v2.SetVertex(1.0, 0.0, 100.0);
            v3.SetVertex(1.0, 100.0, 0.0);

            if (!UnE.Geometry.Math.MakePlane(v1, v2, v3, out a, out b, out c, out d))
                return false;

            vTarget = UnE.Geometry.Math.GetNearestVertex(vertex, a, b, c, d);

            if (!CheckVertex("TestNearestVertex 세번째 Test", vTarget, new Vertex3D(1.0, 50.0, 50.0)))
                return false;

            v1.SetVertex(0.0, 1.0, 0.0);
            v2.SetVertex(100.0, 1.0, 0.0);
            v3.SetVertex(0.0, 1.0, 100.0);

            if (!UnE.Geometry.Math.MakePlane(v1, v2, v3, out a, out b, out c, out d))
                return false;

            vTarget = UnE.Geometry.Math.GetNearestVertex(vertex, a, b, c, d);

            if (!CheckVertex("TestNearestVertex 네번째 Test", vTarget, new Vertex3D(50.0, 1.0, 50.0)))
                return false;

            return true;
        }
    }
}
