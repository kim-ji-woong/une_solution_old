#include "stdafx.h"
#include "GeometryAPI.h"
#include "GVertex.h"
#include "GMath.h"
#include "GLine.h"
#include "GEArc.h"
#include "GArc.h"
#include <math.h>
#include <vector>

using namespace System;
using namespace System::Text;
using namespace System::Collections::Generic;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

using namespace UnE::Geometry;

namespace Geometrycpp
{
	[TestClass]
	public ref class MirrorTest
	{
	private:
		TestContext^ testContextInstance;

	public: 
		/// <summary>
		///현재 테스트 실행에 대한 정보 및 기능을
		///제공하는 테스트 컨텍스트를 가져오거나 설정합니다.
		///</summary>
		property Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ TestContext
		{
			Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ get()
			{
				return testContextInstance;
			}
			System::Void set(Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ value)
			{
				testContextInstance = value;
			}
		};

		#pragma region Additional test attributes
		//
		//테스트를 작성할 때 다음 추가 특성을 사용할 수 있습니다.
		//
		//ClassInitialize를 사용하여 클래스의 첫 번째 테스트를 실행하기 전에 코드를 실행합니다.
		//[ClassInitialize()]
		//static void MyClassInitialize(TestContext^ testContext) {};
		//
		//ClassCleanup을 사용하여 클래스의 테스트를 모두 실행한 후에 코드를 실행합니다.
		//[ClassCleanup()]
		//static void MyClassCleanup() {};
		//
		//TestInitialize를 사용하여 각 테스트를 실행하기 전에 코드를 실행합니다.
		//[TestInitialize()]
		//void MyTestInitialize() {};
		//
		//TestCleanup을 사용하여 각 테스트를 실행하기 전에 코드를 실행합니다.
		//[TestCleanup()]
		//void MyTestCleanup() {};
		//
		#pragma endregion 

		[TestMethod]
		void TestMirror()
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
		};

		bool ErrorMessage(String^ strMessage)
		{
			Assert::Inconclusive(strMessage);
			return false;
		}

