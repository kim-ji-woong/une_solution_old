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


#ifndef C4TextureImporter_h
#define C4TextureImporter_h


#include "C4Plugins.h"
#include "C4Configuration.h"
#include "C4TextureTool.h"


namespace C4
{
	enum
	{
		kMaxTextureImportCount		= 3
	};
	
	
	enum
	{
		kTextureImportMipmaps				= 1 << 0,
		kTextureImportVectorData			= 1 << 1,
		kTextureImportInvertGreen			= 1 << 2,
		kTextureImportHalfScale				= 1 << 3,
		kTextureImportFlipVertical			= 1 << 4,
		kTextureImportCompressionBC13		= 1 << 5,
		kTextureImportBleedAlphaTest		= 1 << 6,
		kTextureImportNormalMap				= 1 << 7,
		kTextureImportParallaxMap			= 1 << 8,
		kTextureImportHorizonMap			= 1 << 9,
		kTextureImportHorizonHalfScale		= 1 << 10,
		kTextureImportAmbientOcclusion		= 1 << 11,
		kTextureImportApplyHaze				= 1 << 12,
		kTextureImportRemember				= 1 << 13
	};
	
	
	class CompressionBC1Job : public BatchJob
	{
		private:
			
			int32			imageWidth;
			Rect			blockRect;
			const Color4C	*sourceImage;
			unsigned_int8	*outputCode;
			
			static void Compress(Job *job, void *cookie);
		
		public:
			
			CompressionBC1Job(int32 width, const Rect& rect, const Color4C *source, unsigned_int8 *output);
			~CompressionBC1Job();
	};
	
	
	class CompressionBC3Job : public BatchJob
	{
		private:
			
			int32			imageWidth;
			Rect			blockRect;
			const Color4C	*sourceImage;
			unsigned_int8	*outputCode;
			
			static void Compress(Job *job, void *cookie);
		
		public:
			
			CompressionBC3Job(int32 width, const Rect& rect, const Color4C *source, unsigned_int8 *output);
			~CompressionBC3Job();
	};
	
	
	class CompressionNormalBC3Job : public BatchJob
	{
		private:
			
			int32			imageWidth;
			Rect			blockRect;
			const Color4C	*sourceImage;
			unsigned_int8	*outputCode;
			
			static void Compress(Job *job, void *cookie);
		
		public:
			
			CompressionNormalBC3Job(int32 width, const Rect& rect, const Color4C *source, unsigned_int8 *output);
			~CompressionNormalBC3Job();
	};
	
	
	class AmbientJob : public BatchJob
	{
		private:
			
			int32			textureWidth;
			int32			textureHeight;
			Rect			computeRect;
			const Color4C	*sourceImage;
			Color4C			*destinImage;
			float			heightScale;
			bool			swrapFlag;
			bool			twrapFlag; 
			
			static void Compute(Job *job, void *cookie); 
		 
		public: 
			
			AmbientJob(int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *destin, float scale, bool swrap, bool twrap); 
			~AmbientJob();
	};
	
	 
	class HorizonJob : public BatchJob
	{
		private:
			 
			int32			horizonIndex;
			int32			textureWidth;
			int32			textureHeight;
			Rect			computeRect;
			const Color4C	*sourceImage;
			Color4C			*destinImage;
			float			heightScale;
			bool			swrapFlag;
			bool			twrapFlag;
			
			static void Compute(Job *job, void *cookie);
		
		public:
			
			HorizonJob(int32 index, int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *destin, float scale, bool swrap, bool twrap);
			~HorizonJob();
	};
	
	
	class BleedAlphaTestJob : public BatchJob
	{
		private:
			
			int32			textureWidth;
			int32			textureHeight;
			Rect			computeRect;
			const Color4C	*sourceImage;
			Color4C			*destinImage;
			unsigned_int32	testValue;
			
			static void Compute(Job *job, void *cookie);
		
		public:
			
			BleedAlphaTestJob(int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *destin, unsigned_int32 test);
			~BleedAlphaTestJob();
	};
	
	
	class BleedNormalJob : public BatchJob
	{
		private:
			
			int32			textureWidth;
			int32			textureHeight;
			Rect			computeRect;
			const Color4C	*sourceImage;
			Color4C			*destinImage;
			
			static void Compute(Job *job, void *cookie);
		
		public:
			
			BleedNormalJob(int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *destin);
			~BleedNormalJob();
	};
	
	
	class TextureImporter : public Configurable
	{
		private:
		
			ResourceName		inputTextureName;
			ResourceName		outputTextureName;
			unsigned_int32		textureImportFlags;
			
			float				heightScale;
			int32				heightChannel;
			int32				horizonIndex;
			
			float				parallaxScale;
			Point2D				imageCenter;
			
			Vector2D			impostorSize;
			float				impostorClipData[4];
			
			ColorRGBA			hazeColor;
			int32				hazeElevation;
			
