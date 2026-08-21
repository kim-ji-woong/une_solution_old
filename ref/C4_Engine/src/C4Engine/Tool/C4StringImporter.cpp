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


#include "C4StringImporter.h"


using namespace C4;


StringImporter *C4::TheStringImporter = nullptr;


ResourceDescriptor TextResource::descriptor("txt");


C4::Plugin *ConstructPlugin(void)
{
	return (new StringImporter);
}


TextResource::TextResource(const char *name, ResourceCatalog *catalog) : Resource<TextResource>(name, catalog)
{
}

TextResource::~TextResource()
{
}


StringList::StringList()
{
}

StringList::~StringList()
{
}

unsigned_int32 StringList::ReadTable(char *text, unsigned_int32 size, unsigned_int32 pos)
{
	unsigned_int32	id;
	const char		*start;
	int32			offset;
	
	bool identifier = false;
	bool string = false;
	bool control = false;
	
	while (pos < size)
	{
		int32 k = text[pos++];
		
		if ((k == '{') && (!identifier) && (!string))
		{
			StringInfo *info = Last();
			if (!info)
			{
				info = new StringInfo(0, "");
				Append(info);
			}
			
			StringList *list = &info->stringList;
			pos = list->ReadTable(text, size, pos);
		}
		else if ((k == '}') && (!identifier) && (!string))
		{
			break;
		}
		else if ((k == '\'') && (!string))
		{
			identifier = !identifier;
			if (identifier) id = 0;
		}
		else if ((k == '"') && (!identifier) && (!control))
		{
			if (!string)
			{
				start = &text[pos];
				string = true;
				offset = 0;
			}
			else
			{
				string = false;
				text[pos - 1 - offset] = 0;
				StringInfo *info = new StringInfo(id, start);
				Append(info);
			}
		}
		else
		{
			if (identifier)
			{
				id = (id << 8) | k;
			}
			else if (string)
			{
				if (control)
				{
					offset++;
					if (k == 't') k = 9;
					else if (k == 'n') k = 10;
					else if (k == 'r') k = 13;
				}
				 
				text[pos - 1 - offset] = (char) k;
			} 
		} 
		 
		control = ((!control) && (k == '\\'));
	} 
	
	return (pos);
}
 
ResourceResult StringList::ReadTextFile(const char *name)
{
	TextResource *textResource = TextResource::Get(name, 0, ThePluginMgr->GetImportCatalog());
	if (textResource) 
	{
		const char *text = static_cast<const char *>(textResource->GetData());
		ReadTable(const_cast<char *>(text), textResource->GetSize(), 0);
		textResource->Release();
		return (kResourceOkay);
	}
	
	return (kResourceNotFound);
}

void StringList::WriteTable(File *file)
{
	StringInfo *info = First();
	while (info)
	{
		StringHeader sh(info->stringID);
		
		unsigned_int32 headerPosition = file->GetPosition();
		file->Write(&sh, sizeof(StringHeader));
		
		unsigned_int32 len = info->string.Length() + 1;
		if (len > 1)
		{
			file->Write(&info->string[0], len);
			file->WritePad(4);
		}
		else
		{
			static int32 zero = 0;
			file->Write(&zero, 4);
		}
		
		if (info->stringList.First())
		{
			sh.SetFirstSubstringOffset(sizeof(StringHeader) + ((len + 3) & ~3));
			info->stringList.WriteTable(file);
		}
		
		info = info->Next();
		unsigned_int32 position = file->GetPosition();
		if (info) sh.SetNextStringOffset(position - headerPosition);
		
		file->SetPosition(headerPosition);
		file->Write(&sh, sizeof(StringHeader));
		file->SetPosition(position);
	}
}

void StringList::WriteResourceFile(const char *name)
{
	File			file;
	ResourcePath	path;
	
	TheResourceMgr->GetGenericCatalog()->GetResourcePath(StringTableResource::GetDescriptor(), name, &path);
	TheResourceMgr->CreateDirectoryPath(path);
	
	if (file.Open(path, kFileCreate) == kFileOkay)
	{
		int32 endian = 1;
		file.Write(&endian, 4);
		
		WriteTable(&file);
	}
}


StringInfo::StringInfo(unsigned_int32 id, const char *text) : string(text)
{
	stringID = id;
}

StringInfo::~StringInfo()
{
}


StringImporter::StringImporter() :
		Singleton<StringImporter>(TheStringImporter),
		stringTable("StringImporter/strings"),
		importStringCommandObserver(this, &StringImporter::HandleImportStringCommand),
		importStringCommand("istring", &importStringCommandObserver),
		importStringMenuItem(stringTable.GetString(StringID('MCMD')), WidgetObserver<StringImporter>(this, &StringImporter::HandleImportStringMenuItem))
{
	TheEngine->AddCommand(&importStringCommand);
	ThePluginMgr->AddToolMenuItem(&importStringMenuItem);
}

StringImporter::~StringImporter()
{
	FilePicker *picker = importStringPicker;
	delete picker;
}

void StringImporter::ImportStringPicked(FilePicker *picker, void *cookie)
{
	ResourceName	name;
	StringList		list;
	
	if (picker)
	{
		name = picker->GetFileName();
		name[Text::GetResourceNameLength(name)] = 0;
	}
	else
	{
		name = static_cast<const char *>(cookie);
	}
	
	if (list.ReadTextFile(name) == kResourceOkay)
	{
		list.WriteResourceFile(name);
	}
	else
	{
		const StringTable *table = TheStringImporter->GetStringTable();
		String<kMaxCommandLength> output(table->GetString(StringID('NRES')));
		output += name;
		Engine::Report(output);
	}
}

void StringImporter::HandleImportStringMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	FilePicker *picker = importStringPicker;
	if (picker)
	{
		TheInterfaceMgr->SetActiveWindow(picker);
	}
	else
	{
		const char *title = stringTable.GetString(StringID('OPEN'));
		
		picker = new FilePicker('SIMP', title, ThePluginMgr->GetImportCatalog(), TextResource::GetDescriptor());
		picker->SetCompletionProc(&ImportStringPicked);
		
		importStringPicker = picker;
		TheInterfaceMgr->AddWidget(picker);
	}
}

void StringImporter::HandleImportStringCommand(Command *command, const char *text)
{
	if (*text != 0)
	{
		ResourceName	name;
		
		Text::ReadString(text, name, kMaxResourceNameLength);
		ImportStringPicked(nullptr, &name[0]);
	}
	else
	{
		HandleImportStringMenuItem(nullptr, nullptr);
	}
}

// ZYURVUR
