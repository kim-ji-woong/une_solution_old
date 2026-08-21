#include "StdAfx.h"
#include "Properties.h"

#include <string>

using namespace std;



namespace UNE
{
	

	Properties::Properties(void)
	{
		InitValues();
	}


	Properties::~Properties(void)
	{	
		PropertyMap::iterator iter = m_pairs.begin();
		for( ; iter != m_pairs.end() ; iter++)
		{
			Property * p = iter->second;
			if( p != NULL)
				delete p;
		}
		m_pairs.clear();
	}

	//////////////////////////////////////////////////////////////////////////
	// 주의 : 모든 정보는 char array -> string 으로 저장할것 . 
	//////////////////////////////////////////////////////////////////////////
	void Properties::InitValues()
	{

		// aes key 여기
		char szKeyName [] = { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' ,'\0'};
		char szKeyValue [] = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6','\0' };
		
		SetValue(std::string(szKeyName), std::string(szKeyValue));
		
		// db 사용자 , 암호 여기
		char szDBUserKey [] = { 'S','O','P','D','B','U','S','E','R','\0' };
		char szDBUserName [] = { 's','a','\0'};

		SetValue(std::string(szDBUserKey), std::string(szDBUserName));

		char szDBPassKey [] = { 'S','O','P','D','B','P','A','S','S','\0' };
		char szDBPassValue [] = { '9','4','4','9','9','6','6','A','b','\0' };

		SetValue(std::string(szDBPassKey), std::string(szDBPassValue));
		
		// 추가 정보 여기에 


	}

	void Properties::SetValue( std::string key, std::string val )
	{		
		if( m_pairs.size() > 0)
		{
			PropertyMap::iterator iter = m_pairs.find(key);		
			if( iter != m_pairs.end())
			{
				Property * p = iter->second;
				p->SetValue(val);
				// change vlaue;
				return;
			}
		}
		
		// Add Value
		Property * prop = new Property();
		prop->SetName(key);
		prop->SetValue(val);
		m_pairs.insert(make_pair(key, prop));
	}

	void Properties::SetValue( std::string key, int val )
	{
		if( m_pairs.size() > 0)
		{
			PropertyMap::iterator iter = m_pairs.find(key);		
			if( iter != m_pairs.end())
			{
				Property * p = iter->second;
				p->SetValue(val);
				// change vlaue;
				return;
			}
		}

		// Add Value
		Property * prop = new Property();
		prop->SetName(key);
		prop->SetValue(val);
		m_pairs.insert(make_pair(key, prop));
	}

	void Properties::SetValue( std::string key, float val )
	{
		if( m_pairs.size() > 0)
		{
			PropertyMap::iterator iter = m_pairs.find(key);		
			if( iter != m_pairs.end())
			{
				Property * p = iter->second;
				p->SetValue(val);
				// change vlaue;
				return;
			}
		}

		// Add Value
		Property * prop = new Property();
		prop->SetName(key);
		prop->SetValue(val);
		m_pairs.insert(make_pair(key, prop));
	}

	void Properties::SetValue( std::string key, double val )
	{
		if( m_pairs.size() > 0)
		{
			PropertyMap::iterator iter = m_pairs.find(key);		
			if( iter != m_pairs.end())
			{
				Property * p = iter->second;
				p->SetValue(val);
				// change vlaue;
				return;
			}
		}

		// Add Value
		Property * prop = new Property();
		prop->SetName(key);
		prop->SetValue(val);
		m_pairs.insert(make_pair(key, prop));
	}

	bool Properties::GetValue( std::string key, std::string& val )
	{
		if( m_pairs.size() > 0)
		{
			PropertyMap::iterator iter = m_pairs.find(key);		
			if( iter != m_pairs.end())
			{
				Property * p = iter->second;
				if( p != NULL)
					return p->GetValue(val);
			}
		}
		return false;
	}

	bool Properties::GetValue( std::string key, int& val )
	{
		if( m_pairs.size() > 0)
		{
			PropertyMap::iterator iter = m_pairs.find(key);		
			if( iter != m_pairs.end())
			{
				Property * p = iter->second;
				if( p != NULL)
					return p->GetValue(val);
			}
		}
		return false;
	}

	bool Properties::GetValue( std::string key, float& val )
	{
		if( m_pairs.size() > 0)
		{
			PropertyMap::iterator iter = m_pairs.find(key);		
			if( iter != m_pairs.end())
			{
				Property * p = iter->second;
				if( p != NULL)
					return p->GetValue(val);
			}
		}
		return false;
	}

	bool Properties::GetValue( std::string key, double& val )
	{
		if( m_pairs.size() > 0)
		{
			PropertyMap::iterator iter = m_pairs.find(key);		
			if( iter != m_pairs.end())
			{
				Property * p = iter->second;
				if( p != NULL)
					return p->GetValue(val);
			}
		}
		return false;
	}

	void Properties::Clear()
	{
		unsigned int nSize = m_pairs.size();

		PropertyMap::iterator iter = m_pairs.begin();
		for( ; iter != m_pairs.end() ; iter++)
		{
			Property * p = iter->second;
			if( p != NULL)
				delete p;
		}

		m_pairs.clear();

		InitValues();
	}

	//////////////////////////////////////////////////////////////////////////

	Property::Property()
	{
		m_nType = 0;
		m_szName = "";
		m_dValue = 0.0;
		m_szValue = "";

		m_nValue = 0;
		m_fValue = 0.0f;
	}	

	Property::~Property()
	{
	}

	void Property::SetName( std::string szName )
	{
		m_szName = szName;
	}

	bool Property::GetValue( std::string& value )
	{
		if( m_nType != 1)
			return false;

		value = m_szValue;
		return true;
	}

	bool Property::GetValue( int& value )
	{
		if( m_nType != 2)
			return false;

		value = m_nValue;
		return true;
	}

	bool Property::GetValue( float& value )
	{
		if( m_nType != 3)
			return false;

		value = m_fValue;
		return true;
	}

	bool Property::GetValue( double& value )
	{
		if( m_nType != 4)
			return false;

		value = m_dValue;
		return true;
	}

	void Property::SetValue( std::string val )
	{
		m_nType = 1;
		m_szValue = val;
	}

	void Property::SetValue( int val )
	{
		m_nType = 2;
		m_nValue = val;
	}

	void Property::SetValue( float val )
	{
		m_nType = 3;
		m_fValue = val;
	}

	void Property::SetValue( double val )
	{
		m_nType = 4;
		m_dValue = val;
	}
};