		bool CheckVertex2D(String^ strTag, Vertex2D v, Vertex2D vResult)
        {
            if (v != vResult)
            {
                String^ strMsg = String::Format("{0} vTarget이 ({1}, {2})이어야 하나 ({3}, {4})이다.",
                    strTag, v.x, v.y, vResult.x, vResult.y);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckVertex3D(String^ strTag, Vertex3D v, Vertex3D vResult)
        {
            if (v != vResult)
            {
                String^ strMsg = String::Format("{0} vTarget이 ({1}, {2}, {3})이어야 하나 ({4}, {5}, {6})이다.",
                    strTag, v.x, v.y, v.z, vResult.x, vResult.y, vResult.z);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool TestVertex()
        {
            Vertex2D v1(0.0, 0.0);
            Vertex2D v2(10.0, 10.0);

            Vertex2D vLineBegin(5.0, 0.0);
            Vertex2D vLineEnd(5.0, 5.0);

            Vertex2D vResult;

            if (!v1.Mirror(vLineBegin, vLineEnd, vResult))
            {
                return ErrorMessage("Vertex2D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex2D("TestVertex 2D", Vertex2D(10.0, 0.0), vResult))
                return false;

            if (!v2.Mirror(vLineBegin, vLineEnd, vResult))
            {
                return ErrorMessage("Vertex2D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex2D("TestVertex 2D", Vertex2D(0.0, 10.0), vResult))
                return false;

            Vertex3D v3(0.0, 0.0, 0.0);
            Vertex3D v4(10.0, 10.0, 10.0);

            Vertex3D vLineBegin3D(5.0, 0.0, 0.0);
            Vertex3D vLineEnd3D(5.0, 5.0, 0.0);

            Vertex3D vPlane1(5.0, 0.0, 0.0);
            Vertex3D vPlane2(5.0, 5.0, 0.0);
            Vertex3D vPlane3(5.0, 5.0, 5.0);

            Vertex3D vResult3D;

            if (!v3.Mirror(vLineBegin3D, vLineEnd3D, vResult3D))
            {
                return ErrorMessage("Vertex3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex3D("TestVertex 3D", Vertex3D(10.0, 0.0, 0.0), vResult3D))
                return false;

            if (!v4.Mirror(vLineBegin3D, vLineEnd3D, vResult3D))
            {
                return ErrorMessage("Vertex3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex3D("TestVertex 3D", Vertex3D(0.0, 10.0, -10.0), vResult3D))
                return false;

            if (!v3.Mirror(vPlane1, vPlane2, vPlane3, vResult3D))
            {
                return ErrorMessage("Vertex3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex3D("TestVertex 3D", Vertex3D(10.0, 0.0, 0.0), vResult3D))
                return false;

            if (!v4.Mirror(vPlane1, vPlane2, vPlane3, vResult3D))
            {
                return ErrorMessage("Vertex3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckVertex3D("TestVertex 3D", Vertex3D(0.0, 10.0, 10.0), vResult3D))
                return false;

            return true;
        }

		bool CheckLine2D(String^ strTag, Line2D line, Line2D lineResult)
        {
            char* strLineType[] = { "LINE", "HALF_LINE_BEGIN_2_END", "HALF_LINE_END_2_BEGIN", "SEGMENT", "NO_LINE" };

            if (line.GetLineType() != lineResult.GetLineType())
            {
                String^ strMsg = String::Format("{0} LineType이 {1}이어야 하나 {2}이다.",
                    strTag, gcnew String(strLineType[(int)line.GetLineType()]), gcnew String(strLineType[(int)lineResult.GetLineType()]));
                return ErrorMessage(strMsg);
            }

            Vertex2D v1 = line.GetVertex(true);
            Vertex2D v2 = line.GetVertex(false);
            Vertex2D v3 = lineResult.GetVertex(true);
            Vertex2D v4 = lineResult.GetVertex(false);

            if (v1 != v3)
            {
                String^ strMsg = String::Format("{0} 시작점이 {{1}, {2})이어야 하나 ({3}, {4})이다.",
                    strTag, v1.x, v1.y, v3.x, v3.y);
                return ErrorMessage(strMsg);
            }

            if (v2 != v4)
            {
                String^ strMsg = String::Format("{0} 끝점이 {{1}, {2})이어야 하나 ({3}, {4})이다.",
                    strTag, v2.x, v2.y, v4.x, v4.y);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckLine3D(String^ strTag, Line3D line, Line3D lineResult)
        {
            char* strLineType[] = { "LINE", "HALF_LINE_BEGIN_2_END", "HALF_LINE_END_2_BEGIN", "SEGMENT", "NO_LINE" };

            if (line.GetLineType() != lineResult.GetLineType())
            {
                String^ strMsg = String::Format("{0} LineType이 {1}이어야 하나 {2}이다.",
                    strTag, gcnew String(strLineType[(int)line.GetLineType()]), gcnew String(strLineType[(int)lineResult.GetLineType()]));
                return ErrorMessage(strMsg);
            }

            Vertex3D v1 = line.GetVertex(true);
            Vertex3D v2 = line.GetVertex(false);
            Vertex3D v3 = lineResult.GetVertex(true);
            Vertex3D v4 = lineResult.GetVertex(false);

            if (v1 != v3)
            {
                String^ strMsg = String::Format("{0} 시작점이 {{1}, {2}, {3})이어야 하나 ({4}, {5}, {6})이다.",
                    strTag, v1.x, v1.y, v1.z, v3.x, v3.y, v3.z);
                return ErrorMessage(strMsg);
            }

            if (v2 != v4)
            {
                String^ strMsg = String::Format("{0} 끝점이 {{1}, {2}, {3})이어야 하나 ({4}, {5}, {6})이다.",
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
            Vertex2D v1(0.0, 0.0);
            Vertex2D v2(0.0, 10.0);
            Vertex2D v3(5.0, 0.0);
            Vertex2D v4(10.0, 5.0);

            Line2D line1(v1, v2);

            Line2D line1_1(Vertex2D(5.0, -5.0), Vertex2D(15.0, -5.0));

            Line2D lineResult;
            if (!line1.Mirror(v3, v4, lineResult))
            {
                return ErrorMessage("Line2D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckLine2D("TestLine2D", line1_1, lineResult))
                return false;

            return true;
        }

        bool TestLine3D()
        {
            Vertex3D v1(0.0, 0.0, 0.0);
            Vertex3D v2(0.0, 10.0, 0.0);

            Vertex3D v3(5.0, 0.0, 0.0);
            Vertex3D v4(10.0, 5.0, 0.0);
            Vertex3D v5(5.0, 0.0, 5.0);

            Line3D line1(v1, v2);

            Line3D line1_1(Vertex3D(5.0, -5.0, 0.0), Vertex3D(15.0, -5.0, 0.0));

            Line3D lineResult;
            if (!line1.Mirror(v3, v4, v5, lineResult))
            {
                return ErrorMessage("Line3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckLine3D("TestLine3D", line1_1, lineResult))
                return false;

            return true;
        }

		bool CheckEArc2D(String^ strTag, EArc2D earc1, EArc2D earc2, Vertex2D vTL, Vertex2D vBL, Vertex2D vBR)
        {
            if (earc1.IsClockWise() == earc2.IsClockWise())
            {
                String^ strMsg = String::Format("{0} EArc2D Mirror 변환후 ClockWise 속성이 반대가 되어야 하나 변함이 없다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            if (earc1.GetA() != earc2.GetA())
            {
                String^ strMsg = String::Format("{0} EArc2D Mirror 변환후 A 속성이 변하였다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            if (earc1.GetB() != earc2.GetB())
            {
                String^ strMsg = String::Format("{0} EArc2D Mirror 변환후 B 속성이 변하였다.",
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

        bool CheckEArc3D(String^ strTag, EArc3D earc1, EArc3D earc2, Vertex3D vTL, Vertex3D vBL, Vertex3D vBR)
        {
            if (earc1.IsClockWise() == earc2.IsClockWise())
            {
                String^ strMsg = String::Format("{0} EArc3D Mirror 변환후 ClockWise 속성이 반대가 되어야 하나 변함이 없다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            if (earc1.GetA() != earc2.GetA())
            {
                String^ strMsg = String::Format("{0} EArc3D Mirror 변환후 A 속성이 변하였다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            if (earc1.GetB() != earc2.GetB())
            {
                String^ strMsg = String::Format("{0} EArc3D Mirror 변환후 B 속성이 변하였다.",
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
            Vertex2D vTL(20.0, 20.0);
            Vertex2D vBL(20.0, -20.0);
            Vertex2D vBR(80.0, -20.0);

            EArc2D earc(vTL, vBL, vBR, 6.0, 3.5, false);

            Vertex2D vLineBegin(0.0, 0.0);
            Vertex2D vLineEnd(10.0, 10.0);

            EArc2D earcResult;

            if (!earc.Mirror(vLineBegin, vLineEnd, earcResult))
            {
                return ErrorMessage("EArc2D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            Vertex2D vTL1(20.0, 80.0);
            Vertex2D vBL1(-20.0, 80.0);
            Vertex2D vBR1(-20.0, 20.0);

            if (!CheckEArc2D("TestEArc2D", earc, earcResult, vTL1, vBL1, vBR1))
                return false;

            return true;
        }

        bool TestEArc3D()
        {
            Vertex3D vTL(20.0, 20.0, 0.0);
            Vertex3D vBL(20.0, -20.0, 0.0);
            Vertex3D vBR(80.0, -20.0, 0.0);

            EArc3D earc(vTL, vBL, vBR, 6.0, 3.5, false);

            Vertex3D v1(0.0, 0.0, 0.0);
            Vertex3D v2(10.0, 10.0, 0.0);
            Vertex3D v3(0.0, 0.0, 10.0);

            EArc3D earcResult;

            if (!earc.Mirror(v1, v2, v3, earcResult))
            {
                return ErrorMessage("EArc3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            Vertex3D vTL1(20.0, 80.0, 0.0);
            Vertex3D vBL1(-20.0, 80.0, 0.0);
            Vertex3D vBR1(-20.0, 20.0, 0.0);

            if (!CheckEArc3D("TestEArc3D", earc, earcResult, vTL1, vBL1, vBR1))
                return false;

            return true;
        }

		bool CheckArc2D(String^ strTag, Arc2D arc1, Arc2D arc2, Vertex2D vTL, Vertex2D vBL, Vertex2D vBR, Vertex2D vArcBegin, Vertex2D vArcEnd)
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
                String^ strMsg = String::Format("{0} Arc2D Mirror 변환후 반지름이 변하였다.",
                    strTag);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckArc3D(String^ strTag, Arc3D arc1, Arc3D arc2, Vertex3D vTL, Vertex3D vBL, Vertex3D vBR, Vertex3D vArcBegin, Vertex3D vArcEnd)
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
                String^ strMsg = String::Format("{0} Arc3D Mirror 변환후 반지름이 변하였다.",
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
            Vertex2D v1(50.0, 25.0);
            Vertex2D v2(75.0, 0.0);
            Vertex2D v3(50.0, -25.0);

            Vertex2D vLineBegin(0.0, 0.0);
            Vertex2D vLineEnd(10.0, 10.0);

            Arc2D arc(v1, v2, v3);

            Vertex2D vArcBegin(25.0, 50.0);
            Vertex2D vArcEnd(-25.0, 50.0);

            Vertex2D vTL(-25.0, 75.0);
            Vertex2D vBL(-25.0, 25.0);
            Vertex2D vBR(25.0, 25.0);
            Arc2D arcResult;
            
            if (!arc.Mirror(vLineBegin, vLineEnd, arcResult))
            {
                return ErrorMessage("Arc2D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckArc2D("TestArc2D", arc, arcResult, vTL, vBL, vBR, vArcBegin, vArcEnd))
                return false;

            return true;
        }

        bool TestArc3D()
        {
            Vertex3D v1(50.0, 25.0, 0.0);
            Vertex3D v2(75.0, 0.0, 0.0);
            Vertex3D v3(50.0, -25.0, 0.0);

            Vertex3D vPlane1(0.0, 0.0, 0.0);
            Vertex3D vPlane2(10.0, 10.0, 0.0);
            Vertex3D vPlane3(0.0, 0.0, 10.0);

            Arc3D arc(v1, v2, v3);

            Vertex3D vArcBegin(25.0, 50.0, 0.0);
            Vertex3D vArcEnd(-25.0, 50.0, 0.0);

            Vertex3D vTL, vBL, vBR;
            Arc3D arcResult;

            Vertex3D _vTR = arc.GetTL() + arc.GetBR() - arc.GetBL();

            // Mirror 연산이므로 좌우가 바뀐다.
            _vTR.Mirror(vPlane1, vPlane2, vPlane3, vTL);
            arc.GetBR().Mirror(vPlane1, vPlane2, vPlane3, vBL);
            arc.GetBL().Mirror(vPlane1, vPlane2, vPlane3, vBR);

            if (!arc.Mirror(vPlane1, vPlane2, vPlane3, arcResult))
            {
                return ErrorMessage("Arc3D의 Mirror() 함수 호출이 실패하였습니다.");
            }

            if (!CheckArc3D("TestArc3D", arc, arcResult, vTL, vBL, vBR, vArcBegin, vArcEnd))
                return false;

            return true;
        }
	};
}
