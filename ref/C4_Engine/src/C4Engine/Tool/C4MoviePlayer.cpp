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


#include "C4MoviePlayer.h"


using namespace C4;


MoviePlayer *C4::TheMoviePlayer = nullptr;


List<MovieWindow> MovieWindow::windowList;


C4::Plugin *ConstructPlugin(void)
{
	return (new MoviePlayer);
}


MovieWindow::MovieWindow(const char *name, C4::Movie *movie, const Vector2D& size) : Window("MoviePlayer/Window")
{
	resourceName = name;
	windowList.Append(this);
	
	SetWidgetSize(Vector2D(Fmax(size.x + 16.0F, 200.0F), Fmax(size.y + 68.0F, 96.0F)));
	
	SetWindowTitle(name);
	SetStripTitle(&name[Text::GetDirectoryPathLength(name)]);
	SetStripIcon("MoviePlayer/window");
	
	movie->Retain();
	movie->SetCompletionProc(&MovieComplete, this);
	
	movieObject = movie;
	movieSize = size;
}

MovieWindow::~MovieWindow()
{
	if (movieObject) movieObject->Release();
}

MovieResult MovieWindow::Open(const char *name)
{
	MovieWindow *window = windowList.First();
	while (window)
	{
		if (window->resourceName == name)
		{
			TheInterfaceMgr->SetActiveWindow(window);
			return (kMovieOkay);
		}
		
		window = window->ListElement<MovieWindow>::Next();
	}
	
	MovieResult result = TheMoviePlugin->Initialize();
	if (result == kMovieOkay)
	{
		AutoRelease<Movie> movie(new Movie);
		result = movie->Load(name);
		if (result == kMovieOkay)
		{
			int32 width = movie->GetMovieWidth();
			int32 height = movie->GetMovieHeight();
			
			int32 maxWidth = TheDisplayMgr->GetDisplayWidth() - 24;
			int32 maxHeight = TheDisplayMgr->GetDisplayHeight() - 104;
			
			if (width > maxWidth)
			{
				height = height * maxWidth / width;
				width = maxWidth;
			}
			
			if (height > maxHeight)
			{
				width = width * maxHeight / height;
				height = maxHeight;
			}
			
			window = new MovieWindow(name, movie, Vector2D((float) width, (float) height));
			TheInterfaceMgr->AddWidget(window);
		}
	}
	
	return (result);
}

void MovieWindow::Preprocess(void)
{
	Window::Preprocess();
	
	Widget *movieBorder = FindWidget("MovieBorder");
	movieBorder->SetWidgetSize(movieSize);
	
	const Point3D& moviePosition = movieBorder->GetWidgetPosition();
	
	playButton = static_cast<IconButtonWidget *>(FindWidget("Play"));
	playButton->SetWidgetPosition(Point3D(playButton->GetWidgetPosition().x, moviePosition.y + movieSize.y + 26.0F, 0.0F));
	
	stopButton = static_cast<IconButtonWidget *>(FindWidget("Stop"));
	stopButton->SetWidgetPosition(Point3D(stopButton->GetWidgetPosition().x, moviePosition.y + movieSize.y + 26.0F, 0.0F)); 
	
	loopBox = static_cast<CheckWidget *>(FindWidget("Loop")); 
	loopBox->SetWidgetPosition(Point3D(loopBox->GetWidgetPosition().x, moviePosition.y + movieSize.y + 31.0F, 0.0F)); 
	 
	progressBar = static_cast<ProgressWidget *>(FindWidget("Progress"));
	progressBar->SetWidgetPosition(Point3D(progressBar->GetWidgetPosition().x, moviePosition.y + movieSize.y + 8.0F, 0.0F)); 
	progressBar->SetWidgetSize(Vector2D(movieSize.x, 8.0F));
	progressBar->SetMaxValue((int32) movieObject->GetDuration());
	
	Widget *progressBorder = FindWidget("ProgressBorder"); 
	progressBorder->SetWidgetPosition(progressBar->GetWidgetPosition());
	progressBorder->SetWidgetSize(progressBar->GetWidgetSize());
	
	MovieWidget *movieWidget = new MovieWidget(movieSize, movieObject); 
	movieWidget->SetWidgetPosition(movieBorder->GetWidgetPosition());
	AddNewSubnode(movieWidget);
}

