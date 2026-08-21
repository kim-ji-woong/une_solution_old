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


#include "C4Fonts.h"


using namespace C4;


ResourceDescriptor FontResource::descriptor("fnt", 0, 0, "font/Gui");


#if C4LEGACY

	namespace
	{
		struct OldGlyphData
		{
			unsigned_int32	glyphFlags;
			float			glyphWidth;
			Point2D			glyphTexcoord[2];
		};
		
		void Reverse(OldGlyphData *gd)
		{
			C4::Reverse(&gd->glyphFlags);
			C4::Reverse(&gd->glyphWidth);
			C4::Reverse(&gd->glyphTexcoord[0]);
			C4::Reverse(&gd->glyphTexcoord[1]);
		}
		
		struct OldFontHeader
		{
			float			fontHeight;
			float			fontBaseline;
			float			fontSpacing;
			float			fontLeading;
			
			int32			glyphCount;
			int32			glyphStart;
			OldGlyphData	glyphData[1];
		};
		
		void Reverse(OldFontHeader *fh)
		{
			C4::Reverse(&fh->fontHeight);
			C4::Reverse(&fh->fontBaseline);
			C4::Reverse(&fh->fontSpacing);
			C4::Reverse(&fh->fontLeading);
			C4::Reverse(&fh->glyphCount);
			C4::Reverse(&fh->glyphStart);
			
			int32 count = fh->glyphCount;
			for (machine a = 0; a < count; a++) Reverse(&fh->glyphData[a]);
		}
	}

#endif

	
void C4::Reverse(GlyphData *gd)
{
	Reverse(&gd->glyphFlags);
	Reverse(&gd->glyphWidth);
	Reverse(&gd->glyphHeight);
	Reverse(&gd->glyphTop);
	Reverse(&gd->glyphTexcoord[0]);
	Reverse(&gd->glyphTexcoord[1]);
}

void C4::Reverse(FontHeader *fh)
{
	Reverse(&fh->fontHeight);
	Reverse(&fh->fontBaseline);
	Reverse(&fh->fontSpacing);
	Reverse(&fh->fontLeading);
	
	Reverse(&fh->shadowOffset[0]);
	Reverse(&fh->shadowOffset[1]);
	
	Reverse(&fh->pageCount);
	Reverse(&fh->pageIndexOffset);
	Reverse(&fh->pageTableOffset);
	
	Reverse(&fh->glyphCount);
	Reverse(&fh->glyphDataOffset);
	
	Reverse(&fh->auxiliaryDataSize);
	Reverse(&fh->auxiliaryDataOffset);
	
	int32 count = fh->pageCount * 256;
	unsigned_int16 *pageTable = fh->GetPageTable();
	for (machine a = 0; a < count; a++) Reverse(&pageTable[a]);
	
	count = fh->glyphCount;
	GlyphData *glyphData = fh->GetGlyphData();
	for (machine a = 0; a < count; a++) Reverse(&glyphData[a]);
}


FontResource::FontResource(const char *name, ResourceCatalog *catalog) : Resource<FontResource>(name, catalog)
{
	owningFont = nullptr;
}
 
FontResource::~FontResource()
{ 
} 
 
void FontResource::Preprocess(void)
{ 
	int32 *data = static_cast<int32 *>(GetData());
	if (data[0] != 1)
	{
		Reverse(&data[1]); 
		
		#if C4LEGACY
		
			int32 version = data[1]; 
			if (version > 0) Reverse(reinterpret_cast<FontHeader *>(data + 2));
			else Reverse(reinterpret_cast<OldFontHeader *>(data + 2));
		
		#else
		
			Reverse(reinterpret_cast<FontHeader *>(data + 2));
		
		#endif
	}
}


