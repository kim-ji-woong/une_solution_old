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


#include "C4TextureViewer.h"
#include "C4Graphics.h"


using namespace C4;


namespace
{
	const float kDefaultHeightScale = 8.0F;
	
	
	enum
	{
		kTextureTypeIDCount = 3,
		kTextureFormatIDCount = 4,
		kTextureAlphaIDCount = 8,
		kTextureWrapIDCount = 6
	};
	
	
	const unsigned_int32 kTextureTypeID[kTextureTypeIDCount] =
	{
		kTexture2D, kTextureRectangle, kTextureCube
	};
	
	const unsigned_int32 kTextureFormatID[kTextureFormatIDCount] =
	{
		kTextureRGBA8, kTextureLA8, kTextureL8, kTextureI8
	};
	
	const unsigned_int32 kTextureAlphaID[kTextureAlphaIDCount] =
	{
		'NONE', kTextureSemanticTransparency, kTextureSemanticGloss, kTextureSemanticEmission, kTextureSemanticOpacity, kTextureSemanticGlow, kTextureSemanticOcclusion, kTextureSemanticParallax
	};
	
	const unsigned_int32 kTextureWrapID[kTextureWrapIDCount] =
	{
		kTextureRepeat, kTextureClamp, kTextureClampBorder, kTextureMirrorRepeat, kTextureMirrorClamp, kTextureMirrorClampBorder
	};
}


List<ImportTextureWindow> ImportTextureWindow::windowList;


CompressionBC1Job::CompressionBC1Job(int32 width, const Rect& rect, const Color4C *source, unsigned_int8 *output) : BatchJob(&Compress)
{
	imageWidth = width;
	blockRect = rect;
	sourceImage = source;
	outputCode = output;
}

CompressionBC1Job::~CompressionBC1Job()
{
}

void CompressionBC1Job::Compress(Job *job, void *cookie)
{
	const CompressionBC1Job *compJob = static_cast<CompressionBC1Job *>(job);
	
	int32 width = compJob->imageWidth;
	const Color4C *image = compJob->sourceImage + compJob->blockRect.top * width * 4;
	unsigned_int8 *code = compJob->outputCode + compJob->blockRect.top * width * 2;
	
	for (machine j = compJob->blockRect.top; j < compJob->blockRect.bottom; j++)
	{
		for (machine i = compJob->blockRect.left; i < compJob->blockRect.right; i++)
		{
			Image::CompressColorBlock(4, 4, width, false, image + i * 4, code + i * 8);
		}
		
		image += width * 4;
		code += width * 2;
	}
}


CompressionBC3Job::CompressionBC3Job(int32 width, const Rect& rect, const Color4C *source, unsigned_int8 *output) : BatchJob(&Compress)
{
	imageWidth = width;
	blockRect = rect;
	sourceImage = source;
	outputCode = output;
}

CompressionBC3Job::~CompressionBC3Job()
{
}

void CompressionBC3Job::Compress(Job *job, void *cookie)
{
	const CompressionBC3Job *compJob = static_cast<CompressionBC3Job *>(job);
	
	int32 width = compJob->imageWidth;
	const Color4C *image = compJob->sourceImage + compJob->blockRect.top * width * 4;
	unsigned_int8 *code = compJob->outputCode + compJob->blockRect.top * width * 4;
	
	for (machine j = compJob->blockRect.top; j < compJob->blockRect.bottom; j++)
	{
		for (machine i = compJob->blockRect.left; i < compJob->blockRect.right; i++) 
		{
			unsigned_int8	gray[16]; 
			 
			const Color4C *input = image + i * 4; 
			unsigned_int8 *output = code + i * 16;
			 
			for (machine n = 0; n < 4; n++)
			{
				for (machine m = 0; m < 4; m++) gray[n * 4 + m] = (unsigned_int8) input[n * width + m].GetAlpha();
			} 
			
			Image::CompressGrayBlock(4, 4, 4, gray, output);
			Image::CompressColorBlock(4, 4, width, true, input, output + 8);
		} 
		
		image += width * 4;
		code += width * 4;
	}
}


CompressionNormalBC3Job::CompressionNormalBC3Job(int32 width, const Rect& rect, const Color4C *source, unsigned_int8 *output) : BatchJob(&Compress)
{
	imageWidth = width;
	blockRect = rect;
	sourceImage = source;
	outputCode = output;
}

CompressionNormalBC3Job::~CompressionNormalBC3Job()
{
}

void CompressionNormalBC3Job::Compress(Job *job, void *cookie)
{
	const CompressionNormalBC3Job *compJob = static_cast<CompressionNormalBC3Job *>(job);
	
	int32 width = compJob->imageWidth;
	const Color4C *image = compJob->sourceImage + compJob->blockRect.top * width * 4;
	unsigned_int8 *code = compJob->outputCode + compJob->blockRect.top * width * 4;
	
	for (machine j = compJob->blockRect.top; j < compJob->blockRect.bottom; j++)
	{
		for (machine i = compJob->blockRect.left; i < compJob->blockRect.right; i++)
		{
			unsigned_int8	gray[16];
			
			const Color4C *input = image + i * 4;
			unsigned_int8 *output = code + i * 16;
			
			for (machine n = 0; n < 4; n++)
			{
				for (machine m = 0; m < 4; m++) gray[n * 4 + m] = (unsigned_int8) input[n * width + m].GetAlpha();
			}
			
			Image::CompressGrayBlock(4, 4, 4, gray, output);
			Image::CompressGreenBlock(4, 4, width, input, output + 8);
		}
		
		image += width * 4;
		code += width * 4;
	}
}


AmbientJob::AmbientJob(int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *destin, float scale, bool swrap, bool twrap) : BatchJob(&Compute)
{
	textureWidth = width;
	textureHeight = height;
	computeRect = rect;
	sourceImage = source;
	destinImage = destin;
	heightScale = scale;
	swrapFlag = swrap;
	twrapFlag = twrap;
}

AmbientJob::~AmbientJob()
{
}

void AmbientJob::Compute(Job *job, void *cookie)
{
	const AmbientJob *ambientJob = static_cast<AmbientJob *>(job);
	Image::CalculateAmbientOcclusionChannel(ambientJob->textureWidth, ambientJob->textureHeight, ambientJob->computeRect, ambientJob->sourceImage, ambientJob->destinImage, ambientJob->heightScale, ambientJob->swrapFlag, ambientJob->twrapFlag);
}


HorizonJob::HorizonJob(int32 index, int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *destin, float scale, bool swrap, bool twrap) : BatchJob(&Compute)
{
	horizonIndex = index;
	textureWidth = width;
	textureHeight = height;
	computeRect = rect;
	sourceImage = source;
	destinImage = destin;
	heightScale = scale;
	swrapFlag = swrap;
	twrapFlag = twrap;
}

HorizonJob::~HorizonJob()
{
}

void HorizonJob::Compute(Job *job, void *cookie)
{
	const HorizonJob *horizonJob = static_cast<HorizonJob *>(job);
	Image::CalculateHorizonMap(horizonJob->horizonIndex, horizonJob->textureWidth, horizonJob->textureHeight, horizonJob->computeRect, horizonJob->sourceImage, horizonJob->destinImage, horizonJob->heightScale, horizonJob->swrapFlag, horizonJob->twrapFlag);
}


BleedAlphaTestJob::BleedAlphaTestJob(int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *destin, unsigned_int32 test) : BatchJob(&Compute)
{
	textureWidth = width;
	textureHeight = height;
	computeRect = rect;
	sourceImage = source;
	destinImage = destin;
	testValue = test;
}

BleedAlphaTestJob::~BleedAlphaTestJob()
{
}

void BleedAlphaTestJob::Compute(Job *job, void *cookie)
{
	const BleedAlphaTestJob *bleedAlphaTestJob = static_cast<BleedAlphaTestJob *>(job);
	Image::BleedAlphaTestMap(bleedAlphaTestJob->textureWidth, bleedAlphaTestJob->textureHeight, bleedAlphaTestJob->computeRect, bleedAlphaTestJob->sourceImage, bleedAlphaTestJob->destinImage, bleedAlphaTestJob->testValue);
}


BleedNormalJob::BleedNormalJob(int32 width, int32 height, const Rect& rect, const Color4C *source, Color4C *destin) : BatchJob(&Compute)
{
	textureWidth = width;
	textureHeight = height;
	computeRect = rect;
	sourceImage = source;
	destinImage = destin;
}

BleedNormalJob::~BleedNormalJob()
{
}

void BleedNormalJob::Compute(Job *job, void *cookie)
{
	const BleedNormalJob *bleedNormalJob = static_cast<BleedNormalJob *>(job);
	Image::BleedNormalMap(bleedNormalJob->textureWidth, bleedNormalJob->textureHeight, bleedNormalJob->computeRect, bleedNormalJob->sourceImage, bleedNormalJob->destinImage);
}


