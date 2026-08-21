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


#ifndef C4Time_h
#define C4Time_h


//# \component	Time Manager
//# \prefix		TimeMgr/


#include "C4Packing.h"


namespace C4
{
	#if C4WINDOWS
	
		typedef LONGLONG			RawTimeValue;
	
	#elif C4MACOS || C4IOS
	
		typedef uint64_t			RawTimeValue;
	
	#elif C4LINUX
		
		typedef int64				RawTimeValue;
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
	
	
	//# \enum	TaskFlags
	
	enum
	{
		kTaskNonpersistent			= 1 << 0		//## The task is nonpersistent. If this flag is set, then the task is automatically destroyed after it is triggered.
	};
	
	
	//# \enum	InterpolatorMode
	
	enum
	{
		kInterpolatorStop			= 0,			//## The interpolator is stopped.
		kInterpolatorForward		= 1 << 0,		//## The interpolator is moving forward. Only one of $kInterpolatorForward$ and $kInterpolatorBackward$ may be set.
		kInterpolatorBackward		= 1 << 1,		//## The interpolator is moving backward. Only one of $kInterpolatorForward$ and $kInterpolatorBackward$ may be set.
		kInterpolatorLoop			= 1 << 2,		//## The interpolator is loops in the same direction when it reaches the maximum or minimum value.
		kInterpolatorOscillate		= 1 << 3		//## The interpolator reverses direction when it reaches the maximum value. If $kInterpolatorLoop$ is also set, the interpolator reverses direction when it reaches the minimum value as well.
	};
	
	
	struct SubrangeData
	{
		int32			subrangeCount;
		Range<float>	subrange[2];
	};
	
	
	//# \struct	DateTime		Contains information about a date and time.
	//
	//# The $DateTime$ structure contains information about a date and time.
	//
	//# \def	struct DateTime
	//
	//# \data	DateTime
	
	
	//# \member		DateTime
	
	struct DateTime
	{
		unsigned_int32	year;		//## The year.
		unsigned_int32	month;		//## The month, in the range 1&ndash;12.
		unsigned_int32	day;		//## The day of the month, in the range 1&ndash;31.
		unsigned_int32	hour;		//## The hour of the day, in the range 0&ndash;23.
		unsigned_int32	minute;		//## The minute, in the range 0&ndash;59.
		unsigned_int32	second;		//## The second, in the range 0&ndash;59.
	};
	
	
	//# \class	DeferredTask	General base class for objects which can be scheduled to perform a task after a specific amount of time.
	//
	//# \def	class DeferredTask : public ListElement<DeferredTask>, public Completable<DeferredTask>
	//
	//# \ctor	DeferredTask(CompletionProc *proc, void *cookie = nullptr);
	//
	//# \param	proc	The procedure to invoke when the task is triggered.
	//# \param	cookie	The cookie that is passed to the trigger procedure as its last parameter.
	//
	//# \desc
	//# The $DeferredTask$ class encapsulates a callback procedure that is invoked at some future time when the deferred task object is
	//# triggered by some means. Once a deferred task or one of its subclasses has been created, it should be added to the Time Manager
	//# using the $@TimeMgr::AddTask@$ function. The callback procedure specified by the $proc$ parameter should have the following prototype.
	//
	//# \code	typedef void CompletionProc(DeferredTask *, void *);
	//
	//# When the deferred task is triggered, the callback is invoked with the pointer specified by the $cookie$ parameter passed to it as its
	//# second argument. A deferred task is triggered only once and must be resubmitted with the $@TimeMgr::AddTask@$ function in order to
	//# be triggered again.
	//#
	//# The $DeferredTask$ base class is always triggered, so the callback procedure will be invoked at the beginning of the next frame once 
	//# the deferred task object has been registered using the $@TimeMgr::AddTask@$ function. See the $@Timer@$ class for a deferred task that
	//# is triggered after a specified period of time. 
	//# 
	//# Deferred tasks can be persistent or nonpersistent. A persistent task continues to exist after it has been triggered, but a nonpersistent 
	//# task is automatically destroyed after being triggered. By default, a deferred task is persistent. The persistence state can be changed
	//# using the $@DeferredTask::SetTaskFlags@$ function. 
	//
	//# \base	Utilities/ListElement<DeferredTask>		Used internally by the Time Manager.
	//# \base	Utilities/Completable<DeferredTask>		The completion procedure is called when the task is triggered.
	// 
	//# \also	$@Timer@$
	//# \also	$@TimeMgr::AddTask@$
	//# \also	$@TimeMgr::RemoveTask@$
	 
	
	//# \function	DeferredTask::GetTaskFlags		Returns the task flags.
	//
	//# \proto	unsigned_int32 GetTaskFlags(void) const;
	//
	//# \desc
	//# The $GetTaskFlags$ function returns the task flags, which can be a combination (through logical OR) of the following values.
	//
	//# \table	TaskFlags
	//
	//# The initial value of the task flags is 0.
	//
	//# \also	$@DeferredTask::SetTaskFlags@$
	
	
	//# \function	DeferredTask::SetTaskFlags		Sets the task flags.
	//
	//# \proto	void SetTaskFlags(unsigned_int32 flags);
	//
	//# \param	flags		The new task flags.
	//
	//# \desc
	//# The $SetTaskFlags$ function sets the task flags, which can be a combination (through logical OR) of the following values.
	//
	//# \table	TaskFlags
	//
	//# The initial value of the task flags is 0.
	//
	//# \also	$@DeferredTask::GetTaskFlags@$
	
	
	class C4_API DeferredTask : public ListElement<DeferredTask>, public Completable<DeferredTask>
	{
		friend class TimeMgr;
		
