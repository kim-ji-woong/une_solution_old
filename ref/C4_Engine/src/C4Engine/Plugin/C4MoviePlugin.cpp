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


#include "C4MoviePlugin.h"
#include "C4Configuration.h"
#include "C4Geometries.h"


#if C4VISUALC

	#pragma warning (disable: 4311)		// pointer truncation
	#pragma warning (disable: 4312)		// conversion from 'int32' to 'C4::Movie *' of greater size

#endif


using namespace C4;


MoviePlugin *C4::TheMoviePlugin = nullptr;


ResourceDescriptor MovieResource::descriptor("mov", kResourceDontAppendType);


QT::MovieDrawingCompleteUPP C4::Movie::movieDrawUPP;
QT::MoviePrePrerollCompleteUPP C4::Movie::prerollCompleteUPP;


C4::Plugin *ConstructPlugin(void)
{
	return (new MoviePlugin);
}


MovieResource::MovieResource(const char *name, ResourceCatalog *catalog) : Resource<MovieResource>(name, catalog)
{
}

MovieResource::~MovieResource()
{
}


C4::Movie::Movie()
{
	movieState = 0;
}

C4::Movie::~Movie()
{
	if (movieState & kMovieLoaded)
	{
		if (playStatus == kMoviePrerolling) AbortPrePrerollMovie(qtMovie, QT::noErr);
		
		DisposeMovie(qtMovie);
		if (movieWorld) DisposeGWorld(movieWorld);
	}
}

MovieResult C4::Movie::Load(const char *name, unsigned_int32 loadFlags)
{
	QT::Handle		handle;
	QT::OSType		type;
	QT::Rect		movieBox;
	
	if (loadFlags & kMovieAbsolutePath)
	{
		#if C4WINDOWS
		
			QT::CFStringRef string = CFStringCreateWithCStringNoCopy(kCFAllocatorDefault, FileName(name), QT::kCFStringEncodingUTF8, kCFAllocatorNull);
			if (!string) return (kMovieLoadFailed);
			
			QTNewDataReferenceFromFullPathCFString(string, QT::kQTWindowsPathStyle, 0, &handle, &type);
			CFRelease(string);
		
		#elif C4MACOS
		
			QT::CFStringRef string = CFStringCreateWithCStringNoCopy(kCFAllocatorDefault, FileName(name), QT::kCFStringEncodingUTF8, kCFAllocatorNull);
			if (!string) return (kMovieLoadFailed);
			
			QTNewDataReferenceFromFullPathCFString(string, QT::kQTPOSIXPathStyle, 0, &handle, &type);
			CFRelease(string);
		
		#endif
		
		movieState &= ~kMovieStreaming;
	}
	else if (loadFlags & kMovieRemoteURL)
	{
		unsigned_int32 size = Text::GetTextLength(name) + 1;
		handle = NewHandle(size);
		MemoryMgr::CopyMemory(name, *handle, size);
		type = QT::URLDataHandlerSubType;
		
		movieState |= kMovieStreaming;
	}
	else
	{
		ResourcePath	path;
		
		TheResourceMgr->GetGenericCatalog()->GetResourcePath(MovieResource::GetDescriptor(), name, &path);
		
		#if C4WINDOWS
		 
			LPSTR		namePosition;
			char		fullPath[256]; 
			 
			GetFullPathNameA(FileName(path), 255, fullPath, &namePosition); 
			
			QT::CFStringRef string = CFStringCreateWithCStringNoCopy(kCFAllocatorDefault, fullPath, QT::kCFStringEncodingUTF8, kCFAllocatorNull); 
			if (!string) return (kMovieLoadFailed);
			
			QTNewDataReferenceFromFullPathCFString(string, QT::kQTWindowsPathStyle, 0, &handle, &type);
			CFRelease(string); 
		
		#elif C4MACOS
		
			FileMgr::FilePath fullPath(TheFileMgr->GetResourcesPath()); 
			fullPath += path;
			
			CFStringRef string = CFStringCreateWithCStringNoCopy(kCFAllocatorDefault, fullPath, kCFStringEncodingUTF8, kCFAllocatorNull);
			CFURLRef pathURL = CFURLCreateWithFileSystemPath(kCFAllocatorDefault, string, kCFURLPOSIXPathStyle, false);
			CFRelease(string);
			
			if (!pathURL) return (kMovieLoadFailed);
			
			QTNewDataReferenceFromCFURL(pathURL, 0, &handle, &type);
			CFRelease(pathURL);
		
		#endif
		
		movieState &= ~kMovieStreaming;
	}
	
	QT::OSErr error = NewMovieFromDataRef(&qtMovie, QT::newMovieActive, nullptr, handle, type);
	DisposeHandle(handle);
	
	if (error != QT::noErr) return (kMovieLoadFailed);
	
	movieWorld = nullptr;
	
	GetMovieBox(qtMovie, &movieBox);
	movieWidth = movieBox.right - movieBox.left;
	movieHeight = movieBox.bottom - movieBox.top;
	
	playStatus = kMovieStopped;
	loopCount = 0;
	
	int32 volume = GetMoviePreferredVolume(qtMovie);
	movieVolume = (float) volume * K::one_over_65536;
	
	TheMoviePlugin->movieList.Append(this);
	SetMovieDrawingCompleteProc(qtMovie, QT::movieDrawingCallWhenChanged, movieDrawUPP, (int32) this);
	
	movieState = (movieState & kMovieStreaming) | kMovieLoaded;
	return (kMovieOkay);
}

