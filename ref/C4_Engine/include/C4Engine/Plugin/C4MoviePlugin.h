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


#ifndef C4MoviePlugin_h
#define C4MoviePlugin_h


//# \component	Movies Plugin
//# \prefix		MoviesPlugin/


#include "C4Plugins.h"
#include "C4Panels.h"
#include "C4Image.h"
#include "C4QuickTime.h"


#ifdef C4MOVIES

	#define C4MOVIEAPI C4MODULEEXPORT
	
	
	extern "C"
	{
		C4MODULEEXPORT C4::Plugin *ConstructPlugin(void);
	}

#else

	#define C4MOVIEAPI C4MODULEIMPORT

#endif


namespace C4
{
	typedef EngineResult MovieResult;
	
	
	enum
	{
		kMovieStopped			= 0,
		kMoviePrerolling		= 1,
		kMoviePlaying			= 2,
		kMoviePaused			= 3
	};
	
	
	enum
	{
		kMovieLoopInfinite		= -1
	};
	
	
	enum
	{
		kManagerMovie			= 'MV',
		
		kMovieOkay				= kEngineOkay,
		kMovieInitFailed		= (kManagerMovie << 16) | 0x0001,
		kMovieLoadFailed		= (kManagerMovie << 16) | 0x0002
	};
	
	
	enum
	{
		kWidgetMovie			= 'moov'
	};
	
	
	enum
	{
		kControllerMovie		= 'moov'
	};
	
	
	enum
	{
		kFunctionPlayMovieWidget		= 'plym',
		kFunctionStopMovieWidget		= 'stpm',
		kFunctionPauseMovieWidget		= 'pasm',
		kFunctionResumeMovieWidget		= 'resm',
		kFunctionSetMovieWidgetTime		= 'mtim'
	};
	
	
	enum
	{
		kFunctionPlayMovie		= 'play',
		kFunctionStopMovie		= 'stop'
	};
	
	
	//# \enum	MovieFlags
	
	enum
	{
		kMovieInitialPlay		= 1 << 0,		//## Movie is initially playing.
		kMovieLoop				= 1 << 1		//## Movie loops when it reaches the end.
	};
	
	
	//# \enum	MovieLoadFlags
	
	enum 
	{
		kMovieRemoteURL			= 1 << 0,		//## The movie name represents a URL to a remote resource. 
		kMovieAbsolutePath		= 1 << 1		//## The movie name represents an absolute path name. The $kMovieRemoteURL$ flag is ignored if this flag is specified. 
	}; 
	
	 
	enum
	{
		kMovieLoaded			= 1 << 0,
		kMovieStreaming			= 1 << 1, 
		kMovieUpdated			= 1 << 2
	};
	
	 
	class MovieResource : public Resource<MovieResource>
	{
		friend class Resource<MovieResource>;
		
		private:
			
			static C4MOVIEAPI ResourceDescriptor	descriptor;
			
			~MovieResource();
		
		public:
			
			MovieResource(const char *name, ResourceCatalog *catalog);
	};
	
	
	//# \class	Movie		Encapsulates functionality for a movie.
	//
	//# \def	class Movie : public ListElement<Movie>, public Completable<Movie>
	//
	//# \ctor	Movie();
	//
	//# \desc
	//# 
	//
	//# \base	Utilities/ListElement<Movie>	Used internally by the Movie Manager.
	//# \base	Utilities/Completable<Movie>	The completion procedure is called when the movie finishes playing.
	
	
	//# \function	Movie::Load		Loads a movie.
	//
	//# \proto	MovieResult Load(const char *name);
	//
	//# \param	name	The name of the movie resource, including the filename extension.
	//
	//# \desc
	//
	
	
	//# \function	Movie::Play		Plays a movie.
	//
	//# \proto	void Play(void);
	//
	//# \desc
	//
	//# \also		$@Movie::Stop@$
	//# \also		$@Movie::Pause@$
	
	
	//# \function	Movie::Stop		Stops a movie.
	//
	//# \proto	void Stop(void);
	//
	//# \desc
	//
	//# \also		$@Movie::Play@$
	//# \also		$@Movie::Pause@$
	
	
	//# \function	Movie::Pause	Pauses a movie.
	//
	//# \proto	void Pause(void);
	//
	//# \desc
	//
	//# \also		$@Movie::Resume@$
	//# \also		$@Movie::Stop@$
	
	
	//# \function	Movie::Resume	Resumes a movie.
	//
	//# \proto	void Resume(void);
	//
	//# \desc
	//
	//# \also		$@Movie::Pause@$
	
	
	//# \div
	//# \function	Movie::GetMovieWidth		Returns the width of a movie.
	//
	//# \proto	int32 GetMovieWidth(void) const;
	//
	//# \desc
	//
	//# \also	$@Movie::GetMovieHeight@$
	
	
	//# \function	Movie::GetMovieHeight		Returns the height of a movie.
	//
	//# \proto	int32 GetMovieHeight(void) const;
	//
	//# \desc
	//
	//# \also	$@Movie::GetMovieWidth@$
	
	
	class Movie : public Shared, public ListElement<Movie>, public Completable<Movie>
	{
		friend class MoviePlugin;
		
