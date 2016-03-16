﻿//2016, MIT, WinterDev
using System;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.CefBridge;


namespace CefBridgeTest
{
    /// <summary>
    /// Cef3 init essential for WindowForm
    /// </summary>
    class MyCef3InitEssential : LayoutFarm.CefBridge.Cef3InitEssential
    {
        static Form tinyForm;
#if DEBUG
        static string libPath = @"D:\projects\CefBridge\cef3_output\cefclient\Debug";
#else
        static string libPath = @"D:\projects\CefBridge\cef3_output\cefclient\Release";
#endif

        static MyCef3InitEssential initEssential;

        private MyCef3InitEssential(string[] startArgs)
            : base(startArgs)
        {

        }
        public override string GetLibCefFileName()
        {
            return libPath + "\\libcef.dll";
        }
        public override string GetCefClientFileName()
        {
            return libPath + "\\cefclient.dll";
        }

        public override IWindowForm CreateNewWindow(int width, int height)
        {
            Form form1 = new Form();
            form1.Width = width;
            form1.Height = height;
            return new MyWindowForm(form1);
        }
        public override void SaveUIInvoke(SimpleDel simpleDel)
        {
            tinyForm.Invoke(simpleDel);
        }
        public override IWindowForm CreateNewBrowserWindow(int width, int height)
        {
            Form1 form1 = new Form1();
            form1.Width = width;
            form1.Height = height;
            return new MyWindowForm(form1);
        }
        public override void AfterProcessLoaded(CefStartArgs cefStartArg)
        {
            //if (Cef3InitEssential.IsInRenderProcess)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

#if DEBUG
            //if(Cef3InitEssential.IsInRenderProcess)
            //{
            //    Console.WriteLine("this is renderer process");
            //    System.Diagnostics.Debugger.Launch();
            //}
            //if (cefStartArg.IsValidCefArgs)
            //{
            //    if (cefStartArg.ProcessType == "renderer")
            //    {
            //        //StringBuilder stbuilder = new StringBuilder();
            //        //stbuilder.Append("rrrr");
            //        //foreach (var str in startArgs)
            //        //{
            //        //    stbuilder.Append(str + " ");
            //        //}
            //        //s_dbugIsRendererProcess = true;
            //        //System.Windows.Forms.MessageBox.Show(stbuilder.ToString(), DateTime.Now.ToString());
            //        //request debugger in render process
            //        Console.WriteLine("this is renderer process");
            //        System.Diagnostics.Debugger.Launch();

            //    }
            //    //set break point after alert if we want to stop debugger                            
            //}
#endif
        }
        public override CefClientApp CreateClientApp()
        {

            var renderProcListener = new CefRendererProcessListener();
            var clientApp = new CefClientApp(
                System.Diagnostics.Process.GetCurrentProcess().Handle,
                renderProcListener);

            return clientApp;
        }

        public override IntPtr SetupPreRun()
        {

            //----------------------------------
            //2. as usual in WindowForm
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //-------------------------------------------------


            if (tinyForm == null)
            {
                tinyForm = new System.Windows.Forms.Form();
                tinyForm.Size = new System.Drawing.Size(10, 10);
                tinyForm.Visible = false;
            }
            IntPtr handle = tinyForm.Handle;//force it create handle**** 

            //Cef3's message pump
            System.Windows.Forms.Application.Idle += (sender, e) =>
            {
                DoMessageLoopWork();
            };

            return handle;
        }
        protected override void OnAfterShutdown()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }


        public static bool LoadAndInitCef3(string[] args)
        {
            initEssential = new MyCef3InitEssential(args);
            if (!initEssential.Init())
            {
                return false;
            }
            initEssential.SetupPreRun();

            return true;
        }
        public static void ShutDownCef3()
        {
            //----------------------------------
            //4. 
            initEssential.Shutdown();
            //---------------------------------- 
        }
    }


    class CefRendererProcessListener : MyCefRenderProcessListener
    {

        public override void OnContextCreated(MyCefContextArgs args)
        {
            //sample !!!
            //call window.test001() from js 
            CefV8Value cefV8Global = args.context.GetGlobal();
            Cef3FuncHandler funcHandler = Cef3FuncHandler.CreateFuncHandler(Test001);
            Cef3Func func = Cef3Func.CreateFunc("test001", funcHandler);
            cefV8Global.Set("test001", func);
        }

        void Test001(int id, IntPtr argsPtr)
        {

#if DEBUG
            //if (Cef3Binder.s_dbugIsRendererProcess)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
#endif
            var nativeCallArgs = new NativeCallArgs(argsPtr);
            nativeCallArgs.SetOutput(0, "hello from managed side !");

        }
    }
}