void C4::Movie::SetPixelMap(Color4C *image, int32 width, int32 height, int32 rowPixels)
{
	if (movieWorld)
	{
		DisposeGWorld(movieWorld);
		movieWorld = nullptr;
	}
	
	QT::Rect rect = {0, 0, (int16) height, (int16) width};
	QTNewGWorldFromPtr(&movieWorld, QT::k32RGBAPixelFormat, &rect, nullptr, nullptr, 0, image, rowPixels * sizeof(Color4C));
	
	SetMovieGWorld(qtMovie, movieWorld, nullptr);
	SetMovieBox(qtMovie, &rect);
}

void C4::Movie::SetVolume(float volume)
{
	if (movieVolume != volume)
	{
		movieVolume = volume;
		SetMovieVolume(qtMovie, (int32) (volume * 65536.0F));
	}
}

float C4::Movie::GetDuration(void) const
{
	QT::TimeValue time = GetMovieDuration(qtMovie);
	return ((float) time / (float) GetMovieTimeScale(qtMovie) * 1000.0F);
}

float C4::Movie::GetMovieTime(void) const
{
	QT::TimeValue time = ::GetMovieTime(qtMovie, nullptr);
	return ((float) time / (float) GetMovieTimeScale(qtMovie) * 1000.0F);
}

void C4::Movie::SetMovieTime(float time)
{
	QT::TimeValue value = (QT::TimeValue) (time * 0.001F * (float) GetMovieTimeScale(qtMovie));
	SetMovieTimeValue(qtMovie, value);
}

QT::OSErr QTCallback C4::Movie::MovieDrawProc(QT::Movie qtMovie, long data)
{
	((Movie *) data)->movieState |= kMovieUpdated;
	return (QT::noErr);
}

void QTCallback C4::Movie::PrerollCompleteProc(QT::Movie qtMovie, QT::OSErr err, void *cookie)
{
	C4::Movie *movie = static_cast<C4::Movie *>(cookie);
	if (err == QT::noErr)
	{
		movie->playStatus = kMoviePlaying;
		movie->Start();
	}
	else
	{
		movie->playStatus = kMovieStopped;
	}
}

void C4::Movie::Play(void)
{
	if (!Playing())
	{
		int32 trackCount = GetMovieTrackCount(qtMovie);
		for (machine a = 1; a <= trackCount; a++)
		{
			bool enable = false;
			QT::Track track = GetMovieIndTrack(qtMovie, a);
			QT::Media media = GetTrackMedia(track);
			if (media)
			{
				QT::OSType	type;
				
				GetMediaHandlerDescription(media, &type, nullptr, nullptr);
				if ((movieWorld) || (type == QT::SoundMediaType) || (type == QT::MusicMediaType)) enable = true;
			}
			
			SetTrackEnabled(track, enable);
		}
		
		if (movieState & kMovieStreaming)
		{
			playStatus = kMoviePrerolling;
			PrePrerollMovie(qtMovie, ::GetMovieTime(qtMovie, nullptr), GetMoviePreferredRate(qtMovie), prerollCompleteUPP, this);
		}
		else
		{
			playStatus = kMoviePlaying;
			Start();
		}
	}
}

void C4::Movie::Start(void)
{
	SetMovieVolume(qtMovie, (int32) (movieVolume * 65536.0F));
	StartMovie(qtMovie);
}

void C4::Movie::Stop(void)
{
	if (Playing())
	{
		playStatus = kMovieStopped;
		StopMovie(qtMovie);
	}
}

void C4::Movie::Pause(void)
{
	if (playStatus == kMoviePlaying)
	{
		playStatus = kMoviePaused;
		StopMovie(qtMovie);
	}
}

void C4::Movie::Resume(void)
{
	if (playStatus == kMoviePaused)
	{
		playStatus = kMoviePlaying;
		StartMovie(qtMovie);
	}
}

void C4::Movie::MovieTask(void)
{
	if (IsMovieDone(qtMovie))
	{
		if (loopCount > 0) loopCount--;
		if (loopCount != 0)
		{
			GoToBeginningOfMovie(qtMovie);
			StartMovie(qtMovie);
		}
		else
		{
			playStatus = kMovieStopped;
			CallCompletionProc();
		}
	}
}


MovieWidget::MovieWidget() : ImageWidget(kWidgetMovie)
{
	movieFlags = 0;
	movieLoadFlags = 0;
	movieName[0] = 0;
	
	movieObject = nullptr;
	movieTexture = nullptr;
	
	SetWidgetUsage(kWidgetGeneratedImage);
	SetImageBlendState(kBlendReplace);
}

MovieWidget::MovieWidget(const Vector2D& size, const char *name, unsigned_int32 loadFlags) : ImageWidget(kWidgetMovie, size)
{
	movieFlags = 0;
	movieLoadFlags = loadFlags;
	movieName = name;
	
	movieObject = nullptr;
	movieTexture = nullptr;
	
	SetWidgetUsage(kWidgetGeneratedImage);
	SetImageBlendState(kBlendReplace);
}

MovieWidget::MovieWidget(const Vector2D& size, C4::Movie *movie) : ImageWidget(kWidgetMovie, size)
{
	movieFlags = 0;
	movieLoadFlags = 0;
	movieName[0] = 0;
	
	movie->Retain();
	movieObject = movie;
	movieTexture = MoviePlugin::NewMovieTexture(movie);
	SetTexture(0, movieTexture);
	
	SetWidgetUsage(kWidgetGeneratedImage);
	SetImageBlendState(kBlendReplace);
}

