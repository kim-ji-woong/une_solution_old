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


#ifndef C4SoundPlayer_h
#define C4SoundPlayer_h


#include "C4Plugins.h"


extern "C"
{
	C4MODULEEXPORT C4::Plugin *ConstructPlugin(void);
}


namespace C4
{
	class Sound;
	
	
	enum
	{
		kWidgetWave		= 'wave'
	};
	
	
	class WaveWidget : public RenderableWidget
	{
		private:
			
			int32				quadCount;
			Point3D				*waveVertex;
			
			List<Attribute>		attributeList;
			DiffuseAttribute	diffuseAttribute;
		
		public:
			
			WaveWidget(const Vector2D& size, int32 count, const ColorRGBA& color);
			~WaveWidget();
			
			void Preprocess(void);
			
			void BuildWave(const Sound *sound, int32 channel = 0);
	};
	
	
	class SoundWindow : public Window, public ListElement<SoundWindow>
	{
		friend class SoundPlayer;
		
		private:
			
			ResourceName				resourceName;
			Sound						*soundObject;
			bool						streamingFlag;
			
			IconButtonWidget			*playButton;
			IconButtonWidget			*stopButton;
			CheckWidget					*loopBox;
			SliderWidget				*volumeSlider;
			
			LineWidget					*markLeft;
			LineWidget					*markRight;
			WaveWidget					*waveLeft;
			WaveWidget					*waveRight;
			
			static List<SoundWindow>	windowList;
			
			static void SoundComplete(Sound *sound, void *cookie);
		
		public:
			
			SoundWindow(const char *name);
			~SoundWindow();
			
			const char *GetResourceName(void) const
			{
				return (resourceName);
			}
			
			static SoundWindow *Open(const char *name);
			
			void Preprocess(void);
			void Move(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class SoundPlayer : public Plugin, public Singleton<SoundPlayer>
	{
		private:
			
			StringTable						stringTable;
			
			CommandObserver<SoundPlayer>	soundCommandObserver;
			Command							soundCommand;
			MenuItemWidget					soundMenuItem;
			
			Link<FilePicker>				soundPicker;
			
			static void SoundPicked(FilePicker *picker, void *cookie); 

			void HandleOpenSoundMenuItem(Widget *widget, const WidgetEventData *eventData); 
			void HandleSoundCommand(Command *command, const char *text); 
			 
		public:
			 
			SoundPlayer();
			~SoundPlayer();
			
			const StringTable *GetStringTable(void) const 
			{
				return (&stringTable);
			}
	}; 
	
	
	extern SoundPlayer *TheSoundPlayer;
}


#endif

// ZYURVUR
