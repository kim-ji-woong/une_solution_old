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


#include "C4Types.h"


using namespace C4;


void C4::Reverse(Vector2D *v)
{
	Reverse(&v->x);
	Reverse(&v->y);
}

void C4::Reverse(Vector3D *v)
{
	Reverse(&v->x);
	Reverse(&v->y);
	Reverse(&v->z);
}

void C4::Reverse(Vector4D *v)
{
	Reverse(&v->x);
	Reverse(&v->y);
	Reverse(&v->z);
	Reverse(&v->w);
}

void C4::Reverse(Antivector4D *v)
{
	Reverse(&v->x);
	Reverse(&v->y);
	Reverse(&v->z);
	Reverse(&v->w);
}

void C4::Reverse(Matrix3D *m)
{
	float *f = &(*m)(0,0);
	for (machine a = 0; a < 9; a++) Reverse(&f[a]);
}

void C4::Reverse(Matrix4D *m)
{
	float *f = &(*m)(0,0);
	for (machine a = 0; a < 16; a++) Reverse(&f[a]);
}

void C4::Reverse(Quaternion *q)
{
	Reverse(&q->w);
	Reverse(&q->x);
	Reverse(&q->y);
	Reverse(&q->z);
}

void C4::Reverse(ColorRGB *c)
{
	Reverse(&c->red);
	Reverse(&c->green);
	Reverse(&c->blue);
}

void C4::Reverse(ColorRGBA *c)
{
	Reverse(&c->red);
	Reverse(&c->green);
	Reverse(&c->blue);
	Reverse(&c->alpha);
}

void C4::Reverse(Fixed2D *v)
{
	Reverse(&v->x);
	Reverse(&v->y);
}

void C4::Reverse(Fixed3D *v)
{
	Reverse(&v->x);
	Reverse(&v->y);
	Reverse(&v->z);
}

void C4::Reverse(Integer2D *v)
{
	Reverse(&v->x);
	Reverse(&v->y);
}

void C4::Reverse(Integer3D *v)
{
	Reverse(&v->x);
	Reverse(&v->y);
	Reverse(&v->z);
}

void C4::Reverse(Rect *r)
{
	Reverse(&r->left);
	Reverse(&r->top);
	Reverse(&r->right);
	Reverse(&r->bottom); 
}

// ZYURVUR
