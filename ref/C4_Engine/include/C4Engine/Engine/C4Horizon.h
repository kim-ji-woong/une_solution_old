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


#ifndef C4Horizon_h
#define C4Horizon_h


#include "C4Processes.h"


namespace C4
{
	class HorizonProcess : public TextureMapProcess
	{
		private:
			
			unsigned_int32		horizonFlags;
			
			Texture				*secondaryTextureObject;
			ResourceName		secondaryTextureName;
			
			static Texture		*horizonTexture;
			
			HorizonProcess(const HorizonProcess& horizonProcess);
			
			Process *Replicate(void) const override;
			
			bool ProcessEnabled(const ShaderCompileData *compileData) const;
		
		public:
			
			HorizonProcess();
			~HorizonProcess();
			
			static void Initialize(void);
			static void Terminate(void);
			
			unsigned_int32 GetHorizonFlags(void) const
			{
				return (horizonFlags);
			}
			
			void SetHorizonFlags(unsigned_int32 flags)
			{
				horizonFlags = flags;
			}
			
			static bool ValidShader(ProcessType type, int32 shader);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			C4API void SetSecondaryTexture(const char *name);
			
			int32 GetPortCount(void) const;
			const char *GetPortName(int32 index) const;
			
			#if C4PLAYSTATION3
			
				unsigned_int32 GetPortCompileFlags(int32 index) const;
			
			#endif
			
			int32 GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const;
			int32 GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const;
			void GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const;
			int32 GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const;
			
			int32 GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const;
			int32 GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const;
	};
}


#endif

// ZYURVUR
