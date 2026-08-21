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
    public class MirrorTest
    {
        public MirrorTest()
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
        public void TestMirror()
        {
            //
            // TODO: 테스트 논리를 여기에 추가합니다.
            //
            if (!TestVertex())
                return;
            if (!TestLine())
                return;
            if (!TestEArc())
                return;
            if (!TestArc())
                return;
        }

        bool ErrorMessage(String strMessage)
		{
			Assert.Inconclusive(strMessage);
			return false;
		}

        bool CheckVertex2D(String strTag, Vertex2D v, Vertex2D vResult)
        {
            if (v != vResult)
            {
                String strMsg = String.Format("{0} vTarget이 ({1}, {2})이어야 하나 ({3}, {4})이다.",
                    strTag, v.x, v.y, vResult.x, vResult.y);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckVertex3D(String strTag, Vertex3D v, Vertex3D vResult)
        {
            if (v != vResult)
            {
                String strMsg = String.Format("{0} vTarget이 ({1}, {2}, {3})이어야 하나 ({4}, {5}, {6})이다.",
                    strTag, v.x, v.y, v.z, vResult.x, vResult.y, vResult.z);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool TestVertex()
        {
            Vertex2D v1 = new Vertex2D(0.0, 0.0);
            Vertex2D v2 = new Vertex2D(10.0, 10.0);

            Vertex2D vLineBegin = new Vertex2D(5.0, 0.0);
            Vertex2D vLineEnd = new Vertex2D(5.0, 5.0);

            Vertex2D vResult;

            if (!v1.Mirror(vLineBegin, vLineEnd, out vResult))
            {
                return ErrorMessage("Vertex2D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex2D("TestVertex 2D", new Vertex2D(10.0, 0.0), vResult))
                return false;

            if (!v2.Mirror(vLineBegin, vLineEnd, out vResult))
            {
                return ErrorMessage("Vertex2D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex2D("TestVertex 2D", new Vertex2D(0.0, 10.0), vResult))
                return false;

            Vertex3D v3 = new Vertex3D(0.0, 0.0, 0.0);
            Vertex3D v4 = new Vertex3D(10.0, 10.0, 10.0);

            Vertex3D vLineBegin3D = new Vertex3D(5.0, 0.0, 0.0);
            Vertex3D vLineEnd3D = new Vertex3D(5.0, 5.0, 0.0);

            Vertex3D vPlane1 = new Vertex3D(5.0, 0.0, 0.0);
            Vertex3D vPlane2 = new Vertex3D(5.0, 5.0, 0.0);
            Vertex3D vPlane3 = new Vertex3D(5.0, 5.0, 5.0);

            Vertex3D vResult3D;

            if (!v3.Mirror(vLineBegin3D, vLineEnd3D, out vResult3D))
            {
                return ErrorMessage("Vertex3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex3D("TestVertex 3D", new Vertex3D(10.0, 0.0, 0.0), vResult3D))
                return false;

            if (!v4.Mirror(vLineBegin3D, vLineEnd3D, out vResult3D))
            {
                return ErrorMessage("Vertex3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex3D("TestVertex 3D", new Vertex3D(0.0, 10.0, -10.0), vResult3D))
                return false;

            if (!v3.Mirror(vPlane1, vPlane2, vPlane3, out vResult3D))
            {
                return ErrorMessage("Vertex3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex3D("TestVertex 3D", new Vertex3D(10.0, 0.0, 0.0), vResult3D))
                return false;

            if (!v4.Mirror(vPlane1, vPlane2, vPlane3, out vResult3D))
            {
                return ErrorMessage("Vertex3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex3D("TestVertex 3D", new Vertex3D(0.0, 10.0, 10.0), vResult3D))
                return false;

            return true;
        }

        bool CheckLine2D(String strTag, Line2D line, Line2D lineResult)
        {
            string[] strLineType = new string[] { "LINE", "HALF_LINE_BEGIN_2_END", "HALF_LINE_END_2_BEGIN", "SEGMENT", "NO_LINE" };

            if (line.GetLineType() != lineResult.GetLineType())
            {
                String strMsg = String.Format("{0} LineType이 {1}이어야 하나 {2}이다.",
                    strTag, strLineType[(int)line.GetLineType()], strLineType[(int)lineResult.GetLineType()]);
                return ErrorMessage(strMsg);
            }

            Vertex2D v1 = line.GetVertex(true);
            Vertex2D v2 = line.GetVertex(false);
            Vertex2D v3 = lineResult.GetVertex(true);
            Vertex2D v4 = lineResult.GetVertex(false);

            if (v1 != v3)
            {
                String strMsg = String.Format("{0} 시작점이 {{1}, {2})이어야 하나 ({3}, {4})이다.",
                    strTag, v1.x, v1.y, v3.x, v3.y);
                return ErrorMessage(strMsg);
            }

            if (v2 != v4)
            {
                String strMsg = String.Format("{0} 끝점이 {{1}, {2})이어야 하나 ({3}, {4})이다.",
                    strTag, v2.x, v2.y, v4.x, v4.y);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckLine3D(String strTag, Line3D line, Line3D lineResult)
        {
            string[] strLineType = new string[] { "LINE", "HALF_LINE_BEGIN_2_END", "HALF_LINE_END_2_BEGIN", "SEGMENT", "NO_LINE" };

            if (line.GetLineType() != lineResult.GetLineType())
            {
                String strMsg = String.Format("{0} LineType이 {1}이어야 하나 {2}이다.",
                    strTag, strLineType[(int)line.GetLineType()], strLineType[(int)lineResult.GetLineType()]);
                return ErrorMessage(strMsg);
            }

            Vertex3D v1 = line.GetVertex(true);
            Vertex3D v2 = line.GetVertex(false);
            Vertex3D v3 = lineResult.GetVertex(true);
            Vertex3D v4 = lineResult.GetVertex(false);

            if (v1 != v3)
            {
                String strMsg = String.Format("{0} 시작점이 {{1}, {2}, {3})이어야 하나 ({4}, {5}, {6})이다.",
                    strTag, v1.x, v1.y, v1.z, v3.x, v3.y, v3.z);
                return ErrorMessage(strMsg);
            }

            if (v2 != v4)
            {
                String strMsg = String.Format("{0} 끝점이 {{1}, {2}, {3})이어야 하나 ({4}, {5}, {6})이다.",
                    strTag, v2.x, v2.y, v2.z, v4.x, v4.y, v4.z);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool TestLine()
        {
            if (!TestLine2D())
                return false;
            if (!TestLine3D())
                return false;

            return true;
        }

        bool TestLine2D()
        {
            Vertex2D v1 = new Vertex2D(0.0, 0.0);
            Vertex2D v2 = new Vertex2D(0.0, 10.0);
            Vertex2D v3 = new Vertex2D(5.0, 0.0);
            Vertex2D v4 = new Vertex2D(10.0, 5.0);

            Line2D line1 = new Line2D(v1, v2);

            Line2D line1_1 = new Line2D(new Vertex2D(5.0, -5.0), new Vertex2D(15.0, -5.0));

            Line2D lineResult;
            if (!line1.Mirror(v3, v4, out lineResult))
            {
                return ErrorMessage("Line2D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckLine2D("TestLine2D", line1_1, lineResult))
                return false;

            return true;
        }

        bool TestLine3D()
        {
            Vertex3D v1 = new Vertex3D(0.0, 0.0, 0.0);
            Vertex3D v2 = new Vertex3D(0.0, 10.0, 0.0);

            Vertex3D v3 = new Vertex3D(5.0, 0.0, 0.0);
            Vertex3D v4 = new Vertex3D(10.0, 5.0, 0.0);
            Vertex3D v5 = new Vertex3D(5.0, 0.0, 5.0);

            Line3D line1 = new Line3D(v1, v2);

            Line3D line1_1 = new Line3D(new Vertex3D(5.0, -5.0, 0.0), new Vertex3D(15.0, -5.0, 0.0));

            Line3D lineResult;
            if (!line1.Mirror(v3, v4, v5, out lineResult))
            {
                return ErrorMessage("Line3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckLine3D("TestLine3D", line1_1, lineResult))
                return false;

            return true;
        }

        bool CheckEArc2D(String strTag, EArc2D earc1, EArc2D earc2, Vertex2D vTL, Vertex2D vBL, Vertex2D vBR)
        {
            if (earc1.IsClockWise() == earc2.IsClockWise())
            {
                String strMsg = String.Format("{0} EArc2D Mirror 변환후 ClockWise 속성이 반대가 되어야 하나 변함이 없다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            if (earc1.GetA() != earc2.GetA())
            {
                String strMsg = String.Format("{0} EArc2D Mirror 변환후 A 속성이 변하였다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            if (earc1.GetB() != earc2.GetB())
            {
                String strMsg = String.Format("{0} EArc2D Mirror 변환후 B 속성이 변하였다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            Vertex2D _vTL = earc2.GetTL();
            Vertex2D _vBL = earc2.GetBL();
            Vertex2D _vBR = earc2.GetBR();

            if (!CheckVertex2D(strTag + " TL", vTL, _vTL))
                return false;
            if (!CheckVertex2D(strTag + " BL", vBL, _vBL))
                return false;
            if (!CheckVertex2D(strTag + " BR", vBR, _vBR))
                return false;

            return true;
        }

        bool CheckEArc3D(String strTag, EArc3D earc1, EArc3D earc2, Vertex3D vTL, Vertex3D vBL, Vertex3D vBR)
        {
            if (earc1.IsClockWise() == earc2.IsClockWise())
            {
                String strMsg = String.Format("{0} EArc3D Mirror 변환후 ClockWise 속성이 반대가 되어야 하나 변함이 없다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            if (earc1.GetA() != earc2.GetA())
            {
                String strMsg = String.Format("{0} EArc3D Mirror 변환후 A 속성이 변하였다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            if (earc1.GetB() != earc2.GetB())
            {
                String strMsg = String.Format("{0} EArc3D Mirror 변환후 B 속성이 변하였다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            Vertex3D _vTL = earc2.GetTL();
            Vertex3D _vBL = earc2.GetBL();
            Vertex3D _vBR = earc2.GetBR();

            if (!CheckVertex3D(strTag + " TL", vTL, _vTL))
                return false;
            if (!CheckVertex3D(strTag + " BL", vBL, _vBL))
                return false;
            if (!CheckVertex3D(strTag + " BR", vBR, _vBR))
                return false;

            return true;
        }

        bool TestEArc()
        {
            if (!TestEArc2D())
                return false;
            if (!TestEArc3D())
                return false;

            return true;
        }

        bool TestEArc2D()
        {
            Vertex2D vTL = new Vertex2D(20.0, 20.0);
            Vertex2D vBL = new Vertex2D(20.0, -20.0);
            Vertex2D vBR = new Vertex2D(80.0, -20.0);

            EArc2D earc = new EArc2D(vTL, vBL, vBR, 6.0, 3.5, false);

            Vertex2D vLineBegin = new Vertex2D(0.0, 0.0);
            Vertex2D vLineEnd = new Vertex2D(10.0, 10.0);

            EArc2D earcResult;

            if (!earc.Mirror(vLineBegin, vLineEnd, out earcResult))
            {
                return ErrorMessage("EArc2D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            Vertex2D vTL1 = new Vertex2D(20.0, 80.0);
            Vertex2D vBL1 = new Vertex2D(-20.0, 80.0);
            Vertex2D vBR1 = new Vertex2D(-20.0, 20.0);

            if (!CheckEArc2D("TestEArc2D", earc, earcResult, vTL1, vBL1, vBR1))
                return false;

            return true;
        }

        bool TestEArc3D()
        {
            Vertex3D vTL = new Vertex3D(20.0, 20.0, 0.0);
            Vertex3D vBL = new Vertex3D(20.0, -20.0, 0.0);
            Vertex3D vBR = new Vertex3D(80.0, -20.0, 0.0);

            EArc3D earc = new EArc3D(vTL, vBL, vBR, 6.0, 3.5, false);

            Vertex3D v1 = new Vertex3D(0.0, 0.0, 0.0);
            Vertex3D v2 = new Vertex3D(10.0, 10.0, 0.0);
            Vertex3D v3 = new Vertex3D(0.0, 0.0, 10.0);

            EArc3D earcResult;

            if (!earc.Mirror(v1, v2, v3, out earcResult))
            {
                return ErrorMessage("EArc3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            Vertex3D vTL1 = new Vertex3D(20.0, 80.0, 0.0);
            Vertex3D vBL1 = new Vertex3D(-20.0, 80.0, 0.0);
            Vertex3D vBR1 = new Vertex3D(-20.0, 20.0, 0.0);

            if (!CheckEArc3D("TestEArc3D", earc, earcResult, vTL1, vBL1, vBR1))
                return false;

            return true;
        }

        bool CheckArc2D(String strTag, Arc2D arc1, Arc2D arc2, Vertex2D vTL, Vertex2D vBL, Vertex2D vBR, Vertex2D vArcBegin, Vertex2D vArcEnd)
        {
            if (!CheckEArc2D(strTag, arc1, arc2, vTL, vBL, vBR))
                return false;

            Vertex2D vBegin1 = arc2.GetBeginVertex();
            Vertex2D vEnd1 = arc2.GetEndVertex();

            if (!CheckVertex2D(strTag + " BeginVertex", vArcBegin, vBegin1))
                return false;
            if (!CheckVertex2D(strTag + " EndVertex", vArcEnd, vEnd1))
                return false;

            if (arc1.GetRadius() != arc2.GetRadius())
            {
                String strMsg = String.Format("{0} Arc2D Mirror 변환후 반지름이 변하였다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckArc3D(String strTag, Arc3D arc1, Arc3D arc2, Vertex3D vTL, Vertex3D vBL, Vertex3D vBR, Vertex3D vArcBegin, Vertex3D vArcEnd)
        {
            if (!CheckEArc3D(strTag, arc1, arc2, vTL, vBL, vBR))
                return false;

            Vertex3D vBegin1 = arc2.GetBeginVertex();
            Vertex3D vEnd1 = arc2.GetEndVertex();

            if (!CheckVertex3D(strTag + " BeginVertex", vArcBegin, vBegin1))
                return false;
            if (!CheckVertex3D(strTag + " EndVertex", vArcEnd, vEnd1))
                return false;

            if (arc1.GetRadius() != arc2.GetRadius())
            {
                String strMsg = String.Format("{0} Arc3D Mirror 변환후 반지름이 변하였다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool TestArc()
        {
            if (!TestArc2D())
                return false;
            if (!TestArc3D())
                return false;

            return true;
        }

        bool TestArc2D()
        {
            Vertex2D v1 = new Vertex2D(50.0, 25.0);
            Vertex2D v2 = new Vertex2D(75.0, 0.0);
            Vertex2D v3 = new Vertex2D(50.0, -25.0);

            Vertex2D vLineBegin = new Vertex2D(0.0, 0.0);
            Vertex2D vLineEnd = new Vertex2D(10.0, 10.0);

            Arc2D arc = new Arc2D(v1, v2, v3);

            Vertex2D vArcBegin = new Vertex2D(25.0, 50.0);
            Vertex2D vArcEnd = new Vertex2D(-25.0, 50.0);

            Vertex2D vTL = new Vertex2D(-25.0, 75.0);
            Vertex2D vBL = new Vertex2D(-25.0, 25.0);
            Vertex2D vBR = new Vertex2D(25.0, 25.0);
            Arc2D arcResult;
            
            if (!arc.Mirror(vLineBegin, vLineEnd, out arcResult))
            {
                return ErrorMessage("Arc2D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckArc2D("TestArc2D", arc, arcResult, vTL, vBL, vBR, vArcBegin, vArcEnd))
                return false;

            return true;
        }

        bool TestArc3D()
        {
            Vertex3D v1 = new Vertex3D(50.0, 25.0, 0.0);
            Vertex3D v2 = new Vertex3D(75.0, 0.0, 0.0);
            Vertex3D v3 = new Vertex3D(50.0, -25.0, 0.0);

            Vertex3D vPlane1 = new Vertex3D(0.0, 0.0, 0.0);
            Vertex3D vPlane2 = new Vertex3D(10.0, 10.0, 0.0);
            Vertex3D vPlane3 = new Vertex3D(0.0, 0.0, 10.0);

            Arc3D arc = new Arc3D(v1, v2, v3);

            Vertex3D vArcBegin = new Vertex3D(25.0, 50.0, 0.0);
            Vertex3D vArcEnd = new Vertex3D(-25.0, 50.0, 0.0);

            Vertex3D vTL, vBL, vBR;
            Arc3D arcResult;

            Vertex3D _vTR = arc.GetTL() + arc.GetBR() - arc.GetBL();

            // Mirror 연산이므로 좌우가 바뀐다.
            _vTR.Mirror(vPlane1, vPlane2, vPlane3, out vTL);
            arc.GetBR().Mirror(vPlane1, vPlane2, vPlane3, out vBL);
            arc.GetBL().Mirror(vPlane1, vPlane2, vPlane3, out vBR);

            if (!arc.Mirror(vPlane1, vPlane2, vPlane3, out arcResult))
            {
                return ErrorMessage("Arc3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckArc3D("TestArc3D", arc, arcResult, vTL, vBL, vBR, vArcBegin, vArcEnd))
                return false;

            return true;
        }
    }
}