TextureImporter::TextureImporter(const char *name, unsigned_int32 importFlags)
{
	ResourcePath	configPath;
	
	inputTextureName[0] = 0;
	outputTextureName = name;
	
	for (machine a = 0; a < kMaxTextureImportCount; a++)
	{
		textureHeader[a].textureType = kTexture2D;
		textureHeader[a].textureFlags = 0;
		textureHeader[a].colorSemantic = kTextureSemanticNone;
		textureHeader[a].alphaSemantic = kTextureSemanticNone;
		textureHeader[a].imageFormat = kTextureRGBA8;
		textureHeader[a].imageWidth = 0;
		textureHeader[a].imageHeight = 0;
		textureHeader[a].imageDepth = 1;
		textureHeader[a].wrapMode[0] = kTextureRepeat;
		textureHeader[a].wrapMode[1] = kTextureRepeat;
		textureHeader[a].wrapMode[2] = kTextureRepeat;
	}
	
	textureImportFlags = importFlags | kTextureImportMipmaps;
	heightScale = kDefaultHeightScale;
	heightChannel = 0;
	horizonIndex = 0;
	
	parallaxScale = 0.0F;
	imageCenter.Set(0.0F, 0.0F);
	
	impostorSize.Set(0.0F, 0.0F);
	impostorClipData[0] = 0.0F;
	impostorClipData[1] = 0.0F;
	impostorClipData[2] = 0.0F;
	impostorClipData[3] = 0.0F;
	
	hazeColor.Set(1.0F, 1.0F, 1.0F, 1.0F);
	hazeElevation = 5;
	
	for (machine a = 0; a < kMaxTextureImportCount; a++) textureImage[a] = nullptr;
	
	GetInputConfigPath(&configPath);
	ConfigResource *config = ConfigResource::Get(name, 0, ThePluginMgr->GetImportCatalog());
	if (config)
	{
		const char *text = config->GetText();
		text += Text::GetWhitespaceLength(text);
		if (*text != 0)
		{
			String<kMaxCommandLength>	command;
			
			text += Text::ReadString(text, command, kMaxCommandLength);
			if (command == "itexture")
			{
				text += Text::GetWhitespaceLength(text);
				if (*text != 0)
				{
					text += Text::ReadString(text, command, kMaxCommandLength);
					text += Text::GetWhitespaceLength(text);
					
					int32 length = Text::FindUnquotedChar(text, ';');
					if (length < 0)
					{
						ProcessCommandLine(text);
					}
					else
					{
						Text::CopyText(text, command, Min(length, kMaxCommandLength));
						ProcessCommandLine(command);
					}
				}
			}
		}
		
		config->Release();
	}
}

TextureImporter::~TextureImporter()
{
	for (machine a = kMaxTextureImportCount - 1; a >= 0; a--) delete[] textureImage[a];
}

int32 TextureImporter::GetSettingCount(void) const
{
	return (34);
}

Setting *TextureImporter::GetSetting(int32 index) const
{
	const StringTable *table = TheTextureTool->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('ITEX', 'FRMT'));
		return (new HeadingSetting('FRMT', title));
	}
	
	if (index == 1)
	{
		int32 selection = 0;
		for (machine a = 1; a < kTextureTypeIDCount; a++)
		{
			if (textureHeader[0].textureType == kTextureTypeID[a])
			{
				selection = a;
				break;
			}
		}
		
		const char *title = table->GetString(StringID('ITEX', 'TYPE'));
		MenuSetting *menu = new MenuSetting('TYPE', selection, title, kTextureTypeIDCount);
		for (machine a = 0; a < kTextureTypeIDCount; a++) menu->SetMenuItemString(a, table->GetString(StringID('ITEX', 'TYPE', kTextureTypeID[a])));
		return (menu);
	}
	
	if (index == 2)
	{
		int32 selection = 0;
		for (machine a = 1; a < kTextureFormatIDCount; a++)
		{
			if (textureHeader[0].imageFormat == kTextureFormatID[a])
			{
				selection = a;
				break;
			}
		}
		
		const char *title = table->GetString(StringID('ITEX', 'FORM'));
		MenuSetting *menu = new MenuSetting('FORM', selection, title, kTextureFormatIDCount);
		for (machine a = 0; a < kTextureFormatIDCount; a++) menu->SetMenuItemString(a, table->GetString(StringID('ITEX', 'FORM', kTextureFormatID[a])));
		return (menu);
	}
	
	if (index == 3)
	{
		int32 selection = 0;
		for (machine a = 1; a < kTextureAlphaIDCount; a++)
		{
			if (textureHeader[0].alphaSemantic == kTextureAlphaID[a])
			{
				selection = a;
				break;
			}
		}
		
		const char *title = table->GetString(StringID('ITEX', 'ALFA'));
		MenuSetting *menu = new MenuSetting('ALFA', selection, title, kTextureAlphaIDCount);
		for (machine a = 0; a < kTextureAlphaIDCount; a++) menu->SetMenuItemString(a, table->GetString(StringID('ITEX', 'ALFA', kTextureAlphaID[a])));
		return (menu);
	}
	
	if ((index == 4) || (index == 5))
	{
		int32 selection = 0;
		TextureWrap wrap = textureHeader[0].wrapMode[index - 4];
		for (machine a = 1; a < kTextureWrapIDCount; a++)
		{
			if (wrap == kTextureWrapID[a])
			{
				selection = a;
				break;
			}
		}
		
		unsigned_int32 id = (index == 4) ? 'WRPS' : 'WRPT';
		const char *title = table->GetString(StringID('ITEX', id));
		MenuSetting *menu = new MenuSetting(id, selection, title, kTextureWrapIDCount);
		for (machine a = 0; a < kTextureWrapIDCount; a++) menu->SetMenuItemString(a, table->GetString(StringID('ITEX', 'WRAP', kTextureWrapID[a])));
		return (menu);
	}
	
	if (index == 6)
	{
		const char *title = table->GetString(StringID('ITEX', 'BC13'));
		return (new BooleanSetting('BC13', ((textureImportFlags & kTextureImportCompressionBC13) != 0), title));
	}
	
	if (index == 7)
	{
		const char *title = table->GetString(StringID('ITEX', 'FLAG'));
		return (new HeadingSetting('FLAG', title));
	}
	
	if (index == 8)
	{
		const char *title = table->GetString(StringID('ITEX', 'FILT'));
		return (new BooleanSetting('FILT', ((textureHeader[0].textureFlags & kTextureFilterInhibit) != 0), title));
	}
	
	if (index == 9)
	{
		const char *title = table->GetString(StringID('ITEX', 'ANIS'));
		return (new BooleanSetting('ANIS', ((textureHeader[0].textureFlags & kTextureAnisotropicFilterInhibit) != 0), title));
	}
	
	if (index == 10)
	{
		const char *title = table->GetString(StringID('ITEX', 'RESO'));
		return (new BooleanSetting('RESO', ((textureHeader[0].textureFlags & kTextureForceHighQuality) != 0), title));
	}
	
	if (index == 11)
	{
		const char *title = table->GetString(StringID('ITEX', 'MAPG'));
		return (new HeadingSetting('MAPG', title));
	}
	
	if (index == 12)
	{
		const char *title = table->GetString(StringID('ITEX', 'NRML'));
		return (new BooleanSetting('NRML', ((textureImportFlags & kTextureImportNormalMap) != 0), title));
	}
	
	if (index == 13)
	{
		const char *title = table->GetString(StringID('ITEX', 'PLAX'));
		return (new BooleanSetting('PLAX', ((textureImportFlags & kTextureImportParallaxMap) != 0), title));
	}
	
	if (index == 14)
	{
		const char *title = table->GetString(StringID('ITEX', 'HRZN'));
		return (new BooleanSetting('HRZN', ((textureImportFlags & kTextureImportHorizonMap) != 0), title));
	}
	
	if (index == 15)
	{
		const char *title = table->GetString(StringID('ITEX', 'OCCL'));
		return (new BooleanSetting('OCCL', ((textureImportFlags & kTextureImportAmbientOcclusion) != 0), title));
	}
	
	if (index == 16)
	{
		const char *title = table->GetString(StringID('ITEX', 'SCAL'));
		return (new TextSetting('SCAL', heightScale, title));
	}
	
	if (index == 17)
	{
		const char *title = table->GetString(StringID('ITEX', 'HCHN'));
		MenuSetting *menu = new MenuSetting('HCHN', heightChannel, title, 4);
		
		menu->SetMenuItemString(0, table->GetString(StringID('ITEX', 'HCHN', 'RED ')));
		menu->SetMenuItemString(1, table->GetString(StringID('ITEX', 'HCHN', 'GREN')));
		menu->SetMenuItemString(2, table->GetString(StringID('ITEX', 'HCHN', 'BLUE')));
		menu->SetMenuItemString(3, table->GetString(StringID('ITEX', 'HCHN', 'ALFA')));
		
		return (menu);
	}
	
	if (index == 18)
	{
		const char *title = table->GetString(StringID('ITEX', 'OPTN'));
		return (new HeadingSetting('OPTN', title));
	}
	
	if (index == 19)
	{
		// Subtract 3 from kMaxResourceNameLength so that "-h1" or "-h2" can be added for horizon maps.
		
		const char *title = table->GetString(StringID('ITEX', 'NAME'));
		return (new TextSetting('NAME', outputTextureName, title, kMaxResourceNameLength - 3));
	}
	
	if (index == 20)
	{
		const char *title = table->GetString(StringID('ITEX', 'MPMP'));
		return (new BooleanSetting('MPMP', ((textureImportFlags & kTextureImportMipmaps) != 0), title));
	}
	
	if (index == 21)
	{
		const char *title = table->GetString(StringID('ITEX', 'BLED'));
		return (new BooleanSetting('BLED', ((textureImportFlags & kTextureImportBleedAlphaTest) != 0), title));
	}
	
	if (index == 22)
	{
		const char *title = table->GetString(StringID('ITEX', 'VECT'));
		return (new BooleanSetting('VECT', ((textureImportFlags & kTextureImportVectorData) != 0), title));
	}
	
	if (index == 23)
	{
		const char *title = table->GetString(StringID('ITEX', 'IGRN'));
		return (new BooleanSetting('IGRN', ((textureImportFlags & kTextureImportInvertGreen) != 0), title));
	}
	
	if (index == 24)
	{
		const char *title = table->GetString(StringID('ITEX', 'HALF'));
		return (new BooleanSetting('HALF', ((textureImportFlags & kTextureImportHalfScale) != 0), title));
	}
	
	if (index == 25)
	{
		const char *title = table->GetString(StringID('ITEX', 'HHOR'));
		return (new BooleanSetting('HHOR', ((textureImportFlags & kTextureImportHorizonHalfScale) != 0), title));
	}
	
	if (index == 26)
	{
		const char *title = table->GetString(StringID('ITEX', 'FLIP'));
		return (new BooleanSetting('FLIP', ((textureImportFlags & kTextureImportFlipVertical) != 0), title));
	}
	
	if (index == 27)
	{
		const char *title = table->GetString(StringID('ITEX', 'RMEM'));
		return (new BooleanSetting('RMEM', ((textureImportFlags & kTextureImportRemember) != 0), title));
	}
	
	if (index == 28)
	{
		const char *title = table->GetString(StringID('ITEX', 'HAZE'));
		return (new HeadingSetting('HAZE', title));
	}
	
	if (index == 29)
	{
		const char *title = table->GetString(StringID('ITEX', 'HCOL'));
		const char *picker = table->GetString(StringID('ITEX', 'HPCK'));
		return (new CheckColorSetting('HCOL', ((textureImportFlags & kTextureImportApplyHaze) != 0), hazeColor, title, picker));
	}
	
	if (index == 30)
	{
		const char *title = table->GetString(StringID('ITEX', 'ELEV'));
		return (new IntegerSetting('ELEV', hazeElevation, title, 1, 255, 1));
	}
	
	if (index == 31)
	{
		const char *title = table->GetString(StringID('ITEX', 'CURS'));
		return (new HeadingSetting('CURS', title));
	}
	
	if (index == 32)
	{
		const char *title = table->GetString(StringID('ITEX', 'CENX'));
		return (new TextSetting('CENX', imageCenter.x, title));
	}
	
	if (index == 33)
	{
		const char *title = table->GetString(StringID('ITEX', 'CENY'));
		return (new TextSetting('CENY', imageCenter.y, title));
	}
	
	return (nullptr);
}

