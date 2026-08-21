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


#include "C4ScriptEditor.h"
#include "C4WorldEditor.h"
#include "C4Dialog.h"
#include "C4Paths.h"


using namespace C4;


namespace
{
	enum
	{
		kMaxScriptUndoCount		= 20
	};
	
	
	enum
	{
		kWidgetPartOutput		= 'OUTP',
		kWidgetPartCurve		= 'CURV'
	};
	
	
	const float kMethodBoxWidth = 120.0F;
	const float kMethodBoxHeight = 48.0F;
	const float kMethodBoxThickness = 12.0F;
	const float kMethodBoxPadding = 40.0F;
	
	const float kOutputDotRadius = 7.5F;
	
	const float kSectionTitleHeight = 18.0F;
	const float kMinSectionSize = 40.0F;
	
	
	const TextureHeader fiberTextureHeader =
	{
		kTexture2D,
		kTextureForceHighQuality,
		kTextureSemanticDiffuse,
		kTextureSemanticTransparency,
		kTextureI8,
		16, 8, 1,
		{kTextureClamp, kTextureRepeat, kTextureClamp},
		5
	};
	
	
	const unsigned_int8 fiberTextureImage[171] =
	{
		0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0xFF, 0x40, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0xFF, 0xC0, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0xFF, 0xFF, 0xFF, 0x40, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0xFF, 0xFF, 0xFF, 0xC0, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x40, 0x00,
		0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xC0, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xC0, 0x00,
		0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00,
		0x00, 0xFF, 0x00, 0x00, 0x00, 0x70, 0x10, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x10, 0xF0, 0x80, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x80, 0xFF, 0xF0, 0x10,
		0x00, 0xFF, 0x00, 0x00, 0x70, 0xFF, 0x80, 0x30,
		0xFF, 0x00, 0x5C, 0x24,
		0x00, 0x00, 0xBC, 0x6C,
		0xFF, 0x6A,
		0xFF
	};
}


const Triangle MethodWidget::methodTriangle[16] =
{
	{{ 0,  4,  1}},
	{{ 1,  4,  5}},
	{{ 1,  5,  2}},
	{{ 2,  5,  6}},
	{{ 2,  6,  3}},
	{{ 3,  6,  7}},
	{{ 3,  7,  0}},
	{{ 0,  7,  4}},
	
	{{ 8, 12,  9}},
	{{ 9, 12, 13}},
	{{ 9, 13, 10}},
	{{10, 13, 14}},
	{{10, 14, 11}},
	{{11, 14, 15}},
	{{11, 15,  8}},
	{{ 8, 15, 12}}
};


const ConstPoint2D MethodWidget::backgroundVertex[4] =
{
	{-9.0F, -5.0F},
	{-9.0F, 61.0F},
	{129.0F, -5.0F},
	{129.0F, 61.0F}
};


const ConstPoint2D MethodWidget::backgroundTexcoord[4] = 
{
	{0.0F, 66.0F}, 
	{0.0F, 0.0F}, 
	{138.0F, 66.0F}, 
	{138.0F, 0.0F}
}; 


const ConstPoint2D MethodWidget::outputVertex[4] =
{ 
	{51.0F, 40.0F},
	{51.0F, 57.0F},
	{68.0F, 40.0F},
	{68.0F, 57.0F} 
};


const ConstPoint2D MethodWidget::outputTexcoord[4] =
{
	{0.0F, 17.0F},
	{0.0F, 0.0F},
	{17.0F, 17.0F},
	{17.0F, 0.0F}
};


const Triangle ScriptSectionWidget::sectionTriangle[21] =
{
	{{ 0,  1,  2}},
	{{ 0,  2,  3}},
	{{ 4,  5,  6}},
	{{ 4,  6,  7}},
	
	{{ 8, 12,  9}},
	{{ 9, 12, 13}},
	{{ 9, 13, 10}},
	{{10, 13, 14}},
	{{10, 14, 11}},
	{{11, 14, 15}},
	{{11, 15,  8}},
	{{ 8, 15, 12}},
	
	{{16, 17, 18}},
	
	{{19, 23, 20}},
	{{20, 23, 24}},
	{{20, 24, 21}},
	{{21, 24, 25}},
	{{21, 25, 22}},
	{{22, 25, 26}},
	{{22, 26, 19}},
	{{19, 26, 23}}
};


ScriptGraph ScriptEditor::editorClipboard;


ScriptUndoData::ScriptUndoData()
{
}

ScriptUndoData::~ScriptUndoData()
{
}


CreateScriptUndoData::CreateScriptUndoData(MethodWidget *method)
{
	createdMethodList.Append(new MethodReference(method));
}

CreateScriptUndoData::CreateScriptUndoData(FiberWidget *fiber)
{
	createdFiberList.Append(new FiberReference(fiber));
}

CreateScriptUndoData::CreateScriptUndoData(ScriptSectionWidget *section)
{
	createdSectionList.Append(new ScriptSectionReference(section));
}

CreateScriptUndoData::CreateScriptUndoData(const List<MethodWidget> *methodList, const List<FiberWidget> *fiberList, const List<ScriptSectionWidget> *sectionList)
{
	MethodWidget *methodWidget = methodList->First();
	while (methodWidget)
	{
		createdMethodList.Append(new MethodReference(methodWidget));
		methodWidget = methodWidget->Next();
	}
	
	FiberWidget *fiberWidget = fiberList->First();
	while (fiberWidget)
	{
		createdFiberList.Append(new FiberReference(fiberWidget));
		fiberWidget = fiberWidget->Next();
	}
	
	ScriptSectionWidget *sectionWidget = sectionList->First();
	while (sectionWidget)
	{
		createdSectionList.Append(new ScriptSectionReference(sectionWidget));
		sectionWidget = sectionWidget->Next();
	}
}

CreateScriptUndoData::~CreateScriptUndoData()
{
}

void CreateScriptUndoData::Undo(ScriptEditor *scriptEditor)
{
	const FiberReference *fiberReference = createdFiberList.First();
	while (fiberReference)
	{
		scriptEditor->DeleteFiber(fiberReference->GetFiberWidget());
		fiberReference = fiberReference->Next();
	}
	
	const MethodReference *methodReference = createdMethodList.First();
	while (methodReference)
	{
		scriptEditor->DeleteMethod(methodReference->GetMethodWidget());
		methodReference = methodReference->Next();
	}
	
	const ScriptSectionReference *sectionReference = createdSectionList.First();
	while (sectionReference)
	{
		scriptEditor->DeleteSection(sectionReference->GetSectionElement());
		sectionReference = sectionReference->Next();
	}
}


DeleteScriptUndoData::DeleteScriptUndoData(List<MethodWidget> *methodList, List<FiberWidget> *fiberList, List<ScriptSectionWidget> *sectionList)
{
	for (;;)
	{
		MethodWidget *widget = methodList->First();
		if (!widget) break;
		
		deletedMethodList.Append(widget);
	}
	
	for (;;)
	{
		FiberWidget *widget = fiberList->First();
		if (!widget) break;
		
		deletedFiberList.Append(widget);
	}
	
	for (;;)
	{
		ScriptSectionWidget *widget = sectionList->First();
		if (!widget) break;
		
		deletedSectionList.Append(widget);
	}
}

DeleteScriptUndoData::~DeleteScriptUndoData()
{
	const FiberWidget *fiberWidget = deletedFiberList.First();
	while (fiberWidget)
	{
		delete fiberWidget->GetScriptFiber();
		fiberWidget = fiberWidget->Next();
	}
	
	const MethodWidget *methodWidget = deletedMethodList.First();
	while (methodWidget)
	{
		delete methodWidget->GetScriptMethod();
		methodWidget = methodWidget->Next();
	}
	
	const ScriptSectionWidget *sectionWidget = deletedSectionList.First();
	while (sectionWidget)
	{
		delete sectionWidget->GetSectionMethod();
		sectionWidget = sectionWidget->Next();
	}
}

void DeleteScriptUndoData::Undo(ScriptEditor *scriptEditor)
{
	for (;;)
	{
		MethodWidget *widget = deletedMethodList.First();
		if (!widget) break;
		
		scriptEditor->ReattachMethod(widget);
	}
	
	for (;;)
	{
		FiberWidget *widget = deletedFiberList.First();
		if (!widget) break;
		
		scriptEditor->ReattachFiber(widget);
	}
	
	for (;;)
	{
		ScriptSectionWidget *widget = deletedSectionList.First();
		if (!widget) break;
		
		scriptEditor->ReattachSection(widget);
	}
}


MoveScriptUndoData::MoveScriptUndoData(const List<MethodWidget> *methodList, const List<ScriptSectionWidget> *sectionList)
{
	MethodWidget *widget = methodList->First();
	while (widget)
	{
		movedMethodList.Append(new MovedMethodReference(widget));
		widget = widget->Next();
	}
	
	ScriptSectionWidget *sectionWidget = sectionList->First();
	while (sectionWidget)
	{
		movedSectionList.Append(new MovedSectionReference(sectionWidget));
		sectionWidget = sectionWidget->Next();
	}
}

MoveScriptUndoData::~MoveScriptUndoData()
{
}

MoveScriptUndoData::MovedMethodReference::MovedMethodReference(MethodWidget *widget) : MethodReference(widget)
{
	position = widget->GetScriptMethod()->GetMethodPosition();
}

MoveScriptUndoData::MovedSectionReference::MovedSectionReference(ScriptSectionWidget *widget) : ScriptSectionReference(widget)
{
	position = widget->GetSectionMethod()->GetMethodPosition();
}

void MoveScriptUndoData::Undo(ScriptEditor *scriptEditor)
{
	const MethodReference *reference = movedMethodList.First();
	while (reference)
	{
		const MovedMethodReference *moved = static_cast<const MovedMethodReference *>(reference);
		
		MethodWidget *widget = moved->GetMethodWidget();
		Method *method = widget->GetScriptMethod();
		
		const Point2D& p = moved->GetPosition();
		method->SetMethodPosition(Point2D(p.x, p.y));
		widget->SetWidgetPosition(Point3D(p.x, p.y, 0.0F));
		widget->Invalidate();
		
		scriptEditor->RebuildFiberWidgets(method);
		
		reference = reference->Next();
	}
	
	const ScriptSectionReference *sectionReference = movedSectionList.First();
	while (sectionReference)
	{
		const MovedSectionReference *moved = static_cast<const MovedSectionReference *>(sectionReference);
		
		ScriptSectionWidget *widget = moved->GetSectionElement();
		SectionMethod *section = widget->GetSectionMethod();
		
		const Point2D& p = moved->GetPosition();
		section->SetMethodPosition(Point2D(p.x, p.y));
		widget->SetWidgetPosition(Point3D(p.x, p.y, 0.0F));
		widget->Invalidate();
		
		sectionReference = sectionReference->Next();
	}
}


ResizeScriptUndoData::ResizeScriptUndoData(ScriptSectionWidget *widget)
{
	sectionWidget = widget;
	
	const SectionMethod *section = widget->GetSectionMethod();
	sectionWidth = section->GetSectionWidth();
	sectionHeight = section->GetSectionHeight();
}

ResizeScriptUndoData::~ResizeScriptUndoData()
{
}

void ResizeScriptUndoData::Undo(ScriptEditor *scriptEditor)
{
	SectionMethod *section = sectionWidget->GetSectionMethod();
	section->SetSectionSize(sectionWidth, sectionHeight);
	
	sectionWidget->SetWidgetSize(Vector2D(sectionWidth, sectionHeight));
	sectionWidget->Invalidate();
}


FiberScriptUndoData::FiberScriptUndoData(const List<FiberWidget> *selectionList)
{
	FiberWidget *widget = selectionList->First();
	while (widget)
	{
		fiberList.Append(new CycledReference(widget));
		widget = widget->Next();
	}
}

FiberScriptUndoData::~FiberScriptUndoData()
{
}

FiberScriptUndoData::CycledReference::CycledReference(FiberWidget *widget) : FiberReference(widget)
{
	flags = widget->GetScriptFiber()->GetFiberFlags();
}

void FiberScriptUndoData::Undo(ScriptEditor *scriptEditor)
{
	const FiberReference *reference = fiberList.First();
	while (reference)
	{
		const CycledReference *cycled = static_cast<const CycledReference *>(reference);
		
		FiberWidget *widget = cycled->GetFiberWidget();
		widget->GetScriptFiber()->SetFiberFlags(cycled->GetFlags());
		widget->UpdateColor();
		
		reference = reference->Next();
	}
}


MethodWidget::MethodWidget(ScriptEditor *editor, Method *method, const MethodRegistration *registration) :
		TextWidget(kWidgetMethod, nullptr, "font/Normal"),
		methodRenderable(kRenderIndexedTriangles),
		backgroundDiffuseAttribute(kAttributeMutable),
		backgroundTextureMap("ScriptEditor/Graph"),
		backgroundRenderable(kRenderTriangleStrip),
		outputDiffuseAttribute(kAttributeMutable),
		outputTextureMap("ScriptEditor/Output"),
		outputRenderable(kRenderTriangleStrip)
{
	scriptEditor = editor;
	scriptMethod = method;
	methodRegistration = registration;
	
	methodWidgetState = 0;
	viewportScale = 1.0F;
	
	SetWidgetSize(Vector2D(kMethodBoxWidth, kMethodBoxHeight));
	SetTextAlignment(kTextAlignCenter);
	SetTextFlags(kTextWrapped);
	SetTextLeading(-2.0F);
	SetRenderLineCount(4);
	
	UpdateText();
	
	InitRenderable(&methodRenderable);
	methodRenderable.SetVertexCount(16);
	methodRenderable.SetAttributeArray(kArrayVertex, methodVertex);
	methodRenderable.SetAttributeArray(kArrayColor0, methodColor);
	methodRenderable.SetTriangleArray(8, methodTriangle);
	
	InitRenderable(&backgroundRenderable);
	backgroundRenderable.SetVertexCount(4);
	backgroundRenderable.SetAttributeArray(kArrayVertex, &backgroundVertex[0]);
	backgroundRenderable.SetAttributeArray(kArrayTexture0, &backgroundTexcoord[0]);
	backgroundAttributeList.Append(&backgroundDiffuseAttribute);
	backgroundAttributeList.Append(&backgroundTextureMap);
	backgroundRenderable.SetMaterialAttributeList(&backgroundAttributeList);
	
	InitRenderable(&outputRenderable);
	outputRenderable.SetVertexCount(4);
	outputRenderable.SetAttributeArray(kArrayVertex, &outputVertex[0]);
	outputRenderable.SetAttributeArray(kArrayTexture0, &outputTexcoord[0]);
	outputAttributeList.Append(&outputDiffuseAttribute);
	outputAttributeList.Append(&outputTextureMap);
	outputRenderable.SetMaterialAttributeList(&outputAttributeList);
	
	UpdateOutputColor(false);
	UpdateColor(kMethodColorNormal);
	
	for (machine a = 0; a < 8; a++) methodColor[a].Set(0.0F, 0.0F, 0.0F, 1.0F);
	const ColorRGB& hiliteColor = TheInterfaceMgr->GetInterfaceColor(kInterfaceColorHilite).GetColorRGB();
	for (machine a = 8; a < 16; a++) methodColor[a].Set(hiliteColor, 0.625F);
}

MethodWidget::~MethodWidget()
{
}

bool MethodWidget::CalculateBoundingBox(Box2D *box) const
{
	box->min.Set(0.0F, 0.0F);
	box->max.Set(kMethodBoxWidth, kMethodBoxHeight + kOutputDotRadius);
	return (true);
}

void MethodWidget::UpdateOutputColor(bool hilite)
{
	if (hilite)
	{
		const ColorRGB& hiliteColor = TheInterfaceMgr->GetInterfaceColor(kInterfaceColorHilite).GetColorRGB();
		outputDiffuseAttribute.SetDiffuseColor(ColorRGBA(hiliteColor, 1.0F));
	}
	else
	{
		outputDiffuseAttribute.SetDiffuseColor(ColorRGBA(0.9325F, 0.9325F, 0.9325F, 1.0F));
	}
}

void MethodWidget::UpdateColor(int32 index)
{
	static const ConstColorRGBA interiorColor[kMethodColorCount] =
	{
		{1.0F, 1.0F, 1.0F, 1.0F},
		{0.625F, 1.0F, 0.625F, 1.0F},
		{1.0F, 0.625F, 0.625F, 1.0F},
		{1.0F, 1.0F, 0.25F, 1.0F}
	};
	
	backgroundDiffuseAttribute.SetDiffuseColor(interiorColor[index]);
}

