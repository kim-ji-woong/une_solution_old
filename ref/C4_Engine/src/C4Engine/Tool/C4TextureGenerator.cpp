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


#include "C4TextureGenerator.h"
#include "C4Cameras.h"
#include "C4Skybox.h"
#include "C4World.h"


using namespace C4;


const ConstMatrix3D CubeCamera::cameraRotation[6] =
{
	{{{0.0F, 0.0F, -1.0F}, {0.0F, 1.0F, 0.0F}, {1.0F, 0.0F, 0.0F}}},
	{{{0.0F, 0.0F, 1.0F}, {0.0F, 1.0F, 0.0F}, {-1.0F, 0.0F, 0.0F}}},
	{{{1.0F, 0.0F, 0.0F}, {0.0F, 0.0F, -1.0F}, {0.0F, 1.0F, 0.0F}}},
	{{{1.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 1.0F}, {0.0F, -1.0F, 0.0F}}},
	{{{1.0F, 0.0F, 0.0F}, {0.0F, 1.0F, 0.0F}, {0.0F, 0.0F, 1.0F}}},
	{{{-1.0F, 0.0F, 0.0F}, {0.0F, 1.0F, 0.0F}, {0.0F, 0.0F, -1.0F}}}
};


List<AmbientTextureGenerator> AmbientTextureGenerator::windowList;

TextureGeneratorWindow *C4::TheTextureGeneratorWindow = nullptr;


CubeCamera::CubeCamera(bool) : FrustumCamera(kCameraCube)
{
}

CubeCamera::CubeCamera() : FrustumCamera(kCameraCube)
{
	SetNewObject(new FrustumCameraObject(1.0F, 1.0F));
	faceIndex = kFacePositiveX;
}

CubeCamera::CubeCamera(const CubeCamera& cubeCamera) : FrustumCamera(cubeCamera)
{
	faceIndex = cubeCamera.faceIndex;
}

CubeCamera::~CubeCamera()
{
}

Node *CubeCamera::Replicate(void) const
{
	return (new CubeCamera(*this));
}

void CubeCamera::CalculateWorldTransform(void)
{
	Transform4D transform = GetNodeTransform() * cameraRotation[faceIndex];
	
	Node *node = GetSuperNode();
	if (node) SetWorldTransform(node->GetWorldTransform() * transform);
	else SetWorldTransform(transform);
}


TextureGenerator::TextureGenerator(World *world, const char *name) : TextureImporter(name)
{
	currentWorld = world;
	filterSize = 0;
}

TextureGenerator::~TextureGenerator()
{
}

void TextureGenerator::GetOutputTexturePath(ResourcePath *path) const
{
	const char *name = GetTextureName();
	if (name[0] == '/') TheResourceMgr->GetGenericCatalog()->GetResourcePath(TextureResource::GetDescriptor(), &name[1], path);
	else TheResourceMgr->GetGenericCatalog()->GetResourcePath(TextureResource::GetDescriptor(), name, currentWorld->GetResourceLocation(), path);
}

void TextureGenerator::FilterCubeTexture(Color4C *image, int32 pixelCount, int32 width)
{
	struct RemapData
	{
		int8	x[4];
		int8	y[4];
	};
	
	static const int8 faceRemap[6][4] =
	{
		{4, 5, 3, 2},
		{5, 4, 3, 2},
		{1, 0, 4, 5},
		{1, 0, 5, 4},
		{1, 0, 3, 2},
		{0, 1, 3, 2}
	};
	
	static const RemapData remapData[6][4] =
	{
		{{{ 1,  0,  1,  0}, { 0,  1,  0,  0}},
		 {{ 1,  0, -1,  0}, { 0,  1,  0,  0}},
		 {{ 0, -1,  2, -1}, { 1,  0,  0,  0}},
		 {{ 0,  1,  1,  0}, {-1,  0,  1, -1}}},
		
		{{{ 1,  0,  1,  0}, { 0,  1,  0,  0}}, 
		 {{ 1,  0, -1,  0}, { 0,  1,  0,  0}},
		 {{ 0,  1, -1,  0}, {-1,  0,  1, -1}}, 
		 {{ 0, -1,  0, -1}, { 1,  0,  0,  0}}}, 
		 
		{{{ 0,  1,  0,  0}, {-1,  0,  0, -1}},
		 {{ 0, -1,  1, -1}, { 1,  0, -1,  0}}, 
		 {{ 1,  0,  0,  0}, { 0,  1, -1,  0}},
		 {{-1,  0,  1, -1}, { 0, -1,  0, -1}}},
		
		{{{ 0, -1,  1, -1}, { 1,  0,  1,  0}}, 
		 {{ 0,  1,  0,  0}, {-1,  0,  2, -1}},
		 {{-1,  0,  1, -1}, { 0, -1,  2, -1}},
		 {{ 1,  0,  0,  0}, { 0,  1,  1,  0}}},
		 
		{{{ 1,  0,  1,  0}, { 0,  1,  0,  0}},
		 {{ 1,  0, -1,  0}, { 0,  1,  0,  0}},
		 {{ 1,  0,  0,  0}, { 0,  1, -1,  0}},
		 {{ 1,  0,  0,  0}, { 0,  1,  1,  0}}},
		
		{{{ 1,  0,  1,  0}, { 0,  1,  0,  0}},
		 {{ 1,  0, -1,  0}, { 0,  1,  0,  0}},
		 {{-1,  0,  1, -1}, { 0, -1,  2, -1}},
		 {{-1,  0,  1, -1}, { 0, -1,  0, -1}}}
	};
	
	for (machine a = 0; a < 6; a++)
	{
		Color4C *destin = image + pixelCount * (11 - a);
		
		for (machine y = 0; y < width; y++)
		{
			Color4C *dst = destin + y * width;
			
			int32 jmin = y - filterSize;
			int32 jmax = y + filterSize + 1;
			
			for (machine x = 0; x < width; x++)
			{
				unsigned_int32 red = 0;
				unsigned_int32 green = 0;
				unsigned_int32 blue = 0;
				unsigned_int32 alpha = 0;
				
				int32 imin = x - filterSize;
				int32 imax = x + filterSize + 1;
				
				for (machine j = jmin; j < jmax; j++)
				{
					for (machine i = imin; i < imax; i++)
					{
						machine face = a;
						machine ip = i;
						machine jp = j;
						
						for (machine r = 0; r < 2; r++)
						{
							if (ip < 0)
							{
								const RemapData *data = &remapData[face][0];
								face = faceRemap[face][0];
								machine t = data->x[0] * ip + data->x[1] * jp + data->x[2] * width + data->x[3];
								jp = data->y[0] * ip + data->y[1] * jp + data->y[2] * width + data->y[3];
								ip = t;
							}
							
							if (ip >= width)
							{
								const RemapData *data = &remapData[face][1];
								face = faceRemap[face][1];
								machine t = data->x[0] * ip + data->x[1] * jp + data->x[2] * width + data->x[3];
								jp = data->y[0] * ip + data->y[1] * jp + data->y[2] * width + data->y[3];
								ip = t;
							}
							
							if (jp < 0)
							{
								const RemapData *data = &remapData[face][3];
								face = faceRemap[face][3];
								machine t = data->x[0] * ip + data->x[1] * jp + data->x[2] * width + data->x[3];
								jp = data->y[0] * ip + data->y[1] * jp + data->y[2] * width + data->y[3];
								ip = t;
							}
							
							if (jp >= width)
							{
								const RemapData *data = &remapData[face][2];
								face = faceRemap[face][2];
								machine t = data->x[0] * ip + data->x[1] * jp + data->x[2] * width + data->x[3];
								jp = data->y[0] * ip + data->y[1] * jp + data->y[2] * width + data->y[3];
								ip = t;
							}
						}
						
						const Color4C *source = image + pixelCount * (5 - face);
						const Color4C& color = source[jp * width + ip];
						
						red += color.GetRed();
						green += color.GetGreen();
						blue += color.GetBlue();
						alpha += color.GetAlpha();
					}
				}
				
				int32 d = (imax - imin) * (jmax - jmin);
				red /= d;
				green /= d;
				blue /= d;
				alpha /= d;
				
				dst[x].Set(red, green, blue, alpha);
			}
		}
	}
}

