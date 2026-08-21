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


#include "C4VertexPrograms.h"
#include "C4Shaders.h"
#include "C4Engine.h"


#define C4LOG_VERTEX_PROGRAMS		0


using namespace C4;


#if C4OPENGL

	#define RESULT_POSITION			"gl_Position"
	#define RESULT_COLOR0			"gl_FrontColor"
	#define RESULT_COLOR1			"gl_FrontSecondaryColor"
	#define RESULT_POINTSIZE		"gl_PointSize"
	#define RESULT_TEXCOORD0		"gl_TexCoord[0]"
	
	#define FLOAT2					"vec2"
	#define FLOAT3					"vec3"
	#define FLOAT4					"vec4"
	
	#define RSQRT					"inversesqrt"
	#define FRAC					"fract"

#else

	#define RESULT_POSITION			"result.position"
	#define RESULT_COLOR0			"result.color0"
	#define RESULT_COLOR1			"result.color1"
	#define RESULT_POINTSIZE		"result.pointsize"
	#define RESULT_TEXCOORD0		"result.texcoord[0]"
	
	#define FLOAT2					"float2"
	#define FLOAT3					"float3"
	#define FLOAT4					"float4"
	
	#define RSQRT					"rsqrt"
	#define FRAC					"frac"

#endif


HashTable<VertexProgram> *VertexProgram::hashTable;
char VertexProgram::hashTableStorage[sizeof(HashTable<VertexProgram>)];


const VertexSnippet VertexProgram::nullTransform =
{
	'NULL', 0,
	
	"MOV		result.position, vertex.attrib[0];\n",
	
	RESULT_POSITION " = attrib[0];\n"
};

const VertexSnippet VertexProgram::modelviewProjectTransform =
{
	'MODL', 0,
	
	"DP4		result.position.x, vertex.attrib[0], program.env[" VERTEX_PARAM_MATRIX_MVP0 "];\n"
	"DP4		result.position.y, vertex.attrib[0], program.env[" VERTEX_PARAM_MATRIX_MVP1 "];\n"
	"DP4		result.position.z, vertex.attrib[0], program.env[" VERTEX_PARAM_MATRIX_MVP2 "];\n"
	"DP4		result.position.w, vertex.attrib[0], program.env[" VERTEX_PARAM_MATRIX_MVP3 "];\n",
	
	RESULT_POSITION ".x = dot(attrib[0], param[" VERTEX_PARAM_MATRIX_MVP0 "]);\n"
	RESULT_POSITION ".y = dot(attrib[0], param[" VERTEX_PARAM_MATRIX_MVP1 "]);\n"
	RESULT_POSITION ".z = dot(attrib[0], param[" VERTEX_PARAM_MATRIX_MVP2 "]);\n"
	RESULT_POSITION ".w = dot(attrib[0], param[" VERTEX_PARAM_MATRIX_MVP3 "]);\n"
};

const VertexSnippet VertexProgram::modelviewProjectTransformInfinite =
{
	'MDLI', 0,
	
	"DP3		result.position.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_MVP0 "];\n"
	"DP3		result.position.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_MVP1 "];\n"
	"DP3		result.position.z, %OPOS, program.env[" VERTEX_PARAM_MATRIX_MVP2 "];\n"
	"DP3		result.position.w, %OPOS, program.env[" VERTEX_PARAM_MATRIX_MVP3 "];\n",
	
	RESULT_POSITION ".x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_MVP0 "].xyz);\n"
	RESULT_POSITION ".y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_MVP1 "].xyz);\n"
	RESULT_POSITION ".z = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_MVP2 "].xyz);\n"
	RESULT_POSITION ".w = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_MVP3 "].xyz);\n"
};

const VertexSnippet VertexProgram::modelviewProjectTransformHomogeneous =
{
	'MDLH', 0,
	
	"DPH		result.position.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_MVP0 "];\n"
	"DPH		result.position.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_MVP1 "];\n"
	"DPH		result.position.z, %OPOS, program.env[" VERTEX_PARAM_MATRIX_MVP2 "];\n"
	"DPH		result.position.w, %OPOS, program.env[" VERTEX_PARAM_MATRIX_MVP3 "];\n",
	
	RESULT_POSITION ".x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_MVP0 "].xyz) + param[" VERTEX_PARAM_MATRIX_MVP0 "].w;\n"
	RESULT_POSITION ".y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_MVP1 "].xyz) + param[" VERTEX_PARAM_MATRIX_MVP1 "].w;\n"
	RESULT_POSITION ".z = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_MVP2 "].xyz) + param[" VERTEX_PARAM_MATRIX_MVP2 "].w;\n"
	RESULT_POSITION ".w = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_MVP3 "].xyz) + param[" VERTEX_PARAM_MATRIX_MVP3 "].w;\n"
};
 
const VertexSnippet VertexProgram::calculateCameraDirection =
{ 
	'CDIR', 0, 
	 
	"TEMP		cdir;\n"
	 
	"SUB		cdir.xyz, vertex.attrib[0], program.env[" VERTEX_PARAM_CAMERA_POSITION "];\n",
	
	FLOAT3 " cdir = attrib[0].xyz - param[" VERTEX_PARAM_CAMERA_POSITION "].xyz;\n"
}; 

const VertexSnippet VertexProgram::calculateCameraDirection4D =
{
	'CDR4', 0, 
	
	"TEMP		cdir;\n"
	
	"MAD		cdir.xyz, vertex.attrib[0], program.env[" VERTEX_PARAM_CAMERA_POSITION "].w, -program.env[" VERTEX_PARAM_CAMERA_POSITION "];\n",
	
	FLOAT3 " cdir = attrib[0].xyz * param[" VERTEX_PARAM_CAMERA_POSITION "].w - param[" VERTEX_PARAM_CAMERA_POSITION "].xyz;\n"
};

const VertexSnippet VertexProgram::scaleVertexCalculateCameraDirection =
{
	'SCDR', 0,
	
	"TEMP		vrtx, cdir;\n"
	
	"MUL		vrtx.xyz, vertex.attrib[0], program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "];\n"
	"MUL		vrtx.w, vertex.attrib[6].w, program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].w;\n"
	"SUB		cdir.xyz, vrtx, program.env[" VERTEX_PARAM_CAMERA_POSITION "];\n",
	
	FLOAT4 " vrtx;\n"
	
	"vrtx.xyz = attrib[0].xyz * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].xyz;\n"
	"vrtx.w = attrib[6].w * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].w;\n"
	FLOAT3 " cdir = vrtx.xyz - param[" VERTEX_PARAM_CAMERA_POSITION "].xyz;\n"
	
};

const VertexSnippet VertexProgram::scaleVertexCalculateCameraDirection4D =
{
	'SCD4', 0,
	
	"TEMP		vrtx, cdir;\n"
	
	"MUL		vrtx.xyz, vertex.attrib[0], program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "];\n"
	"MUL		vrtx.w, vertex.attrib[6].w, program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].w;\n"
	"MAD		cdir.xyz, vrtx, program.env[" VERTEX_PARAM_CAMERA_POSITION "].w, -program.env[" VERTEX_PARAM_CAMERA_POSITION "];\n",
	
	FLOAT4 " vrtx;\n"
	
	"vrtx.xyz = attrib[0].xyz * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].xyz;\n"
	"vrtx.w = attrib[6].w * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].w;\n"
	FLOAT3 " cdir = vrtx.xyz * param[" VERTEX_PARAM_CAMERA_POSITION "].w - param[" VERTEX_PARAM_CAMERA_POSITION "].xyz;\n"
};

const VertexSnippet VertexProgram::calculateBillboardPosition =
{
	'BILL', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"MAD		opos.xyz, program.env[" VERTEX_PARAM_CAMERA_RIGHT "], vertex.attrib[6].x, vertex.attrib[0];\n"
	"MAD		opos.xyz, program.env[" VERTEX_PARAM_CAMERA_DOWN "], vertex.attrib[6].y, opos;\n",
	
	FLOAT4 " opos;\n"
	
	"opos.xyz = attrib[0].xyz + param[" VERTEX_PARAM_CAMERA_RIGHT "].xyz * attrib[6].x + param[" VERTEX_PARAM_CAMERA_DOWN "].xyz * attrib[6].y;\n"
};

const VertexSnippet VertexProgram::calculateBillboardScalePosition =
{
	'SBLL', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"MUL		temp.xy, vertex.attrib[6], program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "];\n"
	"MAD		opos.xyz, program.env[" VERTEX_PARAM_CAMERA_RIGHT "], temp.x, vertex.attrib[0];\n"
	"MAD		opos.xyz, program.env[" VERTEX_PARAM_CAMERA_DOWN "], temp.y, opos;\n",
	
	FLOAT4 " opos;\n"
	
	"temp.xy = attrib[6].xy * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].xy;\n"
	"opos.xyz = attrib[0].xyz + param[" VERTEX_PARAM_CAMERA_RIGHT "].xyz * temp.x + param[" VERTEX_PARAM_CAMERA_DOWN "].xyz * temp.y;\n"
};

const VertexSnippet VertexProgram::calculateVertexBillboardPosition =
{
	'VBLL', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"MUL		opos.xyz, program.env[" VERTEX_PARAM_CAMERA_RIGHT "], vertex.attrib[0].x;\n"
	"MAD		opos.xyz, program.env[" VERTEX_PARAM_CAMERA_DOWN "], vertex.attrib[0].y, opos;\n",
	
	FLOAT4 " opos;\n"
	
	"opos.xyz = param[" VERTEX_PARAM_CAMERA_RIGHT "].xyz * attrib[0].x + param[" VERTEX_PARAM_CAMERA_DOWN "].xyz * attrib[0].y;\n"
};

const VertexSnippet VertexProgram::calculateVertexBillboardScalePosition =
{
	'VSBL', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"MUL		temp.xy, vertex.attrib[0], program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "];\n"
	"MUL		opos.xyz, program.env[" VERTEX_PARAM_CAMERA_RIGHT "], temp.x;\n"
	"MAD		opos.xyz, program.env[" VERTEX_PARAM_CAMERA_DOWN "], temp.y, opos;\n",
	
	FLOAT4 " opos;\n"
	
	"temp.xy = attrib[0].xy * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].xy;\n"
	"opos.xyz = param[" VERTEX_PARAM_CAMERA_RIGHT "].xyz * temp.x + param[" VERTEX_PARAM_CAMERA_DOWN "].xyz * temp.y;\n"
};

const VertexSnippet VertexProgram::calculateLightedBillboardPosition =
{
	'LBLL', kVertexSnippetPositionFlag,
	
	"TEMP		opos, tang, btng;\n"
	
	"MUL		tang.xyz, program.env[" VERTEX_PARAM_CAMERA_RIGHT "], vertex.attrib[6].x;\n"
	"MAD		tang.xyz, program.env[" VERTEX_PARAM_CAMERA_DOWN "], vertex.attrib[6].y, tang;\n"
	"MUL		btng.xyz, program.env[" VERTEX_PARAM_CAMERA_RIGHT "], -vertex.attrib[6].y;\n"
	"MAD		btng.xyz, program.env[" VERTEX_PARAM_CAMERA_DOWN "], vertex.attrib[6].x, btng;\n"
	
	"MAD		opos.xyz, tang, vertex.attrib[6].z, vertex.attrib[0];\n"
	"MAD		opos.xyz, btng, vertex.attrib[6].w, opos;\n",
	
	FLOAT4 " opos, tang, btng;\n"
	
	"tang.xyz = param[" VERTEX_PARAM_CAMERA_RIGHT "].xyz * attrib[6].x + param[" VERTEX_PARAM_CAMERA_DOWN "].xyz * attrib[6].y;\n"
	"btng.xyz = param[" VERTEX_PARAM_CAMERA_DOWN "].xyz * attrib[6].x - param[" VERTEX_PARAM_CAMERA_RIGHT "].xyz * attrib[6].y;\n"
	"opos.xyz = attrib[0].xyz + tang.xyz * attrib[6].z + btng * attrib[6].w;\n"
};

