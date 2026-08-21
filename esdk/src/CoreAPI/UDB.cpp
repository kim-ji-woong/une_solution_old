#include "StdAfx.h"

#include "UDB.h"
#include "UObject.h"
#include "UAnimation.h"


#include "UBaseDriver.h"
#include "UBaseOperator.h"
#include "UBaseView.h"
#include "UMouseOperator.h"


namespace UnE
{
	namespace Core
	{
		//-----------------------------------------------------------------------
		UDB *				UDB::m_pHDB = NULL;
		unsigned long		UDB::next_cookie = 0;
		std::string			UDB::m_pDriverType = "";
		std::string			UDB::m_pInstanceBase = "";

		int					UDB::m_InstanceCount = 0;
		bool				UDB::m_bCleanInclude = true;

		UAnimationManager*	UDB::m_pAniManager = NULL;
		//UObjectManager*		UDB::m_pObjectManger = NULL;


		std::map<int, UnE::Core::UBaseOperator*> UDB::m_mouseOps;
		std::map<int, UBaseView *> UDB::m_BaseViewList;

		//-----------------------------------------------------------------------
		UDB::UDB()
		{
			if(m_pHDB != NULL)
				return;

			m_pHDB = this;			
		}
		//-----------------------------------------------------------------------
		UDB::~UDB()
		{
			if(m_InstanceCount == 0)
			{
				//reset to initial state
				m_pHDB = 0;
				m_InstanceCount = 0;				
				m_bCleanInclude = true;
				// Reset system

				
				// all manager delete
				//if( m_pObjectManger != NULL)
				//{
				//	delete m_pObjectManger;
				//	m_pObjectManger = NULL;
				//}
			}
		}
		//-----------------------------------------------------------------------
		void UDB::Init()
		{
			// Set Color Map
			// Set int DB , Segment
		}
		//-----------------------------------------------------------------------
		UDB * UDB::GetUDB()
		{
			 return m_pHDB;
		}
		//-----------------------------------------------------------------------
		void UDB::ReportError( const char *message, int severity /*= 1*/, int category /*= 0*/, int specific /*= 0 */ )
		{

		}
		//-----------------------------------------------------------------------
		void UDB::IncrementInstanceCount()
		{
			m_InstanceCount++;
		}
		//-----------------------------------------------------------------------
		void UDB::DecrementInstanceCount()
		{
			m_InstanceCount--;
		}
		//-----------------------------------------------------------------------
		void UDB::SetInstanceBase( std::string& szName )
		{
			m_pInstanceBase = szName;
		}
		//-----------------------------------------------------------------------
		std::string UDB::GetInstanceBase()
		{
			return m_pInstanceBase;
		}
		//-----------------------------------------------------------------------
		void UDB::SetDriverType( std::string& szDriver )
		{
			m_pDriverType = szDriver;
		}
		//-----------------------------------------------------------------------
		std::string UDB::GetDriverType()
		{
			return m_pDriverType;
		}
		//-----------------------------------------------------------------------
		