void TextureImporter::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'TYPE')
	{
		int32 selection = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
		textureHeader[0].textureType = kTextureTypeID[selection];
	}
	else if (identifier == 'ALFA')
	{
		int32 selection = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
		textureHeader[0].alphaSemantic = (selection == 0) ? kTextureSemanticNone : kTextureAlphaID[selection];
	}
	else if (identifier == 'FORM')
	{
		int32 selection = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
		textureHeader[0].imageFormat = kTextureFormatID[selection];
	}
	else if (identifier == 'WRPS')
	{
		int32 selection = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
		textureHeader[0].wrapMode[0] = kTextureWrapID[selection];
	}
	else if (identifier == 'WRPT')
	{
		int32 selection = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
		textureHeader[0].wrapMode[1] = kTextureWrapID[selection];
	}
	else if (identifier == 'BC13')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportCompressionBC13;
		else textureImportFlags &= ~kTextureImportCompressionBC13;
	}
	else if (identifier == 'FILT')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureHeader[0].textureFlags |= kTextureFilterInhibit;
		else textureHeader[0].textureFlags &= ~kTextureFilterInhibit;
	}
	else if (identifier == 'ANIS')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureHeader[0].textureFlags |= kTextureAnisotropicFilterInhibit;
		else textureHeader[0].textureFlags &= ~kTextureAnisotropicFilterInhibit;
	}
	else if (identifier == 'RESO')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureHeader[0].textureFlags |= kTextureForceHighQuality;
		else textureHeader[0].textureFlags &= ~kTextureForceHighQuality;
	}
	else if (identifier == 'NRML')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportNormalMap;
		else textureImportFlags &= ~kTextureImportNormalMap;
	}
	else if (identifier == 'PLAX')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportParallaxMap;
		else textureImportFlags &= ~kTextureImportParallaxMap;
	}
	else if (identifier == 'HRZN')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportHorizonMap;
		else textureImportFlags &= ~kTextureImportHorizonMap;
	}
	else if (identifier == 'OCCL')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportAmbientOcclusion;
		else textureImportFlags &= ~kTextureImportAmbientOcclusion;
	}
	else if (identifier == 'SCAL')
	{
		heightScale = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
	}
	else if (identifier == 'HCHN')
	{
		heightChannel = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
	}
	else if (identifier == 'NAME')
	{
		outputTextureName = static_cast<const TextSetting *>(setting)->GetText();
	}
	else if (identifier == 'MPMP')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportMipmaps;
		else textureImportFlags &= ~kTextureImportMipmaps;
	}
	else if (identifier == 'BLED')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportBleedAlphaTest;
		else textureImportFlags &= ~kTextureImportBleedAlphaTest;
	}
	else if (identifier == 'VECT')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportVectorData;
		else textureImportFlags &= ~kTextureImportVectorData;
	}
	else if (identifier == 'IGRN')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportInvertGreen;
		else textureImportFlags &= ~kTextureImportInvertGreen;
	}
	else if (identifier == 'HALF')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportHalfScale;
		else textureImportFlags &= ~kTextureImportHalfScale;
	}
	else if (identifier == 'HHOR')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportHorizonHalfScale;
		else textureImportFlags &= ~kTextureImportHorizonHalfScale;
	}
	else if (identifier == 'FLIP')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportFlipVertical;
		else textureImportFlags &= ~kTextureImportFlipVertical;
	}
	else if (identifier == 'RMEM')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) textureImportFlags |= kTextureImportRemember;
		else textureImportFlags &= ~kTextureImportRemember;
	}
	else if (identifier == 'HCOL')
	{
		const CheckColorSetting *checkColorSetting = static_cast<const CheckColorSetting *>(setting);
		bool b = checkColorSetting->GetCheckValue();
		if (b)
		{
			textureImportFlags |= kTextureImportApplyHaze;
			hazeColor = checkColorSetting->GetColor();
		}
		else
		{
			textureImportFlags &= ~kTextureImportApplyHaze;
		}
	}
	else if (identifier == 'ELEV')
	{
		hazeElevation = static_cast<const IntegerSetting *>(setting)->GetIntegerValue();
	}
	else if (identifier == 'CENX')
	{
		imageCenter.x = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
	}
	else if (identifier == 'CENY')
	{
		imageCenter.y = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
	}
}

TextureImportResult TextureImporter::ValidateSettings(int32 *textureCount, int32 *textureComponentCount)
{
	int32 count = 0;
	do
	{
		if (!textureImage[count]) break;
		
		TextureHeader *header = &textureHeader[count];
		
		int32 componentCount = 1;
		int32 width = header->imageWidth;
		int32 height = header->imageHeight;
		
		TextureType textureType = header->textureType;
		if (textureType == kTextureCube)
		{
			height /= 6;
			if (((width & (width - 1)) != 0) || (height != width) || (height * 6 != header->imageHeight)) return (kTextureImportBadTextureSize);
			
			header->imageHeight = height;
			componentCount = 6;
		}
		else if (textureType == kTextureArray2D)
		{
			if ((width & (width - 1)) != 0) return (kTextureImportBadTextureSize);
			
			int32 depth = height / width;
			if (width * depth != height) return (kTextureImportBadTextureSize);
			
			header->imageHeight = width;
			header->imageDepth = depth;
		}
		else if (textureType != kTextureRectangle)
		{
			if (((width & (width - 1)) != 0) || ((height & (height - 1)) != 0)) return (kTextureImportBadTextureSize);
		}
		
		textureComponentCount[count] = componentCount;
		
		TextureType type = header->textureType;
		if (type == kTextureRectangle)
		{
			header->wrapMode[0] = kTextureClamp;
			header->wrapMode[1] = kTextureClamp;
			
			textureImportFlags &= ~(kTextureImportMipmaps | kTextureImportCompressionBC13);
		}
		else if (type == kTextureCube)
		{
			textureImportFlags &= ~(kTextureImportFlipVertical | kTextureImportAmbientOcclusion | kTextureImportNormalMap | kTextureImportApplyHaze);
		}
		
		TextureFormat format = header->imageFormat;
		if ((format == kTextureL8) || (format == kTextureI8))
		{
			header->alphaSemantic = kTextureSemanticNone;
			textureImportFlags &= ~kTextureImportAmbientOcclusion;
		}
		
		if (header->textureFlags & kTextureFilterInhibit)
		{
			header->textureFlags &= ~kTextureAnisotropicFilterInhibit;
			textureImportFlags &= ~kTextureImportMipmaps;
		}
		
		if (textureImportFlags & kTextureImportNormalMap)
		{
			header->colorSemantic = kTextureSemanticNormal;
			header->imageFormat = kTextureRGBA8;
			
			if (textureImportFlags & kTextureImportParallaxMap)
			{
				header->alphaSemantic = kTextureSemanticParallax;
				textureImportFlags &= ~(kTextureImportCompressionBC13 | kTextureImportAmbientOcclusion);
				parallaxScale = heightScale * 0.5F;
			}
			else
			{
				if (header->alphaSemantic == kTextureSemanticParallax) header->alphaSemantic = kTextureSemanticNone;
			}
			
			if ((textureImportFlags & kTextureImportHorizonMap) && (horizonIndex > 0))
			{
				textureImportFlags &= ~kTextureImportCompressionBC13;
			}
			else if (textureImportFlags & kTextureImportCompressionBC13)
			{
				header->alphaSemantic = kTextureSemanticNormal;
				textureImportFlags &= ~kTextureImportAmbientOcclusion;
			}
		}
		else
		{
			textureImportFlags &= ~(kTextureImportParallaxMap | kTextureImportHorizonMap);
			
			if ((textureImportFlags & (kTextureImportVectorData | kTextureImportCompressionBC13)) == (kTextureImportVectorData | kTextureImportCompressionBC13))
			{
				header->alphaSemantic = kTextureSemanticNormal;
				textureImportFlags &= ~kTextureImportAmbientOcclusion;
			}
			else
			{
				if (header->alphaSemantic == kTextureSemanticParallax) header->alphaSemantic = kTextureSemanticNone;
			}
		}
		
		if (textureImportFlags & kTextureImportCompressionBC13) header->imageFormat = kTextureBC13;
		
	} while (++count < kMaxTextureImportCount);
	
	*textureCount = count;
	return (kTextureImportOkay);
}

void TextureImporter::CopyTarga_L8(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		unsigned_int32 value = data[0];
		image[a].Set(value, value, value, 255);
		data++;
	}
}