const VertexSnippet VertexProgram::calculatePostboardPosition =
{
	'POST', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"MAD		temp.xy, vertex.attrib[0], program.env[" VERTEX_PARAM_CAMERA_POSITION "].w, -program.env[" VERTEX_PARAM_CAMERA_POSITION "];\n"
	"MUL		temp.w, temp.x, temp.x;\n"
	"MAD		temp.w, temp.y, temp.y, temp.w;\n"
	"RSQ		temp.w, temp.w;\n"
	"MUL		opos.x, temp.y, temp.w;\n"
	"MUL		opos.y, -temp.x, temp.w;\n"
	"MAD		opos.xy, opos, vertex.attrib[6].x, vertex.attrib[0];\n"
	"MOV		opos.z, vertex.attrib[0];\n",
	
	FLOAT4 " opos;\n"
	
	"temp.xy = attrib[0].xy * param[" VERTEX_PARAM_CAMERA_POSITION "].w - param[" VERTEX_PARAM_CAMERA_POSITION "].xy;\n"
	"temp.w = " RSQRT " (temp.x * temp.x + temp.y * temp.y);\n"
	"opos.xy = " FLOAT2 "(temp.y * temp.w, -temp.x * temp.w) * attrib[6].x + attrib[0].xy;\n"
	"opos.z = attrib[0].z;\n"
};

const VertexSnippet VertexProgram::calculatePostboardScalePosition =
{
	'SPST', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"MUL		temp.z, vertex.attrib[6].x, program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].x;\n"
	"MAD		temp.xy, vertex.attrib[0], program.env[" VERTEX_PARAM_CAMERA_POSITION "].w, -program.env[" VERTEX_PARAM_CAMERA_POSITION "];\n"
	"MUL		temp.w, temp.x, temp.x;\n"
	"MAD		temp.w, temp.y, temp.y, temp.w;\n"
	"RSQ		temp.w, temp.w;\n"
	"MUL		opos.x, temp.y, temp.w;\n"
	"MUL		opos.y, -temp.x, temp.w;\n"
	"MAD		opos.xy, opos, temp.z, vertex.attrib[0];\n"
	"MUL		opos.z, vertex.attrib[0], program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "];\n",
	
	FLOAT4 " opos;\n"
	
	"temp.xy = attrib[0].xy * param[" VERTEX_PARAM_CAMERA_POSITION "].w - param[" VERTEX_PARAM_CAMERA_POSITION "].xy;\n"
	"temp.w = " RSQRT " (temp.x * temp.x + temp.y * temp.y);\n"
	"opos.xy = " FLOAT2 "(temp.y * temp.w, -temp.x * temp.w) * (attrib[6].x * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].x) + attrib[0].xy;\n"
	"opos.z = attrib[0].z * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].z;\n"
};

const VertexSnippet VertexProgram::calculatePolyboardNormal =
{
	'POLY', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"DP3		temp.w, cdir, cdir;\n"
	"RSQ		temp.w, temp.w;\n"
	"MUL		cdir.xyz, cdir, temp.w;\n"
	"XPD		opos.xyz, cdir, vertex.attrib[6];\n"
	
	"DP3		temp.w, opos, opos;\n"
	"RSQ		temp.y, temp.w;\n"
	"MUL		temp.x, temp.y, temp.w;\n"
	
	"MAD		temp.z, temp.x, 132.741, -130.37;\n"
	"MAD		temp.z, temp.z, temp.x, 34.6667;\n"
	"MAD		temp.z, temp.z, temp.x, 1.0;\n"
	"MIN		temp.y, temp.y, temp.z;\n"
	"MUL		opos.xyz, opos, temp.y;\n",
	
	FLOAT4 " opos;\n"
	
	"cdir *= " RSQRT "(dot(cdir, cdir));\n"
	"opos.xyz = cross(cdir, attrib[6].xyz);\n"
	"temp.w = dot(opos.xyz, opos.xyz);\n"
	"temp.y = " RSQRT "(temp.w);\n"
	"temp.x = temp.w * temp.y;\n"
	"temp.z = temp.x * (temp.x * (temp.x * 132.741 - 130.37) + 34.6667) + 1.0;\n"
	"opos.xyz *= min(temp.y, temp.z);\n"
};

const VertexSnippet VertexProgram::calculateLinearPolyboardNormal =
{
	'LPOL', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"XPD		opos.xyz, cdir, vertex.attrib[6];\n"
	"DP3		temp.w, opos, opos;\n"
	"RSQ		temp.w, temp.w;\n"
	"MUL		opos.xyz, opos, temp.w;\n",
	
	FLOAT4 " opos;\n"
	
	"opos.xyz = cross(cdir, attrib[6].xyz);\n"
	"opos.xyz *= " RSQRT "(dot(opos.xyz, opos.xyz));\n"
};

const VertexSnippet VertexProgram::calculatePolyboardPosition =
{
	'CPBP', 0,
	
	"MAD		opos.xyz, opos, vertex.attrib[6].w, vertex.attrib[0];\n",
	
	"opos.xyz = opos.xyz * attrib[6].w + attrib[0].xyz;\n"
};

const VertexSnippet VertexProgram::calculatePolyboardScalePosition =
{
	'CPSP', 0,
	
	"MAD		opos.xyz, opos, vrtx.w, vrtx;\n",
	
	"opos.xyz = opos.xyz * vrtx.w + vrtx.xyz;\n"
};

const VertexSnippet VertexProgram::calculateScalePosition =
{
	'SPOS', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"MUL		opos.xyz, vertex.attrib[0], program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "];\n",
	
	FLOAT4 " opos;\n"
	
	"opos.xyz = attrib[0].xyz * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].xyz;\n"
};

const VertexSnippet VertexProgram::calculateScaleOffsetPosition =
{
	'SOPS', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"MUL		opos.xyz, vertex.attrib[6], program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].w;\n"
	"MAD		opos.xyz, vertex.attrib[0], program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "], opos;\n",
	
	FLOAT4 " opos;\n"
	
	"opos.xyz = attrib[6].xyz * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].w + attrib[0].xyz * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].xyz;\n"
};

const VertexSnippet VertexProgram::calculateExpandNormalPosition =
{
	'NEPT', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"MAD		opos.xyz, %NRML, program.env[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].x, vertex.attrib[0];\n",
	
	FLOAT4 " opos;\n"
	
	"opos.xyz = %NRML.xyz * param[" VERTEX_PARAM_VERTEX_SCALE_OFFSET "].x + attrib[0].xyz;\n"
};

const VertexSnippet VertexProgram::calculateTerrainBorderPosition =
{
	'TRBP', kVertexSnippetPositionFlag,
	
	"TEMP		opos, prim;\n"
	
	"SGE		prim.xyz, vertex.attrib[5], 0.75;\n"
	"SLT		temp.xyz, vertex.attrib[5], 0.25;\n"
	
	"MUL		prim.xyz, prim, program.env[" VERTEX_PARAM_TERRAIN_PARAMETER0 "];\n"
	"MAD		prim.xyz, temp, program.env[" VERTEX_PARAM_TERRAIN_PARAMETER1 "], prim;\n"
	"MAX		prim.x, prim.x, prim.y;\n"
	"MAX		prim.x, prim.x, prim.z;\n"
	
	"SUB		prim.w, 1.0, prim.x;\n"
	"MUL		temp.xyz, vertex.attrib[0], prim.x;\n"
	"MAD		opos.xyz, vertex.attrib[1], prim.w, temp;\n",
	
	FLOAT4 " opos;\n"
	
	FLOAT3 " prim = " FLOAT3 "(attrib[5].x >= 0.75, attrib[5].y >= 0.75, attrib[5].z >= 0.75);\n"
	"temp.xyz = " FLOAT3 "(attrib[5].x < 0.25, attrib[5].y < 0.25, attrib[5].z < 0.25);\n"
	
	"prim = prim * param[" VERTEX_PARAM_TERRAIN_PARAMETER0 "].xyz + temp.xyz * param[" VERTEX_PARAM_TERRAIN_PARAMETER1 "].xyz;\n"
	"prim.x = max(max(prim.x, prim.y), prim.z);\n"
	"opos.xyz = attrib[0].xyz * prim.x + attrib[1].xyz * (1.0 - prim.x);\n"
};

const VertexSnippet VertexProgram::calculateWaterHeightPosition =
{
	'WHTP', kVertexSnippetPositionFlag,
	
	"TEMP		opos;\n"
	
	"MOV		opos.xy, vertex.attrib[0];\n"
	"MOV		opos.z, vertex.attrib[2].w;\n",
	
	FLOAT4 " opos;\n"
	
	"opos.xy = attrib[0].xy;\n"
	"opos.z = attrib[2].w;\n"
};

const VertexSnippet VertexProgram::texcoordVertexTransform =
{
	'TXVT', 0,
	
	"DPH		result.position.x, vertex.attrib[8], program.env[" VERTEX_PARAM_MATRIX_MVP0 "];\n"
	"DPH		result.position.y, vertex.attrib[8], program.env[" VERTEX_PARAM_MATRIX_MVP1 "];\n"
	"DPH		result.position.z, vertex.attrib[8], program.env[" VERTEX_PARAM_MATRIX_MVP2 "];\n"
	"DPH		result.position.w, vertex.attrib[8], program.env[" VERTEX_PARAM_MATRIX_MVP3 "];\n",
	
	RESULT_POSITION ".x = dot(attrib[8].xyz, param[" VERTEX_PARAM_MATRIX_MVP0 "].xyz) + param[" VERTEX_PARAM_MATRIX_MVP0 "].w;\n"
	RESULT_POSITION ".y = dot(attrib[8].xyz, param[" VERTEX_PARAM_MATRIX_MVP1 "].xyz) + param[" VERTEX_PARAM_MATRIX_MVP1 "].w;\n"
	RESULT_POSITION ".z = dot(attrib[8].xyz, param[" VERTEX_PARAM_MATRIX_MVP2 "].xyz) + param[" VERTEX_PARAM_MATRIX_MVP2 "].w;\n"
	RESULT_POSITION ".w = dot(attrib[8].xyz, param[" VERTEX_PARAM_MATRIX_MVP3 "].xyz) + param[" VERTEX_PARAM_MATRIX_MVP3 "].w;\n"
};

const VertexSnippet VertexProgram::extractGlowTransform =
{
	'EXGT', 0,
	
	#if !C4PLAYSTATION3
	
		"MOV		result.position, vertex.attrib[0];\n"
		"ADD		temp.xy, vertex.attrib[0], {1.0, 1.0, 0.0, 0.0};\n"
		"MAD		result.texcoord.xy, temp, program.env[" VERTEX_PARAM_VIEWPORT_TRANSFORM "], program.env[" VERTEX_PARAM_VIEWPORT_TRANSFORM "].zwzw;\n",
		
		RESULT_POSITION " = attrib[0];\n"
		RESULT_TEXCOORD0 ".xy = " FLOAT2 "(attrib[0].x + 1.0, attrib[0].y + 1.0) * param[" VERTEX_PARAM_VIEWPORT_TRANSFORM "].xy + param[" VERTEX_PARAM_VIEWPORT_TRANSFORM "].zw;\n"
	
	#else
	
		"MOV		result.position, vertex.attrib[0];\n"
		"ADD		temp.xy, vertex.attrib[0], {1.0, 0.0, 0.0, 0.0};\n"
		"MAD		result.texcoord.xy, temp, program.env[" VERTEX_PARAM_VIEWPORT_TRANSFORM "], program.env[" VERTEX_PARAM_VIEWPORT_TRANSFORM "].zwzw;\n",
		
		RESULT_POSITION " = attrib[0];\n"
		RESULT_TEXCOORD0 ".xy = " FLOAT2 "(attrib[0].x + 1.0, attrib[0].y) * param[" VERTEX_PARAM_VIEWPORT_TRANSFORM "].xy + param[" VERTEX_PARAM_VIEWPORT_TRANSFORM "].zw;\n"
	
	#endif
};

const VertexSnippet VertexProgram::postProcessTransform =
{
	'PSTP', 0,
	
	"MOV		result.position, vertex.attrib[0];\n"
	"ADD		temp.xy, vertex.attrib[0], {1.0, 1.0, 0.0, 0.0};\n"
	"MUL		temp.zw, program.env[" VERTEX_PARAM_VIEWPORT_TRANSFORM "].xyxy, 0.25;\n"
	"MAD		result.texcoord.xy, temp, temp.zwzw, program.env[" VERTEX_PARAM_VIEWPORT_TRANSFORM "].zwzw;\n",
	
	RESULT_POSITION " = attrib[0];\n"
	RESULT_TEXCOORD0 ".xy = " FLOAT2 "(attrib[0].x + 1.0, attrib[0].y + 1.0) * (param[" VERTEX_PARAM_VIEWPORT_TRANSFORM "].xy * 0.25) + param[" VERTEX_PARAM_VIEWPORT_TRANSFORM "].zw;\n"
};