		private:
			
			unsigned_int32		taskFlags;
		
		protected:
		
			virtual bool GetTriggerState(void);
			
		public:
			
			DeferredTask(CompletionProc *proc, void *cookie = nullptr);
			virtual ~DeferredTask();
			
			unsigned_int32 GetTaskFlags(void) const
			{
				return (taskFlags);
			}
			
			void SetTaskFlags(unsigned_int32 flags)
			{
				taskFlags = flags;
			}
	};
	
	
	//# \class	Timer	A deferred task which signals the expiration of a specific amount of time.
	//
	//# \def	class Timer : public DeferredTask
	//
	//# \ctor	Timer(int32 time, CompletionProc *proc, void *cookie = nullptr);
	//
	//# \param	time	The length of time, in milliseconds, which should pass before the timer is triggered.
	//# \param	proc	The procedure to invoke when the task is triggered.
	//# \param	cookie	The cookie that is passed to the trigger procedure as its last parameter.
	//
	//# \desc
	//# The $Timer$ class encapsulates a deferred task which is triggered after the time specified by the $time$ parameter.
	//# Once a timer is registered using the $@TimeMgr::AddTask@$ function, its remaining time is reduced each frame until
	//# it reaches zero, at which point its callback procedure is invoked. The callback procedure specified by the $proc$
	//# parameter should have the following prototype.
	//
	//# \code	typedef void CompletionProc(DeferredTask *, void *);
	//
	//# When the timer is triggered, the callback is invoked with the pointer specified by the $cookie$ parameter passed to it as its
	//# second argument. A timer is triggered only once and must be resubmitted with the $@TimeMgr::AddTask@$ function in order to
	//# be triggered again.
	//
	//# \base	DeferredTask	$Timer$ objects provide a trigger for $DeferredTask$ objects.
	
	
	//# \function	Timer::GetTime		Returns the length of time until the timer is triggered.
	//
	//# \proto	int32 GetTime(void) const;
	//
	//# \desc
	//# The $GetTime$ function returns the length of time remaining, in milliseconds, before a timer object is triggered.
	//
	//# \also	$@Timer::SetTime@$
	
	
	//# \function	Timer::SetTime		Sets the length of time which should pass before the timer is triggered.
	//
	//# \proto	void SetTime(int32 time);
	//
	//# \param	time	The length of time, in milliseconds, which should pass before the timer is triggered.
	//
	//# \desc
	//# The $SetTime$ function sets the length of time remaining, in milliseconds, before a timer object is triggered.
	//# If the time is zero or negative, then the timer will be triggered on the next frame.
	//
	//# \also	$@Timer::GetTime@$
	
	
	class C4_API Timer : public DeferredTask
	{
		private:
			
			int32	remainingTime;
		
			bool GetTriggerState(void);
			
		public:
			
			Timer(CompletionProc *proc, void *cookie = nullptr);
			Timer(int32 time, CompletionProc *proc, void *cookie = nullptr);
			~Timer();
			
