#ifndef __CKGL15RASTERIZER_H__
#define __CKGL15RASTERIZER_H__ "$Id:$"
/*************************************************************************/
/*	File : CKGL15Rasterizer.h											 */
/*	Author : FLorian Delizy 											 */	
/*																		 */	
/*	+ OpenGL Rasterizer declaration										 */
/*	+ Some methods of these classes are already implemented in the 		 */
/*	CKRasterizerLib	library	as they are common to all rasterizers		 */
/*																		 */
/*	Virtools SDK 														 */	 
/*	Copyright (c) Virtools 2000, All Rights Reserved.					 */	
/*************************************************************************/



#ifdef macintosh
	#include "gl.h"
	#include "opengl/glext.h"
	typedef long GLWrapInt;	
#else
	#include <windows.h>
	#include <GL/gl.h>
	#include <GL/glu.h>

	#include "opengl/glext.h" // local version to have the last available
	#include "win32/wglext.h"

	typedef int GLWrapInt;
#endif

#include <CKRasterizer.h>

//#include "CKGL15RasterizerContext.h"

namespace VCKGL15Rasterizer // used to prevent conflicts in the lib version
{

	
	class CKGLRasterizerDriver;
	class CKGLRasterizerContext;
	class CKGLRasterizer;
	class CKGLContext;



	/******************************************************************
	 The render state mechanism of CKRasterizerContext is the same
	 than DirectX, for openGL which use a function for each state 
	 we create a table of render state functions that call the 
	 appropriate GL functions (see CKGLRasterizerContextFunct.cpp)
	******************************************************************/

	typedef void (*SetGLRenderStateFunc)(CKGLRasterizerContext* ctx,CKDWORD Value);
	typedef CKDWORD (*GetGLRenderStateFunc)(CKGLRasterizerContext* ctx); 
	typedef BOOL (*SetGLTextureStateFunc)(CKGLRasterizerContext* ctx,int Stage,CKDWORD Value);

	void glInitRenderStateFunctions(SetGLRenderStateFunc*,GetGLRenderStateFunc*,SetGLTextureStateFunc*);
	void glValidateExtensions(CKGLRasterizerDriver* Drv);

	//---------------------------------------------
	// DDS file reader function to allow CreateTextureFromFile() method to the GLRasterizer.
	extern "C" __declspec(dllexport) 
	BOOL glCreateTextureFromDDSFile(CKGLRasterizerContext* ctx, const char* iFilename, GLuint &oGlName, GLint &oInternalformat,
									int &ioMipMapCount, int &oBytesPerPixel, int &oWidth, int &oHeight, int &oDepth);


	/*******************************************
	A Frame Buffer object for OpenGL :
	*********************************************/
	class CKGLFrameBuffer
	{
	public:
		CKGLFrameBuffer(){}
		~CKGLFrameBuffer(){}

		virtual BOOL Create(CKGLRasterizerContext* iRstCtx, CKTextureDesc* iTex) = 0;
		virtual int	 Destroy() =0;

		virtual BOOL Bind() = 0;
		virtual BOOL MakeAsCurrentRenderCtx() = 0;
		virtual BOOL RestoreRenderCtx() = 0;

		virtual int	 AddRenderTarget(CKTextureDesc* iTex) = 0;
		virtual int  RemoveRenderTarget(CKTextureDesc* iTex) = 0;

		virtual int	 SetCurrentFace(CKRST_CUBEFACE iFace) = 0;
	};

	typedef struct CKGLRenderBuffer
	{
	public:
		GLuint m_GLDepthRBIndex;
		GLuint m_GLStencilRBIndex;
		CKGLRenderBuffer(void)  {m_RefCount = 0; m_GLDepthRBIndex=0; m_GLStencilRBIndex=0;}
		~CKGLRenderBuffer(void) {}

	private:
		unsigned int m_RefCount;
	}CKGLRenderBuffer;

#define CKGL_MAX_RENDER_TARGET 4
	class CKGLFrameBufferObject : CKGLFrameBuffer
	{
	public:
		CKGLFrameBufferObject(){m_FBO_Id=0;m_RenderBuffer=NULL;m_Width=m_Height=0;m_RefCount=m_TargetCount=0;m_RstCtx=NULL;m_BindTarget=GL_TEXTURE_2D;}
		~CKGLFrameBufferObject(){;}
		virtual BOOL Create(CKGLRasterizerContext* iRstCtx, CKTextureDesc* iTex);
		virtual int	 Destroy();

		virtual BOOL Bind();
		virtual BOOL MakeAsCurrentRenderCtx();
		virtual BOOL RestoreRenderCtx();

		virtual int	 AddRenderTarget(CKTextureDesc* iTex);
		virtual int  RemoveRenderTarget(CKTextureDesc* iTex);

		virtual int	 SetCurrentFace(CKRST_CUBEFACE iFace);

	protected:
		GLuint			  m_FBO_Id;
		GLuint			  m_RenderTargets[CKGL_MAX_RENDER_TARGET];
		GLuint			  m_BindTarget;
		CKGLRenderBuffer* m_RenderBuffer;
		int				  m_Width;
		int				  m_Height;
		int				  m_GLRcWidth;
		int				  m_GLRcHeight;
		int				  m_RefCount;
		int				  m_TargetCount;
		BOOL			  m_doMipMap;
		CKGLRasterizerContext* m_RstCtx;
	};