MovieWidget::MovieWidget(const MovieWidget& movieWidget) : ImageWidget(movieWidget)
{
	movieFlags = movieWidget.movieFlags;
	movieLoadFlags = movieWidget.movieLoadFlags;
	movieName = movieWidget.movieName;
	
	movieObject = nullptr;
	movieTexture = nullptr;
}

MovieWidget::~MovieWidget()
{
	if (movieObject) movieObject->Release();
	if (movieTexture) MoviePlugin::ReleaseMovieTexture(movieTexture);
}

Widget *MovieWidget::Replicate(void) const
{
	return (new MovieWidget(*this));
}

void MovieWidget::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ImageWidget::Pack(data, packFlags);
	
	data << ChunkHeader('FLAG', 4);
	data << movieFlags;
	
	data << ChunkHeader('LFLG', 4);
	data << movieLoadFlags;
	
	PackHandle handle = data.BeginChunk('MOOV');
	data << movieName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void MovieWidget::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ImageWidget::Unpack(data, unpackFlags);
	
	#if C4LEGACY
	
		if (data.GetVersion() >= 25) UnpackChunkList<MovieWidget>(data, unpackFlags);
	
	#else
	
		UnpackChunkList<MovieWidget>(data, unpackFlags);
	
	#endif
}

bool MovieWidget::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> movieFlags;
			return (true);
		
		case 'LFLG':
			
			data >> movieLoadFlags;
			return (true);
		
		case 'MOOV':
			
			data >> movieName;
			return (true);
	}
	
	return (false);
}

int32 MovieWidget::GetSettingCount(void) const
{
	return (ImageWidget::GetSettingCount() + 5);
}

Setting *MovieWidget::GetSetting(int32 index) const
{
	int32 count = ImageWidget::GetSettingCount();
	if (index < count) return (ImageWidget::GetSetting(index));
	
	const StringTable *table = TheMoviePlugin->GetStringTable();
	
	if (index == count)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'SETT'));
		return (new HeadingSetting(kWidgetMovie, title));
	}
	
	if (index == count + 1)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'MNAM'));
		const char *picker = table->GetString(StringID('WDGT', kWidgetMovie, 'MPCK'));
		return (new ResourceSetting('MNAM', movieName, title, picker, MovieResource::GetDescriptor(), nullptr, kResourceSettingGenericPath));
	}
	
	if (index == count + 2)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'RURL'));
		return (new BooleanSetting('RURL', ((movieLoadFlags & kMovieRemoteURL) != 0), title));
	}
	
	if (index == count + 3)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'PLAY'));
		return (new BooleanSetting('PLAY', ((movieFlags & kMovieInitialPlay) != 0), title));
	}
	
	if (index == count + 4)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'LOOP'));
		return (new BooleanSetting('LOOP', ((movieFlags & kMovieLoop) != 0), title));
	}
	
	return (nullptr);
}

void MovieWidget::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'MNAM')
	{
		movieName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
	}
	else if (identifier == 'RURL')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) movieLoadFlags |= kMovieRemoteURL;
		else movieLoadFlags &= ~kMovieRemoteURL;
	}
	else if (identifier == 'PLAY')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) movieFlags |= kMovieInitialPlay;
		else movieFlags &= ~kMovieInitialPlay;
	}
	else if (identifier == 'LOOP')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) movieFlags |= kMovieLoop;
		else movieFlags &= ~kMovieLoop;
	}
	else
	{
		ImageWidget::SetSetting(setting);
	}
}

void MovieWidget::SendInitialStateMessages(Player *player) const
{
	Movie *movieObject = GetMovie();
	if (movieObject)
	{
		SetMovieTimeWidgetFunction function(movieObject->GetMovieTime());
		player->SendMessage(FunctionMessage(GetPanelController()->GetControllerIndex(), &function));
	}
}

void MovieWidget::Preprocess(void)
{
	ImageWidget::Preprocess();
	
	if (!GetManipulator())
	{
		const PanelController *controller = GetPanelController();
		if ((!controller) || (!controller->GetTargetNode()->GetManipulator()))
		{
			if ((!movieObject) && (movieName[0] != 0))
			{
				SetMovie(movieName, movieLoadFlags);
				if (movieObject)
				{
					unsigned_int32 flags = movieFlags;
					if (flags & kMovieLoop) movieObject->SetLoopCount(kMovieLoopInfinite);
					if (flags & kMovieInitialPlay) movieObject->Play();
				}
			}
		}
	}
	
	if (!movieObject) SetTexture(0, "C4/checker");
}

void MovieWidget::SetMovie(const char *name, unsigned_int32 loadFlags)
{
	if (TheMoviePlugin->Initialize() == kMovieOkay)
	{
		if (movieObject) movieObject->Release();
		if (movieTexture) MoviePlugin::ReleaseMovieTexture(movieTexture);
		
		movieObject = nullptr;
		movieTexture = nullptr;
		
		Movie *movie = new Movie;
		if (movie->Load(name, loadFlags) == kMovieOkay)
		{
			movieObject = movie;
			movieTexture = MoviePlugin::NewMovieTexture(movie);
			SetTexture(0, movieTexture);
		}
	}
}

