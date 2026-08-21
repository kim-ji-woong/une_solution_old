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


#ifndef SimpleBall_h
#define SimpleBall_h


#include "C4Application.h"
#include "C4Input.h"
#include "C4Interface.h"
#include "C4World.h"
#include "C4Particles.h"


// Every application/game module needs to declare a function called ConstructApplication()
// exactly as follows. (It must be declared extern "C", and it must include the tag C4MODULEEXPORT.)
// The engine looks for this function in the DLL and calls it to construct an instance of 
// the subclass of the Application class that the application/game module defines.

extern "C"
{
	C4::Application *ConstructApplication(void);
}


namespace C4
{
	// These are action types used to define action bindings in the
	// Input Manager. If the four-character code for an action is
	// 'abcd', then any input control (there can be more than one)
	// bound to %abcd triggers the associated action.
	
	enum
	{
		kActionForward			= 'frwd',
		kActionBackward			= 'bkwd',
		kActionLeft				= 'left',
		kActionRight			= 'rght',
		kActionUp				= 'jump',
		kActionDown				= 'down'
	};
	
	
	// Model types are associated with a model resource using the ModelRegistration
	// class. Models are registered with the engine in the Game constructor.
	
	enum
	{
		kModelBall				= 'ball'
	};
	
	
	// New controller types are registered with the engine in the Game constructor.
	enum
	{
		kControllerBall			= 'ball'
	};
	
	
	// This is the type of our custom particle system.
	
	enum
	{
		kParticleSystemSpark	= 'sprk'
	};
	
	
	// New locator types are registered with the engine in the Game constructor.
	// The 'spec' locator is used to specify where the spectator camera should
	// be positioned when a world is loaded.
	
	enum
	{
		kLocatorSpectator		= 'spec'
	};
	
	
	// An Action object represents an input action that can be triggered by
	// some input control, such as a key on the keyboard or a button on a joystick.
	// The Begin() and End() methods are called when the button is pressed and
	// released, respectively. Actions are registered with the Input Manager when
	// the Game class is constructed.
	
	class MovementAction : public Action
	{
		private:
			
			unsigned_int32	movementFlag;
			
		public:
			
			MovementAction(unsigned_int32 type, unsigned_int32 flag);
			~MovementAction();
			
			void Begin(void);
			void End(void);
	};
	
	
	// Controllers are used to control anything that moves in the world.
	// New types of controllers defined by the application/game module are
	// registered with the engine when the Game class is constructed.
	//
	// The BallController inherits from the built-in rigid body controller, 
	// which handles the ball's motion and collision detection. We are only
	// adding a little bit of functionality that causes a particle system 
	// to be created when a ball hits another ball. 
	 
	class BallController : public RigidBodyController
	{ 
		private:
			
			BallController(const BallController& ballController);
			 
			Controller *Replicate(void) const override;
		
		public:
			 
			BallController();
			~BallController();
			
			static bool ValidNode(const Node *node);
			
			void Preprocess(void);
			
			RigidBodyStatus HandleNewRigidBodyContact(const RigidBodyContact *contact, RigidBodyController *contactBody);
	};
	
	
	// The SparkParticleSystem class implements a simple particle system that
	// creates a small burst of sparks.
	
	class SparkParticleSystem : public LineParticleSystem
	{
		// This friend declaration allows the particle system registration object
		// to construct a SparkParticleSystem object using the private default constructor.
		
		friend class ParticleSystemReg<SparkParticleSystem>;
		
		private:
			
			enum
			{
				kMaxParticleCount = 100000
			};
			
			int32				sparkCount;
			
			// This is where information about each particle is stored.
			
			ParticlePool<>		particlePool;
			Particle			particleArray[kMaxParticleCount];
			
			SparkParticleSystem();
			
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
		
		public:
			
			SparkParticleSystem(int32 count);
			~SparkParticleSystem();
			
			void Preprocess(void);
			void AnimateParticles(void);
	};
	
	
	// The StartWindow class is a simple example of a window that handles a button click.
	// We add the Singleton base class so that a pointer to the window can be tracked easily.
	
	class StartWindow : public Window, public Singleton<StartWindow>
	{
		private:
			
			WidgetObserver<StartWindow>		startButtonObserver;
			
			void HandleStartButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			StartWindow();
			~StartWindow();
			
			void Preprocess(void);
	};
	
	
	// The application/game module will usually define a subclass of the World
	// class so that extra information can be associated with the current world.
	// In this case, an instance of the SpectatorCamera class is included
	// with the world. A new instance of this World subclass should be returned
	// when the Game::ConstructWorld() function is called (see below).
	
	class BaseWorld : public World
	{
		private:
			
			SpectatorCamera		spectatorCamera;
			
			static const LocatorMarker *FindSpectatorLocator(const Zone *zone);
		
		public:
			
			BaseWorld(const char *name);
			~BaseWorld();
			
			SpectatorCamera *GetSpectatorCamera(void)
			{
				return (&spectatorCamera);
			}
			
			WorldResult Preprocess(void);
			void Render(void);
	};
	
	
	// Every application/game module needs to define a subclass of the Application
	// class to serve as the primary interface with the engine. This subclass is
	// constructed and returned to the engine in the ConstructApplication() function.
	// There should be only one instance of this class, so it inherits from the
	// Singleton template. A pointer to the Game instance is declared below.
	
	class BaseSDK : public Application, public Singleton<BaseSDK>
	{
		private:
			
			DisplayEventHandler						displayEventHandler;
			
			ModelRegistration						ballModelReg;
			ControllerReg<BallController>			controllerReg;
			ParticleSystemReg<SparkParticleSystem>	sparkParticleSystemReg;
			LocatorRegistration						locatorReg;
			
			InputMgr::KeyProc						*prevEscapeProc;
			void									*prevEscapeCookie;
			
			MovementAction							*forwardAction;
			MovementAction							*backwardAction;
			MovementAction							*leftAction;
			MovementAction							*rightAction;
			MovementAction							*upAction;
			MovementAction							*downAction;
			
			static World *ConstructWorld(const char *name, void *cookie);
			
			static void HandleDisplayEvent(const DisplayEventData *eventData, void *cookie);
			
			static void EscapeProc(void *cookie);
		
		public:
			
			BaseSDK();
			~BaseSDK();
	};
	
	
	// This is a pointer to the one instance of the Game class through which
	// any other part of the application/game module can access it.
	
	extern BaseSDK *TheBase;
	
	// This is a pointer to the start window. We only keep this around so that
	// we can delete the window before exiting if it's still on the screen.
	
	extern StartWindow *TheStartWindow;
}


#endif

// ZYURVUR