void MethodWidget::UpdateText(void)
{
	String<>	text;
	
	const char *output = scriptMethod->GetOutputValueName();
	if (output[0] != 0)
	{
		text = output;
		text += " = \n";
	}
	
	bool standard = true;
	MethodType type = scriptMethod->GetMethodType();
	
	if (type == kMethodExpression)
	{
		const char *expr = static_cast<ExpressionMethod *>(scriptMethod)->GetExpressionText();
		if ((expr) && (expr[0] != 0))
		{
			text += expr;
			standard = false;
		}
	}
	else if (type == kMethodFunction)
	{
		const Function *function = static_cast<FunctionMethod *>(scriptMethod)->GetFunction();
		if (function)
		{
			const ControllerRegistration *controllerRegistration = Controller::FindRegistration(function->GetControllerType());
			const FunctionRegistration *functionRegistration = controllerRegistration->FindFunctionRegistration(function->GetFunctionType());
			text += functionRegistration->GetFunctionName();
			standard = false;
		}
	}
	
	if (standard) text += methodRegistration->GetMethodName();
	
	const ConnectorKey& key = scriptMethod->GetTargetKey();
	if (key[0] != 0)
	{
		const StringTable *table = TheWorldEditor->GetStringTable();
		
		text += "\n(";
		
		if (key[0] == '$')
		{
			text += table->GetString(StringID('SCPT', 'TARG', Text::StringToType(&key[1])));
		}
		else
		{
			text += table->GetString(StringID('SCPT', 'TARG', 'CONN'));
			text += key;
		}
		
		text += ')';
	}
	
	SetText(text);
	SplitLines();
	SetTextRenderOffset(Vector3D(0.0F, 21.0F - (float) Min(GetLineCount(), 4) * 6.0F, 0.0F));
}

void MethodWidget::Select(unsigned_int32 state)
{
	methodWidgetState |= kMethodWidgetSelected | state;
	methodRenderable.SetFaceCount(16);
}

void MethodWidget::Unselect(void)
{
	methodWidgetState &= ~(kMethodWidgetSelected | kMethodWidgetTempSelected);
	methodRenderable.SetFaceCount(8);
}

WidgetPart MethodWidget::TestPosition(const Point3D& position) const
{
	float x = position.x - kMethodBoxWidth * 0.5F;
	float y = position.y - kMethodBoxHeight;
	if (x * x + y * y < kOutputDotRadius * kOutputDotRadius) return (kWidgetPartOutput);
	
	return ((position.y < kMethodBoxHeight) ? kWidgetPartInterior : kWidgetPartNone);
}

void MethodWidget::Build(void)
{
	float thickness = (viewportScale < 4.0F) ? viewportScale * 2.0F : viewportScale;
	
	methodVertex[0].Set(0.0F, 0.0F);
	methodVertex[1].Set(0.0F, kMethodBoxHeight);
	methodVertex[2].Set(kMethodBoxWidth, kMethodBoxHeight);
	methodVertex[3].Set(kMethodBoxWidth, 0.0F);
	
	methodVertex[4].Set(-thickness, -thickness);
	methodVertex[5].Set(-thickness, kMethodBoxHeight + thickness);
	methodVertex[6].Set(kMethodBoxWidth + thickness, kMethodBoxHeight + thickness);
	methodVertex[7].Set(kMethodBoxWidth + thickness, -thickness);
	
	methodVertex[8].Set(-thickness, -thickness);
	methodVertex[9].Set(-thickness, kMethodBoxHeight + thickness);
	methodVertex[10].Set(kMethodBoxWidth + thickness, kMethodBoxHeight + thickness);
	methodVertex[11].Set(kMethodBoxWidth + thickness, -thickness);
	
	thickness = Fmax(kMethodBoxThickness, viewportScale * 4.0F);
	
	methodVertex[12].Set(-thickness, -thickness);
	methodVertex[13].Set(-thickness, kMethodBoxHeight + thickness);
	methodVertex[14].Set(kMethodBoxWidth + thickness, kMethodBoxHeight + thickness);
	methodVertex[15].Set(kMethodBoxWidth + thickness, -thickness);
	
	TextWidget::Build();
}

void MethodWidget::Render(List<Renderable> *renderList)
{
	renderList->Append(&backgroundRenderable);
	renderList->Append(&methodRenderable);
	renderList->Append(&outputRenderable);
	TextWidget::Render(renderList);
}


FiberWidget::FiberWidget(ScriptEditor *editor, Fiber *fiber) :
		RenderableWidget(kWidgetFiber, kRenderTriangleStrip),
		fiberDiffuseAttribute(ColorRGBA(0.0F, 0.0F, 0.0F, 1.0F), kAttributeMutable),
		fiberTextureMapAttribute(&fiberTextureHeader, fiberTextureImage),
		selectionDiffuseAttribute(TheInterfaceMgr->GetInterfaceColor(kInterfaceColorHilite)),
		selectionTextureMapAttribute(&fiberTextureHeader, fiberTextureImage),
		selectionRenderable(kRenderTriangleStrip)
{
	scriptEditor = editor;
	scriptFiber = fiber;
	
	fiberWidgetState = 0;
	
	SetAmbientBlendState(BlendState(kBlendOne, kBlendInvSourceAlpha));
	SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard | kShaderOrthoPolyboard);
	
	fiberAttributeList.Append(&fiberDiffuseAttribute);
	fiberAttributeList.Append(&fiberTextureMapAttribute);
	SetMaterialAttributeList(&fiberAttributeList);
	
	SetVertexCount(70);
	SetAttributeArray(kArrayVertex, fiberVertex);
	SetAttributeArray(kArrayTangent, fiberTangent);
	SetAttributeArray(kArrayTexture0, fiberTexcoord);
	
	selectionRenderable.SetAmbientBlendState(kBlendInterpolate);
	selectionRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard | kShaderOrthoPolyboard);
	
	selectionAttributeList.Append(&selectionDiffuseAttribute);
	selectionAttributeList.Append(&selectionTextureMapAttribute);
	selectionRenderable.SetMaterialAttributeList(&selectionAttributeList);
	
	selectionRenderable.SetVertexCount(70);
	selectionRenderable.SetAttributeArray(kArrayVertex, fiberVertex);
	selectionRenderable.SetAttributeArray(kArrayTangent, selectionTangent);
	selectionRenderable.SetAttributeArray(kArrayTexture0, selectionTexcoord);
	
	for (machine a = 0; a <= 64; a += 2)
	{
		fiberTexcoord[a].Set(0.0, 0.0F);
		fiberTexcoord[a + 1].Set(0.4375F, 0.0F);
	}
	
	fiberTexcoord[66].Set(0.5, 0.9375F);
	fiberTexcoord[67].Set(0.9375F, 0.9375F);
	fiberTexcoord[68].Set(0.5, 0.0625F);
	fiberTexcoord[69].Set(0.9375F, 0.0625F);
	
	for (machine a = 0; a < 70; a += 2)
	{
		selectionTexcoord[a].Set(0.0F, 0.0F);
		selectionTexcoord[a + 1].Set(0.4375F, 0.0F);
	}
	
	UpdateColor();
	Build();
}

FiberWidget::~FiberWidget()
{
}

bool FiberWidget::CalculateBoundingBox(Box2D *box) const
{
	float xmin = fiberVertex[0].x;
	float xmax = fiberVertex[0].x;
	float ymin = fiberVertex[0].y;
	float ymax = fiberVertex[0].y;
	
	for (machine a = 2; a < 70; a += 2)
	{
		const Point2D& p = fiberVertex[a];
		xmin = Fmin(xmin, p.x);
		xmax = Fmax(xmax, p.x);
		ymin = Fmin(ymin, p.y);
		ymax = Fmax(ymax, p.y);
	}
	
	box->min.Set(xmin - 4.0F, ymin - 4.0F);
	box->max.Set(xmax + 4.0F, ymax + 4.0F);
	return (true);
}

void FiberWidget::UpdateColor(void)
{
	unsigned_int32 flags = scriptFiber->GetFiberFlags();
	float red = 0.0F;
	float green = 0.0F;
	
	if (flags & kFiberConditionTrue) green = 0.75F;
	else if (flags & kFiberConditionFalse) red = 0.75F;
	
	fiberDiffuseAttribute.SetDiffuseColor(ColorRGBA(red, green, 0.0F, 1.0F));
}

void FiberWidget::Select(void)
{
	fiberWidgetState |= kFiberWidgetSelected;
}

void FiberWidget::Unselect(void)
{
	fiberWidgetState &= ~kFiberWidgetSelected;
}

WidgetPart FiberWidget::TestPosition(const Point3D& position) const
{
	for (machine a = 0; a <= 66; a += 2)
	{
		if (a != 64)
		{
			Point3D q1 = fiberVertex[a];
			Point3D q2 = fiberVertex[a + 2];
			Vector3D dq = q2 - q1;
			
			if (Math::SquaredDistancePointToLine(position, q1, dq) < 16.0F)
			{
				float m = SquaredMag(dq);
				float d = (position - q1) * dq * InverseSqrt(m);
				if ((d >= 0.0F) && (d * d < m)) return (kWidgetPartCurve);
			}
		}
	}
	
	return (kWidgetPartNone);
}

void FiberWidget::Build(void)
{
	Invalidate();
	
	bool looping = ((scriptFiber->GetFiberFlags() & kFiberLooping) != 0);
	
	Point2D p1 = scriptFiber->GetStartElement()->GetMethodPosition() + Vector2D(kMethodBoxWidth * 0.5F, kMethodBoxHeight + 2.0F);
	Point2D p5 = scriptFiber->GetFinishElement()->GetMethodPosition() + Vector2D(kMethodBoxWidth * 0.5F, -2.0F);
	Point2D texpoint = p1;
	
	float dx = (1.0F - Exp(Fnabs(p5.x - p1.x) * 0.01F)) * (kMethodBoxWidth * 0.25F);
	float dy = (p5.y - p1.y - 8.0F) * K::one_over_3;
	
	float f = (p1.x < p5.x) ? 1.0F : -1.0F;
	dx *= f;
	
	if (dy >= 8.0F)
	{
		p5.x -= dx;
		
		Point2D p2(p1.x + dx, p1.y + dy);
		Point2D p3(p5.x - dx, p5.y - dy);
		
		Vector2D tangent = (p5 - p3).Normalize();
		Point2D p4 = p5 - tangent * 9.0F;
		
		fiberVertex[68] = p5;
		fiberVertex[69] = p5;
		fiberTangent[68].Set(tangent.x, tangent.y, 0.0F, -3.0F);
		fiberTangent[69].Set(tangent.x, tangent.y, 0.0F, 4.0F);
		
		BezierPathComponent path(p1, p2, p3, p4);
		
		float u = 0.0F;
		float texcoord = 0.0F;
		
		for (machine a = 0; a <= 64; a += 2)
		{
			Point3D p = path.BezierPathComponent::GetPosition(u);
			Vector3D t = path.BezierPathComponent::GetTangent(u);
			t.Normalize();
			
			fiberVertex[a] = p.GetPoint2D();
			fiberVertex[a + 1] = p.GetPoint2D();
			fiberTangent[a].Set(t, -3.0F);
			fiberTangent[a + 1].Set(t, 4.0F);
			
			if (looping)
			{
				texcoord += Magnitude(p.GetPoint2D() - texpoint) * 0.0625F;
				texpoint = p.GetPoint2D();
			}
			
			fiberTexcoord[a].y = texcoord;
			fiberTexcoord[a + 1].y = texcoord;
			
			u += 0.03125F;
		}
	}
	else
	{
		Point2D		p2, p3, pa, pb;
		
		if (dy < Fnabs(p1.x - p5.x) * 0.25F)
		{
			p5.x -= dx;
			f *= kMethodBoxWidth;
			
			p2.Set(p1.x - f, p1.y + kMethodBoxHeight);
			p3.Set(p5.x - f, p5.y - kMethodBoxHeight);
			
			pa.Set(p2.x - f, p1.y);
			pb.Set(p3.x - f, p5.y);
		}
		else
		{
			p5.x -= dx;
			
			p2.Set(p1.x + dx, p1.y + kMethodBoxHeight);
			p3.Set(p5.x - dx, p5.y - kMethodBoxHeight);
			
			pa.Set(p2.x + dx, p2.y);
			pb.Set(p3.x - dx, p3.y);
		}
		
		Point2D pc = (pa + pb) * 0.5F;
		
		Vector2D tangent = (p5 - p3).Normalize();
		Point2D p4 = p5 - tangent * 9.0F;
		
		fiberVertex[68] = p5;
		fiberVertex[69] = p5;
		fiberTangent[68].Set(tangent.x, tangent.y, 0.0F, -3.0F);
		fiberTangent[69].Set(tangent.x, tangent.y, 0.0F, 4.0F);
		
		BezierPathComponent path1(p1, p2, pa, pc);
		BezierPathComponent path2(pc, pb, p3, p4);
		
		float u = 0.0F;
		float texcoord = 0.0F;
		
		for (machine a = 0; a < 32; a += 2)
		{
			Point3D p = path1.BezierPathComponent::GetPosition(u);
			Vector3D t = path1.BezierPathComponent::GetTangent(u);
			t.Normalize();
			
			fiberVertex[a] = p.GetPoint2D();
			fiberVertex[a + 1] = p.GetPoint2D();
			fiberTangent[a].Set(t, -3.0F);
			fiberTangent[a + 1].Set(t, 4.0F);
			
			if (looping)
			{
				texcoord += Magnitude(p.GetPoint2D() - texpoint) * 0.0625F;
				texpoint = p.GetPoint2D();
			}
			
			fiberTexcoord[a].y = texcoord;
			fiberTexcoord[a + 1].y = texcoord;
			
			u += 0.0625F;
		}
		
		u = 0.0F;
		for (machine a = 32; a <= 64; a += 2)
		{
			Point3D p = path2.BezierPathComponent::GetPosition(u);
			Vector3D t = path2.BezierPathComponent::GetTangent(u);
			t.Normalize();
			
			fiberVertex[a] = p.GetPoint2D();
			fiberVertex[a + 1] = p.GetPoint2D();
			fiberTangent[a].Set(t, -3.0F);
			fiberTangent[a + 1].Set(t, 4.0F);
			
			if (looping)
			{
				texcoord += Magnitude(p.GetPoint2D() - texpoint) * 0.0625F;
				texpoint = p.GetPoint2D();
			}
			
			fiberTexcoord[a].y = texcoord;
			fiberTexcoord[a + 1].y = texcoord;
			
			u += 0.0625F;
		}
	}
	
	fiberVertex[66] = fiberVertex[64];
	fiberVertex[67] = fiberVertex[65];
	fiberTangent[66] = fiberTangent[64];
	fiberTangent[67] = fiberTangent[65];
	
	for (machine a = 0; a < 70; a += 2)
	{
		selectionTangent[a].Set(fiberTangent[a].GetVector3D(), -15.0F);
		selectionTangent[a + 1].Set(fiberTangent[a + 1].GetVector3D(), 16.0F);
	}
}

void FiberWidget::Render(List<Renderable> *renderList)
{
	if (fiberWidgetState & kFiberWidgetSelected) renderList->Append(&selectionRenderable);
	RenderableWidget::Render(renderList);
}


ScriptSectionWidget::ScriptSectionWidget(ScriptEditor *editor, SectionMethod *method) : 
		TextWidget(kWidgetScriptSection, method->GetSectionComment(), "font/Gui"),
		sectionRenderable(kRenderIndexedTriangles)
{
	scriptEditor = editor;
	sectionMethod = method;
	
	sectionWidgetState = 0;
	viewportScale = 1.0F;
	
	float width = Fmax(method->GetSectionWidth(), kMinSectionSize);
	float height = Fmax(method->GetSectionHeight(), kMinSectionSize);
	SetWidgetSize(Vector2D(width, height));
	
	SetTextFlags(kTextClipped);
	SetTextFormatExclusionMask(0);
	SetTextAlignment(kTextAlignCenter);
	SetTextRenderOffset(Vector3D(1.0F, 2.0F, 0.0F));
	
	sectionRenderable.SetAmbientBlendState(kBlendInterpolate);
	sectionRenderable.SetTransformable(this);
	
	sectionRenderable.SetVertexCount(27);
	sectionRenderable.SetAttributeArray(kArrayVertex, sectionVertex);
	sectionRenderable.SetAttributeArray(kArrayColor0, sectionColor);
	sectionRenderable.SetTriangleArray(13, sectionTriangle);
	
	for (machine a = 0; a < 4; a++) sectionColor[a].Set(0.75F, 0.75F, 0.75F, 1.0F);
	for (machine a = 8; a < 16; a++) sectionColor[a].Set(0.0F, 0.0F, 0.0F, 1.0F);
	for (machine a = 16; a < 19; a++) sectionColor[a].Set(0.625F, 0.625F, 0.625F, 1.0F);
	
	const ColorRGB& hiliteColor = TheInterfaceMgr->GetInterfaceColor(kInterfaceColorHilite).GetColorRGB();
	for (machine a = 19; a < 27; a++) sectionColor[a].Set(hiliteColor, 0.625F);
	
	UpdateColor();
}