void MovieWidget::Build(void)
{
	if (movieObject)
	{
		SetImageOffset(Vector2D(0.0F, 1.0F));
		SetImageScale(Vector2D(1.0F, -1.0F));
	}
	else
	{
		const Vector2D& size = GetWidgetSize();
		SetImageScale(Vector2D(size.x * 0.03125F, size.y * 0.03125F));
	}
	
	ImageWidget::Build();
}

void MovieWidget::Render(List<Renderable> *renderList)
{
	if ((movieObject) && (movieObject->Playing()) && (movieObject->Updated()))
	{
		int32 width = movieObject->GetMovieWidth();
		int32 height = movieObject->GetMovieHeight();
		if ((width != 0) && (height != 0)) GetTexture()->Update(Rect(0, 0, width, height));
	}
	
	ImageWidget::Render(renderList);
}


PlayMovieWidgetFunction::PlayMovieWidgetFunction() : WidgetFunction(kFunctionPlayMovieWidget)
{
}

PlayMovieWidgetFunction::PlayMovieWidgetFunction(const PlayMovieWidgetFunction& playMovieWidgetFunction) : WidgetFunction(playMovieWidgetFunction)
{
}

PlayMovieWidgetFunction::~PlayMovieWidgetFunction()
{
}

Function *PlayMovieWidgetFunction::Replicate(void) const
{
	return (new PlayMovieWidgetFunction(*this));
}

bool PlayMovieWidgetFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	if ((type == kFunctionPlayMovieWidget) || (type == kFunctionStopMovieWidget) || (type == kFunctionPauseMovieWidget) || (type == kFunctionResumeMovieWidget))
	{
		return (static_cast<const WidgetFunction *>(function)->GetWidgetKey() == GetWidgetKey());
	}
	
	return (false);
}

void PlayMovieWidgetFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetMovie)
		{
			Movie *movie = static_cast<MovieWidget *>(widget)->GetMovie();
			if (movie) movie->Play();
		}
		
		widget = widget->GetNextWidgetWithSameKey();
	}
	
	CallCompletionProc();
}


StopMovieWidgetFunction::StopMovieWidgetFunction() : WidgetFunction(kFunctionStopMovieWidget)
{
}

StopMovieWidgetFunction::StopMovieWidgetFunction(const StopMovieWidgetFunction& stopMovieWidgetFunction) : WidgetFunction(stopMovieWidgetFunction)
{
}

StopMovieWidgetFunction::~StopMovieWidgetFunction()
{
}

Function *StopMovieWidgetFunction::Replicate(void) const
{
	return (new StopMovieWidgetFunction(*this));
}

bool StopMovieWidgetFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	if ((type == kFunctionPlayMovieWidget) || (type == kFunctionStopMovieWidget) || (type == kFunctionPauseMovieWidget) || (type == kFunctionResumeMovieWidget))
	{
		return (static_cast<const WidgetFunction *>(function)->GetWidgetKey() == GetWidgetKey());
	}
	
	return (false);
}

void StopMovieWidgetFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetMovie)
		{
			Movie *movie = static_cast<MovieWidget *>(widget)->GetMovie();
			if (movie) movie->Stop();
		}
		
		widget = widget->GetNextWidgetWithSameKey();
	}
	
	CallCompletionProc();
}


PauseMovieWidgetFunction::PauseMovieWidgetFunction() : WidgetFunction(kFunctionPauseMovieWidget)
{
}

PauseMovieWidgetFunction::PauseMovieWidgetFunction(const PauseMovieWidgetFunction& pauseMovieWidgetFunction) : WidgetFunction(pauseMovieWidgetFunction)
{
}

PauseMovieWidgetFunction::~PauseMovieWidgetFunction()
{
}

Function *PauseMovieWidgetFunction::Replicate(void) const
{
	return (new PauseMovieWidgetFunction(*this));
}

bool PauseMovieWidgetFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	if ((type == kFunctionPauseMovieWidget) || (type == kFunctionResumeMovieWidget))
	{
		return (static_cast<const WidgetFunction *>(function)->GetWidgetKey() == GetWidgetKey());
	}
	
	return (false);
}

void PauseMovieWidgetFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetMovie)
		{
			Movie *movie = static_cast<MovieWidget *>(widget)->GetMovie();
			if (movie) movie->Pause();
		}
		
		widget = widget->GetNextWidgetWithSameKey();
	}
	
	CallCompletionProc();
}


ResumeMovieWidgetFunction::ResumeMovieWidgetFunction() : WidgetFunction(kFunctionResumeMovieWidget)
{
}

ResumeMovieWidgetFunction::ResumeMovieWidgetFunction(const ResumeMovieWidgetFunction& resumeMovieWidgetFunction) : WidgetFunction(resumeMovieWidgetFunction)
{
}

ResumeMovieWidgetFunction::~ResumeMovieWidgetFunction()
{
}

Function *ResumeMovieWidgetFunction::Replicate(void) const
{
	return (new ResumeMovieWidgetFunction(*this));
}

bool ResumeMovieWidgetFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	if ((type == kFunctionPauseMovieWidget) || (type == kFunctionResumeMovieWidget))
	{
		return (static_cast<const WidgetFunction *>(function)->GetWidgetKey() == GetWidgetKey());
	}
	
	return (false);
}

void ResumeMovieWidgetFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetMovie)
		{
			Movie *movie = static_cast<MovieWidget *>(widget)->GetMovie();
			if (movie) movie->Resume();
		}
		
		widget = widget->GetNextWidgetWithSameKey();
	}
	
	CallCompletionProc();
}


SetMovieTimeWidgetFunction::SetMovieTimeWidgetFunction(float time) : WidgetFunction(kFunctionSetMovieWidgetTime)
{
	movieTime = time;
}

SetMovieTimeWidgetFunction::SetMovieTimeWidgetFunction(const SetMovieTimeWidgetFunction& setMovieTimeWidgetFunction) : WidgetFunction(setMovieTimeWidgetFunction)
{
	movieTime = setMovieTimeWidgetFunction.movieTime;
}

SetMovieTimeWidgetFunction::~SetMovieTimeWidgetFunction()
{
}

Function *SetMovieTimeWidgetFunction::Replicate(void) const
{
	return (new SetMovieTimeWidgetFunction(*this));
}

void SetMovieTimeWidgetFunction::Pack(Packer& data, unsigned_int32 packFlags) const
{
	WidgetFunction::Pack(data, packFlags);
	
	data << movieTime;
}

void SetMovieTimeWidgetFunction::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	WidgetFunction::Unpack(data, unpackFlags);
	
	data >> movieTime;
}

void SetMovieTimeWidgetFunction::Compress(Compressor& data) const
{
	WidgetFunction::Compress(data);
	
	data << movieTime;
}

bool SetMovieTimeWidgetFunction::Decompress(Decompressor& data)
{
	if (WidgetFunction::Decompress(data))
	{
		data >> movieTime;
		return (true);
	}
	
	return (false);
}

int32 SetMovieTimeWidgetFunction::GetSettingCount(void) const
{
	return (WidgetFunction::GetSettingCount() + 1);
}

Setting *SetMovieTimeWidgetFunction::GetSetting(int32 index) const
{
	int32 count = WidgetFunction::GetSettingCount();
	if (index < count) return (WidgetFunction::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheMoviePlugin->GetStringTable();
		const char *title = table->GetString(StringID('FUNC', kFunctionSetMovieWidgetTime, 'TIME'));
		return (new TextSetting('TIME', movieTime * 0.001F, title));
	}
	
	return (nullptr);
}

void SetMovieTimeWidgetFunction::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'TIME')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		movieTime = Text::StringToFloat(text) * 1000.0F;
	}
	else
	{
		WidgetFunction::SetSetting(setting);
	}
}

void SetMovieTimeWidgetFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetMovie)
		{
			Movie *movie = static_cast<MovieWidget *>(widget)->GetMovie();
			if (movie) movie->SetMovieTime(movieTime);
		}
		
		widget = widget->GetNextWidgetWithSameKey();
	}
	
	CallCompletionProc();
}


C4::MovieController::MovieController() : Controller(kControllerMovie)
{
	movieName[0] = 0;
	movieFlags = 0;
	movieLoadFlags = 0;
	attributeType = kAttributeEmissionMap;
	
	movieObject = nullptr;
	movieAttribute = nullptr;
	movieTexture = nullptr;
}

C4::MovieController::MovieController(const char *name, unsigned_int32 loadFlags) : Controller(kControllerMovie)
{
	movieName = name;
	movieFlags = 0;
	movieLoadFlags = loadFlags;
	attributeType = kAttributeEmissionMap;
	
	movieObject = nullptr;
	movieAttribute = nullptr;
	movieTexture = nullptr;
}

C4::MovieController::MovieController(const C4::MovieController& movieController) : Controller(movieController)
{
	movieName = movieController.movieName;
	movieFlags = movieController.movieFlags;
	movieLoadFlags = movieController.movieLoadFlags;
	attributeType = movieController.attributeType;
	
	movieObject = nullptr;
	movieAttribute = nullptr;
	movieTexture = nullptr;
}

C4::MovieController::~MovieController()
{
	delete movieAttribute;
	if (movieObject) movieObject->Release();
	if (movieTexture) MoviePlugin::ReleaseMovieTexture(movieTexture);
}

Controller *C4::MovieController::Replicate(void) const
{
	return (new MovieController(*this));
}

bool C4::MovieController::ValidNode(const Node *node)
{
	return (node->GetNodeType() == kNodeGeometry);
}

void C4::MovieController::RegisterFunctions(ControllerRegistration *registration)
{
	const StringTable *table = TheMoviePlugin->GetStringTable();
	
	static FunctionReg<PlayMovieFunction> playMovieRegistration(registration, kFunctionPlayMovie, table->GetString(StringID('CTRL', kControllerMovie, 'FUNC', kFunctionPlayMovie)));
	static FunctionReg<StopMovieFunction> stopMovieRegistration(registration, kFunctionStopMovie, table->GetString(StringID('CTRL', kControllerMovie, 'FUNC', kFunctionStopMovie)));
}

void C4::MovieController::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Controller::Pack(data, packFlags);
	
	PackHandle handle = data.BeginChunk('NAME');
	data << movieName;
	data.EndChunk(handle);
	
	data << ChunkHeader('FLAG', 4);
	data << movieFlags;
	
	data << ChunkHeader('LFLG', 4);
	data << movieLoadFlags;
	
	data << ChunkHeader('ATTR', 4);
	data << attributeType;
	
	data << TerminatorChunk;
}

void C4::MovieController::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Controller::Unpack(data, unpackFlags);
	UnpackChunkList<MovieController>(data, unpackFlags);
}

