//---THIS-FILE-IS-PATCHED , org=D:\projects\cef_binary_3.3071.1647.win32\cpptoc\menu_model_delegate_cpptoc.cc
// Copyright (c) 2017 The Chromium Embedded Framework Authors. All rights
// reserved. Use of this source code is governed by a BSD-style license that
// can be found in the LICENSE file.
//
// ---------------------------------------------------------------------------
//
// This file was generated by the CEF translator tool. If making changes by
// hand only do so within the body of existing method and function
// implementations. See the translator.README.txt file in the tools directory
// for more information.
//
// $hash=8df90eb582df20061513fb80264da53b9d97849b$
//

#include "libcef_dll/cpptoc/menu_model_delegate_cpptoc.h"
#include "libcef_dll/ctocpp/menu_model_ctocpp.h"

//---kneadium-ext-begin
#include "../myext/ExportFuncAuto.h"
#include "../myext/InternalHeaderForExportFunc.h"
//---kneadium-ext-end

namespace {

	// MEMBER FUNCTIONS - Body may be edited by hand.

	void CEF_CALLBACK
		menu_model_delegate_execute_command(struct _cef_menu_model_delegate_t* self,
			cef_menu_model_t* menu_model,
			int command_id,
			cef_event_flags_t event_flags) {
		// AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

		DCHECK(self);
		if (!self)
			return;
		// Verify param: menu_model; type: refptr_diff
		DCHECK(menu_model);
		if (!menu_model)
			return;

		//---kneadium-ext-begin
		auto me = CefMenuModelDelegateCppToC::Get(self);
		const int CALLER_CODE = (CefMenuModelDelegateExt::_typeName << 16) | CefMenuModelDelegateExt::CefMenuModelDelegateExt_ExecuteCommand_1;
		auto m_callback = me->GetManagedCallBack(CALLER_CODE);
		if (m_callback) {
			CefMenuModelDelegateExt::ExecuteCommandArgs args1(menu_model, command_id, event_flags);
			m_callback(CALLER_CODE, &args1.arg);
			if (((args1.arg.myext_flags >> 21) & 1) == 1) {
				return;
			}
		}
		//---kneadium-ext-end

		  // Execute
		CefMenuModelDelegateCppToC::Get(self)->ExecuteCommand(
			CefMenuModelCToCpp::Wrap(menu_model), command_id, event_flags);
	}

	void CEF_CALLBACK
		menu_model_delegate_mouse_outside_menu(struct _cef_menu_model_delegate_t* self,
			cef_menu_model_t* menu_model,
			const cef_point_t* screen_point) {
		// AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

		DCHECK(self);
		if (!self)
			return;
		// Verify param: menu_model; type: refptr_diff
		DCHECK(menu_model);
		if (!menu_model)
			return;
		// Verify param: screen_point; type: simple_byref_const
		DCHECK(screen_point);
		if (!screen_point)
			return;

		// Translate param: screen_point; type: simple_byref_const
		CefPoint screen_pointVal = screen_point ? *screen_point : CefPoint();

		//---kneadium-ext-begin
		auto me = CefMenuModelDelegateCppToC::Get(self);
		const int CALLER_CODE = (CefMenuModelDelegateExt::_typeName << 16) | CefMenuModelDelegateExt::CefMenuModelDelegateExt_MouseOutsideMenu_2;
		auto m_callback = me->GetManagedCallBack(CALLER_CODE);
		if (m_callback) {
			CefMenuModelDelegateExt::MouseOutsideMenuArgs args1(menu_model, &screen_pointVal);
			m_callback(CALLER_CODE, &args1.arg);
			if (((args1.arg.myext_flags >> 21) & 1) == 1) {
				return;
			}
		}
		//---kneadium-ext-end

		  // Execute
		CefMenuModelDelegateCppToC::Get(self)->MouseOutsideMenu(
			CefMenuModelCToCpp::Wrap(menu_model), screen_pointVal);
	}

