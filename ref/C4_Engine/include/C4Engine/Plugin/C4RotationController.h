//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This file is part of the C4 Engine and is provided under the
// terms of the license agreement entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#ifndef C4RotationController_h
#define C4RotationController_h


//# \component	Extras Plugin
//# \prefix		ExtrasPlugin/


#include "C4ExtrasBase.h"
#include "C4Controller.h"


namespace C4
{
	enum
	{
		kControllerRotation			= 'rotr'
	};
	
	
	enum
	{
		kFunctionChangeRotation		= 'crot',
		kFunctionSetRotationState	= 'stat'
	};
	
	
	enum
	{
		kRotationInitialized		= 1 << 0,
		kRotationDisabled			= 1 << 1,
		kRotationReverse			= 1 << 2,
		kRotationRestricted			= 1 << 3
	};
	
	
	//# \class	RotationController		Manages a node that rotates.
	//
	//# The $RotationController$ class manages a node that rotates.
	//
	//# \def	class RotationController : public Controller
	//
	//# \ctor	RotationController();
	//
	//# \desc
	//# 
	//
	//# \base	Controller/Controller		A $RotationController$ is a specific type of controller.
	
	
	class C4EXTRASAPI RotationController : public Controller
	{
		private:
			
			unsigned_int32		rotationFlags;
			
			float				rotationAngle;
			float				rotationSpeed;
			float				targetSpeed;
			float				acceleration;
			
			float				startAngle;
			float				finishAngle;
			
			const Point3D		*centerPosition;
			Vector3D			rotationAxis;
			
			Transform4D			originalTransform;
			
			RotationController(const RotationController& rotationController);
			
			Controller *Replicate(void) const override;
			
			void UpdateRotationAngle(float angle);
		
		public:
			
			enum
			{
				kRotationMessageUpdate,
				kRotationMessageState
			};
			
			RotationController();
			~RotationController();
			
			float GetRotationAngle(void) const
			{
				return (rotationAngle);
			}
			
			float GetRotationSpeed(void) const
			{
				return (rotationSpeed);
			}
			
			static void RegisterFunctions(ControllerRegistration *registration);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			 
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const; 
			void SetSetting(const Setting *setting); 
			 
			ControllerMessage *ConstructMessage(ControllerMessageType type) const;
			void ReceiveMessage(const ControllerMessage *message); 
			void SendInitialStateMessages(Player *player) const;
			
			void Preprocess(void);
			void Move(void); 
			
			void Activate(Node *trigger, Node *activator);
	};
	 
	
	class C4EXTRASAPI ChangeRotationFunction : public Function
	{
		private:
			
			float		rotationSpeed;
			float		accelerationTime;
			
			ChangeRotationFunction(const ChangeRotationFunction& changeRotationFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			ChangeRotationFunction();
			~ChangeRotationFunction();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class C4EXTRASAPI SetRotationStateFunction : public Function
	{
		private:
			
			unsigned_int32	rotationFlags;
			
			SetRotationStateFunction(const SetRotationStateFunction& setRotationStateFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			SetRotationStateFunction();
			~SetRotationStateFunction();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class C4EXTRASAPI UpdateRotationMessage : public ControllerMessage
	{
		friend class RotationController;
		
		private:
			
			float		rotationAngle;
			float		rotationSpeed;
			float		accelerationTime;
			
			UpdateRotationMessage(int32 controllerIndex);
		
		public:
			
			UpdateRotationMessage(int32 controllerIndex, float angle, float speed, float time);
			~UpdateRotationMessage();
			
			float GetRotationAngle(void) const
			{
				return (rotationAngle);
			}
			
			float GetRotationSpeed(void) const
			{
				return (rotationSpeed);
			}
			
			float GetAccelerationTime(void) const
			{
				return (accelerationTime);
			}
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
	};
	
	
	class C4EXTRASAPI RotationStateMessage : public ControllerMessage
	{
		friend class RotationController;
		
		private:
			
			float			rotationAngle;
			float			rotationSpeed;
			unsigned_int32	rotationFlags;
			
			RotationStateMessage(int32 controllerIndex);
		
		public:
			
			RotationStateMessage(int32 controllerIndex, float angle, float speed, unsigned_int32 flags);
			~RotationStateMessage();
			
			float GetRotationAngle(void) const
			{
				return (rotationAngle);
			}
			
			float GetRotationSpeed(void) const
			{
				return (rotationSpeed);
			}
			
			unsigned_int32 GetRotationFlags(void) const
			{
				return (rotationFlags);
			}
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
	};
}


#endif

// ZYURVUR
