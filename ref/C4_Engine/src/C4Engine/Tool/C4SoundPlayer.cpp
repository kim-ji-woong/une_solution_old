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


#include "C4SoundPlayer.h"
#include "C4Sound.h"


using namespace C4;


SoundPlayer *C4::TheSoundPlayer = nullptr;


List<SoundWindow> SoundWindow::windowList;


C4::Plugin *ConstructPlugin(void)
{
	return (new SoundPlayer);
}


WaveWidget::WaveWidget(const Vector2D& size, int32 count, const ColorRGBA& color) :
		RenderableWidget(kWidgetWave, kRenderQuads, size),
		diffuseAttribute(color)
{
	quadCount = count;
	waveVertex = new Point3D[count * 4];
}

WaveWidget::~WaveWidget()
{
	delete[] waveVertex;
}

void WaveWidget::Preprocess(void)
{
	RenderableWidget::Preprocess();
	
	attributeList.Append(&diffuseAttribute);
	SetMaterialAttributeList(&attributeList);
	
	SetVertexCount(quadCount * 4);
	SetAttributeArray(kArrayVertex, waveVertex);
}

void WaveWidget::BuildWave(const Sound *sound, int32 channel)
{
	float x1 = 0.0F;
	Point3D *vertex = waveVertex;
	
	float h = GetWidgetSize().y - 1.0F;
	float y0 = h * 0.5F;
	float scale = h / 65536.0F;
	
	if (!sound->Streaming())
	{
		const Sample *data = sound->GetSampleData() + channel;
		float interval = (float) sound->GetFrameCount() / (float) quadCount;
		
		for (machine a = 0; a < quadCount; a++)
		{
			float f = (float) a * interval;
			int32 p1 = (int32) f;
			int32 p2 = (int32) (f + interval);
			int32 value = Abs(ReadLittleEndianS16(&data[p1 & ~1]));
			for (machine p = p1 + 1; p < p2; p++) value = Max(value, Abs(ReadLittleEndianS16(&data[p & ~1])));
			
			float y = (float) value * scale;
			float y1 = y0 - y;
			float y2 = y0 + y + 1.0F;
			float x2 = x1 + 1.0F;
			
			vertex[0].Set(x1, y1, 0.0F);
			vertex[1].Set(x1, y2, 0.0F);
			vertex[2].Set(x2, y2, 0.0F);
			vertex[3].Set(x2, y1, 0.0F);
			
			x1 = x2;
			vertex += 4;
		}
	}
	else
	{
		int32 count = quadCount / 2;
		float interval = (float) (sound->GetSoundStreamer()->GetStreamBufferSize() / 2) / (float) count;
		
		for (machine buffer = 0; buffer < 2; buffer++)
		{
			const Sample *data = sound->GetSoundStreamer()->GetStreamBuffer(buffer)->GetSampleData() + channel;
			
			for (machine a = 0; a < count; a++)
			{
				float f = (float) a * interval;
				int32 p1 = (int32) f;
				int32 p2 = (int32) (f + interval);
				int32 value = Abs(ReadLittleEndianS16(&data[p1 & ~1]));
				for (machine p = p1 + 1; p < p2; p++) value = Max(value, Abs(ReadLittleEndianS16(&data[p & ~1])));
				
				float y = (float) value * scale;
				float y1 = y0 - y;
				float y2 = y0 + y + 1.0F;
				float x2 = x1 + 1.0F;
				
				vertex[0].Set(x1, y1, 0.0F); 
				vertex[1].Set(x1, y2, 0.0F);
				vertex[2].Set(x2, y2, 0.0F); 
				vertex[3].Set(x2, y1, 0.0F); 
				 
				x1 = x2;
				vertex += 4; 
			}
		}
	}
} 


SoundWindow::SoundWindow(const char *name) : Window("SoundPlayer/Window")
{ 
	SoundResult		result;
	
	resourceName = name;
	windowList.Append(this);
	
	ResourcePath title(name);
	SetWindowTitle(title += SoundResource::GetDescriptor()->GetExtension());
	SetStripTitle(&title[Text::GetDirectoryPathLength(title)]);
	SetStripIcon("SoundPlayer/window");
	
	soundObject = new Sound;
	
	if (SoundResource::DetermineStreaming(name))
	{
		streamingFlag = true;
		
		WaveStreamer *streamer = new WaveStreamer;
		soundObject->Stream(streamer);
		
		result = streamer->AddComponent(name);
	}
	else
	{
		streamingFlag = false;
		result = soundObject->Load(name);
	}
	
	if (result == kSoundOkay)
	{
		soundObject->SetCompletionProc(&SoundComplete, this);
		soundObject->SetSoundFlags(kSoundPersistent);
		soundObject->Play();
	}
	else
	{
		resourceName[0] = 0;
	}
}

