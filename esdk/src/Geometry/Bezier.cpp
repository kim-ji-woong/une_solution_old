#include "StdAfx.h"
#include "GBezier.h"
#include <math.h>

#ifdef DOTNET
using namespace System;
#endif

BEGIN_NS(UnE)
BEGIN_NS(Geometry)

BezierCurve2D::BezierCurve2D()
{
#ifdef DOTNET
	FactorialLookup = dnonlynew array<double>(33);
#endif
	CreateFactorialTable();
}

// just check if n is appropriate, then return the result
double BezierCurve2D::factorial(int n)
{
    /*if (n < 0) { throw new Exception("n is less than 0"); }
    if (n > 32) { throw new Exception("n is greater than 32"); }*/

    return FactorialLookup[n]; /* returns the value n! as a SUMORealing point number */
}

// create lookup table for fast factorial calculation
void BezierCurve2D::CreateFactorialTable()
{
    // fill untill n=32. The rest is too high to represent
    FactorialLookup[0] = 1.0;
    FactorialLookup[1] = 1.0;
    FactorialLookup[2] = 2.0;
    FactorialLookup[3] = 6.0;
    FactorialLookup[4] = 24.0;
    FactorialLookup[5] = 120.0;
    FactorialLookup[6] = 720.0;
    FactorialLookup[7] = 5040.0;
    FactorialLookup[8] = 40320.0;
    FactorialLookup[9] = 362880.0;
    FactorialLookup[10] = 3628800.0;
    FactorialLookup[11] = 39916800.0;
    FactorialLookup[12] = 479001600.0;
    FactorialLookup[13] = 6227020800.0;
    FactorialLookup[14] = 87178291200.0;
    FactorialLookup[15] = 1307674368000.0;
    FactorialLookup[16] = 20922789888000.0;
    FactorialLookup[17] = 355687428096000.0;
    FactorialLookup[18] = 6402373705728000.0;
    FactorialLookup[19] = 121645100408832000.0;
    FactorialLookup[20] = 2432902008176640000.0;
    FactorialLookup[21] = 51090942171709440000.0;
    FactorialLookup[22] = 1124000727777607680000.0;
    FactorialLookup[23] = 25852016738884976640000.0;
    FactorialLookup[24] = 620448401733239439360000.0;
    FactorialLookup[25] = 15511210043330985984000000.0;
    FactorialLookup[26] = 403291461126605635584000000.0;
    FactorialLookup[27] = 10888869450418352160768000000.0;
    FactorialLookup[28] = 304888344611713860501504000000.0;
    FactorialLookup[29] = 8841761993739701954543616000000.0;
    FactorialLookup[30] = 265252859812191058636308480000000.0;
    FactorialLookup[31] = 8222838654177922817725562880000000.0;
    FactorialLookup[32] = 263130836933693530167218012160000000.0;
}

double BezierCurve2D::Ni(int n, int i)
{
    double ni;
    double a1 = factorial(n);
    double a2 = factorial(i);
    double a3 = factorial(n - i);
    ni =  a1/ (a2 * a3);
    return ni;
}

// Calculate Bernstein basis
double BezierCurve2D::Bernstein(int n, int i, double t)
{
    double basis;
    double ti; /* t^i */
    double tni; /* (1 - t)^i */

    /* Prevent problems with pow */

    if (t == 0.0 && i == 0) 
        ti = 1.0; 
    else 
        ti = pow(t, i);

    if (n == i && t == 1.0) 
        tni = 1.0; 
    else 
        tni = pow((1 - t), (n - i));

    //Bernstein basis
    basis = Ni(n, i) * ti * tni; 
    return basis;
}

// nCurvePoint는 반드시 0보다 크고 33보다 작거나 같아야 한다.
#ifdef DOTNET
bool BezierCurve2D::Calc(array<Vertex2D^>^ arrCurvePoints, int nCurvePointCount, array<Vertex2D^>^ arrResultPoints, int nResultCount)
#else
bool BezierCurve2D::Calc(Vertex2D arrCurvePoints[], int nCurvePointCount, Vertex2D arrResultPoints[], int nResultCount)
#endif
{
	if (nCurvePointCount < 0 || nCurvePointCount > 33)
		return false;

    // Calculate points on curve
    int nIndex1 = 0, nIndex2;
    double t = 0;
    double step = (double)1.0 / (nResultCount - 1);

    for (int i1 = 0; i1 != nResultCount; i1++)
    { 
        if ((1.0 - t) < 5e-6) 
            t = 1.0;

        nIndex2 = 0;
        
		arrResultPoints[nIndex1] = dnonlynew Vertex2D(0.0, 0.0);
		/*OF(arrResultPoints[nIndex1], x) = 0.0;
        OF(arrResultPoints[nIndex1], y) = 0.0;*/

        for (int i = 0; i != nCurvePointCount; i++)
        {
            double basis = Bernstein(nCurvePointCount - 1, i, t);
            
			OF(arrResultPoints[nIndex1], x) += basis * OF(arrCurvePoints[nIndex2], x);
            OF(arrResultPoints[nIndex1], y) += basis * OF(arrCurvePoints[nIndex2], y);
            
			nIndex2++;
        }

        nIndex1++;
        t += step;
    }

	return true;
}

END_NS
END_NS