		private:
			
			QT::Movie			qtMovie;
			
			QT::CGrafPtr		movieWorld;
			int32				movieWidth;
			int32				movieHeight;
			
			unsigned_int32		movieState;
			unsigned_int32		playStatus;
			int32				loopCount;
			float				movieVolume;
			
			static QT::MovieDrawingCompleteUPP		movieDrawUPP;
			static QT::MoviePrePrerollCompleteUPP	prerollCompleteUPP;
			
			~Movie();
			
			static QT::OSErr QTCallback MovieDrawProc(QT::Movie qtMovie, long data);
			static void QTCallback PrerollCompleteProc(QT::Movie qtMovie, QT::OSErr err, void *cookie);
			
			void Start(void);
			void MovieTask(void);
		
		public:
			
			C4MOVIEAPI Movie();
			
			int32 GetMovieWidth(void) const
			{
				return (movieWidth);
			}
			
			int32 GetMovieHeight(void) const
			{
				return (movieHeight);
			}
			
			bool Playing(void) const
			{
				return (playStatus >= kMoviePlaying);
			}
			
			bool Paused(void) const
			{
				return (playStatus == kMoviePaused);
			}
			
			int32 GetLoopCount(void) const
			{
				return (loopCount);
			}
			
			void SetLoopCount(int32 count)
			{
				loopCount = count;
			}
			
			float GetVolume(void) const
			{
				return (movieVolume);
			}
			
			bool Updated(void) const
			{
				return ((movieState & kMovieUpdated) != 0);
			}
			
			C4MOVIEAPI MovieResult Load(const char *name, unsigned_int32 flags = 0);
			
			C4MOVIEAPI void SetPixelMap(Color4C *image, int32 width, int32 height, int32 rowPixels);
			C4MOVIEAPI void SetVolume(float volume);
			
			C4MOVIEAPI float GetDuration(void) const;
			C4MOVIEAPI float GetMovieTime(void) const;
			C4MOVIEAPI void SetMovieTime(float time);
			
			C4MOVIEAPI void Play(void);
			C4MOVIEAPI void Stop(void);
			
			C4MOVIEAPI void Pause(void);
			C4MOVIEAPI void Resume(void);
	};
	
	
	//# \class	MovieWidget		The interface widget that displays a movie.
	//
	//# The $MovieWidget$ class represents an interface widget that displays a movie.
	//
	//# \def	class MovieWidget : public ImageWidget
	//
	//# \ctor	MovieWidget(const Vector2D& size, const char *name, unsigned_int32 loadFlags = 0);
	//# \ctor	MovieWidget(const Vector2D& size, C4::Movie *movie);
	//
	//# \param	size		The size of the quad to which the movie is scaled.
	//# \param	name		The name of the movie resource.
	//# \param	loadFlags	The movie load flags. See below for a list of possible values.
	//# \param	movie		A pointer to an existing movie object.
	//
	//# \desc
	//# The $MovieWidget$ class displays a QuickTime movie.
	//#
	//# The movie load flags specified by the $loadFlags$ parameter can be any valid combination
	//# (through logical OR) of the following values.
	//
	//# \table	MovieLoadFlags
	//
	//# \base	InterfaceMgr/ImageWidget	A $MovieWidget$ is a specialized image widget.
	
	
	class MovieWidget : public ImageWidget
	{
		friend class WidgetReg<MovieWidget>;
		
		private:
			
			unsigned_int32		movieFlags;
			unsigned_int32		movieLoadFlags;
			
			C4::Movie			*movieObject;
			TextureHeader		*movieTexture;
			