SoundWindow::~SoundWindow()
{
	soundObject->Release();
}

SoundWindow *SoundWindow::Open(const char *name)
{
	SoundWindow *window = windowList.First();
	while (window)
	{
		if (window->resourceName == name)
		{
			TheInterfaceMgr->SetActiveWindow(window);
			return (window);
		}
		
		window = window->ListElement<SoundWindow>::Next();
	}
	
	window = new SoundWindow(name);
	if (window->GetResourceName()[0] == 0)
	{
		delete window;
		return (nullptr);
	}
	
	TheInterfaceMgr->AddWidget(window);
	return (window);
}

void SoundWindow::Preprocess(void)
{
	Window::Preprocess();
	
	playButton = static_cast<IconButtonWidget *>(FindWidget("Play"));
	stopButton = static_cast<IconButtonWidget *>(FindWidget("Stop"));
	loopBox = static_cast<CheckWidget *>(FindWidget("Loop"));
	volumeSlider = static_cast<SliderWidget *>(FindWidget("Volume"));
	
	TextWidget *freqText = static_cast<TextWidget *>(FindWidget("Freq"));
	freqText->SetText(String<63>(Text::FloatToString((float) soundObject->GetSampleRate() * 0.001F)) += " kHz");
	
	if (soundObject->GetChannelCount() == 1)
	{
		FindWidget("Stereo")->Hide();
		
		Widget *widget = FindWidget("Center");
		const Vector2D& size = widget->GetWidgetSize();
		
		waveLeft = new WaveWidget(size, (int32) size.x, ColorRGBA(0.125F, 0.625F, 1.0F));
		widget->AddNewSubnode(waveLeft);
		
		markLeft = new LineWidget(Vector2D(size.y, 1.0F), kLineSolid, ColorRGBA(0.5F, 0.5F, 0.5F));
		markLeft->SetWidgetMatrix3D(K::y_unit, K::minus_x_unit, K::z_unit);
		widget->AddNewSubnode(markLeft);
		
		waveLeft->BuildWave(soundObject);
		
		waveRight = nullptr;
		markRight = nullptr;
	}
	else
	{
		FindWidget("Mono")->Hide();
		
		Widget *leftWidget = FindWidget("Left");
		Widget *rightWidget = FindWidget("Right");
		const Vector2D& size = leftWidget->GetWidgetSize();
		
		waveLeft = new WaveWidget(size, (int32) size.x, ColorRGBA(0.125F, 0.625F, 1.0F));
		waveLeft->SetWidgetPosition(leftWidget->GetWidgetPosition());
		leftWidget->AddNewSubnode(waveLeft);
		
		waveRight = new WaveWidget(size, (int32) size.x, ColorRGBA(0.125F, 0.625F, 1.0F));
		rightWidget->AddNewSubnode(waveRight);
		
		markLeft = new LineWidget(Vector2D(size.y, 1.0F), kLineSolid, ColorRGBA(0.5F, 0.5F, 0.5F));
		markLeft->SetWidgetMatrix3D(K::y_unit, K::minus_x_unit, K::z_unit);
		leftWidget->AddNewSubnode(markLeft);
		
		markRight = new LineWidget(Vector2D(size.y, 1.0F), kLineSolid, ColorRGBA(0.5F, 0.5F, 0.5F));
		markRight->SetWidgetMatrix3D(K::y_unit, K::minus_x_unit, K::z_unit);
		rightWidget->AddNewSubnode(markRight);
		
		waveLeft->BuildWave(soundObject, 0);
		waveRight->BuildWave(soundObject, 1);
	}
}

