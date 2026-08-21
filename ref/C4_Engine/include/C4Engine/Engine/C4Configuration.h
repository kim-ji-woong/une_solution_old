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


#ifndef C4Configuration_h
#define C4Configuration_h


//# \component	Interface Manager
//# \prefix		InterfaceMgr/

//# \import		C4ColorPicker.h


#include "C4Configurable.h"
#include "C4FilePicker.h"
#include "C4ColorPicker.h"
#include "C4Messages.h"


namespace C4
{
	//# \tree	Setting
	//
	//# \node	HeadingSetting
	//# \node	BooleanSetting
	//# \node	IntegerSetting
	//# \node	PowerTwoSetting
	//# \node	FloatSetting
	//# \node	TextSetting
	//# \node	MenuSetting
	//# \node	ColorSetting
	//# \sub
	//#		\node	CheckColorSetting
	//# \end
	//# \node	ResourceSetting
	//# \node	MultiResourceSetting
	
	
	enum
	{
		kMaxValueNameLength		= 15
	};
	
	
	typedef Type							SettingType;
	typedef String<kMaxValueNameLength>		ValueName;
	
	
	//# \enum	SettingType
	
	enum
	{
		kSettingHeading			= 'HEAD',		//## A setting that simply displays a heading and has no value.
		kSettingBoolean			= 'BOOL',		//## A boolean setting represented by a check box.
		kSettingInteger			= 'INT ',		//## An integer setting represented by a slider.
		kSettingPowerTwo		= 'POW2',		//## A power-of-two setting represented by a slider.
		kSettingFloat			= 'FLOT',		//## A floating-point setting represented by a slider.
		kSettingText			= 'TEXT',		//## A text setting represented by an editable text box.
		kSettingMenu			= 'MENU',		//## A multi-valued setting represented by a popup menu.
		kSettingColor			= 'COLR',		//## A color setting represented by a color box.
		kSettingCheckColor		= 'CCLR',		//## A color setting represented by a color box with a check box for enable/disable.
		kSettingResource		= 'RSRC',		//## A resource name setting represented by a text box and a browse button.
		kSettingMultiResource	= 'MULT'		//## A setting showing a list of resource names in a text box with a browse button.
	};
	
	
	enum
	{
		kMaxSettingTitleLength			= 255,
		kMaxTextSettingLength			= 255
	};
	
	
	//# \enum	ResourceSettingFlags
	
	enum
	{
		kResourceSettingGenericPath		= 1 << 0,	//## Return the generic resource path instead of the virtual resource path.
		kResourceSettingImportCatalog	= 1 << 1	//## Show files in the Import folder instead of the Data folder.
	};
	
	
	enum
	{
		kConfigurationScript	= 1 << 0
	};
	
	
	enum
	{
		kWidgetConfiguration	= 'CNFG'
	};
	
	
	class Value;
	class FilePicker;
	class ColorPicker;
	class SettingInterface;
	class ConfigurationWidget;
	
	
	//# \class	Setting		The base class for all user-configurable settings.
	//
	//# Every user-configurable setting is a subclass of the $Setting$ class.
	// 
	//# \def	class Setting : public ListElement<Setting>, public Packable
	// 
	//# \ctor	Setting(SettingType type, Type identifier); 
	// 
	//# \param	type			The setting type.
	//# \param	identifier		The setting's unique identifier. 
	//
	//# \desc
	//# The $Setting$ class is the base class for all user-configurable setting objects.
	// 
	//# \table	SettingType
	//
	//# \base	Utilities/ListElement<Setting>		Used internally by the Interface Manager.
	//# \base	ResourceMgr/Packable				A setting can be packed for storage in resources. 
	//
	//# \also	$@Configurable@$
	
	
	//# \function	Setting::GetSettingType		Returns the setting type.
	//
	//# \proto	SettingType GetSettingType(void) const;
	//
	//# \desc
	//# The $GetSettingType$ function returns the type of a setting, which is one of the following values.
	//
	//# \table	SettingType
	//
	//# \also	$@Setting::GetSettingIdentifier@$
	
	
	//# \function	Setting::GetSettingIdentifier		Returns the setting's unique identifier.
	//
	//# \proto	Type GetSettingIdentifier(void) const;
	//
	//# \desc
	//# The $GetSettingIdentifier$ returns the unique identifier that was passed to the
	//# $@Setting@$ constructor.
	//
	//# \also	$@Setting::GetSettingType@$
	
	
	class Setting : public ListElement<Setting>, public Packable
	{
		private:
			
			SettingType			settingType;
			Type				settingIdentifier;
			ValueName			settingValueName;
			
			SettingInterface	*settingInterface;
			
			void operator =(const Setting& setting) /*= delete;*/ {}	// C++11
			
			virtual Setting *Replicate(void) const = 0;
		
		protected:
			
			C4API Setting(SettingType type);
			C4API Setting(SettingType type, Type identifier);
			C4API Setting(const Setting& setting);
		
		public:
			
			C4API virtual ~Setting();
			
			SettingType GetSettingType(void) const
			{
				return (settingType);
			}
			
			Type GetSettingIdentifier(void) const
			{
				return (settingIdentifier);
			}
			
			const char *GetSettingValueName(void) const
			{
				return (settingValueName);
			}
			
			void SetSettingValueName(const char *name)
			{
				settingValueName = name;
			}
			
			SettingInterface *GetSettingInterface(void) const
			{
				return (settingInterface);
			}
			
			void SetSettingInterface(SettingInterface *intrface)
			{
				settingInterface = intrface;
			}
			
			Setting *Clone(void) const
			{
				return (Replicate());
			}
			
			static Setting *Construct(Unpacker& data, unsigned_int32 unpackFlags = 0);
			