void MovieWindow::Move(void)
{
	Window::Move();
	
	if (movieObject->Playing()) progressBar->SetValue((int32) movieObject->GetMovieTime());
}

void MovieWindow::MovieComplete(C4::Movie *movie, void *cookie)
{
	MovieWindow *window = static_cast<MovieWindow *>(cookie);
	window->playButton->Enable();
	window->stopButton->Disable();
	window->progressBar->SetValue((int32) window->movieObject->GetMovieTime());
}

bool MovieWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
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

void MovieWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	EventType eventType = eventData->eventType;
	
	if (eventType == kEventWidgetActivate)
	{
		if (widget == playButton)
		{
			if (movieObject->GetMovieTime() >= movieObject->GetDuration()) movieObject->SetMovieTime(0.0F);
			
			movieObject->Play();
			playButton->Disable();
			stopButton->Enable();
		}
		else if (widget == stopButton)
		{
			movieObject->Stop();
			playButton->Enable();
			stopButton->Disable();
		}
	}
	else if (eventType == kEventWidgetChange)
	{
		if (widget == loopBox)
		{
			movieObject->SetLoopCount((loopBox->GetValue() == 0) ? 0 : kMovieLoopInfinite);
		}
	}
}


MoviePlayer::MoviePlayer() :
		Singleton<MoviePlayer>(TheMoviePlayer),
		stringTable("MoviePlayer/strings"),
		movieCommandObserver(this, &MoviePlayer::HandleMovieCommand),
		movieCommand("movie", &movieCommandObserver),
		movieMenuItem(stringTable.GetString(StringID('MCMD')), WidgetObserver<MoviePlayer>(this, &MoviePlayer::HandleOpenMovieMenuItem))
{
	TheEngine->AddCommand(&movieCommand);
	ThePluginMgr->AddToolMenuItem(&movieMenuItem);
}

MoviePlayer::~MoviePlayer()
{
	FilePicker *picker = moviePicker;
	delete picker;
	
	MovieWindow::windowList.Purge();
}

void MoviePlayer::MoviePicked(FilePicker *picker, void *cookie)
{
	ResourceName name((picker) ? &picker->GetFileName()[0] : static_cast<const char *>(cookie));
	MovieResult result = MovieWindow::Open(name);
	if (result != kMovieOkay)
	{
		const StringTable *table = TheMoviePlayer->GetStringTable();
		if (result == kMovieInitFailed)
		{
			Engine::Report(table->GetString(StringID('NQTM')));
		}
		else
		{
			String<kMaxCommandLength> output(table->GetString(StringID('NRES')));
			output += name;
			Engine::Report(output);
		}
	}
}

void MoviePlayer::HandleOpenMovieMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	FilePicker *picker = moviePicker;
	if (picker)
	{
		TheInterfaceMgr->SetActiveWindow(picker);
	}
	else
	{
		const char *title = stringTable.GetString(StringID('OPEN'));
		
		picker = new FilePicker('MOOV', title, TheResourceMgr->GetGenericCatalog(), MovieResource::GetDescriptor());
		picker->SetCompletionProc(&MoviePicked);
		
		moviePicker = picker;
		TheInterfaceMgr->AddWidget(picker);
	}
}

void MoviePlayer::HandleMovieCommand(Command *command, const char *text)
{
	if (*text != 0)
	{
		ResourceName	name;
		
		Text::ReadString(text, name, kMaxResourceNameLength);
		MoviePicked(nullptr, &name[0]);
	}
	else
	{
		HandleOpenMovieMenuItem(nullptr, nullptr);
	}
}

// ZYURVUR