bool C4::MovieController::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'NAME':
			
			data >> movieName;
			return (true);
		
		case 'FLAG':
			
			data >> movieFlags;
			return (true);
		
		case 'LFLG':
			
			data >> movieLoadFlags;
			return (true);
		
		case 'ATTR':
			
			data >> attributeType;
			return (true);
	}
	
	return (false);
}

int32 C4::MovieController::GetSettingCount(void) const
{
	return (5);
}

Setting *C4::MovieController::GetSetting(int32 index) const
{
	const StringTable *table = TheMoviePlugin->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerMovie, 'MNAM'));
		const char *picker = table->GetString(StringID('CTRL', kControllerMovie, 'MPCK'));
		return (new ResourceSetting('MNAM', movieName, title, picker, MovieResource::GetDescriptor(), nullptr, kResourceSettingGenericPath));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerMovie, 'RURL'));
		return (new BooleanSetting('RURL', ((movieLoadFlags & kMovieRemoteURL) != 0), title));
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerMovie, 'PLAY'));
		return (new BooleanSetting('PLAY', ((movieFlags & kMovieInitialPlay) != 0), title));
	}
	
	if (index == 3)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerMovie, 'LOOP'));
		return (new BooleanSetting('LOOP', ((movieFlags & kMovieLoop) != 0), title));
	}
	
	if (index == 4)
	{
		int32 selection = 3;
		if (attributeType == kAttributeTextureMap) selection = 0;
		else if (attributeType == kAttributeNormalMap) selection = 1;
		else if (attributeType == kAttributeGlossMap) selection = 2;
		
		const char *title = table->GetString(StringID('CTRL', kControllerMovie, 'ATTR'));
		MenuSetting *menu = new MenuSetting('ATTR', selection, title, 4);
		
		menu->SetMenuItemString(0, table->GetString(StringID('CTRL', kControllerMovie, 'ATTR', 'TEXT')));
		menu->SetMenuItemString(1, table->GetString(StringID('CTRL', kControllerMovie, 'ATTR', 'NRML')));
		menu->SetMenuItemString(2, table->GetString(StringID('CTRL', kControllerMovie, 'ATTR', 'GLOS')));
		menu->SetMenuItemString(3, table->GetString(StringID('CTRL', kControllerMovie, 'ATTR', 'EMIS')));
		
		return (menu);
	}
	
	return (nullptr);
}

void C4::MovieController::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'MNAM')
	{
		movieName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
	}
	else if (identifier == 'RURL')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) movieLoadFlags |= kMovieRemoteURL;
		else movieLoadFlags &= ~kMovieRemoteURL;
	}
	else if (identifier == 'PLAY')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) movieFlags |= kMovieInitialPlay;
		else movieFlags &= ~kMovieInitialPlay;
	}
	else if (identifier == 'LOOP')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) movieFlags |= kMovieLoop;
		else movieFlags &= ~kMovieLoop;
	}
	else if (identifier == 'ATTR')
	{
		int32 selection = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
		if (selection == 0) attributeType = kAttributeTextureMap;
		else if (selection == 1) attributeType = kAttributeNormalMap;
		else if (selection == 2) attributeType = kAttributeGlossMap;
		else attributeType = kAttributeEmissionMap;
	}
}

ControllerMessage *C4::MovieController::ConstructMessage(ControllerMessageType type) const
{
	switch (type)
	{
		case kMovieMessagePlay:
		case kMovieMessageStop:
			
			return (new MoviePlayStopMessage(type, GetControllerIndex()));
		
		case kMovieMessageTime:
			
			return (new MovieTimeMessage(GetControllerIndex()));
	}
	
	return (Controller::ConstructMessage(type));
}

void C4::MovieController::ReceiveMessage(const ControllerMessage *message)
{
	switch (message->GetControllerMessageType())
	{
		case kMovieMessagePlay:
			
			if (movieObject) movieObject->Play();
			break;
		
		case kMovieMessageStop:
			
			if (movieObject) movieObject->Stop();
			break;
		
		case kMovieMessageTime:
			
			if (movieObject) movieObject->SetMovieTime(static_cast<const MovieTimeMessage *>(message)->GetMovieTime());
			break;
		
		default:
			
			Controller::ReceiveMessage(message);
			break;
	}
}

void C4::MovieController::SendInitialStateMessages(Player *player) const
{
	if (movieObject) player->SendMessage(MovieTimeMessage(GetControllerIndex(), movieObject->GetMovieTime()));
}

void C4::MovieController::Preprocess(void)
{
	Controller::Preprocess();
	
	Geometry *geometry = static_cast<Geometry *>(GetTargetNode());
	if (!geometry->GetManipulator())
	{
		delete movieAttribute;
		if (movieObject) movieObject->Release();
		if (movieTexture) MoviePlugin::ReleaseMovieTexture(movieTexture);
		
		movieAttribute = nullptr;
		movieObject = nullptr;
		movieTexture = nullptr;
		
		if (TheMoviePlugin->Initialize() == kMovieOkay)
		{
			Movie *movie = new Movie;
			if (movie->Load(movieName, movieLoadFlags) == kMovieOkay)
			{
				movieObject = movie;
				
				movieTexture = MoviePlugin::NewMovieTexture(movie);
				switch (attributeType)
				{
					case kAttributeTextureMap:
						
						movieAttribute = new TextureMapAttribute(movieTexture);
						break;
					
					case kAttributeNormalMap:
						
						movieAttribute = new NormalMapAttribute(movieTexture);
						break;
					
					case kAttributeGlossMap:
						
						movieAttribute = new GlossMapAttribute(movieTexture);
						break;
					
					default:
						
						movieAttribute = new EmissionMapAttribute(movieTexture);
						break;
				}
				
				attributeList.Append(movieAttribute);
				geometry->SetMaterialAttributeList(&attributeList);
				
				unsigned_int32 flags = movieFlags;
				if (flags & kMovieLoop) movieObject->SetLoopCount(kMovieLoopInfinite);
				if (flags & kMovieInitialPlay) movieObject->Play();
			}
		}
	}
}

