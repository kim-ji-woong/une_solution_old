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


#include "C4Computation.h"


using namespace C4;


namespace C4
{
	const float kJacobiEpsilon	= 1.0e-10F;
	const float kTriangleEpsilon = 5.0e-4F;
	
	
	struct SubtractVertex
	{
		Point3D		position;
		int32		direction;
		int32		planeIndex;
		bool		clipFlag;
		
		SubtractVertex& Set(const Point3D& p, int32 dir = 0, int32 plane = -1, bool clip = false)
		{
			position = p;
			direction = dir;
			planeIndex = plane;
			clipFlag = clip;
			return (*this);
		}
	};
}


void C4::Reverse(Line *l)
{
	Reverse(&l->index[0]);
	Reverse(&l->index[1]);
}

void C4::Reverse(Triangle *t)
{
	Reverse(&t->index[0]);
	Reverse(&t->index[1]);
	Reverse(&t->index[2]);
}

void C4::Reverse(Quad *q)
{
	Reverse(&q->index[0]);
	Reverse(&q->index[1]);
	Reverse(&q->index[2]);
	Reverse(&q->index[3]);
}

void C4::Reverse(Edge *e)
{
	Reverse(&e->vertexIndex[0]);
	Reverse(&e->vertexIndex[1]);
	Reverse(&e->faceIndex[0]);
	Reverse(&e->faceIndex[1]);
}


int32 Polyhedron::GetTriangleCount(void) const
{
	int32 count = 0;
	for (machine a = 0; a < faceCount; a++) count += face[a].edgeCount;
	return (count - faceCount * 2);
}

void Polyhedron::GetTriangleArray(Triangle *triangle) const
{
	for (machine a = 0; a < faceCount; a++)
	{
		int32	i0, i1;
		
		const Face *polyhedronFace = &face[a];
		int32 faceEdgeCount = polyhedronFace->edgeCount;
		
		const Edge *faceEdge = &edge[polyhedronFace->edgeIndex[0]];
		if (faceEdge->faceIndex[0] == a)
		{
			i0 = faceEdge->vertexIndex[0];
			i1 = faceEdge->vertexIndex[1];
		}
		else
		{
			i0 = faceEdge->vertexIndex[1];
			i1 = faceEdge->vertexIndex[0];
		}
		
		for (machine b = 2; b < faceEdgeCount; b++)
		{
			faceEdge = &edge[polyhedronFace->edgeIndex[b]];
			int32 i2 = faceEdge->vertexIndex[faceEdge->faceIndex[0] != a];
			triangle->Set(i0, i1, i2);
			
			i1 = i2;
			triangle++;
		}
	}
}

 
void Math::CalculateBarycentricCoordinates(const Point3D& p1, const Point3D& p2, const Point3D& p3, const Point3D& c, float *w1, float *w2, float *w3)
{ 
	Vector3D q1 = p1 - p3; 
	Vector3D q2 = p2 - p3; 
	
	float q1q1 = q1 * q1; 
	float q2q2 = q2 * q2;
	float q1q2 = q1 * q2;
	
	float d = q1q1 * q2q2 - q1q2 * q1q2; 
	if (Fabs(d) > K::min_float)
	{
		d = 1.0F / d;
		 
		Vector3D m = c - p3;
		float mq1 = m * q1;
		float mq2 = m * q2;
		
		float v1 = (q2q2 * mq1 - q1q2 * mq2) * d;
		float v2 = (q1q1 * mq2 - q1q2 * mq1) * d;
		
		*w1 = v1;
		*w2 = v2;
		*w3 = 1.0F - v1 - v2;
	}
	else
	{
		if (q1q1 < 1.0e-7F)
		{
			float v1 = FmaxZero(Fmin(p1 * (c - p2) / (p1 * p1 - p1 * p2), 1.0F));
			float v2 = 1.0F - v1;
			v1 *= 0.5F;
			*w1 = v1;
			*w2 = v2;
			*w3 = v1;
		}
		else if (q2q2 < 1.0e-7F)
		{
			float v3 = FmaxZero(Fmin(p3 * (c - p1) / (p3 * p3 - p3 * p1), 1.0F));
			float v1 = 1.0F - v3;
			v3 *= 0.5F;
			*w1 = v1;
			*w2 = v3;
			*w3 = v3;
		}
		else
		{
			float v2 = FmaxZero(Fmin(p2 * (c - p3) / (p2 * p2 - p2 * p3), 1.0F));
			float v3 = 1.0F - v2;
			v2 *= 0.5F;
			*w1 = v2;
			*w2 = v2;
			*w3 = v3;
		}
	}
}

bool Math::CalculateTangent(const Point3D& p1, const Point3D& p2, const Point3D& p3, const Point2D& u1, const Point2D& u2, const Point2D& u3, Vector4D *tangent)
{
	float x1 = p2.x - p1.x;
	float x2 = p3.x - p1.x;
	float y1 = p2.y - p1.y;
	float y2 = p3.y - p1.y;
	float z1 = p2.z - p1.z;
	float z2 = p3.z - p1.z;
	
	float s1 = u2.x - u1.x;
	float s2 = u3.x - u1.x;
	float t1 = u2.y - u1.y;
	float t2 = u3.y - u1.y;
	
	float r = s1 * t2 - s2 * t1;
	if (Fabs(r) > K::min_float)
	{
		Vector3D sdir(t2 * x1 - t1 * x2, t2 * y1 - t1 * y2, t2 * z1 - t1 * z2);
		Vector3D tdir(s1 * x2 - s2 * x1, s1 * y2 - s2 * y1, s1 * z2 - s2 * z1);
		Vector3D nrml(y1 * z2 - y2 * z1, z1 * x2 - z2 * x1, x1 * y2 - x2 * y1);
		
		r = 1.0F / r;
		float w = (sdir % tdir * nrml > 0.0F) ? 1.0F : -1.0F;
		tangent->Set(sdir.x * r, sdir.y * r, sdir.z * r, w);
		return (true);
	}
	
	return (false);
}

bool Math::IntersectSegmentAndTriangle(const Point3D& p1, const Point3D& p2, const Point3D& v1, const Point3D& v2, const Point3D& v3, Point3D *position, Vector3D *normal, float *t)
{
	Bivector4D edge1 = v1 ^ v2;
	Antivector4D plane = edge1 ^ v3;
	plane.Standardize();
	
	float d1 = plane ^ p1;
	float d2 = plane ^ p2;
	
	if ((!(d1 < 0.0F)) && (d2 < 0.0F))
	{
		Bivector4D line(p1, p2);
		Bivector4D edge2 = v2 ^ v3;
		Bivector4D edge3 = v3 ^ v1;
		
		if ((!((line ^ edge1) > 0.0F)) && (!((line ^ edge2) > 0.0F)) && (!((line ^ edge3) > 0.0F)))
		{
			float s = d1 / (d1 - d2);
			*t = s;
			*position = p1 + line.GetTangent() * s;
			*normal = plane.GetAntivector3D();
			return (true);
		}
	}
	
	return (false);
}

bool Math::IntersectRayAndSphere(const Ray *ray, const Point3D& center, float radius, float *t1, float *t2)
{
	radius += ray->radius;
	
	Vector3D q = ray->origin - center;
	const Vector3D& v = ray->direction;
	
	float a = v * v;
	float b = q * v;
	float c = q * q - radius * radius;
	
	float b2 = b * b;
	float ac = a * c;
	if (ac < b2)
	{
		a = 1.0F / a;
		b = -b;
		float D = Sqrt(b2 - ac);
		
		float u1 = (b - D) * a;
		float u2 = (b + D) * a;
		if ((u1 < ray->tmax) && (u2 > ray->tmin))
		{
			*t1 = u1;
			*t2 = u2;
			return (true);
		}
	}
	
	return (false);
}


