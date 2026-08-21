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


#ifndef C4Files_h
#define C4Files_h


//# \component	File Manager
//# \prefix		FileMgr/


#include "C4Types.h"


namespace C4
{
	typedef EngineResult FileResult;
	
	
	enum
	{
		kMaxFileNameLength		= 127,
		kMaxFilePathLength		= 260
	};
	
	
	//# \enum	FileReferenceFlags
	
	enum
	{
		kFileDirectory		= 1 << 0,	//## The file reference represents a directory.
		kFileInvisible		= 1 << 1	//## The file is invisible.
	};
	
	
	//# \enum	FileOpenMode
	
	enum FileOpenMode
	{
		kFileReadOnly,		//## The file is to be opened with read-only access.
		kFileReadWrite,		//## The file is to be opened with read/write access.
		kFileCreate			//## A new file is to be created and opened with read/write access. Any existing file with the same name will be overwritten.
	};
	
	
	//# \enum	FilePositioningMode
	
	enum FilePositioningMode
	{
		kFileBegin,			//## The new position is relative to the beginning of the file.
		kFileCurrent,		//## The new position is relative to the current position.
		kFileEnd			//## The new position is relative to the end of the file.
	};
	
	
	//# \enum	FileResult
	
	enum
	{
		kFileOkay				= kEngineOkay,						//## The operation succeeded.
		kFileOpenFailed			= (kManagerFile << 16) | 0x0001,	//## The file could not be opened because the operating system returned an error.
		kFileNotOpen			= (kManagerFile << 16) | 0x0002,	//## The read or write operation failed because the file is not open.
		kFileIOFailed			= (kManagerFile << 16) | 0x0003,	//## The read or write operation failed because the operating system returned an error.
		kFileLocked				= (kManagerFile << 16) | 0x0004,	//## The file operation failed because the file is read-only.
		kFileAccessDenied		= (kManagerFile << 16) | 0x0005,	//## The file operation failed because access was denied by the operating system.
		kFileWriteProtected		= (kManagerFile << 16) | 0x0006,	//## The file operation failed because the disk is write protected.
		kFileDiskFull			= (kManagerFile << 16) | 0x0007,	//## The file operation failed because the disk is full.
		kFileCreateFailed		= (kManagerFile << 16) | 0x0008,	//## The file could not be created because the operating system returned an error.
		kFileDeleteFailed		= (kManagerFile << 16) | 0x0009		//## The file could not be deleted because the operating system returned an error.
	};
	
	
	class Thread;	

	class C4_API FileName
	{
	private:

		String<kMaxFileNameLength>			fileName;		


	public:

		FileName(const char *name);

		operator const char *(void) const
		{
			return (fileName);
		}
	};

	
	 
	//# \class	FileReference	Represents one item in a list of files.
	//
	//# The $FileReference$ class is used to represent one item in a list of files returned
	//# by the $@FileMgr::BuildFileList@$ function. 
	//
	//# \def	class FileReference : public ListElement<FileReference>
	//
	//# \ctor	FileReference(const char *name, unsigned_int32 flags = 0); 
	//
	//# \param	name	A pointer to the name of the file. If a path name is specified, it is
	//#					interpreted relative to the application. Directory names preceding a
	//#					file name or another directory name should be followed by a single
	//#					forward slash character (/).
	//# \param	flags	The flags associated with the file.
	//
	//# \desc
	//# The $FileReference$ class stores the name of a single file. If the $name$ parameter
	//# points to a string longer than $kMaxFileNameLength$, then it is truncated to this length.
	//# Applications typically do not need to construct $FileReference$ objects since they are
	//# returned in the list built by the $@FileMgr::BuildFileList@$ function.
	//# 
	//# The $flags$ parameter can be a combination (through logical OR) of the following bit values.
	//
	//# \table	FileReferenceFlags
	//
	//# \base	Utilities/ListElement<FileReference>	$FileReference$ objects are stored in a list by
	//#													the $@FileMgr::BuildFileList@$ function.
	//
	//# \also	$@File@$
	//# \also	$@FileMgr@$
	
	
	//# \function	FileReference::GetName		Returns the name stored in a $@FileReference@$ object.
	//
	//# \proto	const char *GetName(void) const;
	//
	//# \desc
	//# The $GetName$ function returns a pointer to the name of the file corresponding to
	//# the $@FileReference@$ object.
	//
	//# \also	$@FileReference::GetFlags@$
	
	
	//# \function	FileReference::GetFlags		Returns the flags associated with a $@FileReference@$ object.
	//
	//# \proto	unsigned_int32 GetFlags(void) const;
	//
	//# \desc
	//# The $GetFlags$ function returns either zero or a combination (through logical OR) of the following bit values.
	//
	//# \table	FileReferenceFlags
	//
	//# \also	$@FileReference::GetName@$
	
	
	class C4_API FileReference : public ListElement<FileReference>
	{
		private:
			
