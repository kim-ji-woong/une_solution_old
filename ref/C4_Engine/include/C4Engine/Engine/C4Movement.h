//=============================================================
//
// C4 Engine version 2.10 beta
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


#ifndef C4Movement_h
#define C4Movement_h


//# \component	Physics Manager
//# \prefix		PhysicsMgr/


#include "C4Controller.h"
#include "C4Connector.h"


namespace C4
{
	enum
	{
		kControllerMovement			= 'MOVE',
		kControllerRotation			= 'ROTA',
		kControllerSpin				= 'SPIN'
	};


	enum
	{
		kFunctionGetMovementSpeed	= 'GSPD',
		kFunctionSetMovementSpeed	= 'SSPD',
		kFunctionMoveToStart		= 'STAR',
		kFunctionMoveToFinish		= 'FINI'
	};


	enum
	{
		kFunctionGetRotationSpeed	= 'GSPD',
		kFunctionSetRotationSpeed	= 'SSPD',
		kFunctionRotateToStart		= 'STAR',
		kFunctionRotateToFinish		= 'FINI'
	};


	enum
	{
		kFunctionGetSpinSpeed		= 'GSPD',
		kFunctionSetSpinSpeed		= 'SSPD'
	};


	//# \class	MovementController		Manages a node that moves between two points.
	//
	//# The $MovementController$ class manages a node that moves between two points.
	//
	//# \def	class MovementController : public Controller, public Observable<MovementController, Type>
	//
	//# \ctor	MovementController();
	//
	//# \desc
	//# The $MovementController$ class can be assigned to a node in order to make it move along a line between
	//# two locations, a start position and a finish position.
	//
	//# \base	Controller		A $MovementController$ is a specific type of controller.
	//
	//# \also	$@RotationController@$
	//# \also	$@SpinController@$


	class MovementController : public Controller, public Observable<MovementController, Type>
	{
		private:

			enum
			{
				kMovementInitialized	= 1 << 0
			};

			unsigned_int32		movementState;
			float				currentDistance;

			float				targetSpeed;
			float				currentSpeed;
			float				currentAcceleration;

			float				minMovementSpeed;
			float				decelerationDistance;
			float				decelerationRate;

			float				movementSpeed;
			float				accelerationTime;
			float				decelerationTime;

			const Point3D		*startPosition;
			const Point3D		*finishPosition;
			float				movementDistance;
			float				inverseMovementDistance;
			Vector3D			originalNodeOffset;

			ConnectorKey		startConnectorKey;
			ConnectorKey		finishConnectorKey;

			MovementController(const MovementController& movementController);

			Controller *Replicate(void) const override;
 
			void CalculateMovementParameters(void);
			void CalculateForwardParameters(void); 
			void CalculateBackwardParameters(void); 
 
			void UpdateNodeDistance(float distance);
			void SetGeometryVelocity(const Vector3D& velocity) const; 

		public:

			enum 
			{
				kMovementEventReachedStart		= 'STAR',
				kMovementEventReachedFinish		= 'FINI'
			}; 

			enum
			{
				kMovementMessageState
			};

			C4API MovementController();
			C4API ~MovementController();

			float GetMovementSpeed(void) const
			{
				return (movementSpeed);
			}

			float GetAccelerationTime(void) const
			{
				return (accelerationTime);
			}

			void SetAccelerationTime(float time)
			{
				accelerationTime = time;
			}

			float GetDecelerationTime(void) const
			{
				return (decelerationTime);
			}

			void SetDecelerationTime(float time)
			{
				decelerationTime = time;
			}

			const ConnectorKey& GetStartConnectorKey(void) const
			{
				return (startConnectorKey);
			}

			void SetStartConnectorKey(const ConnectorKey& key)
			{
				startConnectorKey = key;
			}

			const ConnectorKey& GetFinishConnectorKey(void) const
			{
				return (finishConnectorKey);
			}

			void SetFinishConnectorKey(const ConnectorKey& key)
			{
				finishConnectorKey = key;
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
			void Sleep(void);
			void Move(void);
			void Activate(Node *trigger, Node *activator);

			C4API void SetMovementSpeed(float speed);
			C4API void MoveToStart(void);
			C4API void MoveToFinish(void);
	};


	class GetMovementSpeedFunction : public Function
	{
		private:

			GetMovementSpeedFunction(const GetMovementSpeedFunction& getMovementSpeedFunction);

			Function *Replicate(void) const override;

		public:

			GetMovementSpeedFunction();
			~GetMovementSpeedFunction();

			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};