			ResourceName		movieName;
			
			MovieWidget();
			MovieWidget(const MovieWidget& movieWidget);
			
			Widget *Replicate(void) const override;
		
		public:
			
			C4MOVIEAPI MovieWidget(const Vector2D& size, const char *name, unsigned_int32 loadFlags = 0);
			C4MOVIEAPI MovieWidget(const Vector2D& size, C4::Movie *movie);
			C4MOVIEAPI ~MovieWidget();
			
			C4::Movie *GetMovie(void) const
			{
				return (movieObject);
			}
			
			unsigned_int32 GetMovieFlags(void) const
			{
				return (movieFlags);
			}
			
			void SetMovieFlags(unsigned_int32 flags)
			{
				movieFlags = flags;
			}
			
			unsigned_int32 GetMovieLoadFlags(void) const
			{
				return (movieLoadFlags);
			}
			
			void SetMovieLoadFlags(unsigned_int32 loadFlags)
			{
				movieLoadFlags = loadFlags;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void SendInitialStateMessages(Player *player) const;
				
			C4MOVIEAPI void SetMovie(const char *name, unsigned_int32 loadFlags = 0);
		
			void Preprocess(void);
			
			void Build(void);
			void Render(List<Renderable> *renderList);
	};
	
	
	class PlayMovieWidgetFunction : public WidgetFunction
	{
		private:
			