const VertexSnippet VertexProgram::shadowInfiniteExtrusionTransform =
{
	'SIET', 0,
	
	"TEMP		opos;\n"
	
	"SLT		temp.w, vertex.attrib[0].w, 0.5;\n"
	"ADD		opos, vertex.attrib[0], program.env[" VERTEX_PARAM_LIGHT_POSITION "];\n"
	"MAD		opos, opos, -temp.w, vertex.attrib[0];\n"
	
	"DP4		result.position.x, opos, program.env[" VERTEX_PARAM_MATRIX_MVP0 "];\n"
	"DP4		result.position.y, opos, program.env[" VERTEX_PARAM_MATRIX_MVP1 "];\n"
	"DP4		result.position.z, opos, program.env[" VERTEX_PARAM_MATRIX_MVP2 "];\n"
	"DP4		result.position.w, opos, program.env[" VERTEX_PARAM_MATRIX_MVP3 "];\n",
	
	"temp.w = (attrib[0].w < 0.5);\n"
	FLOAT4 " opos = attrib[0] - (attrib[0] + param[" VERTEX_PARAM_LIGHT_POSITION "]) * temp.w;\n"
	
	RESULT_POSITION ".x = dot(opos, param[" VERTEX_PARAM_MATRIX_MVP0 "]);\n"
	RESULT_POSITION ".y = dot(opos, param[" VERTEX_PARAM_MATRIX_MVP1 "]);\n"
	RESULT_POSITION ".z = dot(opos, param[" VERTEX_PARAM_MATRIX_MVP2 "]);\n"
	RESULT_POSITION ".w = dot(opos, param[" VERTEX_PARAM_MATRIX_MVP3 "]);\n"
};

const VertexSnippet VertexProgram::shadowPointExtrusionTransform =
{
	'SPET', 0,
	
	"TEMP		opos;\n"
	
	"SLT		temp.w, vertex.attrib[0].w, 0.5;\n"
	"MAD		opos.xyz, program.env[" VERTEX_PARAM_LIGHT_POSITION "], -temp.w, vertex.attrib[0];\n"
	"MOV		opos.w, vertex.attrib[0].w;\n"
	
	"DP4		result.position.x, opos, program.env[" VERTEX_PARAM_MATRIX_MVP0 "];\n"
	"DP4		result.position.y, opos, program.env[" VERTEX_PARAM_MATRIX_MVP1 "];\n"
	"DP4		result.position.z, opos, program.env[" VERTEX_PARAM_MATRIX_MVP2 "];\n"
	"DP4		result.position.w, opos, program.env[" VERTEX_PARAM_MATRIX_MVP3 "];\n",
	
	FLOAT4 " opos;\n"
	
	"temp.w = (attrib[0].w < 0.5);\n"
	"opos.xyz = attrib[0].xyz - param[" VERTEX_PARAM_LIGHT_POSITION "].xyz * temp.w;\n"
	"opos.w = attrib[0].w;\n"
	
	RESULT_POSITION ".x = dot(opos, param[" VERTEX_PARAM_MATRIX_MVP0 "]);\n"
	RESULT_POSITION ".y = dot(opos, param[" VERTEX_PARAM_MATRIX_MVP1 "]);\n"
	RESULT_POSITION ".z = dot(opos, param[" VERTEX_PARAM_MATRIX_MVP2 "]);\n"
	RESULT_POSITION ".w = dot(opos, param[" VERTEX_PARAM_MATRIX_MVP3 "]);\n"
};

const VertexSnippet VertexProgram::shadowEndcapProjectionTransform =
{
	'SBCT', 0,
	
	"TEMP		opos;\n"
	
	"SUB		opos.xyz, vertex.attrib[0], program.env[" VERTEX_PARAM_LIGHT_POSITION "];\n"
	"DP3		result.position.x, opos, program.env[" VERTEX_PARAM_MATRIX_MVP0 "];\n"
	"DP3		result.position.y, opos, program.env[" VERTEX_PARAM_MATRIX_MVP1 "];\n"
	"DP3		result.position.z, opos, program.env[" VERTEX_PARAM_MATRIX_MVP2 "];\n"
	"DP3		result.position.w, opos, program.env[" VERTEX_PARAM_MATRIX_MVP3 "];\n",
	
	FLOAT4 " opos;\n"
	
	"opos.xyz = attrib[0].xyz - param[" VERTEX_PARAM_LIGHT_POSITION "].xyz;\n"
	RESULT_POSITION ".x = dot(opos.xyz, param[" VERTEX_PARAM_MATRIX_MVP0 "].xyz);\n"
	RESULT_POSITION ".y = dot(opos.xyz, param[" VERTEX_PARAM_MATRIX_MVP1 "].xyz);\n"
	RESULT_POSITION ".z = dot(opos.xyz, param[" VERTEX_PARAM_MATRIX_MVP2 "].xyz);\n"
	RESULT_POSITION ".w = dot(opos.xyz, param[" VERTEX_PARAM_MATRIX_MVP3 "].xyz);\n"
};

const VertexSnippet VertexProgram::outputPrimaryColor =
{
	'PCOL', 0,
	
	"MOV		result.color, vertex.attrib[3];\n",
	
	RESULT_COLOR0 " = attrib[3];\n"
};

const VertexSnippet VertexProgram::outputSecondaryColor =
{
	'SCOL', 0,
	
	"MOV		result.color.secondary, vertex.attrib[4];\n",
	
	RESULT_COLOR1 " = attrib[4];\n"
};

const VertexSnippet VertexProgram::outputPointSize =
{
	'PSIZ', 0,
	
	#if !C4PLAYSTATION3
	
		"DPH		temp.w, %OPOS, program.env[" VERTEX_PARAM_POINT_CAMERA_PLANE "];\n"
		"RCP		temp.w, temp.w;\n"
		"MUL		result.pointsize, temp.w, vertex.attrib[6].x;\n",
		
		RESULT_POINTSIZE " = attrib[6].x / (%OPOS.xyz * param[" VERTEX_PARAM_POINT_CAMERA_PLANE "].xyz + param[" VERTEX_PARAM_POINT_CAMERA_PLANE "].w);\n"
	
	#else
	
		"DPH		temp.w, %OPOS, program.env[" VERTEX_PARAM_POINT_CAMERA_PLANE "];\n"
		"RCP		temp.w, temp.w;\n"
		"MUL		temp.w, temp.w, vertex.attrib[6].x;\n"
		"MAX		result.pointsize, temp.w, 0.125;\n",
		
		RESULT_POINTSIZE " = max(attrib[6].x / (%OPOS.xyz * param[" VERTEX_PARAM_POINT_CAMERA_PLANE "].xyz + param[" VERTEX_PARAM_POINT_CAMERA_PLANE "].w), 0.125);\n"
	
	#endif
};

const VertexSnippet VertexProgram::outputInfinitePointSize =
{
	'IPSZ', 0,
	
	"MUL		result.pointsize, vertex.attrib[6].x, program.env[" VERTEX_PARAM_RADIUS_POINT_FACTOR "].x;\n",
	
	RESULT_POINTSIZE " = attrib[6].x * param[" VERTEX_PARAM_RADIUS_POINT_FACTOR "].x;\n"
};

const VertexSnippet VertexProgram::copyPrimaryTexcoord0 =
{
	'CPT0', 0,
	
	"MOV		$TEX0, vertex.attrib[8].xyxy;\n",
	
	"$TEX0 = attrib[8].xy;\n"
};

const VertexSnippet VertexProgram::copyPrimaryTexcoord1 =
{
	'CPT1', 0,
	
	"MOV		$TEX1, vertex.attrib[8].xyxy;\n",
	
	"$TEX1 = attrib[8].xy;\n"
};

const VertexSnippet VertexProgram::copySecondaryTexcoord1 =
{
	'CST1', 0,
	
	"MOV		$TEX1, vertex.attrib[9].xyxy;\n",
	
	"$TEX1 = attrib[9].xy;\n"
};

const VertexSnippet VertexProgram::transformPrimaryTexcoord0 =
{
	'TPT0', 0,
	
	"MAD		$TEX0, vertex.attrib[8].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].zwzw;\n",
	
	"$TEX0 = attrib[8].xy * param[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].xy + param[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].zw;\n"
};

const VertexSnippet VertexProgram::transformPrimaryTexcoord1 =
{
	'TPT1', 0,
	
	"MAD		$TEX1, vertex.attrib[8].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zwzw;\n",
	
	"$TEX1 = attrib[8].xy * param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].xy + param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zw;\n"
};

const VertexSnippet VertexProgram::transformSecondaryTexcoord1 =
{
	'TST1', 0,
	
	"MAD		$TEX1, vertex.attrib[9].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zwzw;\n",
	
	"$TEX1 = attrib[9].xy * param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].xy + param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zw;\n"
};

const VertexSnippet VertexProgram::animatePrimaryTexcoord0 =
{
	'APT0', 0,
	
	"MAD		$TEX0, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY0 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, vertex.attrib[8].xyxy;\n",
	
	"$TEX0 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY0 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + attrib[8].xy;\n"
};

const VertexSnippet VertexProgram::animatePrimaryTexcoord1 =
{
	'APT1', 0,
	
	"MAD		$TEX1, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, vertex.attrib[8].xyxy;\n",
	
	"$TEX1 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + attrib[8].xy;\n"
};

const VertexSnippet VertexProgram::animateSecondaryTexcoord1 =
{
	'AST1', 0,
	
	"MAD		$TEX1, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, vertex.attrib[9].xyxy;\n",
	
	"$TEX1 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + attrib[9].xy;\n"
};

const VertexSnippet VertexProgram::transformAnimatePrimaryTexcoord0 =
{
	'XPT0', 0,
	
	"MAD		temp.xy, vertex.attrib[8], program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "], program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].zwzw;\n"
	"MAD		$TEX0, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY0 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, temp.xyxy;\n",
	
	"temp.xy = attrib[8].xy * param[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].xy + param[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].zw;\n"
	"$TEX0 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY0 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + temp.xy;\n"
};

const VertexSnippet VertexProgram::transformAnimatePrimaryTexcoord1 =
{
	'XPT1', 0,
	
	"MAD		temp.xy, vertex.attrib[8], program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "], program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zwzw;\n"
	"MAD		$TEX1, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, temp.xyxy;\n",
	
	"temp.xy = attrib[8].xy * param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].xy + param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zw;\n"
	"$TEX1 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + temp.xy;\n"
};

const VertexSnippet VertexProgram::transformAnimateSecondaryTexcoord1 =
{
	'XST1', 0,
	
	"MAD		temp.xy, vertex.attrib[9], program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "], program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zwzw;\n"
	"MAD		$TEX1, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, temp.xyxy;\n",
	
	"temp.xy = attrib[9].xy * param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].xy + param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zw;\n"
	"$TEX1 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + temp.xy;\n"
};

const VertexSnippet VertexProgram::generateTexcoord0 =
{
	'GTX0', 0,
	
	"MUL		$TEX0, vertex.attrib[0].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_GENERATE "].xyxy;\n",
	
	"$TEX0 = attrib[0].xy * param[" VERTEX_PARAM_TEXCOORD_GENERATE "].xy;\n"
};

const VertexSnippet VertexProgram::generateTexcoord1 =
{
	'GTX1', 0,
	
	"MUL		$TEX1, vertex.attrib[0].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_GENERATE "].xyxy;\n",
	
	"$TEX1 = attrib[0].xy * param[" VERTEX_PARAM_TEXCOORD_GENERATE "].xy;\n"
};

const VertexSnippet VertexProgram::generateTransformTexcoord0 =
{
	'GTT0', 0,
	
	"MAD		$TEX0, vertex.attrib[0].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].zwzw;\n",
	
	"$TEX0 = attrib[0].xy * param[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].xy + param[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].zw;\n"
};

const VertexSnippet VertexProgram::generateTransformTexcoord1 =
{
	'GTT1', 0,
	
	"MAD		$TEX1, vertex.attrib[0].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].xyxy, program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zwzw;\n",
	
	"$TEX1 = attrib[0].xy * param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].xy + param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zw;\n"
};

const VertexSnippet VertexProgram::generateBaseTexcoord =
{
	'GBTC', 0,
	
	"TEMP		btex;\n"
	
	"MUL		btex.xy, vertex.attrib[0], program.env[" VERTEX_PARAM_TEXCOORD_GENERATE "];\n",
	
	FLOAT2 " btex = attrib[0].xy * param[" VERTEX_PARAM_TEXCOORD_GENERATE "].xy;\n"
};

const VertexSnippet VertexProgram::generateAnimateTexcoord0 =
{
	'GAT0', 0,
	
	"MAD		$TEX0, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY0 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, btex.xyxy;\n",
	
	"$TEX0 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY0 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + btex;\n"
};