float Math::DistancePointToLine(const Point3D& q, const Point3D& s, const Vector3D& v)
{
	Vector3D dqs = q - s;
	float kp = dqs * v;
	return (Sqrt(dqs * dqs - kp * kp / (v * v)));
}

float Math::SquaredDistancePointToLine(const Point3D& q, const Point3D& s, const Vector3D& v)
{
	Vector3D dqs = q - s;
	float kp = dqs * v;
	return (dqs * dqs - kp * kp / (v * v));
}

float Math::DistanceLineToLine(const Point3D& s1, const Vector3D& v1, const Point3D& s2, const Vector3D& v2)
{
	float vp2 = v1 * v1;
	float vq2 = v2 * v2;
	float v1v2 = v1 * v2;
	
	Vector3D ds = s2 - s1;
	float kp = ds * v1;
	
	float f = vp2 * vq2 - v1v2 * v1v2;
	if (Fabs(f) > K::min_float)
	{
		f = 1.0F / f;
		
		float kq = ds * v2;
		float ps = (vq2 * kp - v1v2 * kq) * f;
		float qt = (v1v2 * kp - vp2 * kq) * f;
		
		return (Magnitude(ds - v1 * ps + v2 * qt));
	}
	
	return (Sqrt(ds * ds - kp * kp / vp2));
}

float Math::SquaredDistanceLineToLine(const Point3D& s1, const Vector3D& v1, const Point3D& s2, const Vector3D& v2)
{
	float vp2 = v1 * v1;
	float vq2 = v2 * v2;
	float v1v2 = v1 * v2;
	
	Vector3D ds = s2 - s1;
	float kp = ds * v1;
	
	float f = vp2 * vq2 - v1v2 * v1v2;
	if (Fabs(f) > K::min_float)
	{
		f = 1.0F / f;
		
		float kq = ds * v2;
		float ps = (vq2 * kp - v1v2 * kq) * f;
		float qt = (v1v2 * kp - vp2 * kq) * f;
		
		return (SquaredMag(ds - v1 * ps + v2 * qt));
	}
	
	return (ds * ds - kp * kp / vp2);
}

bool Math::CalculateNearestParameters(const Point3D& s1, const Vector3D& v1, const Point3D& s2, const Vector3D& v2, float *s, float *t)
{
	float vp2 = v1 * v1;
	float vq2 = v2 * v2;
	float v1v2 = v1 * v2;
	
	Vector3D ds = s2 - s1;
	float kp = ds * v1;
	
	float f = vp2 * vq2 - v1v2 * v1v2;
	if (Fabs(f) > K::min_float)
	{
		f = 1.0F / f;
		
		float kq = ds * v2;
		*s = (vq2 * kp - v1v2 * kq) * f;
		*t = (v1v2 * kp - vp2 * kq) * f;
		
		return (true);
	}
	
	return (false);
}


bool Math::SolveLinearSystem(int32 n, float *m, float *r)
{
	float *rowNormalizer = new float[n];
	bool result = false;
	
	for (machine i = 0; i < n; i++)
	{
		const float *entry = m + i;
		float maxvalue = 0.0F;
		
		for (machine j = 0; j < n; j++)
		{
			float value = Fabs(*entry);
			if (value > maxvalue) maxvalue = value;
			entry += n;
		}
		
		if (maxvalue == 0.0F) goto exit;
		rowNormalizer[i] = 1.0F / maxvalue;
	}
	
	for (machine j = 0; j < n - 1; j++)
	{
		machine pivotRow = -1;
		float maxvalue = 0.0F;
		for (machine i = j; i < n; i++)
		{
			float p = Fabs(m[j * n + i]) * rowNormalizer[i];
			if (p > maxvalue)
			{
				maxvalue = p;
				pivotRow = i;
			}
		}
		
		if (pivotRow != j)
		{
			if (pivotRow == -1) goto exit;
			
			for (machine k = 0; k < n; k++)
			{
				float temp = m[k * n + j];
				m[k * n + j] = m[k * n + pivotRow];
				m[k * n + pivotRow] = temp;
			}
			
			float temp = r[j];
			r[j] = r[pivotRow];
			r[pivotRow] = temp;
			
			rowNormalizer[pivotRow] = rowNormalizer[j];
		}
		
		float denom = 1.0F / m[j * n + j];
		for (machine i = j + 1; i < n; i++)
		{
			float factor = m[j * n + i] * denom;
			r[i] -= r[j] * factor;
			for (machine k = 0; k < n; k++) m[k * n + i] -= m[k * n + j] * factor;
		}
	}
	
	for (machine i = n - 1; i >= 0; i--)
	{
		float sum = r[i];
		for (machine k = i + 1; k < n; k++) sum -= m[k * n + i] * r[k];
		r[i] = sum / m[i * n + i];
	}
	
	result = true;
	
	exit:
	delete[] rowNormalizer;
	return (result);
}

bool Math::LUDecompose(int32 n, float *m, unsigned_int16 *index, float *detSign)
{
	float *rowNormalizer = new float[n];
	float exchangeParity = 1.0F;
	bool result = false;
	
	for (machine i = 0; i < n; i++)
	{
		const float *entry = m + i;
		float maxvalue = 0.0F;
		
		for (machine j = 0; j < n; j++)
		{
			float value = Fabs(*entry);
			if (value > maxvalue) maxvalue = value;
			entry += n;
		}
		
		if (maxvalue == 0.0F) goto exit;
		rowNormalizer[i] = 1.0F / maxvalue;
		index[i] = (unsigned_int16) i;
	}
	
	for (machine j = 0; j < n; j++)
	{
		for (machine i = 1; i < j; i++)
		{
			float sum = m[j * n + i];
			for (machine k = 0; k < i; k++) sum -= m[k * n + i] * m[j * n + k];
			m[j * n + i] = sum;
		}
		
		machine pivotRow = -1;
		float maxvalue = 0.0F;
		for (machine i = j; i < n; i++)
		{
			float sum = m[j * n + i];
			for (machine k = 0; k < j; k++) sum -= m[k * n + i] * m[j * n + k];
			m[j * n + i] = sum;
			
			sum = Fabs(sum) * rowNormalizer[i];
			if (sum > maxvalue)
			{
				maxvalue = sum;
				pivotRow = i;
			}
		}
		
		if (pivotRow != j)
		{
			if (pivotRow == -1) goto exit;
			
			for (machine k = 0; k < n; k++)
			{
				float temp = m[k * n + j];
				m[k * n + j] = m[k * n + pivotRow];
				m[k * n + pivotRow] = temp;
			}
			
			Exchange(index[j], index[pivotRow]);
			rowNormalizer[pivotRow] = rowNormalizer[j];
			exchangeParity = -exchangeParity;
		}
		
		if (j != n - 1)
		{
			float denom = 1.0F / m[j * n + j];
			for (machine i = j + 1; i < n; i++) m[j * n + i] *= denom;
		}
	}
	
	if (detSign) *detSign = exchangeParity;
	result = true;
	
	exit:
	delete[] rowNormalizer;
	return (result);
}