void TextureGenerator::GenerateAmbientMap(Color4C *image, int32 pixelCount, int32 width)
{
	float w = 1.0F / (float) width;
	
	for (machine a = 0; a < 6; a++)
	{
		const Matrix3D& transform = CubeCamera::cameraRotation[a];
		Color4C *destin = image + pixelCount * (11 - a);
		
		for (machine y = 0; y < width; y++)
		{
			float t = ((float) y * 2.0F + 1.0F) * w - 1.0F;
			Color4C *dst = destin + y * width;
			
			for (machine x = 0; x < width; x++)
			{
				float s = ((float) x * 2.0F + 1.0F) * w - 1.0F;
				
				Vector3D direction = transform * Vector3D(s, -t, 1.0F) * InverseSqrt(s * s + t * t + 1.0F);
				ColorRGB color(0.0F, 0.0F, 0.0F);
				float sum = 0.0F;
				
				for (machine k = 0; k < 6; k++)
				{
					const Matrix3D& m = CubeCamera::cameraRotation[k];
					const Color4C *source = image + pixelCount * (5 - k);
					
					for (machine j = 0; j < width; j++)
					{
						float v = ((float) j * 2.0F + 1.0F) * w - 1.0F;
						for (machine i = 0; i < width; i++)
						{
							float u = ((float) i * 2.0F + 1.0F) * w - 1.0F;
							
							Vector3D d = m * Vector3D(u, -v, 1.0F) * InverseSqrt(u * u + v * v + 1.0F);
							float f = FmaxZero(direction * d);
							sum += f;
							
							const Color4C& c = source[j * width + i];
							color.red += (float) c.GetRed() * f;
							color.green += (float) c.GetGreen() * f;
							color.blue += (float) c.GetBlue() * f;
						}
					}
				}
				
				sum = 1.0F / sum;
				int32 red = Min((int32) (color.red * sum), 255);
				int32 green = Min((int32) (color.green * sum), 255);
				int32 blue = Min((int32) (color.blue * sum), 255);
				dst[x].Set(red, green, blue, 0);
			}
		}
	}
}

TextureImportResult TextureGenerator::GenerateCubeTexture(Node *node, int32 width, TextureFormat format)
{
	CubeCamera	cubeCamera;
	
	node->AddNewSubnode(&cubeCamera);
	currentWorld->SetCamera(&cubeCamera);
	currentWorld->SetRenderSize(width, width);
	cubeCamera.GetObject()->SetNearDepth(0.03125F);
	
	int32 pixelCount = width * width;
	Color4C *image = new Color4C[pixelCount * 12];
	
	for (machine a = 0; a < 6; a++)
	{
		cubeCamera.SetFaceIndex(a);
		
		currentWorld->Update();
		currentWorld->BeginRendering();
		currentWorld->Render();
		currentWorld->EndRendering();
		
		TheGraphicsMgr->ReadImageBuffer(Rect(0, 0, width, width), image + pixelCount * (5 - a), width, Integer2D(0, 0));
	}
	
	if (filterSize == 0)
	{
		SetTextureImage(0, width, width * 6, image);
	}
	else
	{
		FilterCubeTexture(image, pixelCount, width);
		SetTextureImage(0, width, width * 6, image + pixelCount * 6);
	}
	
	delete[] image;
	
	TextureHeader *header = GetTextureHeader();
	header->textureType = kTextureCube;
	header->imageFormat = format;
	header->wrapMode[0] = kTextureClamp;
	header->wrapMode[1] = kTextureClamp;
	header->wrapMode[2] = kTextureClamp;
	
	return (ImportTextureImage());
}

