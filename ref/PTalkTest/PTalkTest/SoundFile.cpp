// SoundFile.cpp: implementation of the CSoundFile class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundFile.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CSoundFile::CSoundFile(std::wstring FileName, WAVEFORMATEX* format)
{
	m_hFile = NULL;
	m_FileName = std::wstring(FileName);

	ZeroMemory(&m_MMCKInfoParent,sizeof(MMCKINFO));
	ZeroMemory(&m_MMCKInfoChild,sizeof(MMCKINFO));
	ZeroMemory(&m_MMCKInfoData,sizeof(MMCKINFO));
	
	if(format == NULL)
	{
		m_Mode = READ;
		OpenWaveFile();
	}
	else
	{
		m_Mode = WRITE;
		m_Format = *format;
		CreateWaveFile();
	}
	
	if(m_Mode == ERROR) 
		Close();
}

CSoundFile::~CSoundFile()
{
	Close();
}

void CSoundFile::Close()
{
	if(m_hFile)
	{
		if(m_Mode == WRITE)
		{
		::mmioAscend(m_hFile, &m_MMCKInfoChild, 0);
		::mmioAscend(m_hFile, &m_MMCKInfoParent, 0);
		}
		::mmioClose(m_hFile, 0);
		m_hFile = NULL;
	}	

}

bool CSoundFile::Write(CBuffer *buffer)
{
	if(m_Mode == WRITE)
	{
		int length = mmioWrite(m_hFile, buffer->ptr.c, buffer->ByteLen);
		if(length == buffer->ByteLen)
			return true;
	}
	return false;
}

CBuffer* CSoundFile::Read()
{
	// create a new buffer
	CBuffer* buf = new CBuffer(m_Format.nBlockAlign*m_MMCKInfoChild.cksize);
	if(buf == NULL) 
		return NULL;
	
	if(Read(buf))
		return buf;

	// if we reach here there was an error
	delete buf;
	return NULL;
}
int CSoundFile::ReadData(char ** ptr)
{
	int nSize = m_Format.nBlockAlign*m_MMCKInfoChild.cksize;
	char* buff = new char[nSize];

	DWORD readSize;

	HGLOBAL mh_read_data = NULL;


	// 데이터의 크기만큼 글로벌 메모리 영역을 할당받는다.
	mh_read_data = ::GlobalAlloc(GMEM_MOVEABLE, nSize);
	// 메모리를 사용할 수 있게 p_data와 연결시킨다.
	char *p_data = (char *)::GlobalLock(mh_read_data);

	// 해당 메모리 영역에 웨이브 파일에서 읽은 데이터를 읽어서 저장한다.
	int nByte = ::mmioRead(m_hFile, p_data, nSize);
	
	// p_data와 글로벌 메모리의 연결을 해제한다.
	::GlobalUnlock(p_data);

	//ReadFile(m_hFile, buff, nSize, &readSize, 0);

	//int nByte = ::mmioRead(m_hFile, buff, nSize);
	if (nByte > 0)
	{	
		*ptr = p_data;
		return nByte;
	}	
	return 0;
}

bool CSoundFile::Read(CBuffer *buffer)
{
	if(m_Mode == READ)
	{		
		int nByte = ::mmioRead(m_hFile, buffer->ptr.c, buffer->ByteLen);
		if (nByte > 0)
		{
			buffer->ByteLen = nByte;
			return true;
		}
	}
	return false;
}