C4::Font::Font(FontResource *resource, const char *name) : textureMapAttribute(name)
{
	fontResource = resource;
	resource->SetOwningFont(this);
	
	#if C4LEGACY
	
		if (resource->GetVersion() == 0)
		{
			char *storage = new char[sizeof(FontHeader) + 768 + sizeof(GlyphData) * 256];
			bcHeader = reinterpret_cast<FontHeader *>(storage);
			fontHeader = bcHeader;
			
			const OldFontHeader *oldHeader = reinterpret_cast<const OldFontHeader *>(resource->GetFontHeader());
			bcHeader->fontHeight = oldHeader->fontHeight;
			bcHeader->fontBaseline = oldHeader->fontBaseline;
			bcHeader->fontSpacing = oldHeader->fontSpacing;
			bcHeader->fontLeading = oldHeader->fontLeading;
			
			bcHeader->shadowOffset[0].Set(0.0F, 0.0F);
			bcHeader->shadowOffset[1].Set(0.0F, 0.0F);
			
			bcHeader->pageCount = 1;
			bcHeader->pageIndexOffset = sizeof(FontHeader);
			bcHeader->pageTableOffset = sizeof(FontHeader) + 256;
			
			bcHeader->glyphCount = 256;
			bcHeader->glyphDataOffset = sizeof(FontHeader) + 768;
			
			bcHeader->auxiliaryDataSize = 0;
			bcHeader->auxiliaryDataOffset = 0;
			
			unsigned_int8 *pageIndex = bcHeader->GetPageIndex();
			for (machine a = 0; a < 256; a++) pageIndex[a] = 0;
			
			unsigned_int16 *pageTable = bcHeader->GetPageTable();
			for (machine a = 0; a < 256; a++) pageTable[a] = (unsigned_int16) a;
			
			GlyphData *glyphData = bcHeader->GetGlyphData();
			const OldGlyphData *oldGlyphData = oldHeader->glyphData;
			
			float height = oldHeader->fontHeight;
			for (machine a = 0; a < 256; a++)
			{
				glyphData->glyphFlags = 0;
				glyphData->glyphWidth = oldGlyphData->glyphWidth;
				glyphData->glyphHeight = height;
				glyphData->glyphTop = 0.0F;
				glyphData->glyphTexcoord[0] = oldGlyphData->glyphTexcoord[0];
				glyphData->glyphTexcoord[1] = oldGlyphData->glyphTexcoord[1];
				
				glyphData++;
				oldGlyphData++;
			}
		}
		else
		{
			fontHeader = resource->GetFontHeader();
			bcHeader = nullptr;
		}
	
	#else
	
		fontHeader = resource->GetFontHeader();
	
	#endif
}

C4::Font::~Font()
{
	#if C4LEGACY
	
		delete[] reinterpret_cast<char *>(bcHeader);
	
	#endif
	
	fontResource->Release();
}

C4::Font *C4::Font::Get(const char *name)
{
	FontResource *resource = FontResource::Get(name);
	
	Font *font = resource->GetOwningFont();
	if (font)
	{
		font->Retain();
		resource->Release();
		return (font);
	}
	
	return (new Font(resource, name));
}

float C4::Font::GetTextWidth(const char *text) const
{
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	float w = -spacing;
	for (;; text++)
	{
		unsigned_int32 code = *reinterpret_cast<const unsigned_int8 *>(text);
		if (code == 0) break;
		
		w += glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
	}
	
	return (FmaxZero(w));
}

float C4::Font::GetTextWidth(const char *text, int32 length) const
{
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	float w = -spacing;
	while (length > 0)
	{
		unsigned_int32 code = *reinterpret_cast<const unsigned_int8 *>(text);
		if (code == 0) break;
		
		text++;
		length--;
		w += glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
	}
	
	return (FmaxZero(w));
}

int32 C4::Font::GetTextLengthFitWidth(const char *text, float width, float *actual) const
{
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	float w = -spacing;
	int32 len = 0;
	
	for (;; text++)
	{
		unsigned_int32 code = *reinterpret_cast<const unsigned_int8 *>(text);
		if (code == 0) break;
		
		float x = w + glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
		if (x > width) break;
		
		w = x;
		len++;
	}
	
	if (actual) *actual = FmaxZero(w);
	return (len);
}

int32 C4::Font::GetTextLengthFitWidth(const char *text, int32 length, float width, float *actual) const
{
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	float w = -spacing;
	int32 len = 0;
	
	for (machine a = 0; a < length; a++, text++)
	{
		unsigned_int32 code = *reinterpret_cast<const unsigned_int8 *>(text);
		if (code == 0) break;
		
		float x = w + glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
		if (x > width) break;
		
		w = x;
		len++;
	}
	
	if (actual) *actual = FmaxZero(w);
	return (len);
}

