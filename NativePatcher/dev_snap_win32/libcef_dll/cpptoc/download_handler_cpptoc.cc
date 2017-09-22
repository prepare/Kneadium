//---THIS-FILE-WAS-PATCHED , org=D:\projects\cef_binary_3.3071.1647.win32\cpptoc\download_handler_cpptoc.cc
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
// $hash=34d34f4a980b476678152ac69842f7d0b3a95a36$
//

#include "libcef_dll/cpptoc/download_handler_cpptoc.h"
#include "libcef_dll/ctocpp/before_download_callback_ctocpp.h"
#include "libcef_dll/ctocpp/browser_ctocpp.h"
#include "libcef_dll/ctocpp/download_item_callback_ctocpp.h"
#include "libcef_dll/ctocpp/download_item_ctocpp.h"

//---kneadium-ext-begin
#include "../myext/ExportFuncAuto.h"
#include "../myext/InternalHeaderForExportFunc.h"
//---kneadium-ext-end

namespace {

// MEMBER FUNCTIONS - Body may be edited by hand.

void CEF_CALLBACK
download_handler_on_before_download(struct _cef_download_handler_t* self,
                                    cef_browser_t* browser,
                                    struct _cef_download_item_t* download_item,
                                    const cef_string_t* suggested_name,
                                    cef_before_download_callback_t* callback) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;
  // Verify param: download_item; type: refptr_diff
  DCHECK(download_item);
  if (!download_item)
    return;
  // Verify param: suggested_name; type: string_byref_const
  DCHECK(suggested_name);
  if (!suggested_name)
    return;
  // Verify param: callback; type: refptr_diff
  DCHECK(callback);
  if (!callback)
    return;

//---kneadium-ext-begin
#if ENABLE_KNEADIUM_EXT
auto me = CefDownloadHandlerCppToC::Get(self);
const int CALLER_CODE=(CefDownloadHandlerExt::_typeName << 16) | CefDownloadHandlerExt::CefDownloadHandlerExt_OnBeforeDownload_1;
auto m_callback= me->GetManagedCallBack(CALLER_CODE);
if(m_callback){
CefString tmp_arg3 (suggested_name);
CefDownloadHandlerExt::OnBeforeDownloadArgs args1(browser,download_item,tmp_arg3,callback);
m_callback(CALLER_CODE, &args1.arg);
 if (((args1.arg.myext_flags >> 21) & 1) == 1){
return;
}
}
#endif
//---kneadium-ext-end

  // Execute
  CefDownloadHandlerCppToC::Get(self)->OnBeforeDownload(
      CefBrowserCToCpp::Wrap(browser),
      CefDownloadItemCToCpp::Wrap(download_item), CefString(suggested_name),
      CefBeforeDownloadCallbackCToCpp::Wrap(callback));
}

void CEF_CALLBACK
download_handler_on_download_updated(struct _cef_download_handler_t* self,
                                     cef_browser_t* browser,
                                     struct _cef_download_item_t* download_item,
                                     cef_download_item_callback_t* callback) {
  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  DCHECK(self);
  if (!self)
    return;
  // Verify param: browser; type: refptr_diff
  DCHECK(browser);
  if (!browser)
    return;
  // Verify param: download_item; type: refptr_diff
  DCHECK(download_item);
  if (!download_item)
    return;
  // Verify param: callback; type: refptr_diff
  DCHECK(callback);
  if (!callback)
    return;

//---kneadium-ext-begin
#if ENABLE_KNEADIUM_EXT
auto me = CefDownloadHandlerCppToC::Get(self);
const int CALLER_CODE=(CefDownloadHandlerExt::_typeName << 16) | CefDownloadHandlerExt::CefDownloadHandlerExt_OnDownloadUpdated_2;
auto m_callback= me->GetManagedCallBack(CALLER_CODE);
if(m_callback){
CefDownloadHandlerExt::OnDownloadUpdatedArgs args1(browser,download_item,callback);
m_callback(CALLER_CODE, &args1.arg);
 if (((args1.arg.myext_flags >> 21) & 1) == 1){
return;
}
}
#endif
//---kneadium-ext-end

  // Execute
  CefDownloadHandlerCppToC::Get(self)->OnDownloadUpdated(
      CefBrowserCToCpp::Wrap(browser),
      CefDownloadItemCToCpp::Wrap(download_item),
      CefDownloadItemCallbackCToCpp::Wrap(callback));
}

}  // namespace

// CONSTRUCTOR - Do not edit by hand.

CefDownloadHandlerCppToC::CefDownloadHandlerCppToC() {
  GetStruct()->on_before_download = download_handler_on_before_download;
  GetStruct()->on_download_updated = download_handler_on_download_updated;
}

template <>
CefRefPtr<CefDownloadHandler> CefCppToCRefCounted<
    CefDownloadHandlerCppToC,
    CefDownloadHandler,
    cef_download_handler_t>::UnwrapDerived(CefWrapperType type,
                                           cef_download_handler_t* s) {
  NOTREACHED() << "Unexpected class type: " << type;
  return NULL;
}

#if DCHECK_IS_ON()
template <>
base::AtomicRefCount CefCppToCRefCounted<CefDownloadHandlerCppToC,
                                         CefDownloadHandler,
                                         cef_download_handler_t>::DebugObjCt =
    0;
#endif

template <>
CefWrapperType CefCppToCRefCounted<CefDownloadHandlerCppToC,
                                   CefDownloadHandler,
                                   cef_download_handler_t>::kWrapperType =
    WT_DOWNLOAD_HANDLER;
