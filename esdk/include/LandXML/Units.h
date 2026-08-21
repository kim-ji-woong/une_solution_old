#pragma once

namespace UnE
{
	namespace LX
	{
		class Units
		{
		public:
			enum AngularUnit { RADIAN = 0, GRADS, DEGREE, DEGREE_DD_MM_SS };
			enum LinearUnit { MILLI_METER = 0, CENTI_METER, METER, KILO_METER, INCH, FEET, YARD, MILE };

		public:
			Units(void);
			virtual ~Units(void);

		public:
			void SetDirectionUnit(AngularUnit unit);
			void SetAngularUnit(AngularUnit unit);
			void SetLinearUnit(LinearUnit unit);

			AngularUnit GetDirectionUnit() const;
			AngularUnit GetAngularUnit() const;
			LinearUnit GetLinearUnit() const;

			void SetAttrib(wchar_t* strAttrName, wchar_t* strAttrValue);

		private:
			AngularUnit m_directionUnit;
			AngularUnit m_angularUnit;
			LinearUnit m_linearUnit;
		};
	}
}