void Math::LUBacksubstitute(int32 n, const float *d, const unsigned_int16 *index, const float *r, float *x)
{
	for (machine i = 0; i < n; i++) x[i] = r[index[i]];
	
	for (machine i = 0; i < n; i++)
	{
		float sum = x[i];
		for (machine k = 0; k < i; k++) sum -= d[k * n + i] * x[k];
		x[i] = sum;
	}
	
	for (machine i = n - 1; i >= 0; i--)
	{
		float sum = x[i];
		for (machine k = i + 1; k < n; k++) sum -= d[k * n + i] * x[k];
		x[i] = sum / d[i * n + i];
	}
}

void Math::LURefineSolution(int32 n, const float *m, const float *d, const unsigned_int16 *index, const float *r, float *x)
{
	float *t = new float[n];
	
	for (machine i = 0; i < n; i++)
	{
		double q = -r[i];
		for (machine k = 0; k < n; k++) q += m[k * n + i] * x[k];
		t[i] = (float) q;
	}
	
	LUBacksubstitute(n, d, index, t, t);
	for (machine i = 0; i < n; i++) x[i] -= t[i];
	
	delete[] t;
}

void Math::SolveTridiagonalSystem(int32 n, const float *a, const float *b, const float *c, const float *r, float *x)
{
	float *t = new float[n - 1];
	
	float recipBeta = 1.0F / b[0];
	x[0] = r[0] * recipBeta;
	
	for (machine i = 1; i < n; i++)
	{
		t[i - 1] = c[i - 1] * recipBeta;
		recipBeta = 1.0F / (b[i] - a[i] * t[i - 1]);
		x[i] = (r[i] - a[i] * x[i - 1]) * recipBeta;
	}
	
	for (machine i = n - 2; i >= 0; i--) x[i] -= t[i] * x[i + 1];
	
	delete[] t;
}


void Math::CalculateEigensystem(const Matrix3D& m, Vector3D *eigenvalue, Matrix3D *eigenvector)
{
	double m11 = m(0,0);
	double m12 = m(0,1);
	double m13 = m(0,2);
	double m22 = m(1,1);
	double m23 = m(1,2);
	double m33 = m(2,2);
	
	Matrix3D& r = *eigenvector;
	r.SetIdentity();
	
	for (machine a = 0; a < 32; a++)
	{
		if ((Fabs(m12) < kJacobiEpsilon) && (Fabs(m13) < kJacobiEpsilon) && (Fabs(m23) < kJacobiEpsilon)) break;
		
		if (Fabs(m12) > K::min_float)
		{
			double u = (m22 - m11) * 0.5F / m12;
			double u2 = u * u;
			double u2p1 = u2 + 1.0F;
			double t = (u2p1 != u2) ? ((u < 0.0F) ? -1.0F : 1.0F) * (Sqrt(u2p1) - Fabs(u)) : 0.5F / u;
			double c = InverseSqrt(t * t + 1.0F);
			double s = c * t;
			
			m11 -= t * m12;
			m22 += t * m12;
			m12 = 0.0F;
			
			double temp = c * m13 - s * m23;
			m23 = s * m13 + c * m23;
			m13 = temp;
			
			for (machine i = 0; i < 3; i++)
			{
				temp = c * r(i,0) - s * r(i,1);
				r(i,1) = (float) (s * r(i,0) + c * r(i,1));
				r(i,0) = (float) temp;
			}
		}
		
		if (Fabs(m13) > K::min_float)
		{
			double u = (m33 - m11) * 0.5F / m13;
			double u2 = u * u;
			double u2p1 = u2 + 1.0F;
			double t = (u2p1 != u2) ? ((u < 0.0F) ? -1.0F : 1.0F) * (Sqrt(u2p1) - Fabs(u)) : 0.5F / u;
			double c = InverseSqrt(t * t + 1.0F);
			double s = c * t;
			
			m11 -= t * m13;
			m33 += t * m13;
			m13 = 0.0F;
			
			double temp = c * m12 - s * m23;
			m23 = s * m12 + c * m23;
			m12 = temp;
			
			for (machine i = 0; i < 3; i++)
			{
				temp = c * r(i,0) - s * r(i,2);
				r(i,2) = (float) (s * r(i,0) + c * r(i,2));
				r(i,0) = (float) temp;
			}
		}
		
		if (Fabs(m23) > K::min_float)
		{
			double u = (m33 - m22) * 0.5F / m23;
			double u2 = u * u;
			double u2p1 = u2 + 1.0F;
			double t = (u2p1 != u2) ? ((u < 0.0F) ? -1.0F : 1.0F) * (Sqrt(u2p1) - Fabs(u)) : 0.5F / u;
			double c = InverseSqrt(t * t + 1.0F);
			double s = c * t;
			
			m22 -= t * m23;
			m33 += t * m23;
			m23 = 0.0F;
			
			double temp = c * m12 - s * m13;
			m13 = s * m12 + c * m13;
			m12 = temp;
			
			for (machine i = 0; i < 3; i++)
			{
				temp = c * r(i,1) - s * r(i,2);
				r(i,2) = (float) (s * r(i,1) + c * r(i,2));
				r(i,1) = (float) temp;
			}
		}
	}
	
	if (eigenvalue)
	{
		eigenvalue->x = (float) m11;
		eigenvalue->y = (float) m22;
		eigenvalue->z = (float) m33;
	}
}

void Math::CalculatePrincipalAxes(int32 vertexCount, const Point3D *vertex, Point3D *center, Vector3D *axis)
{
	Vector3D	eigenvalue;
	Matrix3D	eigenvector;
	
	float one_over_N = 1.0F / (float) vertexCount;
	
	center->Set(0.0F, 0.0F, 0.0F);
	for (machine i = 0; i < vertexCount; i++) *center += vertex[i];
	*center *= one_over_N;
	
	float c11 = 0.0F;
	float c22 = 0.0F;
	float c33 = 0.0F;
	float c12 = 0.0F;
	float c13 = 0.0F;
	float c23 = 0.0F;
	
	for (machine i = 0; i < vertexCount; i++)
	{
		Vector3D v = vertex[i] - *center;
		c11 += v.x * v.x;
		c22 += v.y * v.y;
		c33 += v.z * v.z;
		c12 += v.x * v.y;
		c13 += v.x * v.z;
		c23 += v.y * v.z;
	}
	
	c11 *= one_over_N;
	c22 *= one_over_N;
	c33 *= one_over_N;
	c12 *= one_over_N;
	c13 *= one_over_N;
	c23 *= one_over_N;
	
	CalculateEigensystem(Matrix3D(c11, c12, c13, c12, c22, c23, c13, c23, c33), &eigenvalue, &eigenvector);
		
	c11 = Fabs(eigenvalue[0]);
	c22 = Fabs(eigenvalue[1]);
	c33 = Fabs(eigenvalue[2]);
	
	if ((c11 > c22) && (c11 > c33))
	{
		axis[0] = eigenvector[0];
		if (c22 > c33)
		{
			axis[1] = eigenvector[1];
			axis[2] = eigenvector[2];
		}
		else
		{
			axis[1] = eigenvector[2];
			axis[2] = eigenvector[1];
		}
	}
	else if (c22 > c33)
	{
		axis[0] = eigenvector[1];
		if (c11 > c33)
		{
			axis[1] = eigenvector[0];
			axis[2] = eigenvector[2];
		}
		else
		{
			axis[1] = eigenvector[2];
			axis[2] = eigenvector[0];
		}
	}
	else
	{
		axis[0] = eigenvector[2];
		if (c11 > c22)
		{
			axis[1] = eigenvector[0];
			axis[2] = eigenvector[1];
		}
		else
		{
			axis[1] = eigenvector[1];
			axis[2] = eigenvector[0];
		}
	}
}


