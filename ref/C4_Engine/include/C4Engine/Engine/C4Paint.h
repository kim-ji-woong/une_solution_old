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


#ifndef C4Paint_h
#define C4Paint_h


//# \component	Interface Manager
//# \prefix		InterfaceMgr/


#include "C4SpaceObjects.h"
#include "C4Panels.h"


namespace C4
{
	enum
	{
		kWidgetPaint			= 'PANT'
	};
	
	
	enum
	{
		kFunctionGetPaintWidgetBrushRadius		= 'GBRD',
		kFunctionSetPaintWidgetBrushRadius		= 'SBRD',
		kFunctionGetPaintWidgetBrushFuzziness	= 'GBFZ',
		kFunctionSetPaintWidgetBrushFuzziness	= 'SBFZ',
		kFunctionGetPaintWidgetBrushColor		= 'GBCL',
		kFunctionSetPaintWidgetBrushColor		= 'SBCL'
	};
	
	
	enum
	{
		kPaintMinResolution		= 16,
		kPaintMaxResolution		= 1024
	};
	
	
	class PaintState : public Packable
	{
		private:
			
			float				brushRadius;
			float				brushFuzziness;
			float				brushOpacity;
			ColorRGBA			brushColor;
			
			bool				channelMask[4];
		
		public:
			
			C4API PaintState();
			C4API PaintState(const PaintState& paintState);
			C4API ~PaintState();
			
			float GetBrushRadius(void) const
			{
				return (brushRadius);
			}
			
			void SetBrushRadius(float radius)
			{
				brushRadius = radius;
			}
			
			float GetBrushFuzziness(void) const
			{
				return (brushFuzziness);
			}
			
			void SetBrushFuzziness(float fuzziness)
			{
				brushFuzziness = fuzziness;
			}
			
			float GetBrushOpacity(void) const
			{
				return (brushOpacity);
			}
			
			void SetBrushOpacity(float opacity)
			{
				brushOpacity = opacity;
			}
			
			const ColorRGBA& GetBrushColor(void) const
			{
				return (brushColor);
			}
			
			void SetBrushColor(const ColorRGBA& color)
			{
				brushColor = color;
			}
			
			const bool *GetChannelMask(void) const
			{
				return (channelMask);
			}
			
			void SetChannelMask(bool red, bool green, bool blue, bool alpha)
			{ 
				channelMask[0] = red;
				channelMask[1] = green; 
				channelMask[2] = blue; 
				channelMask[3] = alpha; 
			}
			 
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
	}; 
	
	
	class Painter
	{ 
		private:
			
			Integer2D			paintResolution;
			int32				channelCount;
			
			void				*paintImage;
			const PaintState	*paintState;
			
			unsigned_int8		*previousImage;
			unsigned_int8		*transferImage;
			
			Rect				paintBounds;
			
			void UpdateImage1(void);
			void UpdateImage2(void);
			void UpdateImage4(void);
		
		public:
			
			C4API Painter(const Integer2D& resolution, int32 count, void *image, const PaintState *state);
			C4API ~Painter();
			
			const Rect& GetPaintBounds(void) const
			{
				return (paintBounds);
			}
			
			C4API void BeginPainting(void);
			C4API void EndPainting(void);
			
			C4API bool UpdateImage(void);
			
			C4API const void *CreateUndoImage(const Rect& rect) const;
			C4API static void ApplyUndoImage(const PaintSpaceObject *object, const Rect& rect, const void *undoImage);
			C4API static void ReleaseUndoImage(const void *undoImage);
			
			C4API void DrawDot(const Point2D& p);
			C4API void DrawLine(const Point2D& p1, const Point2D& p2);
	};
	
	
	//# \class	PaintWidget		The interface widget that displays a painting canvas.
	//
	//# The $PaintWidget$ class represents an interface widget that displays a painting canvas.
	//
	//# \def	class PaintWidget : public ImageWidget
	//
	//# \ctor	PaintWidget(const Vector2D& size, const Integer2D& resolution);
	//
	//# \param	size		The size of the quad in which the canvas is rendered, in pixels.
	//# \param	resolution	The internal resolution of the painting canvas, in pixels.
	//
	//# \desc
	//# The $PaintWidget$ class displays an interactive painting canvas.
	//
	//# \base	ImageWidget		An $PaintWidget$ is a specialized $ImageWidget$.
	
	
	class PaintWidget : public ImageWidget
	{
		friend class WidgetReg<PaintWidget>;
		