void TextureImporter::CopyTarga_RGB16(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		unsigned_int32 p = ReadLittleEndianU16(reinterpret_cast<const unsigned_int16 *>(data));
		unsigned_int32 red = p >> 10;
		unsigned_int32 green = (p >> 5) & 31;
		unsigned_int32 blue = p & 31;
		
		red = (red << 3) | (red >> 2);
		green = (green << 3) | (green >> 2);
		blue = (blue << 3) | (blue >> 2);
		
		image[a].Set(red, green, blue, 255);
		data += 2;
	}
}

void TextureImporter::CopyTarga_RGB24(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image[a].Set(data[2], data[1], data[0], 255);
		data += 3;
	}
}

void TextureImporter::CopyTarga_RGB32(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image[a].Set(data[2], data[1], data[0], 255);
		data += 4;
	}
}

void TextureImporter::CopyTarga_RGBA32(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image[a].Set(data[2], data[1], data[0], data[3]);
		data += 4;
	}
}

void TextureImporter::InvertGreenChannel(Color4C *image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image->SetGreen(255 - image->GetGreen());
		image++;
	}
}

void TextureImporter::ClearAlphaChannel(Color4C *image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image->SetAlpha(255);
		image++;
	}
}

void TextureImporter::CopyRedToAlpha(Color4C *image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image->SetAlpha(image->GetRed());
		image++;
	}
}

void TextureImporter::CopyGreenToAlpha(Color4C *image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image->SetAlpha(image->GetGreen());
		image++;
	}
}

void TextureImporter::CopyBlueToAlpha(Color4C *image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image->SetAlpha(image->GetBlue());
		image++;
	}
}

void TextureImporter::CopyGreenToRed(Color4C *image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image->SetRed(image->GetGreen());
		image++;
	}
}

void TextureImporter::CopyBlueToRed(Color4C *image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image->SetRed(image->GetBlue());
		image++;
	}
}

void TextureImporter::CopyAlphaToRed(Color4C *image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image->SetRed(image->GetAlpha());
		image++;
	}
}

void TextureImporter::Convert_RGBVector_XY(Color4C *image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		unsigned_int32 x = image->GetRed();
		unsigned_int32 y = image->GetGreen();
		image->Set(0, y, 0, x);
		image++;
	}
}

void TextureImporter::Convert_RGBA8_LA8(const Color4C *source, Color2C *restrict destin, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		const Color4C& c = *source++;
		unsigned_int32 lum = Max(Max(c.GetRed(), c.GetGreen()), c.GetBlue());
		destin->Set(lum, c.GetAlpha());
		destin++;
	}
}

void TextureImporter::Convert_RGBA8_L8(const Color4C *source, Color1C *restrict destin, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		const Color4C& c = *source++;
		*destin++ = (Color1C) Max(Max(c.GetRed(), c.GetGreen()), c.GetBlue());
	}
}

void TextureImporter::ApplyHazeColor(Color4C *image, int32 width, int32 height, const ColorRGBA& color, int32 elevation)
{
	float w = (float) (width - 1);
	float xoffset = w * -0.5F;
	float xscale = 2.0F / w;
	float yscale = 1.0F / (float) (height - 1);
	float normalizer = 128.0F / (float) elevation;
	
	unsigned_int32 red = (unsigned_int32) (color.red * 255.0F);
	unsigned_int32 green = (unsigned_int32) (color.green * 255.0F);
	unsigned_int32 blue = (unsigned_int32) (color.blue * 255.0F);
	
	for (machine j = 0; j < height; j++)
	{
		float y = (float) j * yscale;
		if (y * InverseSqrt(y * y + 2.0F) * normalizer < 1.0F)
		{
			for (machine i = 0; i < width; i++)
			{
				float x = ((float) i + xoffset) * xscale;
				UnsignedFixed t = (UnsignedFixed) (Fmin(y * InverseSqrt(x * x + y * y + 1.0F) * normalizer, 1.0F) * 256.0F);
				UnsignedFixed u = 256 - t;
				
				unsigned_int32 r = (image->GetRed() * t + red * u) >> 8;
				unsigned_int32 g = (image->GetGreen() * t + green * u) >> 8;
				unsigned_int32 b = (image->GetBlue() * t + blue * u) >> 8;
				image->Set(r, g, b, image->GetAlpha());
				
				image++;
			}
		}
		else
		{
			image += width;
		}
	}
}

TextureImportResult TextureImporter::ScaleHalfResolution(int32 index)
{
	int32 width = textureHeader[index].imageWidth;
	int32 height = textureHeader[index].imageHeight;
	if (((width | height) & 1) != 0) return (kTextureImportBadTextureSize);
	
	int32 row = width;
	
	width >>= 1;
	height >>= 1;
	textureHeader[index].imageWidth = width;
	textureHeader[index].imageHeight = height;
	
	const Color4C *source = textureImage[index];
	Color4C *destin = textureImage[index];
	
	for (machine j = 0; j < height; j++)
	{
		for (machine i = 0; i < width; i++)
		{
			const Color4C& c1 = source[0];
			const Color4C& c2 = source[1];
			const Color4C& c3 = source[row];
			const Color4C& c4 = source[row + 1];
			source += 2;
			
			unsigned_int32 red = (c1.GetRed() + c2.GetRed() + c3.GetRed() + c4.GetRed() + 2) >> 2;
			unsigned_int32 green = (c1.GetGreen() + c2.GetGreen() + c3.GetGreen() + c4.GetGreen() + 2) >> 2;
			unsigned_int32 blue = (c1.GetBlue() + c2.GetBlue() + c3.GetBlue() + c4.GetBlue() + 2) >> 2;
			unsigned_int32 alpha = (c1.GetAlpha() + c2.GetAlpha() + c3.GetAlpha() + c4.GetAlpha() + 2) >> 2;
			
			destin->Set(red, green, blue, alpha);
			destin++;
		}
		
		source += row;
	}
	
	if (textureImportFlags & kTextureImportVectorData)
	{
		int32 pixelCount = width * height;
		Color4C *image = textureImage[index];
		
		for (machine a = 0; a < pixelCount; a++)
		{
			const Color4C& color = image[a];
			int32 red = color.GetRed();
			int32 green = color.GetGreen();
			int32 blue = color.GetBlue();
			
			float r = (float) (red - 128) * K::one_over_127;
			float g = (float) (green - 128) * K::one_over_127;
			float b = (float) (blue - 128) * K::one_over_127;
			
			float m = InverseSqrt(r * r + g * g + b * b) * 127.0F;
			red = (int32) (r * m + 0.5F) + 128;
			green = (int32) (g * m + 0.5F) + 128;
			blue = (int32) (b * m + 0.5F) + 128;
			
			image[a].Set(red, green, blue, color.GetAlpha());
		}
	}
	
	return (kTextureImportOkay);
}

TextureImportResult TextureImporter::SetTextureImage(int32 textureIndex, const char *name)
{
	delete[] textureImage[textureIndex];
	textureImage[textureIndex] = nullptr;
	
	TargaResource *resource = TargaResource::Get(name, 0, ThePluginMgr->GetImportCatalog());
	if (!resource) return (kTextureImportFileNotFound);
	AutoRelease<TargaResource> targa(resource);
	
	const TargaHeader *targaHeader = targa->GetTargaHeader();
	int32 pixelDepth = targaHeader->pixelDepth;
	int32 imageType = targaHeader->imageType;
	
	if ((targaHeader->colorMapType != 0) || ((pixelDepth != 8) && (pixelDepth != 16) && (pixelDepth != 24) && (pixelDepth != 32)) || ((imageType != 2) && (imageType != 3) && (imageType != 10) && (imageType != 11))) return (kTextureImportBadFileFormat);
	
	inputTextureName = name;
	
	int32 width = targaHeader->width;
	int32 height = targaHeader->height;
	textureHeader[textureIndex].imageWidth = width;
	textureHeader[textureIndex].imageHeight = height;
	
	int32 pixelCount = width * height;
	Color4C *image = new Color4C[pixelCount];
	textureImage[textureIndex] = image;
	
	const unsigned_int8 *data = targaHeader->GetPixelData();
	
	if ((imageType & 8) == 0)
	{
		if (pixelDepth == 8) CopyTarga_L8(data, image, pixelCount);
		else if (pixelDepth == 16) CopyTarga_RGB16(data, image, pixelCount);
		else if (pixelDepth == 24) CopyTarga_RGB24(data, image, pixelCount);
		else CopyTarga_RGBA32(data, image, pixelCount);
	}
	else
	{
		if (pixelDepth == 8) Image::DecompressImageRLE_L8(data, image, pixelCount);
		else if (pixelDepth == 16) Image::DecompressImageRLE_RGB16(data, image, pixelCount);
		else if (pixelDepth == 24) Image::DecompressImageRLE_RGB24(data, image, pixelCount);
		else Image::DecompressImageRLE_RGBA32(data, image, pixelCount);
	}
	
	if (textureImportFlags & kTextureImportHalfScale)
	{
		TextureImportResult result = ScaleHalfResolution(textureIndex);
		if (result != kTextureImportOkay) return (result);
	}
	
	if (((targaHeader->imageDescriptor & 0x20) != 0) ^ ((textureImportFlags & kTextureImportFlipVertical) != 0))
	{
		int32 h = height >> 1;
		for (machine a = 0; a < h; a++)
		{
			Color4C *top = image + a * width;
			Color4C *bottom = image + (height - a - 1) * width;
			for (machine b = 0; b < width; b++)
			{
				Color4C c = *top;
				*top++ = *bottom;
				*bottom++ = c;
			}
		}
	}
	
	return (kTextureImportOkay);
}

void TextureImporter::SetTextureImage(int32 textureIndex, int32 width, int32 height, const Color4C *image)
{
	delete[] textureImage[textureIndex];
	
	int32 pixelCount = width * height;
	Color4C *storage = new Color4C[pixelCount];
	textureImage[textureIndex] = storage;
	
	MemoryMgr::CopyMemory(image, storage, pixelCount * sizeof(Color4C));
	
	textureHeader[textureIndex].imageFormat = kTextureRGBA8;
	textureHeader[textureIndex].imageWidth = width;
	textureHeader[textureIndex].imageHeight = height;
}

