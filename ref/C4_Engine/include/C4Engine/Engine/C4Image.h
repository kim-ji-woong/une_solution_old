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


#ifndef C4Image_h
#define C4Image_h


#include "C4Resources.h"
#include "C4Bounding.h"


namespace C4
{
	typedef unsigned_int8	Color1C;
	
	
	enum
	{
		kMipmapNormalize		= 1 << 0,
		kMipmapBoostAlpha		= 1 << 1,
		kMipmapDampenAlpha		= 1 << 2
	};
	
	
	class Color2C
	{
		private:
			
			unsigned_int16	color;
		
		public:
			
			Color2C() {}
			
			Color2C(unsigned_int32 x, unsigned_int32 y)
			{
				#if C4BIGENDIAN
				
					color = (unsigned_int16) ((x << 8) | y);
				
				#else
				
					color = (unsigned_int16) ((y << 8) | x);
				
				#endif
			}
			
			bool operator ==(const Color2C& c) const
			{
				return (color == c.color);
			}
			
			bool operator !=(const Color2C& c) const
			{
				return (color != c.color);
			}
			
			Color2C& Set(unsigned_int32 x, unsigned_int32 y)
			{
				#if C4BIGENDIAN
				
					color = (unsigned_int16) ((x << 8) | y);
				
				#else
				
					color = (unsigned_int16) ((y << 8) | x);
				
				#endif
				
				return (*this);
			}
			
			Color2C& Clear(void)
			{
				color = 0;
				return (*this);
			}
			
			unsigned_int16 GetPackedColor(void) const
			{
				return (color);
			}
			
			Color2C& SetPackedColor(unsigned_int16 c)
			{
				color = c;
				return (*this);
			}
			
			unsigned_int32 GetRed(void) const
			{
				#if C4BIGENDIAN
					
					return ((color >> 8) & 0xFF);
				
				#else
					
					return (color & 0xFF);
				
				#endif
			}
			
			Color2C& SetRed(unsigned_int32 red)
			{
				#if C4BIGENDIAN 
					
					color = (unsigned_int16) ((color & 0x00FF) | (red << 8)); 
				 
				#else 
				
					color = (unsigned_int16) ((color & 0xFF00) | red); 
				
				#endif
				
				return (*this); 
			}
			
			unsigned_int32 GetGreen(void) const
			{ 
				#if C4BIGENDIAN
					
					return (color & 0xFF);
				
				#else
					
					return ((color >> 8) & 0xFF);
				
				#endif
			}
			
			Color2C& SetGreen(unsigned_int32 green)
			{
				#if C4BIGENDIAN
					
					color = (unsigned_int16) ((color & 0xFF00) | green);
				
				#else
				
					color = (unsigned_int16) ((color & 0x00FF) | (green << 8));
				
				#endif
				
				return (*this);
			}
			
			unsigned_int32 GetLum(void) const
			{
				#if C4BIGENDIAN
					
					return ((color >> 8) & 0xFF);
				
				#else
					
					return (color & 0xFF);
				
				#endif
			}
			
			Color2C& SetLum(unsigned_int32 lum)
			{
				#if C4BIGENDIAN
					
					color = (unsigned_int16) ((color & 0x00FF) | (lum << 8));
				
				#else
				
					color = (unsigned_int16) ((color & 0xFF00) | lum);
				
				#endif
				
				return (*this);
			}
			
			unsigned_int32 GetAlpha(void) const
			{
				#if C4BIGENDIAN
					
					return (color & 0xFF);
				
				#else
					
					return ((color >> 8) & 0xFF);
				
				#endif
			}
			
			Color2C& SetAlpha(unsigned_int32 alpha)
			{
				#if C4BIGENDIAN
					
					color = (unsigned_int16) ((color & 0xFF00) | alpha);
				
				#else
				
					color = (unsigned_int16) ((color & 0x00FF) | (alpha << 8));
				
				#endif
				
				return (*this);
			}
	};
	
	
	class Color3C
	{
		private:
			
			unsigned_int32	color;
		
		public:
			
			Color3C() {}
			