	//---------------------------------------------
	// Texture parameter as set by glTexParameter()
	// that are unique to each texture object 
	// need to keep track of these as they may invalidate the 
	// texture stage states cache
	typedef struct CKGLTexParameter 
	{
		CKDWORD BorderColor;
		CKDWORD MinFilter;
		CKDWORD MagFilter;
		CKDWORD AddressU;
		CKDWORD AddressV;

		CKGLTexParameter(void) : 
			MinFilter(0xFFFFFFFF), MagFilter(0xFFFFFFFF), 
			AddressU(0), AddressV(0)
		{
		}

	} CKGLTexParameter;

	//---------------------------------------------
	// Video memory of a texture can not be locked directly 
	// instead we can copy it to a system memory surface
	// and return the pointer to the user...
	typedef struct CKGLTexLockData 
	{
		VxImageDescEx	Image;
		VxMemoryPool	ImageMemory;
		DWORD			LockFlags;
	} CKGLTexLockData;

	/*******************************************
	 A texture object for OpenGL : contains a cache
	 for its attribute (Blend,Filter,etc...) and 
	 the index the texture is binded to...
	*********************************************/
	typedef struct CKGLTextureDesc : public CKTextureDesc 
	{


		public:
			CKGLTexParameter TexParam;
			int	 GLTextureIndex;

			CKGLFrameBuffer* GLFrameBuffer;
			CKGLTexLockData* LockData;
			
		public:
			void Release(void);
			CKGLTextureDesc(void)  { LockData = NULL; GLTextureIndex=-1; GLFrameBuffer=NULL;}
			~CKGLTextureDesc(void) { Release(); GLTextureIndex=-1; }

	} CKGLTextureDesc;


	/*******************************************
	 A vertex buffer object for OpenGL :
	*******************************************/
	#ifdef _DEBUG
	extern int g_nbVboAllocated; // Used to keep track of creation/deletion of vbo debug only
	extern int g_nbIboAllocated; // Used to keep track of creation/deletion of ibo debug only
	#endif // #ifdef _DEBUG


	typedef struct CKGLVertexBufferDesc : public CKVertexBufferDesc
	{
		private :
			CKGLRasterizerContext * m_parent;
		
		public :
			GLuint m_glName; 						   
			BOOL m_isBounded;
			GLenum m_usage;



		public : 

			CKGLVertexBufferDesc(CKGLRasterizerContext * parent) : m_parent(parent), m_glName(0), m_isBounded(FALSE) 
			{
				#ifdef _DEBUG
				g_nbVboAllocated ++;
				#endif // #ifdef _DEBUG
			};
			
			~CKGLVertexBufferDesc(void);

			CKGLVertexBufferDesc& operator=(const CKGLVertexBufferDesc& src) 
			{ 
				m_glName = src.m_glName; 
				return *this;
			}
	} CKGLVertexBufferDesc;

	// Shared Vertex Buffer desc :

	// This structure is only used for dynamic buffers
	struct CKGLSharedVBDesc : public CKSharedVBDesc
	{
		public :

			CKDWORD			m_VBid;			// VB Name for CKGL15Rasterizer (which does not create shared vertex buffers)
			CKGLRasterizerContext * m_parent;
			
		
		public :

			CKGLSharedVBDesc(void) : m_VBid(-1), m_parent(NULL) {}

			CKGLSharedVBDesc(CKGLRasterizerContext * ctx);

			~CKGLSharedVBDesc(void);
	};


	// Shared Index Buffer desc


	// This structure is only used for dynamic buffers
	struct CKGLSharedIBDesc : public CKSharedIBDesc
	{
		public :

			CKDWORD			m_IBid;			// Store the IB id if the Shared IB is not shared.

			CKGLSharedIBDesc(void) : m_IBid(-1), m_parent(NULL) {}
			CKGLRasterizerContext * m_parent;

		public :

			CKGLSharedIBDesc(CKGLRasterizerContext * ctx);

			~CKGLSharedIBDesc(void);


	};
	

	/*********************************************
	An Index Buffer object for OpenGL :
	**********************************************/

	typedef struct CKGLIndexBufferDesc : public CKIndexBufferDesc
	{
		private :
			CKGLRasterizerContext * m_parent;

		public :
			GLuint m_glName; 
			BOOL m_isBounded;
			GLenum m_usage;

		
		public : 


			CKGLIndexBufferDesc(CKGLRasterizerContext * parent) : m_parent(parent), m_glName(0), m_isBounded(FALSE) 
			{
				#ifdef _DEBUG
				g_nbIboAllocated  ++;
				#endif // #ifdef _DEBUG
			};

			~CKGLIndexBufferDesc(void);
	
			CKGLIndexBufferDesc& operator=(const CKGLIndexBufferDesc& src) 
			{ 
				m_glName = src.m_glName; 
				return *this;
			}

	} CKGLIndexBufferDesc;



	/****************************************************************/
	/* CKGLRasterizer : the rasterizer itself						*/
	/****************************************************************/
	class CKGLRasterizer : public CKRasterizer
	{
		public:
			CKGLRasterizer(void);
			virtual ~CKGLRasterizer(void);

			virtual BOOL Start(WIN_HANDLE AppWnd);
			virtual void Close(void);

		public:

			BOOL					m_Init;
	};



} // namespace CKGL15Rasterizer

#endif // __CKGL15RASTERIZER_H__