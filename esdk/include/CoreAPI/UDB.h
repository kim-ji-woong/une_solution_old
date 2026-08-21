#ifndef __UNE_CORE_UDB_H_INCLUDED__
#define __UNE_CORE_UDB_H_INCLUDED__

#pragma once

#include <map>
#include "CoreAPI.h"
#include "UBaseOperator.h"

namespace UnE
{
	namespace Core
	{
		//-----------------------------------------------------------------------
		class UBaseManager;
		class UObjectManager;
		class USceneNodeManager;
		class UMaterialManager;
		class UAnimationManager;
		class UEventManager;
		class UBaseView;
		class UBaseModel;
		class UMouseOperator;
		//-----------------------------------------------------------------------

		
		// Shared Main Context
		class CORE_API UDB
		{
	
		public:
			UDB ();
			virtual ~UDB ();
						
			virtual void Init ();
			static UDB * GetUDB();  

			static void IncrementInstanceCount(); 
			static void DecrementInstanceCount();

			static void SetInstanceBase(std::string& szName);			
			static std::string GetInstanceBase();

			static void SetDriverType(std::string& szDriver);
			static std::string GetDriverType();

			static bool GetCleanInclude() { return m_bCleanInclude; }
			static void SetCleanInclude(bool value) { m_bCleanInclude = value; }
			
			static int GetInstanceCount() {return m_InstanceCount; }
			static void SetInstanceCount(int count) {m_InstanceCount = count;}

			static unsigned long GetNextCookie() { return next_cookie++; };
			

			static void ReportError( const char *message, int severity = 1, int category = 0, int specific = 0 );


			static UnE::Core::UObjectManager*	GetObjectManger(HWND hWnd);
			//UnE::Core::UObjectManager*  SetObjectManger(UnE::Core::UObjectManager* val);
			
			static UnE::Core::UAnimationManager* GetAnimationManager();
			static UnE::Core::UAnimationManager*  SetAnimationManager(UnE::Core::UAnimationManager* val);


			static UnE::Core::UBaseView * GetBaseView(int hWnd);
			static void					  AddBaseView(UBaseView* pView);
			static void					  RemoveBaseView(int hWnd, bool bDelete = false);
			static void					  RemoveAllBaseView(bool bDelete = false);

			static UnE::Core::UBaseOperator* GetOperator(int hWnd, UOpType nType);
			static void						 AddOperator(UBaseOperator * pOp);
			static void						 RemoveOperator(int hWnd, bool bDelete = false);		
			static void						 RemoveAllOperator(bool Delete = false);

			static UnE::Core::UBaseModel * GetBaseModel(int hWnd);

		private:

			static UDB *		m_pHDB;

			static int			m_InstanceCount;	//!< number of driver instances
			static std::string	m_pInstanceBase;	//!< base name for driver instances
			static std::string	m_pDriverType;		//!< base driver type for driver instances
			static bool			m_bCleanInclude;	//!< move global includes to model specific include segments
			

			//static UObjectManager*	 m_pObjectManger;
			static UAnimationManager* m_pAniManager;
			static unsigned long next_cookie;	//!< the next identifier to be used when GetNextCookie is called


			static std::map<int, UBaseView *> m_BaseViewList;
			static std::map<int, UnE::Core::UBaseOperator*> m_mouseOps;
			
			
		};
		
	}
}

#endif//__UNE_CORE_UDB_H_INCLUDED__



