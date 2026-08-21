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


#ifndef C4StringImporter_h
#define C4StringImporter_h


#include "C4Plugins.h"


extern "C"
{
	C4MODULEEXPORT C4::Plugin *ConstructPlugin(void);
}


namespace C4
{
	class StringInfo;
	
	
	class TextResource : public Resource<TextResource>
	{
		friend class Resource<TextResource>;
		
		private:
			
			static ResourceDescriptor	descriptor;
			
			~TextResource();
		
		public:
			
			TextResource(const char *name, ResourceCatalog *catalog);
	};
	
	
	class StringList : public List<StringInfo>
	{
		private:
			
			unsigned_int32 ReadTable(char *text, unsigned_int32 size, unsigned_int32 pos);
			void WriteTable(File *file);
		
		public:
			
			StringList();
			~StringList();
			
			ResourceResult ReadTextFile(const char *name);
			void WriteResourceFile(const char *name);
	};
	
	
	class StringInfo : public ListElement<StringInfo>
	{
		public:
			
			StringList		stringList;
			unsigned_int32	stringID;
			String<>		string;
			
			StringInfo(unsigned_int32 id, const char *text);
			~StringInfo();
	};
	
	
	class StringImporter : public Plugin, public Singleton<StringImporter>
	{
		private:
			
			StringTable							stringTable;
			
			CommandObserver<StringImporter>		importStringCommandObserver;
			Command								importStringCommand;
			MenuItemWidget						importStringMenuItem;
			
			Link<FilePicker>					importStringPicker;
			
			static void ImportStringPicked(FilePicker *picker, void *cookie);

			void HandleImportStringMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleImportStringCommand(Command *command, const char *text);
		
		public:
			
			StringImporter();
			~StringImporter();
			
			const StringTable *GetStringTable(void) const
			{
				return (&stringTable);
			}
	};
	
	
	extern StringImporter *TheStringImporter;
}


#endif

// ZYURVUR
