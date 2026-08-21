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


#include "C4World.h"
#include "C4Plugins.h"
#include "C4ToolWindows.h"
#include "C4Application.h"


using namespace C4;


Command::Command(const char *name, ObserverType *observer)
{
	commandName = name;
	AddObserver(observer);
}

Command::~Command()
{
}


#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]

void Engine::HandleQuitCommand(Command *command, const char *text)
{
	Quit();
}

void Engine::HandleWireCommand(Command *command, const char *text)
{
	#if C4DIAGNOSTICS
	
		unsigned_int32 flags = TheGraphicsMgr->GetDiagnosticFlags();
		
		if (text[0] == 0)
		{
			TheGraphicsMgr->SetDiagnosticFlags(flags ^ kDiagnosticWireframe);
		}
		else
		{
			int32 n = Text::StringToInteger(text);
			flags &= ~(kDiagnosticWireframe | kDiagnosticDepthTest);
			
			if (n == 0) TheGraphicsMgr->SetDiagnosticFlags(flags);
			else if (n == 1) TheGraphicsMgr->SetDiagnosticFlags(flags | kDiagnosticWireframe);
			else TheGraphicsMgr->SetDiagnosticFlags(flags | (kDiagnosticWireframe | kDiagnosticDepthTest));
		}
	
	#endif
}

void Engine::HandleNormCommand(Command *command, const char *text)
{
	#if C4DIAGNOSTICS
	
		TheGraphicsMgr->SetDiagnosticFlags(TheGraphicsMgr->GetDiagnosticFlags() ^ kDiagnosticNormals);
	
	#endif
}

void Engine::HandleTangCommand(Command *command, const char *text)
{
	#if C4DIAGNOSTICS
	
		TheGraphicsMgr->SetDiagnosticFlags(TheGraphicsMgr->GetDiagnosticFlags() ^ kDiagnosticTangents);
	
	#endif
}

void Engine::HandleShadCommand(Command *command, const char *text)
{
	#if C4DIAGNOSTICS
	
		TheGraphicsMgr->SetDiagnosticFlags(TheGraphicsMgr->GetDiagnosticFlags() ^ kDiagnosticShadows);
	
	#endif
}

void Engine::HandleSbndCommand(Command *command, const char *text)
{
	#if C4DIAGNOSTICS
	
		TheGraphicsMgr->SetDiagnosticFlags(TheGraphicsMgr->GetDiagnosticFlags() ^ kDiagnosticShadowBounds);
	
	#endif
}

void Engine::HandleLrgnCommand(Command *command, const char *text)
{
	#if C4DIAGNOSTICS
	
		World *world = TheWorldMgr->GetWorld();
		if (world) world->SetDiagnosticFlags(world->GetDiagnosticFlags() ^ kDiagnosticLightRegions);
	
	#endif
}

void Engine::HandleSrgnCommand(Command *command, const char *text)
{ 
	#if C4DIAGNOSTICS
	 
		World *world = TheWorldMgr->GetWorld(); 
		if (world) 
		{
			world->PurgeShadowDiagnosticData(); 
			world->SetDiagnosticFlags(world->GetDiagnosticFlags() | kDiagnosticShadowRegions);
		}
	
	#endif 
}

void Engine::HandleDoffCommand(Command *command, const char *text)
{ 
	#if C4DIAGNOSTICS
	
		World *world = TheWorldMgr->GetWorld();
		if (world)
		{
			world->SetDiagnosticFlags(world->GetDiagnosticFlags() & ~kDiagnosticShadowRegions);
			world->PurgeShadowDiagnosticData();
		}
	
	#endif
}

void Engine::HandleSpthCommand(Command *command, const char *text)
{
	#if C4DIAGNOSTICS
	
		World *world = TheWorldMgr->GetWorld();
		if (world) world->SetDiagnosticFlags(world->GetDiagnosticFlags() ^ kDiagnosticSourcePaths);
	
	#endif
}