	void CEF_CALLBACK menu_model_delegate_unhandled_open_submenu(
		struct _cef_menu_model_delegate_t* self,
		cef_menu_model_t* menu_model,
		int is_rtl) {
		// AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

		DCHECK(self);
		if (!self)
			return;
		// Verify param: menu_model; type: refptr_diff
		DCHECK(menu_model);
		if (!menu_model)
			return;

		//---kneadium-ext-begin
		auto me = CefMenuModelDelegateCppToC::Get(self);
		const int CALLER_CODE = (CefMenuModelDelegateExt::_typeName << 16) | CefMenuModelDelegateExt::CefMenuModelDelegateExt_UnhandledOpenSubmenu_3;
		auto m_callback = me->GetManagedCallBack(CALLER_CODE);
		if (m_callback) {
			CefMenuModelDelegateExt::UnhandledOpenSubmenuArgs args1(menu_model, is_rtl);
			m_callback(CALLER_CODE, &args1.arg);
			if (((args1.arg.myext_flags >> 21) & 1) == 1) {
				return;
			}
		}
		//---kneadium-ext-end

		  // Execute
		CefMenuModelDelegateCppToC::Get(self)->UnhandledOpenSubmenu(
			CefMenuModelCToCpp::Wrap(menu_model), is_rtl ? true : false);
	}

	void CEF_CALLBACK menu_model_delegate_unhandled_close_submenu(
		struct _cef_menu_model_delegate_t* self,
		cef_menu_model_t* menu_model,
		int is_rtl) {
		// AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

		DCHECK(self);
		if (!self)
			return;
		// Verify param: menu_model; type: refptr_diff
		DCHECK(menu_model);
		if (!menu_model)
			return;

		//---kneadium-ext-begin
		auto me = CefMenuModelDelegateCppToC::Get(self);
		const int CALLER_CODE = (CefMenuModelDelegateExt::_typeName << 16) | CefMenuModelDelegateExt::CefMenuModelDelegateExt_UnhandledCloseSubmenu_4;
		auto m_callback = me->GetManagedCallBack(CALLER_CODE);
		if (m_callback) {
			CefMenuModelDelegateExt::UnhandledCloseSubmenuArgs args1(menu_model, is_rtl);
			m_callback(CALLER_CODE, &args1.arg);
			if (((args1.arg.myext_flags >> 21) & 1) == 1) {
				return;
			}
		}
		//---kneadium-ext-end

		  // Execute
		CefMenuModelDelegateCppToC::Get(self)->UnhandledCloseSubmenu(
			CefMenuModelCToCpp::Wrap(menu_model), is_rtl ? true : false);
	}

	void CEF_CALLBACK
		menu_model_delegate_menu_will_show(struct _cef_menu_model_delegate_t* self,
			cef_menu_model_t* menu_model) {
		// AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

		DCHECK(self);
		if (!self)
			return;
		// Verify param: menu_model; type: refptr_diff
		DCHECK(menu_model);
		if (!menu_model)
			return;

		//---kneadium-ext-begin
		auto me = CefMenuModelDelegateCppToC::Get(self);
		const int CALLER_CODE = (CefMenuModelDelegateExt::_typeName << 16) | CefMenuModelDelegateExt::CefMenuModelDelegateExt_MenuWillShow_5;
		auto m_callback = me->GetManagedCallBack(CALLER_CODE);
		if (m_callback) {
			CefMenuModelDelegateExt::MenuWillShowArgs args1(menu_model);
			m_callback(CALLER_CODE, &args1.arg);
			if (((args1.arg.myext_flags >> 21) & 1) == 1) {
				return;
			}
		}
		//---kneadium-ext-end

		  // Execute
		CefMenuModelDelegateCppToC::Get(self)->MenuWillShow(
			CefMenuModelCToCpp::Wrap(menu_model));
	}