			C4API void PackType(Packer& data) const;
			C4API void Pack(Packer& data, unsigned_int32 packFlags) const;
			C4API void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			C4API void *BeginSettingsUnpack(void);
			
			C4API virtual void Compress(Compressor& data) const;
			C4API virtual bool Decompress(Decompressor& data);
			
			virtual void Copy(const Setting *setting);
			virtual bool operator ==(const Setting& setting) const = 0;
			
			C4API virtual bool SetValue(const Value *value);
	};
	
	
	class SettingInterface : public ExclusiveObservable<SettingInterface>
	{
		friend class ConfigurationWidget;
		
		private:
			
			Setting								*settingData;
			Widget								*settingGroup;
			
			EditTextWidget						*valueNameBox;
			WidgetObserver<SettingInterface>	valueNameObserver;
			
			String<kMaxSettingTitleLength>		settingTitle;
			
			void HandleValueNameEvent(Widget *widget, const WidgetEventData *eventData);
		
		protected:
			
			C4API SettingInterface(Setting *setting, const char *title);
			
			C4API virtual void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			C4API virtual ~SettingInterface();
			
			Setting *GetSettingData(void) const
			{
				return (settingData);
			}
			
			const char *GetValueName(void) const
			{
				return ((valueNameBox) ? valueNameBox->GetText() : nullptr);
			}
			
			void SetValueName(const char *name)
			{
				if (valueNameBox) valueNameBox->SetText(name);
			}
			
			const char *GetSettingTitle(void) const
			{
				return (settingTitle);
			}
			
			void UpdateCurrentSetting(void)
			{
				if (DeterminantValue()) SetDeterminantValue();
			}
			
			C4API virtual bool DeterminantValue(void) const;
			C4API virtual void SetDeterminantValue(void);
			C4API virtual void SetIndeterminantValue(void);
			
			C4API virtual bool ExtractCurrentSetting(void);
	};
	
	
	//# \class	HeadingSetting		A setting that simply displays a heading and has no value.
	//
	//# The $HeadingSetting$ class is used to display a heading in a list of settings.
	//
	//# \def	class HeadingSetting : public Setting
	//
	//# \ctor	HeadingSetting(Type identifier, const char *title);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	title			The title of the setting.
	//
	//# \desc
	//# The $HeadingSetting$ class represents a setting that simply displays a heading (specified
	//# by the $title$ parameter) and has no value. Even though the setting doesn't need to be
	//# identified in the $@Configurable::SetSetting@$ function, it should still have a unique
	//# identifier specified by the $identifier$ parameter.
	//
	//# \base	Setting		A $HeadingSetting$ is a specific type of $@Setting@$.
	
	
	class HeadingSetting : public Setting
	{
		friend class Setting;
		
		private:
			
			HeadingSetting();
			HeadingSetting(const HeadingSetting& headingSetting);
			
			Setting *Replicate(void) const override;
		
		public:
			
			C4API HeadingSetting(Type identifier, const char *title);
			C4API ~HeadingSetting();
			
			bool operator ==(const Setting& setting) const;
	};
	
	
	class HeadingSettingInterface : public SettingInterface
	{
		private:
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			HeadingSettingInterface(HeadingSetting *setting, const char *title);
			~HeadingSettingInterface();
	};
	
	
	//# \class	BooleanSetting		A boolean setting represented by a check box.
	//
	//# The $BooleanSetting$ class is used for a boolean setting represented by a check box.
	//
	//# \def	class BooleanSetting : public Setting
	//
	//# \ctor	BooleanSetting(Type identifier, bool value, const char *title);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	value			The initial value of the setting.
	//# \param	title			The title of the setting.
	//
	//# \desc
	//# The $BooleanSetting$ class represents a setting that displays a check box and
	//# has a boolean value.
	//
	//# \base	Setting		A $BooleanSetting$ is a specific type of $@Setting@$.
	
	
	//# \function	BooleanSetting::GetBooleanValue		Returns the boolean value stored in the setting.
	//
	//# \proto	bool GetBooleanValue(void) const;
	//
	//# \desc
	//# The $GetBooleanValue$ function returns the boolean value stored in the setting object.
	
	
	class BooleanSetting : public Setting
	{
		friend class Setting;
		
		private:
			
			bool	booleanValue;
			
			BooleanSetting();
			BooleanSetting(const BooleanSetting& booleanSetting);
			
			Setting *Replicate(void) const override;
		
		public:
			
			C4API BooleanSetting(Type identifier, bool value, const char *title);
			C4API ~BooleanSetting();
			
			bool GetBooleanValue(void) const
			{
				return (booleanValue);
			}
			
			void SetBooleanValue(bool value)
			{
				booleanValue = value;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			void Copy(const Setting *setting);
			bool operator ==(const Setting& setting) const;
			
			bool SetValue(const Value *value);
	};
	
	
	class BooleanSettingInterface : public SettingInterface
	{
		private:
			
			CheckWidget									*checkWidget;
			WidgetObserver<BooleanSettingInterface>		checkWidgetObserver;
			
			void HandleCheckWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			BooleanSettingInterface(BooleanSetting *setting, const char *title);
			~BooleanSettingInterface();
			
			void SetBooleanValue(int32 value)
			{
				checkWidget->SetValue(value);
			}
			
			bool DeterminantValue(void) const;
			void SetDeterminantValue(void);
			void SetIndeterminantValue(void);
			
			bool ExtractCurrentSetting(void);
	};
	
	
	class SliderSettingInterface : public SettingInterface
	{
		protected:
			
			SliderWidget			*sliderWidget;
			EditTextWidget			*textWidget;
			
			SliderSettingInterface(Setting *setting, const char *title);
		
		public:
			
