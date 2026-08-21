#ifndef __UNE_CORE_UOJBECT_H_INCLUDED__
#define __UNE_CORE_UOJBECT_H_INCLUDED__

#pragma once

#include "CoreAPI.h"

#include <map>

namespace Ogre
{
	class UDotSceneLoader;
}
namespace UnE
{
	namespace Core
	{
		//-------------------------------------------------------------------------

		class UDB;
		class UBaseView;
		class UBaseOperator;
		class UObjectManager;
		class UAssimpLoader;
		
		//-------------------------------------------------------------------------	
		// UObject : All the Entity classes of the parent class 
		//	
		class CORE_API UObject
		{			
			friend class UAssimpLoader;
			friend class UObjectManager;
			friend class UBaseView;
			friend class Ogre::UDotSceneLoader;

		public:
			UObject();
			virtual  ~UObject();;

			virtual std::string GetName();
			virtual std::string GetType();
			virtual std::string GetAlias();
			unsigned int GetID() const;
			void	SetInternal(void * val);
		protected:
			// Internel Use
			void*	GetInternal(){ return pInternal; }
			
			virtual void SetAlias(std::string& szName);
			virtual void SetName(std::string& szName);
			
			std::string szAlias;
			std::string mObjName;
			std::string mTypeName;
			// internal Use
			void*	pInternal;

			unsigned int mID;
			
		};

		//-------------------------------------------------------------------------		
		// UCoreObject : All the System Entity classes of the parent class 
		//
		class CORE_API UCoreObject : public UObject
		{
			friend class UBaseView;
			friend class UBaseOperator;
			friend class UObjectManager;
		protected:						

			UCoreObject();
			virtual ~UCoreObject();
		};
				
		//-------------------------------------------------------------------------		
		// Global Object Manager
		// 
		class CORE_API UObjectManager  
		{
		public:
			
			UObjectManager();

			virtual ~UObjectManager();
			
			// Add object to hashmap. If Object is NULL or Object name is Null string , return Fail
			// Return : success : object ptr, fail : NULL
			UObject * AddUObject(UObject *pObject);

			// Remove object in hashmap. If name is Null string, return fail
			// Return : success : object ptr, fail : NULL
			UObject * RemoveUObject(std::string szName);

			// Get object in hashmap. If name is Null string, return fail
			// Return : success : object ptr, fail : NULL
			UObject * GetUObject(std::string szName);	

			// Get object in hashmap. If name is Null string, return fail
			// Return : success : object ptr, fail : NULL
			UObject * GetUObjectByAlias(std::string szName);	

			void RemoveAll();

			void ClearAll();


		protected:
			// Object Hash Map
			std::map<std::string, UObject*>	m_ObjectHash;
			std::vector<UObject*>				m_objs;

			std::map<int, UObject*>	m_mapViewObj;
 
		};
		//-------------------------------------------------------------------------
	}
}


#endif//__UNE_CORE_UOJBECT_H_INCLUDED__