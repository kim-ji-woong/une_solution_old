template <class Vertex, class Real>
EArc<Vertex, Real>::EArc()
{
	m_dBeginAngle = 0.0f;
	m_dAngle = 0.0f;
	m_isClockWise = false;
	m_isClosed = false;

	m_vTL = dnonlynew Vertex();
	m_vBL = dnonlynew Vertex();
	m_vBR = dnonlynew Vertex();
	m_vCenter = dnonlynew Vertex();

	m_dA = m_dB = 0.0f;
}

template <class Vertex, class Real>
double EArc<Vertex, Real>::GetEndAngle() CONST
{
	if (m_isClosed) return m_dBeginAngle;

	double dEndAngle = m_isClockWise ? m_dBeginAngle - m_dAngle : m_dBeginAngle + m_dAngle;
			
	if (dEndAngle < 0.0) dEndAngle += 6.28318530717958647692;
	else if (dEndAngle > 6.28318530717958647692) dEndAngle -= 6.28318530717958647692;

	return dEndAngle;
}

template <class Vertex, class Real>
bool EArc<Vertex, Real>::CheckValidAngle(double dAngle) CONST
{
	if (!m_isClosed)
	{
		double dEndAngle = ValidAngle(GetEndAngle());
		double dBeginAngle = ValidAngle(GetBeginAngle());

		if (m_isClockWise)
		{
			if (dBeginAngle > dEndAngle)
			{
				if (dAngle < dEndAngle - Math::HALF_TOLERANCE() || dAngle > dBeginAngle + Math::HALF_TOLERANCE())
					return false;
			}
			else
			{
				if (dAngle > dBeginAngle + Math::HALF_TOLERANCE() && dAngle < dEndAngle - Math::HALF_TOLERANCE())
					return false;
			}
		}
		else
		{
			if (dBeginAngle > dEndAngle)
			{
				if (dAngle > dEndAngle + Math::HALF_TOLERANCE() && dAngle < dBeginAngle - Math::HALF_TOLERANCE())
					return false;
			}
			else
			{
				if (dAngle < dBeginAngle - Math::HALF_TOLERANCE() || dAngle > dEndAngle + Math::HALF_TOLERANCE())
					return false;
			}
		}
	}

	return true;
}

// Return값 : radian
template <class Vertex, class Real>
double EArc<Vertex, Real>::GetVertexAngle(REF_CONST(Vertex) rVertex) CONST
{
	INSTANCE(Vertex) vTR = m_vTL + m_vBR - m_vBL;
	INSTANCE(Vertex) vR  = (vTR + m_vBR) / 2;

	double dAngle = Math::GetAngle(vR, m_vCenter, rVertex);

	Real dLen1 = (Real)OF(m_vTL, GetDistance(rVertex));
	Real dLen2 = (Real)OF(m_vBL, GetDistance(rVertex));

	if (dLen2 < dLen1)
		dAngle = Math::_2PI() - dAngle;

	return dAngle;
}

// rEArc를 타원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
template <class Vertex, class Real>
void EArc<Vertex, Real>::_Offset(REF(EArc) rEArc, bool outside, Real dLen) CONST
{
	if (!outside) dLen = -dLen;

	INSTANCE(Vertex) vL = (m_vTL + m_vBL) / 2;
	INSTANCE(Vertex) vB = (m_vBL + m_vBR) / 2;

	vL = Math::GetLinearVertex(vL, m_vCenter, -dLen);
	vB = Math::GetLinearVertex(vB, m_vCenter, -dLen);

	OF(rEArc, m_vBL) = vL + vB - m_vCenter;
	OF(rEArc, m_vBR) = vB * 2 - OF(rEArc, m_vBL);
	OF(rEArc, m_vTL) = vL * 2 - OF(rEArc, m_vBL);
	OF(OF(rEArc, m_vCenter), CopyFrom(m_vCenter));

	OF(rEArc, m_dBeginAngle) = m_dBeginAngle;
	OF(rEArc, m_dAngle)      = m_dAngle;
	OF(rEArc, m_isClockWise) = IsClockWise();
	OF(rEArc, m_isClosed)    = IsClosed();
	OF(rEArc, m_dA)			 = m_dA + dLen;
	OF(rEArc, m_dB)			 = m_dB + dLen;
}

template <class Vertex, class Real>
double EArc<Vertex, Real>::GetRealAngle(double dBeginAngle, double dEndAngle, bool isClockwise)
{
	double angle = 0.0;

	if (isClockwise)
	{
		if (dBeginAngle > dEndAngle)
			angle = dBeginAngle - dEndAngle;
		else
			angle = UnE::Geometry::Math::_2PI() - (dEndAngle - dBeginAngle);
	}
	else
	{
		if (dEndAngle > dBeginAngle)
			angle = dEndAngle - dBeginAngle;
		else
			angle = UnE::Geometry::Math::_2PI() - (dBeginAngle - dEndAngle);
	}

	return angle;
}

template <class Vertex, class Real>
double EArc<Vertex, Real>::ValidAngle(double angle)
{
	double _2pi = Math::_2PI();

	if (angle < 0.0f)
	{
		int nCount = (int)(angle / _2pi);
		angle = angle - (nCount - 1) * _2pi;

		if (angle >= (Real)_2pi)
			angle -= (Real)_2pi;
		/*int nCount = (int)(-angle / _2pi);
		angle += _2pi * (nCount + 1);*/
	}
	else if (angle > _2pi)
	{
		int nCount = (int)(angle / _2pi);
		angle -= _2pi * nCount;
	}

	return angle;
}