TextureImportResult TextureGenerator::GenerateSpotTexture(Node *node, float apex, float aspect, int32 width, TextureFormat format)
{
	FrustumCamera frustumCamera(apex, aspect);
	node->AddNewSubnode(&frustumCamera);
	currentWorld->SetCamera(&frustumCamera);
	currentWorld->SetRenderSize(width, width);
	
	currentWorld->Update();
	currentWorld->BeginRendering();
	currentWorld->Render();
	currentWorld->EndRendering();
	
	int32 pixelCount = width * width;
	Color4C *image = new Color4C[pixelCount * 2];
	TheGraphicsMgr->ReadImageBuffer(Rect(0, 0, width, width), image, width, Integer2D(0, 0));
	
	if (filterSize == 0)
	{
		SetTextureImage(0, width, width, image);
	}
	else
	{
		const Color4C *source = image;
		Color4C *destin = image + pixelCount;
		
		for (machine y = 0; y < width; y++)
		{
			Color4C *dst = destin + y * width;
			
			int32 jmin = MaxZero(y - filterSize);
			int32 jmax = Min(y + filterSize + 1, width);
			
			for (machine x = 0; x < width; x++)
			{
				unsigned_int32 red = 0;
				unsigned_int32 green = 0;
				unsigned_int32 blue = 0;
				unsigned_int32 alpha = 0;
				
				int32 imin = MaxZero(x - filterSize);
				int32 imax = Min(x + filterSize + 1, width);
				
				for (machine j = jmin; j < jmax; j++)
				{
					const Color4C *src = source + j * width;
					
					for (machine i = imin; i < imax; i++)
					{
						const Color4C& color = src[i];
						red += color.GetRed();
						green += color.GetGreen();
						blue += color.GetBlue();
						alpha += color.GetAlpha();
					}
				}
				
				int32 d = (imax - imin) * (jmax - jmin);
				red /= d;
				green /= d;
				blue /= d;
				alpha /= d;
				
				dst[x].Set(red, green, blue, alpha);
			}
		}
		
		SetTextureImage(0, width, width, destin);
	}
	
	delete[] image;
	
	TextureHeader *header = GetTextureHeader();
	header->imageFormat = format;
	header->wrapMode[0] = kTextureClampBorder;
	header->wrapMode[1] = kTextureClampBorder;
	header->wrapMode[2] = kTextureClampBorder;
	
	return (ImportTextureImage());
}

