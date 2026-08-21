#pragma once

namespace UnE
{
	namespace Core
	{
		// Visibility Mask
		enum VisibilityMask // 특정 카메라나 뷰포트에서 보여지기를 원치 않을 때 사용
		{
			VIM_NONE          = 0,           // 모든 오브젝트 보이기
			VIM_VIRTUALCAMERA = 1 << 0,      // 가상카메라와 관련된 오브젝트
			VIM_LENSFLARE     = 1 << 1,      // 렌즈플레어 효과 이미지
			VIM_LIGHT         = 1 << 2,      // 가상조명과 관련된 오브젝트
			VIM_ALL           = 0xFFFFFFFF,  // 모두 감추기
		};
	}
}