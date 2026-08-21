#ifndef __UNE_CORE_ANIMATION_H_INCLUDED__
#define __UNE_CORE_ANIMATION_H_INCLUDED__

#pragma once

#include "CoreAPI.h"
#include "UObject.h"
#include <vector>
#include <list>
#include <set>

namespace UnE
{
	namespace Core
	{
		class UEntity;
		class UAnimationStateSet;
		class CORE_API UAnimationState : public UCoreObject
		{
		protected:
			UAnimationStateSet* mParent;
			std::string mAnimationName;
			float		mLength;
			bool		mEnabled;
			bool		mLoop;

			float		mTimePos;
			float		mWeight;
			
		public:
			UAnimationState(const std::string& aniName, UAnimationStateSet *parent, Real timePos, Real length, Real weight = 1.0f, bool enabled = false);
			~UAnimationState();

			void		SetEnabled(bool bEnable);
			bool		GetEnabled();

			void		AddTime(float tick);


			std::string& GetAnimationName();

			float		GetLength();
			void		SetLength(float fLength);

			void		SetLoop(bool bLoop);
			bool		GetLoop();

			float		GetWeight() { return mWeight; }
			void		SetWeight(float val) { mWeight = val; }

		};

		typedef std::map<std::string, UAnimationState*> UAnimationStateMap;
		typedef UAnimationStateMap::iterator UAnimationStateIterator;
		typedef UAnimationStateMap::const_iterator ConstUAnimationStateIterator;
		// A list of enabled animation states
		typedef std::list<UAnimationState*> EnabledUAnimationStateList;
		typedef EnabledUAnimationStateList::const_iterator ConstEnabledUAnimationStateIterator;


		class CORE_API UAnimationStateSet : public UCoreObject
		{

		public:
			UAnimationStateSet();
			UAnimationStateSet(const UAnimationStateSet& rhs);

			~UAnimationStateSet();

			UAnimationState* createAnimationState(const std::string& animName,  
				float timePos, float length, float weight = 1.0, bool enabled = false);

			UAnimationState* getAnimationState(const std::string& name) const;
			bool hasAnimationState(const std::string& name) const;
			void removeAnimationState(const std::string& name);
			void removeAllAnimationStates(void);
			bool hasEnabledAnimationState(void) const { return !mEnabledAnimationStates.empty(); }
			
			UAnimationStateIterator Begin();
			UAnimationStateIterator	End();

			UEntity* GetParent() const;
			void SetParent(UEntity* val);

		protected:
			UAnimationStateMap mAnimationStates;
			EnabledUAnimationStateList mEnabledAnimationStates;

			UEntity* mParent;
			
		};


		class CORE_API UAnimationManager
		{
			std::set<UAnimationState*> mAnimations;
			bool bAnimate;
		public:
			UAnimationManager();
			virtual ~UAnimationManager();
			void SetEnabled(bool bEnable);
			bool GetEnabled();
			void AddAnimationState(UAnimationState* pState);
			
			void RemoveAnimationState(UAnimationState* pState);

			void RemoveAnimationState(UAnimationStateSet* pStateSet);

			void RemoveAnimationState(UEntity* pEntity);

			void ClearAllAnimation();

			void Animate(float time);
		};
	}
}


#endif//__UNE_CORE_ANIMATION_H_INCLUDED__