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


#ifndef C4Awesomium_h
#define C4Awesomium_h


extern "C"
{
	enum awe_loglevel
	{
		AWE_LL_NONE,
		AWE_LL_NORMAL,
		AWE_LL_VERBOSE
	};
	
	
	enum awe_mousebutton
	{
		AWE_MB_LEFT,
		AWE_MB_MIDDLE,
		AWE_MB_RIGHT
	};
	
	
	enum awe_webkey_type
	{
		AWE_WKT_KEYDOWN,
		AWE_WKT_KEYUP,
		AWE_WKT_CHAR
	};
	
	
	enum
	{
		AWE_WKM_SHIFT_KEY		= 1 << 0,
		AWE_WKM_CONTROL_KEY		= 1 << 1,
		AWE_WKM_ALT_KEY			= 1 << 2, 
		AWE_WKM_META_KEY		= 1 << 3,
		AWE_WKM_IS_KEYPAD		= 1 << 4,
		AWE_WKM_IS_AUTOREPEAT	= 1 << 5
	};
	
	
	struct awe_webview;
	struct awe_renderbuffer;
	struct awe_string;
	
	
	struct awe_webkeyboardevent
	{
		awe_webkey_type		type;
		unsigned int		modifiers;
		int					virtual_key_code;
		int					native_key_code;
		unsigned_int16		text[4];
		unsigned_int16		unmodified_text[4];
		bool				is_system_key;
	};
	
	
	struct awe_rect
	{
		int		x;
		int		y;
		int		width;
		int		height;
	};
	
	
	const awe_string *awe_string_empty(void);
	awe_string *awe_string_create_from_ascii(const char *, size_t);
	void awe_string_destroy(awe_string *);
	int awe_string_to_utf8(const awe_string *, char *, size_t);
	
	void awe_webcore_initialize(bool, bool, bool, const awe_string *, const awe_string *, const awe_string *, awe_loglevel, bool, const awe_string *, bool, const awe_string *, const awe_string *, const awe_string *, const awe_string *, const awe_string *, const awe_string *, bool, int, bool, bool, const awe_string *);
	void awe_webcore_shutdown(void);
	awe_webview *awe_webcore_create_webview(int, int, bool);
	void awe_webcore_update(void);
	
	void awe_webview_destroy(awe_webview *);
	void awe_webview_load_url(awe_webview *, const awe_string *, const awe_string *, const awe_string *, const awe_string *);
	void awe_webview_go_to_history_offset(awe_webview *, int);
	void awe_webview_stop(awe_webview *);
	void awe_webview_reload(awe_webview *);
	bool awe_webview_is_dirty(awe_webview *);
	awe_rect awe_webview_get_dirty_bounds(awe_webview *);
	const awe_renderbuffer *awe_webview_render(awe_webview *);
	void awe_webview_pause_rendering(awe_webview *);
	void awe_webview_resume_rendering(awe_webview *);
	void awe_webview_inject_mouse_move(awe_webview *, int, int);
	void awe_webview_inject_mouse_down(awe_webview *, awe_mousebutton);
	void awe_webview_inject_mouse_up(awe_webview *, awe_mousebutton);
	void awe_webview_inject_mouse_wheel(awe_webview *, int, int);
	void awe_webview_inject_keyboard_event(awe_webview *, awe_webkeyboardevent);
	void awe_webview_focus(awe_webview *);
	void awe_webview_unfocus(awe_webview *);
	void awe_webview_set_transparent(awe_webview *, bool);
	void awe_webview_set_callback_begin_navigation(awe_webview *, void (*)(awe_webview *, const awe_string *, const awe_string *));
	
	int awe_renderbuffer_get_rowspan(const awe_renderbuffer *);
	const unsigned char *awe_renderbuffer_get_buffer(const awe_renderbuffer *);
}

 
#endif

// ZYURVUR