		private:
			
			Integer2D			paintResolution;
			ColorRGBA			backgroundColor;
			PaintState			paintState;
			
			Point2D				brushPosition;
			
			Color4C				*paintImage;
			Painter				*painter;
			
			TextureHeader		textureHeader;
			
			PaintWidget();
			PaintWidget(const PaintWidget& paintWidget);
			
			Widget *Replicate(void) const override;
			
			void Initialize(void);
			
			void UpdateImage(void);
		
		public:
			
			C4API PaintWidget(const Vector2D& size, const Integer2D& resolution);
			C4API ~PaintWidget();
			
			PaintState *GetPaintState(void)
			{
				return (&paintState);
			}
			
			const PaintState *GetPaintState(void) const
			{
				return (&paintState);
			}
			
			static void RegisterFunctions(ControllerRegistration *registration);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Preprocess(void);
			
			void HandleMouseEvent(const PanelMouseEventData *eventData);
	};
	
	
	class GetPaintWidgetBrushRadiusFunction : public WidgetFunction
	{
		private:
			
			GetPaintWidgetBrushRadiusFunction(const GetPaintWidgetBrushRadiusFunction& getPaintWidgetBrushRadiusFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			GetPaintWidgetBrushRadiusFunction();
			~GetPaintWidgetBrushRadiusFunction();
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class SetPaintWidgetBrushRadiusFunction : public WidgetFunction
	{
		private:
			
			float		brushRadius;
			
			SetPaintWidgetBrushRadiusFunction(const SetPaintWidgetBrushRadiusFunction& setPaintWidgetBrushRadiusFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			SetPaintWidgetBrushRadiusFunction();
			~SetPaintWidgetBrushRadiusFunction();
			
			float GetBrushRadius(void) const
			{
				return (brushRadius);
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
	
	
	class GetPaintWidgetBrushFuzzinessFunction : public WidgetFunction
	{
		private:
			
			GetPaintWidgetBrushFuzzinessFunction(const GetPaintWidgetBrushFuzzinessFunction& getPaintWidgetBrushFuzzinessFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			GetPaintWidgetBrushFuzzinessFunction();
			~GetPaintWidgetBrushFuzzinessFunction();
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class SetPaintWidgetBrushFuzzinessFunction : public WidgetFunction
	{
		private:
			
			float		brushFuzziness;
			
			SetPaintWidgetBrushFuzzinessFunction(const SetPaintWidgetBrushFuzzinessFunction& setPaintWidgetBrushFuzzinessFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			SetPaintWidgetBrushFuzzinessFunction();
			~SetPaintWidgetBrushFuzzinessFunction();
			
			float GetBrushFuzziness(void) const
			{
				return (brushFuzziness);
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
	
	
	class GetPaintWidgetBrushColorFunction : public WidgetFunction
	{
		private:
			
			GetPaintWidgetBrushColorFunction(const GetPaintWidgetBrushColorFunction& getPaintWidgetBrushColorFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			GetPaintWidgetBrushColorFunction();
			~GetPaintWidgetBrushColorFunction();
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class SetPaintWidgetBrushColorFunction : public WidgetFunction
	{
		private:
			
			ColorRGBA		brushColor;
			
			SetPaintWidgetBrushColorFunction(const SetPaintWidgetBrushColorFunction& setPaintWidgetBrushColorFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			SetPaintWidgetBrushColorFunction();
			~SetPaintWidgetBrushColorFunction();
			
			const ColorRGBA& GetBrushColor(void) const
			{
				return (brushColor);
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
}


#endif

// ZYURVUR