void TextureImporter::SetTextureImage(int32 textureIndex, int32 width, int32 height, const Color2C *image)
{
	delete[] textureImage[textureIndex];
	
	int32 pixelCount = width * height;
	Color4C *storage = new Color4C[pixelCount];
	textureImage[textureIndex] = storage;
	
	for (machine a = 0; a < pixelCount; a++)
	{
		const Color2C& c = *image++;
		unsigned_int32 lum = c.GetLum();
		storage->Set(lum, lum, lum, c.GetAlpha());
		storage++;
	}
	
	textureHeader[textureIndex].imageFormat = kTextureLA8;
	textureHeader[textureIndex].imageWidth = width;
	textureHeader[textureIndex].imageHeight = height;
}

void TextureImporter::SetTextureImage(int32 textureIndex, int32 width, int32 height, const Color1C *image)
{
	delete[] textureImage[textureIndex];
	
	int32 pixelCount = width * height;
	Color4C *storage = new Color4C[pixelCount];
	textureImage[textureIndex] = storage;
	
	for (machine a = 0; a < pixelCount; a++)
	{
		Color1C c = *image++;
		storage->Set(c, c, c, 255);
		storage++;
	}
	
	textureHeader[textureIndex].imageFormat = kTextureL8;
	textureHeader[textureIndex].imageWidth = width;
	textureHeader[textureIndex].imageHeight = height;
}

void TextureImporter::ProcessImage(int32 textureIndex, int32 componentCount, Color4C *processedImage)
{
	Color4C *tempImage = nullptr;
	
	int32 width = textureHeader[textureIndex].imageWidth;
	int32 height = textureHeader[textureIndex].imageHeight;
	int32 depth = textureHeader[textureIndex].imageDepth;
	int32 pixelCount = width * height;
	
	bool swrap = (textureHeader[textureIndex].wrapMode[0] == kTextureRepeat);
	bool twrap = (textureHeader[textureIndex].wrapMode[1] == kTextureRepeat);
	
	for (machine component = 0; component < componentCount; component++)
	{
		const Color4C *source = textureImage[textureIndex] + (componentCount - 1 - component) * pixelCount * depth;
		for (machine layer = 0; layer < depth; layer++)
		{
			MemoryMgr::CopyMemory(source + (depth - 1 - layer) * pixelCount, processedImage, pixelCount * sizeof(Color4C));
			
			if (textureImportFlags & kTextureImportAmbientOcclusion)
			{
				if (heightChannel == 0) CopyRedToAlpha(processedImage, pixelCount);
				else if (heightChannel == 1) CopyGreenToAlpha(processedImage, pixelCount);
				else if (heightChannel == 2) CopyBlueToAlpha(processedImage, pixelCount);
			}
			
			if (textureImportFlags & kTextureImportNormalMap)
			{
				if (heightChannel == 1) CopyGreenToRed(processedImage, pixelCount);
				else if (heightChannel == 2) CopyBlueToRed(processedImage, pixelCount);
				else if (heightChannel == 3) CopyAlphaToRed(processedImage, pixelCount);
			}
			
			if (textureImportFlags & kTextureImportBleedAlphaTest)
			{
				Batch	batch;
				
				if (!tempImage) tempImage = new Color4C[pixelCount];
				MemoryMgr::CopyMemory(processedImage, tempImage, pixelCount * sizeof(Color4C));
				
				int32 xcount = (width + 63) >> 6;
				int32 ycount = (height + 63) >> 6;
				
				if (!(textureImportFlags & kTextureImportVectorData))
				{
					for (machine j = 0; j < ycount; j++)
					{
						int32 y = j << 6;
						for (machine i = 0; i < xcount; i++)
						{
							int32 x = i << 6;
							BleedAlphaTestJob *job = new BleedAlphaTestJob(width, height, Rect(x, y, Min(x + 64, width), Min(y + 64, height)), tempImage, processedImage, 192);
							job->SetJobFlags(kJobNonpersistent);
							TheJobMgr->SubmitJob(job, &batch);
						}
					}
				}
				else
				{
					for (machine j = 0; j < ycount; j++)
					{
						int32 y = j << 6;
						for (machine i = 0; i < xcount; i++)
						{
							int32 x = i << 6;
							BleedNormalJob *job = new BleedNormalJob(width, height, Rect(x, y, Min(x + 64, width), Min(y + 64, height)), tempImage, processedImage);
							job->SetJobFlags(kJobNonpersistent);
							TheJobMgr->SubmitJob(job, &batch);
						}
					}
				}
				
				TheJobMgr->FinishBatch(&batch);
			}
			
			if (textureImportFlags & kTextureImportAmbientOcclusion)
			{
				Batch	batch;
				
				textureHeader[textureIndex].alphaSemantic = kTextureSemanticOcclusion;
				
				if (!tempImage) tempImage = new Color4C[pixelCount];
				MemoryMgr::CopyMemory(processedImage, tempImage, pixelCount * sizeof(Color4C));
				
				int32 xcount = (width + 63) >> 6;
				int32 ycount = (height + 63) >> 6;
				for (machine j = 0; j < ycount; j++)
				{
					int32 y = j << 6;
					for (machine i = 0; i < xcount; i++)
					{
						int32 x = i << 6;
						AmbientJob *job = new AmbientJob(width, height, Rect(x, y, Min(x + 64, width), Min(y + 64, height)), tempImage, processedImage, heightScale, swrap, twrap);
						job->SetJobFlags(kJobNonpersistent);
						TheJobMgr->SubmitJob(job, &batch);
					}
				}
				
				TheJobMgr->FinishBatch(&batch);
			}
			
			if (textureImportFlags & kTextureImportApplyHaze)
			{
				ApplyHazeColor(processedImage, width, height, hazeColor, hazeElevation);
			}
			
			if (textureImportFlags & kTextureImportNormalMap)
			{
				if (!tempImage) tempImage = new Color4C[pixelCount];
				MemoryMgr::CopyMemory(processedImage, tempImage, pixelCount * sizeof(Color4C));
				
				if ((textureImportFlags & kTextureImportHorizonMap) && (horizonIndex != 0))
				{
					Batch	batch;
					
					TextureSemantic semantic = (horizonIndex == 1) ? kTextureSemanticHorizon1 : kTextureSemanticHorizon2;
					textureHeader[textureIndex].colorSemantic = semantic;
					textureHeader[textureIndex].alphaSemantic = semantic;
					
					int32 xcount = (width + 63) >> 6;
					int32 ycount = (height + 63) >> 6;
					for (machine j = 0; j < ycount; j++)
					{
						int32 y = j << 6;
						for (machine i = 0; i < xcount; i++)
						{
							int32 x = i << 6;
							HorizonJob *job = new HorizonJob(horizonIndex - 1, width, height, Rect(x, y, Min(x + 64, width), Min(y + 64, height)), tempImage, processedImage, heightScale, swrap, twrap);
							job->SetJobFlags(kJobNonpersistent);
							TheJobMgr->SubmitJob(job, &batch);
						}
					}
					
					TheJobMgr->FinishBatch(&batch);
				}
				else
				{
					if (textureHeader[textureIndex].alphaSemantic == kTextureSemanticParallax)
					{
						Image::CalculateParallaxMap(width, height, tempImage, processedImage, heightScale, swrap, twrap);
					}
					else
					{
						if (textureHeader[textureIndex].alphaSemantic == kTextureSemanticNormal) Image::CalculateXYNormalMap(width, height, tempImage, processedImage, heightScale, swrap, twrap);
						else Image::CalculateNormalMap(width, height, tempImage, processedImage, heightScale, swrap, twrap);
					}
				}
			}
			else if (textureImportFlags & kTextureImportVectorData)
			{
				textureHeader[textureIndex].colorSemantic = kTextureSemanticNormal;
				
				if (textureImportFlags & kTextureImportCompressionBC13)
				{
					textureHeader[textureIndex].alphaSemantic = kTextureSemanticNormal;
					Convert_RGBVector_XY(processedImage, pixelCount);
				}
			}
			
			if (textureImportFlags & kTextureImportInvertGreen) InvertGreenChannel(processedImage, pixelCount);
			if (textureHeader[textureIndex].alphaSemantic == kTextureSemanticNone) ClearAlphaChannel(processedImage, pixelCount);
			
			processedImage += pixelCount;
		}
	}
	
	delete[] tempImage;
}

void TextureImporter::ProcessImage(int32 textureIndex, int32 componentCount, Color2C *processedImage)
{
	int32 width = textureHeader[textureIndex].imageWidth;
	int32 height = textureHeader[textureIndex].imageHeight;
	int32 depth = textureHeader[textureIndex].imageDepth;
	int32 pixelCount = width * height;
	
	for (machine a = 0; a < componentCount; a++)
	{
		const Color4C *source = textureImage[textureIndex] + (componentCount - 1 - a) * pixelCount * depth;
		for (machine layer = 0; layer < depth; layer++)
		{
			Convert_RGBA8_LA8(source + (depth - 1 - layer) * pixelCount, processedImage, pixelCount);
			processedImage += pixelCount;
		}
	}
}

void TextureImporter::ProcessImage(int32 textureIndex, int32 componentCount, Color1C *processedImage)
{
	int32 width = textureHeader[textureIndex].imageWidth;
	int32 height = textureHeader[textureIndex].imageHeight;
	int32 depth = textureHeader[textureIndex].imageDepth;
	int32 pixelCount = width * height;
	
	for (machine a = 0; a < componentCount; a++)
	{
		const Color4C *source = textureImage[textureIndex] + (componentCount - 1 - a) * pixelCount * depth;
		for (machine layer = 0; layer < depth; layer++)
		{
			Convert_RGBA8_L8(source + (depth - 1 - layer) * pixelCount, processedImage, pixelCount);
			processedImage += pixelCount;
		}
	}
}

