//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#include "C4Vector2D.h"


using namespace C4;


ConstPoint2D Zero2DType::zero = {0.0F, 0.0F};

const Zero2DType C4::Zero2D = {};


Vector2D& Vector2D::Rotate(float angle)
{
	Vector2D t = CosSin(angle);
	float nx = t.x * x - t.y * y;
	float ny = t.y * x + t.x * y;
	
	x = nx;
	y = ny;
	return (*this);
}

// ZYURVUR