const VertexSnippet VertexProgram::generateAnimateTexcoord1 =
{
	'GAT1', 0,
	
	"MAD		$TEX1, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, btex.xyxy;\n",
	
	"$TEX1 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + btex;\n"
};

const VertexSnippet VertexProgram::generateTransformAnimateTexcoord0 =
{
	'GXT0', 0,
	
	"MAD		temp.xy, vertex.attrib[0], program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "], program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].zwzw;\n"
	"MAD		$TEX0, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY0 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, temp.xyxy;\n",
	
	"temp.xy = attrib[0].xy * param[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].xy + param[" VERTEX_PARAM_TEXCOORD_TRANSFORM0 "].zw;\n"
	"$TEX0 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY0 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + temp.xy;\n"
};

const VertexSnippet VertexProgram::generateTransformAnimateTexcoord1 =
{
	'GXT1', 0,
	
	"MAD		temp.xy, vertex.attrib[0], program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "], program.env[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zwzw;\n"
	"MAD		$TEX1, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, temp.xyxy;\n",
	
	"temp.xy = attrib[0].xy * param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].xy + param[" VERTEX_PARAM_TEXCOORD_TRANSFORM1 "].zw;\n"
	"$TEX0 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + temp.xy;\n"
};

const VertexSnippet VertexProgram::normalizeNormal =
{
	'NMZN', kVertexSnippetNormalFlag,
	
	"TEMP		nrml;\n"
	
	"DP3		nrml.w, vertex.attrib[2], vertex.attrib[2];\n"
	"RSQ		nrml.w, nrml.w;\n"
	"MUL		nrml.xyz, vertex.attrib[2], nrml.w;\n",
	
	FLOAT3 " nrml = normalize(attrib[2].xyz);\n"
};

const VertexSnippet VertexProgram::normalizeTangent =
{
	'NMZT', kVertexSnippetTangentFlag,
	
	"TEMP		tang;\n"
	
	"DP3		tang.w, vertex.attrib[6], vertex.attrib[6];\n"
	"RSQ		tang.w, tang.w;\n"
	"MUL		tang.xyz, vertex.attrib[6], tang.w;\n",
	
	FLOAT3 " tang = normalize(attrib[6].xyz);\n"
};

const VertexSnippet VertexProgram::orthonormalizeTangent =
{
	'ONMT', kVertexSnippetTangentFlag,
	
	"TEMP		tang;\n"
	
	"DP3		temp.w, %NRML, vertex.attrib[6];\n"
	"MAD		temp.xyz, %NRML, -temp.w, vertex.attrib[6];\n"
	"DP3		tang.w, temp, temp;\n"
	"RSQ		tang.w, tang.w;\n"
	"MUL		tang.xyz, temp, tang.w;\n",
	
	FLOAT3 " tang = normalize(attrib[6].xyz - %NRML.xyz * dot(%NRML.xyz, attrib[6].xyz));\n"
};

const VertexSnippet VertexProgram::generateTangent =
{
	'NMTG', kVertexSnippetTangentFlag,
	
	"TEMP		tang;\n"
	
	"MUL		temp, %NRML.zzxx, {1.0, 0.0, -1.0, 0.0};\n"
	"DP3		tang.w, temp, temp;\n"
	"RSQ		tang.w, tang.w;\n"
	"MUL		tang.xyz, temp, tang.w;\n",
	
	"temp.w = " RSQRT "(%NRML.x * %NRML.x + %NRML.z * %NRML.z);\n"
	FLOAT3 " tang = " FLOAT3 "(%NRML.z * temp.w, 0.0, %NRML.x * -temp.w);\n"
};

const VertexSnippet VertexProgram::generateImpostorFrame =
{
	'IFRM', kVertexSnippetNormalFlag | kVertexSnippetTangentFlag,
	
	"TEMP		nrml, tang, btng;\n"
	
	"MOV		nrml.z, 0.0;\n"
	"SUB		nrml.xy, program.env[" VERTEX_PARAM_CAMERA_POSITION "], vertex.attrib[0];\n"
	"MUL		temp.w, nrml.x, nrml.x;\n"
	"MAD		temp.w, nrml.y, nrml.y, temp.w;\n"
	"RSQ		temp.w, temp.w;\n"
	"MUL		nrml.xy, nrml, temp.w;\n"
	
	"MUL		tang.xyz, nrml.yxzw, {-1.0, 1.0, 0.0, 0.0};\n"
	"MOV		btng, {0.0, 0.0, 1.0, 0.0};\n",
	
	FLOAT3 " nrml;\n"
	
	"nrml.z = 0.0;\n"
	"nrml.xy = param[" VERTEX_PARAM_CAMERA_POSITION "].xy - attrib[0].xy;\n"
	"nrml.xy *= " RSQRT "(nrml.x * nrml.x + nrml.y * nrml.y);\n"
	
	FLOAT3 " tang = " FLOAT3 "(-nrml.y, nrml.x, 0.0);\n"
	FLOAT3 " btng = " FLOAT3 "(0.0, 0.0, 1.0);\n"
};

const VertexSnippet VertexProgram::calculateBitangent =
{
	'CBTN', 0,
	
	"TEMP		btng;\n"
	
	"XPD		btng.xyz, %NRML, %TANG;\n",
	
	FLOAT3 " btng = cross(%NRML.xyz, %TANG.xyz);\n"
};

const VertexSnippet VertexProgram::adjustBitangent =
{
	'ABTN', 0,
	
	"MUL		btng.xyz, btng, vertex.attrib[6].w;\n",
	
	"btng *= attrib[6].w;\n"
};