void Engine::HandleBodyCommand(Command *command, const char *text)
{
	#if C4DIAGNOSTICS
	
		World *world = TheWorldMgr->GetWorld();
		if (world)
		{
			unsigned_int32 flags = world->GetDiagnosticFlags();
			if (flags & kDiagnosticRigidBodies)
			{
				world->SetDiagnosticFlags(flags & ~kDiagnosticRigidBodies);
				world->PurgeRigidBodyDiagnosticData();
			}
			else
			{
				world->SetDiagnosticFlags(flags | kDiagnosticRigidBodies);
			}
		}
	
	#endif
}

void Engine::HandleCtacCommand(Command *command, const char *text)
{
	#if C4DIAGNOSTICS
	
		World *world = TheWorldMgr->GetWorld();
		if (world)
		{
			unsigned_int32 flags = world->GetDiagnosticFlags();
			if (flags & kDiagnosticContacts)
			{
				world->SetDiagnosticFlags(flags & ~kDiagnosticContacts);
				world->PurgeContactDiagnosticData();
			}
			else
			{
				world->SetDiagnosticFlags(flags | kDiagnosticContacts);
			}
		}
	
	#endif
}

void Engine::HandleRateCommand(Command *command, const char *text)
{
	RateWindow::Open();
}

void Engine::HandleStatCommand(Command *command, const char *text)
{
	StatsWindow::Open();
}

void Engine::HandleSmapCommand(Command *command, const char *text)
{
	ShadowMapWindow::Open();
}

void Engine::HandleNetCommand(Command *command, const char *text)
{
	NetworkWindow::Open();
}

void Engine::HandleExtCommand(Command *command, const char *text)
{
	ExtensionsWindow::Open();
}

void Engine::HandleRsrcCommand(Command *command, const char *text)
{
	TheConsoleWindow->AddText("\n[+UND][#FFF]Resource type[RGHT]Cache size[-UND]");
	
	const ResourceCatalog *catalog = TheResourceMgr->GetVirtualCatalog();
	const Map<ResourceTracker> *trackerMap = catalog->GetTrackerMap();
	
	const ResourceTracker *tracker = trackerMap->First();
	while (tracker)
	{
		TheConsoleWindow->AddText((String<kMaxCommandLength>(Text::TypeToString((tracker->GetResourceType() << 8) | 0x20)) += "[RGHT]") += tracker->GetCurrentCacheSize());
		tracker = tracker->Next();
	}
}

void Engine::HandleHeapCommand(Command *command, const char *text)
{
	TheConsoleWindow->AddText("\n[+UND][#FFF]Memory heap[RGHT]Bytes used[-UND]");
	
	const Heap *heap = MemoryMgr::GetFirstHeap();
	do
	{
		TheConsoleWindow->AddText((String<kMaxCommandLength>(heap->GetHeapName()) += "[RGHT]") += heap->GetTotalSize());
		heap = heap->GetNextHeap();
	} while (heap);
	
	#if C4DEBUG_MEMORY
	
		unsigned_int32 systemSize = 0;
		const MemBlockHeader *bh = MemoryMgr::GetFirstSystemBlock();
		while (bh)
		{
			systemSize += bh->physicalSize;
			bh = bh->nextBlock;
		}
		
		TheConsoleWindow->AddText(String<kMaxCommandLength>("System[RGHT]") += systemSize);
	
	#endif
}

void Engine::HandleDumpCommand(Command *command, const char *text)
{
	#if C4LEAK_DETECTION
	
		DumpMemory("Memory.txt");
	
	#endif
}

void Engine::HandleVisitCommand(Command *command, const char *text)
{
	unsigned_int32	type;
	
	text += Text::ReadType(text, &type);
	if (type != 0)
	{
		ResourcePath	path;
		
		text += Text::GetWhitespaceLength(text);
		Text::ReadString(text, path, kMaxResourcePathLength);
		
		FilePicker::SetVisit(type, path);
	}
}