			Color3C(unsigned_int32 red, unsigned_int32 green, unsigned_int32 blue)
			{
				#if C4BIGENDIAN
				
					color = (red << 24) | (green << 16) | (blue << 8);
				
				#else
				
					color = (blue << 16) | (green << 8) | red;
				
				#endif
			}
			
			bool operator ==(const Color3C& c) const
			{
				return (color == c.color);
			}
			
			bool operator !=(const Color3C& c) const
			{
				return (color != c.color);
			}
			
			Color3C& Set(unsigned_int32 red, unsigned_int32 green, unsigned_int32 blue)
			{
				#if C4BIGENDIAN
				
					color = (red << 24) | (green << 16) | (blue << 8);
				
				#else
				
					color = (blue << 16) | (green << 8) | red;
				
				#endif
				
				return (*this);
			}
			
			Color3C& Clear(void)
			{
				color = 0;
				return (*this);
			}
			
			unsigned_int32 GetPackedColor(void) const
			{
				return (color);
			}
			
			Color3C& SetPackedColor(unsigned_int32 c)
			{
				#if C4BIGENDIAN
				
					color = c & 0xFFFFFF00;
				
				#else
				
					color = c & 0x00FFFFFF;
				
				#endif
				
				return (*this);
			}
			
			unsigned_int32 GetRed(void) const
			{
				#if C4BIGENDIAN
				
					return (color >> 24);
				
				#else
				
					return (color & 0xFF);
				
				#endif
			}
			
			Color3C& SetRed(unsigned_int32 red)
			{
				#if C4BIGENDIAN
				
					color = (color & 0x00FFFFFF) | (red << 24);
				
				#else
				
					color = (color & 0xFFFFFF00) | red;
				
				#endif
				
				return (*this);
			}
			
			unsigned_int32 GetGreen(void) const
			{
				#if C4BIGENDIAN
				
					return ((color >> 16) & 0xFF);
				
				#else
				
					return ((color >> 8) & 0xFF);
				
				#endif
			}
			
			Color3C& SetGreen(unsigned_int32 green)
			{
				#if C4BIGENDIAN
				
					color = (color & 0xFF00FFFF) | (green << 16);
				
				#else
				
					color = (color & 0xFFFF00FF) | (green << 8);
				
				#endif
				
				return (*this);
			}
			
			unsigned_int32 GetBlue(void) const
			{
				#if C4BIGENDIAN
				
					return ((color >> 8) & 0xFF);
				
				#else
				
					return ((color >> 16) & 0xFF);
				
				#endif
			}
			
			Color3C& SetBlue(unsigned_int32 blue)
			{
				#if C4BIGENDIAN
				
					color = (color & 0xFFFF00FF) | (blue << 8);
				
				#else
				
					color = (color & 0xFF00FFFF) | (blue << 16);
				
				#endif
				
				return (*this);
			}
	};
	
	
	class Color4C
	{
		private:
			
			unsigned_int32	color;
		
		public:
			
			Color4C() {}
			
			Color4C(unsigned_int32 red, unsigned_int32 green, unsigned_int32 blue, unsigned_int32 alpha)
			{
				#if C4BIGENDIAN
				
					color = (red << 24) | (green << 16) | (blue << 8) | alpha;
				
				#else
				
					color = (alpha << 24) | (blue << 16) | (green << 8) | red;
				
				#endif
			}
			
			Color4C(const Color3C& c, unsigned_int32 alpha)
			{
				#if C4BIGENDIAN
				
					color = c.GetPackedColor() | alpha;
				
				#else
				
					color = (alpha << 24) | c.GetPackedColor();
				
				#endif
			}
			
			Color4C& operator =(const Color3C& c)
			{
				color = c.GetPackedColor();
				return (*this);
			}
			
			bool operator ==(const Color4C& c) const
			{
				return (color == c.color);
			}
			
			bool operator !=(const Color4C& c) const
			{
				return (color != c.color);
			}
			