int32 C4::Font::GetBrokenTextLengthFitWidth(const char *text, float width, bool partialWord, float *actual, bool *hardBreak) const
{
	float	textWidth;
	
	bool hard = false;
	int32 length = 0;
	
	for (;;)
	{
		unsigned_int32 code = text[length];
		if (code == 0) break;
		
		length++;
		if ((code == 0x000A) || (code == 0x000D))
		{
			hard = true;
			break;
		}
	}
	
	if (hardBreak) *hardBreak = hard;
	
	if (length == 0)
	{
		if (actual) *actual = 0.0F;
		return (0);
	}
	
	int32 len = GetTextLengthFitWidth(text, length, width, &textWidth);
	if (len == 0)
	{
		if (actual) *actual = 0.0F;
		return (0);
	}
	
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	if (len < length)
	{
		int32 l = len;
		float w = textWidth;
		
		for (; len > 0; len--)
		{
			unsigned_int32 code = reinterpret_cast<const unsigned_int8 *>(text)[len - 1];
			if ((code == 0x0020) || (code == '-') || (code == '/')) break;
			textWidth -= glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
		}
		
		if ((len == 0) && (partialWord))
		{
			len = l;
			textWidth = w;
		}
	}
	
	if (actual)
	{
		float spaceWidth = glyphData[GetGlyphIndex(0x0020)].glyphWidth + spacing;
		
		int32 pos = len - 1;
		while ((pos >= 0) && (text[pos] == 0x0020))
		{
			textWidth -= spaceWidth;
			pos--;
		}
		
		*actual = textWidth;
	}
	
	while (text[len] == 0x0020) len++;
	
	return (len);
}

int32 C4::Font::GetBrokenTextLengthFitWidth(const char *text, int32 length, float width, bool partialWord, float *actual, bool *hardBreak) const
{
	float	textWidth;
	
	int32 hard = false;
	int32 end = length;
	
	for (length = 0; length < end;)
	{
		unsigned_int32 code = text[length];
		if (code == 0) break;
		
		length++;
		if ((code == 0x000A) || (code == 0x000D))
		{
			hard = true;
			break;
		}
	}
	
	if (hardBreak) *hardBreak = hard;
	
	if (length == 0)
	{
		if (actual) *actual = 0.0F;
		return (0);
	}
	
	int32 len = GetTextLengthFitWidth(text, length, width, &textWidth);
	if (len == 0)
	{
		if (actual) *actual = 0.0F;
		return (0);
	}
	
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	if (len < length)
	{
		int32 l = len;
		float w = textWidth;
		
		for (; len > 0; len--)
		{
			unsigned_int32 code = reinterpret_cast<const unsigned_int8 *>(text)[len - 1];
			if ((code == 0x0020) || (code == '-') || (code == '/')) break;
			textWidth -= glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
		}
		
		if ((len == 0) && (partialWord))
		{
			len = l;
			textWidth = w;
		}
	}
	
	if (actual)
	{
		float spaceWidth = glyphData[GetGlyphIndex(0x0020)].glyphWidth + spacing;
		
		int32 pos = len - 1;
		while ((pos >= 0) && (text[pos] == 0x0020))
		{
			textWidth -= spaceWidth;
			pos--;
		}
		
		*actual = textWidth;
	}
	
	while (text[len] == 0x0020) len++;
	
	return (len);
}

int32 C4::Font::CountBrokenTextLines(const char *text, float width) const
{
	int32 length = Text::GetTextLength(text);
	int32 position = 0;
	int32 line = 0;
	
	while (position < length)
	{
		position += GetBrokenTextLengthFitWidth(&text[position], width);
		line++;
	}
	
	return (line);
}

float C4::Font::GetTextWidthUTF8(const char *text) const
{
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	float w = -spacing;
	for (;;)
	{
		unsigned_int32	code;
		
		int32 read = Text::ReadGlyphCodeUTF8(text, &code);
		if (code == 0) break;
		
		text += read;
		w += glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
	}
	
	return (FmaxZero(w));
}

float C4::Font::GetTextWidthUTF8(const char *text, int32 length) const
{
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	float w = -spacing;
	while (length > 0)
	{
		unsigned_int32	code;
		
		int32 read = Text::ReadGlyphCodeUTF8(text, &code);
		if (code == 0) break;
		
		text += read;
		length -= read;
		w += glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
	}
	
	return (FmaxZero(w));
}

int32 C4::Font::GetTextLengthFitWidthUTF8(const char *text, float width, float *actual) const
{
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	float w = -spacing;
	int32 len = 0;
	
	for (;;)
	{
		unsigned_int32	code;
		
		int32 read = Text::ReadGlyphCodeUTF8(text, &code);
		if (code == 0) break;
		
		float x = w + glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
		if (x > width) break;
		
		w = x;
		len += read;
		text += read;
	}
	
	if (actual) *actual = FmaxZero(w);
	return (len);
}

int32 C4::Font::GetTextLengthFitWidthUTF8(const char *text, int32 length, float width, float *actual) const
{
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	float w = -spacing;
	int32 len = 0;
	
	while (len < length)
	{
		unsigned_int32	code;
		
		int32 read = Text::ReadGlyphCodeUTF8(text, &code);
		if (code == 0) break;
		
		float x = w + glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
		if (x > width) break;
		
		w = x;
		len += read;
		text += read;
	}
	
	if (actual) *actual = FmaxZero(w);
	return (len);
}

