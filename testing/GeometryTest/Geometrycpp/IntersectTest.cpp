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
	public ref class IntersectTest
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
		void TestIntersect()
		{
			//
			// TODO: 테스트 논리를 여기에 추가합니다.
			//
			if (!TestLineToLine())
                return;
            if (!TestLineToEArc())
                return;
            if (!TestEArcToEArc())
                return;
			if (!TestEArcToArc())
                return;
            if (!TestArcToArc())
                return;
		};

		bool ErrorMessage(String^ strMessage)
		{
			Assert::Inconclusive(strMessage);
			return false;
		}

		bool ErrorIntersectMessage0(String^ strTag, int nVertexCount, int nResultCount)
        {
            String^ strMsg = String::Format(L"{0} 결과 : VertexCount({1})이어야 하나, ResultCount({2})이다.",
                strTag, nResultCount, nVertexCount);
            return ErrorMessage(strMsg);
        }

        bool ErrorIntersectMessage1(String^ strTag, Vertex2D v1, Line2D::LineType type, Vertex2D vResult1, Line2D::LineType typeResult)
        {
            String^ strMsg = String::Format(L"{0} 결과 : v1({1}, {2}), LineType({3})이어야 하나, vResult1({4}, {5})), LineType({6})이다.",
                 strTag, vResult1.x, vResult1.y, (int)typeResult, v1.x, v1.y, (int)type);
            return ErrorMessage(strMsg);
        }

        bool ErrorIntersectMessage2(String^ strTag, Vertex2D v1, Vertex2D v2, Line2D::LineType type, Vertex2D vResult1, Vertex2D vResult2, Line2D::LineType typeResult)
        {
            String^ strMsg = String::Format(L"{0} 결과 : v1({1}, {2}), v2({3}, {4}), LineType({5})이어야 하나, vResult1({6}, {7}), vResult2({8}, {9}), LineType({10})이다.",
                 strTag, vResult1.x, vResult1.y, vResult2.x, vResult2.y, (int)typeResult, v1.x, v1.y, v2.x, v2.y, (int)type);
            return ErrorMessage(strMsg);
        }

        bool CheckIntersect(String^ strTag, Vertex2D v1, Vertex2D v2, Line2D::LineType type, int nVertexCount, Vertex2D vResult1, Vertex2D vResult2, Line2D::LineType typeResult, int nResultCount)
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

            if (type == Line2D::LINE)
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
            Vertex2D v1(0.0, 0.0);
            Vertex2D v2(100.0, 0.0);

            Line2D line1(v1, v2, Line2D::LINE);
            Line2D halfLine1(v1, v2, Line2D::HALF_LINE_BEGIN_2_END);
            Line2D seg1(v1, v2, Line2D::SEGMENT);

            Vertex2D v3(50.0, 0.0);
            Vertex2D v4(150.0, 0.0);

            Line2D line2(v3, v4, Line2D::LINE);
            Line2D halfLine2(v3, v4, Line2D::HALF_LINE_BEGIN_2_END);
            Line2D seg2(v3, v4, Line2D::SEGMENT);

            Vertex2D v5(50.0, 50.0);
            Vertex2D v6(50.0, -50.0);

            Line2D line3(v5, v6, Line2D::LINE);
            Line2D halfLine3(v5, v6, Line2D::HALF_LINE_BEGIN_2_END);
            Line2D seg3(v5, v6, Line2D::SEGMENT);

            Vertex2D v7(50.0, 150.0);
            Vertex2D v8(50.0, 100.0);
            Vertex2D v9(50.0, 50.0);

            Line2D line4(v7, v8, Line2D::LINE);
            Line2D halfLine4(v7, v8, Line2D::HALF_LINE_BEGIN_2_END);
            Line2D seg4(v7, v8, Line2D::SEGMENT);

            Line2D seg5(v7, v9, Line2D::SEGMENT);

            Vertex2D v12_1(50.0, 0.0);
            Vertex2D v12_2(100.0, 0.0);
            Line2D::LineType type12 = Line2D::LINE;
            int nResult12 = 2;

            Vertex2D v13_1(50.0, 0.0);
            Line2D::LineType type13 = Line2D::NO_LINE;
            int nResult13 = 1;

            Line2D::LineType type34 = Line2D::NO_LINE;
            int nResult34 = 0;

            Vertex2D v35_1(50.0, 50.0);
            Line2D::LineType type35 = Line2D::NO_LINE;
            int nResult35 = 1;

            Vertex2D vNull;

            Vertex2D vertex1, vertex2;
            Line2D::LineType lineType;

            int nResult = line1.IntersectLine(line2, vertex1, vertex2, lineType);
            if (!CheckIntersect("line1 & line2 Intersect", vertex1, vertex2, lineType, nResult, v12_1, v12_2, type12, nResult12))
                return false;

            nResult = line1.IntersectLine(line3, vertex1, vertex2, lineType);
            if (!CheckIntersect("line1 & line3 Intersect", vertex1, vertex2, lineType, nResult, v13_1, vNull, type13, nResult13))
                return false;

            nResult = seg3.IntersectLine(seg4, vertex1, vertex2, lineType);
            if (!CheckIntersect("seg3 & seg4 Intersect", vertex1, vertex2, lineType, nResult, vNull, vNull, type34, nResult34))
                return false;

            nResult = halfLine3.IntersectLine(seg5, vertex1, vertex2, lineType);
            if (!CheckIntersect("halfLine3 & seg5 Intersect", vertex1, vertex2, lineType, nResult, v35_1, vNull, type35, nResult35))
                return false;

            return true;
        }

        bool ErrorIntersectMessage1(String^ strTag, Vertex2D v1, Vertex2D vResult1)
        {
            String^ strMsg = String::Format("{0} 결과 : v1({1}, {2})이어야 하나, vResult1({3}, {4})이다.",
                 strTag, vResult1.x, vResult1.y, v1.x, v1.y);
            return ErrorMessage(strMsg);
        }

        bool ErrorIntersectMessage2(String^ strTag, Vertex2D v1, Vertex2D v2, Vertex2D vResult1, Vertex2D vResult2)
        {
            String^ strMsg = String::Format("{0} 결과 : v1({1}, {2}), v2({3}, {4})이어야 하나, vResult1({5}, {6}), vResult2({7}, {8})이다.",
                 strTag, vResult1.x, vResult1.y, vResult2.x, vResult2.y, v1.x, v1.y, v2.x, v2.y);
            return ErrorMessage(strMsg);
        }

        bool CheckIntersect(String^ strTag, Vertex2D v1, Vertex2D v2, int nVertexCount, Vertex2D vResult1, Vertex2D vResult2, int nResultCount)
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
            Vertex2D v1(0.0, 0.0);
            Vertex2D v2(100.0, 0.0);

            Line2D line1(v1, v2, Line2D::LINE);
            Line2D halfLine1(v1, v2, Line2D::HALF_LINE_BEGIN_2_END);
            Line2D seg1(v1, v2, Line2D::SEGMENT);

            Vertex2D vTL(20.0, 20.0);
            Vertex2D vBL(20.0, -20.0);
            Vertex2D vBR(80.0, -20.0);

            EArc2D earc1(vTL, vBL, vBR, 6.0, 3.5, false);
            EArc2D earc2(vTL, vBL, vBR, 6.0, 3.5, true);

            Vertex2D vNull;

            Vertex2D v11_1(80.0, 0.0);
            Vertex2D v11_2(20.0, 0.0);
            int nResult11 = 2;

            Vertex2D v12_1(20.0, 0.0);
            int nResult12 = 1;

            Vertex2D vertex1, vertex2;
            int nResult = earc1.IntersectLine(line1, vertex1, vertex2);

            if (!CheckIntersect("earc1 & line1 Intersect", vertex1, vertex2, nResult, v11_1, v11_2, nResult11))
                return false;

            nResult = earc2.IntersectLine(line1, vertex1, vertex2);

            if (!CheckIntersect("earc2 & line1 Intersect", vertex1, vertex2, nResult, v12_1, vNull, nResult12))
                return false;

            return true;
        }

		bool ContainVertex(Vertex2D& rVertex, std::vector<Vertex2D>& rVecVertex)
        {
			int nVertexCount = (int)rVecVertex.size();

            for (int i=0;i<nVertexCount;i++)
            {
                if (rVertex == rVecVertex[i])
                    return true;
            }

            return false;
        }

        bool CheckIntersect(String^ strTag, std::vector<Vertex2D>* pVecVertex, std::vector<EArc2D*>* pVecEArc, int nIntersectCount, std::vector<Vertex2D>* pVecVertexResult, std::vector<Vertex2D>* pVecEArcResult, int nResultCount)
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
                if (pVecVertex == 0 || pVecVertexResult == 0)
                {
                    String^ strError = strTag + "pVecVertex 또는 pVecVertexResult가 null입니다.";
                    return ErrorMessage(strError);
                }

                int nCount1 = pVecVertex->size();
                int nCount2 = pVecVertexResult->size();

                if (nVertexCount != nCount1 || nCount1 != nCount2)
                {
                    String^ strError = strTag + String::Format("nVertexCount = {0}, pVecVertex의 개수 = {1}, pVecVertexResult의 개수 = {2}로 서로 일치하지 않습니다.",
                        nVertexCount, nCount1, nCount2);
                    return ErrorMessage(strError);
                }

                for (int i = 0; i < nVertexCount; i++)
                {
                    Vertex2D v1 = (Vertex2D)pVecVertex->at(i);

					if (!ContainVertex(v1, *pVecVertexResult))
                    {
                        Vertex2D v2 = pVecVertexResult->at(i);
                        return ErrorIntersectMessage1(strTag, v1, v2);
                    }
                    //Vertex2D v2 = (Vertex2D)pVecVertexResult->at(i);

                    //if (v1 != v2)
                    //    return ErrorIntersectMessage1(strTag, v1, v2);
                }
            }

            if (nEArcCount > 0)
            {
                if (pVecEArc == 0 || pVecEArcResult == 0)
                {
                    String^ strError = strTag + "pVecEArc 또는 pVecEArcResult가 null입니다.";
                    return ErrorMessage(strError);
                }

                int nCount1 = pVecEArc->size();
                int nCount2 = pVecEArcResult->size() / 2;

                if (nEArcCount != nCount1 || nCount1 != nCount2)
                {
                    String^ strError = strTag + String::Format("nEArcCount = {0}, arrListEArc의 개수 = {1}, arrListEArcResult의 개수 = {2}로 서로 일치하지 않습니다.",
                        nVertexCount, nCount1, nCount2);
                    return ErrorMessage(strError);
                }

                for (int i = 0; i < nEArcCount; i++)
                {
                    EArc2D* arc1 = pVecEArc->at(i);

                    //Vertex2D v1 = (Vertex2D)pVecEArcResult->at(i * 2 + 0);
                    //Vertex2D v2 = (Vertex2D)pVecEArcResult->at(i * 2 + 1);

                    Vertex2D vBegin = arc1->GetBeginVertex();
                    Vertex2D vEnd = arc1->GetEndVertex();

					if (!ContainVertex(vBegin, *pVecEArcResult) || !ContainVertex(vEnd, *pVecEArcResult))
                    {
                        Vertex2D v1 = pVecEArcResult->at(i * 2 + 0);
						Vertex2D v2 = pVecEArcResult->at(i * 2 + 1);
                        return ErrorIntersectMessage2(strTag, vBegin, vEnd, v1, v2);
                    }

                    //if (v1 != vBegin || v2 != vEnd)
                    //    return ErrorIntersectMessage2(strTag, vBegin, vEnd, v1, v2);
                }
            }

            return true;
        }

        bool TestEArcToEArc()
        {
            Vertex2D vTL1(20.0, 20.0);
            Vertex2D vBL1(20.0, -20.0);
            Vertex2D vBR1(80.0, -20.0);

            Vertex2D vTL2(30.0, 30.0);
            Vertex2D vBL2(30.0, -30.0);
            Vertex2D vBR2(70.0, -30.0);

            EArc2D earc1(vTL1, vBL1, vBR1, 6.0, 3.5, false);
            EArc2D earc2(vTL2, vBL2, vBR2, 5.0, 3.5, true);

            std::vector<Vertex2D> arrVertex;
			std::vector<EArc2D*> arrEArc;

			int nResult = earc1.IntersectEArc(earc2, arrVertex, arrEArc);
			
            Vertex2D vNull;

            std::vector<Vertex2D> arrVertex11;
            Vertex2D v11_1(33.358994113243149, 16.641005886756883);
            arrVertex11.push_back(v11_1);

            int nResult11 = 1;

            if (!CheckIntersect("earc1 & earc2 Intersect", &arrVertex, &arrEArc, nResult, &arrVertex11, 0, nResult11))
                return false;
			
            std::vector<Vertex2D> arrVertex3;
			std::vector<EArc2D*> arrEArc3;
            EArc2D earc3(vTL1, vBL1, vBR1, 6.2, 4.0, true);
            nResult = earc1.IntersectEArc(earc3, arrVertex3, arrEArc3);
			
            std::vector<Vertex2D> arrEArc13;

            Vertex2D v13_1(77.494713190712559, -8.0011317688452870);
            Vertex2D v13_2(79.768092332187791, -2.4819955145757739);
            Vertex2D v13_3(36.902718602104358, 17.993347165418133);
            Vertex2D v13_4(20.189874241688958, -2.2466174350372952);
            arrEArc13.push_back(v13_1);
            arrEArc13.push_back(v13_2);
            arrEArc13.push_back(v13_3);
            arrEArc13.push_back(v13_4);

            int nResult13 = 200;

            if (!CheckIntersect("earc1 & earc3 Intersect", &arrVertex3, &arrEArc3, nResult, 0, &arrEArc13, nResult13))
                return false;

			int nEArcCount = (int)arrEArc.size();

			for (int i=0;i<nEArcCount;i++)
			{			
				delete arrEArc[i];
			}

			nEArcCount = (int)arrEArc3.size();

			for (int i=0;i<nEArcCount;i++)
			{			
				delete arrEArc3[i];
			}

            return true;
        }

		bool TestEArcToArc()
        {
            Vertex2D vTL1(20.0, 20.0);
            Vertex2D vBL1(20.0, -20.0);
            Vertex2D vBR1(80.0, -20.0);

            Vertex2D v1(50.0, 25.0);
            Vertex2D v2(75.0, 0.0);
            Vertex2D v3(50.0, -25.0);

            EArc2D earc(vTL1, vBL1, vBR1, 6.0, 3.5, false);
            Arc2D arc(v1, v2, v3);
            
            std::vector<Vertex2D> arrVertex;
			std::vector<EArc2D*> arrArc;
            int nResult = earc.IntersectEArc(arc, arrVertex, arrArc);

            Vertex2D vNull;

            std::vector<Vertex2D> arrVertex11;
            Vertex2D v11_1(70.124611797498119, 14.832396974191322);
            arrVertex11.push_back(v11_1);

            int nResult11 = 1;

            if (!CheckIntersect("earc & arc Intersect", &arrVertex, &arrArc, nResult, &arrVertex11, 0, nResult11))
                return false;

			int nArcCount = (int)arrArc.size();

			for (int i=0;i<nArcCount;i++)
			{			
				delete arrArc[i];
			}

            return true;
        }

		bool TestArcToArc()
        {
            Vertex2D v1(50.0, 25.0);
            Vertex2D v2(25.0, 0.0);
            Vertex2D v3(50.0, -25.0);

            Vertex2D v4(50.0, 25.0);
            Vertex2D v5(75.0, 0.0);
            Vertex2D v6(50.0, -25.0);

            Vertex2D vCenter(50.0, 0.0);

            Arc2D arc1(v1, v2, v3);
            Arc2D arc2(v4, v5, v6);
            Arc2D arc3(vCenter, 25.0, 1.0, UnE::Geometry::Math::_2PI() - 2, false);

            std::vector<Vertex2D> arrVertex;
			std::vector<EArc2D*> arrArc;
            int nResult = arc1.IntersectEArc(arc2, arrVertex, arrArc);

            Vertex2D vNull;

            std::vector<Vertex2D> arrVertex12;
            Vertex2D v12_1(50.0, 25.0);
            Vertex2D v12_2(50.0, -25.0);
            arrVertex12.push_back(v12_1);
            arrVertex12.push_back(v12_2);

            int nResult12 = 2;

            if (!CheckIntersect("arc1 & arc2 Intersect", &arrVertex, &arrArc, nResult, &arrVertex12, 0, nResult12))
                return false;

            std::vector<Vertex2D> arrVertex2;
			std::vector<EArc2D*> arrArc2;

            int nResult2 = arc2.IntersectEArc(arc3, arrVertex2, arrArc2);

            std::vector<Vertex2D> arrArc23;
            Vertex2D v23_1(63.507557646703496, 21.036774620197413);
            Vertex2D v23_2(50.0, 25.0);
            Vertex2D v23_3(50.0, -25.0);
            Vertex2D v23_4(63.507557646703496, -21.036774620197413);
            arrArc23.push_back(v23_1);
            arrArc23.push_back(v23_2);
            arrArc23.push_back(v23_3);
            arrArc23.push_back(v23_4);

            int nResult23 = 200;

            if (!CheckIntersect("arc2 & arc3 Intersect", &arrVertex2, &arrArc2, nResult2, 0, &arrArc23, nResult23))
                return false;

			int nArcCount = (int)arrArc.size();

			for (int i=0;i<nArcCount;i++)
			{
				delete arrArc[i];
			}

			nArcCount = (int)arrArc2.size();

			for (int i=0;i<nArcCount;i++)
			{
				delete arrArc2[i];
			}

            return true;
        }
	};
}