			Color4C& Set(unsigned_int32 red, unsigned_int32 green, unsigned_int32 blue, unsigned_int32 alpha)
			{
				#if C4BIGENDIAN
				
					color = (red << 24) | (green << 16) | (blue << 8) | alpha;
				
				#else
				
					color = (alpha << 24) | (blue << 16) | (green << 8) | red;
				
				#endif
				
				return (*this);
			}
			
			Color4C& Set(const Color3C& c, unsigned_int32 alpha)
			{
				#if C4BIGENDIAN
				
					color = c.GetPackedColor() | alpha;
				
				#else
				
					color = (alpha << 24) | c.GetPackedColor();
				
				#endif
				
				return (*this);
			}
			
			Color4C& Clear(void)
			{
				color = 0;
				return (*this);
			}
			
			Color4C& ClearMaxAlpha(void)
			{
				#if C4BIGENDIAN
				
					color = 0x000000FF;
				
				#else
				
					color = 0xFF000000;
				
				#endif
				
				return (*this);
			}
			
			Color4C& ExchangeRedBlue(void)
			{
				unsigned_int32 c = color;
				
				#if C4BIGENDIAN
				
					color = (c & 0x00FF00FF) | ((c >> 16) & 0x0000FF00) | ((c << 16) & 0xFF000000);
				
				#else
				
					color = (c & 0xFF00FF00) | ((c >> 16) & 0x000000FF) | ((c << 16) & 0x00FF0000);
				
				#endif
				
				return (*this);
			}
			
			unsigned_int32 GetPackedColor(void) const
			{
				return (color);
			}
			
			unsigned_int32 GetPackedRGBColor(void) const
			{
				#if C4BIGENDIAN
				
					return (color & 0xFFFFFF00);
				
				#else
				
					return (color & 0x00FFFFFF);
				
				#endif
			}
			
			Color4C& SetPackedColor(unsigned_int32 c)
			{
				color = c;
				return (*this);
			}
			
			unsigned_int32 GetRed(void) const
			{
				#if C4BIGENDIAN
				
					return (color >> 24);
				
				#else
				
					return (color & 0xFF);
				
				#endif
			}
			
			Color4C& SetRed(unsigned_int32 red)
			{
				#if C4BIGENDIAN
				
					color = (color & 0x00FFFFFF) | (red << 24);
				
				#else
				
					color = (color & 0xFFFFFF00) | red;
				
				#endif
				
				return (*this);
			}
			
			unsigned_int32 GetGreen(void) const
			{
				#if C4BIGENDIAN
				
					return ((color >> 16) & 0xFF);
				
				#else
				
					return ((color >> 8) & 0xFF);
				
				#endif
			}
			
			Color4C& SetGreen(unsigned_int32 green)
			{
				#if C4BIGENDIAN
				
					color = (color & 0xFF00FFFF) | (green << 16);
				
				#else
				
					color = (color & 0xFFFF00FF) | (green << 8);
				
				#endif
				
				return (*this);
			}
			
			unsigned_int32 GetBlue(void) const
			{
				#if C4BIGENDIAN
				
					return ((color >> 8) & 0xFF);
				
				#else
				
					return ((color >> 16) & 0xFF);
				
				#endif
			}
			
			Color4C& SetBlue(unsigned_int32 blue)
			{
				#if C4BIGENDIAN
				
					color = (color & 0xFFFF00FF) | (blue << 8);
				
				#else
				
					color = (color & 0xFF00FFFF) | (blue << 16);
				
				#endif
				
				return (*this);
			}
			
			unsigned_int32 GetAlpha(void) const
			{
				#if C4BIGENDIAN
				
					return (color & 0xFF);
				
				#else
				
					return (color >> 24);
				
				#endif
			}
			
			Color4C& SetAlpha(unsigned_int32 alpha)
			{
				#if C4BIGENDIAN
				
					color = (color & 0xFFFFFF00) | alpha;
				
				#else
				
					color = (color & 0x00FFFFFF) | (alpha << 24);
				
				#endif
				
				return (*this);
			}
			
			Color4C& SetMaxAlpha(void)
			{
				#if C4BIGENDIAN
				
					color |= 0x000000FF;
				
				#else
				
					color |= 0xFF000000;
				
				#endif
				
				return (*this);
			}
			