TextureImportResult TextureGenerator::GenerateImpostorTexture(Node *node, const ImpostorProperty *property)
{
	float	bestRadius1, bestRadius2;
	float	bestHeight1, bestHeight2;
	float	clipDistance[2][63];
	
	float impostorRadius = 0.0F;
	float impostorHeight = 0.0F;
	for (machine b = 0; b < 63; b++)
	{
		clipDistance[0][b] = 0.0F;
		clipDistance[1][b] = 0.0F;
	}
	
	const ConstVector2D *trig = Math::GetTrigTable();
	
	Node *subnode = node->GetFirstSubnode();
	while (subnode)
	{
		if (subnode->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(subnode);
			const Transform4D& transform = geometry->GetWorldTransform();
			
			geometry->SetAmbientBlendState(BlendState(kBlendOne, kBlendZero, kBlendOne, kBlendZero));
			geometry->InvalidateShaderData();
			
			const GeometryObject *object = geometry->GetObject();
			const GeometryLevel *level = object->GetGeometryLevel(0);
			
			int32 vertexCount = level->GetVertexCount();
			const Point3D *vertex = level->GetArray<Point3D>(kArrayVertex);
			
			for (machine a = 0; a < vertexCount; a++)
			{
				Point3D p = transform * vertex[a];
				float r = Magnitude(p.GetVector2D());
				
				impostorRadius = Fmax(impostorRadius, r);
				impostorHeight = Fmax(impostorHeight, p.z);
				
				for (machine i = 0; i < 63; i++)
				{
					const Vector2D& cs = trig[i + 1];
					float x = r * cs.x;
					float y = p.z * cs.y;
					
					float& d1 = clipDistance[0][i];
					float& d2 = clipDistance[1][i];
					d1 = Fmax(d1, x - y);
					d2 = Fmax(d2, x + y);
				}
			}
		}
		
		subnode = node->GetNextNode(subnode);
	}
	
	float bestArea1 = 0.0F;
	float bestArea2 = 0.0F;
	
	for (machine i = 0; i < 63; i++)
	{
		const Vector2D& cs = trig[i + 1];
		float x = 1.0F / cs.x;
		float y = 1.0F / cs.y;
		
		float d1 = clipDistance[0][i];
		float d2 = clipDistance[1][i];
		
		float r1 = d1 * x;
		float h1 = (cs.x * impostorRadius - d1) * y;
		float r2 = (d2 - cs.y * impostorHeight) * x;
		float h2 = (d2 - cs.x * impostorRadius) * y;
		
		float area1 = (impostorRadius - r1) * h1;
		if (area1 > bestArea1)
		{
			bestArea1 = area1;
			bestRadius1 = r1;
			bestHeight1 = h1;
		}
		
		float area2 = (impostorRadius - r2) * (impostorHeight - h2);
		if (area2 > bestArea2)
		{
			bestArea2 = area2;
			bestRadius2 = r2;
			bestHeight2 = h2;
		}
	}
	
	unsigned_int32 impostorFlags = property->GetImpostorFlags();
	if (impostorFlags & ImpostorProperty::kImpostorKeepBottom) bestArea1 = 0.0F;
	if (impostorFlags & ImpostorProperty::kImpostorKeepTop) bestArea2 = 0.0F;
	
	if (bestArea1 > 0.0F)
	{
		float r1 = bestRadius1 / impostorRadius;
		float h1 = bestHeight1 / impostorHeight;
		
		if (bestArea2 > 0.0F)
		{
			float r2 = bestRadius2 / impostorRadius;
			float h2 = bestHeight2 / impostorHeight;
			SetImpostorClipData(r1, r2, h1, Fmax(h1, h2));
		}
		else
		{
			SetImpostorClipData(r1, 1.0F, h1, h1);
		}
	}
	else if (bestArea2 > 0.0F)
	{
		float r2 = bestRadius2 / impostorRadius;
		float h2 = bestHeight2 / impostorHeight;
		SetImpostorClipData(1.0F, r2, h2, h2);
	}
	
	impostorHeight *= 0.5F;
	SetImpostorSize(impostorRadius, impostorHeight);
	
	int32 width = property->GetTextureWidth();
	int32 height = property->GetTextureHeight();
	
	const float d = 1024.0F;
	
	FrustumCamera frustumCamera(d / impostorRadius, 1.0F);
	currentWorld->SetCamera(&frustumCamera);
	currentWorld->SetRenderSize(width, height);
	
	FrustumCameraObject *cameraObject = frustumCamera.GetObject();
	cameraObject->SetClearFlags(kClearColorBuffer | kClearDepthBuffer | kClearStencilBuffer);
	
	int32 pixelCount = width * height * 8;
	Color4C *image = new Color4C[pixelCount];
	
	Type usage = property->GetTextureUsage();
	if (usage != ImpostorProperty::kImpostorShadowMap)
	{
		const Property *clearProperty = node->GetProperty(kPropertyClear);
		if (clearProperty) cameraObject->SetClearColor(static_cast<const ClearProperty *>(clearProperty)->GetClearColor());
		else cameraObject->SetClearColor(ColorRGBA(0.0F, 0.0F, 0.0F, 0.0F));
		
		GraphicsMgr::SetImpostorDepthParams(0.5F / impostorRadius, (impostorRadius - d) * (0.5F / impostorRadius), 0.0F);
		
		for (machine a = 0; a < 8; a++)
		{
			frustumCamera.SetNodePosition(Point3D(trig[a * 32] * d, impostorHeight));
			frustumCamera.LookAtPoint(Point3D(0.0F, 0.0F, impostorHeight));
			
			currentWorld->Update();
			cameraObject->SetAspectRatio(impostorHeight / impostorRadius);
			
			currentWorld->BeginRendering();
			currentWorld->Render();
			currentWorld->EndRendering();
			
			TheGraphicsMgr->ReadImageBuffer(Rect(0, 0, width, height), image, width * 8, Integer2D(width * a, 0));
		}
	}
	else
	{
		cameraObject->SetClearColor(ColorRGBA(1.0F, 1.0F, 1.0F, 1.0F));
		
		Color4C *depthImage = new Color4C[pixelCount];
		
		float elevation = K::pi_over_12;
		for (machine k = 0; k < 4; k++)
		{
			Vector2D v = CosSin(elevation);
			float b = (impostorHeight * v.y - d * 0.5F + Sqrt(d * d * 0.25F + impostorHeight * impostorHeight * v.y * v.y)) / v.y;
			float aspect = (b * v.x * d) / ((d + b * v.y) * impostorRadius);
			
			GraphicsMgr::SetImpostorDepthParams(0.25F * v.x / impostorRadius, 0.25F - 0.25F * d * v.x / impostorRadius, v.y / v.x);
			
			for (machine a = 0; a < 8; a++)
			{
				frustumCamera.SetNodePosition(Point3D(trig[a * 32] * (d * v.x), b + d * v.y));
				frustumCamera.LookAtPoint(Point3D(0.0F, 0.0F, b));
				
				currentWorld->Update();
				cameraObject->SetAspectRatio(aspect);
				
				currentWorld->BeginRendering();
				currentWorld->Render();
				currentWorld->EndRendering();
				
				TheGraphicsMgr->ReadImageBuffer(Rect(0, 0, width, height), depthImage, width * 8, Integer2D(width * a, 0));
			}
			
			const Color4C *src = depthImage;
			unsigned_int8 *dst = reinterpret_cast<unsigned_int8 *>(image) + k;
			for (machine a = 0; a < pixelCount; a++)
			{
				*dst = (unsigned_int8) src[a].GetAlpha();
				dst += 4;
			}
			
			elevation += K::pi_over_12;
		}
		
		delete[] depthImage;
	}
	
	SetTextureImage(0, width * 8, height, image);
	delete[] image;
	
	unsigned_int32 importFlags = GetTextureImportFlags();
	if (usage == ImpostorProperty::kImpostorColorMap) importFlags |= kTextureImportCompressionBC13 | kTextureImportBleedAlphaTest;
	else if (usage == ImpostorProperty::kImpostorNormalMap) importFlags |= kTextureImportVectorData | kTextureImportBleedAlphaTest;
	SetTextureImportFlags(importFlags);
	
	TextureHeader *header = GetTextureHeader();
	if (property->GetImpostorFlags() & ImpostorProperty::kImpostorAlphaChannel) header->alphaSemantic = kTextureSemanticTransparency;
	header->wrapMode[1] = kTextureClamp;
	
	return (ImportTextureImage());
}


