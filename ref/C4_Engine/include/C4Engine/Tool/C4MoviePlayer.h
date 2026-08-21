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


#ifndef C4MoviePlayer_h
#define C4MoviePlayer_h


#include "C4MoviePlugin.h"


extern "C"
{
	C4MODULEEXPORT C4::Plugin *ConstructPlugin(void);
}


namespace C4
{
	class MovieWindow : public Window, public ListElement<MovieWindow>
	{
		friend class MoviePlayer;
		
		private:
			
			ResourcePath				resourceName;
			Movie						*movieObject;
			Vector2D					movieSize;
			
			IconButtonWidget			*playButton;
			IconButtonWidget			*stopButton;
			CheckWidget					*loopBox;
			ProgressWidget				*progressBar;
			
			static List<MovieWindow>	windowList;
			
			static void MovieComplete(Movie *movie, void *cookie);
		
		public:
			
			MovieWindow(const char *name, Movie *movie, const Vector2D& size);
			~MovieWindow();
			
			static MovieResult Open(const char *name);
			
			void Preprocess(void);
			void Move(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class MoviePlayer : public Plugin, public Singleton<MoviePlayer>
	{
		private:
			
			StringTable						stringTable;
			
			CommandObserver<MoviePlayer>	movieCommandObserver;
			Command							movieCommand;
			MenuItemWidget					movieMenuItem;
			
			Link<FilePicker>				moviePicker;
			
			static void MoviePicked(FilePicker *picker, void *cookie);

			void HandleOpenMovieMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleMovieCommand(Command *command, const char *text);
			
		public:
			
			MoviePlayer();
			~MoviePlayer();
			
			const StringTable *GetStringTable(void) const
			{
				return (&stringTable);
			}
	};
	
	
	extern MoviePlayer *TheMoviePlayer;
}


#endif

// ZYURVUR