			Color3C GetColor3C(void) const
			{
				return (Color3C().SetPackedColor(color));
			}
	};
	
	
	inline void Reverse(Color2C *)
	{
	}
	
	inline void Reverse(Color3C *)
	{
	}
	
	inline void Reverse(Color4C *)
	{
	}
	
	
	struct BC1Block
	{
		union
		{
			unsigned_int8	byteData[8];
			unsigned_int32	wordData[2];
		};
		
		BC1Block& operator =(const BC1Block& block)
		{
			wordData[0] = block.wordData[0];
			wordData[1] = block.wordData[1];
			return (*this);
		}
		
		bool operator ==(const BC1Block& block) const
		{
			return ((block.wordData[0] == wordData[0]) && (block.wordData[1] == wordData[1]));
		}
		
		bool operator !=(const BC1Block& block) const
		{
			return ((block.wordData[0] != wordData[0]) || (block.wordData[1] != wordData[1]));
		}
	};
	
	
	struct BC3Block
	{
		union
		{
			unsigned_int8	byteData[16];
			unsigned_int32	wordData[4];
		};
		
		BC3Block& operator =(const BC3Block& block)
		{
			wordData[0] = block.wordData[0];
			wordData[1] = block.wordData[1];
			wordData[2] = block.wordData[2];
			wordData[3] = block.wordData[3];
			return (*this);
		}
		
		bool operator ==(const BC3Block& block) const
		{
			return ((block.wordData[0] == wordData[0]) && (block.wordData[1] == wordData[1]) && (block.wordData[2] == wordData[2]) && (block.wordData[3] == wordData[3]));
		}
		
		bool operator !=(const BC3Block& block) const
		{
			return ((block.wordData[0] != wordData[0]) || (block.wordData[1] != wordData[1]) || (block.wordData[2] != wordData[2]) || (block.wordData[3] != wordData[3]));
		}
	};
	
	
	struct SequenceHeader
	{
		int32		sequenceWidth;
		int32		sequenceHeight;
		
		int32		frameCount;
		Fixed		frameRate;
	};
	
	void Reverse(SequenceHeader *sh);
	
	
	class SequenceResource : public Resource<SequenceResource>
	{
		friend class Resource<SequenceResource>;
		
		private:
			
			static C4API ResourceDescriptor		descriptor;
			
			~SequenceResource();
		
		public:
			
			C4API SequenceResource(const char *name, ResourceCatalog *catalog);
			
			C4API ResourceResult LoadSequenceHeader(ResourceLoader *loader, SequenceHeader *sequenceHeader) const;
			C4API ResourceResult LoadFrameOffsetTable(ResourceLoader *loader, const SequenceHeader *sequenceHeader, unsigned_int32 *frameOffsetTable) const;
			C4API ResourceResult LoadFrameData(ResourceLoader *loader, int32 frame, const unsigned_int32 *frameOffsetTable, unsigned_int8 **frameData) const;
	};
	
	
	struct Image
	{
		private:
		
			static Vector3D CalculateColorAxis(int32 count, const Point3D *color, const Box3D& bounds);
			static void CalculateEndpointCandidates(int32 count, const Point3D *color, int32 *restrict candCount1, int32 *restrict candCount2, unsigned_int16 *restrict cand1, unsigned_int16 *restrict cand2);
			static void CalculateEndpointCandidates(int32 count, const float *green, int32 *restrict candCount1, int32 *restrict candCount2, unsigned_int16 *restrict cand1, unsigned_int16 *restrict cand2);
			static float EncodeColorBlock(int32 width, int32 height, unsigned_int16 color0, unsigned_int16 color1, bool black, const Point3D *image, unsigned_int8 *restrict data);
			static float EncodeGreenBlock(int32 width, int32 height, unsigned_int16 color0, unsigned_int16 color1, const float *image, unsigned_int8 *restrict data);
			static float EncodeGrayBlock(int32 width, int32 height, unsigned_int8 gray0, unsigned_int8 gray1, bool black, const float *image, unsigned_int8 *restrict data);
		
		public:
			