	void CEF_CALLBACK
		menu_model_delegate_menu_closed(struct _cef_menu_model_delegate_t* self,
			cef_menu_model_t* menu_model) {
		// AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

		DCHECK(self);
		if (!self)
			return;
		// Verify param: menu_model; type: refptr_diff
		DCHECK(menu_model);
		if (!menu_model)
			return;

		//---kneadium-ext-begin
		auto me = CefMenuModelDelegateCppToC::Get(self);
		const int CALLER_CODE = (CefMenuModelDelegateExt::_typeName << 16) | CefMenuModelDelegateExt::CefMenuModelDelegateExt_MenuClosed_6;
		auto m_callback = me->GetManagedCallBack(CALLER_CODE);
		if (m_callback) {
			CefMenuModelDelegateExt::MenuClosedArgs args1(menu_model);
			m_callback(CALLER_CODE, &args1.arg);
			if (((args1.arg.myext_flags >> 21) & 1) == 1) {
				return;
			}
		}
		//---kneadium-ext-end

		  // Execute
		CefMenuModelDelegateCppToC::Get(self)->MenuClosed(
			CefMenuModelCToCpp::Wrap(menu_model));
	}

	int CEF_CALLBACK
		menu_model_delegate_format_label(struct _cef_menu_model_delegate_t* self,
			cef_menu_model_t* menu_model,
			cef_string_t* label) {
		// AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

		DCHECK(self);
		if (!self)
			return 0;
		// Verify param: menu_model; type: refptr_diff
		DCHECK(menu_model);
		if (!menu_model)
			return 0;
		// Verify param: label; type: string_byref
		DCHECK(label);
		if (!label)
			return 0;

		// Translate param: label; type: string_byref
		CefString labelStr(label);

		//---kneadium-ext-begin
		auto me = CefMenuModelDelegateCppToC::Get(self);
		const int CALLER_CODE = (CefMenuModelDelegateExt::_typeName << 16) | CefMenuModelDelegateExt::CefMenuModelDelegateExt_FormatLabel_7;
		auto m_callback = me->GetManagedCallBack(CALLER_CODE);
		if (m_callback) {
			CefString tmp_arg2(label);
			CefMenuModelDelegateExt::FormatLabelArgs args1(menu_model, tmp_arg2);
			m_callback(CALLER_CODE, &args1.arg);
			if (((args1.arg.myext_flags >> 21) & 1) == 1) {
				return args1.arg.myext_ret_value;
			}
		}
		//---kneadium-ext-end

		  // Execute
		bool _retval = CefMenuModelDelegateCppToC::Get(self)->FormatLabel(
			CefMenuModelCToCpp::Wrap(menu_model), labelStr);

		// Return type: bool
		return _retval;
	}

}  // namespace

// CONSTRUCTOR - Do not edit by hand.

CefMenuModelDelegateCppToC::CefMenuModelDelegateCppToC() {
	GetStruct()->execute_command = menu_model_delegate_execute_command;
	GetStruct()->mouse_outside_menu = menu_model_delegate_mouse_outside_menu;
	GetStruct()->unhandled_open_submenu =
		menu_model_delegate_unhandled_open_submenu;
	GetStruct()->unhandled_close_submenu =
		menu_model_delegate_unhandled_close_submenu;
	GetStruct()->menu_will_show = menu_model_delegate_menu_will_show;
	GetStruct()->menu_closed = menu_model_delegate_menu_closed;
	GetStruct()->format_label = menu_model_delegate_format_label;
}

template <>
CefRefPtr<CefMenuModelDelegate> CefCppToCRefCounted<
	CefMenuModelDelegateCppToC,
	CefMenuModelDelegate,
	cef_menu_model_delegate_t>::UnwrapDerived(CefWrapperType type,
		cef_menu_model_delegate_t* s) {
	NOTREACHED() << "Unexpected class type: " << type;
	return NULL;
}

#if DCHECK_IS_ON()
template <>
base::AtomicRefCount
CefCppToCRefCounted<CefMenuModelDelegateCppToC,
	CefMenuModelDelegate,
	cef_menu_model_delegate_t>::DebugObjCt = 0;
#endif

template <>
CefWrapperType CefCppToCRefCounted<CefMenuModelDelegateCppToC,
	CefMenuModelDelegate,
	cef_menu_model_delegate_t>::kWrapperType =
	WT_MENU_MODEL_DELEGATE;