bool Math::ClipSegmentAgainstPlane(Point3D *p1, Point3D *p2, const Antivector4D& plane)
{
	float d1 = plane ^ *p1;
	float d2 = plane ^ *p2;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (false);
		
		float t = d1 / (d2 - d1);
		*p1 -= t * (*p2 - *p1);
	}
	else if (d2 < 0.0F)
	{
		float t = d2 / (d1 - d2);
		*p2 -= t * (*p1 - *p2);
	}
	
	return (true);
}

float Math::GetPolygonArea(int32 vertexCount, const Point3D *vertex, const Vector3D& normal)
{
	Vector3D area(0.0F, 0.0F, 0.0F);
	
	const Point3D& p0 = vertex[0];
	Vector3D dp = vertex[1] - p0;
	for (machine a = 2; a < vertexCount; a++)
	{
		const Point3D& q = vertex[a];
		Vector3D dq = q - p0;
		area += dp % dq;
		dp = dq;
	}
	
	return (area * normal * 0.5F);
}

bool Math::ConvexPolygon(int32 vertexCount, const Point3D *vertex, const Vector3D& normal)
{
	const Point3D *p1 = &vertex[vertexCount - 2];
	const Point3D *p2 = &vertex[vertexCount - 1];
	for (machine a = 0; a < vertexCount; a++)
	{
		const Point3D *p3 = &vertex[a];
		
		Vector3D dp = *p3 - *p1;
		Vector3D inward = normal % dp * InverseMag(dp);
		
		float d = inward * (*p2 - *p1);
		if (d > -kBoundaryEpsilon) return (false);
		
		p1 = p2;
		p2 = p3;
	}
	
	return (true);
}

int32 Math::PointInConvexPolygon(const Point3D& p, int32 vertexCount, const Point3D *vertex, const Vector3D& normal)
{
	int32 location = kPolygonInterior;
	
	const Point3D *p1 = &vertex[vertexCount - 1];
	for (machine a = 0; a < vertexCount; a++)
	{
		const Point3D *p2 = &vertex[a];
		
		Vector3D dp = *p2 - *p1;
		Vector3D inward = normal % dp * InverseMag(dp);
		
		float d = inward * (p - *p1);
		if (d < -kBoundaryEpsilon) return (kPolygonExterior);
		else if (d < kBoundaryEpsilon) location = kPolygonBoundary;
		
		p1 = p2;
	}
	
	return (location);
}

int32 Math::ClipPolygonAgainstPlane(int32 vertexCount, const Point3D *vertex, const Antivector4D& plane, int8 *location, Point3D *restrict result)
{
	int32 positive = 0;
	int32 negative = 0;
	
	for (machine a = 0; a < vertexCount; a++)
	{
		float d = plane ^ vertex[a];
		if (d > kBoundaryEpsilon)
		{
			location[a] = kPolygonInterior;
			positive++;
		}
		else if (d < -kBoundaryEpsilon)
		{
			location[a] = kPolygonExterior;
			negative++;
		}
		else
		{
			location[a] = kPolygonBoundary;
		}
	}
	
	if (negative == 0)
	{
		for (machine a = 0; a < vertexCount; a++) result[a] = vertex[a];
		return (vertexCount);
	}
	else if (positive == 0)
	{
		return (0);
	}
	
	int32 count = 0;
	int32 previous = vertexCount - 1;
	for (machine index = 0; index < vertexCount; index++)
	{
		int8 loc = location[index];
		if (loc == kPolygonExterior)
		{
			if (location[previous] == kPolygonInterior)
			{
				const Point3D& v1 = vertex[previous];
				const Point3D& v2 = vertex[index];
				Vector3D dv = v2 - v1;
				
				float t = (plane ^ v2) / (plane ^ dv);
				result[count++] = v2 - dv * t;
			}
		}
		else
		{
			const Point3D& v1 = vertex[index];
			if ((loc == kPolygonInterior) && (location[previous] == kPolygonExterior))
			{
				const Point3D& v2 = vertex[previous];
				Vector3D dv = v2 - v1;
				
				float t = (plane ^ v2) / (plane ^ dv);
				result[count++] = v2 - dv * t;
			}
			
			result[count++] = v1;
		}
		
		previous = index;
	}
	
	return (count);
}