bool CSoundFile::CreateWaveFile()
{
	// check if file is already open
	if(m_hFile) 
		return FALSE;	
	
	// open file

	m_hFile = ::mmioOpen((LPWSTR)m_FileName.c_str(), NULL, MMIO_CREATE | MMIO_WRITE | MMIO_EXCLUSIVE | MMIO_ALLOCBUF);
	if(m_hFile == NULL) 
	{
		m_Mode = FILE_ERROR;
		return FALSE;
	}

	ZeroMemory(&m_MMCKInfoParent, sizeof(MMCKINFO));
	m_MMCKInfoParent.fccType = mmioFOURCC('W','A','V','E');

	MMRESULT mmResult = ::mmioCreateChunk( m_hFile,&m_MMCKInfoParent, MMIO_CREATERIFF);
	
	ZeroMemory(&m_MMCKInfoChild, sizeof(MMCKINFO));
	m_MMCKInfoChild.ckid = mmioFOURCC('f','m','t',' ');
	m_MMCKInfoChild.cksize = sizeof(WAVEFORMATEX) + m_Format.cbSize;
	mmResult = ::mmioCreateChunk(m_hFile, &m_MMCKInfoChild, 0);
	mmResult = ::mmioWrite(m_hFile, (char*)&m_Format, sizeof(WAVEFORMATEX) + m_Format.cbSize); 
	mmResult = ::mmioAscend(m_hFile, &m_MMCKInfoChild, 0);
	m_MMCKInfoChild.ckid = mmioFOURCC('d', 'a', 't', 'a');
	mmResult = ::mmioCreateChunk(m_hFile, &m_MMCKInfoChild, 0);

	return TRUE;
}

bool CSoundFile::OpenWaveFile()
{
	// code taken from Visual C++ Multimedia -- Aitken and Jarol p 122
	
	// check if file is already open
	if(m_hFile) 
		return FALSE; 

	m_hFile = ::mmioOpen((LPWSTR)m_FileName.c_str(),  NULL, MMIO_READ);
	if(m_hFile == NULL) 
	{
		m_Mode = FILE_ERROR;
		return FALSE;
	}

	m_MMCKInfoParent.fccType = mmioFOURCC('W','A','V','E');
	MMRESULT mmResult = ::mmioDescend(m_hFile, &m_MMCKInfoParent,NULL,MMIO_FINDRIFF);
	if(mmResult)
	{
		//AfxMessageBoxA("Error descending into file");
		mmioClose(m_hFile,0);
		m_hFile = NULL;
		m_Mode = FILE_ERROR;
		return FALSE;
	}
	m_MMCKInfoChild.ckid = mmioFOURCC('f','m','t',' ');
	mmResult = ::mmioDescend(m_hFile,&m_MMCKInfoChild,&m_MMCKInfoParent,MMIO_FINDCHUNK);
	if(mmResult)
	{
		//AfxMessageBox("Error descending in wave file");
		mmioClose(m_hFile,0);
		m_Mode = FILE_ERROR;
		m_hFile = NULL;
		return FALSE;
	}
	DWORD bytesRead = ::mmioRead(m_hFile,(LPSTR)&m_Format,m_MMCKInfoChild.cksize);
	if(bytesRead < 0)
	{
		//AfxMessageBox("Error reading PCM wave format record");
		mmioClose(m_hFile,0);
		m_Mode = FILE_ERROR;
		return FALSE;
	}
	
	// open output sound file
	mmResult = ::mmioAscend(m_hFile,&m_MMCKInfoChild,0);
	if(mmResult)
	{
		//AfxMessageBox("Error ascending in File");
		mmioClose(m_hFile,0);
		m_hFile = NULL;
		m_Mode = FILE_ERROR;
		return FALSE;
	}
	m_MMCKInfoChild.ckid = mmioFOURCC('d','a','t','a');
	mmResult = ::mmioDescend(m_hFile,&m_MMCKInfoChild,
		&m_MMCKInfoParent,MMIO_FINDCHUNK);
	if(mmResult)
	{
		//("error reading data chunk");
		mmioClose(m_hFile,0);
		m_hFile = NULL;
		m_Mode = FILE_ERROR;
		return FALSE;
	}

	return TRUE;
}

EREADWRITE CSoundFile::GetMode()
{
	return m_Mode;
}

bool CSoundFile::IsOK()
{
	if(m_Mode == FILE_ERROR)
		return false;
	else
		return true;
}