ScriptSectionWidget::~ScriptSectionWidget()
{
}

void ScriptSectionWidget::UpdateContent(void)
{
	const char *comment = sectionMethod->GetSectionComment();
	if ((comment) && (comment[0] != 0)) SetText(comment);
	else SetText(nullptr);
	
	UpdateColor();
}

void ScriptSectionWidget::UpdateColor(void)
{
	const ColorRGBA& color = sectionMethod->GetSectionColor();
	for (machine a = 4; a < 8; a++) sectionColor[a] = color;
}

void ScriptSectionWidget::Select(void)
{
	sectionWidgetState |= kScriptSectionWidgetSelected;
	sectionRenderable.SetFaceCount(21);
}

void ScriptSectionWidget::Unselect(void)
{
	sectionWidgetState &= ~kScriptSectionWidgetSelected;
	sectionRenderable.SetFaceCount(13);
}

WidgetPart ScriptSectionWidget::TestPosition(const Point3D& position) const
{
	if (position.y < kSectionTitleHeight) return (kWidgetPartTitle);
	if ((position.x > GetWidgetSize().x - 8.0F) && (position.y > GetWidgetSize().y - 8.0F)) return (kWidgetPartResize);
	return (kWidgetPartNone);
}

void ScriptSectionWidget::Build(void)
{
	float width = GetWidgetSize().x;
	float height = GetWidgetSize().y;
	
	sectionVertex[0].Set(0.0F, 0.0F);
	sectionVertex[1].Set(0.0F, kSectionTitleHeight);
	sectionVertex[2].Set(width, kSectionTitleHeight);
	sectionVertex[3].Set(width, 0.0F);
	
	sectionVertex[4].Set(0.0F, kSectionTitleHeight);
	sectionVertex[5].Set(0.0F, height);
	sectionVertex[6].Set(width, height);
	sectionVertex[7].Set(width, kSectionTitleHeight);
	
	sectionVertex[8].Set(0.0F, 0.0F);
	sectionVertex[9].Set(0.0F, height);
	sectionVertex[10].Set(width, height);
	sectionVertex[11].Set(width, 0.0F);
	
	sectionVertex[12].Set(-viewportScale, -viewportScale);
	sectionVertex[13].Set(-viewportScale, height + viewportScale);
	sectionVertex[14].Set(width + viewportScale, height + viewportScale);
	sectionVertex[15].Set(width + viewportScale, -viewportScale);
	
	sectionVertex[16].Set(width - 8.0F, height);
	sectionVertex[17].Set(width, height);
	sectionVertex[18].Set(width, height - 8.0F);
	
	sectionVertex[19].Set(-viewportScale, -viewportScale);
	sectionVertex[20].Set(-viewportScale, height + viewportScale);
	sectionVertex[21].Set(width + viewportScale, height + viewportScale);
	sectionVertex[22].Set(width + viewportScale, -viewportScale);
	
	float thickness = Fmax(kMethodBoxThickness, viewportScale * 4.0F);
	
	sectionVertex[23].Set(-thickness, -thickness);
	sectionVertex[24].Set(-thickness, height + thickness);
	sectionVertex[25].Set(width + thickness, height + thickness);
	sectionVertex[26].Set(width + thickness, -thickness);
	
	TextWidget::Build();
}

void ScriptSectionWidget::Render(List<Renderable> *renderList)
{
	renderList->Append(&sectionRenderable);
	TextWidget::Render(renderList);
}


ScriptPage::ScriptPage(ScriptEditor *editor, const char *panelName) : Page(panelName)
{
	scriptEditor = editor;
}

ScriptPage::~ScriptPage()
{
}


ScriptMethodsPage::ScriptMethodsPage(ScriptEditor *editor) :
		ScriptPage(editor, "ScriptEditor/Methods"),
		multipaneWidgetObserver(this, &ScriptMethodsPage::HandleMultipaneWidgetEvent),
		listWidgetObserver(this, &ScriptMethodsPage::HandleListWidgetEvent)
{
}

ScriptMethodsPage::~ScriptMethodsPage()
{
}

ScriptMethodsPage::ToolWidget::ToolWidget(const Vector2D& size, const MethodRegistration *registration) : TextWidget(size, GetMethodName(registration), "font/Normal")
{
	methodRegistration = registration;
}

ScriptMethodsPage::ToolWidget::~ToolWidget()
{
}

String<127> ScriptMethodsPage::ToolWidget::GetMethodName(const MethodRegistration *registration)
{
	String<127> name(registration->GetMethodName());
	
	for (char *text = name;; text++)
	{
		int32 c = *text;
		if (c == 0) break;
		if (c < 32) *text = 32;
	}
	
	return (name);
}

void ScriptMethodsPage::Preprocess(void)
{
	static const char *const listIdentifier[kMethodPaneCount] =
	{
		"Basic", "Standard", "Custom"
	};
	
	ScriptPage::Preprocess();
	
	multipaneWidget = static_cast<MultipaneWidget *>(FindWidget("Pane"));
	multipaneWidget->SetObserver(&multipaneWidgetObserver);
	
	for (machine a = 0; a < kMethodPaneCount; a++)
	{
		listWidget[a] = static_cast<ListWidget *>(FindWidget(listIdentifier[a]));
		listWidget[a]->SetObserver(&listWidgetObserver);
	}
	
	Vector2D size = listWidget[0]->GetNaturalListItemSize();
	
	const MethodRegistration *registration = Method::GetFirstRegistration();
	while (registration)
	{
		if (registration->GetMethodName()[0] != 0)
		{
			ToolWidget *widget = new ToolWidget(size, registration);
			
			MethodGroup group = registration->GetMethodGroup();
			if (group == 'BASC') listWidget[0]->InsertSortedListItem(widget);
			else if (group == 'STND') listWidget[1]->InsertSortedListItem(widget);
			else listWidget[2]->InsertSortedListItem(widget);
		}
		
		registration = registration->Next();
	}
}

void ScriptMethodsPage::HandleMultipaneWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		int32 selection = static_cast<MultipaneWidget *>(widget)->GetSelection();
		
		for (machine a = 0; a < kMethodPaneCount; a++)
		{
			if (a == selection) listWidget[a]->Show();
			else listWidget[a]->Hide();
		}
		
		GetScriptEditor()->SelectDefaultTool();
	}
}

void ScriptMethodsPage::HandleListWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		ToolWidget *toolWidget = static_cast<ToolWidget *>(static_cast<ListWidget *>(widget)->GetFirstSelectedListItem());
		GetScriptEditor()->SelectMethodTool(toolWidget->methodRegistration);
	}
}

void ScriptMethodsPage::UnselectMethodTool(void)
{
	for (machine a = 0; a < kMethodPaneCount; a++) listWidget[a]->UnselectAllListItems();
}


ScriptVariablesPage::ScriptVariablesPage(ScriptEditor *editor) :
		ScriptPage(editor, "ScriptEditor/Variables"),
		listWidgetObserver(this, &ScriptVariablesPage::HandleListWidgetEvent),
		addButtonObserver(this, &ScriptVariablesPage::HandleAddButtonEvent),
		deleteButtonObserver(this, &ScriptVariablesPage::HandleDeleteButtonEvent)
{
}

ScriptVariablesPage::~ScriptVariablesPage()
{
}

ScriptVariablesPage::VariableWidget::VariableWidget(const Vector2D& size, Value *value) : TextWidget(size, value->GetValueName(), "font/Normal")
{
	variableValue = value;
}

ScriptVariablesPage::VariableWidget::~VariableWidget()
{
}

void ScriptVariablesPage::Preprocess(void)
{
	ScriptPage::Preprocess();
	
	listWidget = static_cast<ListWidget *>(FindWidget("List"));
	listWidget->SetObserver(&listWidgetObserver);
	
	addButton = static_cast<PushButtonWidget *>(FindWidget("Add"));
	deleteButton = static_cast<PushButtonWidget *>(FindWidget("Delete"));
	addButton->SetObserver(&addButtonObserver);
	deleteButton->SetObserver(&deleteButtonObserver);
	
	BuildVariableList();
}

void ScriptVariablesPage::BuildVariableList(void)
{
	listWidget->PurgeListItems();
	deleteButton->Disable();
	
	Vector2D size = listWidget->GetNaturalListItemSize();
	
	Value *value = GetScriptEditor()->GetValueMap()->First();
	while (value)
	{
		listWidget->AppendListItem(new VariableWidget(size, value));
		value = value->Next();
	}
}

void ScriptVariablesPage::HandleListWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	EventType eventType = eventData->eventType;
	
	if (eventType == kEventWidgetActivate)
	{
		ScriptEditor *editor = GetScriptEditor();
		const VariableWidget *widget = static_cast<VariableWidget *>(listWidget->GetFirstSelectedListItem());
		editor->AddSubwindow(new VariableInfoWindow(editor, widget->variableValue));
	}
	else if (eventType == kEventWidgetChange)
	{
		deleteButton->Enable();
	}
}

void ScriptVariablesPage::HandleAddButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		ScriptEditor *editor = GetScriptEditor();
		editor->AddSubwindow(new VariableInfoWindow(editor));
	}
}

void ScriptVariablesPage::HandleDeleteButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		const VariableWidget *variableWidget = static_cast<VariableWidget *>(listWidget->GetFirstSelectedListItem());
		if (variableWidget)
		{
			delete variableWidget->variableValue;
			BuildVariableList();
		}
	}
}


MethodInfoWindow::MethodInfoWindow(ScriptEditor *editor) : Window("ScriptEditor/MethodInfo")
{
	scriptEditor = editor;
	methodWidget = editor->GetFirstSelectedMethod();
	controllerTarget = editor->GetTargetNode();
	
	currentFunction = nullptr;
	functionTable = nullptr;
	
	currentSettingData = nullptr;
	settingDataTable = nullptr;
}

MethodInfoWindow::~MethodInfoWindow()
{
}

MethodInfoWindow::TargetWidget::TargetWidget(const Vector2D& size, const char *text, const char *font, const char *key) : TextWidget(size, text, font)
{
	connectorKey = key;
}

MethodInfoWindow::TargetWidget::~TargetWidget()
{
}