			~SliderSettingInterface();
			
			bool DeterminantValue(void) const;
			void SetIndeterminantValue(void);
	};
	
	
	//# \class	IntegerSetting		An integer setting represented by a slider.
	//
	//# The $IntegerSetting$ class is used for an integer setting represented by a slider.
	//
	//# \def	class IntegerSetting : public Setting
	//
	//# \ctor	IntegerSetting(Type identifier, int32 value, const char *title, int32 min, int32 max, int32 step);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	value			The initial value of the setting.
	//# \param	title			The title of the setting.
	//# \param	min				The minimum value allowed for the setting.
	//# \param	max				The maximum value allowed for the setting.
	//# \param	step			The smallest increment allowed between the minimum and maximum values.
	//
	//# \desc
	//# The $IntegerSetting$ class represents a setting that displays a slider and has an
	//# integer value restricted to a given range. A text box is also displayed, allowing
	//# the user to enter a value directly. The value of the setting is always equal to the minimum
	//# value specified by the $min$ parameter plus a multiple of the $step$ parameter, but is never
	//# greater than the maaximum value specified by the $max$ parameter. If the user enters a
	//# number in the text box, then it is rounded down to the nearest valid value.
	//
	//# \base	Setting		An $IntegerSetting$ is a specific type of $@Setting@$.
	
	
	//# \function	IntegerSetting::GetIntegerValue		Returns the integer value stored in the setting.
	//
	//# \proto	int32 GetIntegerValue(void) const;
	//
	//# \desc
	//# The $GetIntegerValue$ function returns the integer value stored in the setting object.
	
	
	class IntegerSetting : public Setting
	{
		friend class Setting;
		
		private:
			
			int32		integerValue;
			
			IntegerSetting();
			IntegerSetting(const IntegerSetting& integerSetting);
			
			Setting *Replicate(void) const override;
		
		public:
			
			C4API IntegerSetting(Type identifier, int32 value, const char *title, int32 min, int32 max, int32 step);
			C4API ~IntegerSetting();
			
			int32 GetIntegerValue(void) const
			{
				return (integerValue);
			}
			
			void SetIntegerValue(int32 value)
			{
				integerValue = value;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			void Copy(const Setting *setting);
			bool operator ==(const Setting& setting) const;
			
			bool SetValue(const Value *value);
	};
	
	
	class IntegerSettingInterface : public SliderSettingInterface
	{
		private:
			
			int32		minValue;
			int32		maxValue;
			int32		stepValue;
			
			WidgetObserver<IntegerSettingInterface>		sliderWidgetObserver;
			WidgetObserver<IntegerSettingInterface>		textWidgetObserver;
			
			void HandleSliderWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleTextWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			IntegerSettingInterface(IntegerSetting *setting, const char *title, int32 min, int32 max, int32 step);
			~IntegerSettingInterface();
			
			void SetDeterminantValue(void);
			bool ExtractCurrentSetting(void);
	};
	
	
	//# \class	PowerTwoSetting		A power-of-two setting represented by a slider.
	//
	//# The $PowerTwoSetting$ class is used for a power-of-two setting represented by a slider.
	//
	//# \def	class PowerTwoSetting : public Setting
	//
	//# \ctor	PowerTwoSetting(Type identifier, int32 value, const char *title, int32 min, int32 max);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	value			The initial value of the setting.
	//# \param	title			The title of the setting.
	//# \param	min				The minimum value allowed for the setting.
	//# \param	max				The maximum value allowed for the setting.
	//
	//# \desc
	//# The $PowerTwoSetting$ class represents a setting that displays a slider and has an
	//# integer value restricted to a power of two within a given range. A text box is also displayed,
	//# allowing the user to enter a value directly. The value of the setting is always equal to a power
	//# of two between the minimum value specified by the $min$ parameter and the maaximum value
	//# specified by the $max$ parameter. If the user enters a number in the text box, then it is
	//# rounded down to the nearest valid value.
	//
	//# \base	Setting		A $PowerTwoSetting$ is a specific type of $@Setting@$.
	
	
	//# \function	PowerTwoSetting::GetIntegerValue	Returns the integer value stored in the setting.
	//
	//# \proto	int32 GetIntegerValue(void) const;
	//
	//# \desc
	//# The $GetIntegerValue$ function returns the integer value stored in the setting object.
	
	
	class PowerTwoSetting : public Setting
	{
		friend class Setting;
		
		private:
			
			int32		integerValue;
			
			PowerTwoSetting();
			PowerTwoSetting(const PowerTwoSetting& powerTwoSetting);
			
			Setting *Replicate(void) const override;
		
		public:
			
			C4API PowerTwoSetting(Type identifier, int32 value, const char *title, int32 min, int32 max);
			C4API ~PowerTwoSetting();
			
			int32 GetIntegerValue(void) const
			{
				return (integerValue);
			}
			
			void SetIntegerValue(int32 value)
			{
				integerValue = value;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			void Copy(const Setting *setting);
			bool operator ==(const Setting& setting) const;
			
			bool SetValue(const Value *value);
	};
	
	
	class PowerTwoSettingInterface : public SliderSettingInterface
	{
		private:
			
			int32		minValue;
			int32		maxValue;
			
			WidgetObserver<PowerTwoSettingInterface>	sliderWidgetObserver;
			WidgetObserver<PowerTwoSettingInterface>	textWidgetObserver;
			
			void HandleSliderWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleTextWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			PowerTwoSettingInterface(PowerTwoSetting *setting, const char *title, int32 min, int32 max);
			~PowerTwoSettingInterface();
			