void TextureTool::GenerateTextures(World *world, unsigned_int32 flags)
{
	FrustumCamera *camera = world->GetCamera();
	if (camera)
	{
		unsigned_int32 renderOptionFlags = TheGraphicsMgr->GetRenderOptionFlags();
		TheGraphicsMgr->SetRenderOptionFlags(renderOptionFlags & ~(kRenderOptionMotionBlur | kRenderOptionDistortion | kRenderOptionGlowBloom));
		
		unsigned_int32 worldFlags = world->GetWorldFlags();
		int32 worldRenderWidth = world->GetRenderWidth();
		int32 worldRenderHeight = world->GetRenderHeight();
		
		world->SetWorldFlags(worldFlags | (kWorldMotionBlurInhibit | kWorldListenerInhibit));
		
		Node *root = world->GetRootNode();
		
		if (flags & kGenerateImpostorImage)
		{
			const Property *property = root->GetFirstProperty();
			while (property)
			{
				PropertyType propertyType = property->GetPropertyType();
				if (propertyType == kPropertyImpostor)
				{
					const ImpostorProperty *impostorProperty = static_cast<const ImpostorProperty *>(property);
					const char *name = impostorProperty->GetTextureName();
					if (name[0] != 0)
					{
						TextureGenerator generator(world, name);
						generator.GenerateImpostorTexture(root, impostorProperty);
						break;
					}
				}
				
				property = property->Next();
			}
		}
		
		Node *node = root->GetFirstSubnode();
		while (node)
		{
			NodeType nodeType = node->GetNodeType();
			if (nodeType == kNodeLight)
			{
				Light *light = static_cast<Light *>(node);
				if (light->GetObject()->GetLightFlags() & kLightGenerator)
				{
					LightType lightType = light->GetLightType();
					
					if (flags & kGenerateLightProjection)
					{
						if (lightType == kLightCube)
						{
							CubeLight *cubeLight = static_cast<CubeLight *>(light);
							const CubeLightObject *object = cubeLight->GetObject();
							
							const ResourceName& name = object->GetShadowName();
							if (name[0] != 0)
							{
								TextureGenerator generator(world, name);
								generator.SetFilterSize(1);
								generator.GenerateCubeTexture(node, object->GetTextureSize(), object->GetTextureFormat());
							}
						}
						else if (lightType == kLightSpot)
						{
							SpotLight *spotLight = static_cast<SpotLight *>(light);
							const SpotLightObject *object = spotLight->GetObject();
							
							const ResourceName& name = object->GetShadowName();
							if (name[0] != 0)
							{
								TextureGenerator generator(world, name);
								generator.SetFilterSize(1);
								generator.GenerateSpotTexture(spotLight, object->GetApexTangent(), object->GetAspectRatio(), object->GetTextureSize(), object->GetTextureFormat());
							}
						}
					}
				}
			}
			else if (nodeType == kNodeMarker)
			{
				Marker *marker = static_cast<Marker *>(node);
				MarkerType markerType = marker->GetMarkerType();
				
				if (markerType == kMarkerCube)
				{
					if (flags & kGenerateEnvironmentMap)
					{
						CubeMarker *cubeMarker = static_cast<CubeMarker *>(marker);
						TextureGenerator generator(world, cubeMarker->GetTextureName());
						generator.SetFilterSize((cubeMarker->GetCubeFlags() & kCubeFilter) ? 1 : 0);
						generator.GenerateCubeTexture(cubeMarker, cubeMarker->GetCubeSize(), cubeMarker->GetTextureFormat());
					}
				}
			}
			else if (nodeType == kNodeSpace)
			{
				if (flags & kGenerateAmbientSpace)
				{
					Space *space = static_cast<Space *>(node);
					if (space->GetSpaceType() == kSpaceAmbient)
					{
						AmbientSpace *ambientSpace = static_cast<AmbientSpace *>(space);
						const AmbientSpaceObject *spaceObject = ambientSpace->GetObject();
						if (spaceObject->GetAmbientSpaceFlags() & kAmbientSpaceGenerator)
						{
							const ResourceName& name = spaceObject->GetAmbientName();
							if (name[0] != 0) TheInterfaceMgr->AddWidget(new AmbientTextureGenerator(world, ambientSpace, name));
						}
					}
				}
			}
			
			node = root->GetNextNode(node);
		}
		
		world->SetCamera(camera);
		world->SetWorldFlags(worldFlags);
		world->SetRenderSize(worldRenderWidth, worldRenderHeight);
		
		TheGraphicsMgr->SetRenderOptionFlags(renderOptionFlags);
	}
}


AmbientTextureGenerator::AmbientTextureGenerator(World *world, AmbientSpace *space, const char *name) : Window("TextureTool/AmbientSpace")
{
	textureName = name;
	generateWorld = world;
	ambientSpace = space;
	
	nearDistance = 0.015625F;
	cameraSurfaceOffset = nearDistance * 2.0F;
	
	const AmbientSpaceObject *object = space->GetObject();
	samplingRadius = object->GetSamplingRadius();
	
	space->AddNewSubnode(&cubeCamera);
	
	const int32 *textureSize = object->GetTextureSize();
	textureWidth = textureSize[0];
	textureHeight = textureSize[1];
	textureDepth = textureSize[2];
	
	const Vector3D& spaceSize = object->GetBoxSize();
	deltaX = spaceSize.x / (float) (textureWidth - 1);
	deltaY = spaceSize.y / (float) (textureHeight - 1);
	deltaZ = spaceSize.z / (float) (textureDepth - 1);
	
	int32 volume = textureWidth * textureHeight * textureDepth;
	renderData = new unsigned_int32[volume * 6];
	imageBuffer = new Color4C[kRenderArea];
	
	indexI = 0;
	indexJ = 0;
	indexK = 0;
	
	windowList.Append(this);
	if (windowList.First() != this) Hide();
}