bool Math::ClipPolyhedronAgainstPlane(const Polyhedron *polyhedron, const Antivector4D& plane, Polyhedron *result)
{
	int8			vertexLocation[kMaxPolyhedronVertexCount];
	int8			edgeLocation[kMaxPolyhedronEdgeCount];
	
	unsigned_int16	vertexRemap[kMaxPolyhedronVertexCount];
	unsigned_int16	edgeRemap[kMaxPolyhedronEdgeCount];
	unsigned_int16	faceRemap[kMaxPolyhedronFaceCount];
	
	unsigned_int16	planeEdgeTable[kMaxFaceEdgeCount];
	
	int32 maxLocation = 0;
	int32 total = 0;
	
	int32 vertexCount = polyhedron->vertexCount;
	for (machine a = 0; a < vertexCount; a++)
	{
		vertexRemap[a] = 0xFFFF;
		
		float d = plane ^ polyhedron->vertex[a];
		int8 location = (int8) ((d > kBoundaryEpsilon) ? 3 : (d > -kBoundaryEpsilon) ? 1 : 0);
		vertexLocation[a] = location;
		
		maxLocation = Max(maxLocation, location);
		total += location;
	}
	
	int32 edgeCount = polyhedron->edgeCount;
	int32 faceCount = polyhedron->faceCount;
	
	if (total == vertexCount * 3)
	{
		result->vertexCount = vertexCount;
		result->edgeCount = edgeCount;
		result->faceCount = faceCount;
		
		for (machine a = 0; a < vertexCount; a++) result->vertex[a] = polyhedron->vertex[a];
		for (machine a = 0; a < edgeCount; a++) result->edge[a] = polyhedron->edge[a];
		for (machine a = 0; a < faceCount; a++) result->face[a] = polyhedron->face[a];
		
		return (true);
	}
	else if (maxLocation < 3)
	{
		return (false);
	}
	
	for (machine a = 0; a < edgeCount; a++)
	{
		edgeRemap[a] = 0xFFFF;
		
		const Edge *edge = &polyhedron->edge[a];
		edgeLocation[a] = (int8) (vertexLocation[edge->vertexIndex[0]] + vertexLocation[edge->vertexIndex[1]]);
	}
	
	int32 resultFaceCount = 0;
	for (machine a = 0; a < faceCount; a++)
	{
		faceRemap[a] = 0xFFFF;
		
		const Face *face = &polyhedron->face[a];
		int32 faceEdgeCount = face->edgeCount;
		
		for (machine b = 0; b < faceEdgeCount; b++)
		{
			if (edgeLocation[face->edgeIndex[b]] > 2)
			{
				// Face has a vertex on the positive side of the plane.
				
				Face *resultFace = &result->face[resultFaceCount];
				faceRemap[a] = (unsigned_int16) resultFaceCount++;
				resultFace->plane = face->plane;
				break;
			}
		}
	}
	
	int32 resultVertexCount = 0;
	int32 resultEdgeCount = 0;
	int32 planeEdgeCount = 0;
	
	for (machine a = 0; a < edgeCount; a++)
	{
		int32 location = edgeLocation[a];
		if (location > 1)
		{
			// At least one endpoint lies on the positive side of the plane
			// or both endpoints lie in the plane.
			
			edgeRemap[a] = (unsigned_int16) resultEdgeCount;
			
			const Edge *edge = &polyhedron->edge[a];
			Edge *resultEdge = &result->edge[resultEdgeCount];
			
			resultEdge->faceIndex[0] = faceRemap[edge->faceIndex[0]];
			resultEdge->faceIndex[1] = faceRemap[edge->faceIndex[1]];
			
			int32 vertexIndex1 = edge->vertexIndex[0];
			int32 vertexIndex2 = edge->vertexIndex[1];
			
			if (vertexLocation[vertexIndex1] != 0)
			{
				// Vertex 1 is not on the negative side of the plane.
				
				int32 remap = vertexRemap[vertexIndex1];
				if (remap == 0xFFFF)
				{
					remap = resultVertexCount++;
					vertexRemap[vertexIndex1] = (unsigned_int16) remap;
					result->vertex[remap] = polyhedron->vertex[vertexIndex1];
				}
				
				resultEdge->vertexIndex[0] = (unsigned_int16) remap;
			}
			else
			{
				// Vertex 1 is on the negative side, vertex 2 is on the positive side.
				
				const Point3D& p1 = polyhedron->vertex[vertexIndex2];
				const Point3D& p2 = polyhedron->vertex[vertexIndex1];
				Vector3D dp = p2 - p1;
				
				result->vertex[resultVertexCount] = p1 - dp * ((plane ^ p1) / (plane ^ dp));
				resultEdge->vertexIndex[0] = (unsigned_int16) resultVertexCount++;
			}
			
			if (vertexLocation[vertexIndex2] != 0)
			{
				// Vertex 2 is not on the negative side of the plane.
				
				int32 remap = vertexRemap[vertexIndex2];
				if (remap == 0xFFFF)
				{
					remap = resultVertexCount++;
					vertexRemap[vertexIndex2] = (unsigned_int16) remap;
					result->vertex[remap] = polyhedron->vertex[vertexIndex2];
				}
				
				resultEdge->vertexIndex[1] = (unsigned_int16) remap;
			}
			else
			{
				// Vertex 2 is on the negative side, vertex 1 is on the positive side.
				
				const Point3D& p1 = polyhedron->vertex[vertexIndex1];
				const Point3D& p2 = polyhedron->vertex[vertexIndex2];
				Vector3D dp = p2 - p1;
				
				result->vertex[resultVertexCount] = p1 - dp * ((plane ^ p1) / (plane ^ dp));
				resultEdge->vertexIndex[1] = (unsigned_int16) resultVertexCount++;
			}
			
			if (location == 2)
			{
				// Both endpoints lie in the plane.
				
				planeEdgeTable[planeEdgeCount++] = (unsigned_int16) resultEdgeCount;
				
				if (resultEdge->faceIndex[0] == 0xFFFF)
				{
					resultEdge->faceIndex[0] = resultEdge->faceIndex[1];
					resultEdge->faceIndex[1] = 0xFFFF;
					
					unsigned_int16 index = resultEdge->vertexIndex[0];
					resultEdge->vertexIndex[0] = resultEdge->vertexIndex[1];
					resultEdge->vertexIndex[1] = index;
				}
			}
			
			resultEdgeCount++;
		}
		else if (location == 1)
		{
			// One endpoint lies in the plane, the other is on the negative side.
			
			const Edge *edge = &polyhedron->edge[a];
			if ((faceRemap[edge->faceIndex[0]] != 0xFFFF) || (faceRemap[edge->faceIndex[1]] != 0xFFFF))
			{
				int32 vertexIndex1 = edge->vertexIndex[0];
				int32 vertexIndex2 = edge->vertexIndex[1];
				int32 index = (vertexLocation[vertexIndex1] == 1) ? vertexIndex1 : vertexIndex2;
				
				int32 remap = vertexRemap[index];
				if (remap == 0xFFFF)
				{
					remap = resultVertexCount++;
					vertexRemap[index] = (unsigned_int16) remap;
					result->vertex[remap] = polyhedron->vertex[index];
				}
			}
		}
	}
	
	for (machine a = 0; a < faceCount; a++)
	{
		int32 remap = faceRemap[a];
		if (remap != 0xFFFF)
		{
			int32 newEdgeIndex = 0xFFFF;
			
			const Face *face = &polyhedron->face[a];
			int32 faceEdgeCount = face->edgeCount;
			
			Face *resultFace = &result->face[remap];
			int32 resultFaceEdgeCount = 0;
			
			Edge *newEdge = nullptr;
			for (machine b = 0; b < faceEdgeCount; b++)
			{
				int32 edgeIndex = face->edgeIndex[b];
				
				int32 location = edgeLocation[edgeIndex];
				if (location > 0)
				{
					if ((location & 1) == 0)
					{
						// Edge was not clipped, location == 2,4,6.
						
						resultFace->edgeIndex[resultFaceEdgeCount++] = edgeRemap[edgeIndex];
					}
					else
					{
						// One endpoint is on negative side of plane, location == 1,3.
						
						if (!newEdge)
						{
							newEdgeIndex = resultEdgeCount;
							newEdge = &result->edge[resultEdgeCount];
							planeEdgeTable[planeEdgeCount++] = (unsigned_int16) resultEdgeCount++;
							
							newEdge->faceIndex[0] = (unsigned_int16) remap;
							newEdge->faceIndex[1] = 0xFFFF;
						}
						
						const Edge *edge = &polyhedron->edge[edgeIndex];
						unsigned_int16 index = edge->vertexIndex[0];
						
						if (location == 3)
						{
							// One endpoint is on positive side, and other is on negative side.
							
							const Edge *resultEdge = &result->edge[edgeRemap[edgeIndex]];
							if (edge->faceIndex[0] == a)
							{
								if (vertexLocation[index] != 0)
								{
									newEdge->vertexIndex[0] = resultEdge->vertexIndex[1];
									resultFace->edgeIndex[resultFaceEdgeCount++] = edgeRemap[edgeIndex];
									resultFace->edgeIndex[resultFaceEdgeCount++] = (unsigned_int16) newEdgeIndex;
								}
								else
								{
									newEdge->vertexIndex[1] = resultEdge->vertexIndex[0];
									resultFace->edgeIndex[resultFaceEdgeCount++] = edgeRemap[edgeIndex];
								}
							}
							else
							{
								if (vertexLocation[index] != 0)
								{
									newEdge->vertexIndex[1] = resultEdge->vertexIndex[1];
									resultFace->edgeIndex[resultFaceEdgeCount++] = edgeRemap[edgeIndex];
								}
								else
								{
									newEdge->vertexIndex[0] = resultEdge->vertexIndex[0];
									resultFace->edgeIndex[resultFaceEdgeCount++] = edgeRemap[edgeIndex];
									resultFace->edgeIndex[resultFaceEdgeCount++] = (unsigned_int16) newEdgeIndex;
								}
							}
						}
						else
						{
							// One endpoint is in the plane, other is on negative side.
							
							if (edge->faceIndex[0] == a)
							{
								if (vertexLocation[index] != 0)
								{
									newEdge->vertexIndex[0] = vertexRemap[index];
									resultFace->edgeIndex[resultFaceEdgeCount++] = (unsigned_int16) newEdgeIndex;
								}
								else
								{
									newEdge->vertexIndex[1] = vertexRemap[edge->vertexIndex[1]];
								}
							}
							else
							{
								if (vertexLocation[index] != 0)
								{
									newEdge->vertexIndex[1] = vertexRemap[index];
								}
								else
								{
									newEdge->vertexIndex[0] = vertexRemap[edge->vertexIndex[1]];
									resultFace->edgeIndex[resultFaceEdgeCount++] = (unsigned_int16) newEdgeIndex;
								}
							}
						}
					}
				}
			}
			
			resultFace->edgeCount = (unsigned_int16) resultFaceEdgeCount;
		}
	}
	
	if (planeEdgeCount > 2)
	{
		Face *resultFace = &result->face[resultFaceCount];
		resultFace->plane = -plane;
		
		int32 edgeIndex = planeEdgeTable[0];
		Edge *resultEdge = &result->edge[edgeIndex];
		
		resultFace->edgeIndex[0] = (unsigned_int16) edgeIndex;
		resultEdge->faceIndex[1] = (unsigned_int16) resultFaceCount;
		
		int32 faceEdgeCount = 1;
		int32 prevVertexIndex = resultEdge->vertexIndex[0];
		for (machine a = 1; a < planeEdgeCount; a++)
		{
			for (machine b = 1; b < planeEdgeCount; b++)
			{
				edgeIndex = planeEdgeTable[b];
				resultEdge = &result->edge[edgeIndex];
				if (resultEdge->vertexIndex[1] == prevVertexIndex)
				{
					resultFace->edgeIndex[faceEdgeCount++] = (unsigned_int16) edgeIndex;
					resultEdge->faceIndex[1] = (unsigned_int16) resultFaceCount;
					prevVertexIndex = resultEdge->vertexIndex[0];
					break;
				}
			}
		}
		
		resultFace->edgeCount = faceEdgeCount;
		resultFaceCount++;
	}
	
	result->vertexCount = resultVertexCount;
	result->edgeCount = resultEdgeCount;
	result->faceCount = resultFaceCount;
	return (true);
}