		UnE::Core::UObjectManager* UDB::GetObjectManger( HWND hWnd )
		{
			WndCtx * pCtx = GetWndContext(hWnd);
			if( pCtx == NULL)
				return NULL;

			return pCtx->objectManager;
		}
		//-----------------------------------------------------------------------
		UnE::Core::UAnimationManager* UDB::GetAnimationManager()
		{
			if(m_pAniManager == NULL)
				m_pAniManager = new UAnimationManager();

			return m_pAniManager;
		}
		//-----------------------------------------------------------------------
		UnE::Core::UAnimationManager* UDB::SetAnimationManager( UnE::Core::UAnimationManager* val )
		{
			UAnimationManager * pOld = m_pAniManager;
			m_pAniManager = val;
			return pOld;
		}
		//-----------------------------------------------------------------------
		UnE::Core::UBaseView * UDB::GetBaseView( int hWnd )
		{
			if( hWnd <= 0)
				return NULL;

			if(m_BaseViewList.size() == 0)
				return NULL;

			if(m_BaseViewList.find(hWnd) == m_BaseViewList.end())
			{
				return NULL;
			}
			return m_BaseViewList[hWnd];
		}		
		//-----------------------------------------------------------------------
		void UDB::AddBaseView( UnE::Core::UBaseView* pView )
		{
			if( pView == NULL)
			{
				return;
			}


			int hWnd = (int)pView->GetHWnd();


			UBaseView * view = UDB::GetBaseView((int)hWnd);
			if( view == NULL)
			{
				
				m_BaseViewList.insert(std::make_pair((int)hWnd, pView));
			}
		}
		//-----------------------------------------------------------------------
		void UDB::RemoveBaseView( int hWnd, bool bDelete /*= false*/ )
		{
			UBaseView * view = UDB::GetBaseView((int)hWnd);
			if( view == NULL)
			{
				return;
			}
			m_BaseViewList.erase(hWnd);

			if( bDelete == true)
				delete view;

		}
		//-----------------------------------------------------------------------
		void UDB::RemoveAllBaseView( bool bDelete /*= false*/ )
		{
			if( bDelete == true)
			{
				std::map<int, UnE::Core::UBaseView*>::iterator iter;
				for( iter = m_BaseViewList.begin(); iter != m_BaseViewList.end(); iter++)
				{				
					delete iter->second;
				}
			}
			m_BaseViewList.clear();
		}
		//-----------------------------------------------------------------------
		UnE::Core::UBaseOperator* UDB::GetOperator( int hWnd, UOpType nType )
		{
			if( hWnd <= 0)
				return NULL;

			if(m_mouseOps.size() == 0)
				return NULL;

			if(m_mouseOps.find(hWnd) == m_mouseOps.end())
			{
				return NULL;
			}
			UBaseOperator * pOp = m_mouseOps[hWnd];

			if( pOp != NULL && pOp->GetType() == nType)
			{
				return pOp;
			}
			return NULL;
		}
		//-----------------------------------------------------------------------
		void UDB::AddOperator( UnE::Core::UBaseOperator * pOp )
		{
			if( pOp == NULL)
				return;
			if( pOp->m_pTargetView == NULL)
				return;
				
			int hWnd = (int)(pOp->m_pTargetView->GetHWnd());

			UBaseOperator * view = UDB::GetOperator((int)hWnd, pOp->GetType());
			if( view == NULL)
			{
				m_mouseOps.insert(std::make_pair(hWnd, pOp));
			}
		}
		//-----------------------------------------------------------------------
		void UDB::RemoveOperator( int hWnd, bool bDelete /*= false*/ )
		{
			if(m_mouseOps.size() == 0)
				return;

			if(m_mouseOps.find(hWnd) == m_mouseOps.end())
			{
				return;
			}
			UBaseOperator * pOp = m_mouseOps[hWnd];
			if( pOp == NULL)
				return;

			m_mouseOps.erase(hWnd);

			if( bDelete == true)
				delete pOp;
		}
		//-----------------------------------------------------------------------
		void UDB::RemoveAllOperator( bool bDelete /*= false*/ )
		{	
			if( bDelete == true)
			{
				std::map<int, UnE::Core::UBaseOperator*>::iterator iter;
				for( iter = m_mouseOps.begin(); iter != m_mouseOps.end(); iter++)
				{				
					delete iter->second;
				}
			}
			m_mouseOps.clear();
		}

		UnE::Core::UBaseModel * UDB::GetBaseModel( int hWnd )
		{
			UBaseView * pView = GetBaseView(hWnd);
			if( pView == NULL)
				return NULL;

			return pView->GetBaseModel();
		}

		//-----------------------------------------------------------------------
		

		//-----------------------------------------------------------------------
	}
		
}