unsigned_int32 TextureImporter::GetAuxiliaryDataSize(void) const
{
	unsigned_int32 auxiliaryDataSize = 0;
	
	if (parallaxScale != 0.0F) auxiliaryDataSize += sizeof(ChunkHeader) + 4;
	if ((imageCenter.x != 0.0F) || (imageCenter.y != 0.0F)) auxiliaryDataSize += sizeof(ChunkHeader) + sizeof(Point2D);
	if (impostorSize.x != 0.0F) auxiliaryDataSize += sizeof(ChunkHeader) + sizeof(Vector2D);
	if (impostorClipData[2] != 0.0F) auxiliaryDataSize += sizeof(ChunkHeader) + 16;
	
	return (auxiliaryDataSize);
}

void TextureImporter::WriteAuxiliaryData(File& file) const
{
	ChunkHeader		chunkHeader;
	
	if (parallaxScale != 0.0F)
	{
		chunkHeader.chunkType = kTextureChunkParallaxScale;
		chunkHeader.chunkSize = 4;
		
		file.Write(&chunkHeader, sizeof(ChunkHeader));
		file.Write(&parallaxScale, 4);
	}
	
	if ((imageCenter.x != 0.0F) || (imageCenter.y != 0.0F))
	{
		chunkHeader.chunkType = kTextureChunkImageCenter;
		chunkHeader.chunkSize = sizeof(Point2D);
		
		file.Write(&chunkHeader, sizeof(ChunkHeader));
		file.Write(&imageCenter, sizeof(Point2D));
	}
	
	if (impostorSize.x != 0.0F)
	{
		chunkHeader.chunkType = kTextureChunkImpostorSize;
		chunkHeader.chunkSize = sizeof(Vector2D);
		
		file.Write(&chunkHeader, sizeof(ChunkHeader));
		file.Write(&impostorSize, sizeof(Vector2D));
	}
	
	if (impostorClipData[2] != 0.0F)
	{
		chunkHeader.chunkType = kTextureChunkImpostorClipData;
		chunkHeader.chunkSize = 16;
		
		file.Write(&chunkHeader, sizeof(ChunkHeader));
		file.Write(impostorClipData, 16);
	}
}

int32 TextureImporter::BlockCompressImageBC1(int32 count, int32 width, int32 height, const Color4C *image, unsigned_int8 *data)
{
	int32 blockCountX = (width + 3) >> 2;
	int32 blockCountY = (height + 3) >> 2;
	int32 blockWidth = Min(width, 4);
	int32 blockHeight = Min(height, 4);
	
	if (Min(blockCountX, blockCountY) > 1)
	{
		Batch	batch;
		
		int32 xcount = (blockCountX + 15) >> 4;
		int32 ycount = (blockCountY + 15) >> 4;
		
		for (machine a = 0; a < count; a++)
		{
			for (machine j = 0; j < ycount; j++)
			{
				int32 y = j << 4;
				for (machine i = 0; i < xcount; i++)
				{
					int32 x = i << 4;
					CompressionBC1Job *job = new CompressionBC1Job(width, Rect(x, y, Min(x + 16, blockCountX), Min(y + 16, blockCountY)), image, data);
					job->SetJobFlags(kJobNonpersistent);
					TheJobMgr->SubmitJob(job, &batch);
				}
			}
			
			image += width * height;
			data += blockCountX * blockCountY * 8;
		}
		
		TheJobMgr->FinishBatch(&batch);
	}
	else
	{
		for (machine a = 0; a < count; a++)
		{
			for (machine j = 0; j < blockCountY; j++)
			{
				for (machine i = 0; i < blockCountX; i++) Image::CompressColorBlock(blockWidth, blockHeight, width, false, image + i * 4, data + i * 8);
				image += width * blockHeight;
				data += blockCountX * 8;
			}
		}
	}
	
	return (blockCountX * blockCountY * count);
}

int32 TextureImporter::BlockCompressImageBC3(int32 count, int32 width, int32 height, const Color4C *image, unsigned_int8 *data)
{
	int32 blockCountX = (width + 3) >> 2;
	int32 blockCountY = (height + 3) >> 2;
	int32 blockWidth = Min(width, 4);
	int32 blockHeight = Min(height, 4);
	
	if (Min(blockCountX, blockCountY) > 1)
	{
		Batch	batch;
		
		int32 xcount = (blockCountX + 15) >> 4;
		int32 ycount = (blockCountY + 15) >> 4;
		
		for (machine a = 0; a < count; a++)
		{
			for (machine j = 0; j < ycount; j++)
			{
				int32 y = j << 4;
				for (machine i = 0; i < xcount; i++)
				{
					int32 x = i << 4;
					CompressionBC3Job *job = new CompressionBC3Job(width, Rect(x, y, Min(x + 16, blockCountX), Min(y + 16, blockCountY)), image, data);
					job->SetJobFlags(kJobNonpersistent);
					TheJobMgr->SubmitJob(job, &batch);
				}
			}
			
			image += width * height;
			data += blockCountX * blockCountY * 16;
		}
		
		TheJobMgr->FinishBatch(&batch);
	}
	else
	{
		for (machine a = 0; a < count; a++)
		{
			for (machine j = 0; j < blockCountY; j++)
			{
				for (machine i = 0; i < blockCountX; i++)
				{
					unsigned_int8	gray[16];
					
					const Color4C *input = image + i * 4;
					unsigned_int8 *output = data + i * 16;
					
					for (machine n = 0; n < 4; n++)
					{
						for (machine m = 0; m < 4; m++) gray[n * 4 + m] = (unsigned_int8) input[n * width + m].GetAlpha();
					}
					
					Image::CompressGrayBlock(blockWidth, blockHeight, 4, gray, output);
					Image::CompressColorBlock(blockWidth, blockHeight, width, true, input, output + 8);
				}
				
				image += width * blockHeight;
				data += blockCountX * 16;
			}
		}
	}
	
	return (blockCountX * blockCountY * count);
}

int32 TextureImporter::BlockCompressNormalImageBC3(int32 count, int32 width, int32 height, const Color4C *image, unsigned_int8 *data)
{
	int32 blockCountX = (width + 3) >> 2;
	int32 blockCountY = (height + 3) >> 2;
	int32 blockWidth = Min(width, 4);
	int32 blockHeight = Min(height, 4);
	
	if (Min(blockCountX, blockCountY) > 1)
	{
		Batch	batch;
		
		int32 xcount = (blockCountX + 15) >> 4;
		int32 ycount = (blockCountY + 15) >> 4;
		
		for (machine a = 0; a < count; a++)
		{
			for (machine j = 0; j < ycount; j++)
			{
				int32 y = j << 4;
				for (machine i = 0; i < xcount; i++)
				{
					int32 x = i << 4;
					CompressionNormalBC3Job *job = new CompressionNormalBC3Job(width, Rect(x, y, Min(x + 16, blockCountX), Min(y + 16, blockCountY)), image, data);
					job->SetJobFlags(kJobNonpersistent);
					TheJobMgr->SubmitJob(job, &batch);
				}
			}
			
			image += width * height;
			data += blockCountX * blockCountY * 16;
		}
		
		TheJobMgr->FinishBatch(&batch);
	}
	else
	{
		for (machine a = 0; a < count; a++)
		{
			for (machine j = 0; j < blockCountY; j++)
			{
				for (machine i = 0; i < blockCountX; i++)
				{
					unsigned_int8	gray[16];
					
					const Color4C *input = image + i * 4;
					unsigned_int8 *output = data + i * 16;
					
					for (machine n = 0; n < 4; n++)
					{
						for (machine m = 0; m < 4; m++) gray[n * 4 + m] = (unsigned_int8) input[n * width + m].GetAlpha();
					}
					
					Image::CompressGrayBlock(blockWidth, blockHeight, 4, gray, output);
					Image::CompressGreenBlock(blockWidth, blockHeight, width, input, output + 8);
				}
				
				image += width * blockHeight;
				data += blockCountX * 16;
			}
		}
	}
	
	return (blockCountX * blockCountY * count);
}

void TextureImporter::GetInputConfigPath(ResourcePath *path) const
{
	ThePluginMgr->GetImportCatalog()->GetResourcePath(ConfigResource::GetDescriptor(), inputTextureName, path);
}

void TextureImporter::GetOutputTexturePath(ResourcePath *path) const
{
	TheResourceMgr->GetGenericCatalog()->GetResourcePath(TextureResource::GetDescriptor(), outputTextureName, path);
}