			void SetDeterminantValue(void);
			bool ExtractCurrentSetting(void);
	};
	
	
	//# \class	FloatSetting	A floating-point setting represented by a slider.
	//
	//# The $FloatSetting$ class is used for a floating-point setting represented by a slider.
	//
	//# \def	class FloatSetting : public Setting
	//
	//# \ctor	FloatSetting(Type identifier, float value, const char *title, float min, float max, float step);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	value			The initial value of the setting.
	//# \param	title			The title of the setting.
	//# \param	min				The minimum value allowed for the setting.
	//# \param	max				The maximum value allowed for the setting.
	//# \param	step			The smallest increment allowed between the minimum and maximum values.
	//
	//# \desc
	//# The $FloatSetting$ class represents a setting that displays a slider and has an
	//# floating-point value restricted to a given range. A text box is also displayed, allowing
	//# the user to enter a value directly. The value of the setting is always equal to the minimum
	//# value specified by the $min$ parameter plus an integer multiple of the $step$ parameter, but
	//# is never greater than the maaximum value specified by the $max$ parameter. If the user enters a
	//# number in the text box, then it is rounded to the nearest valid value.
	//
	//# \base	Setting		A $FloatSetting$ is a specific type of $@Setting@$.
	
	
	//# \function	FloatSetting::GetFloatValue		Returns the floating-point value stored in the setting.
	//
	//# \proto	float GetFloatValue(void) const;
	//
	//# \desc
	//# The $GetFloatValue$ function returns the floating-point value stored in the setting object.
	
	
	class FloatSetting : public Setting
	{
		friend class Setting;
		
		private:
			
			float		floatValue;
			
			FloatSetting();
			FloatSetting(const FloatSetting& floatSetting);
			
			Setting *Replicate(void) const override;
		
		public:
			
			C4API FloatSetting(Type identifier, float value, const char *title, float min, float max, float step);
			C4API ~FloatSetting();
			
			float GetFloatValue(void) const
			{
				return (floatValue);
			}
			
			void SetFloatValue(float value)
			{
				floatValue = value;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			void Copy(const Setting *setting);
			bool operator ==(const Setting& setting) const;
			
			bool SetValue(const Value *value);
	};
	
	
	class FloatSettingInterface : public SliderSettingInterface
	{
		private:
			
			float		minValue;
			float		maxValue;
			float		stepValue;
			
			WidgetObserver<FloatSettingInterface>	sliderWidgetObserver;
			WidgetObserver<FloatSettingInterface>	textWidgetObserver;
			
			void HandleSliderWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleTextWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			FloatSettingInterface(FloatSetting *setting, const char *title, float min, float max, float step);
			~FloatSettingInterface();
			
			void SetDeterminantValue(void);
			bool ExtractCurrentSetting(void);
	};
	
	
	//# \class	TextSetting		A text setting represented by an editable text box.
	//
	//# The $TextSetting$ class is used for a text setting represented by an editable text box.
	//
	//# \def	class TextSetting : public Setting
	//
	//# \ctor	TextSetting(Type identifier, const char *text, const char *title, int32 maxLen,
	//# \ctor2	EditableTextWidget::FilterProc *filterProc = nullptr);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	text			The initial text for the setting.
	//# \param	title			The title of the setting.
	//# \param	maxLen			The maximum number of characters that can be entered into the text box.
	//# \param	filterProc		A character filter function. If this is $nullptr$, then all characters are allowed.
	//
	//# \desc
	//# The $TextSetting$ class represents a setting that displays a text box and has a
	//# string value. The maximum length of the string is specified by the $maxLen$ parameter,
	//# and the characters allowed in the string can be controlled by specifying a filter with the
	//# $filterProc$ parameter. If no filter is specified, then all characters are allowed.
	//
	//# \base	Setting		A $TextSetting$ is a specific type of $@Setting@$.
	
	
	//# \function	TextSetting::GetText		Returns the text stored in the setting.
	//
	//# \proto	const char *GetText(void) const;
	//
	//# \desc
	//# The $GetText$ function returns a pointer to the text string stored in the setting object.
	
	
	class TextSetting : public Setting
	{
		friend class Setting;
		
		private:
			
			String<kMaxTextSettingLength>		textValue;
			
			TextSetting();
			TextSetting(const TextSetting& textSetting);
			
			Setting *Replicate(void) const override;
		
		public:
			
			C4API TextSetting(Type identifier, const char *text, const char *title, int32 maxLen, EditTextWidget::FilterProc *filterProc = nullptr);
			C4API TextSetting(Type identifier, float value, const char *title);
			C4API ~TextSetting();
			
			const char *GetText(void) const
			{
				return (textValue);
			}
			
			void SetText(const char *text)
			{
				textValue = text;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			void Copy(const Setting *setting);
			bool operator ==(const Setting& setting) const;
			
			bool SetValue(const Value *value);
	};
	
	
	class TextSettingInterface : public SettingInterface
	{
		private:
			
			int32									maxTextLength;
			EditTextWidget::FilterProc				*textFilterProc;
			
			EditTextWidget							*textWidget;
			ImageWidget								*stripesWidget;

			WidgetObserver<TextSettingInterface>	textWidgetObserver;
			
			void HandleTextWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			TextSettingInterface(TextSetting *setting, const char *title, int32 maxLen, EditTextWidget::FilterProc *filterProc = nullptr);
			~TextSettingInterface();
			
			bool DeterminantValue(void) const;
			void SetDeterminantValue(void);
			void SetIndeterminantValue(void);
			