void MethodInfoWindow::Preprocess(void)
{
	Window::Preprocess();
	
	okayButton = static_cast<PushButtonWidget *>(FindWidget("OK"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	configurationWidget = static_cast<ConfigurationWidget *>(FindWidget("Config"));
	
	targetList = static_cast<ListWidget *>(FindWidget("Target"));
	auxiliaryList = static_cast<ListWidget *>(FindWidget("Aux"));
	clearButton = static_cast<PushButtonWidget *>(FindWidget("Clear"));
	
	expressionBox = static_cast<EditTextWidget *>(FindWidget("Expr"));
	
	outputBox = static_cast<EditTextWidget *>(FindWidget("Output"));
	outputText = static_cast<TextWidget *>(FindWidget("OutputText"));
	
	Method *method = methodWidget->GetScriptMethod();
	outputBox->SetText(method->GetOutputValueName());
	BuildTargetList(method);
	
	MethodType type = method->GetMethodType();
	if (type == kMethodFunction)
	{
		auxiliaryList->Show();
		FindWidget("Funcset")->Show();
		FindWidget("Functions")->Show();
		
		UpdateFunctionList(static_cast<FunctionMethod *>(method));
	}
	else if (type == kMethodSetting)
	{
		auxiliaryList->Show();
		clearButton->Show();
		FindWidget("Catset")->Show();
		FindWidget("Categories")->Show();
		
		UpdateCategoryList(static_cast<SettingMethod *>(method));
	}
	else
	{
		FindWidget("Methset")->Show();
		
		if (type == kMethodExpression)
		{
			expressionBox->Show();
			FindWidget("ExprText")->Show();
			
			expressionBox->SetText(static_cast<ExpressionMethod *>(method)->GetExpressionText());
			expressionBox->SetRenderLineCount(4);
		}
		else
		{
			configurationWidget->BuildConfiguration(method);
		}
	}
	
	if (Method::FindRegistration(type)->GetMethodFlags() & kMethodOutputValue)
	{
		outputBox->Show();
		outputText->Show();
	}
	
	const Setting *input = method->GetFirstInputValue();
	while (input)
	{
		const Setting *setting = configurationWidget->FindSetting(input->GetSettingIdentifier());
		if (setting) setting->GetSettingInterface()->SetValueName(input->GetSettingValueName());
		
		input = input->Next();
	}
	
	SetNextFocusWidget();
}

const Node *MethodInfoWindow::GetTargetNode(void) const
{
	const TargetWidget *widget = static_cast<TargetWidget *>(targetList->GetFirstSelectedListItem());
	if (widget)
	{
		const ConnectorKey& key = widget->connectorKey;
		if (key[0] != '$') return (controllerTarget->GetConnectedNode(key));
		if (key == kConnectorKeyController) return (controllerTarget);
	}
	
	return (nullptr);
}

void MethodInfoWindow::BuildTargetList(const Method *method)
{
	MethodType type = method->GetMethodType();
	unsigned_int32 flags = Method::FindRegistration(type)->GetMethodFlags();
	if (!(flags & kMethodNoTarget))
	{
		bool enableControllerTarget = !(flags & kMethodNoSelfTarget);
		if (!enableControllerTarget)
		{
			Controller *controller = controllerTarget->GetController();
			enableControllerTarget = ((controller) && (controller->GetControllerType() != kControllerScript));
		}
		
		const StringTable *table = TheWorldEditor->GetStringTable();
		Vector2D size = targetList->GetNaturalListItemSize();
		const char *font = targetList->GetFontName();
		
		ConnectorKey targetKey = method->GetTargetKey();
		int32 index = 0;
		
		if (enableControllerTarget)
		{
			targetList->AppendListItem(new TargetWidget(size, table->GetString(StringID('SCPT', 'TARG', 'CTRL')), font, kConnectorKeyController));
			if (targetKey == kConnectorKeyController) targetList->SelectListItem(index);
			index++;
		}
		
		if (type != kMethodFunction)
		{
			targetList->AppendListItem(new TargetWidget(size, table->GetString(StringID('SCPT', 'TARG', 'TRIG')), font, kConnectorKeyTrigger));
			if (targetKey == kConnectorKeyTrigger) targetList->SelectListItem(index);
			index++;
			
			targetList->AppendListItem(new TargetWidget(size, table->GetString(StringID('SCPT', 'TARG', 'ACTV')), font, kConnectorKeyActivator));
			if (targetKey == kConnectorKeyActivator) targetList->SelectListItem(index);
			index++;
		}
		
		const Hub *hub = controllerTarget->GetHub();
		if (hub)
		{
			const char *string = table->GetString(StringID('SCPT', 'TARG', 'CONN'));
			
			const Connector *connector = hub->GetFirstOutgoingEdge();
			while (connector)
			{
				const ConnectorKey& key = connector->GetConnectorKey();
				targetList->AppendListItem(new TargetWidget(size, String<16 + kMaxConnectorKeyLength>(string) += key, font, key));
				if (targetKey == key) targetList->SelectListItem(index);
				index++;
				
				connector = connector->GetNextOutgoingEdge();
			}
		}
	}
}

void MethodInfoWindow::UpdateFunctionList(FunctionMethod *method)
{
	auxiliaryList->PurgeListItems();
	controllerRegistration = nullptr;
	
	const Node *targetNode = GetTargetNode();
	if (targetNode)
	{
		const Controller *controller = targetNode->GetController();
		if (controller)
		{
			ControllerType controllerType = controller->GetControllerType();
			
			controllerRegistration = Controller::FindRegistration(controller->GetControllerType());
			if (controllerRegistration)
			{
				int32 count = 0;
				int32 selection = -1;
				
				const FunctionRegistration *functionRegistration = controllerRegistration->GetFirstFunctionRegistration();
				while (functionRegistration)
				{
					auxiliaryList->AppendListItem(functionRegistration->GetFunctionName());
					
					if (controllerRegistration->GetControllerType() == controllerType)
					{
						Function *function = method->GetFunction();
						if ((function) && (function->GetFunctionType() == functionRegistration->GetFunctionType())) selection = count;
					}
					
					count++;
					functionRegistration = functionRegistration->Next();
				}
				
				functionCount = count;
				if (count != 0)
				{
					functionTable = new Function *[count];
					for (machine a = 0; a < count; a++) functionTable[a] = nullptr;
					
					if (selection != -1)
					{
						auxiliaryList->SelectListItem(selection);
						SelectFunction(method, selection);
					}
				}
			}
		}
	}
}

void MethodInfoWindow::SelectFunction(FunctionMethod *method, int32 index, bool commit, bool final)
{
	if (currentFunction)
	{
		if (commit)
		{
			configurationWidget->CommitConfiguration(currentFunction);
			
			if (final)
			{
				const Setting *setting = configurationWidget->GetFirstSetting();
				while (setting)
				{
					if (setting->GetSettingValueName()[0] != 0)
					{
						Setting *clone = setting->Clone();
						if (clone) method->AddInputValue(clone);
					}
					
					setting = setting->Next();
				}
			}
		}
		
		configurationWidget->ReleaseConfiguration();
		currentFunction = nullptr;
	}
	
	if (index >= 0)
	{
		const FunctionRegistration *functionRegistration = controllerRegistration->GetFunctionRegistration(index);
		
		Function *function = functionTable[index];
		if (!function)
		{
			if (functionRegistration)
			{
				FunctionType functionType = functionRegistration->GetFunctionType();
				function = controllerRegistration->ConstructFunction(functionType);
				
				currentFunction = function;
				functionTable[index] = function;
				
				Function *methodFunction = method->GetFunction();
				if ((methodFunction) && (methodFunction->GetFunctionType() == functionType)) function = methodFunction;
				configurationWidget->BuildConfiguration(function);
			}
		}
		else
		{
			currentFunction = function;
			configurationWidget->BuildConfiguration(function);
		}
		
		if (functionRegistration->GetFunctionFlags() & kFunctionOutputValue)
		{
			outputBox->Show();
			outputText->Show();
		}
		else
		{
			outputBox->Hide();
			outputText->Hide();
		}
	}
	else
	{
		if (functionTable)
		{
			const Function *function = method->GetFunction();
			for (machine a = 0; a < functionCount; a++)
			{
				if (functionTable[a] != function) delete functionTable[a];
			}
			
			delete[] functionTable;
			functionTable = nullptr;
		}
		
		outputBox->Hide();
		outputText->Hide();
	}
}

void MethodInfoWindow::UpdateCategoryList(SettingMethod *method)
{
	auxiliaryList->PurgeListItems();
	controllerRegistration = nullptr;
	
	const Node *targetNode = GetTargetNode();
	if (targetNode)
	{
		const Object *object = targetNode->GetObject();
		if (object)
		{
			settingObject = object;
			
			int32 count = object->GetCategoryCount();
			categoryCount = count;
			
			if (count != 0)
			{
				settingDataTable = new SettingData *[count];
				
				int32 selection = -1;
				Type categoryType = method->GetSettingCategoryType();
				
				for (machine a = 0; a < count; a++)
				{
					const char	*title;
					
					settingDataTable[a] = nullptr;
					if (object->GetCategoryType(a, &title) == categoryType) selection = a;
					auxiliaryList->AppendListItem(title);
				}
				
				if (selection != -1)
				{
					auxiliaryList->SelectListItem(selection);
					SelectCategory(method, selection);
				}
			}
		}
	}
}

void MethodInfoWindow::SelectCategory(SettingMethod *method, int32 index, bool commit, bool final)
{
	if (currentSettingData)
	{
		if (commit)
		{
			currentSettingData->settingList.Purge();
			
			const Setting *setting = configurationWidget->GetFirstSetting();
			while (setting)
			{
				bool active = setting->GetSettingInterface()->ExtractCurrentSetting();
				const char *valueName = setting->GetSettingValueName();
				if (valueName[0] == 0)
				{
					if (active)
					{
						Setting *clone = setting->Clone();
						if (clone) currentSettingData->settingList.Append(clone);
					}
				}
				else
				{
					if (final)
					{
						Setting *clone = setting->Clone();
						if (clone) method->AddInputValue(clone);
					}
				}
				
				setting = setting->Next();
			}
		}
		
		configurationWidget->ReleaseConfiguration();
		currentSettingData = nullptr;
	}
	
	if (index >= 0)
	{
		SettingData *data = settingDataTable[index];
		if (!data)
		{
			const char	*title;
			
			data = new SettingData;
			settingDataTable[index] = data;
			
			Type categoryType = settingObject->GetCategoryType(index, &title);
			data->categoryType = categoryType;
			
			if (method->GetSettingCategoryType() == categoryType)
			{
				const Setting *setting = method->GetFirstSetting();
				while (setting)
				{
					Setting *clone = setting->Clone();
					if (clone) data->settingList.Append(clone);
					
					setting = setting->Next();
				}
				
				setting = method->GetFirstInputValue();
				while (setting)
				{
					Setting *clone = setting->Clone();
					if (clone) data->settingList.Append(clone);
					
					setting = setting->Next();
				}
			}
		}
		
		currentSettingData = data;
		configurationWidget->BuildCategoryConfiguration(settingObject, data->categoryType);
		
		Setting *configSetting = configurationWidget->GetFirstSetting();
		while (configSetting)
		{
			Type identifier = configSetting->GetSettingIdentifier();
			
			const Setting *setting = data->settingList.First();
			while (setting)
			{
				if (setting->GetSettingIdentifier() == identifier) break;
				setting = setting->Next();
			}
			
			if (setting)
			{
				configSetting->Copy(setting);
				configSetting->GetSettingInterface()->UpdateCurrentSetting();
			}
			else
			{
				configSetting->GetSettingInterface()->SetIndeterminantValue();
			}
			
			configSetting = configSetting->Next();
		}
	}
	else
	{
		if (settingDataTable)
		{
			for (machine a = 0; a < categoryCount; a++) delete settingDataTable[a];
			
			delete[] settingDataTable;
			settingDataTable = nullptr;
		}
	}
}

bool MethodInfoWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeReturn)
		{
			okayButton->Activate();
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

void MethodInfoWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	Method *method = methodWidget->GetScriptMethod();
	EventType eventType = eventData->eventType;
	
	if (eventType == kEventWidgetActivate)
	{
		if (widget == okayButton)
		{
			method->PurgeInputValues();
			
			const TargetWidget *widget = static_cast<TargetWidget *>(targetList->GetFirstSelectedListItem());
			method->SetTargetKey((widget) ? widget->connectorKey : "");
			
			if (outputBox->Visible()) method->SetOutputValueName(outputBox->GetText());
			else method->SetOutputValueName("");
			
			MethodType type = method->GetMethodType();
			if (type == kMethodFunction)
			{
				FunctionMethod *functionMethod = static_cast<FunctionMethod *>(method);
				functionMethod->SetFunction(currentFunction);
				SelectFunction(functionMethod, -1, true, true);
			}
			else if (type == kMethodSetting)
			{
				SettingMethod *settingMethod = static_cast<SettingMethod *>(method);
				
				SettingData *data = currentSettingData;
				if (data) settingDataTable[auxiliaryList->GetFirstSelectedIndex()] = nullptr;
				SelectCategory(settingMethod, -1, true, true);
				
				settingMethod->PurgeSettings();
				
				if (data)
				{
					settingMethod->SetSettingCategoryType(data->categoryType);
					
					for (;;)
					{
						Setting *setting = data->settingList.First();
						if (!setting) break;
						
						settingMethod->AddSetting(setting);
					}
				}
				else
				{
					settingMethod->SetSettingCategoryType(0);
				}
				
				delete data;
			}
			else if (type == kMethodExpression)
			{
				static_cast<ExpressionMethod *>(method)->SetExpressionText(expressionBox->GetText());
			}
			else
			{
				configurationWidget->CommitConfiguration(method);
				
				const Setting *setting = configurationWidget->GetFirstSetting();
				while (setting)
				{
					if (setting->GetSettingValueName()[0] != 0)
					{
						Setting *clone = setting->Clone();
						if (clone) method->AddInputValue(clone);
					}
					
					setting = setting->Next();
				}
			}
			
			methodWidget->UpdateText();
			scriptEditor->AddEditorState(kScriptEditorModified | kScriptEditorUpdateGraph);
			Close();
		}
		else if (widget == cancelButton)
		{
			MethodType type = method->GetMethodType();
			if (type == kMethodFunction)
			{
				SelectFunction(static_cast<FunctionMethod *>(method), -1, false);
			}
			else if (type == kMethodSetting)
			{
				SelectCategory(static_cast<SettingMethod *>(method), -1, false);
			}
			
			Close();
		}
		else if (widget == clearButton)
		{
			if (currentSettingData)
			{
				currentSettingData->settingList.Purge();
				
				Setting *setting = configurationWidget->GetFirstSetting();
				while (setting)
				{
					SettingInterface *settingInterface = setting->GetSettingInterface();
					settingInterface->SetIndeterminantValue();
					settingInterface->SetValueName(nullptr);
					setting = setting->Next();
				}
			}
		}
		else if (widget == targetList)
		{
			MethodType type = method->GetMethodType();
			if ((type != kMethodFunction) && (type != kMethodSetting)) okayButton->Activate();
		}
	}
	else if (eventType == kEventWidgetChange)
	{
		if (widget == targetList)
		{
			MethodType type = method->GetMethodType();
			
			if (type == kMethodFunction)
			{
				FunctionMethod *functionMethod = static_cast<FunctionMethod *>(method);
				SelectFunction(functionMethod, -1);
				UpdateFunctionList(functionMethod);
			}
			else if (type == kMethodSetting)
			{
				SettingMethod *settingMethod = static_cast<SettingMethod *>(method);
				SelectCategory(settingMethod, -1);
				UpdateCategoryList(settingMethod);
			}
		}
		else if (widget == auxiliaryList)
		{
			if (method->GetMethodType() == kMethodFunction) SelectFunction(static_cast<FunctionMethod *>(method), auxiliaryList->GetFirstSelectedIndex());
			else SelectCategory(static_cast<SettingMethod *>(method), auxiliaryList->GetFirstSelectedIndex());
		}
	}
}


ScriptSectionInfoWindow::ScriptSectionInfoWindow(ScriptEditor *editor) : Window("ScriptEditor/SectionInfo")
{
	scriptEditor = editor;
	sectionWidget = editor->GetFirstSelectedSection();
}

ScriptSectionInfoWindow::~ScriptSectionInfoWindow()
{
}

void ScriptSectionInfoWindow::Preprocess(void)
{
	Window::Preprocess();
	
	okayButton = static_cast<PushButtonWidget *>(FindWidget("OK"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	configurationWidget = static_cast<ConfigurationWidget *>(FindWidget("Config"));
	configurationWidget->BuildConfiguration(sectionWidget->GetSectionMethod());
	
	SetNextFocusWidget();
}

bool ScriptSectionInfoWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeReturn)
		{
			okayButton->Activate();
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

void ScriptSectionInfoWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == okayButton)
		{
			configurationWidget->CommitConfiguration(sectionWidget->GetSectionMethod());
			sectionWidget->UpdateContent();
			
			scriptEditor->AddEditorState(kScriptEditorModified);
			Close();
		}
		else if (widget == cancelButton)
		{
			Close();
		}
	}
}


VariableInfoWindow::VariableInfoWindow(ScriptEditor *editor, Value *value) :
		Window("ScriptEditor/VariableInfo"),
		configurationObserver(this, &VariableInfoWindow::HandleConfigurationEvent)
{
	scriptEditor = editor;
	originalValue = value;
}

VariableInfoWindow::~VariableInfoWindow()
{
}

void VariableInfoWindow::Preprocess(void)
{
	Window::Preprocess();
	
	okayButton = static_cast<PushButtonWidget *>(FindWidget("OK"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	if (originalValue)
	{
		currentValue = originalValue->Clone();
	}
	else
	{
		currentValue = new BooleanValue;
		okayButton->Disable();
	}
	
	configurationWidget = static_cast<ConfigurationWidget *>(FindWidget("Config"));
	configurationWidget->BuildConfiguration(currentValue);
	configurationWidget->SetObserver(&configurationObserver);
	
	SetNextFocusWidget();
}

void VariableInfoWindow::HandleConfigurationEvent(SettingInterface *settingInterface)
{
	Value *oldValue = currentValue;
	ValueType oldType = oldValue->GetValueType();
	configurationWidget->CommitConfiguration(oldValue);
	
	ValueType newType = oldValue->GetValueType();
	if (newType != oldType)
	{
		Value *newValue = Value::New(newType);
		newValue->SetValueName(oldValue->GetValueName());
		newValue->SetValueScope(oldValue->GetValueScope());
		
		delete oldValue;
		currentValue = newValue;
		
		configurationWidget->ReleaseConfiguration();
		configurationWidget->BuildConfiguration(newValue);
		configurationWidget->SetObserver(&configurationObserver);
	}
	
	const char *name = currentValue->GetValueName();
	unsigned_int32 c = name[0];
	
	if ((c - 65 < 26U) || (c - 97 < 26U))
	{
		if ((!Text::CompareText(name, "true")) && (!Text::CompareText(name, "false")))
		{
			const Value *value = scriptEditor->FindValue(name);
			if ((!value) || (value == originalValue))
			{
				okayButton->Enable();
				return;
			}
		}
	}
	
	okayButton->Disable();
}

bool VariableInfoWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeReturn)
		{
			okayButton->Activate();
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

void VariableInfoWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == okayButton)
		{
			delete originalValue;
			scriptEditor->AddValue(currentValue);
			Close();
		}
		else if (widget == cancelButton)
		{
			delete currentValue;
			Close();
		}
	}
}


ScriptEditor::ScriptEditor(const Node *target, ScriptObject *object) :
		Window("ScriptEditor/Window"),
		toolButtonObserver(this, &ScriptEditor::HandleToolButtonEvent),
		dragRect(ColorRGBA(0.5F, 0.5F, 0.5F, 1.0F)),
		fiberDiffuseColor(ColorRGBA(0.0F, 0.0F, 0.0F, 1.0F)),
		fiberTextureMap(&fiberTextureHeader, fiberTextureImage),
		fiberRenderable(kRenderQuads)
{
	targetNode = target;
	scriptObject = object;
	object->Retain();
	
	SetMinWindowSize(Vector2D(640.0F, 512.0F));
	SetStripIcon("ScriptEditor/window");
	
	ScriptObject::CloneScript(object->GetScriptGraph(), &scriptGraph);
	
	Value *value = scriptObject->GetFirstValue();
	while (value)
	{
		scriptValueMap.Insert(value->Clone());
		value = value->Next();
	}
	
	graphRoot = new Widget;
	sectionRoot = new Widget;
	BuildScriptGraph();
}

ScriptEditor::~ScriptEditor()
{
	TheInterfaceMgr->SetCursor(nullptr);
	
	delete sectionRoot;
	delete graphRoot;
	scriptObject->Release();
}

void ScriptEditor::SetWidgetSize(const Vector2D& size)
{
	Window::SetWidgetSize(size);
	PositionWidgets();
	
	editorState |= kScriptEditorUpdateGrid;
}