void SoundWindow::Move(void)
{
	Window::Move();
	
	if (soundObject->GetSoundState() == kSoundPlaying)
	{
		int32 playFrame = soundObject->GetPlayFrame();
		int32 channelCount = soundObject->GetChannelCount();
		
		if (!streamingFlag)
		{
			int32 length = soundObject->GetFrameCount();
			float x = PositiveFloor((float) playFrame * waveLeft->GetWidgetSize().x / (float) length);
			
			if (channelCount == 1)
			{
				markLeft->SetWidgetPosition(Point3D(x, 0.0F, 0.0F));
				markLeft->Invalidate();
			}
			else
			{
				markLeft->SetWidgetPosition(Point3D(x, 0.0F, 0.0F));
				markLeft->Invalidate();
				
				markRight->SetWidgetPosition(Point3D(x, 0.0F, 0.0F));
				markRight->Invalidate();
			}
		}
		else
		{
			int32 length = soundObject->GetSoundStreamer()->GetStreamBufferSize() / sizeof(Sample);
			float w = waveLeft->GetWidgetSize().x * 0.5F;
			float x = PositiveFloor((float) (playFrame * channelCount) * w / (float) length);
			if (soundObject->GetPlayBuffer() != 0) x += w;
			
			if (channelCount == 1)
			{
				markLeft->SetWidgetPosition(Point3D(x, 0.0F, 0.0F));
				markLeft->Invalidate();
				
				waveLeft->BuildWave(soundObject);
			}
			else
			{
				markLeft->SetWidgetPosition(Point3D(x, 0.0F, 0.0F));
				markLeft->Invalidate();
				
				markRight->SetWidgetPosition(Point3D(x, 0.0F, 0.0F));
				markRight->Invalidate();
				
				waveLeft->BuildWave(soundObject, 0);
				waveRight->BuildWave(soundObject, 1);
			}
		}
	}
}

void SoundWindow::SoundComplete(Sound *sound, void *cookie)
{
	SoundWindow *window = static_cast<SoundWindow *>(cookie);
	window->playButton->Enable();
	window->stopButton->Disable();
	
	window->markLeft->Hide();
	if (window->markRight) window->markRight->Hide();
}

bool SoundWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyCommand)
	{
		if (eventData->keyCode == 'W')
		{
			Close();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void SoundWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	EventType eventType = eventData->eventType;
	
	if (eventType == kEventWidgetActivate)
	{
		if (widget == playButton)
		{
			soundObject->Play();
			playButton->Disable();
			stopButton->Enable();
			
			markLeft->Show();
			if (markRight) markRight->Show();
		}
		else if (widget == stopButton)
		{
			soundObject->Stop();
			playButton->Enable();
			stopButton->Disable();
			
			markLeft->Hide();
			if (markRight) markRight->Hide();
		}
	}
	else if (eventType == kEventWidgetChange)
	{
		if (widget == loopBox)
		{
			soundObject->SetLoopCount((loopBox->GetValue() == 0) ? 0 : kSoundLoopInfinite);
		}
		else if (widget == volumeSlider)
		{
			soundObject->SetSoundProperty(kSoundVolume, (float) volumeSlider->GetValue() * 0.01F);
		}
	}
}


SoundPlayer::SoundPlayer() :
		Singleton<SoundPlayer>(TheSoundPlayer),
		stringTable("SoundPlayer/strings"),
		soundCommandObserver(this, &SoundPlayer::HandleSoundCommand),
		soundCommand("sound", &soundCommandObserver),
		soundMenuItem(stringTable.GetString(StringID('MCMD')), WidgetObserver<SoundPlayer>(this, &SoundPlayer::HandleOpenSoundMenuItem))
{
	TheEngine->AddCommand(&soundCommand);
	ThePluginMgr->AddToolMenuItem(&soundMenuItem);
}

SoundPlayer::~SoundPlayer()
{
	FilePicker *picker = soundPicker;
	delete picker;
	
	SoundWindow::windowList.Purge();
}

void SoundPlayer::SoundPicked(FilePicker *picker, void *cookie)
{
	ResourceName	name;
	
	if (picker) name = picker->GetResourceName();
	else name = static_cast<const char *>(cookie);
	
	if (!SoundWindow::Open(name))
	{
		const StringTable *table = TheSoundPlayer->GetStringTable();
		String<kMaxCommandLength> output(table->GetString(StringID('NRES')));
		output += name;
		Engine::Report(output);
	}
}

void SoundPlayer::HandleOpenSoundMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	FilePicker *picker = soundPicker;
	if (picker)
	{
		TheInterfaceMgr->SetActiveWindow(picker);
	}
	else
	{
		const char *title = stringTable.GetString(StringID('OPEN'));
		
		picker = new SoundPicker('SOND', title, TheResourceMgr->GetGenericCatalog(), SoundResource::GetDescriptor());
		picker->SetCompletionProc(&SoundPicked);
		
		soundPicker = picker;
		TheInterfaceMgr->AddWidget(picker);
	}
}

void SoundPlayer::HandleSoundCommand(Command *comamnd, const char *text)
{
	if (*text != 0)
	{
		ResourceName	name;
		
		Text::ReadString(text, name, kMaxResourceNameLength);
		SoundPicked(nullptr, &name[0]);
	}
	else
	{
		HandleOpenSoundMenuItem(nullptr, nullptr);
	}
}

// ZYURVUR
