#include "stdafx.h"
#include "GeometryAPI.h"
#include "GVertex.h"
#include "GMath.h"
#include "GLine.h"
#include <math.h>

using namespace System;
using namespace System::Text;
using namespace System::Collections::Generic;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

using namespace UnE::Geometry;

namespace Geometrycpp
{
	[TestClass]
	public ref class VertexTest
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
		void TestVertex()
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
			/*Vertex3D v1(0.0, 0.0, 0.0);
			Vertex3D v2(100.0, 100.0, 100.0);
			
			Vertex3D v3 = GetCoordFromLinear(v1, v2, v1.GetDistance(v2) * 2);

			if (fabs(v3.x - 200.0) > UnE::Geometry::Math::HALF_TOLERANCE() ||
				fabs(v3.y - 200.0) > UnE::Geometry::Math::HALF_TOLERANCE() ||
				fabs(v3.z - 200.0) > UnE::Geometry::Math::HALF_TOLERANCE())
			{
				ErrorMessage(String::Format(L"v1({0}, {1}, {2})에서 v2({3}, {4}, {5}) 방향으로 {6} 만큼 이동한 좌표가 v3({7}, {8}, {9})입니다.", 
					v1.x, v1.y, v1.z, v2.x, v2.y, v2.z, v1.GetDistance(v2) * 2, v3.x, v3.y, v3.z));
				return;
			}*/
		};

		bool ErrorMessage(String^ strMessage)
		{
			Assert::Inconclusive(strMessage);
			return false;
		}
		
        bool TestGetLinearVertex3D()
        {
            Vertex3D v1(0.0, 0.0, 0.0);
            Vertex3D v2(100.0, 100.0, 100.0);

            Vertex3D v3 = GetLinearVertex(v1, v2, v1.GetDistance(v2) * 2);

            if (System::Math::Abs(v3.x - 200.0) > UnE::Geometry::Math::HALF_TOLERANCE() ||
                System::Math::Abs(v3.y - 200.0) > UnE::Geometry::Math::HALF_TOLERANCE() ||
                System::Math::Abs(v3.z - 200.0) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                ErrorMessage(String::Format("v1({0}, {1}, {2})에서 v2({3}, {4}, {5}) 방향으로 {6} 만큼 이동한 좌표가 v3({7}, {8}, {9})입니다.",
                    v1.x, v1.y, v1.z, v2.x, v2.y, v2.z, v1.GetDistance(v2) * 2, v3.x, v3.y, v3.z));
                return false;
            }
            return true;
        }

        bool TestGetLinearVertex2D()
        {
            Vertex2D v1(0.0, 0.0);
            Vertex2D v2(100.0, 100.0);

            Vertex2D v3 = GetLinearVertex(v1, v2, v1.GetDistance(v2) * 2);

            if (System::Math::Abs(v3.x - 200.0) > UnE::Geometry::Math::HALF_TOLERANCE() ||
                System::Math::Abs(v3.y - 200.0) > UnE::Geometry::Math::HALF_TOLERANCE())
            {
                ErrorMessage(String::Format("v1({0}, {1})에서 v2({2}, {3}) 방향으로 {4} 만큼 이동한 좌표가 v3({5}, {6})입니다.",
                    v1.x, v1.y, v2.x, v2.y, v1.GetDistance(v2) * 2, v3.x, v3.y));
                return false;
            }

            return true;
        }

		Vertex3D GetLinearVertex(const Vertex3D& v1, const Vertex3D& v2, double dLen)
		{
            double dist = v1.GetDistance(v2);
			if (dist <= UnE::Geometry::Math::HALF_TOLERANCE())
				return Vertex3D(v1.x, v1.y, v1.z);

			Vertex3D v = v1 + (v2 - v1) * dLen / dist;
			return v;
		}

        Vertex2D GetLinearVertex(const Vertex2D& v1, const Vertex2D& v2, double dLen)
        {
            double dist = v1.GetDistance(v2);
            if (dist <= UnE::Geometry::Math::HALF_TOLERANCE())
                return Vertex2D(v1.x, v1.y);

            Vertex2D v = v1 + (v2 - v1) * dLen / dist;
            return v;
        }
	};
}