static int32 GetNextActiveVertex(int32 x, int32 vertexCount, const bool *active)
{
	for (;;)
	{
		if (++x == vertexCount) x = 0;
		if (active[x]) return (x);
	}
}

static int32 GetPrevActiveVertex(int32 x, int32 vertexCount, const bool *active)
{
	for (;;)
	{
		if (--x == -1) x = vertexCount - 1;
		if (active[x]) return (x);
	}
}

static bool ChooseTriangle(const Point3D& vp1, const Point3D& vp2, const Point3D& vm1, const Point3D& vm2)
{
	const float onePlusEpsilon = 1.125F;
	
	Vector3D dp = (vp2 - vp1).Normalize();
	float pd1 = Fabs(((vm1 - vp1).Normalize() - (vp2 - vm1).Normalize()) * dp);
	float md1 = Fabs(((vm2 - vp1).Normalize() - (vp2 - vm2).Normalize()) * dp);
	
	Vector3D dm = (vm2 - vm1).Normalize();
	float md2 = Fabs(((vp1 - vm1).Normalize() - (vm2 - vp1).Normalize()) * dm);
	float pd2 = Fabs(((vp2 - vm1).Normalize() - (vm2 - vp2).Normalize()) * dm);
	
	return (Fmax(pd1, pd2) < Fmax(md1, md2) * onePlusEpsilon);
}

int32 Math::TriangulatePolygon(int32 vertexCount, const Point3D *vertex, const Vector3D& normal, Triangle *triangle, int32 base)
{
	bool *active = new bool[vertexCount];
	for (machine a = 0; a < vertexCount; a++) active[a] = true;
	
	int32 triangleCount = 0;
	int32 start = 0;
	
	int32 p1 = 0;
	int32 p2 = 1;
	int32 m1 = vertexCount - 1;
	int32 m2 = vertexCount - 2;
	
	for (;;)
	{
		if (p2 == m2)
		{
			triangle->Set(m1 + base, p1 + base, p2 + base);
			triangleCount++;
			break;
		}
		
		const Point3D& vp1 = vertex[p1];
		const Point3D& vp2 = vertex[p2];
		const Point3D& vm1 = vertex[m1];
		const Point3D& vm2 = vertex[m2];
		
		bool positive = false;
		bool negative = false;
		
		Vector3D n1 = normal % (vm1 - vp2).Normalize();
		if (n1 * (vp1 - vp2).Normalize() > kTriangleEpsilon)
		{
			positive = true;
			
			Vector3D n2 = (normal % (vp1 - vm1).Normalize());
			Vector3D n3 = (normal % (vp2 - vp1).Normalize());
			
			for (machine a = 0; a < vertexCount; a++) if (active[a])
			{
				const Vector3D& v = vertex[a];
				
				Vector3D dvp1 = v - vp1;
				Vector3D dvp2 = v - vp2;
				Vector3D dvm1 = v - vm1;
				
				float mvp1 = SquaredMag(dvp1);
				float mvp2 = SquaredMag(dvp2);
				float mvm1 = SquaredMag(dvm1);
				
				if ((mvp1 >= kWeldEpsilonSquared) && (mvp2 >= kWeldEpsilonSquared) && (mvm1 >= kWeldEpsilonSquared))
				{
					float d1 = n1 * dvp2 * InverseSqrt(mvp2);
					float d2 = n2 * dvm1 * InverseSqrt(mvm1);
					float d3 = n3 * dvp1 * InverseSqrt(mvp1);
					
					if ((d1 > -kTriangleEpsilon) && (d2 > -kTriangleEpsilon) && (d3 > -kTriangleEpsilon))
					{
						positive = false;
						break;
					}
				}
			}
		}
		
		n1 = normal % (vm2 - vp1).Normalize();
		if (n1 * (vm1 - vp1).Normalize() > kTriangleEpsilon)
		{
			negative = true;
			
			Vector3D n2 = (normal % (vm1 - vm2).Normalize());
			Vector3D n3 = (normal % (vp1 - vm1).Normalize());
			
			for (machine a = 0; a < vertexCount; a++) if (active[a])
			{
				const Vector3D& v = vertex[a];
				
				Vector3D dvm1 = v - vm1;
				Vector3D dvm2 = v - vm2;
				Vector3D dvp1 = v - vp1;
				
				float mvm1 = SquaredMag(dvm1);
				float mvm2 = SquaredMag(dvm2);
				float mvp1 = SquaredMag(dvp1);
				
				if ((mvm1 >= kWeldEpsilonSquared) && (mvm2 >= kWeldEpsilonSquared) && (mvp1 >= kWeldEpsilonSquared))
				{
					float d1 = n1 * dvp1 * InverseSqrt(mvp1);
					float d2 = n2 * dvm2 * InverseSqrt(mvm2);
					float d3 = n3 * dvm1 * InverseSqrt(mvm1);
					
					if ((d1 > -kTriangleEpsilon) && (d2 > -kTriangleEpsilon) && (d3 > -kTriangleEpsilon))
					{
						negative = false;
						break;
					}
				}
			}
		}
		
		if ((positive) && (negative))
		{
			if (ChooseTriangle(vp1, vp2, vm1, vm2)) negative = false;
			else positive = false;
		}
		
		if (positive)
		{
			active[p1] = false;
			triangle->Set(m1 + base, p1 + base, p2 + base);
			triangleCount++;
			triangle++;
			
			p1 = GetNextActiveVertex(p1, vertexCount, active);
			p2 = GetNextActiveVertex(p2, vertexCount, active);
			start = -1;
		}
		else if (negative)
		{
			active[m1] = false;
			triangle->Set(m2 + base, m1 + base, p1 + base);
			triangleCount++;
			triangle++;
			
			m1 = GetPrevActiveVertex(m1, vertexCount, active);
			m2 = GetPrevActiveVertex(m2, vertexCount, active);
			start = -1;
		}
		else
		{
			if (start == -1) start = p2;
			else if (p2 == start) break;
			
			m2 = m1;
			m1 = p1;
			p1 = p2;
			p2 = GetNextActiveVertex(p2, vertexCount, active);
		}
	}
	
	delete[] active;
	return (triangleCount);
}