void ScriptEditor::Preprocess(void)
{
	static const char *const toolIdentifier[kScriptToolCount] =
	{
		"Move", "Scroll", "Zoom"
	};
	
	Window::Preprocess();
	
	okayButton = static_cast<PushButtonWidget *>(FindWidget("OK"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	scriptViewport = static_cast<OrthoViewportWidget *>(FindWidget("Viewport"));
	viewportBorder = static_cast<BorderWidget *>(FindWidget("Border"));
	
	scriptViewport->SetMouseEventProc(&ViewportHandleMouseEvent, this);
	scriptViewport->SetTrackTaskProc(&ViewportTrackTask, this);
	scriptViewport->SetRenderProc(&ViewportRender, this);
	
	OrthoCamera *camera = scriptViewport->GetViewportCamera();
	CameraObject *cameraObject = camera->GetObject();
	cameraObject->SetClearFlags(kClearColorBuffer);
	cameraObject->SetClearColor(K::white);
	cameraObject->SetNearDepth(-1.0F);
	cameraObject->SetFarDepth(1.0F);
	
	const Box2D *box = graphRoot->GetBoundingBox();
	if (box) camera->SetNodePosition(Point3D(Floor((box->min.x + box->max.x) * 0.5F), Floor(box->min.y + scriptViewport->GetWidgetSize().y * 0.5F - 20.0F), 0.0F));
	
	for (machine a = 0; a < kScriptToolCount; a++)
	{
		IconButtonWidget *widget = static_cast<IconButtonWidget *>(FindWidget(toolIdentifier[a]));
		widget->SetObserver(&toolButtonObserver);
		toolButton[a] = widget;
	}
	
	sectionButton = static_cast<IconButtonWidget *>(FindWidget("Section"));
	
	currentMode = kScriptEditorModeTool;
	currentTool = kScriptToolMethodMove;
	currentMethodReg = nullptr;
	toolTracking = false;
	boxSelectFlag = false;
	
	menuBar = static_cast<MenuBarWidget *>(FindWidget("Menu"));
	
	methodsPage = new ScriptMethodsPage(this);
	variablesPage = new ScriptVariablesPage(this);
	
	bookWidget = new BookWidget(Vector2D(192.0F, 0.0F));
	bookWidget->AppendPage(methodsPage);
	bookWidget->AppendPage(variablesPage);
	bookWidget->OrganizePages();
	AddNewSubnode(bookWidget);
	
	PositionWidgets();
	BuildMenus();
	
	editorState = kScriptEditorUpdateMenus | kScriptEditorUpdateGraph | kScriptEditorUpdateGrid;
	
	viewportGrid.SetGridLineSpacing(16.0F);
	viewportGrid.SetMajorLineInterval(2);
	viewportGrid.SetMinorLineColor(ColorRGB(0.96875F, 0.96875F, 0.96875F));
	viewportGrid.SetMajorLineColor(ColorRGB(0.9375F, 0.9375F, 0.9375F));
	viewportGrid.SetAxisLineColor(ColorRGB(0.9375F, 0.9375F, 0.9375F));
	
	fiberRenderable.SetVertexCount(4);
	fiberRenderable.SetAttributeArray(kArrayVertex, fiberVertex);
	fiberRenderable.SetAttributeArray(kArrayTangent, fiberTangent);
	fiberRenderable.SetAttributeArray(kArrayTexture0, fiberTexcoord);
	fiberRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard | kShaderOrthoPolyboard);
	fiberRenderable.SetAmbientBlendState(BlendState(kBlendOne, kBlendInvSourceAlpha));
	fiberAttributeList.Append(&fiberDiffuseColor);
	fiberAttributeList.Append(&fiberTextureMap);
	fiberRenderable.SetMaterialAttributeList(&fiberAttributeList);
	
	fiberTexcoord[0].Set(0.0F, 1.0F);
	fiberTexcoord[1].Set(0.4375F, 1.0F);
	fiberTexcoord[2].Set(0.4375F, 1.0F);
	fiberTexcoord[3].Set(0.0F, 1.0F);
}

void ScriptEditor::PositionWidgets(void)
{
	float width = GetWidgetSize().x;
	float height = GetWidgetSize().y;
	
	okayButton->SetWidgetPosition(Point3D(width - 72.0F, height - 27.0F, 0.0F));
	okayButton->Invalidate();
	
	cancelButton->SetWidgetPosition(Point3D(width - 144.0F, height - 27.0F, 0.0F));
	cancelButton->Invalidate();
	
	const Point3D& position = scriptViewport->GetWidgetPosition();
	Vector2D viewportSize(width - position.x - 4.0F, height - position.y - 32.0F);
	
	scriptViewport->SetWidgetSize(viewportSize);
	viewportBorder->SetWidgetSize(viewportSize);
	
	float menuBarHeight = menuBar->GetWidgetSize().y;
	menuBar->SetWidgetSize(Vector2D(width, menuBarHeight));
	
	bookWidget->SetWidgetPosition(Point3D(4.0F, menuBarHeight + 31.0F, 0.0F));
	bookWidget->SetWidgetSize(Vector2D(bookWidget->GetWidgetSize().x, height - menuBarHeight - 35.0F));
	bookWidget->Invalidate();
	
	float x = position.x;
	float y = position.y;
	float w = viewportSize.x;
	float h = viewportSize.y;
	
	SetBackgroundQuad(0, Point3D(0.0F, 0.0F, 0.0F), Vector2D(x - 1.0F, height));
	SetBackgroundQuad(1, Point3D(x + w + 1.0F, 0.0F, 0.0F), Vector2D(width - x - w - 1.0F, height));
	SetBackgroundQuad(2, Point3D(x - 1.0F, 0.0F, 0.0F), Vector2D(width - x + 1.0F, y));
	SetBackgroundQuad(3, Point3D(x - 1.0F, y + h + 1.0F, 0.0F), Vector2D(width - x + 1.0F, height - y - h - 1.0F));
}

void ScriptEditor::BuildMenus(void)
{
	const StringTable *table = TheWorldEditor->GetStringTable();
	
	// Edit Menu
	
	editMenu = new PulldownMenuWidget(table->GetString(StringID('SCPT', 'MENU', 'EDIT')));
	menuBar->AppendMenu(editMenu);
	
	MenuItemWidget *widget = new MenuItemWidget(table->GetString(StringID('SCPT', 'MENU', 'EDIT', 'UNDO')), WidgetObserver<ScriptEditor>(this, &ScriptEditor::HandleUndoMenuItem), Shortcut('Z'));
	scriptMenuItem[kScriptMenuUndo] = widget;
	widget->Disable();
	editMenu->AppendMenuItem(widget);
	
	editMenu->AppendMenuItem(new MenuItemWidget(kLineSolid));
	
	widget = new MenuItemWidget(table->GetString(StringID('SCPT', 'MENU', 'EDIT', 'CUT ')), WidgetObserver<ScriptEditor>(this, &ScriptEditor::HandleCutMenuItem), Shortcut('X'));
	scriptMenuItem[kScriptMenuCut] = widget;
	editMenu->AppendMenuItem(widget);
	
	widget = new MenuItemWidget(table->GetString(StringID('SCPT', 'MENU', 'EDIT', 'COPY')), WidgetObserver<ScriptEditor>(this, &ScriptEditor::HandleCopyMenuItem), Shortcut('C'));
	scriptMenuItem[kScriptMenuCopy] = widget;
	editMenu->AppendMenuItem(widget);
	
	widget = new MenuItemWidget(table->GetString(StringID('SCPT', 'MENU', 'EDIT', 'PAST')), WidgetObserver<ScriptEditor>(this, &ScriptEditor::HandlePasteMenuItem), Shortcut('V'));
	scriptMenuItem[kScriptMenuPaste] = widget;
	if (editorClipboard.Empty()) widget->Disable();
	editMenu->AppendMenuItem(widget);
	
	widget = new MenuItemWidget(table->GetString(StringID('SCPT', 'MENU', 'EDIT', 'CLER')), WidgetObserver<ScriptEditor>(this, &ScriptEditor::HandleClearMenuItem), Shortcut(kKeyCodeDelete, kShortcutUnmodified));
	scriptMenuItem[kScriptMenuClear] = widget;
	editMenu->AppendMenuItem(widget);
	
	editMenu->AppendMenuItem(new MenuItemWidget(kLineSolid));
	editMenu->AppendMenuItem(new MenuItemWidget(table->GetString(StringID('SCPT', 'MENU', 'EDIT', 'SALL')), WidgetObserver<ScriptEditor>(this, &ScriptEditor::HandleSelectAllMenuItem), Shortcut('A')));
	
	widget = new MenuItemWidget(table->GetString(StringID('SCPT', 'MENU', 'EDIT', 'DUPL')), WidgetObserver<ScriptEditor>(this, &ScriptEditor::HandleDuplicateMenuItem), Shortcut('D'));
	scriptMenuItem[kScriptMenuDuplicate] = widget;
	editMenu->AppendMenuItem(widget);
	
	// Script Menu
	
	scriptMenu = new PulldownMenuWidget(table->GetString(StringID('SCPT', 'MENU', 'SCPT')));
	menuBar->AppendMenu(scriptMenu);
	
	widget = new MenuItemWidget(table->GetString(StringID('SCPT', 'MENU', 'SCPT', 'INFO')), WidgetObserver<ScriptEditor>(this, &ScriptEditor::HandleGetInfoMenuItem), Shortcut('I'));
	scriptMenuItem[kScriptMenuGetInfo] = widget;
	widget->Disable();
	scriptMenu->AppendMenuItem(widget);
	
	widget = new MenuItemWidget(table->GetString(StringID('SCPT', 'MENU', 'SCPT', 'FCON')), WidgetObserver<ScriptEditor>(this, &ScriptEditor::HandleCycleFiberConditionMenuItem), Shortcut('F'));
	scriptMenuItem[kScriptMenuCycleFiberCondition] = widget;
	widget->Disable();
	scriptMenu->AppendMenuItem(widget);
}

void ScriptEditor::BuildScriptGraph(void)
{
	bool arrange = true;
	Method *method = scriptGraph.GetFirstElement();
	while (method)
	{
		MethodType type = method->GetMethodType();
		if (type != kMethodSection)
		{
			MethodWidget *widget = new MethodWidget(this, method, Method::FindRegistration(type));
			graphRoot->AddSubnode(widget);
			methodWidgetList.Append(widget);
			
			Point3D position = method->GetMethodPosition();
			if ((position.x != 0.0F) || (position.y != 0.0F)) arrange = false;
			widget->SetWidgetPosition(position);
		}
		else
		{
			SectionMethod *section = static_cast<SectionMethod *>(method);
			ScriptSectionWidget *widget = new ScriptSectionWidget(this, section);
			sectionRoot->AddSubnode(widget);
			sectionWidgetList.Append(widget);
			
			Point3D position = method->GetMethodPosition();
			widget->SetWidgetPosition(position);
			widget->SetWidgetSize(Vector2D(section->GetSectionWidth(), section->GetSectionHeight()));
		}
		
		method = method->GetNextElement();
	}
	
	if (arrange)
	{
		float x = -kMethodBoxWidth - kMethodBoxPadding + 16.0F;
		float y = 0.0F;
		
		Widget *widget = graphRoot->GetFirstSubnode();
		while (widget)
		{
			Method *method = static_cast<MethodWidget *>(widget)->GetScriptMethod();
			if (!method->GetFirstIncomingEdge())
			{
				x += kMethodBoxWidth + kMethodBoxPadding + 16.0F;
				y = 0.0F;
			}
			
			widget->SetWidgetPosition(Point3D(x, y, 0.0F));
			method->SetMethodPosition(Point2D(x, y));
			
			y += kMethodBoxHeight + kMethodBoxPadding + 16.0F;
			widget = widget->Next();
		}
	}
	
	method = scriptGraph.GetFirstElement();
	while (method)
	{
		Fiber *fiber = method->GetFirstIncomingEdge();
		while (fiber)
		{
			FiberWidget *widget = new FiberWidget(this, fiber);
			graphRoot->AddFirstSubnode(widget);
			fiberWidgetList.Append(widget);
			
			fiber = fiber->GetNextIncomingEdge();
		}
		
		method = method->GetNextElement();
	}
	
	graphRoot->Preprocess();
	sectionRoot->Preprocess();
	
	graphRoot->Update();
	sectionRoot->Update();
}

void ScriptEditor::UpdateScriptGraph(void)
{
	List<Reference<Method> >	initialList;
	
	MethodWidget *methodWidget = methodWidgetList.First();
	for (machine a = 0; a < 2; a++)
	{
		while (methodWidget)
		{
			Method *method = methodWidget->GetScriptMethod();
			
			method->SetMethodIndex(-1);
			if (!method->GetFirstIncomingEdge())
			{
				methodWidget->UpdateColor(kMethodColorInitial);
				initialList.Append(new Reference<Method>(method));
			}
			else
			{
				methodWidget->UpdateColor(kMethodColorNormal);
			}
			
			methodWidget = methodWidget->Next();
		}
		
		methodWidget = selectedMethodList.First();
	}
	
	Reference<Method> *reference = initialList.First();
	while (reference)
	{
		TraverseScriptGraph(reference->GetTarget(), 0);
		reference = reference->Next();
	}
	
	methodWidget = methodWidgetList.First();
	for (machine a = 0; a < 2; a++)
	{
		while (methodWidget)
		{
			const Method *method = methodWidget->GetScriptMethod();
			if (method->GetMethodIndex() < 0) methodWidget->UpdateColor(kMethodColorDead);
			else if (DetectMethodError(method)) methodWidget->UpdateColor(kMethodColorError);
			
			methodWidget = methodWidget->Next();
		}
		
		methodWidget = selectedMethodList.First();
	}
	
	FiberWidget *fiberWidget = fiberWidgetList.First();
	for (machine a = 0; a < 2; a++)
	{
		while (fiberWidget)
		{
			Fiber *fiber = fiberWidget->GetScriptFiber();
			
			unsigned_int32 oldFlags = fiber->GetFiberFlags();
			unsigned_int32 newFlags = oldFlags;
			
			const Method *start = fiber->GetStartElement();
			const Method *finish = fiber->GetFinishElement();
			
			int32 index = finish->GetMethodIndex();
			if ((index >= 0) && (index < start->GetMethodIndex()) && (scriptGraph.Predecessor(finish, start))) newFlags |= kFiberLooping;
			else newFlags &= ~kFiberLooping;
			
			if (newFlags != oldFlags)
			{
				fiber->SetFiberFlags(newFlags);
				fiberWidget->Rebuild();
			}
			
			fiberWidget = fiberWidget->Next();
		}
		
		fiberWidget = selectedFiberList.First();
	}
}

void ScriptEditor::TraverseScriptGraph(Method *method, int32 depth)
{
	method->SetMethodIndex(Max(method->GetMethodIndex(), depth));
	method->SetMethodState(1);
	
	const Fiber *fiber = method->GetFirstOutgoingEdge();
	while (fiber)
	{
		Method *finish = fiber->GetFinishElement();
		if (finish->GetMethodState() == 0) TraverseScriptGraph(finish, depth + 1);
		
		fiber = fiber->GetNextOutgoingEdge();
	}
	
	method->SetMethodState(0);
}

bool ScriptEditor::DetectMethodError(const Method *method)
{
	if (method->GetMethodType() == kMethodExpression)
	{
		const ExpressionMethod *expressionMethod = static_cast<const ExpressionMethod *>(method);
		if ((expressionMethod->GetExpressionText()) && (!expressionMethod->GetEvaluatorRoot())) return (true);
	}
	
	return (false);
}

void ScriptEditor::ReattachMethod(MethodWidget *widget)
{
	scriptGraph.AddElement(widget->GetScriptMethod());
	graphRoot->AddSubnode(widget);
	methodWidgetList.Append(widget);
	
	widget->SetViewportScale(scriptViewport->GetOrthoScale().x);
}

void ScriptEditor::ReattachFiber(FiberWidget *widget)
{
	widget->GetScriptFiber()->Attach();
	graphRoot->AddFirstSubnode(widget);
	fiberWidgetList.Append(widget);
}

void ScriptEditor::ReattachSection(ScriptSectionWidget *widget)
{
	scriptGraph.AddElement(widget->GetSectionMethod());
	sectionRoot->AddSubnode(widget);
	sectionWidgetList.Append(widget);
	
	widget->SetViewportScale(scriptViewport->GetOrthoScale().x);
}

void ScriptEditor::RebuildFiberWidgets(const Method *method)
{
	FiberWidget *fiberWidget = fiberWidgetList.First();
	while (fiberWidget)
	{
		const Fiber *fiber = fiberWidget->GetScriptFiber();
		if ((fiber->GetStartElement() == method) || (fiber->GetFinishElement() == method)) fiberWidget->Rebuild();
		
		fiberWidget = fiberWidget->Next();
	}
	
	fiberWidget = selectedFiberList.First();
	while (fiberWidget)
	{
		const Fiber *fiber = fiberWidget->GetScriptFiber();
		if ((fiber->GetStartElement() == method) || (fiber->GetFinishElement() == method)) fiberWidget->Rebuild();
		
		fiberWidget = fiberWidget->Next();
	}
}

void ScriptEditor::SelectDefaultTool(void)
{
	if ((currentMode != kScriptEditorModeTool) || (currentTool != kScriptToolMethodMove))
	{
		UnselectCurrentTool();
		toolButton[kScriptToolMethodMove]->SetValue(1);
		
		currentMode = kScriptEditorModeTool;
		currentTool = kScriptToolMethodMove;
	}
}

void ScriptEditor::SelectMethodTool(const MethodRegistration *registration)
{
	if ((currentMode != kScriptEditorModeMethod) || (currentMethodReg != registration))
	{
		if (currentMode != kScriptEditorModeMethod) UnselectCurrentTool();
		
		currentMode = kScriptEditorModeMethod;
		currentTool = 0;
		currentMethodReg = registration;
	}
}

void ScriptEditor::UnselectCurrentTool(void)
{
	switch (currentMode)
	{
		case kScriptEditorModeTool:
			
			toolButton[currentTool]->SetValue(0);
			break;
		
		case kScriptEditorModeMethod:
			
			methodsPage->UnselectMethodTool();
			break;
		
		case kScriptEditorModeSection:
			
			sectionButton->SetValue(0);
			break;
	}
}

void ScriptEditor::UpdateViewportScale(float scale)
{
	scale = Clamp(scale, 1.0F, 8.0F);
	
	scriptViewport->SetOrthoScale(Vector2D(scale, scale));
	editorState |= kScriptEditorUpdateGrid;
	
	MethodWidget *method = methodWidgetList.First();
	while (method)
	{
		method->SetViewportScale(scale);
		method = method->Next();
	}
	
	method = selectedMethodList.First();
	while (method)
	{
		method->SetViewportScale(scale);
		method = method->Next();
	}
	
	ScriptSectionWidget *section = sectionWidgetList.First();
	while (section)
	{
		section->SetViewportScale(scale);
		section = section->Next();
	}
	
	section = selectedSectionList.First();
	while (section)
	{
		section->SetViewportScale(scale);
		section = section->Next();
	}
}

void ScriptEditor::SelectMethod(MethodWidget *widget, unsigned_int32 state)
{
	selectedMethodList.Append(widget);
	widget->Select(state);
	
	editorState |= kScriptEditorUpdateMenus;
}

void ScriptEditor::UnselectMethod(MethodWidget *widget)
{
	methodWidgetList.Append(widget);
	widget->Unselect();
	
	editorState |= kScriptEditorUpdateMenus;
}

void ScriptEditor::SelectFiber(FiberWidget *widget)
{
	selectedFiberList.Append(widget);
	widget->Select();
	
	editorState |= kScriptEditorUpdateMenus;
}

void ScriptEditor::UnselectFiber(FiberWidget *widget)
{
	fiberWidgetList.Append(widget);
	widget->Unselect();
	
	editorState |= kScriptEditorUpdateMenus;
}

void ScriptEditor::SelectSection(ScriptSectionWidget *widget)
{
	selectedSectionList.Append(widget);
	widget->Select();
	
	editorState |= kScriptEditorUpdateMenus;
}

void ScriptEditor::UnselectSection(ScriptSectionWidget *widget)
{
	sectionWidgetList.Append(widget);
	widget->Unselect();
	
	editorState |= kScriptEditorUpdateMenus;
}

void ScriptEditor::SelectAll(void)
{
	for (;;)
	{
		FiberWidget *widget = selectedFiberList.First();
		if (!widget) break;
		
		UnselectFiber(widget);
	}
	
	for (;;)
	{
		MethodWidget *widget = methodWidgetList.First();
		if (!widget) break;
		
		SelectMethod(widget);
	}
	
	for (;;)
	{
		ScriptSectionWidget *widget = sectionWidgetList.First();
		if (!widget) break;
		
		SelectSection(widget);
	}
}

void ScriptEditor::UnselectAll(void)
{
	for (;;)
	{
		MethodWidget *widget = selectedMethodList.First();
		if (!widget) break;
		
		UnselectMethod(widget);
	}
	
	for (;;)
	{
		FiberWidget *widget = selectedFiberList.First();
		if (!widget) break;
		
		UnselectFiber(widget);
	}
	
	for (;;)
	{
		ScriptSectionWidget *widget = selectedSectionList.First();
		if (!widget) break;
		
		UnselectSection(widget);
	}
}

void ScriptEditor::UnselectAllTemp(void)
{
	MethodWidget *widget = selectedMethodList.First();
	while (widget)
	{
		MethodWidget *next = widget->Next();
		if (widget->GetMethodWidgetState() & kMethodWidgetTempSelected) UnselectMethod(widget);
		widget = next;
	}
}

void ScriptEditor::DeleteMethod(MethodWidget *methodWidget, List<MethodWidget> *deletedMethodList, List<FiberWidget> *deletedFiberList)
{
	Method *method = methodWidget->GetScriptMethod();
	
	FiberWidget *fiberWidget = fiberWidgetList.First();
	while (fiberWidget)
	{
		FiberWidget *next = fiberWidget->Next();
		
		Fiber *fiber = fiberWidget->GetScriptFiber();
		if ((fiber->GetStartElement() == method) || (fiber->GetFinishElement() == method))
		{
			if (deletedFiberList)
			{
				fiber->GraphEdge<Method, Fiber>::Detach();
				
				fiberWidget->Unselect();
				fiberWidget->Widget::Detach();
				
				deletedFiberList->Append(fiberWidget);
			}
			else
			{
				delete fiber;
				delete fiberWidget;
			}
		}
		
		fiberWidget = next;
	}
	
	if (deletedMethodList)
	{
		scriptGraph.RemoveElement(method);
		
		methodWidget->Unselect();
		methodWidget->Widget::Detach();
		
		deletedMethodList->Append(methodWidget);
	}
	else
	{
		delete method;
		delete methodWidget;
	}
	
	editorState |= kScriptEditorUpdateMenus | kScriptEditorUpdateGraph;
}

void ScriptEditor::DeleteFiber(FiberWidget *fiberWidget, List<FiberWidget> *deletedFiberList)
{
	Fiber *fiber = fiberWidget->GetScriptFiber();
	if (deletedFiberList)
	{
		fiber->GraphEdge<Method, Fiber>::Detach();
		
		fiberWidget->Unselect();
		fiberWidget->Widget::Detach();
		
		deletedFiberList->Append(fiberWidget);
	}
	else
	{
		delete fiber;
		delete fiberWidget;
	}
	
	editorState |= kScriptEditorUpdateMenus | kScriptEditorUpdateGraph;
}

void ScriptEditor::DeleteSection(ScriptSectionWidget *sectionWidget, List<ScriptSectionWidget> *deletedSectionList)
{
	if (deletedSectionList)
	{
		scriptGraph.RemoveElement(sectionWidget->GetSectionMethod());
		
		sectionWidget->Unselect();
		sectionWidget->Widget::Detach();
		
		deletedSectionList->Append(sectionWidget);
	}
	else
	{
		delete sectionWidget->GetSectionMethod();
		delete sectionWidget;
	}
	
	editorState |= kScriptEditorUpdateMenus | kScriptEditorUpdateGraph;
}

void ScriptEditor::AddUndoData(ScriptUndoData *data)
{
	if (undoList.GetElementCount() >= kMaxScriptUndoCount) delete undoList.First();
	
	undoList.Append(data);
	
	scriptMenuItem[kScriptMenuUndo]->Enable();
	editorState |= kScriptEditorModified;
}

void ScriptEditor::RemoveUndoData(ScriptUndoData *data)
{
	delete data;
	
	if (undoList.Empty()) scriptMenuItem[kScriptMenuUndo]->Disable();
	editorState |= kScriptEditorModified;
}

void ScriptEditor::HandleUndoMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	ScriptUndoData *undoData = undoList.Last();
	if (undoData)
	{
		undoData->Undo(this);
		RemoveUndoData(undoData);
		
		editorState |= kScriptEditorUpdateMenus | kScriptEditorUpdateGraph;
	}
}