void Engine::HandleShotCommand(Command *command, const char *text)
{
	TargaHeader		header;
	File			file;
	
	String<kMaxFileNameLength> name(text);
	int32 p = Text::FindChar(text, '#');
	if (p != -1)
	{
		static int32 shotIndex = 0;
		
		name[p] = 0;
		
		int32 i = ++shotIndex;
		if (i < 10) name += "000";
		else if (i < 100) name += "00";
		else if (i < 1000) name += "0";
		
		name += i;
	}
	
	name += ".tga";
	
	String<kMaxFileNameLength> path(TheResourceMgr->GetSaveCatalog()->GetRootPath());
	if (file.Open(path += name, kFileCreate) == kFileOkay)
	{
		int32 width = TheDisplayMgr->GetDisplayWidth();
		int32 height = TheDisplayMgr->GetDisplayHeight();
		
		int32 pixelCount = width * height;
		Color4C *image = new Color4C[pixelCount * 2];
		TheGraphicsMgr->ReadImageBuffer(Rect(0, 0, width, height), image, width, Integer2D(0, 0));
		
		for (machine a = 0; a < pixelCount; a++) image[a].SetMaxAlpha();
		
		MemoryMgr::ClearMemory(&header, sizeof(TargaHeader));
		header.width = (int16) width;
		header.height = (int16) height;
		header.pixelDepth = 32;
		header.imageDescriptor = 8;
		
		#if C4BIGENDIAN
		
			Reverse(&header.width);
			Reverse(&header.height);
		
		#endif
		
		unsigned_int8 *compressedData = reinterpret_cast<unsigned_int8 *>(image + pixelCount);
		unsigned_int32 compressedSize = Image::CompressImageRLE_RGBA32(image, compressedData, pixelCount);
		if (compressedSize != 0)
		{
			header.imageType = 10;
			
			file.Write(&header, sizeof(TargaHeader));
			file.Write(compressedData, compressedSize);
		}
		else
		{
			header.imageType = 2;
			for (machine a = 0; a < pixelCount; a++) image[a].ExchangeRedBlue();
			
			file.Write(&header, sizeof(TargaHeader));
			file.Write(image, pixelCount * sizeof(Color4C));
		}
		
		delete[] image;
	}
}

void Engine::HandleUndefCommand(Command *command, const char *text)
{
	if (*text == '$')
	{
		Variable *variable = GetVariable(text + 1);
		if ((variable) && (!(variable->GetVariableFlags() & kVariablePermanent))) delete variable;
	}
}

void Engine::HandleBindCommand(Command *command, const char *text)
{
	if (*text != 0)
	{
		Variable *variable = GetVariable("device");
		InputDevice *device = (variable) ? TheInputMgr->FindDevice(variable->GetValue()) : nullptr;
		InputDevice *keyboard = TheInputMgr->FindDevice(kInputKeyboard);
		if (!device) device = keyboard;
		if (device)
		{
			String<kMaxVariableValueLength>		name;
			
			text += Text::ReadString(text, name, kMaxInputControlNameLength);
			InputControl *control = device->FindControl(name);
			if ((!control) && (keyboard)) control = keyboard->FindControl(name);
			if (control)
			{
				control->SetControlAction(nullptr);
				
				text += Text::GetWhitespaceLength(text);
				Text::ReadString(text, name, kMaxVariableValueLength);
				
				unsigned_int32 k = name[0];
				if (k != 0)
				{
					if (k == '%')
					{
						if (name.Length() == 5)
						{
							unsigned_int32 type = (name[1] << 24) | (name[2] << 16) | (name[3] << 8) | name[4];
							Action *action = TheInputMgr->FindAction(type);
							if (action) control->SetControlAction(action);
						}
					}
					else
					{
						CommandAction *command = new CommandAction(name);
						TheInputMgr->AddAction(command);
						control->SetControlAction(command);
					}
				}
			}
		}
	}
}

