#include "stdafx.h"
#include "GeometryAPI.h"
#include "GVertex.h"
#include "GMath.h"
#include "GLine.h"
#include "GEArc.h"
#include "GArc.h"
#include <Math.h>
#include <vector>

using namespace System;
using namespace System::Text;
using namespace System::Collections::Generic;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

using namespace UnE::Geometry;

namespace Geometrycpp
{
	[TestClass]
	public ref class OffsetTest
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
		void OffsetIntersect()
		{
			//
			// TODO: 테스트 논리를 여기에 추가합니다.
			//
			if (!TestLine2D())
                return;
            if (!TestLine3D())
                return;
            if (!TestEArc2D())
                return;
            if (!TestEArc3D())
                return;
            if (!TestArc2D())
                return;
            if (!TestArc3D())
                return;
		};

		bool ErrorMessage(String^ strMessage)
		{
			Assert::Inconclusive(strMessage);
			return false;
		}

		bool CheckOffset(String^ strTag, Line2D& line1, Line2D& line2)
        {
            wchar_t* strLineType[] = {L"LINE", L"HALF_LINE_BEGIN_2_END", L"HALF_LINE_END_2_BEGIN", L"SEGMENT"};

            if (line1.GetLineType() != line2.GetLineType())
            {
                String^ strMsg = String::Format(strTag + L" line1의 LineType이 {0}이어야 하나 {1}이다",
                    gcnew String(strLineType[(int)line2.GetLineType()]), gcnew String(strLineType[(int)line1.GetLineType()]));
                return ErrorMessage(strMsg);
            }

            Vertex2D vBegin1 = line1.GetVertex(true);
            Vertex2D vEnd1   = line1.GetVertex(false);
            Vertex2D vBegin2 = line2.GetVertex(true);
            Vertex2D vEnd2 =   line2.GetVertex(false);

            if (vBegin1 != vBegin2)
            {
                String^ strMsg = String::Format(strTag + " line1의 시작점이 ({0}, {1})이어야 하나 ({2}, {3})이다",
                    vBegin2.x, vBegin2.y, vBegin1.x, vBegin1.y);
                return ErrorMessage(strMsg);
            }

            if (vEnd1 != vEnd2)
            {
                String^ strMsg = String::Format(strTag + " line1의 끝점이 ({0}, {1})이어야 하나 ({2}, {3})이다",
                    vEnd2.x, vEnd2.y, vEnd1.x, vEnd1.y);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckOffset(String^ strTag, Line3D& line1, Line3D& line2)
        {
            char* strLineType[] = { "LINE", "HALF_LINE_BEGIN_2_END", "HALF_LINE_END_2_BEGIN", "SEGMENT" };

            if (line1.GetLineType() != line2.GetLineType())
            {
                String^ strMsg = String::Format(strTag + " line1의 LineType이 {0}이어야 하나 {1}이다",
                    gcnew String(strLineType[(int)line2.GetLineType()]), gcnew String(strLineType[(int)line1.GetLineType()]));
                return ErrorMessage(strMsg);
            }

            Vertex3D vBegin1 = line1.GetVertex(true);
            Vertex3D vEnd1 = line1.GetVertex(false);
            Vertex3D vBegin2 = line2.GetVertex(true);
            Vertex3D vEnd2 = line2.GetVertex(false);

            if (vBegin1 != vBegin2)
            {
                String^ strMsg = String::Format(strTag + " line1의 시작점이 ({0}, {1}, {2})이어야 하나 ({3}, {4}, {5})이다",
                    vBegin2.x, vBegin2.y, vBegin2.z, vBegin1.x, vBegin1.y, vBegin1.z);
                return ErrorMessage(strMsg);
            }

            if (vEnd1 != vEnd2)
            {
                String^ strMsg = String::Format(strTag + " line1의 끝점이 ({0}, {1}, {2})이어야 하나 ({3}, {4}, {5})이다",
                    vEnd2.x, vEnd2.y, vEnd2.z, vEnd1.x, vEnd1.y, vEnd1.z);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool ErrorOffset(String^ strTag, Vertex2D v1, Vertex2D v2)
        {
            String^ strMsg = String::Format("{0}이 ({1}, {2})이어야 하나 ({3}, {4})이다.",
                strTag, v2.x, v2.y, v1.x, v1.y);
            return ErrorMessage(strMsg);
        }

        bool ErrorOffset(String^ strTag, Vertex3D v1, Vertex3D v2)
        {
            String^ strMsg = String::Format("{0}이 ({1}, {2})이어야 하나 ({3}, {4})이다.",
                strTag, v2.x, v2.y, v1.x, v1.y);
            return ErrorMessage(strMsg);
        }

        bool CheckOffset(String^ strTag, EArc2D& earc1, EArc2D& earc2)
        {
            double a1 = earc1.GetA();
            double angle1 = earc1.GetAngle();
            double b1 = earc1.GetB();
            double beginAngle1 = earc1.GetBeginAngle();
            Vertex2D vBegin1 = earc1.GetBeginVertex();
            Vertex2D vBL1 = earc1.GetBL();
            Vertex2D vBR1 = earc1.GetBR();
            Vertex2D vCenter1 = earc1.GetCenter();
            double endAngle1 = earc1.GetEndAngle();
            Vertex2D vEnd1 = earc1.GetEndVertex();
            Vertex2D vTL1 = earc1.GetTL();
            EArc2D::EArcType type1 = earc1.GetType();
            bool clockWise1 = earc1.IsClockWise();
            bool closed1 = earc1.IsClosed();

            double a2 = earc2.GetA();
            double angle2 = earc2.GetAngle();
            double b2 = earc2.GetB();
            double beginAngle2 = earc2.GetBeginAngle();
            Vertex2D vBegin2 = earc2.GetBeginVertex();
            Vertex2D vBL2 = earc2.GetBL();
            Vertex2D vBR2 = earc2.GetBR();
            Vertex2D vCenter2 = earc2.GetCenter();
            double endAngle2 = earc2.GetEndAngle();
            Vertex2D vEnd2 = earc2.GetEndVertex();
            Vertex2D vTL2 = earc2.GetTL();
            EArc2D::EArcType type2 = earc2.GetType();
            bool clockWise2 = earc2.IsClockWise();
            bool closed2 = earc2.IsClosed();

            if (vBegin1 != vBegin2)
            {
                return ErrorOffset(strTag + " earc1의 시작점", vBegin1, vBegin2);
            }

            if (vEnd1 != vEnd2)
            {
                return ErrorOffset(strTag + " earc1의 끝점", vEnd1, vEnd2);
            }

            if (vTL1 != vTL2)
            {
                return ErrorOffset(strTag + " earc1의 TL", vTL1, vTL2);
            }

            if (vBL1 != vBL2)
            {
                return ErrorOffset(strTag + " earc1의 BL", vBL1, vBL2);
            }

            if (vBR1 != vBR2)
            {
                return ErrorOffset(strTag + " earc1의 BR", vBR1, vBR2);
            }

            if (System::Math::Abs(a1 - a2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " earc1의 A가 {0}이어야 하나 {1}이다.",
                    a2, a1);
                return ErrorMessage(strMsg);
            }

            if (System::Math::Abs(b1 - b2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " earc1의 B가 {0}이어야 하나 {1}이다.",
                    b2, b1);
                return ErrorMessage(strMsg);
            }

            if (System::Math::Abs(angle1 - angle2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " earc1의 각도가 {0}이어야 하나 {1}이다.",
                    angle2, angle1);
                return ErrorMessage(strMsg);
            }

            if (System::Math::Abs(beginAngle1 - beginAngle2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " earc1의 시작각도가 {0}이어야 하나 {1}이다.",
                    beginAngle2, beginAngle1);
                return ErrorMessage(strMsg);
            }

            if (System::Math::Abs(endAngle1 - endAngle2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " earc1의 타원 각도가 {0}이어야 하나 {1}이다.",
                    endAngle2, endAngle1);
                return ErrorMessage(strMsg);
            }

            if (clockWise1 != clockWise2)
            {
                String^ strMsg = String::Format(strTag + " earc1의 타원 진행방향이 {0}이어야 하나 {1}이다.",
                    clockWise1 ? "시계방향" : "반시계방향", clockWise2 ? "시계방향" : "반시계방향");
                return ErrorMessage(strMsg);
            }

            if (closed1 != closed2)
            {
                String^ strMsg = String::Format(strTag + " earc1는 {0}이어야 하나 {1}이다.",
                    closed1 ? "완전히 닫힌 상태" : "열린 상태", closed2 ? "완전히 닫힌 상태" : "열린 상태");
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckOffset(String^ strTag, EArc3D& earc1, EArc3D& earc2)
        {
            double a1 = earc1.GetA();
            double angle1 = earc1.GetAngle();
            double b1 = earc1.GetB();
            double beginAngle1 = earc1.GetBeginAngle();
            Vertex3D vBegin1 = earc1.GetBeginVertex();
            Vertex3D vBL1 = earc1.GetBL();
            Vertex3D vBR1 = earc1.GetBR();
            Vertex3D vCenter1 = earc1.GetCenter();
            double endAngle1 = earc1.GetEndAngle();
            Vertex3D vEnd1 = earc1.GetEndVertex();
            Vertex3D vTL1 = earc1.GetTL();
            EArc3D::EArcType type1 = earc1.GetType();
            bool clockWise1 = earc1.IsClockWise();
            bool closed1 = earc1.IsClosed();

            double a2 = earc2.GetA();
            double angle2 = earc2.GetAngle();
            double b2 = earc2.GetB();
            double beginAngle2 = earc2.GetBeginAngle();
            Vertex3D vBegin2 = earc2.GetBeginVertex();
            Vertex3D vBL2 = earc2.GetBL();
            Vertex3D vBR2 = earc2.GetBR();
            Vertex3D vCenter2 = earc2.GetCenter();
            double endAngle2 = earc2.GetEndAngle();
            Vertex3D vEnd2 = earc2.GetEndVertex();
            Vertex3D vTL2 = earc2.GetTL();
            EArc3D::EArcType type2 = earc2.GetType();
            bool clockWise2 = earc2.IsClockWise();
            bool closed2 = earc2.IsClosed();

            if (vBegin1 != vBegin2)
            {
                return ErrorOffset(strTag + " earc1의 시작점", vBegin1, vBegin2);
            }

            if (vEnd1 != vEnd2)
            {
                return ErrorOffset(strTag + " earc1의 끝점", vEnd1, vEnd2);
            }

            if (vTL1 != vTL2)
            {
                return ErrorOffset(strTag + " earc1의 TL", vTL1, vTL2);
            }

            if (vBL1 != vBL2)
            {
                return ErrorOffset(strTag + " earc1의 BL", vBL1, vBL2);
            }

            if (vBR1 != vBR2)
            {
                return ErrorOffset(strTag + " earc1의 BR", vBR1, vBR2);
            }

            if (System::Math::Abs(a1 - a2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " earc1의 A가 {0}이어야 하나 {1}이다.",
                    a2, a1);
                return ErrorMessage(strMsg);
            }

            if (System::Math::Abs(b1 - b2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " earc1의 B가 {0}이어야 하나 {1}이다.",
                    b2, b1);
                return ErrorMessage(strMsg);
            }

            if (System::Math::Abs(angle1 - angle2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " earc1의 각도가 {0}이어야 하나 {1}이다.",
                    angle2, angle1);
                return ErrorMessage(strMsg);
            }

            if (System::Math::Abs(beginAngle1 - beginAngle2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " earc1의 시작각도가 {0}이어야 하나 {1}이다.",
                    beginAngle2, beginAngle1);
                return ErrorMessage(strMsg);
            }

            if (System::Math::Abs(endAngle1 - endAngle2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " earc1의 타원 각도가 {0}이어야 하나 {1}이다.",
                    endAngle2, endAngle1);
                return ErrorMessage(strMsg);
            }

            if (clockWise1 != clockWise2)
            {
                String^ strMsg = String::Format(strTag + " earc1의 타원 진행방향이 {0}이어야 하나 {1}이다.",
                    clockWise1 ? "시계방향" : "반시계방향", clockWise2 ? "시계방향" : "반시계방향");
                return ErrorMessage(strMsg);
            }

            if (closed1 != closed2)
            {
                String^ strMsg = String::Format(strTag + " earc1는 {0}이어야 하나 {1}이다.",
                    closed1 ? "완전히 닫힌 상태" : "열린 상태", closed2 ? "완전히 닫힌 상태" : "열린 상태");
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckOffset(String^ strTag, Arc2D& arc1, Arc2D& arc2)
        {
            if (!CheckOffset(strTag, (EArc2D&)arc1, (EArc2D&)arc2))
                return false;

            double dRadius1 = arc1.GetRadius();
            double dRadius2 = arc2.GetRadius();

            if (System::Math::Abs(dRadius1 - dRadius2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " arc1의 반지름이 {0}이어야 하나 {1}이다.",
                    dRadius2, dRadius1);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool CheckOffset(String^ strTag, Arc3D& arc1, Arc3D& arc2)
        {
            if (!CheckOffset(strTag, (EArc3D&)arc1, (EArc3D&)arc2))
                return false;

            double dRadius1 = arc1.GetRadius();
            double dRadius2 = arc2.GetRadius();

            if (System::Math::Abs(dRadius1 - dRadius2) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                String^ strMsg = String::Format(strTag + " arc1의 반지름이 {0}이어야 하나 {1}이다.",
                    dRadius2, dRadius1);
                return ErrorMessage(strMsg);
            }

            return true;
        }

        bool TestLine2D()
        {
            Vertex2D v1(0.0, 0.0);
            Vertex2D v2(100.0, 0.0);

            Line2D line1(v1, v2, Line2D::LINE);
            Line2D halfLine1(v1, v2, Line2D::HALF_LINE_BEGIN_2_END);
            Line2D seg1(v1, v2, Line2D::SEGMENT);

            Line2D line1_1 = line1.Offset(Vertex2D(50.0, 50.0), 30.0);
            Line2D line1_2 = line1.Offset(true, 30.0);
            Line2D line1_3 = line1.Offset(false, 30.0);

            Line2D line1_1Result(Vertex2D(0.0, 30.0), Vertex2D(100.0, 30.0), Line2D::LINE);
            Line2D line1_2Result(Vertex2D(0.0, 30.0), Vertex2D(100.0, 30.0), Line2D::LINE);
            Line2D line1_3Result(Vertex2D(0.0, -30.0), Vertex2D(100.0, -30.0), Line2D::LINE);

            Line2D halfLine1_1 = halfLine1.Offset(Vertex2D(50.0, 50.0), 30.0);
            Line2D halfLine1_2 = halfLine1.Offset(true, 30.0);
            Line2D halfLine1_3 = halfLine1.Offset(false, 30.0);

            Line2D halfLine1_1Result(Vertex2D(0.0, 30.0), Vertex2D(100.0, 30.0), Line2D::HALF_LINE_BEGIN_2_END);
            Line2D halfLine1_2Result(Vertex2D(0.0, 30.0), Vertex2D(100.0, 30.0), Line2D::HALF_LINE_BEGIN_2_END);
            Line2D halfLine1_3Result(Vertex2D(0.0, -30.0), Vertex2D(100.0, -30.0), Line2D::HALF_LINE_BEGIN_2_END);

            Line2D seg1_1 = seg1.Offset(Vertex2D(50.0, 50.0), 30.0);
            Line2D seg1_2 = seg1.Offset(true, 30.0);
            Line2D seg1_3 = seg1.Offset(false, 30.0);

            Line2D seg1_1Result(Vertex2D(0.0, 30.0), Vertex2D(100.0, 30.0), Line2D::SEGMENT);
            Line2D seg1_2Result(Vertex2D(0.0, 30.0), Vertex2D(100.0, 30.0), Line2D::SEGMENT);
            Line2D seg1_3Result(Vertex2D(0.0, -30.0), Vertex2D(100.0, -30.0), Line2D::SEGMENT);

            if (!CheckOffset("TestLine2D line1_1", line1_1, line1_1Result))
                return false;
            if (!CheckOffset("TestLine2D line1_2", line1_2, line1_2Result))
                return false;
            if (!CheckOffset("TestLine2D line1_3", line1_3, line1_3Result))
                return false;

            if (!CheckOffset("TestLine2D halfLine1_1", halfLine1_1, halfLine1_1Result))
                return false;
            if (!CheckOffset("TestLine2D halfLine1-2", halfLine1_2, halfLine1_2Result))
                return false;
            if (!CheckOffset("TestLine2D halfLine1_3", halfLine1_3, halfLine1_3Result))
                return false;

            if (!CheckOffset("TestLine2D seg1_1", seg1_1, seg1_1Result))
                return false;
            if (!CheckOffset("TestLine2D seg1_2", seg1_2, seg1_2Result))
                return false;
            if (!CheckOffset("TestLine2D seg1_3", seg1_3, seg1_3Result))
                return false;

            return true;
        }

        bool TestLine3D()
        {
            Vertex3D v1(0.0, 0.0, 0.0);
            Vertex3D v2(100.0, 0.0, 0.0);

            Line3D line1(v1, v2, Line3D::LINE);
            Line3D halfLine1(v1, v2, Line3D::HALF_LINE_BEGIN_2_END);
            Line3D seg1(v1, v2, Line3D::SEGMENT);

            Line3D line1_1 = line1.Offset(Vertex3D(50.0, 50.0, 0.0), 30.0);
            Line3D line1_1Result(Vertex3D(0.0, 30.0, 0.0), Vertex3D(100.0, 30.0, 0.0), Line3D::LINE);
            
            Line3D halfLine1_1 = halfLine1.Offset(Vertex3D(50.0, 50.0, 0.0), 30.0);
            Line3D halfLine1_1Result(Vertex3D(0.0, 30.0, 0.0), Vertex3D(100.0, 30.0, 0.0), Line3D::HALF_LINE_BEGIN_2_END);
            
            Line3D seg1_1 = seg1.Offset(Vertex3D(50.0, 50.0, 0.0), 30.0);
            Line3D seg1_1Result(Vertex3D(0.0, 30.0, 0.0), Vertex3D(100.0, 30.0, 0.0), Line3D::SEGMENT);

            if (!CheckOffset("TestLine3D line1", line1_1, line1_1Result))
                return false;

            if (!CheckOffset("TestLine3D halfLine1", halfLine1_1, halfLine1_1Result))
                return false;

            if (!CheckOffset("TestLine3D seg1", seg1_1, seg1_1Result))
                return false;

            return true;
        }

        bool TestEArc2D()
        {
            Vertex2D vTL(20.0, 20.0);
            Vertex2D vBL(20.0, -20.0);
            Vertex2D vBR(80.0, -20.0);

            EArc2D earc1(vTL, vBL, vBR, 6.0, 3.5, false);
            EArc2D earc2(vTL, vBL, vBR, 6.0, 3.5, true);

            EArc2D earc1_1 = earc1.Offset(true, 30.0);
            EArc2D earc1_2 = earc1.Offset(false, 10.0);

            EArc2D earc2_1 = earc2.Offset(true, 30.0);
            EArc2D earc2_2 = earc2.Offset(false, 10.0);

            Vertex2D vCenter = (vTL + vBR) / 2;
            Vertex2D vL = (vTL + vBL) / 2;
            Vertex2D vB = (vBL + vBR) / 2;
                        
            Vertex2D _vL = UnE::Geometry::Math::GetLinearVertex(vL, vCenter, -30);
            Vertex2D _vB = UnE::Geometry::Math::GetLinearVertex(vB, vCenter, -30);

            Vertex2D _vBL = _vL + _vB - vCenter;
            Vertex2D _vBR = _vB * 2 - _vBL;
            Vertex2D _vTL = _vL * 2 - _vBL;

            Vertex2D __vL = UnE::Geometry::Math::GetLinearVertex(vL, vCenter, 10);
            Vertex2D __vB = UnE::Geometry::Math::GetLinearVertex(vB, vCenter, 10);

            Vertex2D __vBL = __vL + __vB - vCenter;
            Vertex2D __vBR = __vB * 2 - __vBL;
            Vertex2D __vTL = __vL * 2 - __vBL;

            EArc2D earc1_1Result(_vTL, _vBL, _vBR, 6.0, 3.5, false);
            EArc2D earc1_2Result(__vTL, __vBL, __vBR, 6.0, 3.5, false);

            EArc2D earc2_1Result(_vTL, _vBL, _vBR, 6.0, 3.5, true);
            EArc2D earc2_2Result(__vTL, __vBL, __vBR, 6.0, 3.5, true);

            if (!CheckOffset("TestEArc2D earc1_1", earc1_1, earc1_1Result))
                return false;
            if (!CheckOffset("TestEArc2D earc1_2", earc1_2, earc1_2Result))
                return false;
            if (!CheckOffset("TestEArc2D earc2_1", earc2_1, earc2_1Result))
                return false;
            if (!CheckOffset("TestEArc2D earc2_2", earc2_2, earc2_2Result))
                return false;

            return true;
        }

        bool TestEArc3D()
        {
            Vertex3D vTL(20.0, 20.0, 0.0);
            Vertex3D vBL(20.0, -20.0, 0.0);
            Vertex3D vBR(80.0, -20.0, 0.0);

            EArc3D earc1(vTL, vBL, vBR, 6.0, 3.5, false);
            EArc3D earc2(vTL, vBL, vBR, 6.0, 3.5, true);

            EArc3D earc1_1 = earc1.Offset(true, 30.0);
            EArc3D earc1_2 = earc1.Offset(false, 10.0);

            EArc3D earc2_1 = earc2.Offset(true, 30.0);
            EArc3D earc2_2 = earc2.Offset(false, 10.0);

            Vertex3D vCenter = (vTL + vBR) / 2;
            Vertex3D vL = (vTL + vBL) / 2;
            Vertex3D vB = (vBL + vBR) / 2;

            Vertex3D _vL = UnE::Geometry::Math::GetLinearVertex(vL, vCenter, -30);
            Vertex3D _vB = UnE::Geometry::Math::GetLinearVertex(vB, vCenter, -30);

            Vertex3D _vBL = _vL + _vB - vCenter;
            Vertex3D _vBR = _vB * 2 - _vBL;
            Vertex3D _vTL = _vL * 2 - _vBL;

            Vertex3D __vL = UnE::Geometry::Math::GetLinearVertex(vL, vCenter, 10);
            Vertex3D __vB = UnE::Geometry::Math::GetLinearVertex(vB, vCenter, 10);

            Vertex3D __vBL = __vL + __vB - vCenter;
            Vertex3D __vBR = __vB * 2 - __vBL;
            Vertex3D __vTL = __vL * 2 - __vBL;

            EArc3D earc1_1Result(_vTL, _vBL, _vBR, 6.0, 3.5, false);
            EArc3D earc1_2Result(__vTL, __vBL, __vBR, 6.0, 3.5, false);

            EArc3D earc2_1Result(_vTL, _vBL, _vBR, 6.0, 3.5, true);
            EArc3D earc2_2Result(__vTL, __vBL, __vBR, 6.0, 3.5, true);

            if (!CheckOffset("TestEArc3D earc1_1", earc1_1, earc1_1Result))
                return false;
            if (!CheckOffset("TestEArc3D earc1_2", earc1_2, earc1_2Result))
                return false;
            if (!CheckOffset("TestEArc3D earc2_1", earc2_1, earc2_1Result))
                return false;
            if (!CheckOffset("TestEArc3D earc2_2", earc2_2, earc2_2Result))
                return false;

            return true;
        }

        bool TestArc2D()
        {
            Vertex2D v1(50.0, 25.0);
            Vertex2D v2(25.0, 0.0);
            Vertex2D v3(50.0, -25.0);

            Arc2D arc1(v1, v2, v3);

            Arc2D arc1_1 = arc1.Offset(true, 30);
            Arc2D arc1_2 = arc1.Offset(false, 10);

            Vertex2D _v1(50.0, 55.0);
            Vertex2D _v2(-5.0, 0.0);
            Vertex2D _v3(50.0, -55.0);

            Vertex2D __v1(50.0, 15.0);
            Vertex2D __v2(35.0, 0.0);
            Vertex2D __v3(50.0, -15.0);

            Arc2D arc1_1Result(_v1, _v2, _v3);
            Arc2D arc1_2Result(__v1, __v2, __v3);

            if (!CheckOffset("TestArc2D arc1_1", arc1_1, arc1_1Result))
                return false;
            if (!CheckOffset("TestArc2D arc1_2", arc1_2, arc1_2Result))
                return false;
 
            return true;
        }

        bool TestArc3D()
        {
            Vertex3D v1(50.0, 25.0, 0.0);
            Vertex3D v2(25.0, 0.0, 0.0);
            Vertex3D v3(50.0, -25.0, 0.0);

            Arc3D arc1(v1, v2, v3);

            Arc3D arc1_1 = arc1.Offset(true, 30);
            Arc3D arc1_2 = arc1.Offset(false, 10);

            Vertex3D _v1(50.0, 55.0, 0.0);
            Vertex3D _v2(-5.0, 0.0, 0.0);
            Vertex3D _v3(50.0, -55.0, 0.0);

            Vertex3D __v1(50.0, 15.0, 0.0);
            Vertex3D __v2(35.0, 0.0, 0.0);
            Vertex3D __v3(50.0, -15.0, 0.0);

            Arc3D arc1_1Result(_v1, _v2, _v3);
            Arc3D arc1_2Result(__v1, __v2, __v3);

            if (!CheckOffset("TestArc3D arc1_1", arc1_1, arc1_1Result))
                return false;
            if (!CheckOffset("TestArc3D arc1_2", arc1_2, arc1_2Result))
                return false;

            return true;
        }
	};
}
