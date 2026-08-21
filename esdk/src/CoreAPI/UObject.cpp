#include "stdafx.h"
#include <map>
#include "UDB.h"
#include "UObject.h"

namespace UnE
{

	namespace Core
	{

		//////////////////////////////////////////////////////////////////////////
		// UObject Implementation

		UObject::UObject()
		{
			pInternal = NULL;
			mObjName = "";
			mTypeName = "UObject";
			mID = UDB::GetNextCookie();
		}
		//-------------------------------------------------------------------------
		UObject::~UObject()
		{

		}
		//-------------------------------------------------------------------------
		void UObject::SetName( std::string& szName )
		{
			mObjName = szName;
		}
		//-------------------------------------------------------------------------
		std::string UObject::GetName()
		{
			return mObjName;
		}
		//-------------------------------------------------------------------------
		std::string UObject::GetType()
		{
			return mTypeName;
		}
		//-------------------------------------------------------------------------
		void UObject::SetInternal( void * val )
		{
			pInternal = val;
		}

		unsigned int UObject::GetID() const
		{
			return mID;
		}

		void UObject::SetAlias( std::string& szName )
		{
			szAlias = szName;
		}

		std::string UObject::GetAlias()
		{
			return szAlias;
		}



		//////////////////////////////////////////////////////////////////////////
		// UObject Implementation

		UCoreObject::UCoreObject()
		{
			mObjName = "";
			mTypeName = "UCoreObject";
		}
		//-------------------------------------------------------------------------	
		UCoreObject::~UCoreObject()
		{

		}
				
		//////////////////////////////////////////////////////////////////////////
		// UObjectManager Implementation

		UObjectManager::UObjectManager()
		{
			
		}
		//-------------------------------------------------------------------------
		UObjectManager::~UObjectManager()
		{
			RemoveAll();
		}
		//-------------------------------------------------------------------------
		UObject * UObjectManager::AddUObject( UObject *object )
		{
			if( object == NULL)
				return NULL;

			if( object->GetName() == "")
				return NULL;

			UObject * pRet = (UObject*)this->m_ObjectHash[object->GetName()];
			if( pRet != NULL)
				return pRet;

			this->m_ObjectHash.insert(std::make_pair(object->GetName(), object));
			m_objs.push_back(object);
			return object;
		}	

		//-------------------------------------------------------------------------
		UObject * UObjectManager::RemoveUObject( std::string szName )
		{
			if( szName == "")
				return NULL;

			UObject * pRet = (UObject*)this->m_ObjectHash[szName];
			if( pRet == NULL)
				return NULL;

			m_ObjectHash.erase(szName);
			
			std::vector<UObject*>::iterator iter = std::find(m_objs.begin(), m_objs.end(), pRet);
			if( iter != m_objs.end())
			{
				iter = m_objs.erase(iter);
			}
			
			return pRet;
		}
		//-------------------------------------------------------------------------
		UObject * UObjectManager::GetUObject( std::string szName )
		{
			if( szName == "")
				return NULL;
			for( int i = 0 ; i <m_objs.size(); i++)
			{
				if ( m_objs[i]->GetName() == szName)
					return m_objs[i];
			}
			UObject * pRet = (UObject*)this->m_ObjectHash[szName];
			
			return pRet;
		}
		//-------------------------------------------------------------------------
		UObject * UObjectManager::GetUObjectByAlias( std::string szName )
		{
			if( szName == "")
				return NULL;
			for( int i = 0 ; i <m_objs.size(); i++)
			{
				if ( m_objs[i]->GetAlias() == szName)
					return m_objs[i];
			}
			return NULL;
		}	

		void UObjectManager::RemoveAll()
		{
			m_ObjectHash.clear();
			m_objs.clear();
		}

		void UObjectManager::ClearAll()
		{
			for( int i = 0 ; i <m_objs.size(); i++)
			{
				delete m_objs[i];
			}
			RemoveAll();
		}

	
		//-------------------------------------------------------------------------
	}
}
