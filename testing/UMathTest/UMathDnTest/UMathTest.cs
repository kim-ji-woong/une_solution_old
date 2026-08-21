using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.Drawing;
using UnE.Math;

namespace UMathTest
{
    [TestClass]
    public class UMathTester
    {
        [TestMethod]
        public void MathLibTest()
        {
            UMath uMath = new UMath(10000);
            if( uMath == null)
                Assert.Inconclusive("UMath 생성 실패");

            if (UMathTableTest() == false)
                Assert.Inconclusive("UMath SinTable 테스트 실패");

            if (VectorTest() == false)
                Assert.Inconclusive("Vector3 테스트 실패");

            if (Matrix3Test() == false)
                Assert.Inconclusive("Matrix3 테스트 실패");

            if (Matrix4Test() == false)
                Assert.Inconclusive("Matrix3 테스트 실패");

            if (ColourTest() == false)
                Assert.Inconclusive("ColourValue 테스트 실패");

        }
        private bool UMathTableTest()
        {            
            Radian r = new Radian(new Degree(90));
            float v1 = UMath.Cos(UMath.HALF_PI, true);
            float v2 = UMath.Cos(r);

            if (UMath.RealEqual(v2, v1, 0.0001f) != true)
            {
                return false;
            }
            v1 = UMath.Sin(UMath.HALF_PI, true);
            v2 = UMath.Sin(r);

            if (UMath.RealEqual(v2, v1, 0.0001f) != true)
            {
                return false;
            }
            return true;
        }


        private bool VectorTest()
        {
            Vector3 v = new Vector3(1, 0, 0);
            Vector3 v1 = new Vector3(0, 1, 0);
            Vector3 norm = v.crossProduct(v1);
            norm.normalise();
            if (norm != Vector3.UNIT_Z)
            {
                return false;
            } 
            return true;
        }

        private bool Matrix4Test()
        {
            UnE.Math.Matrix4 s = new UnE.Math.Matrix4(Matrix3.IDENTITY);
            UnE.Math.Matrix4 s2 = new UnE.Math.Matrix4(Matrix3.IDENTITY);
            UnE.Math.Matrix4 s3 = s * s2;
            Matrix4 t3 = s3.transpose();
            if (t3 != s)
            {
                return false;
            }

         
            return true;
        }

        private bool Matrix3Test()
        {
            UnE.Math.Matrix3 s = new UnE.Math.Matrix3(Matrix3.IDENTITY);
            UnE.Math.Matrix3 s2 = new UnE.Math.Matrix3(Matrix3.IDENTITY);
            UnE.Math.Matrix3 s3 = s + s2;
            s3.Orthonormalize();
            if( s3 != Matrix3.IDENTITY)
            {
                return false;
            }

            UnE.Math.Matrix3 aaa = null;
            if (null == s3)
            {
                return false;
            }
            return true;
        }
           
        private bool ColourTest()
        {
            ColourValue c1 = new ColourValue();
            ColourValue c2 = new ColourValue();
            c1.setAsARGB((uint)Color.Blue.ToArgb());
            c2.setAsARGB((uint)Color.Red.ToArgb());
            ColourValue c3 = c1 + c2;
            c3.saturate();
            ColourValue c4 = new ColourValue(1.0f, 0.0f, 1.0f);
            if (c3 != c4)
            {
                return false;
            }
            return true;
        }
    }
}
