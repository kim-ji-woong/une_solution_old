#include "stdafx.h"
#include "UObject.h"
#include "UEntity.h"
#include "UAnimation.h"

namespace UnE
{

	namespace Core
	{


		UAnimationState::UAnimationState(const std::string& aniName, UAnimationStateSet *parent, Real timePos, Real length, Real weight, bool enabled)
			: mAnimationName(aniName)
			, mParent(parent)
			, mTimePos(timePos)
			, mLength(length)
			, mWeight(weight)
			, mEnabled(enabled)
			, mLoop(true)
		{
			mTypeName = "UAnimationState";
			mObjName  = aniName;
		}
		
		UAnimationState::~UAnimationState()
		{

		}
		
		void UAnimationState::SetEnabled( bool bEnable )
		{
			
			mEnabled = bEnable;
			Ogre::AnimationState * pState = (Ogre::AnimationState*)pInternal;
			if( bEnable == true)
			{
				pState->setTimePosition(0);
			}
			pState->setEnabled(bEnable);
		}

		bool UAnimationState::GetEnabled()
		{
			return mEnabled;
		}

		void UAnimationState::AddTime( float tick )
		{		
			
			Ogre::AnimationState * pState = (Ogre::AnimationState*)pInternal;
			pState->addTime(tick);
		}

		void UAnimationState::SetLoop( bool bLoop )
		{
			mLoop = bLoop;
			Ogre::AnimationState * pState = (Ogre::AnimationState*)pInternal;
			pState->setLoop(bLoop);
		}

		bool UAnimationState::GetLoop()
		{
			return mLoop;
		}

		float UAnimationState::GetLength()
		{
			return mLength;
		}

		void UAnimationState::SetLength( float fLength )
		{
			mLength = fLength;
			Ogre::AnimationState * pState = (Ogre::AnimationState*)pInternal;
			pState->setLength(fLength);
		}

		std::string& UAnimationState::GetAnimationName()
		{
			return mAnimationName;
		}



		//////////////////////////////////////////////////////////////////////////
		// UAnimationStateSet Implementation

		UAnimationStateSet::UAnimationStateSet()
		{
			mParent = NULL;
		}

		UAnimationStateSet::UAnimationStateSet( const UAnimationStateSet& rhs )
			: UCoreObject(rhs)
		{
			removeAllAnimationStates();
			mAnimationStates = rhs.mAnimationStates;
			mParent = rhs.mParent;		
		}

		UAnimationStateSet::~UAnimationStateSet()
		{
			removeAllAnimationStates();
		}

		UAnimationState* UAnimationStateSet::createAnimationState( const std::string& name, Real timePos, Real length, Real weight /*= 1.0*/, bool enabled /*= false*/ )
		{
			UAnimationStateMap::iterator iter = mAnimationStates.find(name);
			if( iter != mAnimationStates.end() )
				return iter->second;

			UAnimationState * pState = new UAnimationState(name, this, timePos, length, weight, enabled);			
			mAnimationStates.insert(std::make_pair(name, pState));
			return pState;
		}

		UAnimationState* UAnimationStateSet::getAnimationState( const std::string& name ) const
		{
			UAnimationStateMap::const_iterator iter = mAnimationStates.find(name);
			if( iter != mAnimationStates.end() )
				return iter->second;
			return NULL;
		}

		bool UAnimationStateSet::hasAnimationState( const std::string& name ) const
		{
			UAnimationStateMap::const_iterator iter = mAnimationStates.find(name);
			if( iter != mAnimationStates.end() )
				return true;
			return false;
		}

		void UAnimationStateSet::removeAnimationState( const std::string& name )
		{
			UAnimationStateMap::const_iterator iter = mAnimationStates.find(name);
			if( iter != mAnimationStates.end() )
			{
				mAnimationStates.erase(iter);
			}
		}

		void UAnimationStateSet::removeAllAnimationStates( void )
		{
			UAnimationStateMap::iterator iter = mAnimationStates.begin();
			UAnimationStateMap::iterator iter2 = mAnimationStates.end();
			for( iter ; iter != iter2; iter++)
			{
				UAnimationState * pState = iter->second;
				delete pState;
			}
			mAnimationStates.clear();
		}

		UnE::Core::UAnimationStateIterator UAnimationStateSet::Begin()
		{
			return mAnimationStates.begin();
		}

		UnE::Core::UAnimationStateIterator UAnimationStateSet::End()
		{
			return mAnimationStates.end();
		}

		UEntity* UAnimationStateSet::GetParent() const
		{
			return mParent;
		}

		void UAnimationStateSet::SetParent( UEntity* val )
		{
			mParent = val;
		}

		//////////////////////////////////////////////////////////////////////////
		// UAnimationManager Implementaion

		UAnimationManager::UAnimationManager()
		{
			bAnimate = false;
		}		

		UAnimationManager::~UAnimationManager()
		{
			bAnimate = false;
			mAnimations.clear();
		}

		void UAnimationManager::AddAnimationState( UAnimationState* pState )
		{
			if( pState == NULL)
				return;
			mAnimations.insert(pState);
		}

		void UAnimationManager::RemoveAnimationState( UAnimationState* pState )
		{
			if( pState == NULL)
				return;
			pState->SetEnabled(false);
			mAnimations.erase(pState);
		}

		void UAnimationManager::RemoveAnimationState( UAnimationStateSet* pStateSet )
		{
			if( pStateSet == NULL)
				return;

			UAnimationStateMap::const_iterator iter1 = pStateSet->Begin();
			UAnimationStateMap::const_iterator iter2 = pStateSet->End();
			for( iter1; iter1 != iter2 ; iter1++)
			{
				UAnimationState* pState = iter1->second;
				if( pState != NULL)
				{
					pState->SetEnabled(false);
					mAnimations.erase(pState);
				}
			}
		}

		void UAnimationManager::RemoveAnimationState( UEntity* pEntity )
		{
			if( pEntity == NULL)
				return;
			RemoveAnimationState(pEntity->GetAllAnimationState());
		}

		void UAnimationManager::ClearAllAnimation()
		{
			mAnimations.clear();
		}

		void UAnimationManager::Animate( float time )
		{
			if( bAnimate == true)
			{
				std::set<UAnimationState*>::const_iterator itor = mAnimations.begin();
				std::set<UAnimationState*>::const_iterator end  = mAnimations.end();

				while( itor != end )
				{
					(*itor)->AddTime( time );
					++itor;
				}
			}			
		}


		void UAnimationManager::SetEnabled( bool bEnable )
		{
			bAnimate = bEnable;
		}

		bool UAnimationManager::GetEnabled()
		{
			return bAnimate;
		}

		

	}

}