			Color4C				*textureImage[kMaxTextureImportCount];
			TextureHeader		textureHeader[kMaxTextureImportCount];
			
			TextureImportResult ValidateSettings(int32 *textureCount, int32 *textureComponentCount);
			
			static void CopyTarga_L8(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			static void CopyTarga_RGB16(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			static void CopyTarga_RGB24(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			static void CopyTarga_RGB32(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			static void CopyTarga_RGBA32(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			
			static void InvertGreenChannel(Color4C *image, int32 pixelCount);
			static void ClearAlphaChannel(Color4C *image, int32 pixelCount);
			
			static void CopyRedToAlpha(Color4C *image, int32 pixelCount);
			static void CopyGreenToAlpha(Color4C *image, int32 pixelCount);
			static void CopyBlueToAlpha(Color4C *image, int32 pixelCount);
			
			static void CopyGreenToRed(Color4C *image, int32 pixelCount);
			static void CopyBlueToRed(Color4C *image, int32 pixelCount);
			static void CopyAlphaToRed(Color4C *image, int32 pixelCount);
			
			static void Convert_RGBVector_XY(Color4C *image, int32 pixelCount);
			static void Convert_RGBA8_LA8(const Color4C *source, Color2C *restrict destin, int32 pixelCount);
			static void Convert_RGBA8_L8(const Color4C *source, Color1C *restrict destin, int32 pixelCount);
			
			static void ApplyHazeColor(Color4C *image, int32 width, int32 height, const ColorRGBA& color, int32 elevation);
			
			TextureImportResult ScaleHalfResolution(int32 index);
			
			void ProcessImage(int32 textureIndex, int32 componentCount, Color4C *processedImage);
			void ProcessImage(int32 textureIndex, int32 componentCount, Color2C *processedImage);
			void ProcessImage(int32 textureIndex, int32 componentCount, Color1C *processedImage);
			
			unsigned_int32 GetAuxiliaryDataSize(void) const;
			void WriteAuxiliaryData(File& file) const;
			
			static int32 BlockCompressImageBC1(int32 count, int32 width, int32 height, const Color4C *image, unsigned_int8 *data);
			static int32 BlockCompressImageBC3(int32 count, int32 width, int32 height, const Color4C *image, unsigned_int8 *data);
			static int32 BlockCompressNormalImageBC3(int32 count, int32 width, int32 height, const Color4C *image, unsigned_int8 *data);
			
			void GetInputConfigPath(ResourcePath *path) const;
			void WriteCommandLine(File& file);
		
		protected:
			
			void SetImpostorSize(float radius, float height)
			{
				impostorSize.Set(radius, height);
			}
			
			void SetImpostorClipData(float r1, float r2, float h1, float h2)
			{
				impostorClipData[0] = r1;
				impostorClipData[1] = r2;
				impostorClipData[2] = h1;
				impostorClipData[3] = h2;
			}
			
			virtual void GetOutputTexturePath(ResourcePath *path) const;
		
		public:
			
			C4TEXTUREAPI TextureImporter(const char *name, unsigned_int32 importFlags = 0);
			C4TEXTUREAPI ~TextureImporter();
			
			const ResourceName& GetTextureName(void) const
			{
				return (outputTextureName);
			}
			
			unsigned_int32 GetTextureImportFlags(void) const
			{
				return (textureImportFlags);
			}
			
			void SetTextureImportFlags(unsigned_int32 flags)
			{
				textureImportFlags = flags;
			}
			
			TextureHeader *GetTextureHeader(int32 textureIndex = 0)
			{
				return (&textureHeader[textureIndex]);
			}
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			C4TEXTUREAPI TextureImportResult SetTextureImage(int32 textureIndex, const char *name);
			C4TEXTUREAPI void SetTextureImage(int32 textureIndex, int32 width, int32 height, const Color4C *image);
			C4TEXTUREAPI void SetTextureImage(int32 textureIndex, int32 width, int32 height, const Color2C *image);
			C4TEXTUREAPI void SetTextureImage(int32 textureIndex, int32 width, int32 height, const Color1C *image);
			
			C4TEXTUREAPI TextureImportResult ImportTextureImage(void);
			
			C4TEXTUREAPI void ImportTexture(const char *name);
			C4TEXTUREAPI void ProcessCommandLine(const char *text);
	};
	
	
	class ImportTextureWindow : public Window, public ListElement<ImportTextureWindow>
	{
		friend class TextureTool;
		
		private:
			
			ResourceName			resourceName;
			TextureImporter			*textureImporter;
			
			PushButtonWidget		*importButton;
			PushButtonWidget		*cancelButton;
			
			ConfigurationWidget		*configurationWidget;
			
			static List<ImportTextureWindow>	windowList;
		
		public:
			
			ImportTextureWindow(const char *name);
			~ImportTextureWindow();
			
			static void Open(const char *name);
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
}


#endif

// ZYURVUR