void Engine::HandleUnbindCommand(Command *command, const char *text)
{
	if (*text != 0)
	{
		Variable *variable = GetVariable("device");
		InputDevice *device = (variable) ? TheInputMgr->FindDevice(variable->GetValue()) : nullptr;
		InputDevice *keyboard = TheInputMgr->FindDevice(kInputKeyboard);
		if (!device) device = keyboard;
		if (device)
		{
			String<kMaxInputControlNameLength>		name;
			
			Text::ReadString(text, name, kMaxInputControlNameLength);
			InputControl *control = device->FindControl(name);
			if (!control) control = keyboard->FindControl(name);
			if (control) control->SetControlAction(nullptr);
		}
	}
}

void Engine::HandleSayCommand(Command *command, const char *text)
{
	TheMessageMgr->SendChatMessage(text);
}

void Engine::HandleAddressCommand(Command *command, const char *text)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	if (!TheMessageMgr->Multiplayer()) Report(table->GetString(StringID('CONS', 'NONT')));
	Report(String<kMaxCommandLength>(table->GetString(StringID('CONS', 'ADDR'))) + MessageMgr::AddressToString(TheNetworkMgr->GetLocalAddress(), true));
}

void Engine::HandleResolveCommand(Command *command, const char *text)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	if (!TheMessageMgr->Multiplayer())
	{
		Report(table->GetString(StringID('CONS', 'NONT')));
	}
	else
	{
		const char *message = table->GetString(StringID('CONS', 'RSLV'));
		Report(String<kMaxCommandLength>(message) + text);
		
		DomainNameResolver *resolver = TheNetworkMgr->ResolveAddress(text);
		resolver->SetCompletionProc(&ResolverComplete);
	}
}

void Engine::ResolverComplete(DomainNameResolver *resolver, void *cookie)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	if (resolver->GetResolveResult() == kEngineOkay)
	{
		TheConsoleWindow->AddText(String<kMaxCommandLength>(table->GetString(StringID('CONS', 'RNAM'))) + resolver->GetName());
		TheConsoleWindow->AddText(String<kMaxCommandLength>(table->GetString(StringID('CONS', 'RADR'))) + MessageMgr::AddressToString(resolver->GetAddress()));
	}
	else
	{
		Report(table->GetString(StringID('CONS', 'RERR')));
	}
}

void Engine::HandleDisconnectCommand(Command *command, const char *text)
{
	TheMessageMgr->DisconnectAll();
}

void Engine::HandleExecCommand(Command *command, const char *text)
{
	ExecuteFile(text);
}

void Engine::HandleImportCommand(Command *command, const char *text)
{
	ExecuteFile(text, ThePluginMgr->GetImportCatalog());
}

void Engine::HandleCmdCommand(Command *command, const char *text)
{
	TheConsoleWindow->AddText("\n[+UND][#FFF]Commands[-UND]");
	
	const Command *cmd = GetFirstCommand();
	while (cmd)
	{
		TheConsoleWindow->AddText(cmd->GetName());
		cmd = cmd->Next();
	}
}

void Engine::HandleVarCommand(Command *command, const char *text)
{
	TheConsoleWindow->AddText("\n[+UND][#FFF]Variables[-UND]");
	
	const Variable *variable = GetFirstVariable();
	while (variable)
	{
		String<kMaxVariableNameLength + kMaxVariableValueLength + 6> string("$");
		string += variable->GetName();
		string += " = \"";
		string += variable->GetValue();
		string += "\"";
		
		TheConsoleWindow->AddText(string);
		variable = variable->Next();
	}
}

void Engine::HandleLoadCommand(Command *command, const char *text)
{
	if (*text != 0)
	{
		ResourceName	name;
		
		Text::ReadString(text, name, kMaxResourceNameLength);
		TheApplication->LoadWorld(name);
	}
}

void Engine::HandleUnloadCommand(Command *command, const char *text)
{
	TheApplication->UnloadWorld();
}

// ZYURVUR
