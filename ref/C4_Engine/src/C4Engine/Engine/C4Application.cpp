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


#include "C4Application.h"
#include "C4Engine.h"
#include "C4World.h"


using namespace C4;


Application *C4::TheApplication = nullptr;


Application::Application() : Singleton<Application>(TheApplication)
{
}

Application::~Application()
{
}

#if C4DEBUG

	void Application::DebugApplication(void)
	{
	}

#elif C4OPTIMIZED

	void Application::OptimizedApplication(void)
	{
	}

#elif C4SERVER

	void Application::ServerApplication(void)
	{
	}

#endif

void Application::HandleConnectionEvent(ConnectionEvent event, const NetworkAddress& address, const void *param)
{
	String<63>	text;
	
	switch (event)
	{
		case kConnectionAttemptFailed:
			
			text = "[#FFF]Fail: [#8FF]";
			text += MessageMgr::AddressToString(address, true);
			Engine::Report(text);
			break;
		
		case kConnectionClientOpened:
			
			text = "[#FFF]Open: [#8FF]";
			text += MessageMgr::AddressToString(address, true);
			Engine::Report(text);
			break;
		
		case kConnectionServerAccepted:
			
			text = "[#FFF]Accept: [#8FF]";
			text += MessageMgr::AddressToString(address, true);
			Engine::Report(text);
			break;
		
		case kConnectionServerClosed:
		case kConnectionClientClosed:
			
			text = "[#FFF]Close: [#8FF]";
			text += MessageMgr::AddressToString(address, true);
			Engine::Report(text);
			break;
		
		case kConnectionServerTimedOut:
		case kConnectionClientTimedOut:
			
			text = "[#FFF]Timeout: [#8FF]";
			text += MessageMgr::AddressToString(address, true);
			Engine::Report(text);
			break;
	}
}

void Application::HandlePlayerEvent(PlayerEvent event, Player *player, const void *param)
{
}

void Application::HandleGameEvent(GameEvent event, const void *param)
{
}

Message *Application::ConstructMessage(MessageType type, Decompressor& data) const
{
	return (nullptr);
}

void Application::ReceiveMessage(Player *from, const NetworkAddress& address, const Message *message)
{
}

EngineResult Application::LoadWorld(const char *name) 
{
	return (TheWorldMgr->LoadWorld(name)); 
} 
 
void Application::UnloadWorld(void)
{ 
	TheWorldMgr->UnloadWorld();
}

void Application::ApplicationTask(void) 
{
}

void Application::WorldRenderTask(void) 
{
}

void Application::InterfaceRenderTask(void)
{
}

// ZYURVUR
GenericModel*  gModel;
void C4::Application::DrawCube()
{
	//gModel = new GenericModel("tutorial/model/ball");
	//TheWorldMgr->GetWorld()->("tutorial/model/ball", gModel);
}