			bool ExtractCurrentSetting(void);
	};
	
	
	//# \class	MenuSetting		A multi-valued setting represented by a popup menu.
	//
	//# The $MenuSetting$ class is used for a multi-valued setting represented by a popup menu.
	//
	//# \def	class MenuSetting : public Setting
	//
	//# \ctor	MenuSetting(Type identifier, int32 selection, const char *title, int32 itemCount);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	selection		The initial selection for the setting.
	//# \param	title			The title of the setting.
	//# \param	itemCount		The number of items that will appear in the menu.
	//
	//# \desc
	//# The $MenuSetting$ class represents a setting that displays a popup menu and has an
	//# integer value in the range [0,&nbsp;<i>n</i>&nbsp;&minus;&nbsp;1], where <i>n</i> is
	//# the number of menu items specified by the $itemCount$ parameter.
	//#
	//# After a $MenuSetting$ is created, the $@MenuSetting::SetMenuItemString@$ function should
	//# be called for each menu item to specify its text.
	//
	//# \base	Setting		A $MenuSetting$ is a specific type of $@Setting@$.
	
	
	//# \function	MenuSetting::SetMenuItemString		Sets the name of a single menu item.
	//
	//# \proto	void SetMenuItemString(int32 index, const char *string);
	//
	//# \param	index		The index of the menu item whose name is being set.
	//# \param	string		The name of the menu item.
	//
	//# \desc
	//# The $SetMenuItemString$ function is used to specify the text for a particular menu item
	//# belonging to a $MenuSetting$ object. This function should be called for each menu item
	//# after a $MenuSetting$ object has been created with the $index$ parameter ranging from
	//# 0 to <i>n</i>&nbsp;&minus;&nbsp;1, where <i>n</i> is the number of menu items specified
	//# by the $itemCount$ parameter of the $MenuSetting$ constructor.
	
	
	//# \function	MenuSetting::GetMenuSelection		Returns the menu selection for the setting.
	//
	//# \proto	int32 GetMenuSelection(void) const;
	//
	//# \desc
	//# The $GetMenuSelection$ function returns the index of the menu selection stored in the setting object.
	
	
	class MenuSetting : public Setting
	{
		friend class Setting;
		
		private:
			
			int32		menuSelection;
			
			MenuSetting();
			MenuSetting(const MenuSetting& menuSetting);
			
			Setting *Replicate(void) const override;
		
		public:
			
			C4API MenuSetting(Type identifier, int32 selection, const char *title, int32 itemCount);
			C4API ~MenuSetting();
			
			int32 GetMenuSelection(void) const
			{
				return (menuSelection);
			}
			
			void SetMenuSelection(int32 selection)
			{
				menuSelection = selection;
			}
			
			void SetMenuItemString(int32 index, const char *string);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			void Copy(const Setting *setting);
			bool operator ==(const Setting& setting) const;
			
			bool SetValue(const Value *value);
	};
	
	
	class MenuSettingInterface : public SettingInterface
	{
		private:
			
			int32				menuItemCount;
			const char			**menuString;

			PopupMenuWidget							*menuWidget;
			WidgetObserver<MenuSettingInterface>	menuWidgetObserver;
			
			void HandleMenuWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			MenuSettingInterface(MenuSetting *setting, const char *title, int32 itemCount);
			~MenuSettingInterface();
			
			void SetMenuItemString(int32 index, const char *string)
			{
				menuString[index] = string;
			}
			
			bool DeterminantValue(void) const;
			void SetDeterminantValue(void);
			void SetIndeterminantValue(void);
			
			bool ExtractCurrentSetting(void);
	};
	
	
	inline void MenuSetting::SetMenuItemString(int32 index, const char *string)
	{
		static_cast<MenuSettingInterface *>(GetSettingInterface())->SetMenuItemString(index, string);
	}
	
	
	//# \class	ColorSetting		A color setting represented by a color box.
	//
	//# The $ColorSetting$ class is used for a color setting represented by a color box.
	//
	//# \def	class ColorSetting : public Setting
	//
	//# \ctor	ColorSetting(Type identifier, const ColorRGBA& color, const char *title, const char *picker, unsigned_int32 flags = 0);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	color			The initial color for the setting.
	//# \param	title			The title of the setting.
	//# \param	picker			The title of the color picker dialog.
	//# \param	flags			The flags passed to the color picker dialog.
	//
	//# \desc
	//# The $ColorSetting$ class represents a setting that displays a color box and has an
	//# RGBA color value. The $flags$ parameter specifies the color picker flags that are
	//# passed to the $@InterfaceMgr/ColorPicker@$ constructor when the user clicks on the
	//# color box. The flags can be 0 or the following value.
	//
	//# \table	ColorPickerFlags
	//
	//# \base	Setting		A $ColorSetting$ is a specific type of $@Setting@$.
	
	
	//# \function	ColorSetting::GetColor		Returns the color stored in the setting.
	//
	//# \proto	const ColorRGBA& GetColor(void) const;
	//
	//# \desc
	//# The $GetColor$ function returns the RGBA color value stored in the setting object.
	//
	//# \also	$@Math/ColorRGBA@$
	
	
	class ColorSetting : public Setting
	{
		friend class Setting;
		
		private:
			
			ColorRGBA		colorValue;
			
			Setting *Replicate(void) const override;
		
		protected:
			
			ColorSetting(SettingType type = kSettingColor);
			ColorSetting(SettingType type, Type identifier, const ColorRGBA& color);
			ColorSetting(const ColorSetting& colorSetting);
		
		public:
			
			C4API ColorSetting(Type identifier, const ColorRGBA& color, const char *title, const char *picker, unsigned_int32 flags = 0);
			C4API ~ColorSetting();
			
			const ColorRGBA& GetColor(void) const
			{
				return (colorValue);
			}
			
			void SetColor(const ColorRGBA& color)
			{
				colorValue = color;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			void Copy(const Setting *setting);
			bool operator ==(const Setting& setting) const;
			
			bool SetValue(const Value *value);
	};
	
	
	class ColorSettingInterface : public SettingInterface
	{
		private:
			
