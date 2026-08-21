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


#include "C4Graphics.h"
#include "C4EditorBase.h"


using namespace C4;


EditorEvent::EditorEvent(EditorEventType type)
{
	eventType = type;
}

EditorEvent::~EditorEvent()
{
}


NodeEditorEvent::NodeEditorEvent(EditorEventType type, Node *node) : EditorEvent(type)
{
	eventNode = node;
}

NodeEditorEvent::~NodeEditorEvent()
{
}


GizmoEditorEvent::GizmoEditorEvent(EditorEventType type, Node *gizmoTarget) : EditorEvent(type)
{
	eventGizmoTarget = gizmoTarget;
}

GizmoEditorEvent::~GizmoEditorEvent()
{
}


MaterialEditorEvent::MaterialEditorEvent(EditorEventType type, MaterialObject *materialObject) : EditorEvent(type)
{
	eventMaterialObject = materialObject;
}

MaterialEditorEvent::~MaterialEditorEvent()
{
}

// ZYURVUR
