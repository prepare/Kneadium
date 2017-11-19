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
// $hash=7ee7e6684f3703ede9b5dcfb40170923a9d5242e$
//

#include "libcef_dll/ctocpp/auth_callback_ctocpp.h"

// VIRTUAL METHODS - Body may be edited by hand.

void CefAuthCallbackCToCpp::Continue(const CefString& username,
                                     const CefString& password) {
  cef_auth_callback_t* _struct = GetStruct();
  if (CEF_MEMBER_MISSING(_struct, cont))
    return;

  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  // Verify param: username; type: string_byref_const
  DCHECK(!username.empty());
  if (username.empty())
    return;
  // Unverified params: password

  // Execute
  _struct->cont(_struct, username.GetStruct(), password.GetStruct());
}

void CefAuthCallbackCToCpp::Cancel() {
  cef_auth_callback_t* _struct = GetStruct();
  if (CEF_MEMBER_MISSING(_struct, cancel))
    return;

  // AUTO-GENERATED CONTENT - DELETE THIS COMMENT BEFORE MODIFYING

  // Execute
  _struct->cancel(_struct);
}

// CONSTRUCTOR - Do not edit by hand.

CefAuthCallbackCToCpp::CefAuthCallbackCToCpp() {}

template <>
cef_auth_callback_t*
CefCToCppRefCounted<CefAuthCallbackCToCpp,
                    CefAuthCallback,
                    cef_auth_callback_t>::UnwrapDerived(CefWrapperType type,
                                                        CefAuthCallback* c) {
  NOTREACHED() << "Unexpected class type: " << type;
  return NULL;
}

#if DCHECK_IS_ON()
template <>
base::AtomicRefCount CefCToCppRefCounted<CefAuthCallbackCToCpp,
                                         CefAuthCallback,
                                         cef_auth_callback_t>::DebugObjCt
    ATOMIC_DECLARATION;
#endif

template <>
CefWrapperType CefCToCppRefCounted<CefAuthCallbackCToCpp,
                                   CefAuthCallback,
                                   cef_auth_callback_t>::kWrapperType =
    WT_AUTH_CALLBACK;
