using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ClipperLib;
using UnE.Geometry;

namespace ClipperTest
{
    using Polygon = List<IntPoint>;
    using Polygons = List<List<IntPoint>>;
    using ExPolygons = List<ExPolygon>;

    ///////////////////////////////////////////////////
    // ADD BY SKKIM 2012-09-19 : support double polygon    

    using VertexPolygon = List<UnE.Geometry.Vertex2D>;
    using VertexPolygons = List<List<UnE.Geometry.Vertex2D>>;
    using ExVertexPolygons = List<ExVertexPolygon>;

    using DoublePolygon = List<DoublePoint>;
    using DoublePolygons = List<List<DoublePoint>>;
    using ExDoublePolygons = List<ExDoublePolygon>;
    /// <summary>
    /// UnitTest1의 요약 설명
    /// </summary>
    [TestClass]
    public class ClipperTest
    {
        public ClipperTest()
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
        public void ClipperVertexPolygonDiffTest()
        {            
            VertexPolygon vpg1 = new VertexPolygon();
            vpg1.Add(new Vertex2D(0.0, 0.0));
            vpg1.Add(new Vertex2D(10.0, 0.0));
            vpg1.Add(new Vertex2D(10.0, 10.0));
            vpg1.Add(new Vertex2D(0.0, 10.0));
            vpg1.Add(new Vertex2D(0.0, 0.0));
           
            VertexPolygon vpg2 = new VertexPolygon();
            vpg2.Add(new Vertex2D(0.0, 0.0));
            vpg2.Add(new Vertex2D(5.0, 0.0));
            vpg2.Add(new Vertex2D(5.0, 10.0));
            vpg2.Add(new Vertex2D(0.0, 10.0));
            vpg2.Add(new Vertex2D(0.0, 0.0));

            ExVertexPolygons result = new ExVertexPolygons();
            Clipper clipper = new Clipper();

            clipper.AddPolygon(vpg1, PolyType.ptSubject);
            clipper.AddPolygon(vpg2, PolyType.ptClip);


            ExVertexPolygon okResult = new ExVertexPolygon();
            okResult.holes = null;
            okResult.outer = new VertexPolygon();
            okResult.outer.Add(new Vertex2D(5.0, 10.0));
            okResult.outer.Add(new Vertex2D(5.0, 0.0));
            okResult.outer.Add(new Vertex2D(10.0, 0.0));
            okResult.outer.Add(new Vertex2D(10.0, 10.0));


            clipper.Execute(ClipType.ctDifference, result);

            int nCount = result.Count;
            if (nCount != 1)
            {
                Assert.Inconclusive("");
            }
            for (int i = 0; i < nCount; i++)
            {
                ExVertexPolygon pg = result[i];
                VertexPolygon vg = pg.outer;

                for (int j = 0; j < vg.Count; j++)
                {
                    if (vg[j] != okResult.outer[j])
                    {
                        Assert.Inconclusive("");
                    }
                }                
            }

        }
    }
}