			String<kMaxSettingTitleLength>			pickerString;
			unsigned_int32							pickerFlags;
			
			ColorWidget								*colorWidget;
			ImageWidget								*stripesWidget;
			
			WidgetObserver<ColorSettingInterface>	colorWidgetObserver;
			
			void HandleColorWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			ColorSettingInterface(ColorSetting *setting, const char *title, const char *picker, unsigned_int32 flags = 0);
			~ColorSettingInterface();
			
			bool DeterminantValue(void) const;
			void SetDeterminantValue(void);
			void SetIndeterminantValue(void);
			
			bool ExtractCurrentSetting(void);
	};
	
	
	//# \class	CheckColorSetting		A color setting represented by a color box with a check box for enable/disable.
	//
	//# The $CheckColorSetting$ class is used for a color setting represented by a color box with a check box for enable/disable.
	//
	//# \def	class CheckColorSetting : public ColorSetting
	//
	//# \ctor	CheckColorSetting(Type identifier, bool check, const ColorRGBA& color, const char *title, const char *picker, unsigned_int32 flags = 0);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	check			The initial value for the checkbox.
	//# \param	color			The initial color for the setting.
	//# \param	title			The title of the setting.
	//# \param	picker			The title of the color picker dialog.
	//# \param	flags			The flags passed to the color picker dialog.
	//
	//# \desc
	//# The $CheckColorSetting$ class represents a setting that displays a color box and has an
	//# RGBA color value. This setting also displays a check box and contains an extra boolean value.
	//
	//# \base	ColorSetting	A $CheckColorSetting$ is a special type of $@ColorSetting@$.
	
	
	//# \function	CheckColorSetting::GetCheckValue	Returns the check value stored in the setting.
	//
	//# \proto	bool GetCheckValue(void) const;
	//
	//# \desc
	//# The $GetCheckValue$ function returns the boolean value stored in the setting object.
	
	
	class CheckColorSetting : public ColorSetting
	{
		friend class Setting;
		
		private:
			
			bool		checkValue;
			
			CheckColorSetting();
			CheckColorSetting(const CheckColorSetting& checkColorSetting);
			
			Setting *Replicate(void) const override;
		
		public:
			
			C4API CheckColorSetting(Type identifier, bool check, const ColorRGBA& color, const char *title, const char *picker, unsigned_int32 flags = 0);
			C4API ~CheckColorSetting();
			
			bool GetCheckValue(void) const
			{
				return (checkValue);
			}
			
			void SetCheckValue(bool value)
			{
				checkValue = value;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			void Copy(const Setting *setting);
			bool operator ==(const Setting& setting) const;
	};
	
	
	class CheckColorSettingInterface : public SettingInterface
	{
		private:
			
			String<kMaxSettingTitleLength>	pickerString;
			unsigned_int32					pickerFlags;
			
			CheckWidget						*checkWidget;
			ColorWidget						*colorWidget;
			ImageWidget						*stripesWidget;
			
			WidgetObserver<CheckColorSettingInterface>		checkWidgetObserver;
			WidgetObserver<CheckColorSettingInterface>		colorWidgetObserver;
			
			void HandleCheckWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleColorWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			CheckColorSettingInterface(CheckColorSetting *setting, const char *title, const char *picker, unsigned_int32 flags = 0);
			~CheckColorSettingInterface();
			
			bool DeterminantValue(void) const;
			void SetDeterminantValue(void);
			void SetIndeterminantValue(void);
			
			bool ExtractCurrentSetting(void);
	};
	
	
	class FilePickerSettingInterface : public SettingInterface
	{
		protected:
			
			ResourceName					subdirectory;
			unsigned_int32					settingFlags;
			
			String<kMaxSettingTitleLength>	pickerString;
			const ResourceDescriptor		*resourceDescriptor;
			
			EditTextWidget					*resourceWidget;
			GuiButtonWidget					*browseWidget;
			ImageWidget						*stripesWidget;
			
			FilePickerSettingInterface(Setting *setting, const char *title, const char *picker, const ResourceDescriptor *descriptor, const char *subdir, unsigned_int32 flags);
		
		public:
			
			~FilePickerSettingInterface();
			
			bool DeterminantValue(void) const;
			void SetIndeterminantValue(void);
	};
	
	
	//# \class	ResourceSetting		A resource name setting represented by a text box and a browse button.
	//
	//# The $ResourceSetting$ class is used for a resource name setting represented by a text box and a browse button.
	//
	//# \def	class ResourceSetting : public Setting
	//
	//# \ctor	ResourceSetting(Type identifier, const char *name, const char *title, const char *picker,
	//# \ctor2	const ResourceDescriptor *descriptor, const char *subdir = nullptr, unsigned_int32 flags = 0);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	name			The initial resource name for the setting.
	//# \param	title			The title of the setting.
	//# \param	picker			The title of the file picker dialog.
	//# \param	descriptor		A pointer to the resource descriptor for the type of resource that can be chosen with the setting.
	//# \param	subdir			A subdirectory within the main resource directory to which the resource selection should be restricted.
	//# \param	flags			Flags that affect the behavior of the setting.
	//
	//# \desc
	//# The $ResourceSetting$ class represents a setting that displays a text box and has a
	//# resource name value. A button is also displayed that causes a file picker dialog to appear
	//# when clicked by the user. When a file is chosen through the file picker, the returned resource
	//# name is the virtual path to the resource if the $flags$ parameter is 0. If the following value
	//# is specified for the $flags$ parameter, then the generic path to the resource is returned.
	//
	//# \table	ResourceSettingFlags
	//
	//# \base	Setting		A $ResourceSetting$ is a specific type of $@Setting@$.
	
	
	//# \function	ResourceSetting::GetResourceName		Returns the resource name stored in the setting.
	//
	//# \proto	const ResourceName& GetResourceName(void) const;
	//
	//# \desc
	//# The $GetResourceName$ function returns the resource name stored in the setting object.
	
	
	class ResourceSetting : public Setting
	{
		friend class Setting;
		