AmbientTextureGenerator::~AmbientTextureGenerator()
{
	delete[] imageBuffer;
	delete[] renderData;
	
	AmbientTextureGenerator *window = ListElement<AmbientTextureGenerator>::Next();
	if (window) window->Show();
}

void AmbientTextureGenerator::Preprocess(void)
{
	Window::Preprocess();
	
	stopButton = static_cast<PushButtonWidget *>(FindWidget("Stop"));
	progressBar = static_cast<ProgressWidget *>(FindWidget("Progress"));
	
	nameText = static_cast<TextWidget *>(FindWidget("Name"));
	nameText->SetText(textureName);
}

void AmbientTextureGenerator::InvalidateAllShaderData(void) const
{
	Node *root = generateWorld->GetRootNode();
	Node *node = root->GetFirstSubnode();
	while (node)
	{
		NodeType type = node->GetNodeType();
		if (type == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			geometry->InvalidateShaderData();
		}
		else if (type == kNodeSkybox)
		{
			Skybox *skybox = static_cast<Skybox *>(node);
			skybox->InvalidateShaderData();
		}
		
		node = root->GetNextNode(node);
	}
}

bool AmbientTextureGenerator::DetectSolidEntrance(const Point3D& p1, const Point3D& p2, CollisionData *data) const
{
	if ((generateWorld->DetectCollision(p1, p2, 0.0F, kCollisionCamera, data)) && (!(data->geometry->GetPerspectiveExclusionMask() & kPerspectiveAmbientSpace)))
	{
		CollisionData	reverseData;
		
		if ((!generateWorld->DetectCollision(p2, p1, 0.0F, kCollisionCamera, &reverseData)) || (1.0F - reverseData.param < data->param)) return (true);
	}
	
	return (false);
}

TextureImportResult AmbientTextureGenerator::WriteTextureResource(void)
{
	File					file;
	ResourcePath			path;
	TextureResourceHeader	resourceHeader;
	TextureHeader			textureHeader[2];
	TextureMipmapData		mipmapData[2];
	
	TheResourceMgr->GetGenericCatalog()->GetResourcePath(TextureResource::GetDescriptor(), textureName, generateWorld->GetResourceLocation(), &path);
	TheResourceMgr->CreateDirectoryPath(path);
	if (file.Open(path, kFileCreate) != kFileOkay) return (kTextureImportCreateFailed);
	
	int32 volume = textureWidth * textureHeight * textureDepth;
	Color4C *image1 = new Color4C[volume * 4];
	Color4C *image2 = image1 + volume;
	
	float inverseArea = 1.0F / ((float) kRenderArea * 255.0F);
	
	const AmbientSpaceObject *object = ambientSpace->GetObject();
	float exponent = object->GetOcclusionExponent();
	float offset = object->GetMinAmbientValue();
	float scale = 1.0F - offset;
	
	unsigned_int32 *data = renderData;
	for (machine a = 0; a < volume; a++)
	{
		float xpos = (float) data[0] * inverseArea;
		float xneg = (float) data[1] * inverseArea;
		float ypos = (float) data[2] * inverseArea;
		float yneg = (float) data[3] * inverseArea;
		float zpos = (float) data[4] * inverseArea;
		float zneg = (float) data[5] * inverseArea;
		
		float red1 = (Pow(xpos, exponent) * scale + offset) * 255.0F;
		float green1 = (Pow(ypos, exponent) * scale + offset) * 255.0F;
		float blue1 = (Pow(zpos, exponent) * scale + offset) * 255.0F;
		
		float red2 = (Pow(xneg, exponent) * scale + offset) * 255.0F;
		float green2 = (Pow(yneg, exponent) * scale + offset) * 255.0F;
		float blue2 = (Pow(zneg, exponent) * scale + offset) * 255.0F;
		
		image1[a].Set((unsigned_int32) red1, (unsigned_int32) green1, (unsigned_int32) blue1, 0);
		image2[a].Set((unsigned_int32) red2, (unsigned_int32) green2, (unsigned_int32) blue2, 0);
		
		data += 6;
	}
	
	mipmapData[0].compressionType = kTextureCompressionNone;
	mipmapData[1].compressionType = kTextureCompressionNone;
	
	const void *outputBuffer1 = image1;
	const void *outputBuffer2 = image2;
	unsigned_int32 outputSize1 = volume * sizeof(Color4C);
	unsigned_int32 outputSize2 = volume * sizeof(Color4C);
	
	unsigned_int8 *compressedImage1 = reinterpret_cast<unsigned_int8 *>(image2 + volume);
	unsigned_int8 *compressedImage2 = compressedImage1 + volume * sizeof(Color4C);
	
	unsigned_int32 compressedSize1 = Comp::CompressData(image1, outputSize1, compressedImage1);
	if (compressedSize1 != 0)
	{
		outputBuffer1 = compressedImage1;
		outputSize1 = compressedSize1;
		mipmapData[0].compressionType = kTextureCompressionGeneral;
	}
	
	unsigned_int32 compressedSize2 = Comp::CompressData(image2, outputSize2, compressedImage2);
	if (compressedSize2 != 0)
	{
		outputBuffer2 = compressedImage2;
		outputSize2 = compressedSize2;
		mipmapData[1].compressionType = kTextureCompressionGeneral;
	}
	
	resourceHeader.endian = 1;
	resourceHeader.headerDataSize = sizeof(TextureHeader) * 2 + sizeof(TextureMipmapData) * 2;
	resourceHeader.textureCount = 2;
	
	textureHeader[0].textureType = kTexture3D;
	textureHeader[0].textureFlags = 0;
	textureHeader[0].colorSemantic = kTextureSemanticAmbient1;
	textureHeader[0].alphaSemantic = kTextureSemanticNone;
	textureHeader[0].imageFormat = kTextureRGBA8;
	textureHeader[0].imageWidth = textureWidth;
	textureHeader[0].imageHeight = textureHeight;
	textureHeader[0].imageDepth = textureDepth;
	textureHeader[0].wrapMode[0] = kTextureClamp;
	textureHeader[0].wrapMode[1] = kTextureClamp;
	textureHeader[0].wrapMode[2] = kTextureClamp;
	textureHeader[0].mipmapCount = 1;
	textureHeader[0].mipmapDataOffset = sizeof(TextureHeader) * 2;
	textureHeader[0].auxiliaryDataSize = 0;
	textureHeader[0].auxiliaryDataOffset = 0;
	
	textureHeader[1].textureType = kTexture3D;
	textureHeader[1].textureFlags = 0;
	textureHeader[1].colorSemantic = kTextureSemanticAmbient2;
	textureHeader[1].alphaSemantic = kTextureSemanticNone;
	textureHeader[1].imageFormat = kTextureRGBA8;
	textureHeader[1].imageWidth = textureWidth;
	textureHeader[1].imageHeight = textureHeight;
	textureHeader[1].imageDepth = textureDepth;
	textureHeader[1].wrapMode[0] = kTextureClamp;
	textureHeader[1].wrapMode[1] = kTextureClamp;
	textureHeader[1].wrapMode[2] = kTextureClamp;
	textureHeader[1].mipmapCount = 1;
	textureHeader[1].mipmapDataOffset = sizeof(TextureHeader) + sizeof(TextureMipmapData);
	textureHeader[1].auxiliaryDataSize = 0;
	textureHeader[1].auxiliaryDataOffset = 0;
	
	file.Write(&resourceHeader, sizeof(TextureResourceHeader));
	file.Write(textureHeader, sizeof(TextureHeader) * 2);
	
	mipmapData[0].imageOffset = sizeof(TextureMipmapData) * 2;
	mipmapData[0].imageSize = outputSize1;
	mipmapData[0].chainSize = outputSize1;
	
	mipmapData[1].imageOffset = sizeof(TextureMipmapData) + ((outputSize1 + 3) & ~3);
	mipmapData[1].imageSize = outputSize2;
	mipmapData[1].chainSize = outputSize2;
	
	file.Write(mipmapData, sizeof(TextureMipmapData) * 2);
	file.Write(outputBuffer1, outputSize1);
	file.WritePad(4);
	file.Write(outputBuffer2, outputSize2);
	
	delete[] image1;
	
	return (kTextureImportOkay);
}