			String<kMaxFileNameLength>	fileName;
			unsigned_int32				fileFlags;
		
		public:
			
			FileReference(const char *name, unsigned_int32 flags = 0);
			~FileReference();
			
			const char *GetName(void) const
			{
				return (fileName);
			}
			
			unsigned_int32 GetFlags(void) const
			{
				return (fileFlags);
			}
	};
	
	
	//# \class	File	Used for reading from and writing to disk files.
	//
	//# The $File$ class is used for reading from and writing to disk files.
	//
	//# \def	class File
	//
	//# \ctor	File();
	//
	//# \desc
	//# The $File$ class encapsulates the opening, closing, reading, and writing of disk files.
	//# Once a $File$ object has been constructed, a file can be opened using the $@File::Open@$ function.
	//# A file is automatically closed when the $File$ object is destroyed, but it's also possible to
	//# explicitly close a file using the $@File::Close@$ function.
	//
	//# \also	$@File::Open@$
	//# \also	$@File::Close@$
	//# \also	$@FileMgr@$
	
	
	//# \function	File::Open		Opens a file.
	//
	//# \proto	FileResult Open(const char *name, FileOpenMode mode = kFileReadOnly);
	//
	//# \param	name	A pointer to the name of the file. If a path name is specified, it
	//#					is interpreted relative to the application. Directory names preceding a
	//#					file name or another directory name should be followed by a single forward
	//#					slash character (/).
	//# \param	mode	Specifies how the file should be opened.
	//
	//# \desc
	//# The $Open$ function opens the file specified by the $name$ parameter.
	//# The $mode$ parameter specifies one of the following open modes.
	//
	//# \table	FileOpenMode
	//
	//# If the open operation succeeeds, then the return value is $kFileOkay$. Otherwise, one of the following
	//# file result codes is returned.
	//
	//# \value	kFileOpenFailed		The file could not be opened because the operating system returned an error.
	//# \value	kFileCreateFailed	The file could not be created because the operating system returned an error.
	//
	//# \also	$@File::Close@$
	
	
	//# \function	File::Close		Closes a file.
	//
	//# \proto	void Close(void);
	//
	//# \desc
	//# The $Close$ function closes a file. If no file was previously opened, then this function has no effect.
	//
	//# \also	$@File::Open@$
	
	
	//# \function	File::Read		Reads data from a file.
	//
	//# \proto	FileResult Read(void *buffer, unsigned_int32 size);
	//
	//# \param	buffer	A pointer to the location that will receive the data read from the file.
	//# \param	size	The number of bytes to read from the file.
	//
	//# \desc
	//# The $Read$ function attempts to perform a synchronous read operation for a file object.
	//# If successful, this function blocks until the read operation completes. The return value
	//# is one of the following file result codes.
	//
	//# \value	kFileOkay			The read operation was successfully completed.
	//# \value	kFileNotOpen		No file has been opened for the $File$ object.
	//# \value	kFileIOFailed		The operation failed because the operating system returned an error.
	//
	//# \also	$@File::Write@$
	
	
	//# \function	File::Write		Writes data to a file.
	//
	//# \proto	FileResult Write(const void *buffer, unsigned_int32 size);
	//
	//# \param	buffer	A pointer to the location from which data will be written to the file.
	//# \param	size	The number of bytes to write to the file.
	//
	//# \desc
	//# The $Write$ function attempts to perform a synchronous write operation for a file object.
	//# If successful, this function blocks until the write operation completes. The return value
	//# is one of the following file result codes.
	//
	//# \value	kFileOkay			The write operation was successfully completed.
	//# \value	kFileNotOpen		No file has been opened for the $File$ object.
	//# \value	kFileIOFailed		The operation failed because the operating system returned an error.
	//
	//# \also	$@File::Read@$
	
	
	//# \function	File::GetPosition		Returns the current read/write position for a file.
	//
	//# \proto	unsigned_int64 GetPosition(void) const;
	//
	//# \desc
	//# The $GetPosition$ function returns the current absolute position within the file, in bytes,
	//# at which the next read or write operation will take place. When a file is initially opened,
	//# its read/write position is 0.
	//
	//# \also	$@File::SetPosition@$
	//# \also	$@File::GetSize@$
	
	
	//# \function	File::SetPosition		Sets the current read/write position for a file.
	//
	//# \proto	unsigned_int64 SetPosition(int64 position, FilePositioningMode mode = kFileBegin);
	//
	//# \param	position	The position, in bytes, at which the next read or write operation will take place.
	//#						The interpretation of this position depends on the value of the $mode$ parameter.
	//# \param	mode		Specifies how the $position$ parameter is interpreted using one of the
	//#						file positioning modes listed below.
	//
	//# \desc
	//# The $SetPosition$ function sets the current position within the file, in bytes, at which
	//# the next read or write operation will take place. The $mode$ parameter specifies the position
	//# relative to which the $position$ parameter is applied and may be one of the following values.
	//
	//# \table	FilePositioningMode
	//
	//# The return value is the new absolute read/write position.
	//
	//# \also	$@File::GetPosition@$
	//# \also	$@File::GetSize@$
	
	
	//# \function	File::GetSize		Returns the current size of a file.
	//
	//# \proto	unsigned_int64 GetSize(void) const;
	//
	//# \desc
	//# The $GetSize$ function returns the current size, in bytes, of a file.
	//
	//# \also	$@File::GetPosition@$
	//# \also	$@File::SetPosition@$
	
	
	class C4_API File
	{
		friend class FileMgr;
		