const VertexSnippet VertexProgram::vertexSnippet[kVertexSnippetCount] =
{
	// kVertexSnippetOutputObjectPosition
	{
		'POSI', 0,
		
		"MOV		$POSI.xyz, %OPOS;\n",
		
		"$POSI.xyz = %OPOS.xyz;\n"
	},
	
	// kVertexSnippetOutputObjectNormal
	{
		'NRML', 0,
		
		"MOV		$NRML.xyz, %NRML;\n",
		
		"$NRML.xyz = %NRML.xyz;\n"
	},
	
	// kVertexSnippetOutputObjectTangent
	{
		'TANG', 0,
		
		"MOV		$TANG.xyz, %TANG;\n",
		
		"$TANG.xyz = %TANG.xyz;\n"
	},
	
	// kVertexSnippetOutputObjectBitangent
	{
		'BTNG', 0,
		
		"MOV		$BTNG.xyz, btng;\n",
		
		"$BTNG.xyz = btng;\n"
	},
	
	// kVertexSnippetOutputWorldPosition
	{
		'WPOS', 0,
		
		"DP4		$WPOS.x, program.env[" VERTEX_PARAM_MATRIX_WORLD0 "], %OPOS;\n"
		"DP4		$WPOS.y, program.env[" VERTEX_PARAM_MATRIX_WORLD1 "], %OPOS;\n"
		"DP4		$WPOS.z, program.env[" VERTEX_PARAM_MATRIX_WORLD2 "], %OPOS;\n",
		
		"$WPOS.x = dot(param[" VERTEX_PARAM_MATRIX_WORLD0 "], %OPOS);\n"
		"$WPOS.y = dot(param[" VERTEX_PARAM_MATRIX_WORLD1 "], %OPOS);\n"
		"$WPOS.z = dot(param[" VERTEX_PARAM_MATRIX_WORLD2 "], %OPOS);\n"
	},
	
	// kVertexSnippetOutputWorldNormal
	{
		'WNRM', 0,
		
		"DP3		$WNRM.x, program.env[" VERTEX_PARAM_MATRIX_WORLD0 "], %NRML;\n"
		"DP3		$WNRM.y, program.env[" VERTEX_PARAM_MATRIX_WORLD1 "], %NRML;\n"
		"DP3		$WNRM.z, program.env[" VERTEX_PARAM_MATRIX_WORLD2 "], %NRML;\n",
		
		"$WNRM.x = dot(param[" VERTEX_PARAM_MATRIX_WORLD0 "].xyz, %NRML.xyz);\n"
		"$WNRM.y = dot(param[" VERTEX_PARAM_MATRIX_WORLD1 "].xyz, %NRML.xyz);\n"
		"$WNRM.z = dot(param[" VERTEX_PARAM_MATRIX_WORLD2 "].xyz, %NRML.xyz);\n"
	},
	
	// kVertexSnippetOutputWorldTangent
	{
		'WTAN', 0,
		
		"DP3		$WTAN.x, program.env[" VERTEX_PARAM_MATRIX_WORLD0 "], %TANG;\n"
		"DP3		$WTAN.y, program.env[" VERTEX_PARAM_MATRIX_WORLD1 "], %TANG;\n"
		"DP3		$WTAN.z, program.env[" VERTEX_PARAM_MATRIX_WORLD2 "], %TANG;\n",
		
		"$WTAN.x = dot(param[" VERTEX_PARAM_MATRIX_WORLD0 "].xyz, %TANG.xyz);\n"
		"$WTAN.y = dot(param[" VERTEX_PARAM_MATRIX_WORLD1 "].xyz, %TANG.xyz);\n"
		"$WTAN.z = dot(param[" VERTEX_PARAM_MATRIX_WORLD2 "].xyz, %TANG.xyz);\n"
	},
	
	// kVertexSnippetOutputWorldBitangent
	{
		'WBTN', 0,
		
		"DP3		$WBTN.x, program.env[" VERTEX_PARAM_MATRIX_WORLD0 "], btng;\n"
		"DP3		$WBTN.y, program.env[" VERTEX_PARAM_MATRIX_WORLD1 "], btng;\n"
		"DP3		$WBTN.z, program.env[" VERTEX_PARAM_MATRIX_WORLD2 "], btng;\n",
		
		"$WBTN.x = dot(param[" VERTEX_PARAM_MATRIX_WORLD0 "].xyz, btng);\n"
		"$WBTN.y = dot(param[" VERTEX_PARAM_MATRIX_WORLD1 "].xyz, btng);\n"
		"$WBTN.z = dot(param[" VERTEX_PARAM_MATRIX_WORLD2 "].xyz, btng);\n"
	},
	
	// kVertexSnippetOutputCameraNormal
	{
		'NRMC', 0,
		
		"DP3		$NRMC.x, program.env[" VERTEX_PARAM_MATRIX_CAMERA0 "], %NRML;\n"
		"DP3		$NRMC.y, program.env[" VERTEX_PARAM_MATRIX_CAMERA1 "], %NRML;\n"
		"DP3		$NRMC.z, program.env[" VERTEX_PARAM_MATRIX_CAMERA2 "], %NRML;\n",
		
		"$NRMC.x = dot(param[" VERTEX_PARAM_MATRIX_CAMERA0 "].xyz, %NRML.xyz);\n"
		"$NRMC.y = dot(param[" VERTEX_PARAM_MATRIX_CAMERA1 "].xyz, %NRML.xyz);\n"
		"$NRMC.z = dot(param[" VERTEX_PARAM_MATRIX_CAMERA2 "].xyz, %NRML.xyz);\n"
	},
	
	// kVertexSnippetOutputVertexGeometry
	{
		'GEOM', 0,
		
		"MOV		$GEOM.xy, vertex.attrib[1];\n"
		"SUB		$GEOM.z, vertex.attrib[1], %OPOS;\n",
		
		"$GEOM.xy = attrib[1].xy;\n"
		"$GEOM.z = attrib[1].z - %OPOS.z;\n"
	},
	
	// kVertexSnippetOutputObjectInfiniteLightDirection
	{
		'OOIL', 0,
		
		"MOV		$OLDR.xyz, program.env[" VERTEX_PARAM_LIGHT_POSITION "];\n",
		
		"$OLDR.xyz = param[" VERTEX_PARAM_LIGHT_POSITION "].xyz;\n"
	},
	
	// kVertexSnippetCalculateObjectPointLightDirection
	{
		'COPL', 0,
		
		"TEMP		ldir;\n"
		
		"SUB		ldir.xyz, program.env[" VERTEX_PARAM_LIGHT_POSITION "], %OPOS;\n",
		
		FLOAT3 " ldir = param[" VERTEX_PARAM_LIGHT_POSITION "].xyz - %OPOS.xyz;\n"
	},
	
	// kVertexSnippetOutputObjectPointLightDirection
	{
		'OOPL', 0,
		
		"MOV		$OLDR.xyz, ldir;\n",
		
		"$OLDR.xyz = ldir;\n"
	},
	
	// kVertexSnippetOutputTangentInfiniteLightDirection
	{
		'OTIL', 0,
		
		"DP3		$LDIR.x, %TANG, program.env[" VERTEX_PARAM_LIGHT_POSITION "];\n"
		"DP3		$LDIR.y, btng, program.env[" VERTEX_PARAM_LIGHT_POSITION "];\n"
		"DP3		$LDIR.z, %NRML, program.env[" VERTEX_PARAM_LIGHT_POSITION "];\n",
		
		"$LDIR.x = dot(%TANG.xyz, param[" VERTEX_PARAM_LIGHT_POSITION "].xyz);\n"
		"$LDIR.y = dot(btng, param[" VERTEX_PARAM_LIGHT_POSITION "].xyz);\n"
		"$LDIR.z = dot(%NRML.xyz, param[" VERTEX_PARAM_LIGHT_POSITION "].xyz);\n"
	},
	
	// kVertexSnippetOutputTangentPointLightDirection
	{
		'OTPL', 0,
		
		"DP3		$LDIR.x, %TANG, ldir;\n"
		"DP3		$LDIR.y, btng, ldir;\n"
		"DP3		$LDIR.z, %NRML, ldir;\n",
		
		"$LDIR.x = dot(%TANG.xyz, ldir);\n"
		"$LDIR.y = dot(btng, ldir);\n"
		"$LDIR.z = dot(%NRML.xyz, ldir);\n"
	},
	
	// kVertexSnippetCalculateObjectViewDirection
	{
		'COVD', 0,
		
		"TEMP		vdir;\n"
		
		"SUB		vdir.xyz, program.env[" VERTEX_PARAM_CAMERA_POSITION "], %OPOS;\n",
		
		FLOAT3 " vdir = param[" VERTEX_PARAM_CAMERA_POSITION "].xyz - %OPOS.xyz;\n"
	},
	
	// kVertexSnippetOutputObjectViewDirection
	{
		'OOVD', 0,
		
		"MOV		$OVDR.xyz, vdir;\n",
		
		"$OVDR.xyz = vdir;\n"
	},
	
	// kVertexSnippetOutputTangentViewDirection
	{
		'OTVD', 0,
		
		"DP3		$VDIR.x, %TANG, vdir;\n"
		"DP3		$VDIR.y, btng, vdir;\n"
		"DP3		$VDIR.z, %NRML, vdir;\n",
		
		"$VDIR.x = dot(%TANG.xyz, vdir);\n"
		"$VDIR.y = dot(btng, vdir);\n"
		"$VDIR.z = dot(%NRML.xyz, vdir);\n"
	},
	
	// kVertexSnippetOutputTangentViewFogDirection
	{
		'TVFD', 0,
		
		"MUL		temp.xyz, vdir, program.env[" VERTEX_PARAM_FOG_PARAMS "].w;\n"
		"DP3		$VDIR.x, %TANG, temp;\n"
		"DP3		$VDIR.y, btng, temp;\n"
		"DP3		$VDIR.z, %NRML, temp;\n",
		
		"temp.xyz = vdir * param[" VERTEX_PARAM_FOG_PARAMS "].w;\n"
		"$VDIR.x = dot(%TANG.xyz, temp.xyz);\n"
		"$VDIR.y = dot(btng, temp.xyz);\n"
		"$VDIR.z = dot(%NRML.xyz, temp.xyz);\n"
	},
	
	// kVertexSnippetOutputAlternateViewFogDirection
	{
		'OAVF', 0,
		
		"MUL		$VDIR.xyz, vdir, program.env[" VERTEX_PARAM_FOG_PARAMS "].w;\n",
		
		"$VDIR.xyz = vdir * param[" VERTEX_PARAM_FOG_PARAMS "].w;\n"
	},
	
	// kVertexSnippetOutputBillboardInfiniteLightDirection
	{
		'OBIL', 0,
		
		"DP3		temp.w, vdir, vdir;\n"
		"RSQ		temp.w, temp.w;\n"
		"MUL		temp.xyz, vdir, temp.w;\n"
		"DP3		$LDIR.x, tang, program.env[" VERTEX_PARAM_LIGHT_POSITION "];\n"
		"DP3		$LDIR.y, btng, program.env[" VERTEX_PARAM_LIGHT_POSITION "];\n"
		"DP3		$LDIR.z, temp, program.env[" VERTEX_PARAM_LIGHT_POSITION "];\n",
		
		"temp.xyz = normalize(vdir);\n"
		"$LDIR.x = dot(tang, param[" VERTEX_PARAM_LIGHT_POSITION "].xyz);\n"
		"$LDIR.y = dot(btng, param[" VERTEX_PARAM_LIGHT_POSITION "].xyz);\n"
		"$LDIR.z = dot(temp, param[" VERTEX_PARAM_LIGHT_POSITION "].xyz);\n"
	},
	
	// kVertexSnippetOutputBillboardPointLightDirection
	{
		'OBPL', 0,
		
		"DP3		temp.w, vdir, vdir;\n"
		"RSQ		temp.w, temp.w;\n"
		"MUL		temp.xyz, vdir, temp.w;\n"
		"DP3		$LDIR.x, tang, ldir;\n"
		"DP3		$LDIR.y, btng, ldir;\n"
		"DP3		$LDIR.z, temp, ldir;\n",
		
		"temp.xyz = normalize(vdir);\n"
		"$LDIR.x = dot(tang, ldir);\n"
		"$LDIR.y = dot(btng, ldir);\n"
		"$LDIR.z = dot(temp, ldir);\n"
	},
	
	// kVertexSnippetCalculateTerrainTangentData
	{
		'CTTD', 0,
		
		"TEMP		ttnd;\n"
		
		"MUL		ttnd.xyz, %NRML, %NRML;\n"
		"ADD		ttnd, ttnd.x, ttnd.yzyz;\n"
		"MAX		ttnd.xy, ttnd, 0.03125;\n"
		"RSQ		ttnd.x, ttnd.x;\n"
		"RSQ		ttnd.y, ttnd.y;\n",
		
		FLOAT4 " ttnd;\n"
		
		"ttnd.xyz = %NRML.xyz * %NRML.xyz;\n"
		"ttnd = ttnd.x + ttnd.yzyz;\n"
		"ttnd.xy = max(ttnd.xy, 0.03125);\n"
		"ttnd.x = " RSQRT "(ttnd.x);\n"
		"ttnd.y = " RSQRT "(ttnd.y);\n"
	},
	
	// kVertexSnippetOutputTerrainInfiniteLightDirection
	{
		'TLDI', 0,
		
		"TEMP		ltan;\n"
		
		"XPD		ltan.xyz, program.env[" VERTEX_PARAM_LIGHT_POSITION "], %NRML;\n"
		"MUL		temp.xy, %NRML.xzzz, program.env[" VERTEX_PARAM_LIGHT_POSITION "].yxxx;\n"
		"MAD		temp.xy, -%NRML.yxxx, program.env[" VERTEX_PARAM_LIGHT_POSITION "].xzzz, temp;\n"
		"MUL		temp.zw, %NRML.xxxz, ltan.yyyx;\n"
		"MAD		temp.zw, -%NRML.yyyx, ltan.xxxz, temp.zzzw;\n"
		"MUL		$TLDR.xy, temp.xzzz, ttnd.x;\n"
		"MUL		$TLD2, temp.ywyw, ttnd.y;\n"
		"DP3		$TLDR.z, %NRML, program.env[" VERTEX_PARAM_LIGHT_POSITION "];\n",
		
		FLOAT3 " ltan = cross(param[" VERTEX_PARAM_LIGHT_POSITION "].xyz, %NRML.xyz);\n"
		"temp.xy = %NRML.xz * param[" VERTEX_PARAM_LIGHT_POSITION "].yx - %NRML.yx * param[" VERTEX_PARAM_LIGHT_POSITION "].xz;\n"
		"temp.zw = %NRML.xz * ltan.yx - %NRML.yx * ltan.xz;\n"
		"$TLDR.xy = temp.xz * ttnd.x;\n"
		"$TLD2 = temp.yw * ttnd.y;\n"
		"$TLDR.z = dot(%NRML.xyz, param[" VERTEX_PARAM_LIGHT_POSITION "].xyz);\n"
	},
	
	// kVertexSnippetOutputTerrainPointLightDirection
	{
		'TLDP', 0,
		
		"TEMP		tldp, ltan;\n"
		
		"SUB		tldp.xyz, program.env[" VERTEX_PARAM_LIGHT_POSITION "], %OPOS;\n"
		"XPD		ltan.xyz, tldp, %NRML;\n"
		"MUL		temp.xy, %NRML.xzzz, tldp.yxxx;\n"
		"MAD		temp.xy, -%NRML.yxxx, tldp.xzzz, temp;\n"
		"MUL		temp.zw, %NRML.xxxz, ltan.yyyx;\n"
		"MAD		temp.zw, -%NRML.yyyx, ltan.xxxz, temp.zzzw;\n"
		"MUL		$TLDR.xy, temp.xzzz, ttnd.x;\n"
		"MUL		$TLD2, temp.ywyw, ttnd.y;\n"
		"DP3		$TLDR.z, %NRML, tldp;\n",
		
		FLOAT3 " tldp = param[" VERTEX_PARAM_LIGHT_POSITION "].xyz - %OPOS.xyz;\n"
		FLOAT3 " ltan = cross(tldp, %NRML.xyz);\n"
		"temp.xy = %NRML.xz * tldp.yx - %NRML.yx * tldp.xz;\n"
		"temp.zw = %NRML.xz * ltan.yx - %NRML.yx * ltan.xz;\n"
		"$TLDR.xy = temp.xz * ttnd.x;\n"
		"$TLD2 = temp.yw * ttnd.y;\n"
		"$TLDR.z = dot(%NRML.xyz, tldp);\n"
	},
	
	// kVertexSnippetOutputTerrainViewDirection
	{
		'TVDR', 0,
		
		"TEMP		tvdp, vtan;\n"
		
		"SUB		tvdp.xyz, program.env[" VERTEX_PARAM_CAMERA_POSITION "], %OPOS;\n"
		"XPD		vtan.xyz, tvdp, %NRML;\n"
		"MUL		temp.xy, %NRML.xzzz, tvdp.yxxx;\n"
		"MAD		temp.xy, -%NRML.yxxx, tvdp.xzzz, temp;\n"
		"MUL		temp.zw, %NRML.xxxz, vtan.yyyx;\n"
		"MAD		temp.zw, -%NRML.yyyx, vtan.xxxz, temp.zzzw;\n"
		"MUL		$TVDR.xy, temp.xzzz, ttnd.x;\n"
		"MUL		$TVD2, temp.ywyw, ttnd.y;\n"
		"DP3		$TVDR.z, %NRML, tvdp;\n",
		
		FLOAT3 " tvdp = param[" VERTEX_PARAM_CAMERA_POSITION "].xyz - %OPOS.xyz;\n"
		FLOAT3 " vtan = cross(tvdp, %NRML.xyz);\n"
		"temp.xy = %NRML.xz * tvdp.yx - %NRML.yx * tvdp.xz;\n"
		"temp.zw = %NRML.xz * vtan.yx - %NRML.yx * vtan.xz;\n"
		"$TVDR.xy = temp.xz * ttnd.x;\n"
		"$TVD2 = temp.yw * ttnd.y;\n"
		"$TVDR.z = dot(%NRML.xyz, tvdp);\n"
	},
	
	// kVertexSnippetOutputTerrainWorldTangentFrame
	{
		'TWTF', 0,
		
		"TEMP		wbtn;\n"
		
		"DP3		temp.x, program.env[" VERTEX_PARAM_MATRIX_WORLD0 "], %NRML;\n"
		"DP3		temp.y, program.env[" VERTEX_PARAM_MATRIX_WORLD1 "], %NRML;\n"
		"DP3		temp.z, program.env[" VERTEX_PARAM_MATRIX_WORLD2 "], %NRML;\n"
		"MOV		$TWNM, temp;\n"
		
		"MUL		$TWTN.xw, -temp.yyxx, ttnd.xxyy;\n"
		"MUL		$TWTN.yz, temp.xxzz, ttnd.xxyy;\n"
		
		"MUL		wbtn.x, -temp.x, temp.z;\n"
		"MUL		wbtn.y, temp.y, temp.z;\n"
		"MUL		$TWB1.xy, wbtn, ttnd.x;\n"
		"MUL		$TWB1.z, ttnd.z, ttnd.x;\n"
		
		"MUL		wbtn.xz, -temp.xxyy, temp.yyzz;\n"
		"MUL		$TWB2.xz, wbtn, ttnd.y;\n"
		"MUL		$TWB2.y, ttnd.w, ttnd.y;\n",
		
		FLOAT3 " wbtn;\n"
		
		"temp.x = dot(param[" VERTEX_PARAM_MATRIX_WORLD0 "].xyz, %NRML.xyz);\n"
		"temp.y = dot(param[" VERTEX_PARAM_MATRIX_WORLD1 "].xyz, %NRML.xyz);\n"
		"temp.z = dot(param[" VERTEX_PARAM_MATRIX_WORLD2 "].xyz, %NRML.xyz);\n"
		"$TWNM = temp.xyz;\n"
		
		"$TWTN.xw = -temp.yx * ttnd.xy;\n"
		"$TWTN.yz = temp.xz * ttnd.xy;\n"
		
		"wbtn.xy = " FLOAT2 "(-temp.x * temp.z, temp.y * temp.z);\n"
		"$TWB1.xy = wbtn.xy * ttnd.x;\n"
		"$TWB1.z = ttnd.z * ttnd.x;\n"
		
		"wbtn.xz = -temp.xy * temp.yz;\n"
		"$TWB2.xz = wbtn.xz * ttnd.y;\n"
		"$TWB2.y = ttnd.w * ttnd.y;\n"
	},
	
	// kVertexSnippetOutputRawTexcoords
	{
		'RTXC', 0,
		
		"MOV		$RTXC.xyz, vertex.attrib[8];\n",
		
		"$RTXC.xyz = attrib[8].xyz;\n"
	},
	
	// kVertexSnippetOutputTerrainTexcoords
	{
		'TERA', 0,
		
		"MUL		$TERA.xyz, %OPOS, program.env[" VERTEX_PARAM_TERRAIN_TEXCOORD_SCALE "].x;\n",
		
		"$TERA.xyz = %OPOS.xyz * param[" VERTEX_PARAM_TERRAIN_TEXCOORD_SCALE "].x;\n"
	},
	
	// kVertexSnippetOutputImpostorTexcoords
	{
		'CITX', 0,
		
		"TEMP		idir, itmp;\n"
		
		"MAD		idir.xy, vertex.attrib[0], -program.env[" VERTEX_PARAM_CAMERA_POSITION "].w, program.env[" VERTEX_PARAM_CAMERA_POSITION "];\n"
		"MUL		temp.w, idir.x, idir.x;\n"
		"MAD		temp.w, idir.y, idir.y, temp.w;\n"
		"RSQ		temp.w, temp.w;\n"
		
		"MUL		temp.x, vertex.attrib[6].y, idir.x;\n"
		"MAD		temp.x, vertex.attrib[6].z, idir.y, temp.x;\n"
		"MUL		temp.x, temp.x, temp.w;\n"
		
		"MUL		temp.y, temp.x, temp.x;\n"
		"SUB		temp.z, 2.0, temp.x;\n"
		"MAD		temp.x, temp.x, -temp.y, temp.z;\n"
		
		"MUL		itmp.x, vertex.attrib[6].y, idir.y;\n"
		"MAD		itmp.x, vertex.attrib[6].z, -idir.x, itmp.x;\n"
		"SLT		itmp.y, itmp.x, 0.0;\n"
		
		"MAD		temp.y, temp.x, -2.0, 8.0;\n"
		"MAD		temp.x, temp.y, itmp.y, temp.x;\n"
		
		"FRC		temp.w, temp.x;\n"
		"SUB		temp.x, temp.x, temp.w;\n"
		"MOV		$IBLD, temp.w;\n"
		
		"ADD		temp.x, temp.x, vertex.attrib[8].x;\n"
		"MOV		$IMPT.xy, vertex.attrib[8];\n"
		"MAD		$IMPT.zw, temp.x, {0.125, 0.125, 0.125, 0.125}, {0.0, 0.0, 0.0, 0.125};\n",
		
		FLOAT2 " itmp;\n"
		
		FLOAT2 " idir = attrib[0].xy * -param[" VERTEX_PARAM_CAMERA_POSITION "].w + param[" VERTEX_PARAM_CAMERA_POSITION "].xy;\n"
		
		"temp.x = (attrib[6].y * idir.x + attrib[6].z * idir.y) * " RSQRT "(dot(idir, idir));\n"
		"temp.x = 2.0 - temp.x - temp.x * temp.x * temp.x;\n"
		
		"itmp.x = attrib[6].y * idir.y - attrib[6].z * idir.x;\n"
		"itmp.y = (itmp.x < 0.0);\n"
		"temp.x += (8.0 - temp.x * 2.0) * itmp.y;\n"
		
		"temp.w = " FRAC "(temp.x);\n"
		"$IBLD = temp.w;\n"
		"temp.x = temp.x - temp.w + attrib[8].x;\n"
		"$IMPT.xy = attrib[8].xy;\n"
		"$IMPT.z = temp.x * 0.125;\n"
		"$IMPT.w = temp.x * 0.125 + 0.125;\n"
	},
	
	// kVertexSnippetOutputImpostorTransitionBlend
	{
		'IXBL', 0,
		
		"SUB		temp.xy, vertex.attrib[0], program.env[" VERTEX_PARAM_IMPOSTOR_CAMERA_POSITION "];\n"
		"MUL		temp.w, temp.x, temp.x;\n"
		"MAD		temp.w, temp.y, temp.y, temp.w;\n"
		"RSQ		temp.x, temp.w;\n"
		"MUL		temp.x, temp.x, temp.w;\n"
		"MAD		$IXBL, temp.x, program.env[" VERTEX_PARAM_IMPOSTOR_TRANSITION "].x, program.env[" VERTEX_PARAM_IMPOSTOR_TRANSITION "].y;\n",
		
		"temp.xy = attrib[0].xy - param[" VERTEX_PARAM_IMPOSTOR_CAMERA_POSITION "].xy;\n"
		"temp.w = dot(temp.xy, temp.xy);\n"
		"$IXBL = " RSQRT "(temp.w) * temp.w * param[" VERTEX_PARAM_IMPOSTOR_TRANSITION "].x + param[" VERTEX_PARAM_IMPOSTOR_TRANSITION "].y;"
	},
	
	// kVertexSnippetOutputGeometryImpostorTexcoords
	{
		'GITX', 0,
		
		"DPH		$GITX:x, vertex.attrib[0], program.env[" VERTEX_PARAM_IMPOSTOR_PLANE_S "];\n"
		"DPH		$GITX:y, vertex.attrib[0], program.env[" VERTEX_PARAM_IMPOSTOR_PLANE_T "];\n",
		
		"$GITX:x = dot(attrib[0].xyz, param[" VERTEX_PARAM_IMPOSTOR_PLANE_S "].xyz) + param[" VERTEX_PARAM_IMPOSTOR_PLANE_S "].w;\n"
		"$GITX:y = dot(attrib[0].xyz, param[" VERTEX_PARAM_IMPOSTOR_PLANE_T "].xyz) + param[" VERTEX_PARAM_IMPOSTOR_PLANE_T "].w;\n"
	},
	
	// kVertexSnippetOutputPaintTexcoords
	{
		'PTXC', 0,
		
		"DPH		$PTXC:x, vertex.attrib[0], program.env[" VERTEX_PARAM_PAINT_PLANE_S "];\n"
		"DPH		$PTXC:y, vertex.attrib[0], program.env[" VERTEX_PARAM_PAINT_PLANE_T "];\n",
		
		"$PTXC:x = dot(attrib[0].xyz, param[" VERTEX_PARAM_PAINT_PLANE_S "].xyz) + param[" VERTEX_PARAM_PAINT_PLANE_S "].w;\n"
		"$PTXC:y = dot(attrib[0].xyz, param[" VERTEX_PARAM_PAINT_PLANE_T "].xyz) + param[" VERTEX_PARAM_PAINT_PLANE_T "].w;\n"
	},
	
	// kVertexSnippetOutputFireTexcoords
	{
		'FIRE', 0,
		
		"MOV		$FIRE.xy, vertex.attrib[8];\n"
		"MUL		$FIRE.z, vertex.attrib[8].y, program.env[" VERTEX_PARAM_FIRE_PARAMS "].x;\n"
		
		"ADD		temp.xy, vertex.attrib[8], vertex.attrib[8].zwzw;\n"
		"MAD		$FIR1, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY0 "], program.env[" VERTEX_PARAM_SHADER_TIME "].x, temp.xyxy;\n"
		"MAD		$FIR2, program.env[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xyxy, program.env[" VERTEX_PARAM_SHADER_TIME "].x, temp.xyxy;\n",
		
		"$FIRE.xy = attrib[8].xy;\n"
		"$FIRE.z = attrib[8].y * param[" VERTEX_PARAM_FIRE_PARAMS "].x;\n"
		
		"temp.xy = attrib[8].xy + attrib[8].zw;\n"
		"$FIR1 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY0 "] * param[" VERTEX_PARAM_SHADER_TIME "].x + temp.xyxy;\n"
		"$FIR2 = param[" VERTEX_PARAM_TEXCOORD_VELOCITY1 "].xy * param[" VERTEX_PARAM_SHADER_TIME "].x + temp.xy;\n"
	},
	
	// kVertexSnippetOutputFireArrayTexcoords
	{
		'FIRA', 0,
		
		"MOV		$FIRE.xy, vertex.attrib[8];\n"
		"MUL		$FIRE.z, vertex.attrib[8].y, vertex.attrib[6].y;\n"
		
		"ADD		temp.xy, vertex.attrib[8], vertex.attrib[8].zwzw;\n"
		"MAD		$FIR1, vertex.attrib[9], program.env[" VERTEX_PARAM_SHADER_TIME "].x, temp.xyxy;\n"
		"MAD		$FIR2, vertex.attrib[6].zwzw, program.env[" VERTEX_PARAM_SHADER_TIME "].x, temp.xyxy;\n",
		
		"$FIRE.xy = attrib[8].xy;\n"
		"$FIRE.z = attrib[8].y * attrib[6].y;\n"
		
		"temp.xy = attrib[8].xy + attrib[8].zw;\n"
		"$FIR1 = attrib[9] * param[" VERTEX_PARAM_SHADER_TIME "].x + temp.xyxy;\n"
		"$FIR2 = attrib[6].zw * param[" VERTEX_PARAM_SHADER_TIME "].x + temp.xy;\n"
	},
	
	// kVertexSnippetCalculateCameraDistance
	{
		'CAMD', 0,
		
		"TEMP		dist;\n"
		
		"DP3		dist.w, vdir, vdir;\n"
		"RSQ		dist.z, dist.w;\n"
		"MUL		dist.w, dist.z, dist.w;\n",
		
		"temp.w = dot(vdir, vdir);\n"
		"float dist = " RSQRT "(temp.w) * temp.w;\n"
	},
	
	// kVertexSnippetOutputCameraWarpFunction
	{
		'WARP', 0,
		
		"DP3		$WARP.x, program.env[" VERTEX_PARAM_CAMERA_RIGHT "], %NRML;\n"
		"DP3		$WARP.y, program.env[" VERTEX_PARAM_CAMERA_DOWN "], %NRML;\n"
		
		"MUL		temp.x, dist.w, 8.0;\n"
		"MAD		temp.y, temp.x, dist.w, 4.0;\n"
		"RCP		temp.y, temp.y;\n"
		"MUL		temp.w, temp.x, temp.y;\n"
		"MUL		$WARP.z, temp.w, program.env[" VERTEX_PARAM_REFLECTION_SCALE "].x;\n"
		"MUL		$WARP.w, temp.w, program.env[" VERTEX_PARAM_REFRACTION_SCALE "].x;\n",
		
		"$WARP.x = dot(param[" VERTEX_PARAM_CAMERA_RIGHT "].xyz, %NRML.xyz);\n"
		"$WARP.y = dot(param[" VERTEX_PARAM_CAMERA_DOWN "].xyz, %NRML.xyz);\n"
		
		"temp.x = dist * 8.0;\n"
		"temp.w = temp.x / (temp.x * dist + 4.0);\n"
		"$WARP.z = temp.w * param[" VERTEX_PARAM_REFLECTION_SCALE "].x;\n"
		"$WARP.w = temp.w * param[" VERTEX_PARAM_REFRACTION_SCALE "].x;\n"
	},
	
	// kVertexSnippetOutputCameraBumpWarpFunction
	{
		'BWRP', 0,
		
		"DP3		$RGHT.x, program.env[" VERTEX_PARAM_CAMERA_RIGHT "], %TANG;\n"
		"DP3		$RGHT.y, program.env[" VERTEX_PARAM_CAMERA_RIGHT "], btng;\n"
		"DP3		$RGHT.z, program.env[" VERTEX_PARAM_CAMERA_RIGHT "], %NRML;\n"
		"DP3		$DOWN.x, program.env[" VERTEX_PARAM_CAMERA_DOWN "], %TANG;\n"
		"DP3		$DOWN.y, program.env[" VERTEX_PARAM_CAMERA_DOWN "], btng;\n"
		"DP3		$DOWN.z, program.env[" VERTEX_PARAM_CAMERA_DOWN "], %NRML;\n"
		
		"MUL		temp.x, dist.w, 8.0;\n"
		"MAD		temp.y, temp.x, dist.w, 4.0;\n"
		"RCP		temp.y, temp.y;\n"
		"MUL		temp.w, temp.x, temp.y;\n"
		"MUL		$RGHT.w, temp.w, program.env[" VERTEX_PARAM_REFLECTION_SCALE "].x;\n"
		"MUL		$DOWN.w, temp.w, program.env[" VERTEX_PARAM_REFRACTION_SCALE "].x;\n",
		
		"$RGHT.x = dot(param[" VERTEX_PARAM_CAMERA_RIGHT "].xyz, %TANG.xyz);\n"
		"$RGHT.y = dot(param[" VERTEX_PARAM_CAMERA_RIGHT "].xyz, btng);\n"
		"$RGHT.z = dot(param[" VERTEX_PARAM_CAMERA_RIGHT "].xyz, %NRML.xyz);\n"
		"$DOWN.x = dot(param[" VERTEX_PARAM_CAMERA_DOWN "].xyz, %TANG.xyz);\n"
		"$DOWN.y = dot(param[" VERTEX_PARAM_CAMERA_DOWN "].xyz, btng);\n"
		"$DOWN.z = dot(param[" VERTEX_PARAM_CAMERA_DOWN "].xyz, %NRML.xyz);\n"
		
		"temp.x = dist * 8.0;\n"
		"temp.w = temp.x / (temp.x * dist + 4.0);\n"
		"$RGHT.w = temp.w * param[" VERTEX_PARAM_REFLECTION_SCALE "].x;\n"
		"$DOWN.w = temp.w * param[" VERTEX_PARAM_REFRACTION_SCALE "].x;\n"
	},
	
	// kVertexSnippetOutputDistortionDepth
	{
		'DDEP', 0,
		
		"DPH		$DDEP, %OPOS, program.env[" VERTEX_PARAM_DISTORT_CAMERA_PLANE "];\n",
		
		"$DDEP = dot(%OPOS.xyz, param[" VERTEX_PARAM_DISTORT_CAMERA_PLANE "].xyz) + param[" VERTEX_PARAM_DISTORT_CAMERA_PLANE "].w;\n"
	},
	
	// kVertexSnippetOutputImpostorDepth
	{
		'IDEP', 0,
		
		"DP4		temp.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_CAMERA1 "];\n"
		"DP4		temp.z, %OPOS, program.env[" VERTEX_PARAM_MATRIX_CAMERA2 "];\n"
		"MAD		temp.z, -temp.y, program.env[" VERTEX_PARAM_IMPOSTOR_DEPTH "].z, temp.z;\n"
		"MAD		$IDEP, temp.z, program.env[" VERTEX_PARAM_IMPOSTOR_DEPTH "].x, program.env[" VERTEX_PARAM_IMPOSTOR_DEPTH "].y;\n",
		
		"temp.y = dot(%OPOS, param[" VERTEX_PARAM_MATRIX_CAMERA1 "]);\n"
		"temp.z = dot(%OPOS, param[" VERTEX_PARAM_MATRIX_CAMERA2 "]) - temp.y * param[" VERTEX_PARAM_IMPOSTOR_DEPTH "].z;\n"
		"$IDEP = temp.z * param[" VERTEX_PARAM_IMPOSTOR_DEPTH "].x + param[" VERTEX_PARAM_IMPOSTOR_DEPTH "].y;\n"
	},
	
	// kVertexSnippetOutputImpostorRadius
	{
		'IRAD', 0,
		
		#if !C4PLAYSTATION3
		
			"ABS		temp.x, vertex.attrib[6].x;\n"
			"MUL		$IRAD, temp.x, {2.0, -1.0, 2.0, -1.0};\n",
		
		#else
		
			"MUL		$IRAD, |vertex.attrib[6].x|, {2.0, -1.0, 2.0, -1.0};\n",
		
		#endif
		
		"$IRAD = abs(attrib[6].x) * " FLOAT2 "(2.0, -1.0);\n"
	},
	
	// kVertexSnippetOutputImpostorShadowRadius
	{
		'ISRD', 0,
		
		#if !C4PLAYSTATION3
		
			"ABS		temp.x, vertex.attrib[6].x;\n"
			"MUL		$ISRD, temp.x, {4.0, -1.0, 4.0, -1.0};\n",
		
		#else
		
			"MUL		$ISRD, |vertex.attrib[6].x|, {4.0, -1.0, 4.0, -1.0};\n",
		
		#endif
		
		"$ISRD = abs(attrib[6].x) * " FLOAT2 "(4.0, -1.0);\n"
	},
	
	// kVertexSnippetOutputPointLightAttenuation
	{
		'CPLA', 0,
		
		"MUL		$ATTN.xyz, ldir, program.env[" VERTEX_PARAM_LIGHT_RANGE "].w;\n",
		
		"$ATTN.xyz = ldir * param[" VERTEX_PARAM_LIGHT_RANGE "].w;\n"
	},
	
	// kVertexSnippetOutputSpotLightAttenuation
	{
		'CSLA', 0,
		
		"DPH		temp.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_LIGHT0 "];\n"
		"DPH		temp.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_LIGHT1 "];\n"
		"DPH		temp.z, %OPOS, program.env[" VERTEX_PARAM_MATRIX_LIGHT2 "];\n"
		"MUL		$ATTN.xyz, temp, program.env[" VERTEX_PARAM_LIGHT_RANGE "].w;\n",
		
		"temp.x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_LIGHT0 "].xyz) + param[" VERTEX_PARAM_MATRIX_LIGHT0 "].w;\n"
		"temp.y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_LIGHT1 "].xyz) + param[" VERTEX_PARAM_MATRIX_LIGHT1 "].w;\n"
		"temp.z = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_LIGHT2 "].xyz) + param[" VERTEX_PARAM_MATRIX_LIGHT2 "].w;\n"
		"$ATTN.xyz = temp.xyz * param[" VERTEX_PARAM_LIGHT_RANGE "].w;\n"
	},
	
	// kVertexSnippetOutputDepthProjectTexcoord
	{
		'DPPT', 0,
		
		"DPH		$SHAD:x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SHADOW0 "];\n"
		"DPH		$SHAD:y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SHADOW1 "];\n"
		"DPH		$SHDZ, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SHADOW2 "];\n",
		
		"$SHAD:x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SHADOW0 "].xyz) + param[" VERTEX_PARAM_MATRIX_SHADOW0 "].w;\n"
		"$SHAD:y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SHADOW1 "].xyz) + param[" VERTEX_PARAM_MATRIX_SHADOW1 "].w;\n"
		"$SHDZ = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SHADOW2 "].xyz) + param[" VERTEX_PARAM_MATRIX_SHADOW2 "].w;\n"
	},
	
	// kVertexSnippetOutputLandscapeProjectTexcoord
	{
		'LSPT', 0,
		
		"DPH		$LAND.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SHADOW0 "];\n"
		"DPH		$LAND.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SHADOW1 "];\n"
		"DPH		$LAND.z, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SHADOW2 "];\n"
		
		"DPH		$SECT.x, %OPOS, program.env[" VERTEX_PARAM_SHADOW_SECTION_PLANE1 "];\n"
		"DPH		$SECT.y, %OPOS, program.env[" VERTEX_PARAM_SHADOW_SECTION_PLANE2 "];\n"
		"DPH		$SECT.z, %OPOS, program.env[" VERTEX_PARAM_SHADOW_SECTION_PLANE3 "];\n",
		
		"$LAND.x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SHADOW0 "].xyz) + param[" VERTEX_PARAM_MATRIX_SHADOW0 "].w;\n"
		"$LAND.y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SHADOW1 "].xyz) + param[" VERTEX_PARAM_MATRIX_SHADOW1 "].w;\n"
		"$LAND.z = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SHADOW2 "].xyz) + param[" VERTEX_PARAM_MATRIX_SHADOW2 "].w;\n"
		
		"$SECT.x = dot(%OPOS.xyz, param[" VERTEX_PARAM_SHADOW_SECTION_PLANE1 "].xyz) + param[" VERTEX_PARAM_SHADOW_SECTION_PLANE1 "].w;\n"
		"$SECT.y = dot(%OPOS.xyz, param[" VERTEX_PARAM_SHADOW_SECTION_PLANE2 "].xyz) + param[" VERTEX_PARAM_SHADOW_SECTION_PLANE2 "].w;\n"
		"$SECT.z = dot(%OPOS.xyz, param[" VERTEX_PARAM_SHADOW_SECTION_PLANE3 "].xyz) + param[" VERTEX_PARAM_SHADOW_SECTION_PLANE3 "].w;\n"
	},
	
	// kVertexSnippetOutputCubeProjectTexcoord
	{
		'CPPT', 0,
		
		"DPH		$PROJ.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_LIGHT0 "];\n"
		"DPH		$PROJ.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_LIGHT1 "];\n"
		"DPH		$PROJ.z, %OPOS, program.env[" VERTEX_PARAM_MATRIX_LIGHT2 "];\n",
		
		"$PROJ.x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_LIGHT0 "].xyz) + param[" VERTEX_PARAM_MATRIX_LIGHT0 "].w;\n"
		"$PROJ.y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_LIGHT1 "].xyz) + param[" VERTEX_PARAM_MATRIX_LIGHT1 "].w;\n"
		"$PROJ.z = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_LIGHT2 "].xyz) + param[" VERTEX_PARAM_MATRIX_LIGHT2 "].w;\n"
	},
	
	// kVertexSnippetOutputSpotProjectTexcoord
	{
		'SPPT', 0,
		
		"DPH		$PROJ.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SHADOW0 "];\n"
		"DPH		$PROJ.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SHADOW1 "];\n"
		"DPH		$PROJ.w, %OPOS, program.env[" VERTEX_PARAM_MATRIX_LIGHT2 "];\n",
		
		"$PROJ.x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SHADOW0 "].xyz) + param[" VERTEX_PARAM_MATRIX_SHADOW0 "].w;\n"
		"$PROJ.y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SHADOW1 "].xyz) + param[" VERTEX_PARAM_MATRIX_SHADOW1 "].w;\n"
		"$PROJ.w = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_LIGHT2 "].xyz) + param[" VERTEX_PARAM_MATRIX_LIGHT2 "].w;\n"
	},
	
	// kVertexSnippetOutputAmbientGradientDistance
	{
		'AMGD', 0,
		
		"DP4		$AMGD, vertex.attrib[0], program.env[" VERTEX_PARAM_AMBIENT_PLANE "];\n",
		
		"$AMGD = dot(attrib[0], param[" VERTEX_PARAM_AMBIENT_PLANE "]);\n"
	},
	
	// kVertexSnippetOutputAmbientSpaceVector
	{
		'AMSV', 0,
		
		"DP3		$AMBT.x, program.env[" VERTEX_PARAM_MATRIX_SPACE0 "], %NRML;\n"
		"DP3		$AMBT.y, program.env[" VERTEX_PARAM_MATRIX_SPACE1 "], %NRML;\n"
		"DP3		$AMBT.z, program.env[" VERTEX_PARAM_MATRIX_SPACE2 "], %NRML;\n"
		
		"DPH		temp.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SPACE0 "];\n"
		"DPH		temp.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SPACE1 "];\n"
		"DPH		temp.z, %OPOS, program.env[" VERTEX_PARAM_MATRIX_SPACE2 "];\n"
		"MUL		$APOS.xyz, temp, program.env[" VERTEX_PARAM_SPACE_SCALE "];\n",
		
		"$AMBT.x = dot(param[" VERTEX_PARAM_MATRIX_SPACE0 "].xyz, %NRML.xyz);\n"
		"$AMBT.y = dot(param[" VERTEX_PARAM_MATRIX_SPACE1 "].xyz, %NRML.xyz);\n"
		"$AMBT.z = dot(param[" VERTEX_PARAM_MATRIX_SPACE2 "].xyz, %NRML.xyz);\n"
		
		"temp.x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SPACE0 "].xyz) + param[" VERTEX_PARAM_MATRIX_SPACE0 "].w;\n"
		"temp.y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SPACE1 "].xyz) + param[" VERTEX_PARAM_MATRIX_SPACE1 "].w;\n"
		"temp.z = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_SPACE2 "].xyz) + param[" VERTEX_PARAM_MATRIX_SPACE2 "].w;\n"
		"$APOS.xyz = temp.xyz * param[" VERTEX_PARAM_SPACE_SCALE "].xyz;\n"
	},
	
	// kVertexSnippetOutputFiniteConstantFogFactors
	{
		'FCFF', 0,
		
		"DPH		$FDTP, %OPOS, program.env[" VERTEX_PARAM_FOG_PLANE "];\n"
		"DP3		$FDTV, vdir, program.env[" VERTEX_PARAM_FOG_PLANE "];\n",
		
		"$FDTP = dot(%OPOS.xyz, param[" VERTEX_PARAM_FOG_PLANE "].xyz) + param[" VERTEX_PARAM_FOG_PLANE "].w;\n"
		"$FDTV = dot(vdir, param[" VERTEX_PARAM_FOG_PLANE "].xyz);\n"
	},
	
	// kVertexSnippetOutputInfiniteConstantFogFactors
	{
		'ICFF', 0,
		
		"MAD		temp.xyz, %OPOS, 1024.0, program.env[" VERTEX_PARAM_CAMERA_POSITION "];\n"
		"DPH		$FDTP, temp, program.env[" VERTEX_PARAM_FOG_PLANE "];\n"
		"DP3		temp.w, %OPOS, program.env[" VERTEX_PARAM_FOG_PLANE "];\n"
		"MUL		$FDTV, temp.w, -1024.0;\n",
		
		"temp.xyz = %OPOS.xyz * 1024.0 + param[" VERTEX_PARAM_CAMERA_POSITION "].xyz;\n"
		"$FDTP = dot(temp.xyz, param[" VERTEX_PARAM_FOG_PLANE "].xyz) + param[" VERTEX_PARAM_FOG_PLANE "].w;\n"
		"$FDTV = dot(%OPOS.xyz, param[" VERTEX_PARAM_FOG_PLANE "].xyz) * -1024.0;\n"
	},
	
	// kVertexSnippetOutputFiniteLinearFogFactors
	{
		'FLFF', 0,
		
		"DPH		temp.z, %OPOS, program.env[" VERTEX_PARAM_FOG_PLANE "];\n"
		"MUL		$FDTP, temp.z, program.env[" VERTEX_PARAM_FOG_PARAMS "].z;\n"
		"DP3		$FDTV, vdir, program.env[" VERTEX_PARAM_FOG_PLANE "];\n"
		"ADD		temp.z, temp.z, program.env[" VERTEX_PARAM_FOG_PARAMS "].x;\n"
		"MUL		$FOGK, temp.z, program.env[" VERTEX_PARAM_FOG_PARAMS "].y;\n",
		
		"temp.z = dot(%OPOS.xyz, param[" VERTEX_PARAM_FOG_PLANE "].xyz) + param[" VERTEX_PARAM_FOG_PLANE "].w;\n"
		"$FDTP = temp.z * param[" VERTEX_PARAM_FOG_PARAMS "].z;\n"
		"$FDTV = dot(vdir, param[" VERTEX_PARAM_FOG_PLANE "].xyz);\n"
		"$FOGK = (temp.z + param[" VERTEX_PARAM_FOG_PARAMS "].x) * param[" VERTEX_PARAM_FOG_PARAMS "].y;\n"
	},
	
	// kVertexSnippetOutputInfiniteLinearFogFactors
	{
		'ILFF', 0,
		
		"TEMP		pdir;\n"
		
		"MAD		temp.xyz, %OPOS, 1024.0, program.env[" VERTEX_PARAM_CAMERA_POSITION "];\n"
		"DPH		temp.z, temp, program.env[" VERTEX_PARAM_FOG_PLANE "];\n"
		"MUL		$FDTP, temp.z, program.env[" VERTEX_PARAM_FOG_PARAMS "].z;\n"
		"DP3		temp.w, %OPOS, program.env[" VERTEX_PARAM_FOG_PLANE "];\n"
		"MUL		$FDTV, temp.w, -1024.0;\n"
		"ADD		temp.z, temp.z, program.env[" VERTEX_PARAM_FOG_PARAMS "].x;\n"
		"MUL		$FOGK, temp.z, program.env[" VERTEX_PARAM_FOG_PARAMS "].y;\n",
		
		"temp.xyz = %OPOS.xyz * 1024.0 + param[" VERTEX_PARAM_CAMERA_POSITION "].xyz;\n"
		"temp.z = dot(temp.xyz, param[" VERTEX_PARAM_FOG_PLANE "].xyz) + param[" VERTEX_PARAM_FOG_PLANE "].w;\n"
		"$FDTP = temp.z + param[" VERTEX_PARAM_FOG_PARAMS "].z;\n"
		"$FDTV = dot(%OPOS.xyz, param[" VERTEX_PARAM_FOG_PLANE "].xyz) * -1024.0;\n"
		"$FOGK = (temp.z + param[" VERTEX_PARAM_FOG_PARAMS "].x) * param[" VERTEX_PARAM_FOG_PARAMS "].y;\n"
	},
	
	// kVertexSnippetMotionBlurTransform
	{
		'BLUR', 0,
		
		"DPH		$VELA.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "];\n"
		"DPH		$VELA.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "];\n"
		"DPH		$VELA.w, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "];\n"
		
		"DPH		$VELB.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "];\n"
		"DPH		$VELB.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "];\n"
		"DPH		$VELB.w, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "];\n",
		
		"$VELA.x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "].w;\n"
		"$VELA.y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "].w;\n"
		"$VELA.w = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "].w;\n"
		
		"$VELB.x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "].w;\n"
		"$VELB.y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "].w;\n"
		"$VELB.w = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "].w;\n"
	},
	
	// kVertexSnippetDeformMotionBlurTransform
	{
		'DBLR', 0,
		
		"DPH		$VELA.x, vertex.attrib[7], program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "];\n"
		"DPH		$VELA.y, vertex.attrib[7], program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "];\n"
		"DPH		$VELA.w, vertex.attrib[7], program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "];\n"
		
		"DPH		$VELB.x, vertex.attrib[0], program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "];\n"
		"DPH		$VELB.y, vertex.attrib[0], program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "];\n"
		"DPH		$VELB.w, vertex.attrib[0], program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "];\n",
		
		"$VELA.x = dot(attrib[7].xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "].w;\n"
		"$VELA.y = dot(attrib[7].xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "].w;\n"
		"$VELA.w = dot(attrib[7].xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "].w;\n"
		
		"$VELB.x = dot(attrib[0].xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "].w;\n"
		"$VELB.y = dot(attrib[0].xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "].w;\n"
		"$VELB.w = dot(attrib[0].xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "].w;\n"
	},
	
	// kVertexSnippetVelocityMotionBlurTransform
	{
		'VBLR', 0,
		
		"MAD		temp.xyz, -vertex.attrib[7], program.env[" VERTEX_PARAM_SHADER_TIME "].y, vertex.attrib[0];\n"
		
		"DPH		$VELA.x, temp, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "];\n"
		"DPH		$VELA.y, temp, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "];\n"
		"DPH		$VELA.w, temp, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "];\n"
		
		"DPH		$VELB.x, vertex.attrib[0], program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "];\n"
		"DPH		$VELB.y, vertex.attrib[0], program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "];\n"
		"DPH		$VELB.w, vertex.attrib[0], program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "];\n",
		
		"temp.xyz = attrib[0].xyz - attrib[7].xyz * param[" VERTEX_PARAM_SHADER_TIME "].y;\n"
		
		"$VELA.x = dot(temp.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "].w;\n"
		"$VELA.y = dot(temp.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "].w;\n"
		"$VELA.w = dot(temp.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "].w;\n"
		
		"$VELB.x = dot(attrib[0].xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "].w;\n"
		"$VELB.y = dot(attrib[0].xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "].w;\n"
		"$VELB.w = dot(attrib[0].xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "].xyz) + param[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "].w;\n"
	},
	
	// kVertexSnippetInfiniteMotionBlurTransform
	{
		'IBLR', 0,
		
		"DP3		$VELA.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "];\n"
		"DP3		$VELA.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "];\n"
		"DP3		$VELA.w, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "];\n"
		
		"DP3		$VELB.x, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "];\n"
		"DP3		$VELB.y, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "];\n"
		"DP3		$VELB.w, %OPOS, program.env[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "];\n",
		
		"$VELA.x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A0 "].xyz);\n"
		"$VELA.y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A1 "].xyz);\n"
		"$VELA.w = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_A3 "].xyz);\n"
		
		"$VELB.x = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B0 "].xyz);\n"
		"$VELB.y = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B1 "].xyz);\n"
		"$VELB.w = dot(%OPOS.xyz, param[" VERTEX_PARAM_MATRIX_VELOCITY_B3 "].xyz);\n"
	}
};