int32 C4::Font::GetBrokenTextLengthFitWidthUTF8(const char *text, float width, bool partialWord, float *actual, bool *hardBreak) const
{
	float	textWidth;
	
	bool hard = false;
	int32 length = 0;
	
	for (;;)
	{
		unsigned_int32	code;
		
		int32 read = Text::ReadGlyphCodeUTF8(&text[length], &code);
		if (code == 0) break;
		
		length += read;
		if ((code == 0x000A) || (code == 0x000D))
		{
			hard = true;
			break;
		}
	}
	
	if (hardBreak) *hardBreak = hard;
	
	if (length == 0)
	{
		if (actual) *actual = 0.0F;
		return (0);
	}
	
	int32 len = GetTextLengthFitWidthUTF8(text, length, width, &textWidth);
	if (len == 0)
	{
		if (actual) *actual = 0.0F;
		return (0);
	}
	
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	if (len < length)
	{
		int32 l = len;
		float w = textWidth;
		
		for (; len > 0; len--)
		{
			const char *byte = &text[len - 1];
			unsigned_int32 code = *reinterpret_cast<const unsigned_int8 *>(byte);
			if (code < 0x0080)
			{
				if ((code == 0x0020) || (code == '-') || (code == '/')) break;
			}
			else if (code >= 0x00C0)
			{
				Text::ReadGlyphCodeUTF8(byte, &code);
			}
			
			textWidth -= glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
		}
		
		if ((len == 0) && (partialWord))
		{
			len = l;
			textWidth = w;
		}
	}
	
	if (actual)
	{
		float spaceWidth = glyphData[GetGlyphIndex(0x0020)].glyphWidth + spacing;
		
		int32 pos = len - 1;
		while ((pos >= 0) && (text[pos] == 0x0020))
		{
			textWidth -= spaceWidth;
			pos--;
		}
		
		*actual = textWidth;
	}
	
	while (text[len] == 0x0020) len++;
	
	return (len);
}

int32 C4::Font::GetBrokenTextLengthFitWidthUTF8(const char *text, int32 length, float width, bool partialWord, float *actual, bool *hardBreak) const
{
	float	textWidth;
	
	int32 hard = false;
	int32 end = length;
	
	for (length = 0; length < end;)
	{
		unsigned_int32	code;
		
		int32 read = Text::ReadGlyphCodeUTF8(&text[length], &code);
		if (code == 0) break;
		
		length += read;
		if ((code == 0x000A) || (code == 0x000D))
		{
			hard = true;
			break;
		}
	}
	
	if (hardBreak) *hardBreak = hard;
	
	if (length == 0)
	{
		if (actual) *actual = 0.0F;
		return (0);
	}
	
	int32 len = GetTextLengthFitWidthUTF8(text, length, width, &textWidth);
	if (len == 0)
	{
		if (actual) *actual = 0.0F;
		return (0);
	}
	
	const GlyphData *glyphData = fontHeader->GetGlyphData();
	float spacing = fontHeader->fontSpacing;
	
	if (len < length)
	{
		int32 l = len;
		float w = textWidth;
		
		for (; len > 0; len--)
		{
			const char *byte = &text[len - 1];
			unsigned_int32 code = *reinterpret_cast<const unsigned_int8 *>(byte);
			if (code < 0x0080)
			{
				if ((code == 0x0020) || (code == '-') || (code == '/')) break;
			}
			else if (code >= 0x00C0)
			{
				Text::ReadGlyphCodeUTF8(byte, &code);
			}
			
			textWidth -= glyphData[GetGlyphIndex(code)].glyphWidth + spacing;
		}
		
		if ((len == 0) && (partialWord))
		{
			len = l;
			textWidth = w;
		}
	}
	
	if (actual)
	{
		float spaceWidth = glyphData[GetGlyphIndex(0x0020)].glyphWidth + spacing;
		
		int32 pos = len - 1;
		while ((pos >= 0) && (text[pos] == 0x0020))
		{
			textWidth -= spaceWidth;
			pos--;
		}
		
		*actual = textWidth;
	}
	
	while (text[len] == 0x0020) len++;
	
	return (len);
}

int32 C4::Font::CountBrokenTextLinesUTF8(const char *text, float width) const
{
	int32 length = Text::GetTextLength(text);
	int32 position = 0;
	int32 line = 0;
	
	while (position < length)
	{
		position += GetBrokenTextLengthFitWidth(&text[position], width);
		line++;
	}
	
	return (line);
}

// ZYURVUR