			int32 GetTime(void) const
			{
				return (remainingTime);
			}
			
			void SetTime(int32 time)
			{
				remainingTime = time;
			}
	};
	
	
	//# \class	Interpolator	Encapsulates a general interpolator.
	//
	//# \def	class Interpolator : public Completable<Interpolator>
	//
	//# \ctor	Interpolator();
	//# \ctor	Interpolator(float value, float rate = 0.0F, unsigned_int32 mode = kInterpolatorStop);
	//
	//# \param	value	The initial value of the interpolator.
	//# \param	rate	The rate at which the interpolated value changes. This is measured in value change per millisecond.
	//# \param	mode	The initial interpolation mode. See below for possible values.
	//
	//# \desc
	//# 
	//
	//# \table	InterpolatorMode
	//
	//# \base	Utilities/Completable<Interpolator>		The completion procedure is called when an interpolator goes into the stopped state.
	
	
	//# \function	Interpolator::GetValue		Returns the current value of an interpolator.
	//
	//# \proto	float GetValue(void) const;
	//
	//# \desc
	//
	//# \also	$@Interpolator::SetValue@$
	
	
	//# \function	Interpolator::SetValue		Sets the current value of an interpolator.
	//
	//# \proto	void SetValue(float value);
	//
	//# \param	value	The new interpolator value.
	//
	//# \desc
	//
	//# \also	$@Interpolator::GetValue@$
	
	
	//# \function	Interpolator::GetMinValue	Returns the minimum value of an interpolator.
	//
	//# \proto	float GetMinValue(void) const;
	//
	//# \desc
	//
	//# \also	$@Interpolator::SetMinValue@$
	//# \also	$@Interpolator::GetMaxValue@$
	//# \also	$@Interpolator::SetMaxValue@$
	//# \also	$@Interpolator::SetRange@$
	
	
	//# \function	Interpolator::SetMinValue		Sets the minimum value of an interpolator.
	//
	//# \proto	void SetMinValue(float min);
	//
	//# \param	min		The new minimum interpolator value.
	//
	//# \desc
	//
	//# \also	$@Interpolator::GetMinValue@$
	//# \also	$@Interpolator::GetMaxValue@$
	//# \also	$@Interpolator::SetMaxValue@$
	//# \also	$@Interpolator::SetRange@$
	
	
	//# \function	Interpolator::GetMaxValue		Returns the maximum value of an interpolator.
	//
	//# \proto	float GetMaxValue(void) const;
	//
	//# \desc
	//
	//# \also	$@Interpolator::SetMaxValue@$
	//# \also	$@Interpolator::GetMinValue@$
	//# \also	$@Interpolator::SetMinValue@$
	//# \also	$@Interpolator::SetRange@$
	
	
	//# \function	Interpolator::SetMaxValue		Sets the maximum value of an interpolator.
	//
	//# \proto	void SetMaxValue(float max);
	//
	//# \param	max		The new maximum interpolator value.
	//
	//# \desc
	//
	//# \also	$@Interpolator::GetMaxValue@$
	//# \also	$@Interpolator::GetMinValue@$
	//# \also	$@Interpolator::SetMinValue@$
	//# \also	$@Interpolator::SetRange@$
	
	
	//# \function	Interpolator::SetRange			Sets the minimum and maximum values of an interpolator.
	//
	//# \proto	void SetRange(float min, float max);
	//
	//# \param	min		The new minimum interpolator value.
	//# \param	max		The new maximum interpolator value.
	//
	//# \desc
	//
	//# \also	$@Interpolator::GetMinValue@$
	//# \also	$@Interpolator::SetMinValue@$
	//# \also	$@Interpolator::GetMaxValue@$
	//# \also	$@Interpolator::SetMaxValue@$
	
	
	//# \function	Interpolator::GetRate		Returns the rate at which an interpolator moves.
	//
	//# \proto	float GetRate(void) const;
	//
	//# \desc
	//
	//# \also	$@Interpolator::SetRate@$
	
	
	//# \function	Interpolator::SetRate		Sets the rate at which an interpolator moves.
	//
	//# \proto	void SetRate(float rate);
	//
	//# \param	rate	The new interpolator rate.
	//
	//# \desc
	//
	//# \also	$@Interpolator::GetRate@$
	
	
	//# \function	Interpolator::GetMode		Returns the current mode for an interpolator.
	//
	//# \proto	unsigned_int32 GetMode(void) const;
	//
	//# \desc
	//
	//# \also	$@Interpolator::SetMode@$
	
	
	//# \function	Interpolator::SetMode		Sets the current mode for an interpolator.
	//
	//# \proto	void SetMode(unsigned_int32 mode);
	//
	//# \param	mode	The new interpolator mode.
	//
	//# \desc
	//# The $SetMode$ function sets the interpolator mode to the value specified by the $mode$ parameter.
	//# This value can be a combination (through logical OR) of the following constants.
	//
	//# \table	InterpolatorMode
	//
	//# \also	$@Interpolator::GetMode@$
	
	
	//# \function	Interpolator::SetLoopProc		Sets the loop callback procedure for an interpolator.
	//
	//# \proto	void SetLoopProc(LoopProc *proc, void *cookie = nullptr);
	//
	//# \param	proc	The new loop callback procedure.
	//# \param	cookie	The cookie that is passed to the callback procedure.
	//
	//# \desc
	//
	//# \code	typedef void LoopProc(Interpolator *, void *);
	
	
	//# \function	Interpolator::Set		Sets the current value, rate, and mode for an interpolator.
	//
	//# \proto	void Set(float value, float rate, unsigned_int32 mode);
	//
	//# \param	value	The new interpolator value.
	//# \param	rate	The new interpolator rate.
	//# \param	mode	The new interpolator mode.
	//
	//# \desc
	//
	//# \also	$@Interpolator::GetValue@$
	//# \also	$@Interpolator::SetValue@$
	//# \also	$@Interpolator::GetRate@$
	//# \also	$@Interpolator::SetRate@$
	//# \also	$@Interpolator::GetMode@$
	//# \also	$@Interpolator::SetMode@$
	
	
	//# \function	Interpolator::UpdateValue		Updates an interpolator.
	//
	//# \proto	float UpdateValue(SubrangeData *subrangeData = nullptr);
	//
	//# \param	subrangeData	An optional pointer to a structure that receives information about what subranges were covered.
	//
	//# \desc
	//#
	
	
	class C4_API Interpolator : public Completable<Interpolator>, public Packable
	{
		public:
			
