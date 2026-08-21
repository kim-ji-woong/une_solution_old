//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#ifndef C4Mover_h
#define C4Mover_h


//# \component	Physics Manager
//# \prefix		PhysicsMgr/


#include "C4Physics.h"


namespace C4
{
	enum
	{
		kControllerMover			= 'MOVR'
	};
	
	
	enum
	{
		kFunctionMoveToBeginning	= 'MBEG',
		kFunctionMoveToEnding		= 'MEND',
		kFunctionStopMover			= 'STOP'
	};
	
	
	class MoverController : public RigidBodyController, public Observable<MoverController, Type>
	{
		private:
			
			enum
			{
				kMoverInitialized	= 1 << 0,
				kMoverActive		= 1 << 1,
				kMoverReverse		= 1 << 2
			};
			
			unsigned_int32		moverState;
			float				moverSpeed;
			
			ConnectorKey		beginningConnectorKey;
			ConnectorKey		endingConnectorKey;
			
			float				moverInverseMass;
			Vector3D			moverOffset;
			
			Point3D				beginningPosition;
			Point3D				endingPosition;
			Vector3D			movementDirection;
			
			MoverController(const MoverController& moverController);
			
			Controller *Replicate(void) const override;
		
		public:
			
			enum
			{
				kMoverEventReachedBeginning		= 'BEGN',
				kMoverEventReachedEnding		= 'ENDG'
			};
			
			C4API MoverController();
			C4API ~MoverController();
			
			float GetMoverSpeed(void) const
			{
				return (moverSpeed);
			}
			
			void SetMoverSpeed(float speed)
			{
				moverSpeed = speed;
			}
			
			const ConnectorKey& GetBeginningConnectorKey(void) const
			{
				return (beginningConnectorKey);
			}
			
			void SetBeginningConnectorKey(const ConnectorKey& key)
			{
				beginningConnectorKey = key;
			}
			
			const ConnectorKey& GetEndingConnectorKey(void) const
			{
				return (endingConnectorKey);
			}
			
			void SetEndingConnectorKey(const ConnectorKey& key)
			{
				endingConnectorKey = key;
			}
			
			static void RegisterFunctions(ControllerRegistration *registration);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			 
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const; 
			void SetSetting(const Setting *setting); 
			 
			void Preprocess(void);
			void Move(void); 
			
			C4API void MoveToBeginning(void);
			C4API void MoveToEnding(void);
			C4API void Stop(void); 
	};
	
	
	class MoveToBeginningFunction : public Function, public Observer<MoveToBeginningFunction, MoverController> 
	{
		private:
			
			MoveToBeginningFunction(const MoveToBeginningFunction& moveToBeginningFunction);
			
			Function *Replicate(void) const override;
			
			void HandleEvent(MoverController *mover, MoverController::ObservableEventType event);
		
		public:
			
			MoveToBeginningFunction();
			~MoveToBeginningFunction();
			
			bool OverridesFunction(const Function *function) const;
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class MoveToEndingFunction : public Function, public Observer<MoveToEndingFunction, MoverController>
	{
		private:
			
			MoveToEndingFunction(const MoveToEndingFunction& moveToEndingFunction);
			
			Function *Replicate(void) const override;
			
			void HandleEvent(MoverController *mover, MoverController::ObservableEventType event);
		
		public:
			
			MoveToEndingFunction();
			~MoveToEndingFunction();
			
			bool OverridesFunction(const Function *function) const;
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class StopMoverFunction : public Function
	{
		private:
			
			StopMoverFunction(const StopMoverFunction& stopMoverFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			StopMoverFunction();
			~StopMoverFunction();
			
			bool OverridesFunction(const Function *function) const;
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
}


#endif

// ZYURVUR