void AmbientTextureGenerator::Move(void)
{
	CollisionData	collisionData;
	Point3D			position[6];
	
	Window::Move();
	if (!Visible()) return;
	
	unsigned_int32 worldFlags = generateWorld->GetWorldFlags();
	FrustumCamera *worldCamera = generateWorld->GetCamera();
	int32 worldRenderWidth = generateWorld->GetRenderWidth();
	int32 worldRenderHeight = generateWorld->GetRenderHeight();
	
	generateWorld->SetWorldFlags(worldFlags | (kWorldAmbientOnly | kWorldMotionBlurInhibit | kWorldListenerInhibit));
	generateWorld->SetWorldPerspective(kPerspectiveAmbientSpace);
	generateWorld->SetRenderSize(kRenderSize, kRenderSize);
	generateWorld->SetCamera(&cubeCamera);
	
	FrustumCameraObject *cameraObject = cubeCamera.GetObject();
	cameraObject->SetFrustumFlags(0);
	cameraObject->SetNearDepth(nearDistance);
	cameraObject->SetFarDepth(samplingRadius);
	cameraObject->SetClearFlags(kClearColorBuffer | kClearDepthBuffer | kClearStencilBuffer);
	cameraObject->SetClearColor(K::white);
	
	TheGraphicsMgr->SetAmbientMode(kAmbientDark);
	InvalidateAllShaderData();
	
	bool completeFlag = false;
	unsigned_int32 *data = renderData + ((indexK * textureHeight + indexJ) * textureWidth + indexI) * 6;
	
	float px = (float) indexI * deltaX;
	float py = (float) indexJ * deltaY;
	float pz = (float) indexK * deltaZ;
	
	for (machine n = 0; n < 8; n++)
	{
		Point3D center(px, py, pz);
		Point3D worldCenter = ambientSpace->GetWorldTransform() * center;
		
		int32 count = 0;
		if (indexI > 0)
		{
			Point3D p = ambientSpace->GetWorldTransform() * Point3D(center.x - samplingRadius, center.y, center.z);
			if (DetectSolidEntrance(p, worldCenter, &collisionData))
			{
				position[count++] = ambientSpace->GetInverseWorldTransform() * (collisionData.position + collisionData.normal * cameraSurfaceOffset);
			}
		}
		
		if (indexI < textureWidth - 1)
		{
			Point3D p = ambientSpace->GetWorldTransform() * Point3D(center.x + samplingRadius, center.y, center.z);
			if (DetectSolidEntrance(p, worldCenter, &collisionData))
			{
				position[count++] = ambientSpace->GetInverseWorldTransform() * (collisionData.position + collisionData.normal * cameraSurfaceOffset);
			}
		}
		
		if (indexJ > 0)
		{
			Point3D p = ambientSpace->GetWorldTransform() * Point3D(center.x, center.y - samplingRadius, center.z);
			if (DetectSolidEntrance(p, worldCenter, &collisionData))
			{
				position[count++] = ambientSpace->GetInverseWorldTransform() * (collisionData.position + collisionData.normal * cameraSurfaceOffset);
			}
		}
		
		if (indexJ < textureHeight - 1)
		{
			Point3D p = ambientSpace->GetWorldTransform() * Point3D(center.x, center.y + samplingRadius, center.z);
			if (DetectSolidEntrance(p, worldCenter, &collisionData))
			{
				position[count++] = ambientSpace->GetInverseWorldTransform() * (collisionData.position + collisionData.normal * cameraSurfaceOffset);
			}
		}
		
		if (indexK > 0)
		{
			Point3D p = ambientSpace->GetWorldTransform() * Point3D(center.x, center.y, center.z - samplingRadius);
			if (DetectSolidEntrance(p, worldCenter, &collisionData))
			{
				position[count++] = ambientSpace->GetInverseWorldTransform() * (collisionData.position + collisionData.normal * cameraSurfaceOffset);
			}
		}
		
		if (indexK < textureDepth - 1)
		{
			Point3D p = ambientSpace->GetWorldTransform() * Point3D(center.x, center.y, center.z + samplingRadius);
			if (DetectSolidEntrance(p, worldCenter, &collisionData))
			{
				position[count++] = ambientSpace->GetInverseWorldTransform() * (collisionData.position + collisionData.normal * cameraSurfaceOffset);
			}
		}
		
		if (count == 0)
		{
			cubeCamera.SetNodePosition(center);
		}
		else
		{
			const Point3D *p = &position[0];
			float d = SquaredMag(position[0] - center);
			
			for (machine a = 1; a < count; a++)
			{
				float f = SquaredMag(position[a] - center);
				if (f < d)
				{
					d = f;
					p = &position[a];
				}
			}
			
			cubeCamera.SetNodePosition(*p);
		}
		
		for (machine a = 0; a < 6; a++)
		{
			cubeCamera.SetFaceIndex(a);
			
			generateWorld->Update();
			generateWorld->BeginRendering();
			generateWorld->Render();
			generateWorld->EndRendering();
			
			TheGraphicsMgr->ReadImageBuffer(Rect(0, 0, kRenderSize, kRenderSize), imageBuffer, kRenderSize, Integer2D(0, 0));
			
			unsigned_int32 value = 0;
			for (machine y = 0; y < kRenderSize; y++)
			{
				for (machine x = 0; x < kRenderSize; x++) value += imageBuffer[y * kRenderSize + x].GetRed();
			}
			
			data[a] = value;
		}
		
		data += 6;
		
		if (++indexI == textureWidth)
		{
			indexI = 0;
			px = 0.0F;
			
			if (++indexJ == textureHeight)
			{
				indexJ = 0;
				py = 0.0F;
				
				if (++indexK == textureDepth)
				{
					WriteTextureResource();
					completeFlag = true;
					break;
				}
				
				pz = (float) indexK * deltaZ;
			}
			else
			{
				py = (float) indexJ * deltaY;
			}
		}
		else
		{
			px = (float) indexI * deltaX;
		}
	}
	
	generateWorld->SetWorldFlags(worldFlags);
	generateWorld->SetWorldPerspective(0);
	generateWorld->SetRenderSize(worldRenderWidth, worldRenderHeight);
	generateWorld->SetCamera(worldCamera);
	
	TheGraphicsMgr->SetAmbientMode(kAmbientNormal);
	InvalidateAllShaderData();
	
	if (completeFlag)
	{
		delete this;
	}
	else
	{
		int32 progress = ((indexK * textureHeight + indexJ) * textureWidth + indexI) * 200 / (textureWidth * textureHeight * textureDepth);
		progressBar->SetValue(progress);
	}
}

