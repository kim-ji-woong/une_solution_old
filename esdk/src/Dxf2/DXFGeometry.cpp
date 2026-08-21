#include "stdafx.h"

BEGIN_NS(Geo)

Geometry::Geometry(void)
{
}

Geometry::~Geometry(void)
{
}

// ptCenter와 원점(0,0,0)을 잇는 직선을 축으로 하여, pt1을 90도 만큼 회전시킨 곳의 좌표(pt2)를 구한다.
// dAngle : Radian
bool Geometry::GetRotatedPointFromAxis(double ptCenter[3], double pt1[3], double pt2[3])
{
	if (ptCenter[0] == 0.0 && ptCenter[1] == 0.0 && ptCenter[2] == 0.0) return false;
	if (pt1[0] == 0.0 && pt1[1] == 0.0 && pt1[2] == 0.0) return false;
	if (ptCenter == pt1) return false;

	// ptCenter, pt1, pt2를 각각 원점과 ptCenter의 차이만큼 이동시킨다.
	// ptCentr => 원점이 되며, ptCenter, pt1, pt2가 위치한 평면이 원점 방향으로 이동하는 것이다.
	// pt1이 이동한 좌표 : pt11
	// pt2가 이동한 좌표 : pt21
	double pt11[3], pt21[3];
	int i;

	for (i=0;i<3;i++)
	{
		pt11[i] = pt1[i] - ptCenter[i];
	}

	// dThetaZ : 원점과 pt11을 잇는 직선이 평면(Z = 0)과 이루는 각(Radian)
	// dThetaY : 원점과 pt11을 잇는 직선이 평면(Y = 0)과 이루는 각(Radian)
	double dThetaZ, dThetaY;

	if (pt11[2] == 0.0)
	{
		if (pt11[0] > 0.0) dThetaZ = 0.0;
		else if (pt11[0] < 0.0) dThetaZ = 3.14159265358979323846;
		else
		{
			pt21[0] = 0.0;
			pt21[1] = -pt11[1];
			pt21[2] = 0.0;

			goto SET_RESULT;
		}
	}
	else if (pt11[2] > 0.0)
	{
		dThetaZ = acos(pt11[0] / sqrt(pt11[0] * pt11[0] + pt11[2] * pt11[2]));
	}
	else
	{
		dThetaZ = 6.28318530717958647692 - acos(pt11[0] / sqrt(pt11[0] * pt11[0] + pt11[2] * pt11[2]));
	}

	if (pt11[1] == 0.0)
	{
		if (pt11[0] > 0.0) dThetaY = 0.0;
		else if (pt11[0] < 0.0) dThetaY = 3.14159265358979323846;
		else
		{
			pt21[0] = 0.0;
			pt21[1] = 0.0;
			pt21[2] = -pt11[2];

			goto SET_RESULT;
		}
	}
	else if (pt11[1] > 0.0)
	{
		dThetaY = acos(pt11[0] / sqrt(pt11[0] * pt11[0] + pt11[1] * pt11[1]));
	}
	else
	{
		dThetaY = 6.28318530717958647692 - acos(pt11[0] / sqrt(pt11[0] * pt11[0] + pt11[1] * pt11[1]));
	}

	double dLen1 = sqrt(pt11[0] * pt11[0] + pt11[1] * pt11[1] + pt11[2] * pt11[2]);
	pt21[0] = 0.0;
	pt21[1] = dLen1;
	pt21[2] = 0.0;

	pt21[1] = cos(dThetaY) * dLen1;
	double dLen2 = sin(dThetaY) * dLen1;
	pt21[0] = cos(dThetaZ + 3.14159265358979323846) * dLen2;
	pt21[2] = sin(dThetaZ + 3.14159265358979323846) * dLen2;

SET_RESULT:
	for (i=0;i<3;i++) pt2[i] = pt21[i] + ptCenter[i];
	return true;
}

END_NS
