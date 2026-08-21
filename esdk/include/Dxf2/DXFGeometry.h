#pragma once

namespace Geo
{
	class Geometry
	{
	public:
		Geometry(void);
		virtual ~Geometry(void);

	public:
		// ptCenter와 원점(0,0,0)을 잇는 직선을 축으로 하여, pt1을 90도 만큼 회전시킨 곳의 좌표(pt2)를 구한다.
		// dAngle : Radian
		static bool GetRotatedPointFromAxis(double ptCenter[3], double pt1[3], double pt2[3]);
	};
}