static bool IntersectPolygon(int32 ip1, int32 ip2, int32 vertexCount, const unsigned_int16 *vertexIndex, const Point3D *vertex)
{
	const Point3D& p1 = vertex[ip1];
	const Point3D& p2 = vertex[ip2];
	Vector3D vp = p2 - p1;
	
	int32 iq1 = vertexIndex[vertexCount - 1];
	for (machine a = 0; a < vertexCount; a++)
	{
		int32 iq2 = vertexIndex[a];
		
		if ((iq1 != ip1) && (iq1 != ip2) && (iq2 != ip1) && (iq2 != ip2))
		{
			float	s, t;
			
			const Point3D& q1 = vertex[iq1];
			const Point3D& q2 = vertex[iq2];
			Vector3D vq = q2 - q1;
			
			if (Math::CalculateNearestParameters(p1, vp, q1, vq, &s, &t))
			{
				if ((s > -kTriangleEpsilon) && (s < 1.0F + kTriangleEpsilon) && (t > -kTriangleEpsilon) && (t < 1.0F + kTriangleEpsilon)) return (true);
			}
		}
		
		iq1 = iq2;
	}
	
	return (false);
}

int32 Math::TriangulatePolygon(int32 windingCount, const int32 *vertexCount, const unsigned_int16 *const *vertexIndex, const Point3D *vertex, const Vector3D& normal, Point3D *result, Triangle *triangle, int32 triangleCount)
{
	int32 outsideVertexCount = vertexCount[0];
	const unsigned_int16 *outsideWinding = vertexIndex[0];
	
	if (windingCount == 1)
	{
		unsigned_int16 *map = new unsigned_int16[outsideVertexCount];
		for (machine a = 0; a < outsideVertexCount; a++)
		{
			int32 index = outsideWinding[a];
			result[a] = vertex[index];
			map[index] = (unsigned_int16) a;
		}
		
		Triangle *t = triangle;
		for (machine a = 0; a < triangleCount; a++)
		{
			t->Set(map[t->index[0]], map[t->index[1]], map[t->index[2]]);
			t++;
		}
		
		delete[] map;
		return (TriangulatePolygon(outsideVertexCount, result, normal, triangle + triangleCount) + triangleCount);
	}
	
	int32 insideVertexCount = vertexCount[windingCount - 1];
	const unsigned_int16 *insideWinding = vertexIndex[windingCount - 1];
	
	int32 *newVertexCount = new int32[windingCount - 1];
	const unsigned_int16 **newVertexIndex = new const unsigned_int16 *[windingCount - 1];
	unsigned_int16 *newOutsideWinding = new unsigned_int16[outsideVertexCount + insideVertexCount];
	
	int32 ip1 = insideWinding[insideVertexCount - 1];
	for (machine inside = 0; inside < insideVertexCount; inside++)
	{
		Point3D		quad[4];
		
		int32 ip2 = insideWinding[inside];
		
		quad[0] = vertex[ip1];
		quad[1] = vertex[ip2];
		
		int32 index = -1;
		float closest = K::infinity;
		
		int32 iq1 = outsideWinding[outsideVertexCount - 1];
		for (machine outside = 0; outside < outsideVertexCount; outside++)
		{
			int32 iq2 = outsideWinding[outside];
			
			quad[2] = vertex[iq1];
			quad[3] = vertex[iq2];
			
			if (ConvexPolygon(4, quad, normal))
			{
				for (machine a = 0; a < windingCount; a++)
				{
					if (IntersectPolygon(ip2, iq1, vertexCount[a], vertexIndex[a], vertex)) goto nextQuad;
				}
				
				for (machine a = 0; a < windingCount; a++)
				{
					if (IntersectPolygon(ip1, iq2, vertexCount[a], vertexIndex[a], vertex)) goto nextQuad;
				}
				
				float distance = SquaredMag(vertex[ip1] - vertex[iq1]);
				if (distance < closest)
				{
					index = outside;
					closest = distance;
				}
			}
			
			nextQuad:
			iq1 = iq2;
		}
		
		if (index != -1)
		{
			int32 prev = index - 1;
			if (prev < 0) prev += outsideVertexCount;
			
			iq1 = outsideWinding[prev];
			int32 iq2 = outsideWinding[index];
			
			Triangle *t = &triangle[triangleCount];
			triangleCount += 2;
			
			if (ChooseTriangle(vertex[ip2], vertex[ip1], vertex[iq1], vertex[iq2]))
			{
				t[0].Set(ip1, ip2, iq1);
				t[1].Set(iq1, iq2, ip1);
			}
			else
			{
				t[0].Set(ip1, ip2, iq2);
				t[1].Set(iq1, iq2, ip2);
			}
			
			for (machine i = 0; i < insideVertexCount; i++)
			{
				newOutsideWinding[i] = insideWinding[inside];
				if (++inside == insideVertexCount) inside = 0;
			}
			
			for (machine i = 0; i < outsideVertexCount; i++)
			{
				newOutsideWinding[insideVertexCount + i] = outsideWinding[index];
				if (++index == outsideVertexCount) index = 0;
			}
			
			break;
		}
		
		ip1 = ip2;
	}
	
	windingCount--;
	
	newVertexCount[0] = outsideVertexCount + insideVertexCount;
	newVertexIndex[0] = newOutsideWinding;
	for (machine a = 1; a < windingCount; a++)
	{
		newVertexCount[a] = vertexCount[a];
		newVertexIndex[a] = vertexIndex[a];
	}
	
	triangleCount = TriangulatePolygon(windingCount, newVertexCount, newVertexIndex, vertex, normal, result, triangle, triangleCount);
	
	delete[] newOutsideWinding;
	delete[] newVertexIndex;
	delete[] newVertexCount;
	
	return (triangleCount);
}

void Math::IntersectConvexPolygons(const int32 *vertexCount, const Point3D *const *vertex, const Vector3D& normal, int32 *resultCount, Point3D *result)
{
	Point3D		*resultVertex[2];
	
	int32 polygonCount = vertexCount[0];
	int32 planeCount = vertexCount[1];
	int32 totalCount = polygonCount + planeCount;
	
	const Point3D *polygonVertex = vertex[0];
	const Point3D *planeVertex = vertex[1];
	
	Buffer buffer(totalCount * (sizeof(Point3D) + 1));
	resultVertex[0] = static_cast<Point3D *>(*buffer);
	resultVertex[1] = result;
	int8 *location = reinterpret_cast<int8 *>(resultVertex[0] + totalCount);
	
	int32 parity = planeCount & 1;
	const Point3D *p1 = &planeVertex[planeCount - 1];
	for (machine a = 0; a < planeCount; a++)
	{
		const Point3D *p2 = &planeVertex[a];
		
		Vector3D dp = *p2 - *p1;
		Vector3D inward = normal % dp * InverseMag(dp);
		Antivector4D plane(inward, *p1);
		
		polygonCount = ClipPolygonAgainstPlane(polygonCount, polygonVertex, plane, location, resultVertex[parity]);
		if (polygonCount == 0) break;
		
		polygonVertex = resultVertex[parity];
		parity ^= 1;
		
		p1 = p2;
	}
	
	*resultCount = polygonCount;
}

