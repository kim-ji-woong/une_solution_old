#ifndef Common_h__
#define Common_h__

#pragma once

#ifndef dnonlynew

	#ifdef OUT
		#undef OUT
	#endif

	#ifdef CONST
		#undef CONST
	#endif

	#ifdef DOTNET // DOTNET

		#define geonew		gcnew
		#define dnonlynew	gcnew

		#define TEMPLATE_DECLARE_CLASS(ClassName)	public ref class ClassName

		#define _DECLARE_CLASS(ClassName)	ref class ClassName;
		#define DECLARE_CLASS(ClassName)	_DECLARE_CLASS(ClassName)					\
			typedef ClassName^ ClassName##Ref;			\
			typedef ClassName^ ClassName##Ref##Const;	\
			typedef ClassName^ ClassName##Instance;

		
		#define DECLARE_NO_EXPORT_CLASS(ClassName)	public class ClassName

		#define PUBLIC
		#define CBR(Data)					Data%		// Call by Reference
		#define	OUT							[System::Runtime::InteropServices::OutAttribute]

		#define REF(ClassName)				ClassName^
		#define REF_CONST(ClassName)		ClassName^
		#define INSTANCE(ClassName)			ClassName^
		#define INST_CONST(ClassName)		ClassName^
		#define POINTER(ClassName)			ClassName^
		#define PTR_CONST(ClassName)		ClassName^
		#define SYS_CONST(ClassName)		ClassName

		#define OF(obj, data)				obj->data
		#define ENUM_OF(EnumName, data)		EnumName::data

		#define ENUM_CLASS					enum class
		#define CONST
		#define ABSTRACT					abstract

		#define THIS_OBJ					this
		#define NULL_PTR					nullptr

		#define POINTER_ADDR(obj)			obj
		#define POINTER_VALUE(obj)			obj

		#define ARR1_PTR(ClassName)			array<ClassName^>^
		#define ARR2_PTR(ClassName,data)	array<ClassName, data>^
		#define IDX(var, LOW, COLUMN)		var[LOW, COLUMN]

		#define CONSTF					
		#define SC_VALUE(value)				=(value)

		#define FRIEND						static
		#define	STD_PAIR(a,b)				System::Collections::Generic::KeyValuePair<a,b>
		#define STD_VECTOR(Class)			System::Collections::Generic::List<Class>
		#define STD_LIST(Class)				System::Collections::Generic::LinkedList<Class>

		#define STD_SWAP					Swap

		#define USR_CONV					static explicit 
		#define CONV_TYPE(ClassName,varb)	ClassName^ varb
		#define EXPORT_FUNCTION
		#define INLINE 

		#define NormalString				System::String^
		#define UnicodeString				System::String^
	#else

		#define geonew		new
		#define dnonlynew

		#define TEMPLATE_DECLARE_CLASS(ClassName)	class ClassName

		#define _DECLARE_CLASS(ClassName)	class ClassName;
		#define DECLARE_CLASS(ClassName)	_DECLARE_CLASS(ClassName)						\
			typedef ClassName& ClassName##Ref;				\
			typedef const ClassName& ClassName##Ref##Const;	\
			typedef ClassName  ClassName##Instance;

		
		#define DECLARE_NO_EXPORT_CLASS(ClassName)	class ClassName

		#define PUBLIC						public
		#define CBR(Data)					Data&		// Call by Reference
		#define	OUT

		#define REF(ClassName)				ClassName&
		#define REF_CONST(ClassName)		const ClassName&
		#define INSTANCE(ClassName)			ClassName
		#define INST_CONST(ClassName)		const ClassName
		#define POINTER(ClassName)			ClassName*
		#define PTR_CONST(ClassName)		const ClassName*
		#define SYS_CONST(ClassName)		const ClassName

		#define CONSTF						const

		#define OF(obj, data)				obj.data
		#define ENUM_OF(EnumName, data)		data
		#define ENUM_CLASS					enum
		#define CONST						const
		#define ABSTRACT

		#define THIS_OBJ					(*this)
		#define NULL_PTR					0
		#define POINTER_ADDR(obj)			(&obj)
		#define POINTER_VALUE(obj)			(*obj)

		#define ARR1_PTR(ClassName)			ClassName*
		#define ARR2_PTR(ClassName,data)	ClassName*[data]
		#define IDX(var,LOW,COLUMN)			var[ LOW ][ COLUMN ]

		#define SC_VALUE(value)
		#define FRIEND						friend

		#define	STD_PAIR(a,b)				std::pair<a,b>
		#define STD_VECTOR(Class)			std::vector<Class>
		#define STD_LIST(Class)				std::list<Class>
		#define STD_SWAP					std::swap
		
		#define USR_CONV
		#define CONV_TYPE(ClassName,varb)	
		#define INLINE						inline

		#define NormalString				std::string
		#define UnicodeString				std::wstring
	#endif

#endif


#define NO_EXPORT_CLASS(ClassName)	DECLARE_CLASS(ClassName)	\
	DECLARE_NO_EXPORT_CLASS(ClassName)


#ifdef DOTNET

namespace UnE
{
	public ref class NullChecker
	{
	public:
		static bool IsNull(System::Object^ obj)
		{
			return obj == nullptr;
		}
	};
}

#endif


#endif // Common_h__