			typedef void LoopProc(Interpolator *, void *);
			
		private:
			
			float				interpolatorValue;
			float				interpolatorRate;
			unsigned_int32		interpolatorMode;
			Range<float>		interpolatorRange;
			
			LoopProc			*loopProc;
			void				*loopCookie;
		
		public:
			
			Interpolator();
			Interpolator(float value, float rate = 0.0F, unsigned_int32 mode = kInterpolatorStop);
			~Interpolator();
			
			float GetValue(void) const
			{
				return (interpolatorValue);
			}
			
			void SetValue(float value)
			{
				interpolatorValue = value;
			}
			
			float GetRate(void) const
			{
				return (interpolatorRate);
			}
			
			void SetRate(float rate)
			{
				interpolatorRate = rate;
			}
			
			unsigned_int32 GetMode(void) const
			{
				return (interpolatorMode);
			}
			
			void SetMode(unsigned_int32 mode)
			{
				interpolatorMode = mode;
			}
			
			const Range<float>& GetRange(void) const
			{
				return (interpolatorRange);
			}
			
			void SetMinValue(float min)
			{
				interpolatorRange.min = min;
			}
			
			void SetMaxValue(float max)
			{
				interpolatorRange.max = max;
			}
			
			void SetRange(float min, float max)
			{
				interpolatorRange.Set(min, max);
			}
			
			void SetLoopProc(LoopProc *proc, void *cookie = nullptr)
			{
				loopProc = proc;
				loopCookie = cookie;
			}
			