bool AmbientTextureGenerator::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		if (eventData->keyCode == kKeyCodeEscape)
		{
			stopButton->Activate();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void AmbientTextureGenerator::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if ((eventData->eventType == kEventWidgetActivate) && (widget == stopButton)) windowList.Purge();
}


TextureGeneratorWindow::TextureGeneratorWindow() :
		Window("TextureTool/TextureGenerator"),
		Singleton<TextureGeneratorWindow>(TheTextureGeneratorWindow)
{
}

TextureGeneratorWindow::~TextureGeneratorWindow()
{
}

void TextureGeneratorWindow::Open(void)
{
	if (TheTextureGeneratorWindow) TheInterfaceMgr->SetActiveWindow(TheTextureGeneratorWindow);
	else TheInterfaceMgr->AddWidget(new TextureGeneratorWindow);
}

void TextureGeneratorWindow::Preprocess(void)
{
	Window::Preprocess();
	
	generateButton = static_cast<PushButtonWidget *>(FindWidget("Generate"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	lightBox = static_cast<CheckWidget *>(FindWidget("Light"));
	environmentBox = static_cast<CheckWidget *>(FindWidget("Environ"));
	ambientBox = static_cast<CheckWidget *>(FindWidget("Ambient"));
	impostorBox = static_cast<CheckWidget *>(FindWidget("Impostor"));
}

bool TextureGeneratorWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	EventType eventType = eventData->eventType;
	
	if (eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeReturn)
		{
			generateButton->Activate();
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

void TextureGeneratorWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == generateButton)
		{
			World *world = TheWorldMgr->GetWorld();
			if (world)
			{
				unsigned_int32 flags = 0;
				
				if (lightBox->GetValue() != 0) flags |= kGenerateLightProjection;
				if (environmentBox->GetValue() != 0) flags |= kGenerateEnvironmentMap;
				if (ambientBox->GetValue() != 0) flags |= kGenerateAmbientSpace;
				if (impostorBox->GetValue() != 0) flags |= kGenerateImpostorImage;
				
				if (flags != 0) TextureTool::GenerateTextures(world, flags);
			}
			
			Close();
		}
		else if (widget == cancelButton)
		{
			Close();
		}
	}
}

// ZYURVUR
