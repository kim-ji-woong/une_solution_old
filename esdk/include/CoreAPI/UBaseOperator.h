#ifndef Manipulator_h__
#define Manipulator_h__

#pragma once

#include <vector>
#include "UObject.h"




namespace UnE
{
	namespace Core
	{
		class UBaseView;

		enum UOpType
		{
			eOp_None = 0,
			eOp_Mouse = 1,
			eOp_Keyboard = 2
		};
		

		struct CORE_API RaySceneQueryResultEntry
		{
			/// Distance along the ray
			float distance;
			/// The movable, or NULL if this is not a movable result
			UObject* pObject;
			/// Comparison operator for sorting
			bool operator < (const RaySceneQueryResultEntry& rhs) const
			{
				return this->distance < rhs.distance;
			}
		};
		typedef std::vector<RaySceneQueryResultEntry> RaySceneQueryResult;
		
		class CORE_API UBaseOperator
		{
			friend class UDB;
			friend class UBaseModel;
			friend class UBaseView;
		protected:
			UBaseView * m_pTargetView;
			UBaseView * SetTargetView() const { return m_pTargetView; }
			void SetTargetView(UBaseView * val) { m_pTargetView = val; }
			UBaseOperator(void);

			virtual void Reset();
		public:			
			virtual ~UBaseOperator(void);
			virtual UOpType GetType();


		};
	}
}

#endif // Manipulator_h__
