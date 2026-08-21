#include "StdAfx.h"
#include "Units.h"
#include <string>

BEGIN_NS(UnE)
BEGIN_NS(LX)

Units::Units(void)
{
	m_directionUnit = DEGREE;
	m_angularUnit = DEGREE;
	m_linearUnit = METER;
}

Units::~Units(void)
{
}

void Units::SetDirectionUnit(Units::AngularUnit unit)
{
	m_directionUnit = unit;
}

void Units::SetAngularUnit(Units::AngularUnit unit)
{
	m_angularUnit = unit;
}

void Units::SetLinearUnit(Units::LinearUnit unit)
{
	m_linearUnit = unit;
}

Units::AngularUnit Units::GetDirectionUnit() const
{
	return m_directionUnit;
}

Units::AngularUnit Units::GetAngularUnit() const
{
	return m_angularUnit;
}

Units::LinearUnit Units::GetLinearUnit() const
{
	return m_linearUnit;
}

void Units::SetAttrib(wchar_t* strAttrName, wchar_t* strAttrValue)
{
	if (!_wcsicmp(strAttrName, L"directionUnit"))
	{
		if (!_wcsicmp(strAttrValue, L"radians"))
			m_directionUnit = RADIAN;
		else if (!_wcsicmp(strAttrValue, L"grads"))
			m_directionUnit = GRADS;
		else if (!_wcsicmp(strAttrValue, L"decimal degrees"))
			m_directionUnit = DEGREE;
		else if (!_wcsicmp(strAttrValue, L"decimal dd.mm.ss"))
			m_directionUnit = DEGREE_DD_MM_SS;
	}
	else if (!_wcsicmp(strAttrName, L"angularUnit"))
	{
		if (!_wcsicmp(strAttrValue, L"radians"))
			m_angularUnit = RADIAN;
		else if (!_wcsicmp(strAttrValue, L"grads"))
			m_angularUnit = GRADS;
		else if (!_wcsicmp(strAttrValue, L"decimal degrees"))
			m_angularUnit = DEGREE;
		else if (!_wcsicmp(strAttrValue, L"decimal dd.mm.ss"))
			m_angularUnit = DEGREE_DD_MM_SS;
	}
	else if (!_wcsicmp(strAttrName, L"linearUnit"))
	{
		if (!_wcsicmp(strAttrValue, L"millimeter"))
			m_linearUnit = MILLI_METER;
		else if (!_wcsicmp(strAttrValue, L"centimeter"))
			m_linearUnit = CENTI_METER;
		else if (!_wcsicmp(strAttrValue, L"meter"))
			m_linearUnit = METER;
		else if (!_wcsicmp(strAttrValue, L"kilometer"))
			m_linearUnit = KILO_METER;
		else if (!_wcsicmp(strAttrValue, L"foot"))
			m_linearUnit = FEET;
		else if (!_wcsicmp(strAttrValue, L"inch"))
			m_linearUnit = INCH;
		else if (!_wcsicmp(strAttrValue, L"mile"))
			m_linearUnit = MILE;
	}
}

END_NS
END_NS