void C4::MovieController::Move(void)
{
	if ((movieObject) && (movieObject->Playing()) && (movieObject->Updated())) Invalidate();
}

void C4::MovieController::Update(void)
{
	Controller::Update();
	
	if (movieObject)
	{
		int32 width = movieObject->GetMovieWidth();
		int32 height = movieObject->GetMovieHeight();
		if ((width != 0) && (height != 0)) movieAttribute->GetTexture()->Update(Rect(0, 0, width, height));
	}
}

void C4::MovieController::Activate(Node *trigger, Node *activator)
{
	if (movieObject) movieObject->Play();
}


PlayMovieFunction::PlayMovieFunction() : Function(kFunctionPlayMovie, kControllerMovie)
{
}

PlayMovieFunction::PlayMovieFunction(const PlayMovieFunction& playMovieFunction) : Function(playMovieFunction)
{
}

PlayMovieFunction::~PlayMovieFunction()
{
}

Function *PlayMovieFunction::Replicate(void) const
{
	return (new PlayMovieFunction(*this));
}

void PlayMovieFunction::MovieComplete(C4::Movie *movie, void *cookie)
{
	movie->SetCompletionProc(nullptr);
	static_cast<PlayMovieFunction *>(cookie)->CallCompletionProc();
}

void PlayMovieFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	MovieController *movieController = static_cast<MovieController *>(controller);
	Movie *movie = movieController->GetMovie();
	if ((movie) && (!movie->Playing()))
	{
		if (movieController->GetMovieFlags() & kMovieLoop)
		{
			TheMessageMgr->SendMessageJournal(new MoviePlayStopMessage(MovieController::kMovieMessagePlay, movieController->GetControllerIndex()));
		}
		else
		{
			movie->SetCompletionProc(&MovieComplete, this);
			TheMessageMgr->SendMessageAll(MoviePlayStopMessage(MovieController::kMovieMessagePlay, movieController->GetControllerIndex()));
			return;
		}
	}
	
	CallCompletionProc();
}


StopMovieFunction::StopMovieFunction() : Function(kFunctionStopMovie, kControllerMovie)
{
}

StopMovieFunction::StopMovieFunction(const StopMovieFunction& stopMovieFunction) : Function(stopMovieFunction)
{
}

StopMovieFunction::~StopMovieFunction()
{
}

Function *StopMovieFunction::Replicate(void) const
{
	return (new StopMovieFunction(*this));
}

void StopMovieFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	Movie *movie = static_cast<MovieController *>(controller)->GetMovie();
	if (movie)
	{
		TheMessageMgr->SendMessageJournal(new MoviePlayStopMessage(MovieController::kMovieMessageStop, controller->GetControllerIndex()));
		movie->CallCompletionProc();
	}
	
	CallCompletionProc();
}


MoviePlayStopMessage::MoviePlayStopMessage(ControllerMessageType type, int32 controllerIndex) : ControllerMessage(type, controllerIndex)
{
}

MoviePlayStopMessage::~MoviePlayStopMessage()
{
}

bool MoviePlayStopMessage::OverridesMessage(const ControllerMessage *message) const
{
	ControllerMessageType type = message->GetControllerMessageType();
	return ((type == MovieController::kMovieMessagePlay) || (type == MovieController::kMovieMessageStop));
}


MovieTimeMessage::MovieTimeMessage(int32 controllerIndex) : ControllerMessage(MovieController::kMovieMessageTime, controllerIndex)
{
}

MovieTimeMessage::MovieTimeMessage(int32 controllerIndex, float time) : ControllerMessage(MovieController::kMovieMessageTime, controllerIndex)
{
	movieTime = time;
}

MovieTimeMessage::~MovieTimeMessage()
{
}

void MovieTimeMessage::Compress(Compressor& data) const
{
	ControllerMessage::Compress(data);
	
	data << movieTime;
}

bool MovieTimeMessage::Decompress(Decompressor& data)
{
	if (ControllerMessage::Decompress(data))
	{
		data >> movieTime;
		return (true);
	}
	
	return (false);
}