TextureImportResult TextureImporter::ImportTextureImage(void)
{
	int32					textureComponentCount[kMaxTextureImportCount];
	int32					textureChainPixelCount[kMaxTextureImportCount];
	int32					textureCount;
	TextureResourceHeader	resourceHeader;
	ResourcePath			texturePath;
	File					textureFile;
	
	TextureImportResult result = ValidateSettings(&textureCount, textureComponentCount);
	if (result != kTextureImportOkay) return (result);
	if (textureCount == 0) return (kTextureImportOkay);
	
	GetOutputTexturePath(&texturePath);
	TheResourceMgr->CreateDirectoryPath(texturePath);
	if (textureFile.Open(texturePath, kFileCreate) != kFileOkay) return (kTextureImportCreateFailed);
	
	if ((textureImportFlags & kTextureImportRemember) && (inputTextureName[0] != 0) && (horizonIndex == 0))
	{
		ResourcePath	configPath;
		File			configFile;
		
		GetInputConfigPath(&configPath);
		if (configFile.Open(configPath, kFileCreate) == kFileOkay) WriteCommandLine(configFile);
	}
	
	unsigned_int32 auxiliaryDataSize = GetAuxiliaryDataSize();
	unsigned_int32 resourceHeaderSize = auxiliaryDataSize;
	unsigned_int32 accumMipmapDataSize = 0;
	
	for (machine textureIndex = 0; textureIndex < textureCount; textureIndex++)
	{
		int32 width = textureHeader[textureIndex].imageWidth;
		int32 height = textureHeader[textureIndex].imageHeight;
		
		int32 pixelCount = width * height;
		textureChainPixelCount[textureIndex] = pixelCount;
		int32 count = (textureImportFlags & kTextureImportMipmaps) ? Image::CalculateMipmapChainSize2D(width, height, &textureChainPixelCount[textureIndex]) : 1;
		
		int32 mipmapDataCount = textureComponentCount[textureIndex] * count;
		unsigned_int32 mipmapDataSize = mipmapDataCount * sizeof(TextureMipmapData);
		resourceHeaderSize += sizeof(TextureHeader) + mipmapDataSize;
		
		unsigned_int32 auxiliaryDataOffset = (textureCount - textureIndex) * sizeof(TextureHeader);
		
		textureHeader[textureIndex].mipmapCount = count;
		textureHeader[textureIndex].mipmapDataOffset = auxiliaryDataOffset + auxiliaryDataSize + accumMipmapDataSize;
		
		textureHeader[textureIndex].auxiliaryDataSize = auxiliaryDataSize;
		textureHeader[textureIndex].auxiliaryDataOffset = (auxiliaryDataSize != 0) ? auxiliaryDataOffset : 0;
		
		accumMipmapDataSize += mipmapDataSize;
	}
	
	resourceHeader.endian = 1;
	resourceHeader.headerDataSize = resourceHeaderSize;
	resourceHeader.textureCount = textureCount;
	
	textureFile.Write(&resourceHeader, sizeof(TextureResourceHeader));
	textureFile.Write(&textureHeader, textureCount * sizeof(TextureHeader));
	WriteAuxiliaryData(textureFile);
	
	unsigned_int32 textureImageOffset = sizeof(TextureResourceHeader) + textureCount * sizeof(TextureHeader) + auxiliaryDataSize + accumMipmapDataSize;
	
	for (machine textureIndex = 0; textureIndex < textureCount; textureIndex++)
	{
		unsigned_int32 pixelSize = 4;
		TextureFormat format = textureHeader[textureIndex].imageFormat;
		if (format == kTextureLA8) pixelSize = 2;
		else if ((format == kTextureL8) || (format == kTextureI8)) pixelSize = 1;
		
		int32 width = textureHeader[textureIndex].imageWidth;
		int32 height = textureHeader[textureIndex].imageHeight;
		int32 depth = textureHeader[textureIndex].imageDepth;
		
		int32 componentCount = textureComponentCount[textureIndex];
		int32 chainStorageSize = textureChainPixelCount[textureIndex] * depth * pixelSize;
		unsigned_int8 *chainStorage = new unsigned_int8[chainStorageSize * componentCount];
		
		if (pixelSize == 4) ProcessImage(textureIndex, componentCount, reinterpret_cast<Color4C *>(chainStorage));
		else if (pixelSize == 2) ProcessImage(textureIndex, componentCount, reinterpret_cast<Color2C *>(chainStorage));
		else ProcessImage(textureIndex, componentCount, reinterpret_cast<Color1C *>(chainStorage));
		
		if (textureImportFlags & kTextureImportMipmaps)
		{
			if (pixelSize == 4)
			{
				unsigned_int32 flags = 0;
				if (textureHeader[textureIndex].colorSemantic == kTextureSemanticNormal)
				{
					if (textureHeader[textureIndex].imageFormat != kTextureBC13)
					{
						flags |= kMipmapNormalize;
						if (textureHeader[textureIndex].alphaSemantic == kTextureSemanticParallax) flags |= kMipmapDampenAlpha;
						else if (textureImportFlags & kTextureImportBleedAlphaTest) flags |= kMipmapBoostAlpha;
					}
				}
				else
				{
					if (textureImportFlags & kTextureImportBleedAlphaTest) flags |= kMipmapBoostAlpha;
				}
				
				Image::GenerateMipmaps2D(componentCount * depth, width, height, reinterpret_cast<Color4C *>(chainStorage), flags);
			}
			else if (pixelSize == 2)
			{
				Image::GenerateMipmaps2D(componentCount * depth, width, height, reinterpret_cast<Color2C *>(chainStorage));
			}
			else
			{
				Image::GenerateMipmaps2D(componentCount * depth, width, height, reinterpret_cast<Color1C *>(chainStorage));
			}
		}
		
		textureFile.SetPosition(sizeof(TextureResourceHeader) + textureIndex * sizeof(TextureHeader));
		textureFile.Write(&textureHeader[textureIndex], sizeof(TextureHeader));
		
		int32 mipmapCount = textureHeader[textureIndex].mipmapCount;
		int32 mipmapDataCount = mipmapCount * componentCount;
		
		TextureMipmapData *mipmapData = new TextureMipmapData[mipmapDataCount];
		MemoryMgr::ClearMemory(mipmapData, mipmapDataCount * sizeof(TextureMipmapData));
		
		unsigned_int32 mipmapDataOffset = sizeof(TextureResourceHeader) + textureIndex * sizeof(TextureHeader) + textureHeader[textureIndex].mipmapDataOffset;
		
		textureFile.SetPosition(mipmapDataOffset);
		textureFile.Write(mipmapData, mipmapDataCount * sizeof(TextureMipmapData));
		textureFile.SetPosition(textureImageOffset);
		
		int32 imageOffset = textureImageOffset - mipmapDataOffset;
		int32 chainSize = 0;
		
		int32 mipmapWidth = width;
		int32 mipmapHeight = height;
		
		unsigned_int32 maxCompressedSize = ((width + 3) & ~3) * ((height + 3) & ~3) * depth * pixelSize;
		unsigned_int8 *compressedStorage = new unsigned_int8[maxCompressedSize * 2];
		unsigned_int8 *blockStorage = compressedStorage + maxCompressedSize;
		const unsigned_int8 *uncompressedStorage = chainStorage;
		
		for (machine level = 0; level < mipmapCount; level++)
		{
			int32 pixelCount = mipmapWidth * mipmapHeight * depth;
			
			for (machine component = 0; component < componentCount; component++)
			{
				const void		*outputBuffer;
				
				TextureMipmapData *data = &mipmapData[level * componentCount + component];
				data->compressionType = kTextureCompressionNone;
				data->imageOffset = imageOffset;
				
				if (textureHeader[textureIndex].imageFormat == kTextureBC13)
				{
					TextureSemantic semantic = textureHeader[textureIndex].alphaSemantic;
					if (semantic == kTextureSemanticNone)
					{
						unsigned_int32 blockCount = BlockCompressImageBC1(depth, mipmapWidth, mipmapHeight, reinterpret_cast<const Color4C *>(uncompressedStorage), blockStorage);
						unsigned_int32 blockImageSize = blockCount * sizeof(BC1Block);
						
						unsigned_int32 compressedSize = Comp::CompressData(blockStorage, blockImageSize, compressedStorage);
						if (compressedSize != 0)
						{
							data->imageSize = compressedSize;
							data->compressionType = kTextureCompressionGeneral;
							outputBuffer = compressedStorage;
						}
						else
						{
							data->imageSize = blockImageSize;
							outputBuffer = blockStorage;
						}
					}
					else
					{
						const Color4C *image = reinterpret_cast<const Color4C *>(uncompressedStorage);
						unsigned_int32 blockCount = (semantic != kTextureSemanticNormal) ? BlockCompressImageBC3(depth, mipmapWidth, mipmapHeight, image, blockStorage) : BlockCompressNormalImageBC3(depth, mipmapWidth, mipmapHeight, image, blockStorage);
						unsigned_int32 blockImageSize = blockCount * sizeof(BC3Block);
						
						unsigned_int32 compressedSize = Comp::CompressData(blockStorage, blockImageSize, compressedStorage);
						if (compressedSize != 0)
						{
							data->imageSize = compressedSize;
							data->compressionType = kTextureCompressionGeneral;
							outputBuffer = compressedStorage;
						}
						else
						{
							data->imageSize = blockImageSize;
							outputBuffer = blockStorage;
						}
					}
				}
				else
				{
					unsigned_int32 compressedSize = Comp::CompressData(uncompressedStorage, pixelCount * pixelSize, compressedStorage);
					if (compressedSize != 0)
					{
						data->imageSize = compressedSize;
						data->compressionType = kTextureCompressionGeneral;
						outputBuffer = compressedStorage;
					}
					else
					{
						data->imageSize = pixelCount * pixelSize;
						outputBuffer = uncompressedStorage;
					}
				}
				
				textureFile.Write(outputBuffer, data->imageSize);
				textureImageOffset += data->imageSize;
				
				chainSize += data->imageSize;
				imageOffset += data->imageSize - sizeof(TextureMipmapData);
				uncompressedStorage += pixelCount * pixelSize;
			}
			
			if (mipmapWidth != 1) mipmapWidth >>= 1;
			if (mipmapHeight != 1) mipmapHeight >>= 1;
		}
		
		for (machine a = 0; a < mipmapDataCount; a++)
		{
			TextureMipmapData *data = &mipmapData[a];
			data->chainSize = chainSize;
			chainSize -= data->imageSize;
		}
		
		textureFile.SetPosition(mipmapDataOffset);
		textureFile.Write(mipmapData, mipmapDataCount * sizeof(TextureMipmapData));
		
		delete[] compressedStorage;
		delete[] mipmapData;
		delete[] chainStorage;
	}
	
	textureFile.Close();
	Texture::Reload(&outputTextureName[Text::GetPrefixDirectoryLength(outputTextureName)]);
	
	return (kTextureImportOkay);
}