			void Set(float value, float rate, unsigned_int32 mode)
			{
				interpolatorValue = value;
				interpolatorRate = rate;
				interpolatorMode = mode;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			float UpdateValue(SubrangeData *subrangeData = nullptr);
	};
	
	
	//# \class	TimeMgr		The Time Manager class.
	//
	//# \def	class TimeMgr : public Manager<TimeMgr>
	//
	//# \desc
	//# The $TimeMgr$ class encapsulates the low-level timing functionality of the C4 Engine.
	//# The single instance of the Time Manager is constructed during an application's initialization
	//# and destroyed at termination.
	//# 
	//# The Time Manager's member functions are accessed through the global pointer $TheTimeMgr$.
	
	
	//# \function	TimeMgr::GetWorldTimeMultiplier		Returns the multiplier that converts normal time into world time.
	//
	//# \proto	float GetWorldTimeMultiplier(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@TimeMgr::SetWorldTimeMultiplier@$
	
	
	//# \function	TimeMgr::SetWorldTimeMultiplier		Sets the multiplier that converts normal time into world time.
	//
	//# \proto	void SetWorldTimeMultiplier(float multiplier);
	//
	//# \param	multiplier		The world time multiplier. The default value is 1.0.
	//
	//# \desc
	//# 
	//
	//# \also	$@TimeMgr::GetWorldTimeMultiplier@$
	
	
	//# \function	TimeMgr::GetDeltaTime		Returns the time difference between the current frame and the previous frame as an integer.
	//
	//# \proto	int32 GetDeltaTime(void) const;
	//
	//# \desc
	//# The $GetDeltaTime$ function returns the integer difference in time between the current frame and the previous
	//# frame. The value returned is in milliseconds.
	//
	//# \also	$@TimeMgr::GetFloatDeltaTime@$
	//# \also	$@TimeMgr::GetAbsoluteTime@$
	
	
	//# \function	TimeMgr::GetFloatDeltaTime		Returns the time difference between the current frame and the previous frame as a floating point number.
	//
	//# \proto	float GetFloatDeltaTime(void) const;
	//
	//# \desc
	//# The $GetFloatDeltaTime$ function returns the floating-point difference in time between the current frame and
	//# the previous frame. The value returned is in milliseconds.
	//
	//# \also	$@TimeMgr::GetDeltaTime@$
	//# \also	$@TimeMgr::GetAbsoluteTime@$
	
	
	//# \function	TimeMgr::GetAbsoluteTime		Returns the current absolute millisecond count.
	//
	//# \proto	unsigned_int32 GetAbsoluteTime(void) const;
	//
	//# \desc
	//
	//# \also	$@TimeMgr::GetDeltaTime@$
	//# \also	$@TimeMgr::GetFloatDeltaTime@$
	
	
	//# \div
	//# \function	TimeMgr::AddTask		Adds a deferred task to the Time Manager task list.
	//
	//# \proto	void AddTask(DeferredTask *task);
	//
	//# \param	task	The deferred task to add.
	//
	//# \desc
	//# The $AddTask$ function registers a deferred task object with the Time Manager. A deferred task can be
	//# triggered only after it has been registered.
	//#
	//# A deferred task can be unregistered explicitly using the $@TimeMgr::RemoveTask@$, or it can simply be deleted.
	//
	//# \also	$@TimeMgr::RemoveTask@$
	//# \also	$@DeferredTask@$
	
	
	//# \function	TimeMgr::RemoveTask		Removes a deferred task from the Time Manager task list.
	//
	//# \proto	void RemoveTask(DeferredTask *task);
	//
	//# \param	task	The deferred task to remove.
	//
	//# \desc
	//# The $RemoveTask$ function unregisters a deferred task object with the Time Manager.
	//
	//# \also	$@TimeMgr::AddTask@$
	//# \also	$@DeferredTask@$
	
	
	//# \div
	//# \function	TimeMgr::GetDateTime		Returns a structure containing the current date and time.
	//
	//# \proto	static void GetDateTime(DateTime *dateTime);
	//
	//# \param	dateTime	The structure that receives the current date and time.
	//
	//# \desc
	//# The $GetDateTime$ function fills in the $@DateTime@$ structure specified by the $dateTime$ parameter with the
	//# current date and time. The date and time correspond to the time zone set for the local machine.
	//
	//# \also	$@TimeMgr::GetDateTimeStrings@$
	//# \also	$@DateTime@$
	
	
	//# \function	TimeMgr::GetDateTimeStrings		Returns strings containing the current date and time.
	//
	//# \proto	static void GetDateTimeStrings(String<127> *date, String<127> *time);
	//
	//# \param	date	A pointer to a string that receives the date.
	//# \param	time	A pointer to a string that receives the time.
	//
	//# \desc
	//# The $GetDateTimeStrings$ function returns strings containing the current date and time in the strings
	//# specified by the $date$ and $time$ parameters. The date and time correspond to the time zone set for the local machine.
	//
	//# \also	$@TimeMgr::GetDateTime@$
	
	
	class C4_API TimeMgr : public Manager<TimeMgr>
	{
		private:
			