void ScriptEditor::HandleCutMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	HandleCopyMenuItem(nullptr, nullptr);
	HandleClearMenuItem(nullptr, nullptr);
}

void ScriptEditor::HandleCopyMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	editorClipboard.Purge();
	
	const MethodWidget *methodWidget = methodWidgetList.First();
	while (methodWidget)
	{
		methodWidget->GetScriptMethod()->SetCloneMethod(nullptr);
		methodWidget = methodWidget->Next();
	}
	
	methodWidget = selectedMethodList.First();
	while (methodWidget)
	{
		Method *method = methodWidget->GetScriptMethod();
		Method *clone = method->Clone();
		method->SetCloneMethod(clone);
		editorClipboard.AddElement(clone);
		
		methodWidget = methodWidget->Next();
	}
	
	methodWidget = selectedMethodList.First();
	while (methodWidget)
	{
		const Method *method = methodWidget->GetScriptMethod();
		Method *finish = method->GetCloneMethod();
		
		const Fiber *fiber = method->GetFirstIncomingEdge();
		while (fiber)
		{
			Method *start = fiber->GetStartElement()->GetCloneMethod();
			if (start) new Fiber(*fiber, start, finish);
			
			fiber = fiber->GetNextIncomingEdge();
		}
		
		methodWidget = methodWidget->Next();
	}
	
	ScriptSectionWidget *sectionWidget = selectedSectionList.First();
	while (sectionWidget)
	{
		Method *clone = sectionWidget->GetSectionMethod()->Clone();
		editorClipboard.AddElement(clone);
		
		sectionWidget = sectionWidget->Next();
	}
	
	MenuItemWidget *widget = scriptMenuItem[kScriptMenuPaste];
	if (editorClipboard.Empty()) widget->Disable();
	else widget->Enable();
}

void ScriptEditor::HandlePasteMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	ScriptGraph					graph;
	List<MethodWidget>			methodList;
	List<FiberWidget>			fiberList;
	List<ScriptSectionWidget>	sectionList;
	
	UnselectAll();
	
	ScriptObject::CloneScript(&editorClipboard, &graph);
	
	float xmin = K::infinity;
	float ymin = K::infinity;
	float xmax = K::minus_infinity;
	float ymax = K::minus_infinity;
	
	Method *method = graph.GetFirstElement();
	while (method)
	{
		MethodType type = method->GetMethodType();
		if (type != kMethodSection)
		{
			const Point2D& p = method->GetMethodPosition();
			xmin = Fmin(xmin, p.x);
			xmax = Fmax(xmax, p.x + kMethodBoxWidth);
			ymin = Fmin(ymin, p.y);
			ymax = Fmax(ymax, p.y + kMethodBoxHeight);
		}
		else
		{
			const Point2D& p = method->GetMethodPosition();
			
			const SectionMethod *section = static_cast<SectionMethod *>(method);
			float width = section->GetSectionWidth();
			float height = section->GetSectionHeight();
			
			xmin = Fmin(xmin, p.x);
			xmax = Fmax(xmax, p.x + width);
			ymin = Fmin(ymin, p.y);
			ymax = Fmax(ymax, p.y + height);
		}
		
		method = method->GetNextElement();
	}
	
	const Box2D *box = graphRoot->GetBoundingBox();
	float dx = (box) ? box->max.x + (kMethodBoxPadding + 16.0F) - xmin : 0.0F;
	dx = Floor((dx + 4.0F) * 0.125F) * 8.0F;
	
	float xcen = xmax + dx + 8.0F;
	float ycen = (ymin + ymax) * 0.5F;
	ShowGraphPosition(xcen, ycen);
	
	method = graph.GetFirstElement();
	while (method)
	{
		const Point2D& p = method->GetMethodPosition();
		method->SetMethodPosition(Point2D(p.x + dx, p.y));
		method = method->GetNextElement();
	}
	
	method = graph.GetFirstElement();
	while (method)
	{
		MethodType type = method->GetMethodType();
		Point3D position = method->GetMethodPosition();
		
		if (type != kMethodSection)
		{
			MethodWidget *widget = new MethodWidget(this, method, Method::FindRegistration(type));
			widget->SetViewportScale(scriptViewport->GetOrthoScale().x);
			graphRoot->AddNewSubnode(widget);
			widget->SetWidgetPosition(position);
			methodList.Append(widget);
		}
		else
		{
			ScriptSectionWidget *widget = new ScriptSectionWidget(this, static_cast<SectionMethod *>(method));
			widget->SetViewportScale(scriptViewport->GetOrthoScale().x);
			sectionRoot->AddNewSubnode(widget);
			widget->SetWidgetPosition(position);
			sectionList.Append(widget);
		}
		
		method = method->GetNextElement();
	}
	
	method = graph.GetFirstElement();
	while (method)
	{
		Fiber *fiber = method->GetFirstIncomingEdge();
		while (fiber)
		{
			FiberWidget *widget = new FiberWidget(this, fiber);
			graphRoot->AddFirstSubnode(widget);
			fiberList.Append(widget);
			widget->Preprocess();
			
			fiber = fiber->GetNextIncomingEdge();
		}
		
		method = method->GetNextElement();
	}
	
	for (;;)
	{
		method = graph.GetFirstElement();
		if (!method) break;
		
		scriptGraph.AddElement(method);
	}
	
	AddUndoData(new CreateScriptUndoData(&methodList, &fiberList, &sectionList));
	
	for (;;)
	{
		FiberWidget *widget = fiberList.First();
		if (!widget) break;
		
		fiberWidgetList.Append(widget);
	}
	
	for (;;)
	{
		MethodWidget *widget = methodList.First();
		if (!widget) break;
		
		SelectMethod(widget);
	}
	
	for (;;)
	{
		ScriptSectionWidget *widget = sectionList.First();
		if (!widget) break;
		
		SelectSection(widget);
	}
	
	graphRoot->Update();
	sectionRoot->Update();
	editorState |= kScriptEditorUpdateGraph;
}

void ScriptEditor::HandleClearMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	List<MethodWidget>			deletedMethodList;
	List<FiberWidget>			deletedFiberList;
	List<ScriptSectionWidget>	deletedSectionList;
	
	FiberWidget *fiberWidget = selectedFiberList.First();
	while (fiberWidget)
	{
		FiberWidget *next = fiberWidget->Next();
		DeleteFiber(fiberWidget, &deletedFiberList);
		fiberWidget = next;
	}
	
	MethodWidget *methodWidget = selectedMethodList.First();
	while (methodWidget)
	{
		MethodWidget *next = methodWidget->Next();
		DeleteMethod(methodWidget, &deletedMethodList, &deletedFiberList);
		methodWidget = next;
	}
	
	ScriptSectionWidget *sectionWidget = selectedSectionList.First();
	while (sectionWidget)
	{
		ScriptSectionWidget *next = sectionWidget->Next();
		DeleteSection(sectionWidget, &deletedSectionList);
		sectionWidget = next;
	}
	
	if ((!deletedMethodList.Empty()) || (!deletedFiberList.Empty()) || (!deletedSectionList.Empty())) AddUndoData(new DeleteScriptUndoData(&deletedMethodList, &deletedFiberList, &deletedSectionList));
}

void ScriptEditor::HandleSelectAllMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	SelectAll();
}

void ScriptEditor::HandleDuplicateMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	HandleCopyMenuItem(nullptr, nullptr);
	HandlePasteMenuItem(nullptr, nullptr);
}

void ScriptEditor::HandleGetInfoMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const MethodWidget *methodWidget = selectedMethodList.First();
	if (methodWidget)
	{
		if (selectedMethodList.Last() == methodWidget) AddSubwindow(new MethodInfoWindow(this));
		return;
	}
	
	const ScriptSectionWidget *sectionWidget = selectedSectionList.First();
	if (sectionWidget)
	{
		if (selectedSectionList.Last() == sectionWidget) AddSubwindow(new ScriptSectionInfoWindow(this));
		return;
	}
}

void ScriptEditor::HandleCycleFiberConditionMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const List<FiberWidget> *fiberList = &selectedFiberList;
	AddUndoData(new FiberScriptUndoData(fiberList));
	
	FiberWidget *widget = fiberList->First();
	while (widget)
	{
		Fiber *fiber = widget->GetScriptFiber();
		
		unsigned_int32 flags = fiber->GetFiberFlags();
		if (flags & kFiberConditionTrue) flags = (flags & ~kFiberConditionTrue) | kFiberConditionFalse;
		else if (flags & kFiberConditionFalse) flags &= ~(kFiberConditionTrue | kFiberConditionFalse);
		else flags |= kFiberConditionTrue;
		fiber->SetFiberFlags(flags);
		
		widget->UpdateColor();
		
		widget = widget->Next();
	}
}

Point3D ScriptEditor::ViewportToGraphPosition(const Point3D& p) const
{
	const Vector2D& scale = scriptViewport->GetOrthoScale();
	OrthoCamera *camera = scriptViewport->GetViewportCamera();
	const Point3D& position = camera->GetNodePosition();
	
	const OrthoCameraObject *object = camera->GetObject();
	return (Point3D(p.x * scale.x + object->GetOrthoRectLeft() + position.x, p.y * scale.y + object->GetOrthoRectTop() + position.y, 0.0F));
}

Point3D ScriptEditor::AlignPositionToGrid(const Point3D& p)
{
	float x = Floor((p.x + 4.0F) * 0.125F) * 8.0F;
	float y = Floor((p.y + 4.0F) * 0.125F) * 8.0F;
	return (Point3D(x, y, 0.0F));
}

void ScriptEditor::ShowGraphPosition(float x, float y)
{
	const Vector2D& scale = scriptViewport->GetOrthoScale();
	OrthoCamera *camera = scriptViewport->GetViewportCamera();
	const Point3D& position = camera->GetNodePosition();
	
	float w = scriptViewport->GetWidgetSize().x * scale.x;
	float h = scriptViewport->GetWidgetSize().y * scale.y;
	
	float left = position.x - w * 0.5F;
	float right = left + w;
	float top = position.y - h * 0.5F;
	float bottom = top + h;
	
	float dx = 0.0F;
	if (x < left) dx = x - left;
	else if (x > right) dx = x - right;
	
	float dy = 0.0F;
	if (y < top) dy = y - top;
	else if (y > bottom) dy = y - bottom;
	
	camera->SetNodePosition(position + Vector3D(Floor(dx + 0.5F), Floor(dy + 0.5F), 0.0F));
	editorState |= kScriptEditorUpdateGrid;
}