		private:
			
		
			HANDLE				fileHandle;
		
			bool					fileOpen;
			unsigned_int64			filePosition;
			
			static FileResult TranslateErrorCode(unsigned_int32 error, FileResult defaultResult);
			
		public:
			
			File();
			~File();
			
			unsigned_int64 GetPosition(void) const
			{
				return (filePosition);
			}
			
			bool Open(void) const
			{
				return (fileOpen);
			}
			
			FileResult Open(const char *name, FileOpenMode mode = kFileReadOnly);
			void Close(void);
			
			FileResult Read(void *buffer, unsigned_int32 size);
			FileResult Write(const void *buffer, unsigned_int32 size);
			
			unsigned_int64 GetSize(void) const;
			unsigned_int64 SetPosition(int64 position, FilePositioningMode mode = kFileBegin);
			
			FileResult WritePad(int32 align);
			
			File& operator <<(const char *text);
			
			File& operator <<(char text)
			{
				Write(&text, 1);
				return (*this);
			}
			
			File& operator <<(int32 num)
			{
				return (*this << String<15>(num));
			}
			
			File& operator <<(unsigned_int32 num)
			{
				return (*this << String<15>(num));
			}
	};
	
	
	//# \class 	FileMgr		The File Manager class.
	//
	//# \def	class FileMgr
	//
	//# \desc
	//# The global File Manager object encapsulates the file management services of the C4 Engine.
	//# The single instance of the File Manager is constructed during an application's initialization
	//# and destroyed at termination.
	//# 
	//# The File Manager's member functions are accessed through the global pointer $TheFileMgr$.
	//
	//# \also	$@File@$
	
	
	//# \function	FileMgr::DeleteFile		Deletes a file.
	//
	//# \proto	static FileResult DeleteFile(const char *name);
	//
	//# \param	name	A pointer to the name of the file to delete. If a path name is specified,
	//#					it is interpreted relative to the application. Directory names preceding
	//#					a file name or another directory name should be followed by a single
	//#					forward slash character (/).
	//
	//# \desc
	//# The $DeleteFile$ function attempts to delete the file whose name is given by the $name$
	//# parameter and returns one of the following file result codes.
	//
	//# \value	kFileOkay			The file was successfully deleted.
	//# \value	kFileDeleteFailed	The file could not be deleted because the operating system returned an error.
	
	
	//# \function	FileMgr::BuildFileList		Enumerates the files in a given directory.
	//
	//# \proto	static void BuildFileList(const char *directory, List<FileReference> *list, BuildProc *proc = &DefaultFilter, void *cookie = nullptr);
	//
	//# \param	directory	A pointer to the name of the directory for which files should be
	//#						enumerated. If a path name is specified, it is interpreted relative
	//#						to the application. Multiple directory names should be separated by
	//#						single forward slash characters (/), but the last directory name in
	//#						the path should not be followed by a slash.
	//# \param	list		A pointer to an existing list object which is to contain the
	//#						$@FileReference@$ objects.
	//# \param	proc		A pointer to an optional filter function.
	//# \param	cookie		The value that is passed to the $cookie$ parameter of the filter function.
	//
	//# \desc
	//# The $BuildFileList$ function enumerates the files in the directory given by the $directory$
	//# parameter and instantiates a $@FileReference@$ object for each file in the directory.
	//# These objects are placed into the list given by the $list$ parameter.
	//# 
	//# One typically calls $BuildFileList$ to obtain a list of files in a particular directory
	//# and then iterates through the list to examine each file individually. When the list of
	//# $@FileReference@$ objects is no longer needed, the memory for each of the objects should
	//# be released. This can be done by calling the $@Utilities/List::Purge@$ function or by
	//# destroying the $@Utilities/List@$ object itself.
	//# 
	//# If a filter function is specified by the $proc$ parameter, then it is called for each
	//# file as the list is being built to determine whether the file should be included in the
	//# list. The $BuildProc$ type is defined as follows.
	//
	//# \code	typedef bool BuildProc(const char *name, unsigned_int32 flags, void *cookie);
	//
	//# The $name$ parameter of the filter function recieves a pointer to the file name, the
	//# $flags$ parameter receives the flags associated with the file, and the $cookie$ parameter
	//# receives the value passed to the $cookie$ parameter of the $BuildFileList$ function.
	//# The flags associated with a file can be a combination (through logical OR) of the following bit values.
	//
	//# \table	FileReferenceFlags
	//
	//# The filter function should return $true$ to indicate that the file should be included
	//# in the file list and should return $false$ to indicate that the file should be skipped.
	//# 
	//# If no filter function is specified, then the default filter function is used. The default
	//# filter function returns $false$ if the $kFileInvisible$ flag is set or if the file name begins
	//# with a period, and returns $true$ otherwise.
	
	
	class C4_API FileMgr : public Manager<FileMgr>
	{
		friend class File;		
		
		public:
			
			typedef bool BuildProc(const char *, unsigned_int32, void *);
			
			FileMgr(int);
			~FileMgr();
			
			EngineResult Construct(void);
			void Destruct(void);			
			
			static FileResult DeleteFile(const char *name);
			
			static FileResult CreateDirectory(const char *directory);
			static FileResult CreateDirectoryPath(const char *path);
			
			static bool DefaultFilter(const char *name, unsigned_int32 flags, void *cookie);
			static bool DirectoryFilter(const char *name, unsigned_int32 flags, void *cookie);
			
			static void BuildFileList(const char *directory, List<FileReference> *list, BuildProc *proc = &DefaultFilter, void *cookie = nullptr);
	};
	
	
	C4_API  extern FileMgr *TheFileMgr;
}


#endif

// ZYURVUR