			#if C4WINDOWS
			
				LARGE_INTEGER				counterFrequency;
			
			#elif C4MACOS || C4IOS
			
				mach_timebase_info_data_t	timebaseInfo;
			
			#endif
			
			RawTimeValue			currentTimeValue;
			RawTimeValue			previousTimeValue;
			
			float					worldTimeMultiplier;
			
			int32					worldDeltaTime;
			float					worldFloatDeltaTime;
			float					worldResidualDeltaTime;
			unsigned_int32			worldAbsoluteTime;
			
			int32					systemDeltaTime;
			float					systemFloatDeltaTime;
			unsigned_int32			systemAbsoluteTime;
			
			List<DeferredTask>		taskList;
			
			static RawTimeValue GetRawTimeValue(void)
			{
				#if C4WINDOWS
				
					LARGE_INTEGER	counter;
					
					QueryPerformanceCounter(&counter);
					return (counter.QuadPart);
				
				#elif C4MACOS || C4IOS
				
					return (mach_absolute_time());
				
				#elif C4LINUX
				
					timespec		spec;
					
					clock_gettime(CLOCK_MONOTONIC, &spec);
					return (RawTimeValue(spec.tv_sec) * 1000000000U + spec.tv_nsec);
				
				#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

				#endif //]
			}
			
			int32 ConvertRawTimeValueToMilliseconds(RawTimeValue time) const
			{
				#if C4WINDOWS
				
					return ((int32) (time * 1000U / counterFrequency.QuadPart));
				
				#elif C4MACOS || C4IOS
				
					return ((int32) (time * timebaseInfo.numer / (timebaseInfo.denom * 1000000U)));
				
				#elif C4LINUX
				
					return ((int32) (time / 1000000U));
				
				#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

				#endif //]
			}
			
			int64 ConvertRawTimeValueToMicroseconds(RawTimeValue time) const
			{
				#if C4WINDOWS
				
					return (time * 1000000U / counterFrequency.QuadPart);
				
				#elif C4MACOS || C4IOS
				
					return (time * timebaseInfo.numer / (timebaseInfo.denom * 1000U));
				
				#elif C4LINUX
				
					return ((int32) (time / 1000U));
				
				#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

				#endif //]
			}
		
		public:
			
			TimeMgr(int);
			~TimeMgr();
			
			EngineResult Construct(void);
			void Destruct(void);
			
			float GetWorldTimeMultiplier(void) const
			{
				return (worldTimeMultiplier);
			}
			
			void SetWorldTimeMultiplier(float multiplier)
			{
				worldTimeMultiplier = multiplier;
			}
			
			int32 GetDeltaTime(void) const
			{
				return (worldDeltaTime);
			}
			
			float GetFloatDeltaTime(void) const
			{
				return (worldFloatDeltaTime);
			}
			
			unsigned_int32 GetAbsoluteTime(void) const
			{
				return (worldAbsoluteTime);
			}
			
			int32 GetSystemDeltaTime(void) const
			{
				return (systemDeltaTime);
			}
			
			float GetSystemFloatDeltaTime(void) const
			{
				return (systemFloatDeltaTime);
			}
			
			unsigned_int32 GetSystemAbsoluteTime(void) const
			{
				return (systemAbsoluteTime);
			}
			
			unsigned_int32 GetMillisecondCount(void) const
			{
				return (ConvertRawTimeValueToMilliseconds(GetRawTimeValue()));
			}
			
			unsigned_int64 GetMicrosecondCount(void) const
			{
				return (ConvertRawTimeValueToMicroseconds(GetRawTimeValue()));
			}
			
			void AddTask(DeferredTask *task)
			{
				taskList.Append(task);
			}
			
			void RemoveTask(DeferredTask *task)
			{
				taskList.Remove(task);
			}
			
			static void GetDateTime(DateTime *dateTime);
			static void GetDateTimeStrings(String<127> *date, String<127> *time);
			
			void ResetTime(void);
			
			void TimeTask(void);
	};
	
	
	C4_API extern TimeMgr *TheTimeMgr;
}


#endif

// ZYURVUR