void ScriptEditor::AutoScroll(const Point3D& p)
{
	float w = scriptViewport->GetWidgetSize().x;
	float h = scriptViewport->GetWidgetSize().y;
	
	if ((p.x < 0.0F) || (p.y < 0.0F) || (p.x > w) || (p.y > h))
	{
		float dt = TheTimeMgr->GetSystemFloatDeltaTime() * 0.01F;
		float dx = 0.0F;
		float dy = 0.0F;
		
		if (p.x < 0.0F) dx = Fmax((p.x - 15.0F) * dt, w * -0.5F);
		else if (p.x > w) dx = Fmin((p.x - w + 16.0F) * dt, w * 0.5F);
		
		if (p.y < 0.0F) dy = Fmax((float) (p.y - 15.0F) * dt, h * -0.5F);
		else if (p.y > h) dy = Fmin((float) (p.y - h + 16.0F) * dt, h * 0.5F);
		
		const Vector2D& scale = scriptViewport->GetOrthoScale();
		OrthoCamera *camera = scriptViewport->GetViewportCamera();
		camera->SetNodePosition(camera->GetNodePosition() + Vector3D(Floor(dx * scale.x + 0.5F), Floor(dy * scale.y + 0.5F), 0.0F));
		editorState |= kScriptEditorUpdateGrid;
	}
}

MethodWidget *ScriptEditor::FindMethodWidget(const Point3D& position) const
{
	WidgetPart	part;
	
	Widget *widget = graphRoot->DetectWidget(position, 0, &part);
	if ((widget) && (part == kWidgetPartInterior)) return (static_cast<MethodWidget *>(widget));
	
	return (nullptr);
}

void ScriptEditor::SortSelectedMethodSublist(List<MethodWidget> *list, float dmin, float dmax)
{
	MethodWidget *widget = list->Last();
	if ((list->First() == widget) || (dmax - dmin < 4.0F))
	{
		while (widget)
		{
			MethodWidget *prev = widget->Previous();
			selectedMethodList.Append(widget);
			widget = prev;
		}
	}
	else
	{
		List<MethodWidget>		nearList;
		
		float dminFar = dmax;
		float dmaxNear = dmin;
		float avg = (dmin + dmax) * 0.5F;
		
		widget = list->First();
		do
		{
			MethodWidget *next = widget->Next();
			float d = widget->GetSortPosition();
			if (d < avg)
			{
				dmaxNear = Fmax(dmaxNear, d);
				nearList.Append(widget);
			}
			else
			{
				dminFar = Fmin(dminFar, d);
			}
			
			widget = next;
		} while (widget);
		
		SortSelectedMethodSublist(list, dminFar, dmax);
		SortSelectedMethodSublist(&nearList, dmin, dmaxNear);
	}
}

void ScriptEditor::SortSelectedMethodList(float dx, float dy)
{
	List<MethodWidget>		widgetList;
	
	float dmin = K::infinity;
	float dmax = K::minus_infinity;
	
	MethodWidget *widget = selectedMethodList.First();
	while (widget)
	{
		MethodWidget *next = widget->Next();
		
		const Point3D& position = widget->GetWidgetPosition();
		float d = position.x * dx + position.y * dy;
		dmin = Fmin(dmin, d);
		dmax = Fmax(dmax, d);
		
		widget->SetSortPosition(d);
		widgetList.Append(widget);
		
		widget = next;
	}
	
	SortSelectedMethodSublist(&widgetList, dmin, dmax);
}

bool ScriptEditor::BoxIntersectsMethodWidget(const Point3D& p1, const Point3D& p2, const MethodWidget *widget)
{
	const Point3D& position = widget->GetWidgetPosition();
	
	float x1 = Fmin(p1.x, p2.x);
	float x2 = Fmax(p1.x, p2.x);
	float y1 = Fmin(p1.y, p2.y);
	float y2 = Fmax(p1.y, p2.y);
	
	if ((x2 < position.x) || (x1 > position.x + kMethodBoxWidth)) return (false);
	if ((y2 < position.y) || (y1 > position.y + kMethodBoxHeight)) return (false);
	
	return (true);
}

bool ScriptEditor::MethodBoxIntersectsAnyMethodWidget(float x, float y, const MethodWidget *exclude)
{
	Point3D p1(x - kMethodBoxPadding, y - kMethodBoxPadding, 0.0F);
	Point3D p2(x + kMethodBoxWidth + kMethodBoxPadding, y + kMethodBoxHeight + kMethodBoxPadding, 0.0F);
	
	const MethodWidget *widget = methodWidgetList.First();
	while (widget)
	{
		if ((widget != exclude) && (BoxIntersectsMethodWidget(p1, p2, widget))) return (true);
		widget = widget->Next();
	}
	
	widget = selectedMethodList.First();
	while (widget)
	{
		if ((widget != exclude) && (BoxIntersectsMethodWidget(p1, p2, widget))) return (true);
		widget = widget->Next();
	}
	
	return (false);
}

void ScriptEditor::BeginTool(const Point3D& p, unsigned_int32 eventFlags)
{
	previousPoint = p;
	anchorPoint = p;
	
	int32 tool = trackingTool;
	if (tool == kScriptToolMethodMove)
	{
		WidgetPart	part;
		
		bool moveable = false;
		bool shift = InterfaceMgr::GetShiftKey();
		anchorPosition = ViewportToGraphPosition(p);
		previousPosition = anchorPosition;
		
		Widget *widget = graphRoot->DetectWidget(previousPosition, 0, &part);
		if (widget)
		{
			if (part == kWidgetPartCurve)
			{
				FiberWidget *fiberWidget = static_cast<FiberWidget *>(widget);
				bool selected = FiberSelected(fiberWidget);
				if (shift)
				{
					if (!selected) SelectFiber(fiberWidget);
					else UnselectFiber(fiberWidget);
				}
				else
				{
					if (!selected)
					{
						UnselectAll();
						SelectFiber(fiberWidget);
					}
				}
			}
			else if (part == kWidgetPartOutput)
			{
				UnselectAll();
				
				MethodWidget *methodWidget = static_cast<MethodWidget *>(widget);
				methodWidget->UpdateOutputColor(true);
				
				fiberStartMethod = methodWidget;
				fiberFinishMethod = nullptr;
				
				fiberVertex[0] = anchorPosition.GetPoint2D();
				fiberVertex[1] = anchorPosition.GetPoint2D();
				fiberVertex[2] = anchorPosition.GetPoint2D();
				fiberVertex[3] = anchorPosition.GetPoint2D();
				
				fiberTangent[0].Set(1.0F, 0.0F, 0.0F, -3.0F);
				fiberTangent[1].Set(1.0F, 0.0F, 0.0F, 4.0F);
				fiberTangent[2].Set(1.0F, 0.0F, 0.0F, 4.0F);
				fiberTangent[3].Set(1.0F, 0.0F, 0.0F, -3.0F);
				
				trackingMode = kScriptEditorModeFiber;
			}
			else
			{
				MethodWidget *methodWidget = static_cast<MethodWidget *>(widget);
				bool selected = MethodSelected(methodWidget);
				if (shift)
				{
					if (!selected) SelectMethod(methodWidget, kMethodWidgetTempSelected);
					else UnselectMethod(methodWidget);
				}
				else
				{
					if (!selected)
					{
						UnselectAll();
						SelectMethod(methodWidget, kMethodWidgetTempSelected);
					}
					
					if (eventFlags & kMouseDoubleClick)
					{
						HandleGetInfoMenuItem(nullptr, nullptr);
						return;
					}
				}
				
				moveable = true;
			}
		}
		else
		{
			Widget *widget = sectionRoot->DetectWidget(previousPosition, 0, &part);
			if (widget)
			{
				ScriptSectionWidget *sectionWidget = static_cast<ScriptSectionWidget *>(widget);
				bool selected = SectionSelected(sectionWidget);
				
				if (part == kWidgetPartTitle)
				{
					if (shift)
					{
						if (!selected) SelectSection(sectionWidget);
						else UnselectSection(sectionWidget);
					}
					else
					{
						if (!selected)
						{
							UnselectAll();
							SelectSection(sectionWidget);
						}
						
						if (eventFlags & kMouseDoubleClick)
						{
							HandleGetInfoMenuItem(nullptr, nullptr);
							return;
						}
					}
					
					moveable = true;
				}
				else if (part == kWidgetPartResize)
				{
					if (!selected)
					{
						UnselectAll();
						SelectSection(sectionWidget);
					}
					
					sectionTrackWidget = sectionWidget;
					anchorPosition = sectionWidget->GetWidgetPosition();
					trackingMode = kScriptEditorModeSection;
					trackingTool = 0;
					
					AddUndoData(new ResizeScriptUndoData(sectionWidget));
				}
			}
			else
			{
				if (!shift) UnselectAll();
				trackingTool = kScriptToolGraphSelect;
			}
		}
		
		if (moveable)
		{
			MethodWidget *methodWidget = selectedMethodList.First();
			while (methodWidget)
			{
				methodWidget->SaveOriginalPosition();
				methodWidget = methodWidget->Next();
			}
			
			ScriptSectionWidget *sectionWidget = selectedSectionList.First();
			while (sectionWidget)
			{
				sectionWidget->SaveOriginalPosition();
				sectionWidget = sectionWidget->Next();
			}
			
			editorState |= kScriptEditorUndoPending;
		}
	}
	
	toolTracking = true;
}

void ScriptEditor::TrackTool(const Point3D& p)
{
	int32 tool = trackingTool;
	if (tool == kScriptToolGraphSelect)
	{
		float dx = Fabs(p.x - anchorPoint.x);
		float dy = Fabs(p.y - anchorPoint.y);
		
		if ((boxSelectFlag) || (dx > 3.0F) || (dy > 3.0F))
		{
			boxSelectFlag = true;
			AutoScroll(p);
			
			Point3D position = ViewportToGraphPosition(p);
			if (position != previousPosition)
			{
				previousPosition = position;
				dragRect.Build(anchorPosition.GetPoint2D(), position.GetPoint2D(), scriptViewport->GetOrthoScale().x);
				
				if ((dx >= 1.0F) && (dy >= 1.0F))
				{
					UnselectAllTemp();
					
					MethodWidget *widget = methodWidgetList.First();
					while (widget)
					{
						MethodWidget *next = widget->Next();
						if (BoxIntersectsMethodWidget(anchorPosition, position, widget)) SelectMethod(widget, kMethodWidgetTempSelected);
						widget = next;
					}
				}
			}
		}
	}
	else if (tool == kScriptToolMethodMove)
	{
		AutoScroll(p);
		
		Point3D position = ViewportToGraphPosition(p);
		if (position != previousPosition)
		{
			unsigned_int32 state = editorState;
			if (state & kScriptEditorUndoPending)
			{
				editorState = state & ~kScriptEditorUndoPending;
				AddUndoData(new MoveScriptUndoData(&selectedMethodList, &selectedSectionList));
			}
			
			SortSelectedMethodList(position.x - previousPosition.x, position.y - previousPosition.y);
			
			previousPosition = position;
			float dx = position.x - anchorPosition.x;
			float dy = position.y - anchorPosition.y;
			
			MethodWidget *methodWidget = selectedMethodList.First();
			while (methodWidget)
			{
				Point3D q = methodWidget->GetOriginalPosition();
				q.x += dx;
				q.y += dy;
				q = AlignPositionToGrid(q);
				
				bool clear = !MethodBoxIntersectsAnyMethodWidget(q.x, q.y, methodWidget);
				if (!clear)
				{
					const Point3D& currentPosition = methodWidget->GetWidgetPosition();
					
					if (!MethodBoxIntersectsAnyMethodWidget(q.x, currentPosition.y, methodWidget))
					{
						clear = true;
						q.y = currentPosition.y;
					}
					
					if ((!clear) && (!MethodBoxIntersectsAnyMethodWidget(currentPosition.x, q.y, methodWidget)))
					{
						clear = true;
						q.x = currentPosition.x;
					}
				}
				
				if (clear)
				{
					Method *method = methodWidget->GetScriptMethod();
					
					method->SetMethodPosition(q.GetPoint2D());
					methodWidget->SetWidgetPosition(q);
					methodWidget->Invalidate();
					
					RebuildFiberWidgets(method);
				}
				
				methodWidget = methodWidget->Next();
			}
			
			ScriptSectionWidget *sectionWidget = selectedSectionList.First();
			while (sectionWidget)
			{
				Point3D q = sectionWidget->GetOriginalPosition();
				q.x += dx;
				q.y += dy;
				q = AlignPositionToGrid(q);
				
				SectionMethod *section = sectionWidget->GetSectionMethod();
				
				section->SetMethodPosition(q.GetPoint2D());
				sectionWidget->SetWidgetPosition(q);
				sectionWidget->Invalidate();
				
				sectionWidget = sectionWidget->Next();
			}
		}
	}
	else
	{
		if (tool == kScriptToolViewportScroll)
		{
			float dx = previousPoint.x - p.x;
			float dy = previousPoint.y - p.y;
			
			if ((dx != 0.0F) || (dy != 0.0F))
			{
				OrthoCamera *camera = scriptViewport->GetViewportCamera();
				const Vector2D& scale = scriptViewport->GetOrthoScale();
				camera->SetNodePosition(camera->GetNodePosition() + Vector3D(Floor(dx * scale.x + 0.5F), Floor(dy * scale.y + 0.5F), 0.0F));
				editorState |= kScriptEditorUpdateGrid;
			}
		}
		else if (tool == kScriptToolViewportZoom)
		{
			float dy = previousPoint.y - p.y;
			if (dy != 0.0F) UpdateViewportScale(scriptViewport->GetOrthoScale().x * Exp(dy * -0.01F));
		}
	}
	
	previousPoint = p;
}

void ScriptEditor::EndTool(const Point3D& p)
{
	boxSelectFlag = false;
	editorState &= ~kScriptEditorUndoPending;
	
	MethodWidget *widget = selectedMethodList.First();
	while (widget)
	{
		widget->SetMethodWidgetState(widget->GetMethodWidgetState() & ~kMethodWidgetTempSelected);
		widget = widget->Next();
	}
}

void ScriptEditor::BeginSection(const Point3D& p)
{
	UnselectAll();
	
	anchorPosition = AlignPositionToGrid(ViewportToGraphPosition(p));
	previousPosition = anchorPosition;
	
	SectionMethod *section = new SectionMethod;
	scriptGraph.AddElement(section);
	
	ScriptSectionWidget *widget = new ScriptSectionWidget(this, section);
	widget->SetViewportScale(scriptViewport->GetOrthoScale().x);
	sectionRoot->AddNewSubnode(widget);
	sectionWidgetList.Append(widget);
	sectionTrackWidget = widget;
	
	section->SetMethodPosition(Point2D(anchorPosition.x, anchorPosition.y));
	widget->SetWidgetPosition(anchorPosition);
	
	SelectSection(widget);
	AddUndoData(new CreateScriptUndoData(widget));
	
	toolTracking = true;
}

void ScriptEditor::TrackSection(const Point3D& p)
{
	AutoScroll(p);
	
	Point3D position = AlignPositionToGrid(ViewportToGraphPosition(p));
	if (position != previousPosition)
	{
		float width = Fmax(position.x - anchorPosition.x, kMinSectionSize);
		float height = Fmax(position.y - anchorPosition.y, kMinSectionSize);
		
		SectionMethod *section = sectionTrackWidget->GetSectionMethod();
		section->SetSectionSize(width, height);
		
		sectionTrackWidget->SetWidgetSize(Vector2D(width, height));
		sectionTrackWidget->Invalidate();
		
		previousPosition = position;
	}
}

void ScriptEditor::EndSection(const Point3D& p)
{
	TrackSection(p);
	toolButton[kScriptToolMethodMove]->SetValue(1, true);
}