		private:
			
			ResourceName		resourceName;
			
			ResourceSetting();
			ResourceSetting(const ResourceSetting& resourceSetting);
			
			Setting *Replicate(void) const override;
		
		public:
			
			C4API ResourceSetting(Type identifier, const char *name, const char *title, const char *picker, const ResourceDescriptor *descriptor, const char *subdir = nullptr, unsigned_int32 flags = 0);
			C4API ~ResourceSetting();
			
			const ResourceName& GetResourceName(void) const
			{
				return (resourceName);
			}
			
			void SetResourceName(const char *name)
			{
				resourceName = name;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			void Copy(const Setting *setting);
			bool operator ==(const Setting& setting) const;
			
			bool SetValue(const Value *value);
	};
	
	
	class ResourceSettingInterface : public FilePickerSettingInterface
	{
		private:
			
			WidgetObserver<ResourceSettingInterface>	browseWidgetObserver;
			WidgetObserver<ResourceSettingInterface>	resourceWidgetObserver;
			
			void HandleBrowseWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleResourceWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			static void ResourcePickProc(FilePicker *picker, void *cookie);
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			ResourceSettingInterface(ResourceSetting *setting, const char *title, const char *picker, const ResourceDescriptor *descriptor, const char *subdir, unsigned_int32 flags);
			~ResourceSettingInterface();
			
			void SetDeterminantValue(void);
			bool ExtractCurrentSetting(void);
	};
	
	
	//# \class	MultiResourceSetting		A setting showing a list of resource names in a text box with a browse button.
	//
	//# The $MultiResourceSetting$ class is used for a setting showing a list of resource names in a text box with a browse button.
	//
	//# \def	class MultiResourceSetting : public Setting
	//
	//# \ctor	ResourceSetting(Type identifier, const char *title, const char *picker,
	//# \ctor2	const ResourceDescriptor *descriptor, const char *subdir = nullptr, unsigned_int32 flags = 0);
	//
	//# \param	identifier		The setting's unique identifier.
	//# \param	title			The title of the setting.
	//# \param	picker			The title of the file picker dialog.
	//# \param	descriptor		A pointer to the resource descriptor for the type of resource that can be chosen with the setting.
	//# \param	subdir			A subdirectory within the main resource directory to which the resource selection should be restricted.
	//# \param	flags			Flags that affect the behavior of the setting.
	//
	//# \desc
	//# The $MultiResourceSetting$ class represents a setting that displays a text box and has a
	//# value which is a list of resource names. A button is also displayed that causes a file picker dialog
	//# to appear when clicked by the user. When a file is chosen through the file picker, the returned resource
	//# name is the virtual path to the resource if the $flags$ parameter is 0. If the following value
	//# is specified for the $flags$ parameter, then the generic path to the resource is returned.
	//
	//# \table	ResourceSettingFlags
	//
	//# When new files are chosen in the file picker, they are added to the list of resource names.
	//
	//# \base	Setting		A $MultiResourceSetting$ is a specific type of $@Setting@$.
	
	
	//# \function	MultiResourceSetting::GetResourceCount		Returns the number of resource names stored in the setting.
	//
	//# \proto	int32 GetResourceCount(void) const;
	//
	//# \desc
	//# The $GetResourceCount$ function returns the number of resource names stored in the setting object.
	//# The individual resource names can be retrieved using the $@MultiResourceSetting::GetResourceName@$ function.
	//
	//# \also	$@MultiResourceSetting::GetResourceName@$
	
	
	//# \function	MultiResourceSetting::GetResourceName		Returns an individual resource name stored in the setting.
	//
	//# \proto	ResourceName GetResourceName(int32 index) const;
	//
	//# \desc
	//# The $GetResourceName$ function returns the individual resource name corresponding to the
	//# index specified by the $index$ parameter. The value of $index$ should be in the range
	//# [0,&nbsp;<i>n</i>&nbsp;&minus;&nbsp;1], where <i>n</i> is the number of resource names
	//# returned by the $@MultiResourceSetting::GetResourceCount@$ function.
	//
	//# \also	$@MultiResourceSetting::GetResourceName@$
	
	
	class MultiResourceSetting : public Setting
	{
		friend class Setting;
		
		private:
			
			String<>		resourceList;
			
			MultiResourceSetting();
			MultiResourceSetting(const MultiResourceSetting& multiResourceSetting);
			
			Setting *Replicate(void) const override;
		
		public:
			
			C4API MultiResourceSetting(Type identifier, const char *title, const char *picker, const ResourceDescriptor *descriptor, const char *subdir = nullptr, unsigned_int32 flags = 0);
			C4API ~MultiResourceSetting();
			
			const String<>& GetResourceList(void) const
			{
				return (resourceList);
			}
			
			void SetResourceList(const char *list)
			{
				resourceList = list;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			void Copy(const Setting *setting);
			bool operator ==(const Setting& setting) const;
			
			bool SetValue(const Value *value);
			
			C4API void AddResourceName(const char *name);
			
			C4API int32 GetResourceCount(void) const;
			C4API ResourceName GetResourceName(int32 index) const;
	};
	
	
	class MultiResourceSettingInterface : public FilePickerSettingInterface
	{
		private:
			
			WidgetObserver<MultiResourceSettingInterface>	browseWidgetObserver;
			WidgetObserver<MultiResourceSettingInterface>	resourceWidgetObserver;
			
