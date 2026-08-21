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


#include "C4Files.h"


using namespace C4;


FileMgr *C4::TheFileMgr = nullptr;


namespace C4
{
	template <> FileMgr Manager<FileMgr>::managerObject(0);
	template <> FileMgr **Manager<FileMgr>::managerPointer = &TheFileMgr;
}

FileName::FileName(const char *name)
{
	char prev = 0;


	char *output = fileName;
	for (machine a = 0; a < kMaxFileNameLength; a++)
	{
		char c = *name;
		if (c != '/')
		{
			*output = c;
			if (c == 0) break;
			output++;
		}
		else if (prev != '/')
		{
			*output = '\\';
			output++;
		}

		prev = c;
		name++;
	}	
}


FileReference::FileReference(const char *name, unsigned_int32 flags)
{
	fileName = name;
	fileFlags = flags;
}

FileReference::~FileReference()
{
}


File::File()
{
	fileOpen = false;
	filePosition = 0;
}

File::~File()
{
	Close();
}

FileResult File::TranslateErrorCode(unsigned_int32 error, FileResult defaultResult)
{	
	if (error == ERROR_ACCESS_DENIED)
		return (kFileAccessDenied);

	if (error == ERROR_WRITE_PROTECT)		
		return (kFileWriteProtected);

	if ((error == ERROR_HANDLE_DISK_FULL) || (error == ERROR_DISK_FULL))
		return (kFileDiskFull);
	
	return (defaultResult);
}

FileResult File::Open(const char *name, FileOpenMode mode)
{
	if (!fileOpen)
	{ 		 
		DWORD access = (mode == kFileReadOnly) ? GENERIC_READ : GENERIC_READ | GENERIC_WRITE; 
		DWORD creation = (mode == kFileCreate) ? CREATE_ALWAYS : OPEN_EXISTING; 
			
		fileHandle = CreateFileA(FileName(name), access, FILE_SHARE_READ, nullptr, creation, FILE_ATTRIBUTE_NORMAL, nullptr); 
		if (fileHandle == INVALID_HANDLE_VALUE) return (TranslateErrorCode(GetLastError(), kFileOpenFailed));
		
		fileOpen = true;
		filePosition = 0;
		return (kFileOkay);
	}	
	return (kFileOpenFailed);
}

void File::Close(void)
{
	if (fileOpen)
	{
		fileOpen = false;
		filePosition = 0;
		
		CloseHandle(fileHandle);
	}
}

FileResult File::Read(void *buffer, unsigned_int32 size)
{
	if (!fileOpen) return (kFileNotOpen);
	
	Assert(buffer, "Reading to nullptr");
	
	LARGE_INTEGER	position;
	DWORD			actual;

	position.QuadPart = filePosition;
	SetFilePointerEx(fileHandle, position, nullptr, FILE_BEGIN);

	if ((ReadFile(fileHandle, buffer, size, &actual, nullptr)) && (actual == size))
	{
		filePosition += size;
		return (kFileOkay);
	}

	return (TranslateErrorCode(GetLastError(), kFileIOFailed));
	
}

FileResult File::Write(const void *buffer, unsigned_int32 size)
{
	if (!fileOpen) return (kFileNotOpen);
	
	Assert(buffer, "Writing from nullptr");

	LARGE_INTEGER	position;
	DWORD			actual;
		
	position.QuadPart = filePosition;
	SetFilePointerEx(fileHandle, position, nullptr, FILE_BEGIN);
		
	if ((WriteFile(fileHandle, buffer, size, &actual, nullptr)) && (actual == size))
	{
		filePosition += size;
		return (kFileOkay);
	}
		
	return (TranslateErrorCode(GetLastError(), kFileIOFailed));
	
	
}

unsigned_int64 File::GetSize(void) const
{
	if (!fileOpen) return (0);
	
	#if C4WINDOWS
	
		LARGE_INTEGER	size;
		
		GetFileSizeEx(fileHandle, &size);
		return (size.QuadPart);
	
	#elif C4POSIX
	
		struct stat		stat;
		
		fstat(fileDesc, &stat);
		return (stat.st_size);
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
}