	class SetMovementSpeedFunction : public Function
	{
		private:

			float		movementSpeed;

			SetMovementSpeedFunction(const SetMovementSpeedFunction& setMovementSpeedFunction);

			Function *Replicate(void) const override;

		public:

			SetMovementSpeedFunction();
			~SetMovementSpeedFunction();

			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);

			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);

			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};


	class MoveToStartFunction : public Function, public Observer<MoveToStartFunction, MovementController>
	{
		private:

			MoveToStartFunction(const MoveToStartFunction& moveToStartFunction);

			Function *Replicate(void) const override;

			void HandleEvent(MovementController *movementController, MovementController::ObservableEventType event);

		public:

			MoveToStartFunction();
			~MoveToStartFunction();

			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};


	class MoveToFinishFunction : public Function, public Observer<MoveToFinishFunction, MovementController>
	{
		private:

			MoveToFinishFunction(const MoveToFinishFunction& moveToFinishFunction);

			Function *Replicate(void) const override;

			void HandleEvent(MovementController *movementController, MovementController::ObservableEventType event);

		public:

			MoveToFinishFunction();
			~MoveToFinishFunction();

			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};


	class MovementStateMessage : public ControllerMessage
	{
		friend class MovementController;

		private:

			float		currentDistance;

			float		targetSpeed;
			float		currentSpeed;
			float		currentAcceleration;

			float		movementSpeed;
			float		accelerationTime;
			float		decelerationTime;

			MovementStateMessage(int32 controllerIndex);

		public:

			MovementStateMessage(int32 controllerIndex, float distance, float targSpeed, float currSpeed, float currAccel, float moveSpeed, float accelTime, float decelTime);
			~MovementStateMessage();

			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
	};


	//# \class	RotationController		Manages a node that rotates between two angles.
	//
	//# The $RotationController$ class manages a node that rotates between two angles.
	//
	//# \def	class RotationController : public Controller, public Observable<RotationController, Type>
	//
	//# \ctor	RotationController();
	//
	//# \desc
	//# The $RotationController$ class can be assigned to a node in order to make it rotate between its
	//# initial position and a given rotation angle.
	//
	//# \base	Controller		A $RotationController$ is a specific type of controller.
	//
	//# \also	$@SpinController@$
	//# \also	$@MovementController@$


	class RotationController : public Controller, public Observable<RotationController, Type>
	{
		private:

			enum
			{
				kRotationInitialized	= 1 << 0
			};

			unsigned_int32		rotationState;
			float				currentAngle;

			float				targetSpeed;
			float				currentSpeed;
			float				currentAcceleration;

			float				minRotationSpeed;
			float				decelerationAngle;
			float				decelerationRate;

			float				rotationAngle;
			float				rotationSpeed;
			float				accelerationTime;
			float				decelerationTime;

			const Transform4D	*centerTransform;
			Transform4D			originalNodeTransform;

			ConnectorKey		centerConnectorKey;

			RotationController(const RotationController& rotationController);

			Controller *Replicate(void) const override;

			void CalculateRotationParameters(void);
			void CalculateForwardParameters(void);
			void CalculateBackwardParameters(void);

			void UpdateNodeAngle(float angle);

		public:

			enum
			{
				kRotationEventReachedStart		= 'STAR',
				kRotationEventReachedFinish		= 'FINI'
			};

			enum
			{
				kRotationMessageState
			};

			C4API RotationController();
			C4API ~RotationController();

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

			void SetAccelerationTime(float time)
			{
				accelerationTime = time;
			}

			float GetDecelerationTime(void) const
			{
				return (decelerationTime);
			}

			void SetDecelerationTime(float time)
			{
				decelerationTime = time;
			}

			const ConnectorKey& GetCenterConnectorKey(void) const
			{
				return (centerConnectorKey);
			}

			void SetCenterConnectorKey(const ConnectorKey& key)
			{
				centerConnectorKey = key;
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
			void Sleep(void);
			void Move(void);
			void Activate(Node *trigger, Node *activator);

			C4API void SetRotationSpeed(float speed);
			C4API void RotateToStart(void);
			C4API void RotateToFinish(void);
	};


	class GetRotationSpeedFunction : public Function
	{
		private:

			GetRotationSpeedFunction(const GetRotationSpeedFunction& getRotationSpeedFunction);

			Function *Replicate(void) const override;

		public:

			GetRotationSpeedFunction();
			~GetRotationSpeedFunction();

			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};


	class SetRotationSpeedFunction : public Function
	{
		private:

			float		rotationSpeed;

			SetRotationSpeedFunction(const SetRotationSpeedFunction& setRotationSpeedFunction);

			Function *Replicate(void) const override;

		public:

			SetRotationSpeedFunction();
			~SetRotationSpeedFunction();

			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);

			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);

			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};


	class RotateToStartFunction : public Function, public Observer<RotateToStartFunction, RotationController>
	{
		private:

			RotateToStartFunction(const RotateToStartFunction& rotateToStartFunction);

			Function *Replicate(void) const override;

			void HandleEvent(RotationController *rotationController, RotationController::ObservableEventType event);

		public:

			RotateToStartFunction();
			~RotateToStartFunction();

			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};


	class RotateToFinishFunction : public Function, public Observer<RotateToFinishFunction, RotationController>
	{
		private:

			RotateToFinishFunction(const RotateToFinishFunction& rotateToFinishFunction);

			Function *Replicate(void) const override;

			void HandleEvent(RotationController *rotationController, RotationController::ObservableEventType event);

		public:

			RotateToFinishFunction();
			~RotateToFinishFunction();

			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};


	class RotationStateMessage : public ControllerMessage
	{
		friend class RotationController;

		private:

			float		currentAngle;

			float		targetSpeed;
			float		currentSpeed;
			float		currentAcceleration;

			float		rotationSpeed;
			float		accelerationTime;
			float		decelerationTime;

			RotationStateMessage(int32 controllerIndex);

		public:

			RotationStateMessage(int32 controllerIndex, float angle, float targSpeed, float currSpeed, float currAccel, float rotateSpeed, float accelTime, float decelTime);
			~RotationStateMessage();

			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
	};


	//# \class	SpinController		Manages a node that spins continuously.
	//
	//# The $SpinController$ class manages a node that spins continuously.
	//
	//# \def	class SpinController : public Controller
	//
	//# \ctor	SpinController();
	//
	//# \desc
	//# The $SpinController$ class can be assigned to a node in order to make it continuously rotate
	//# about a given center position and axis.
	//
	//# \base	Controller		A $SpinController$ is a specific type of controller.
	//
	//# \also	$@RotationController@$
	//# \also	$@MovementController@$


	class SpinController : public Controller
	{
		private:

			enum
			{
				kSpinInitialized	= 1 << 0
			};

			unsigned_int32		spinState;
			float				spinAngle;
			float				spinSpeed;

			float				currentSpeed;
			float				currentAcceleration;

			const Transform4D	*centerTransform;
			Transform4D			originalNodeTransform;

			ConnectorKey		centerConnectorKey;

			SpinController(const SpinController& spinController);

			Controller *Replicate(void) const override;

			void UpdateNodeAngle(float angle);

		public:

			enum
			{
				kSpinMessageState
			};

			C4API SpinController();
			C4API ~SpinController();

			float GetSpinAngle(void) const
			{
				return (spinAngle);
			}

			float GetSpinSpeed(void) const
			{
				return (spinSpeed);
			}

			const ConnectorKey& GetCenterConnectorKey(void) const
			{
				return (centerConnectorKey);
			}

			void SetCenterConnectorKey(const ConnectorKey& key)
			{
				centerConnectorKey = key;
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
			void Sleep(void);
			void Move(void);

			C4API void SetSpinSpeed(float speed, float time);
	};


	class GetSpinSpeedFunction : public Function
	{
		private:

			GetSpinSpeedFunction(const GetSpinSpeedFunction& getSpinSpeedFunction);

			Function *Replicate(void) const override;

		public:

			GetSpinSpeedFunction();
			~GetSpinSpeedFunction();

			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};


	class SetSpinSpeedFunction : public Function
	{
		private:

			float		spinSpeed;
			float		accelerationTime;

			SetSpinSpeedFunction(const SetSpinSpeedFunction& setSpinSpeedFunction);

			Function *Replicate(void) const override;

		public:

			SetSpinSpeedFunction();
			~SetSpinSpeedFunction();

			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);

			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);

			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};


	class SpinStateMessage : public ControllerMessage
	{
		friend class SpinController;

		private:

			float		spinAngle;
			float		spinSpeed;

			float		currentSpeed;
			float		currentAcceleration;

			SpinStateMessage(int32 controllerIndex);

		public:

			SpinStateMessage(int32 controllerIndex, float angle, float speed, float currSpeed, float currAccel);
			~SpinStateMessage();

			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
	};
}


#endif

// ZYURVUR
