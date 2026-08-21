#pragma once

#ifdef LIBPROPERTIES_EXPORTS
#define LIBPROPERTIES_API __declspec(dllexport)
#else
#define LIBPROPERTIES_API __declspec(dllimport)
#endif

#include <map>

namespace UNE
{

	class Property
	{
	public:
		Property();
		virtual ~Property();

		void SetName(std::string szName);

		bool GetValue(std::string& value);
		bool GetValue(int& value);
		bool GetValue(float& value);
		bool GetValue(double& value);
				
		void SetValue(std::string val);
		void SetValue(int value);
		void SetValue(float value);
		void SetValue(double value);

	private:
		std::string m_szName;
		int m_nType;
		
		std::string m_szValue;

		int m_nValue;
		float m_fValue;
		double m_dValue;

	};

	class LIBPROPERTIES_API Properties
	{
	public:
		Properties(void);
		virtual ~Properties(void);

		void Clear();

		void SetValue(std::string key, std::string szValue);
		void SetValue(std::string key, int nValue);
		void SetValue(std::string key, float fValue);
		void SetValue(std::string key, double dValue);

		bool GetValue(std::string key, std::string& value);
		bool GetValue(std::string key, int& nValue);
		bool GetValue(std::string key, float& fValue);
		bool GetValue(std::string key, double& dValue);
	private:
		void InitValues();
	private:
		typedef std::map<std::string, Property* > PropertyMap;
		PropertyMap m_pairs;
		
	};

}