			void HandleBrowseWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleResourceWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			static void ResourcePickProc(FilePicker *picker, void *cookie);
			
			void BuildInterface(Widget *group, const ConfigurationWidget *config);
		
		public:
			
			MultiResourceSettingInterface(MultiResourceSetting *setting, const char *title, const char *picker, const ResourceDescriptor *descriptor, const char *subdir, unsigned_int32 flags);
			~MultiResourceSettingInterface();
			
			void SetDeterminantValue(void);
			bool ExtractCurrentSetting(void);
	};
	
	
	//# \class	ConfigurationWidget		The interface widget that displays a configuration table.
	//
	//# The $ConfigurationWidget$ class represents an interface widget that displays a configuration table.
	//
	//# \def	class ConfigurationWidget : public RenderableWidget
	//
	//# \ctor	ConfigurationWidget(const Vector2D& size, float titleFraction, unsigned_int32 flags = 0);
	//
	//# \param	size			The size of the configuration widget, in pixels.
	//# \param	titleFraction	The fraction of the horizontal width dedicated to displaying setting titles.
	//# \param	flags			The configuration widget flags. This is used internally and should be set to zero.
	//
	//# \desc
	//# The $ConfigurationWidget$ class displays a configuration table that is used to show property settings to the user.
	//#
	//# The default widget color corresponds to the $kWidgetColorBorder$ color type and controls the color of the configuration
	//# table's outer border. Other color types supported by the configuration widget are $kWidgetColorLine$ and $kWidgetColorBackground$.
	//
	//# \base	RenderableWidget	All rendered interface widgets are subclasses of $RenderableWidget$.
	
	
	class ConfigurationWidget : public RenderableWidget
	{
		friend class WidgetReg<ConfigurationWidget>;
		
		private:
			
			unsigned_int32							configurationFlags;
			unsigned_int32							colorOverrideFlags;
			float									titleColumnFraction;
			
			ColorRGBA								lineColor;
			ColorRGBA								backgroundColor;
			
			float									fullSettingWidth;
			float									titleColumnWidth;
			float									valueColumnWidth;
			float									valueColumnPosition;
			
			int32									configurationSettingCount;
			int32									displaySettingCount;
			int32									displaySettingIndex;
			
			List<Setting>							settingList;
			
			char									*vertexStorage;
			Point2D									*configurationVertex;
			ColorRGBA								*configurationColor;
			
			WidgetObserver<ConfigurationWidget>		scrollObserver;
			
			List<Attribute>							borderAttributeList;
			DiffuseAttribute						borderDiffuseAttribute;
			TextureMapAttribute						borderTextureMapAttribute;
			Renderable								borderRenderable;
			
			Point2D									borderVertex[16];
			Point2D									borderTexcoord[16];
			
			Widget									settingGroup;
			ScrollWidget							scrollWidget;
			
			ConfigurationWidget();
			ConfigurationWidget(const ConfigurationWidget& configurationWidget);
			
			Widget *Replicate(void) const override;
			
			void SetDefaultLineColor(void);
			
			void CalculateColumnWidths(void);
			void CalculateStructure(void) override;
			
			void HandleScrollEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			C4API ConfigurationWidget(const Vector2D& size, float titleFraction, unsigned_int32 flags = 0);
			C4API ~ConfigurationWidget();
			
			unsigned_int32 GetConfigurationFlags(void) const
			{
				return (configurationFlags);
			}
			
			void SetConfigurationFlags(unsigned_int32 flags)
			{
				configurationFlags = flags;
			}
			
			float GetFullSettingWidth(void) const
			{
				return (fullSettingWidth);
			}
			
			float GetTitleColumnWidth(void) const
			{
				return (titleColumnWidth);
			}
			
			float GetValueColumnWidth(void) const
			{
				return (valueColumnWidth);
			}
			
			float GetValueColumnPosition(void) const
			{
				return (valueColumnPosition);
			}
			
			Setting *GetFirstSetting(void) const
			{
				return (settingList.First());
			}
			
			Setting *GetLastSetting(void) const
			{
				return (settingList.Last());
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			const ColorRGBA& GetWidgetColor(WidgetColorType type = kWidgetColorDefault) const;
			void SetWidgetColor(const ColorRGBA& color, WidgetColorType type = kWidgetColorDefault);
			void SetWidgetAlpha(float alpha, WidgetColorType type = kWidgetColorDefault);
			void SetDynamicWidgetColor(const ColorRGBA& color, WidgetColorType type = kWidgetColorDefault);
			void SetDynamicWidgetAlpha(float alpha, WidgetColorType type = kWidgetColorDefault);
			
			void SetWidgetSize(const Vector2D& size);
			void Preprocess(void);
			
			void Build(void);
			void Render(List<Renderable> *renderList);
			
			void HandleMouseEvent(const PanelMouseEventData *eventData);
			
			C4API void SetObserver(SettingInterface::ObserverType *observer);
			
			C4API Setting *FindSetting(Type identifier) const;
			
			C4API void BuildConfiguration(const Configurable *configurable);
			C4API void CommitConfiguration(Configurable *configurable) const;
			C4API void BuildCategoryConfiguration(const Configurable *configurable, Type category);
			C4API void CommitCategoryConfiguration(Configurable *configurable, Type category) const;
			C4API void ReleaseConfiguration(void);
	};


	template <class observerType> class ConfigurationObserver : public ExclusiveObserver<observerType, SettingInterface>
	{
		public:
			
			ConfigurationObserver(observerType *observer, void (observerType::*callback)(SettingInterface *)) : ExclusiveObserver<observerType, SettingInterface>(observer, callback)
			{
			}
	};
}


#endif

// ZYURVUR