			PlayMovieWidgetFunction(const PlayMovieWidgetFunction& playMovieWidgetFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			PlayMovieWidgetFunction();
			~PlayMovieWidgetFunction();
			
			bool OverridesFunction(const Function *function) const;
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class StopMovieWidgetFunction : public WidgetFunction
	{
		private:
			
			StopMovieWidgetFunction(const StopMovieWidgetFunction& stopMovieWidgetFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			StopMovieWidgetFunction();
			~StopMovieWidgetFunction();
			
			bool OverridesFunction(const Function *function) const;
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class PauseMovieWidgetFunction : public WidgetFunction
	{
		private:
			
			PauseMovieWidgetFunction(const PauseMovieWidgetFunction& pauseMovieWidgetFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			PauseMovieWidgetFunction();
			~PauseMovieWidgetFunction();
			
			bool OverridesFunction(const Function *function) const;
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class ResumeMovieWidgetFunction : public WidgetFunction
	{
		private:
			
			ResumeMovieWidgetFunction(const ResumeMovieWidgetFunction& resumeMovieWidgetFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			ResumeMovieWidgetFunction();
			~ResumeMovieWidgetFunction();
			
			bool OverridesFunction(const Function *function) const;
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class SetMovieTimeWidgetFunction : public WidgetFunction
	{
		private:
			
			float		movieTime;
			
			SetMovieTimeWidgetFunction(const SetMovieTimeWidgetFunction& setMovieTimeWidgetFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			SetMovieTimeWidgetFunction(float time = 0.0F);
			~SetMovieTimeWidgetFunction();
			
			float GetMovieTime(void) const
			{
				return (movieTime);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	//# \class	MovieController		Manages a node that has a movie playing into its material.
	//
	//# The $MovieController$ class manages a node that has a movie playing into its material.
	//
	//# \def	class MovieController : public Controller
	//
	//# \ctor	MovieController(const char *name, unsigned_int32 loadFlags = 0);
	//
	//# \param	name		The name of the movie resource, including the filename extension.
	//# \param	loadFlags	The movie load flags. See below for a list of possible values.
	//
	//# \desc
	//# The movie load flags specified by the $loadFlags$ parameter can be any valid combination
	//# (through logical OR) of the following values.
	//
	//# \table	MovieLoadFlags
	//
	//# \base	Controller/Controller		A $MovieController$ is a specific type of controller.
	//
	//# \also	$@Movie@$
	
	
	//# \function	MovieController::GetMovieName		Returns the name of the movie resource.
	//
	//# \proto	const ResourceName& GetMovieName(void) const;
	//
	//# \desc
	//# 
	
	
	//# \function	MovieController::GetMovieFlags		Returns the movie flags.
	//
	//# \proto	unsigned_int32 GetMovieFlags(void) const;
	//
	//# \desc
	//# 
	//
	//# \table	MovieFlags
	//
	//# \also	$@MovieController::SetMovieFlags@$
	
	
	//# \function	MovieController::SetMovieFlags		Sets the movie flags.
	//
	//# \proto	void SetMovieFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new movie flags.
	//
	//# \desc
	//# 
	//
	//# \table	MovieFlags
	//
	//# \also	$@MovieController::GetMovieFlags@$
	
	
	class MovieController : public Controller
	{
		friend class ControllerReg<MovieController>;
		
		private:
			
			ResourceName		movieName;
			unsigned_int32		movieFlags;
			unsigned_int32		movieLoadFlags;
			AttributeType		attributeType;
			
			C4::Movie			*movieObject;
			TextureHeader		*movieTexture;
			
			MapAttribute		*movieAttribute;
			List<Attribute>		attributeList;
			
			MovieController();
			MovieController(const MovieController& movieController);
			
			Controller *Replicate(void) const override;
		
		public:
			
			enum
			{
				kMovieMessagePlay,
				kMovieMessageStop,
				kMovieMessageTime
			};
			
			MovieController(const char *name, unsigned_int32 loadFlags = 0);
			~MovieController();
			
			const ResourceName& GetMovieName(void) const
			{
				return (movieName);
			}
			
			unsigned_int32 GetMovieFlags(void) const
			{
				return (movieFlags);
			}
			
			void SetMovieFlags(unsigned_int32 flags)
			{
				movieFlags = flags;
			}
			
			unsigned_int32 GetMovieLoadFlags(void) const
			{
				return (movieLoadFlags);
			}
			
			void SetMovieLoadFlags(unsigned_int32 loadFlags)
			{
				movieLoadFlags = loadFlags;
			}
			
			AttributeType GetAttributeType(void) const
			{
				return (attributeType);
			}
			
			void SetAttributeType(AttributeType type)
			{
				attributeType = type;
			}
			
			C4::Movie *GetMovie(void) const
			{
				return (movieObject);
			}
			
			static bool ValidNode(const Node *node);
			static void RegisterFunctions(ControllerRegistration *registration);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			ControllerMessage *ConstructMessage(ControllerMessageType type) const;
			void ReceiveMessage(const ControllerMessage *message);
			void SendInitialStateMessages(Player *player) const;
			
			void Preprocess(void);
			
			void Move(void);
			void Update(void);
			
			void Activate(Node *trigger, Node *activator);
	};
	
	
	class PlayMovieFunction : public Function
	{
		private:
			
			PlayMovieFunction(const PlayMovieFunction& playMovieFunction);
			
			Function *Replicate(void) const override;
			
			static void MovieComplete(C4::Movie *movie, void *cookie);
		
		public:
			
			PlayMovieFunction();
			~PlayMovieFunction();
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class StopMovieFunction : public Function
	{
		private:
			
			StopMovieFunction(const StopMovieFunction& stopMovieFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			StopMovieFunction();
			~StopMovieFunction();
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class MoviePlayStopMessage : public ControllerMessage
	{
		public:
			
			MoviePlayStopMessage(ControllerMessageType type, int32 controllerIndex);
			~MoviePlayStopMessage();
			
			bool OverridesMessage(const ControllerMessage *message) const;
	};
	
	
	class MovieTimeMessage : public ControllerMessage
	{
		friend class MovieController;
		
		private:
			
			float	movieTime;
			
			MovieTimeMessage(int32 controllerIndex);
		
		public:
			
			MovieTimeMessage(int32 controllerIndex, float time);
			~MovieTimeMessage();
			
			float GetMovieTime(void) const
			{
				return (movieTime);
			}
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
	};
	
	
	class MoviePlugin : public Plugin, public Singleton<MoviePlugin>
	{
		friend class C4::Movie;
		
		private:
			
			bool							quicktimeActive;
			
			List<Movie>						movieList;
			StringTable						stringTable;
			
			WidgetReg<MovieWidget>			movieWidgetReg;
			ControllerReg<MovieController>	movieControllerReg;
		
		public:
			
			MoviePlugin();
			~MoviePlugin();
			
			const StringTable *GetStringTable(void) const
			{
				return (&stringTable);
			}
			
			C4MOVIEAPI MovieResult Initialize(void);
			C4MOVIEAPI void Terminate(void);
			
			static TextureHeader *NewMovieTexture(C4::Movie *movie);
			static void ReleaseMovieTexture(TextureHeader *textureHeader);
			
			void PluginTask(void);
	};
	
	
	C4MOVIEAPI extern MoviePlugin *TheMoviePlugin;
}


#endif

// ZYURVUR
