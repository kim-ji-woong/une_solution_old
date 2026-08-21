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


#ifndef C4TextureGeneration_h
#define C4TextureGeneration_h


#include "C4TextureImporter.h"
#include "C4Cameras.h"


namespace C4
{
	enum
	{
		kGenerateLightProjection	= 1 << 0,
		kGenerateEnvironmentMap		= 1 << 1,
		kGenerateAmbientSpace		= 1 << 2,
		kGenerateImpostorImage		= 1 << 3
	};
	
	
	class CubeCamera : public FrustumCamera
	{
		friend class Camera;
		
		private:
			
			int32	faceIndex;
			
			CubeCamera(bool);
			CubeCamera(const CubeCamera& cubeCamera);
			
			Node *Replicate(void) const override;
			
			void CalculateWorldTransform(void) override;
		
		public:
			
			static const ConstMatrix3D cameraRotation[6];
			
			CubeCamera();
			~CubeCamera();
			
			int32 GetFaceIndex(void) const
			{
				return (faceIndex);
			}
			
			void SetFaceIndex(int32 index)
			{
				faceIndex = index;
			}
	};
	
	
	class AmbientTextureGenerator : public Window, public ListElement<AmbientTextureGenerator>
	{
		friend class TextureTool;
		
		private:
			
			enum
			{
				kRenderSize		= 32,
				kRenderArea		= kRenderSize * kRenderSize
			};
			
			ResourceName			textureName;
			
			PushButtonWidget		*stopButton;
			ProgressWidget			*progressBar;
			TextWidget				*nameText;
			
			CubeCamera				cubeCamera;
			float					nearDistance;
			float					cameraSurfaceOffset;
			float					samplingRadius;
			
			World					*generateWorld;
			AmbientSpace			*ambientSpace;
			
			int32					textureWidth;
			int32					textureHeight;
			int32					textureDepth;
			
			int32					indexI;
			int32					indexJ;
			int32					indexK;
			
			float					deltaX;
			float					deltaY;
			float					deltaZ;
			
			unsigned_int32			*renderData;
			Color4C					*imageBuffer;
			
			static List<AmbientTextureGenerator>	windowList;
			
			void InvalidateAllShaderData(void) const;
			bool DetectSolidEntrance(const Point3D& p1, const Point3D& p2, CollisionData *data) const;
			
			TextureImportResult WriteTextureResource(void);
		
		public: 
			
			AmbientTextureGenerator(World *world, AmbientSpace *ambientSpace, const char *name); 
			~AmbientTextureGenerator(); 
			 
			void Preprocess(void);
			void Move(void); 
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	}; 
	
	
	class TextureGenerator : public TextureImporter
	{ 
		private:
			
			World		*currentWorld;
			int32		filterSize;
			
			void GetOutputTexturePath(ResourcePath *path) const;
			
			void FilterCubeTexture(Color4C *image, int32 pixelCount, int32 width);
			static void GenerateAmbientMap(Color4C *image, int32 pixelCount, int32 width);
		
		public:
			
			TextureGenerator(World *world, const char *name);
			~TextureGenerator();
			
			void SetFilterSize(int32 size)
			{
				filterSize = size;
			}
			
			TextureImportResult GenerateCubeTexture(Node *node, int32 width, TextureFormat format);
			TextureImportResult GenerateSpotTexture(Node *node, float apex, float aspect, int32 width, TextureFormat format);
			TextureImportResult GenerateImpostorTexture(Node *node, const ImpostorProperty *property);
	};
	
	
	class TextureGeneratorWindow : public Window, public Singleton<TextureGeneratorWindow>
	{
		private:
			
			PushButtonWidget	*generateButton;
			PushButtonWidget	*cancelButton;
			
			CheckWidget			*lightBox;
			CheckWidget			*environmentBox;
			CheckWidget			*ambientBox;
			CheckWidget			*impostorBox;
			
			TextureGeneratorWindow();
		
		public:
			
			~TextureGeneratorWindow();
			
			static void Open(void);
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	extern TextureGeneratorWindow *TheTextureGeneratorWindow;
}


#endif

// ZYURVUR