			C4API static bool TrimImageZero_RGBA32(const Color4C *image, int32 width, int32 height, Rect *trim);
			
			C4API static unsigned_int32 CompressImageRLE_RGBA32(const void *image, unsigned_int8 *restrict data, int32 pixelCount);
			
			C4API static void DecompressImageRLE_L8(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			C4API static void DecompressImageRLE_RGB16(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			C4API static void DecompressImageRLE_RGB24(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			C4API static void DecompressImageRLE_RGBZ24(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			C4API static void DecompressImageRLE_RGB32(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			C4API static void DecompressImageRLE_RGBZ32(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			C4API static void DecompressImageRLE_RGBA32(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount);
			C4API static void DecompressImageRLE_L8(const unsigned_int8 *data, Color2C *restrict image, int32 pixelCount);
			
			C4API static void DecompressImageRLE_RGBA32(const unsigned_int8 *code, unsigned_int32 codeSize, void *restrict output);
			C4API static void DecompressImageRLE_LA16(const unsigned_int8 *code, unsigned_int32 codeSize, void *restrict output);
			C4API static void DecompressImageRLE_L8(const unsigned_int8 *code, unsigned_int32 codeSize, void *restrict output);
			C4API static void DecompressImageRLE_DEPTH16(const unsigned_int8 *code, unsigned_int32 codeSize, void *restrict output);
			C4API static void DecompressImageRLE_BC1(const unsigned_int8 *code, unsigned_int32 codeSize, void *restrict output);
			C4API static void DecompressImageRLE_BC3(const unsigned_int8 *code, unsigned_int32 codeSize, void *restrict output);
			
			C4API static void CompressColorBlock(int32 width, int32 height, int32 rowLength, bool alpha, const Color4C *image, unsigned_int8 *restrict data);
			C4API static void CompressGreenBlock(int32 width, int32 height, int32 rowLength, const Color4C *image, unsigned_int8 *restrict data);
			C4API static void CompressGrayBlock(int32 width, int32 height, int32 rowLength, const unsigned_int8 *image, unsigned_int8 *restrict data);
			
			C4API static void BleedAlphaTestMap(int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *restrict destin, unsigned_int32 testValue);
			C4API static void BleedNormalMap(int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *restrict destin);
			
			C4API static void CalculateNormalMap(int32 width, int32 height, const Color4C *source, Color4C *restrict destin, float scale, bool swrap = true, bool twrap = true);
			C4API static void CalculateXYNormalMap(int32 width, int32 height, const Color4C *source, Color4C *restrict destin, float scale, bool swrap = true, bool twrap = true);
			C4API static void CalculateParallaxMap(int32 width, int32 height, const Color4C *source, Color4C *restrict destin, float scale, bool swrap = true, bool twrap = true);
			C4API static void CalculateHorizonMap(int32 index, int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *restrict destin, float scale, bool swrap = true, bool twrap = true);
			C4API static void CalculateAmbientOcclusionChannel(int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *restrict destin, float scale, bool swrap = true, bool twrap = true);
			
			C4API static int32 CalculateMipmapChainSize2D(int32 width, int32 height, int32 *pixelCount);
			C4API static int32 CalculateBlockMipmapChainSize2D(int32 width, int32 height, int32 levelCount);
			C4API static int32 CalculateMipmapChainSize3D(int32 width, int32 height, int32 depth, int32 *pixelCount);
			
			C4API static void GenerateMipmaps2D(int32 count, int32 width, int32 height, Color4C *image, unsigned_int32 flags = 0);
			C4API static void GenerateMipmaps2D(int32 count, int32 width, int32 height, Color2C *image);
			C4API static void GenerateMipmaps2D(int32 count, int32 width, int32 height, Color1C *image);
			
			C4API static void GenerateMipmaps3D(int32 width, int32 height, int32 depth, Color4C *image);
			C4API static void GenerateMipmaps3D(int32 width, int32 height, int32 depth, Color2C *image);
			C4API static void GenerateMipmaps3D(int32 width, int32 height, int32 depth, Color1C *image);
	};
}


#endif

// ZYURVUR