void ScriptEditor::BeginFiber(const Point3D& p)
{
	anchorPosition = ViewportToGraphPosition(p);
	previousPosition = anchorPosition;
	
	MethodWidget *widget = FindMethodWidget(previousPosition);
	if (widget)
	{
		UnselectAll();
		SelectMethod(widget);
		
		fiberStartMethod = widget;
		fiberFinishMethod = nullptr;
		
		fiberVertex[0] = anchorPosition.GetPoint2D();
		fiberVertex[1] = anchorPosition.GetPoint2D();
		fiberVertex[2] = anchorPosition.GetPoint2D();
		fiberVertex[3] = anchorPosition.GetPoint2D();
		
		fiberTangent[0].Set(1.0F, 0.0F, 0.0F, -3.0F);
		fiberTangent[1].Set(1.0F, 0.0F, 0.0F, 4.0F);
		fiberTangent[2].Set(1.0F, 0.0F, 0.0F, 4.0F);
		fiberTangent[3].Set(1.0F, 0.0F, 0.0F, -3.0F);
		
		toolTracking = true;
	}
}

void ScriptEditor::TrackFiber(const Point3D& p)
{
	AutoScroll(p);
	
	Point3D position = ViewportToGraphPosition(p);
	if (position != previousPosition)
	{
		previousPosition = position;
		
		fiberVertex[2] = position.GetPoint2D();
		fiberVertex[3] = position.GetPoint2D();
		
		Vector3D tangent = (position - anchorPosition).Normalize();
		fiberTangent[0].Set(tangent, -3.0F);
		fiberTangent[1].Set(tangent, 4.0F);
		fiberTangent[2].Set(tangent, 4.0F);
		fiberTangent[3].Set(tangent, -3.0F);
		
		if (fiberFinishMethod)
		{
			UnselectMethod(fiberFinishMethod);
			fiberFinishMethod = nullptr;
		}
		
		MethodWidget *widget = FindMethodWidget(position);
		if ((widget) && (widget != fiberStartMethod))
		{
			const Method *start = fiberStartMethod->GetScriptMethod();
			const Method *finish = widget->GetScriptMethod();
			
			const Fiber *fiber = start->GetFirstOutgoingEdge();
			while (fiber)
			{
				if (fiber->GetFinishElement() == finish) return;
				fiber = fiber->GetNextOutgoingEdge();
			}
			
			SelectMethod(widget);
			fiberFinishMethod = widget;
		}
	}
}

void ScriptEditor::EndFiber(const Point3D& p)
{
	fiberStartMethod->UpdateOutputColor(false);
	
	if (fiberFinishMethod)
	{
		Method *start = fiberStartMethod->GetScriptMethod();
		Method *finish = fiberFinishMethod->GetScriptMethod();
		
		Fiber *fiber = new Fiber(start, finish);
		FiberWidget *widget = new FiberWidget(this, fiber);
		graphRoot->AddFirstSubnode(widget);
		fiberWidgetList.Append(widget);
		widget->Preprocess();
		
		UnselectAll();
		SelectFiber(widget);
		AddUndoData(new CreateScriptUndoData(widget));
		
		editorState |= kScriptEditorUpdateGraph;
	}
}

void ScriptEditor::CreateMethod(const Point3D& p)
{
	Point3D position = AlignPositionToGrid(ViewportToGraphPosition(p));
	
	if (!MethodBoxIntersectsAnyMethodWidget(position.x, position.y))
	{
		UnselectAll();
		
		Method *method = Method::New(currentMethodReg->GetMethodType());
		scriptGraph.AddElement(method);
		
		MethodWidget *widget = new MethodWidget(this, method, currentMethodReg);
		widget->SetViewportScale(scriptViewport->GetOrthoScale().x);
		graphRoot->AddNewSubnode(widget);
		methodWidgetList.Append(widget);
		
		method->SetMethodPosition(Point2D(position.x, position.y));
		widget->SetWidgetPosition(position);
		
		SelectMethod(widget);
		AddUndoData(new CreateScriptUndoData(widget));
		
		toolButton[kScriptToolMethodMove]->SetValue(1, true);
		editorState |= kScriptEditorUpdateGraph;
	}
}

void ScriptEditor::ViewportHandleMouseEvent(const MouseEventData *eventData, ViewportWidget *viewport, void *cookie)
{
	ScriptEditor *scriptEditor = static_cast<ScriptEditor *>(cookie);
	
	EventType eventType = eventData->eventType;	
	if ((eventType == kEventMouseDown) || (eventType == kEventMiddleMouseDown))
	{
		int32 mode = scriptEditor->currentMode;
		int32 tool = scriptEditor->currentTool;
		
		if ((eventType == kEventMiddleMouseDown) || (InterfaceMgr::GetOptionKey()))
		{
			mode = kScriptEditorModeTool;
			tool = kScriptToolViewportScroll;
		}
		
		scriptEditor->trackingMode = mode;
		scriptEditor->trackingTool = tool;
		scriptEditor->toolTracking = false;
		
		if (mode == kScriptEditorModeTool) scriptEditor->BeginTool(eventData->mousePosition, eventData->eventFlags);
		else if (mode == kScriptEditorModeMethod) scriptEditor->CreateMethod(eventData->mousePosition);
		else if (mode == kScriptEditorModeSection) scriptEditor->BeginSection(eventData->mousePosition);
		else if (mode == kScriptEditorModeFiber) scriptEditor->BeginFiber(eventData->mousePosition);
	}
	else if ((eventType == kEventMouseUp) || (eventType == kEventMiddleMouseUp))
	{
		if (scriptEditor->toolTracking)
		{
			int32 mode = scriptEditor->trackingMode;
			if (mode == kScriptEditorModeTool) scriptEditor->EndTool(eventData->mousePosition);
			else if (mode == kScriptEditorModeSection) scriptEditor->EndSection(eventData->mousePosition);
			else if (mode == kScriptEditorModeFiber) scriptEditor->EndFiber(eventData->mousePosition);
			
			scriptEditor->toolTracking = false;
		}
	}
	else if (eventType == kEventMouseWheel)
	{
		if (!scriptEditor->toolTracking)
		{
			OrthoViewportWidget *orthoViewport = static_cast<OrthoViewportWidget *>(viewport);
			scriptEditor->UpdateViewportScale(orthoViewport->GetOrthoScale().x * Exp(eventData->mousePosition.y * -0.16F));
		}
	}
}

void ScriptEditor::ViewportTrackTask(const Point3D& position, ViewportWidget *viewport, void *cookie)
{
	ScriptEditor *scriptEditor = static_cast<ScriptEditor *>(cookie);
	if (scriptEditor->toolTracking)
	{
		int32 mode = scriptEditor->trackingMode;
		if (mode == kScriptEditorModeTool) scriptEditor->TrackTool(position);
		else if (mode == kScriptEditorModeSection) scriptEditor->TrackSection(position);
		else if (mode == kScriptEditorModeFiber) scriptEditor->TrackFiber(position);
	}
}

void ScriptEditor::ViewportRender(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie)
{
	ScriptEditor *scriptEditor = static_cast<ScriptEditor *>(cookie);
	
	unsigned_int32 state = scriptEditor->editorState;
	if (state & kScriptEditorUpdateGrid)
	{
		scriptEditor->editorState = state & ~kScriptEditorUpdateGrid;
		
		const OrthoViewportWidget *orthoViewport = scriptEditor->scriptViewport;
		const OrthoCamera *camera = orthoViewport->GetViewportCamera();
		const OrthoCameraObject *cameraObject = camera->GetObject();
		
		const Point3D& position = camera->GetNodePosition();
		float xmin = cameraObject->GetOrthoRectLeft() + position.x;
		float xmax = cameraObject->GetOrthoRectRight() + position.x;
		float ymin = cameraObject->GetOrthoRectTop() + position.y;
		float ymax = cameraObject->GetOrthoRectBottom() + position.y;
		
		scriptEditor->viewportGrid.Build(Point2D(xmin, ymin), Point2D(xmax, ymax), orthoViewport->GetOrthoScale().x);
	}
	
	renderList->Append(&scriptEditor->viewportGrid);
	
	Widget *widget = scriptEditor->sectionRoot;
	widget->Update();
	widget->RenderTree(renderList);
	
	widget = scriptEditor->graphRoot;
	widget->Update();
	widget->RenderTree(renderList);
	
	if (scriptEditor->boxSelectFlag) renderList->Append(&scriptEditor->dragRect);
	else if ((scriptEditor->toolTracking) && (scriptEditor->trackingMode == kScriptEditorModeFiber)) renderList->Append(&scriptEditor->fiberRenderable);
}

void ScriptEditor::HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		SetFocusWidget(nullptr);
		
		for (machine a = 0; a < kScriptToolCount; a++)
		{
			if (widget == toolButton[a])
			{
				if ((currentMode != kScriptEditorModeTool) || (currentTool != a))
				{
					UnselectCurrentTool();
					
					currentMode = kScriptEditorModeTool;
					currentTool = a;
				}
				
				break;
			}
		}
	}
}

void ScriptEditor::ConfirmationDialogComplete(Dialog *dialog, void *cookie)
{
	if (dialog->GetDialogStatus() == kDialogOkay)
	{
		ScriptEditor *scriptEditor = static_cast<ScriptEditor *>(cookie);
		scriptEditor->CallCompletionProc();
		scriptEditor->Close();
	}
}

void ScriptEditor::Move(void)
{
	if ((TheInterfaceMgr->GetActiveWindow() == this) && (!TheInterfaceMgr->GetActiveMenu()))
	{
		const Point3D& position = TheInterfaceMgr->GetCursorPosition();
		
		bool toolCursor = toolTracking;
		if (!toolCursor)
		{
			Vector2D vp = position.GetVector2D() - scriptViewport->GetWorldPosition().GetVector2D();
			if ((vp.x >= 0.0F) && (vp.x < scriptViewport->GetWidgetSize().x) && (vp.y >= 0.0F) && (vp.y < scriptViewport->GetWidgetSize().y)) toolCursor = true;
		}
		
		int32 cursorIndex = kEditorCursorArrow;
		if (toolCursor)
		{
			int32 mode = currentMode;
			int32 tool = currentTool;
			
			if (!toolTracking)
			{
				if (InterfaceMgr::GetOptionKey())
				{
					mode = kScriptEditorModeTool;
					tool = kScriptToolViewportScroll;
				}
			}
			else
			{
				if ((trackingMode != kScriptEditorModeSection) || (currentMode == kScriptEditorModeSection))
				{
					mode = trackingMode;
					tool = trackingTool;
				}
			}
			
			if (mode == kScriptEditorModeTool)
			{
				if (tool == kScriptToolViewportScroll) cursorIndex = (toolTracking) ? kEditorCursorDrag : kEditorCursorHand;
				else if (tool == kScriptToolViewportZoom) cursorIndex = kEditorCursorGlass;
			}
			else
			{
				cursorIndex = kEditorCursorCross;
				if ((!toolTracking) && (mode == kScriptEditorModeMethod))
				{
					Point3D p = AlignPositionToGrid(ViewportToGraphPosition(position - scriptViewport->GetWorldPosition().GetVector3D()));
					if (MethodBoxIntersectsAnyMethodWidget(p.x, p.y)) cursorIndex = kEditorCursorStop;
				}
			}
		}
		
		TheInterfaceMgr->SetCursor(TheWorldEditor->GetEditorCursor(cursorIndex));
	}
	
	if (editorState & kScriptEditorUpdateMenus)
	{
		bool methodSelection = !selectedMethodList.Empty();
		bool fiberSelection = !selectedFiberList.Empty();
		bool sectionSelection = !selectedSectionList.Empty();
		
		MenuItemWidget *getInfoItem = scriptMenuItem[kScriptMenuGetInfo];
		getInfoItem->Disable();
		
		if ((methodSelection) || (sectionSelection))
		{
			scriptMenuItem[kScriptMenuCut]->Enable();
			scriptMenuItem[kScriptMenuCopy]->Enable();
			scriptMenuItem[kScriptMenuDuplicate]->Enable();
			
			if (methodSelection)
			{
				if ((!(fiberSelection | sectionSelection)) && (selectedMethodList.First() == selectedMethodList.Last())) getInfoItem->Enable();
			}
			else
			{
				if ((!fiberSelection) && (selectedSectionList.First() == selectedSectionList.Last())) getInfoItem->Enable();
			}
		}
		else
		{
			scriptMenuItem[kScriptMenuCut]->Disable();
			scriptMenuItem[kScriptMenuCopy]->Disable();
			scriptMenuItem[kScriptMenuDuplicate]->Disable();
		}
		
		MenuItemWidget *menuItem = scriptMenuItem[kScriptMenuCycleFiberCondition];
		if (fiberSelection) menuItem->Enable();
		else menuItem->Disable();
		
		menuItem = scriptMenuItem[kScriptMenuClear];
		if (methodSelection | fiberSelection | sectionSelection) menuItem->Enable();
		else menuItem->Disable();
	}
	
	if (editorState & kScriptEditorUpdateGraph) UpdateScriptGraph();
	
	editorState &= ~(kScriptEditorUpdateMenus | kScriptEditorUpdateGraph);
	
	graphRoot->Move();
	sectionRoot->Move();
	Window::Move();
}

void ScriptEditor::EnterForeground(void)
{
	Window::EnterForeground();
	if (!editorClipboard.Empty()) scriptMenuItem[kScriptMenuPaste]->Enable();
}

void ScriptEditor::EnterBackground(void)
{
	Window::EnterBackground();
	TheInterfaceMgr->SetCursor(nullptr);
}

bool ScriptEditor::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	EventType eventType = eventData->eventType;
	
	if (eventType == kEventKeyDown)
	{
		if (toolTracking) return (true);
		
		unsigned_int32 code = eventData->keyCode;
		if (code == kKeyCodeReturn)
		{
			okayButton->Activate();
			return (true);
		}
		else if (code == kKeyCodeEscape)
		{
			cancelButton->Activate();
			return (true);
		}
		
		if ((code >= '1') && (code <= '7'))
		{
			if (code == '1') toolButton[kScriptToolMethodMove]->SetValue(1, true);
			else if (code == '6') toolButton[kScriptToolViewportScroll]->SetValue(1, true);
			else if (code == '7') toolButton[kScriptToolViewportZoom]->SetValue(1, true);
			return (true);
		}
		else if ((code == kKeyCodeDelete) || (code == kKeyCodeBackspace))
		{
			HandleClearMenuItem(nullptr, nullptr);
			return (true);
		}
	}
	else if (eventType == kEventKeyCommand)
	{
		if (toolTracking) return (true);
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void ScriptEditor::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	EventType eventType = eventData->eventType;
	
	if (eventType == kEventWidgetActivate)
	{
		if (widget == okayButton)
		{
			scriptObject->SetScriptGraph(&scriptGraph);
			
			scriptObject->PurgeValues();
			for (;;)
			{
				Value *value = scriptValueMap.First();
				if (!value) break;
				
				scriptObject->AddValue(value);
			}
			
			const Method *method = scriptGraph.GetFirstElement();
			while (method)
			{
				const ConnectorKey& key = method->GetTargetKey();
				if (key[0] != '$')
				{
					Node *node = targetNode->GetConnectedNode(key);
					if ((node) && (!node->GetController()))
					{
						const MethodRegistration *registration = Method::FindRegistration(method->GetMethodType());
						if (!(registration->GetMethodFlags() & kMethodNoMessage)) node->SetController(new Controller);
					}
				}
				
				method = method->GetNextElement();
			}
			
			CallCompletionProc();
			Close();
		}
		else if (widget == cancelButton)
		{
			if (editorState & kScriptEditorModified)
			{
				const StringTable *table = TheWorldEditor->GetStringTable();
				
				Dialog *dialog = new Dialog(Vector2D(342.0F, 120.0F), table->GetString(StringID('SCPT')), table->GetString(StringID('BTTN', 'DSCD')), table->GetString(StringID('BTTN', 'CANC')));
				
				ImageWidget *image = new ImageWidget(Vector2D(64.0F, 64.0F), "C4/warning");
				image->SetWidgetPosition(Point3D(12.0F, 12.0F, 0.0F));
				dialog->AddSubnode(image);
				
				TextWidget *text = new TextWidget(Vector2D(242.0F, 0.0F), table->GetString(StringID('SCPT', 'CFRM')), "font/Gui");
				text->SetTextFlags(kTextWrapped);
				text->SetWidgetPosition(Point3D(88.0F, 16.0F, 0.0F));
				dialog->AddSubnode(text);
				
				dialog->SetCompletionProc(&ConfirmationDialogComplete, this);
				AddSubwindow(dialog);
			}
			else
			{
				CallCompletionProc();
				Close();
			}
		}
	}
	else if (eventType == kEventWidgetChange)
	{
		if (widget == sectionButton)
		{
			SetFocusWidget(nullptr);
			
			if (currentMode != kScriptEditorModeSection)
			{
				UnselectCurrentTool();
				
				currentMode = kScriptEditorModeSection;
				currentTool = 0;
			}
		}
	}
}

// ZYURVUR
