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
// $hash=4d0908586f8101bdfd5fae36ee451789e650752d$
//

#ifndef CEF_LIBCEF_DLL_CPPTOC_READ_HANDLER_CPPTOC_H_
#define CEF_LIBCEF_DLL_CPPTOC_READ_HANDLER_CPPTOC_H_
#pragma once

#if !defined(WRAPPING_CEF_SHARED)
#error This file can be included wrapper-side only
#endif

#include "include/capi/cef_stream_capi.h"
#include "include/cef_stream.h"
#include "libcef_dll/cpptoc/cpptoc_ref_counted.h"

// Wrap a C++ class with a C structure.
// This class may be instantiated and accessed wrapper-side only.
class CefReadHandlerCppToC : public CefCppToCRefCounted<CefReadHandlerCppToC,
                                                        CefReadHandler,
                                                        cef_read_handler_t> {
 public:
  CefReadHandlerCppToC();
};

#endif  // CEF_LIBCEF_DLL_CPPTOC_READ_HANDLER_CPPTOC_H_