static int32 ClipSubtractPolygon(int32 vertexCount, const SubtractVertex *vertex, const Antivector4D& plane, int32 planeIndex, int32 planeCount, int8 *location, SubtractVertex *result)
{
	int32 positive = 0;
	int32 negative = 0;
	
	for (machine a = 0; a < vertexCount; a++)
	{
		float d = plane ^ vertex[a].position;
		if (d > kBoundaryEpsilon)
		{
			location[a] = kPolygonInterior;
			positive++;
		}
		else
		{
			if (d < -kBoundaryEpsilon)
			{
				location[a] = kPolygonExterior;
				negative++;
			}
			else
			{
				location[a] = kPolygonBoundary;
			}
		}
	}
	
	if (positive == vertexCount)
	{
		for (machine a = 0; a < vertexCount; a++) result[a] = vertex[a];
		return (vertexCount);
	}
	else if (positive == 0)
	{
		return (0);
	}
	
	int32 count = 0;
	int32 index = vertexCount - 1;
	for (machine next = 0; next < vertexCount; next++)
	{
		int32 loc = location[index];
		
		if (loc == kPolygonInterior)
		{
			result[count++] = vertex[index];
			
			if (location[next] == kPolygonExterior)
			{
				const Point3D& v1 = vertex[next].position;
				const Point3D& v2 = vertex[index].position;
				Vector3D dv = v2 - v1;
				
				float t = (plane ^ v2) / (plane ^ dv);
				result[count++].Set(v2 - dv * t, Max(vertex[next].direction - 1, -1), planeIndex, true);
			}
		}
		else if (loc == kPolygonExterior)
		{
			if (location[next] == kPolygonInterior)
			{
				const Point3D& v1 = vertex[index].position;
				const Point3D& v2 = vertex[next].position;
				Vector3D dv = v2 - v1;
				
				float t = (plane ^ v2) / (plane ^ dv);
				result[count++].Set(v2 - dv * t, Min(vertex[index].direction + 1, 1), planeIndex, true);
			}
		}
		else
		{
			int32 direction = vertex[index].direction;
			if (location[next] != kPolygonInterior) direction--;
			else direction++;
			
			result[count++].Set(vertex[index].position, direction, planeIndex, true);
		}
		
		index = next;
	}
	
	return (count);
}

bool Math::SubtractConvexPolygons(const int32 *vertexCount, const Point3D *const *vertex, const Vector3D& normal, int32 *resultCount, int32 *triangleCount, Point3D *result, Triangle *triangle)
{
	SubtractVertex	*subtractWinding[2];
	
	int32 positiveCount = vertexCount[0];
	int32 subtractCount = vertexCount[1];
	int32 totalCount = positiveCount + subtractCount;
	
	const Point3D *positiveVertex = vertex[0];
	const Point3D *subtractVertex = vertex[1];
	
	int32 positiveExteriorCount = 0;
	Vector3D reverseNormal = -normal;
	for (machine a = 0; a < positiveCount; a++)
	{
		int32 location = PointInConvexPolygon(positiveVertex[a], subtractCount, subtractVertex, reverseNormal);
		if (location == kPolygonExterior) positiveExteriorCount++;
	}
	
	if (positiveExteriorCount == 0)
	{
		*resultCount = 0;
		*triangleCount = 0;
		return (true);
	}
	
	int32 subtractInteriorCount = 0;
	for (machine a = 0; a < subtractCount; a++)
	{
		int32 location = PointInConvexPolygon(subtractVertex[a], positiveCount, positiveVertex, normal);
		if (location == kPolygonInterior) subtractInteriorCount++;
	}
	
	if (subtractInteriorCount == subtractCount)
	{
		const unsigned_int16 *vertexIndex[2];
		
		Buffer buffer((sizeof(Point3D) + 2) * totalCount);
		Point3D *newVertex = static_cast<Point3D *>(*buffer);
		unsigned_int16 *newIndex = reinterpret_cast<unsigned_int16 *>(newVertex + totalCount);
		
		for (machine a = 0; a < positiveCount; a++)
		{
			newIndex[a] = (unsigned_int16) a;
			newVertex[a] = positiveVertex[a];
		}
		
		for (machine a = 0; a < subtractCount; a++)
		{
			int32 index = a + positiveCount;
			newIndex[index] = (unsigned_int16) index;
			newVertex[index] = subtractVertex[a];
		}
		
		vertexIndex[0] = &newIndex[0];
		vertexIndex[1] = &newIndex[positiveCount];
		
		*resultCount = totalCount;
		*triangleCount = TriangulatePolygonalComplex(2, vertexCount, vertexIndex, newVertex, normal, result, triangle);
		return (true);
	}
	
	int32 maxVertexCount = totalCount + subtractCount;
	Buffer buffer(maxVertexCount * (sizeof(SubtractVertex) * 2 + 1));
	
	subtractWinding[0] = static_cast<SubtractVertex *>(*buffer);
	subtractWinding[1] = subtractWinding[0] + maxVertexCount;
	int8 *location = reinterpret_cast<int8 *>(subtractWinding[1] + maxVertexCount);
	
	for (machine a = 0; a < subtractCount; a++) subtractWinding[0][a].Set(subtractVertex[a]);
	
	int32 clipVertexCount = subtractCount;
	const Point3D *p1 = &positiveVertex[positiveCount - 1];
	for (machine a = 0; a < positiveCount; a++)
	{
		const Point3D *p2 = &positiveVertex[a];
		
		Vector3D dp = *p2 - *p1;
		Vector3D inward = normal % dp * InverseMag(dp);
		Antivector4D plane(inward, *p1);
		
		int32 p = a & 1;
		clipVertexCount = ClipSubtractPolygon(clipVertexCount, subtractWinding[p], plane, a, positiveCount, location, subtractWinding[p ^ 1]);
		if (clipVertexCount == 0) return (false);
		
		p1 = p2;
	}
	
	const SubtractVertex *clipWinding = subtractWinding[positiveCount & 1];
	
	int32 totalVertexCount = 0;
	int32 totalTriangleCount = 0;
	
	for (machine beginIndex = 0; beginIndex < clipVertexCount; beginIndex++)
	{
		if (clipWinding[beginIndex].direction > 0)
		{
			const SubtractVertex *v = &clipWinding[beginIndex];
			
			result[0] = v->position;
			int32 loopVertexCount = 1;
			
			int32 endIndex = v->planeIndex;
			for (machine clipIndex = beginIndex;;)
			{
				if (++clipIndex == clipVertexCount) clipIndex = 0;
				
				v = &clipWinding[clipIndex];
				result[loopVertexCount++] = v->position;
				
				if (v->clipFlag)
				{
					int32 positiveIndex = v->planeIndex;
					if (SquaredMag(positiveVertex[positiveIndex] - v->position) < kWeldEpsilonSquared) loopVertexCount--;
					
					do
					{
						result[loopVertexCount++] = positiveVertex[positiveIndex];
						if (++positiveIndex == positiveCount) positiveIndex = 0;
					} while (positiveIndex != endIndex);
					
					if (SquaredMag(result[loopVertexCount - 1] - result[0]) < kWeldEpsilonSquared) loopVertexCount--;
					break;
				}
			}
			
			int32 loopTriangleCount = TriangulatePolygon(loopVertexCount, result, normal, triangle);
			if (totalVertexCount != 0)
			{
				Triangle *t = triangle;
				for (machine a = 0; a < loopTriangleCount; a++)
				{
					t->index[0] += totalVertexCount;
					t->index[1] += totalVertexCount;
					t->index[2] += totalVertexCount;
					t++;
				}
			}
			
			totalVertexCount += loopVertexCount;
			result += loopVertexCount;
			
			totalTriangleCount += loopTriangleCount;
			triangle += loopTriangleCount;
		}
	}
	
	*resultCount = totalVertexCount;
	*triangleCount = totalTriangleCount;
	return (true);
}

// ZYURVUR