VertexProgram::VertexProgram(const char *source, unsigned_int32 size, const unsigned_int32 *signature)
{
	MemoryMgr::CopyMemory(signature, shaderSignature, signature[0] * 4 + 4);
	
	Construct();
	SetSourceCode(source, size);
	
	#if C4LOG_VERTEX_PROGRAMS
	
		Engine::LogSource(source);
	
	#endif
}

#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]

VertexProgram::~VertexProgram()
{
	Destruct();
}

unsigned_int32 VertexProgram::Hash(const KeyType& key)
{
	unsigned_int32 hash = 0;
	
	int32 count = key[0];
	for (machine a = 1; a <= count; a++)
	{
		hash += key[a];
		hash = (hash << 5) | (hash >> 27);
	}
	
	return (hash);
}

void VertexProgram::Initialize(void)
{
	hashTable = new(hashTableStorage) HashTable<VertexProgram>(16, 16);
}

void VertexProgram::Terminate(void)
{
	hashTable->~HashTable();
}

VertexProgram *VertexProgram::Get(const unsigned_int32 *signature)
{
	VertexProgram *program = hashTable->Find(signature);
	if (program) program->Retain();
	return (program);
}

VertexProgram *VertexProgram::New(const VertexAssembly *assembly)
{
	unsigned_int32 *signature = assembly->signatureStorage;
	VertexProgram *program = hashTable->Find(signature);
	if (!program)
	{
		#if C4OPENGL
		
			static const char prolog[] =
			{
				"!!ARBvp1.0\n"
				"TEMP temp;\n"
			};
			
			static const char epilog[] =
			{
				"END"
			};
		
		#else
		
			static const char prolog[] =
			{
				"struct resultStruct\n"
				"{\n"
				"float4 position : HPOS;\n"
				"float4 color0 : COL0;\n"
				"float4 color1 : COL1;\n"
				"float pointsize : PSIZ;\n"
				"float4 texcoord[8] : TEX0;\n"
				"};\n"
				
				"resultStruct main(float4 attrib[16] : ATTR0, uniform float4 param[" VERTEX_PARAM_COUNT "] : C0)\n"
				"{\n"
				"resultStruct result;\n"
				"float4 temp;\n"
			};
			
			static const char epilog[] =
			{
				"return result;\n"
				"}\n"
			};
		
		#endif
		
		char *source = ShaderAttribute::sourceStorage;
		int32 size = Text::CopyText(prolog, source);
		
		int32 count = signature[0];
		for (machine a = 0; a < count; a++)
		{
			#if C4OPENGL
			
				size += Text::CopyText(assembly->vertexSnippet[a]->programCode, &source[size]);
			
			#else
			
				size += Text::CopyText(assembly->vertexSnippet[a]->shaderCode, &source[size]);
			
			#endif
		}
		
		size += Text::CopyText(epilog, &source[size]);
		
		program = MemoryMgr::GetMainHeap()->New<VertexProgram>(sizeof(VertexProgram) + signature[0] * 4);
		new(program) VertexProgram(source, size, signature);
		
		hashTable->Insert(program);
	}
	
	program->Retain();
	return (program);
}

VertexProgram *VertexProgram::New(const char *source, unsigned_int32 size, const unsigned_int32 *signature)
{
	VertexProgram *program = MemoryMgr::GetMainHeap()->New<VertexProgram>(sizeof(VertexProgram) + signature[0] * 4);
	new(program) VertexProgram(source, size, signature);
	
	program->Retain();
	hashTable->Insert(program);
	return (program);
}

void VertexProgram::Flush(void)
{
	int32 bucketCount = hashTable->GetBucketCount();
	for (machine a = 0; a < bucketCount; a++)
	{
		VertexProgram *program = hashTable->GetFirstBucketElement(a);
		while (program)
		{
			VertexProgram *next = program->Next();
			if (program->GetReferenceCount() == 1) program->Release();
			program = next;
		}
	}
}

#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]

// ZYURVUR