MoviePlugin::MoviePlugin() :
		Singleton<MoviePlugin>(TheMoviePlugin),
		stringTable("Movies/strings"),
		movieWidgetReg(kWidgetMovie, stringTable.GetString(StringID('WDGT', kWidgetMovie)), "Movies/Movie"),
		movieControllerReg(kControllerMovie, stringTable.GetString(StringID('CTRL', kControllerMovie)))
{
	quicktimeActive = false;
	
	MovieController::RegisterFunctions(&movieControllerReg);
	
	ControllerRegistration *registration = Controller::FindRegistration(kControllerPanel);
	static FunctionReg<PlayMovieWidgetFunction> playMovieWidgetRegistration(registration, kFunctionPlayMovieWidget, stringTable.GetString(StringID('FUNC', kFunctionPlayMovieWidget)), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<StopMovieWidgetFunction> stopMovieWidgetRegistration(registration, kFunctionStopMovieWidget, stringTable.GetString(StringID('FUNC', kFunctionStopMovieWidget)), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<PauseMovieWidgetFunction> pauseMovieWidgetRegistration(registration, kFunctionPauseMovieWidget, stringTable.GetString(StringID('FUNC', kFunctionPauseMovieWidget)), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<ResumeMovieWidgetFunction> resumeMovieWidgetRegistration(registration, kFunctionResumeMovieWidget, stringTable.GetString(StringID('FUNC', kFunctionResumeMovieWidget)), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<SetMovieTimeWidgetFunction> setMovieWidgetTimeRegistration(registration, kFunctionSetMovieWidgetTime, stringTable.GetString(StringID('FUNC', kFunctionSetMovieWidgetTime)), kFunctionRemote);
	
	Movie::movieDrawUPP = NewMovieDrawingCompleteUPP(&Movie::MovieDrawProc);
	Movie::prerollCompleteUPP = NewMoviePrePrerollCompleteUPP(&Movie::PrerollCompleteProc);
}

MoviePlugin::~MoviePlugin()
{
	movieList.Purge();
	Terminate();
	
	DisposeMoviePrePrerollCompleteUPP(Movie::prerollCompleteUPP);
	DisposeMovieDrawingCompleteUPP(Movie::movieDrawUPP);
}

MovieResult MoviePlugin::Initialize(void)
{
	if (!quicktimeActive)
	{
		#if C4WINDOWS
			
			if (InitializeQTML(0) != QT::noErr) return (kMovieInitFailed);
		
		#endif
		
		if (EnterMovies() != QT::noErr)
		{
			#if C4WINDOWS
			
				TerminateQTML();
			
			#endif
			
			return (kMovieInitFailed);
		}
		
		quicktimeActive = true;
	}
	
	return (kMovieOkay);
}

void MoviePlugin::Terminate(void)
{
	if (quicktimeActive)
	{
		quicktimeActive = false;
		ExitMovies();
		
		#if C4WINDOWS
		
			TerminateQTML();
		
		#endif
	}
}

TextureHeader *MoviePlugin::NewMovieTexture(C4::Movie *movie)
{
	int32 movieWidth = movie->GetMovieWidth();
	int32 movieHeight = movie->GetMovieHeight();
	int32 movieArea = movieWidth * movieHeight;
	
	unsigned_int32 size = sizeof(TextureHeader) + sizeof(TextureMipmapData) + movieArea * sizeof(Color4C);
	char *storage = new char[size];
	
	TextureHeader *textureHeader = reinterpret_cast<TextureHeader *>(storage);
	textureHeader->textureType = kTextureRectangle;
	textureHeader->textureFlags = 0;
	textureHeader->colorSemantic = kTextureSemanticNone;
	textureHeader->alphaSemantic = kTextureSemanticTransparency;
	
	#if C4WINDOWS
	
		textureHeader->imageFormat = kTextureRGBA8;
	
	#elif C4MACOS
	
		textureHeader->imageFormat = kTextureARGB8;
	
	#endif
	
	textureHeader->imageWidth = movieWidth;
	textureHeader->imageHeight = movieHeight;
	textureHeader->imageDepth = 1;
	textureHeader->wrapMode[0] = kTextureClamp;
	textureHeader->wrapMode[1] = kTextureClamp;
	textureHeader->wrapMode[2] = kTextureClamp;
	textureHeader->mipmapCount = 1;
	textureHeader->mipmapDataOffset = sizeof(TextureHeader);
	textureHeader->auxiliaryDataSize = 0;
	textureHeader->auxiliaryDataOffset = 0;
	
	TextureMipmapData *mipmapData = textureHeader->GetMipmapData();
	mipmapData->imageOffset = sizeof(TextureMipmapData);
	mipmapData->imageSize = movieArea * sizeof(Color4C);
	mipmapData->chainSize = movieArea * sizeof(Color4C);
	mipmapData->compressionType = kTextureCompressionNone;
	
	Color4C *image = static_cast<Color4C *>(mipmapData->GetMipmapImage());
	MemoryMgr::ClearMemory(image, movieArea * sizeof(Color4C));
	movie->SetPixelMap(image, movieWidth, movieHeight, movieWidth);
	
	return (textureHeader);
}

void MoviePlugin::ReleaseMovieTexture(TextureHeader *textureHeader)
{
	delete[] reinterpret_cast<char *>(textureHeader);
}

void MoviePlugin::PluginTask(void)
{
	int32 taskCount = 0;
	
	Movie *movie = movieList.First();
	while (movie)
	{
		unsigned_int32 status = movie->playStatus;
		if (status == kMoviePlaying)
		{
			movie->movieState &= ~kMovieUpdated;
			if (!movie->Paused()) taskCount++;
		}
		else if (status == kMoviePrerolling)
		{
			taskCount++;
		}
		
		movie = movie->Next();
	}
	
	if (taskCount != 0)
	{
		MoviesTask(nullptr, 0);
		
		movie = movieList.First();
		while (movie)
		{
			Movie *next = movie->Next();
			if (movie->playStatus == kMoviePlaying) movie->MovieTask();
			movie = next;
		}
	}
}

// ZYURVUR