unsigned_int64 File::SetPosition(int64 position, FilePositioningMode mode)
{
	if (!fileOpen) return (0);
	
	switch (mode)
	{
		case kFileBegin:
			
			filePosition = position;
			break;
		
		case kFileCurrent:
			
			filePosition += position;
			break;
		
		case kFileEnd:
			
			filePosition = GetSize() - position;
			break;
	}
	
	return (filePosition);
}

FileResult File::WritePad(int32 align)
{
	static const int32 zero[8] = {0, 0, 0, 0, 0, 0, 0, 0};
	
	if (!fileOpen) return (kFileNotOpen);
	
	align--;
	int32 len = ((filePosition + align) & ~align) - filePosition;
	if (len != 0) return (Write(zero, len));
	return (kFileOkay);
}

File& File::operator <<(const char *text)
{
	unsigned_int32 len = Text::GetTextLength(text);
	if (len != 0) Write(text, len);
	return (*this);
}


FileMgr::FileMgr(int)
{
}

FileMgr::~FileMgr()
{
}

EngineResult FileMgr::Construct(void)
{
	
	return (kEngineOkay);
}

void FileMgr::Destruct(void)
{

}

FileResult FileMgr::DeleteFile(const char *name)
{
	#if C4WINDOWS
	
		if (DeleteFileA(FileName(name))) return (kFileOkay);
		return (kFileDeleteFailed);
	
	#elif C4POSIX
	
		FilePath	path;
		
		TheFileMgr->GetFullPath(name, &path);
		
		if (unlink(path) != 0) return (kFileDeleteFailed);
		return (kFileOkay);
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
}

FileResult FileMgr::CreateDirectory(const char *directory)
{
	#if C4WINDOWS
	
		if (!CreateDirectoryA(FileName(directory), nullptr))
		{
			if (GetLastError() != ERROR_ALREADY_EXISTS) return (kFileCreateFailed);
		}
		
		return (kFileOkay);
	
	#elif C4POSIX
	
		FilePath	path;
		
		TheFileMgr->GetFullPath(directory, &path);
		
		if ((mkdir(path, 0777) != 0) && (errno != EEXIST)) return (kFileCreateFailed);
		return (kFileOkay);
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
}

FileResult FileMgr::CreateDirectoryPath(const char *path)
{
	if (path[0] != 0)
	{
		int32 start = 1;
		while (path[start] != 0)
		{
			String<kMaxFileNameLength>	directory;
			
			int32 slash = Text::FindChar(path + start, '/');
			if (slash < 0) break;
			
			directory.Set(path, start + slash);
			start += slash + 1;
			
			if (CreateDirectory(directory) != kFileOkay) return (kFileCreateFailed);
		}
	}
	
	return (kFileOkay);
}

bool FileMgr::DefaultFilter(const char *name, unsigned_int32 flags, void *cookie)
{
	return ((!(flags & kFileInvisible)) && (name[0] != '.'));
}

bool FileMgr::DirectoryFilter(const char *name, unsigned_int32 flags, void *cookie)
{
	return ((flags & kFileDirectory) && (name[0] != '.'));
}

void FileMgr::BuildFileList(const char *directory, List<FileReference> *list, BuildProc *proc, void *cookie)
{
	if (!proc) proc = &DefaultFilter;

	
		WIN32_FIND_DATA		findData;
		
		HANDLE h = FindFirstFileA(FileName(String<kMaxFileNameLength>(directory) += "/*.*"), &findData);
		if (h == INVALID_HANDLE_VALUE) return;
		
		do
		{
			unsigned_int32 flags = 0;
			if (findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) flags |= kFileDirectory;
			if (findData.dwFileAttributes & FILE_ATTRIBUTE_HIDDEN) flags |= kFileInvisible;
			
			const char *name = findData.cFileName;
			if ((*proc)(name, flags, cookie)) list->Append(new FileReference(name, flags));
			
		} while (FindNextFileA(h, &findData));
		
		FindClose(h);
	
}

// ZYURVUR
