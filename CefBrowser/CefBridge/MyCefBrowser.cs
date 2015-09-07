﻿//2015 MIT, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace LayoutFarm.CefBridge
{

    public class MyCefDevWindow
    {

        IntPtr myCefBrowser;
        MyCefCallback managedCallback;
        internal MyCefDevWindow()
        {
            //create cef browser view handler  
            this.managedCallback = new MyCefCallback(this.MxCallBack);
            this.myCefBrowser = Cef3Binder.MyCefCreateMyWebBrowser(managedCallback);

        }
        public IntPtr GetMyCefBrowser()
        {
            return this.myCefBrowser;
        }
        void MxCallBack(int id, IntPtr argsPtr)
        {
            switch (id)
            {
                case 100:
                    {
                    }
                    break;
            }

        }
    }


    public class MyCefBrowser
    {

        internal event EventHandler BrowserDisposed;
        IntPtr myCefBrowser;
        MyCefCallback managedCallback;
        string initUrl;
        System.Windows.Forms.Control parentControl;
        System.Windows.Forms.Form topForm;
        System.Windows.Forms.Form devForm;
        MyCefDevWindow cefDevWindow;

        internal MyCefBrowser(System.Windows.Forms.Control parentControl,
            int x, int y, int w, int h, string initUrl)
        {
            this.initUrl = initUrl;
            //create cef browser view handler  
            this.parentControl = parentControl;
            this.topForm = (System.Windows.Forms.Form)parentControl.TopLevelControl;

            this.managedCallback = new MyCefCallback(this.MxCallBack);
            this.myCefBrowser = Cef3Binder.MyCefCreateMyWebBrowser(managedCallback);

            Cef3Binder.MyCefSetupBrowserHwnd(myCefBrowser, parentControl.Handle, x, y, w, h, initUrl);
        }

        internal System.Windows.Forms.Control ParentControl { get { return this.parentControl; } }
        internal System.Windows.Forms.Form ParentForm { get { return this.topForm; } }

        void MxCallBack(int id, IntPtr argsPtr)
        {
            switch (id)
            {
                case 100:
                    {
                        if (this.devForm != null)
                        {
                            this.devForm.Close();
                            this.devForm.Dispose();
                            this.devForm = null;
                        }
                        if (this.BrowserDisposed != null)
                        {
                            this.BrowserDisposed(this, EventArgs.Empty);
                        }
                    }
                    break;
                case 101:
                    {


                    }
                    break;
                case 103:
                    {
                        //create pop up window and send window handle to cef 
                        System.Windows.Forms.Form popupWin = new System.Windows.Forms.Form();
                        popupWin.Width = 600;
                        popupWin.Height = 450;
                        popupWin.Show();

                        IntPtr handle = popupWin.Handle;
                        if (argsPtr != IntPtr.Zero)
                        {
                            NativeCallArgs2 args = new NativeCallArgs2(argsPtr);
                            args.SetResult(handle);
                        }

                    }
                    break;
                case 104:
                    {
                        CefClientApp.UISafeInvoke(
                            new SimpleDel(
                            () =>
                            {
                                CefBridgeTest.Form1 newPopupForm = new CefBridgeTest.Form1();

                                NativeCallArgs args = new NativeCallArgs(argsPtr);
                                string url = args.GetArgAsString(0);
                                newPopupForm.InitUrl = url;
                                newPopupForm.Show();
                                args.Dispose();
                            }));

                    }
                    break;
                case 106:
                    {
                        //console.log ...

                        NativeCallArgs args = new NativeCallArgs(argsPtr);
                        string msg = args.GetArgAsString(0);
                        string src = args.GetArgAsString(1);
                        string location = args.GetArgAsString(2);
                        Console.WriteLine(msg);

                    }
                    break;
                case 107:
                    {
                        //show dev tools
                        CefClientApp.UISafeInvoke(new SimpleDel(
                            () =>
                            {
                                CefBridgeTest.Form1 newPopupForm = new CefBridgeTest.Form1();
                                newPopupForm.Show();
                            }));

                    }
                    break;

                case 140:
                    //setup resource mx
                    {

                        var args = new NativeCallArgs(argsPtr);
                        var resourceMx = new NativeResourceMx(args.GetArgAsNativePtr(0));
                        AddResourceProvider(resourceMx);
                    }
                    break;

                case 142:
                    {

                        //filter url name
                        var args = new NativeCallArgs(argsPtr);
                        string reqUrl = args.GetArgAsString(0);
                        if (reqUrl.StartsWith("http://localhost/index2"))
                        {
                            //eg. how to fix request url

                            args.SetOutput(0, 1);
                            //return url-in ascii form 
                            var utf8Buffer = Encoding.ASCII.GetBytes("http://localhost/index2.html");
                            args.SetOutput(1, utf8Buffer);
                        }
                    }
                    break;
                case 145:
                    {
                        //request for binary resource
                        var args = new NativeCallArgs(argsPtr);
                        string url = args.GetArgAsString(0);
                        if (url == "http://localhost/hello_img")
                        {
                            //load sample image and the send to client
                            byte[] img = File.ReadAllBytes("prepare.jpg");
                            int imgLen = img.Length;
                            IntPtr unmanagedPtr = Marshal.AllocHGlobal(imgLen);
                            Marshal.Copy(img, 0, unmanagedPtr, imgLen);

                            args.SetOutput(0, 1);
                            args.UnsafeSetOutput(1, unmanagedPtr, imgLen);
                            args.SetOutputAsAsciiString(2, "image/jpeg");
                        }
                    }
                    break;
                //------------------------------
                //eg. from cefQuery --> 
                case 205:
                    {
                        var args = new NativeCallArgs(argsPtr);
                        QueryRequestArgs reqArgs = QueryRequestArgs.CreateRequest(args.GetArgAsNativePtr(0));
                        HandleCefQueryRequest(reqArgs);
                    }
                    break;
            }

        }

        void HandleCefQueryRequest(QueryRequestArgs reqArgs)
        {
            //filter url  before get actual resource
            string frameUrl = reqArgs.GetFrameUrl();

        }
        void AddResourceProvider(NativeResourceMx resourceMx)
        {
            var resProvider = new ResourceProvider();
            resourceMx.AddResourceProvider(resProvider);


        }
        public void NavigateTo(string url)
        {
            Cef3Binder.MyCefBwNavigateTo(this.myCefBrowser, url);
        }
        public void ExecJavascript(string src, string scriptUrl)
        {
            Cef3Binder.MyCefBwExecJavascript(this.myCefBrowser, src, scriptUrl);
        }
        public void PostData(string url, byte[] data, int len)
        {
            Cef3Binder.MyCefBwPostData(this.myCefBrowser, url, data, len);
        }



        List<MyCefCallback> keepAliveCallBack = new List<MyCefCallback>();
        public void GetText(Action<string> strCallback)
        {
            //keep alive callback
            InternalGetText((id, nativePtr) =>
            {
                var args = new NativeCallArgs(nativePtr);
                string str = args.GetArgAsString(0);
                strCallback(str);
            });
            //Cef3Binder.MyCefDomGetTextWalk(this.myCefBrowser, strCallback);
        }
        public void GetSource(Action<string> strCallback)
        {
            //keep alive callback
            InternalGetSource((id, nativePtr) =>
            {


            });
            //Cef3Binder.MyCefDomGetSourceWalk(this.myCefBrowser, strCallback);
        }
        void InternalGetSource(MyCefCallback strCallback)
        {
            //keep alive callback
            keepAliveCallBack.Add(strCallback);
            Cef3Binder.MyCefDomGetSourceWalk(this.myCefBrowser, strCallback);
        }
        void InternalGetText(MyCefCallback strCallback)
        {
            //keep alive callback
            keepAliveCallBack.Add(strCallback);
            Cef3Binder.MyCefDomGetTextWalk(this.myCefBrowser, strCallback);
        }

        //void OnUnmanagedPartCallBack(int id, IntPtr callBackArgs)
        //{

        //    switch ((MethodName)id)
        //    {
        //        case MethodName.MET_GetResourceHandler:
        //            {
        //                GetResourceHandler(new ResourceRequestArg(
        //                    new NativeCallArgs(callBackArgs)));
        //            }
        //            break;
        //        case MethodName.MET_TCALLBACK:
        //            {
        //                //Console.WriteLine("TCALLBACK");
        //            }
        //            break;
        //    }
        //}
        //protected virtual void GetResourceHandler(ResourceRequestArg req)
        //{
        //    //sample here
        //    string requestURL = req.RequestUrl;
        //    //test change content here 
        //    if (requestURL.StartsWith("http://www.google.com"))
        //    {
        //        req.SetResponseData("text/html", @"<http><body>Hello!</body></http>");
        //    }
        //}

        //public class ResourceRequestArg
        //{
        //    NativeCallArgs nativeArgs;
        //    internal ResourceRequestArg(NativeCallArgs nativeArgs)
        //    {
        //        this.nativeArgs = nativeArgs;
        //    }
        //    public string RequestUrl
        //    {
        //        get
        //        {
        //            return this.nativeArgs.GetArgAsString(0);
        //        }
        //    }
        //    public void SetResponseData(string mime, string str)
        //    {
        //        nativeArgs.SetOutput(0, mime);
        //        var utf8Buffer = Encoding.UTF8.GetBytes(str.ToCharArray());
        //        nativeArgs.SetOutput(1, utf8Buffer);
        //    }
        //    public void SetResponseData(string mime, byte[] dataBuffer)
        //    {
        //        nativeArgs.SetOutput(0, mime);
        //        nativeArgs.SetOutput(1, dataBuffer);
        //    }
        //}

        public void Stop()
        {
            Cef3Binder.MyCefBwStop(myCefBrowser);
        }
        public void GoBack()
        {
            Cef3Binder.MyCefBwGoBack(myCefBrowser);
        }
        public void GoForward()
        {
            Cef3Binder.MyCefBwGoForward(myCefBrowser);
        }
        public void Reload()
        {
            Cef3Binder.MyCefBwReload(myCefBrowser);
        }

        public void ShowDevTools()
        {
            if (cefDevWindow == null)
            {
                cefDevWindow = new MyCefDevWindow();
                devForm = new System.Windows.Forms.Form();
                devForm.Show();

                Cef3Binder.MyCefShowDevTools(this.myCefBrowser, cefDevWindow.GetMyCefBrowser(), devForm.Handle);
            }
        }
        //---------
        //map with unmanaged part
        enum MethodName
        {
            MET_GetResourceHandler = 2,
            MET_TCALLBACK = 3
        }
    }





}