void TextureImporter::ImportTexture(const char *name)
{
	String<kMaxCommandLength>	output;
	
	TextureImportResult result = SetTextureImage(0, name);
	if (result == kTextureImportOkay)
	{
		horizonIndex = 0;
		result = ImportTextureImage();
		
		if ((result == kTextureImportOkay) && (textureImportFlags & kTextureImportHorizonMap))
		{
			if (textureImportFlags & kTextureImportHorizonHalfScale) ScaleHalfResolution(0);
			
			int32 len = outputTextureName.Length();
			
			outputTextureName += "-h1";
			horizonIndex = 1;
			ImportTextureImage();
			
			outputTextureName[len + 2] = '2';
			horizonIndex = 2;
			ImportTextureImage();
			
			outputTextureName[len] = 0;
		}
	}
	
	const StringTable *table = TheTextureTool->GetStringTable();
	if (result == kTextureImportOkay)
	{
		output = table->GetString(StringID('IMPT', 'IMPT'));
		
		output += name;
		output += TargaResource::GetDescriptor()->GetExtension();
		Engine::Report(output);
	}
	else
	{
		if (result == kTextureImportFileNotFound) output = table->GetString(StringID('IMPT', 'FFNF'));
		else if (result == kTextureImportBadFileFormat) output = table->GetString(StringID('IMPT', 'FBFM'));
		else if (result == kTextureImportBadTextureSize) output = table->GetString(StringID('IMPT', 'FBSZ'));
		
		output += name;
		output += TargaResource::GetDescriptor()->GetExtension();
		Engine::Report(output, kReportError);
	}
}

void TextureImporter::ProcessCommandLine(const char *text)
{
	while (*text != 0)
	{
		String<kMaxCommandLength>	param;
		
		text += Text::ReadString(text, param, kMaxCommandLength);
		text += Text::GetWhitespaceLength(text);
		
		if (param == "-o")
		{
			text += Text::ReadString(text, outputTextureName, kMaxResourceNameLength);
		}
		else if (param == "-type")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			if (param == "2D") textureHeader[0].textureType = kTexture2D;
			else if (param == "RECT") textureHeader[0].textureType = kTextureRectangle;
			else if (param == "CUBE") textureHeader[0].textureType = kTextureCube;
		}
		else if (param == "-color")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			textureHeader[0].colorSemantic = Text::StringToType(param);
		}
		else if (param == "-alpha")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			textureHeader[0].alphaSemantic = Text::StringToType(param);
		}
		else if (param == "-format")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			textureHeader[0].imageFormat = Text::StringToType(param);
		}
		else if (param == "-swrap")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			textureHeader[0].wrapMode[0] = Text::StringToType(param);
		}
		else if (param == "-twrap")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			textureHeader[0].wrapMode[1] = Text::StringToType(param);
		}
		else if (param == "-scale")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			heightScale = Text::StringToFloat(param);
		}
		else if (param == "-height")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			heightChannel = MaxZero(Min(Text::StringToInteger(param), 3));
		}
		else if (param == "-haze")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			textureImportFlags |= kTextureImportApplyHaze;
			
			hazeElevation = Min(Max(Text::StringToInteger(param), 1), 255);
			
			text += Text::GetWhitespaceLength(text);
			text += Text::ReadString(text, param, kMaxCommandLength);
			hazeColor.red = Text::StringToFloat(param);
			
			text += Text::GetWhitespaceLength(text);
			text += Text::ReadString(text, param, kMaxCommandLength);
			hazeColor.green = Text::StringToFloat(param);
			
			text += Text::GetWhitespaceLength(text);
			text += Text::ReadString(text, param, kMaxCommandLength);
			hazeColor.blue = Text::StringToFloat(param);
		}
		else if (param == "-x")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			imageCenter.x = Text::StringToFloat(param);
		}
		else if (param == "-y")
		{
			text += Text::ReadString(text, param, kMaxCommandLength);
			imageCenter.y = Text::StringToFloat(param);
		}
		else if (param == "-compress")
		{
			textureImportFlags |= kTextureImportCompressionBC13;
		}
		else if (param == "-normal")
		{
			textureImportFlags |= kTextureImportNormalMap;
		}
		else if (param == "-parallax")
		{
			textureImportFlags |= kTextureImportParallaxMap;
		}
		else if (param == "-horizon")
		{
			textureImportFlags |= kTextureImportHorizonMap;
		}
		else if (param == "-occlusion")
		{
			textureImportFlags |= kTextureImportAmbientOcclusion;
		}
		else if (param == "-half")
		{
			textureImportFlags |= kTextureImportHalfScale;
		}
		else if (param == "-halfhorizon")
		{
			textureImportFlags |= kTextureImportHorizonHalfScale;
		}
		else if (param == "-flip")
		{
			textureImportFlags |= kTextureImportFlipVertical;
		}
		else if (param == "-invgreen")
		{
			textureImportFlags |= kTextureImportInvertGreen;
		}
		else if (param == "-nomipmaps")
		{
			textureImportFlags &= ~kTextureImportMipmaps;
		}
		else if (param == "-bleed")
		{
			textureImportFlags |= kTextureImportBleedAlphaTest;
		}
		else if (param == "-vector")
		{
			textureImportFlags |= kTextureImportVectorData;
		}
		else if (param == "-nofilter")
		{
			textureHeader[0].textureFlags |= kTextureFilterInhibit;
		}
		else if (param == "-noaniso")
		{
			textureHeader[0].textureFlags |= kTextureAnisotropicFilterInhibit;
		}
		else if (param == "-highres")
		{
			textureHeader[0].textureFlags |= kTextureForceHighQuality;
		}
		
		text += Text::GetWhitespaceLength(text);
	}
}

void TextureImporter::WriteCommandLine(File& file)
{
	file << "itexture " << inputTextureName;
	if (!Text::CompareText(outputTextureName, inputTextureName)) file << " -o " << outputTextureName;
	
	TextureType type = textureHeader[0].textureType;
	if (type != kTexture2D) file << " -type " << Text::TypeToString(type);
	
	TextureSemantic semantic = textureHeader[0].alphaSemantic;
	if (semantic != kTextureSemanticNone) file << " -alpha " << Text::TypeToString(semantic);
	
	TextureFormat format = textureHeader[0].imageFormat;
	if ((format != kTextureRGBA8) && (format != kTextureBC13)) file << " -format " << Text::TypeToString(format);
	
	TextureWrap swrap = textureHeader[0].wrapMode[0];
	if (swrap != kTextureRepeat) file << " -swrap " << Text::TypeToString(swrap);
	
	TextureWrap twrap = textureHeader[0].wrapMode[1];
	if (twrap != kTextureRepeat) file << " -twrap " << Text::TypeToString(twrap);
	
	if (heightScale != kDefaultHeightScale) file << " -scale " << Text::FloatToString(heightScale);
	if (heightChannel != 0) file << " -height " << Text::IntegerToString(heightChannel);
	
	if (textureImportFlags & kTextureImportApplyHaze)
	{
		file << " -haze " << Text::IntegerToString(hazeElevation);
		file << " " << Text::FloatToString(hazeColor.red) << " " << Text::FloatToString(hazeColor.green) << " " << Text::FloatToString(hazeColor.blue);
	}
	
	if (imageCenter.x != 0.0F) file << " -x " << Text::FloatToString(imageCenter.x);
	if (imageCenter.y != 0.0F) file << " -y " << Text::FloatToString(imageCenter.y);
	
	if (textureImportFlags & kTextureImportCompressionBC13) file << " -compress";
	if (textureImportFlags & kTextureImportBleedAlphaTest) file << " -bleed";
	if (textureImportFlags & kTextureImportVectorData) file << " -vector";
	if (textureImportFlags & kTextureImportNormalMap) file << " -normal";
	if (textureImportFlags & kTextureImportParallaxMap) file << " -parallax";
	if (textureImportFlags & kTextureImportHorizonMap) file << " -horizon";
	if (textureImportFlags & kTextureImportAmbientOcclusion) file << " -occlusion";
	if (textureImportFlags & kTextureImportHalfScale) file << " -half";
	if (textureImportFlags & kTextureImportHorizonHalfScale) file << " -halfhorizon";
	if (textureImportFlags & kTextureImportFlipVertical) file << " -flip";
	if (textureImportFlags & kTextureImportInvertGreen) file << " -invgreen";
	if (!(textureImportFlags & kTextureImportMipmaps)) file << " -nomipmaps";
	if (textureHeader[0].textureFlags & kTextureFilterInhibit) file << " -nofilter";
	if (textureHeader[0].textureFlags & kTextureAnisotropicFilterInhibit) file << " -noaniso";
	if (textureHeader[0].textureFlags & kTextureForceHighQuality) file << " -highres";
	
	file << "\r\n";
}


ImportTextureWindow::ImportTextureWindow(const char *name) : Window("TextureTool/TextureImporter")
{
	resourceName = name;
	textureImporter = nullptr;
	
	const StringTable *table = TheTextureTool->GetStringTable();
	SetWindowTitle(String<127>(table->GetString(StringID('ITEX'))) + name + TargaResource::GetDescriptor()->GetExtension());
	SetStripTitle(table->GetString(StringID('ITEX', 'STRP')));
	
	windowList.Append(this);
}

ImportTextureWindow::~ImportTextureWindow()
{
	delete textureImporter;
}

void ImportTextureWindow::Open(const char *name)
{
	ImportTextureWindow *window = windowList.First();
	while (window)
	{
		if (window->resourceName == name)
		{
			TheInterfaceMgr->SetActiveWindow(window);
			return;
		}
		
		window = window->ListElement<ImportTextureWindow>::Next();
	}
	
	TheInterfaceMgr->AddWidget(new ImportTextureWindow(name));
}

void ImportTextureWindow::Preprocess(void)
{
	Window::Preprocess();
	
	importButton = static_cast<PushButtonWidget *>(FindWidget("Import"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	configurationWidget = static_cast<ConfigurationWidget *>(FindWidget("Config"));
	
	textureImporter = new TextureImporter(resourceName, kTextureImportRemember);
	configurationWidget->BuildConfiguration(textureImporter);
}

bool ImportTextureWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeReturn)
		{
			importButton->Activate();
			return (true);
		}
		else if (code == kKeyCodeEscape)
		{
			cancelButton->Activate();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void ImportTextureWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == importButton)
		{
			configurationWidget->CommitConfiguration(textureImporter);
			textureImporter->ImportTexture(resourceName);
			Close();
		}
		else if (widget == cancelButton)
		{
			Close();
		}
	}
}

// ZYURVUR
