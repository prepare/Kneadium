﻿//MIT, 2016-2017 ,WinterDev
using System;
using System.Collections.Generic;
using System.Text;

namespace BridgeBuilder
{
    class TypePlan
    {
        public List<MethodPlan> methods = new List<MethodPlan>();
        public List<FieldPlan> fields;
        public TypePlan(CodeTypeDeclaration typedecl)
        {
            this.TypeDecl = typedecl;
        }

        public CefTypeModel CefTypeModel { get; set; }
        public string PlainCTypeName { get; set; }
        public CodeTypeDeclaration TypeDecl
        {
            get;
            set;
        }
        public void AddMethod(MethodPlan met)
        {
            methods.Add(met);
        }
        public void AddField(FieldPlan field)
        {
            if (fields == null)
            {
                fields = new List<FieldPlan>();
            }
            //
            fields.Add(field);
        }

        public int CsInterOpTypeNameId { get; set; }
#if DEBUG
        public override string ToString()
        {
            return TypeDecl.ToString();
        }
#endif
    }


    enum CefTypeModel
    {
        Unknown,

        /// <summary>
        /// All scoped framework classes must extend this class.
        /// </summary>
        CefBaseScoped,
        /// <summary>
        /// All ref-counted framework classes must extend this class.
        /// </summary>
        CefBaseRefCounted,
        // class CefBaseRefCounted
        //  {
        //      public:
        /////
        //// Called to increment the reference count for the object. Should be called
        //// for every new copy of a pointer to a given object.
        /////
        //      virtual void AddRef() const = 0; 
        //      ///
        //      // Called to decrement the reference count for the object. Returns true if
        //      // the reference count is 0, in which case the object should self-delete.
        //      ///
        //      virtual bool Release() const = 0; 
        //      ///
        //      // Returns true if the reference count is 1.
        //      ///
        //      virtual bool HasOneRef() const = 0;

        //      protected:
        //virtual ~CefBaseRefCounted() { }
        //  };
        //

        /// <summary>
        /// Template class that provides common functionality for CEF structure wrapping
        /// </summary>
        CefStructBase,
        //template <class traits>
        //class CefStructBase : public traits::struct_type {
        // public:

        /// <summary>
        /// Wrap a C++ class with a C structure. This is used when the class
        /// implementation exists on this side of the DLL boundary but will have methods
        /// called from the other side of the DLL boundary.
        /// </summary>
        CefCppToCRefCounted,
        // template <class ClassName, class BaseName, class StructName>
        // class CefCppToCRefCounted : public CefBaseRefCounted {


        /// <summary>
        /// Wrap a C++ class with a C structure.This is used when the class
        /// implementation exists on this side of the DLL boundary but will have methods
        /// called from the other side of the DLL boundary.
        /// </summary>
        CefCppToCScoped,
        //template <class ClassName, class BaseName, class StructName>
        //class CefCppToCScoped : public CefBaseScoped { 

        /// <summary>
        /// Wrap a C structure with a C++ class.  This is used when the implementation
        /// exists on the other side of the DLL boundary but will have methods called on
        /// this side of the DLL boundary
        /// </summary>
        CefCToCppScoped,
        //template <class ClassName, class BaseName, class StructName>
        //class CefCToCppScoped : public BaseName { 

        /// <summary>
        /// Wrap a C structure with a C++ class. This is used when the implementation
        /// exists on the other side of the DLL boundary but will have methods called on
        /// this side of the DLL boundary.
        /// </summary>
        CefCToCppRefCounted
        // Wrap a C structure with a C++ class. 
        //template <class ClassName, class BaseName, class StructName>
        //class CefCToCppRefCounted : public BaseName { 
    }

    class FieldPlan
    {
        public CodeFieldDeclaration fieldDecl;
        public FieldPlan(CodeFieldDeclaration fieldDecl)
        {
            this.fieldDecl = fieldDecl;
            this.Name = fieldDecl.Name;
        }
        public string Name { get; set; }

#if DEBUG
        public override string ToString()
        {
            return fieldDecl.ToString();
        }
#endif
    }

    class MethodPlan
    {
        internal CodeMethodDeclaration metDecl;
        public List<MethodParameter> pars = new List<MethodParameter>();
        public MethodPlan(CodeMethodDeclaration metDecl)
        {
            this.metDecl = metDecl;
            this.Name = metDecl.Name;
        }
        public string Name { get; set; }
        public void AddMethodParameterTx(MethodParameter par)
        {
            pars.Add(par);
        }
        public MethodParameter ReturnPlan
        {
            get;
            set;
        }

        internal string CppMethodSwitchCaseName { get; set; }


        bool _dupName;
        internal bool HasDuplicatedMethodName
        {
            get { return _dupName; }
            set
            {
                _dupName = value;
            }
        }
        internal string NewOverloadName { get; set; }

        public bool CsLeftMethodBodyBlank { get; set; }
        internal string CsArgClassName { get; set; }
#if DEBUG
        public override string ToString()
        {
            return metDecl.ToString();
        }
#endif
    }

    class MethodParameter
    {

        public MethodParameter(string name, TypeSymbol typeSymbol)
        {
            this.Name = name;
            this.TypeSymbol = typeSymbol;
            if (typeSymbol is SimpleTypeSymbol)
            {
                SimpleTypeSymbol s = (SimpleTypeSymbol)typeSymbol;
                this.IsVoid = s.PrimitiveTypeKind == PrimitiveTypeKind.Void;
            }
        }
        public bool IsConst { get; set; }
        public TypeSymbol TypeSymbol { get; private set; }
        public bool IsMethodReturnParameter { get; set; }
        public string Name { get; set; }
        public bool IsVoid
        {
            get;
            private set;
        }
        public override string ToString()
        {
            return this.Name;
        }
        public ParameterDirection Direction { get; set; }

        internal string ArgPreExtractCode { get; set; }
        internal string ArgExtractCode { get; set; }
        internal string ArgPostExtractCode { get; set; }
        internal bool ArgByRef { get; set; } //temp
        internal string InnerTypeName { get; set; } //temp
        internal string CppUnwrapType { get; set; }
        internal string CppUnwrapMethod { get; set; }
        internal string CppWrapMethod { get; set; }
        internal void ClearExtractCode()
        {
            ArgByRef = false; //temp
            InnerTypeName = null;//temp
            ArgPreExtractCode = ArgExtractCode = ArgPostExtractCode = null;
        }
    }

    enum ParameterDirection
    {
        Return,
        In,
        Out,
        InOut
    }


    class TypeTranformPlanner
    {
        CodeTypeDeclaration typedecl; //current type decl
        TypePlan typeTxInfo; //current type tx          
        TemplateTypeSymbol3 tt3;
        static int codeGenNum;

        public TypeTranformPlanner()
        {
        }
        public CefTypeCollection CefTypeCollection { get; set; }

        void Generate_CppToC_CefVisitor(SimpleTypeSymbol orgType)
        {
            GenerateMethodCallFromCppToCs(orgType);
        }
        void Generate_CppToC_CefCallback(SimpleTypeSymbol orgType)
        {
            GenerateMethodCallFromCppToCs(orgType);
        }
        void Generate_CppToC_CefHandler(SimpleTypeSymbol orgType)
        {
            GenerateMethodCallFromCppToCs(orgType);
        }

        void GenerateMethodCallFromCppToCs(SimpleTypeSymbol orgType)
        {
            //...
            CodeTypeDeclaration codeTypeDecl = orgType.CreatedByTypeDeclaration;
            if (codeTypeDecl == null)
            {
                throw new NotSupportedException();
            }
            //this is type plan
            TypePlan orgTypeTxInfo = codeTypeDecl.TypePlan;
            //create injected code for managed code
            foreach (CodeMethodDeclaration met in codeTypeDecl.GetMethodIter())
            {
                StringBuilder stbuilder = new StringBuilder();
                int j = met.Parameters.Count;
                //
                stbuilder.AppendLine("//TypeTranformPlanner::GenerateMethodCallFromCppToCs , " + (++codeGenNum));
                stbuilder.AppendLine("autogen!: " + met.ToString());

                stbuilder.AppendLine("if(mcallback_){");
                stbuilder.AppendLine("MethodArgs args;");
                stbuilder.AppendLine("memset(&args,0,sizeof(MethodArgs));");

                MethodPlan metTx = met.methodTxInfo;

                for (int i = 0; i < j; ++i)
                {
                    //set args->
                    CodeMethodParameter par = met.Parameters[i];
                    TypeSymbol parTypeSymbol = par.ParameterType.ResolvedType;
                    TypeBridgeInfo bridgeInfo = parTypeSymbol.BridgeInfo;
                    switch (parTypeSymbol.TypeSymbolKind)
                    {
                        default:
                            break;
                        case TypeSymbolKind.TypeDef:
                            {
                                CTypeDefTypeSymbol cTypeDefSymbol = (CTypeDefTypeSymbol)parTypeSymbol;
                                TypeSymbol referToType = cTypeDefSymbol.ReferToTypeSymbol;
                                switch (referToType.TypeSymbolKind)
                                {
                                    default:
                                        {

                                        }
                                        break;
                                    case TypeSymbolKind.Simple:
                                        {
                                            SimpleTypeSymbol ss = (SimpleTypeSymbol)referToType;
                                            if (ss.PrimitiveTypeKind == PrimitiveTypeKind.NotPrimitiveType)
                                            {
                                                stbuilder.AppendLine("args.SetArgAsNativeObject(" + i + "," + par.ParameterName);
                                            }
                                            else
                                            {

                                            }
                                        }
                                        break;
                                }


                            }
                            break;
                        case TypeSymbolKind.Simple:
                            {
                                //simple type 
                                SimpleTypeSymbol simpleTypeInfo = (SimpleTypeSymbol)parTypeSymbol;
                                switch (simpleTypeInfo.PrimitiveTypeKind)
                                {
                                    default:
                                        break;
                                    case PrimitiveTypeKind.Double:
                                        stbuilder.AppendLine("args.SetArgAsDouble(" + i + "," + par.ParameterName); //cast to int32
                                        break;
                                    case PrimitiveTypeKind.size_t:
                                        stbuilder.AppendLine("args.SetArgAsInt32(" + i + ",(size_t)" + par.ParameterName); //cast to int32
                                        break;
                                    case PrimitiveTypeKind.NaitveInt:
                                        stbuilder.AppendLine("args.SetArgAsInt32(" + i + "," + par.ParameterName);
                                        break;
                                    case PrimitiveTypeKind.Bool:
                                        stbuilder.AppendLine("args.SetArgAsBool(" + i + "," + par.ParameterName);
                                        break;
                                    case PrimitiveTypeKind.Int32:
                                        stbuilder.AppendLine("args.SetArgAsInt32(" + i + "," + par.ParameterName);
                                        break;
                                    case PrimitiveTypeKind.Int64:
                                        stbuilder.AppendLine("args.SetArgAsInt64(" + i + "," + par.ParameterName);
                                        break;
                                    case PrimitiveTypeKind.NotPrimitiveType:
                                        stbuilder.AppendLine("args.SetArgAsNativeObject(" + i + "," + par.ParameterName);
                                        break;
                                }
                            }
                            break;
                        case TypeSymbolKind.ReferenceOrPointer:
                            {
                                ReferenceOrPointerTypeSymbol referenceOrPointer = (ReferenceOrPointerTypeSymbol)parTypeSymbol;
                                TypeSymbol elemType = referenceOrPointer.ElementType;
                                switch (elemType.TypeSymbolKind)
                                {
                                    default:
                                        break;
                                    case TypeSymbolKind.Vec:
                                        {

                                            VecTypeSymbol vec = (VecTypeSymbol)elemType;
                                            stbuilder.AppendLine("args.SetArgAsNativeObject(" + i + ",&" + par.ParameterName + ");");
                                        }
                                        break;
                                    case TypeSymbolKind.ReferenceOrPointer:
                                        {
                                            if (referenceOrPointer.Kind == ContainerTypeKind.ByRef)
                                            {
                                                //eg. CefRefPtr<CefClient>& 
                                                stbuilder.AppendLine("args.SetArgAsNativeObject(" + i + ",&" + par.ParameterName + ");");
                                            }
                                            else
                                            {

                                            }

                                        }
                                        break;
                                    case TypeSymbolKind.TypeDef:
                                        {
                                            //ref of type def
                                            CTypeDefTypeSymbol cefTypeDef = (CTypeDefTypeSymbol)elemType;
                                            TypeSymbol referToType = cefTypeDef.ReferToTypeSymbol;
                                            if (referToType.TypeSymbolKind == TypeSymbolKind.Simple)
                                            {
                                                SimpleTypeSymbol ss = (SimpleTypeSymbol)referToType;
                                                stbuilder.AppendLine("args.SetArgAsNativeObject(" + i + ",&" + par.ParameterName + ");");
                                            }
                                            else if (referToType.TypeSymbolKind == TypeSymbolKind.Template)
                                            {

                                                TemplateTypeSymbol tt = (TemplateTypeSymbol)referToType;
                                                switch (tt.Name)
                                                {
                                                    default:
                                                        break;
                                                    case "CefStructBase":
                                                        stbuilder.AppendLine("args.SetArgAsNativeObject(" + i + ",&" + par.ParameterName + ");");
                                                        break;
                                                }
                                            }
                                            else if (referToType.TypeSymbolKind == TypeSymbolKind.Vec)
                                            {
                                                VecTypeSymbol vec = (VecTypeSymbol)referToType;

                                            }
                                            else
                                            {

                                            }

                                        }
                                        break;
                                    case TypeSymbolKind.Simple:
                                        {
                                            SimpleTypeSymbol simpleTypeInfo = (SimpleTypeSymbol)elemType;
                                            switch (simpleTypeInfo.PrimitiveTypeKind)
                                            {
                                                default:
                                                    break;
                                                case PrimitiveTypeKind.size_t:
                                                    //ref or pointer to size? 
                                                    stbuilder.AppendLine("args.SetArgAsInt32(" + i + ",0);");

                                                    break;
                                                case PrimitiveTypeKind.Int64:
                                                case PrimitiveTypeKind.NaitveInt:
                                                    stbuilder.AppendLine("args.SetArgAsInt64(" + i + ",0);");
                                                    break;
                                                case PrimitiveTypeKind.Bool:
                                                    //reference to some primitive                                                     
                                                    stbuilder.AppendLine("args.SetArgAsNativeBool(" + i + ",false);");
                                                    //after call this we need to get data of this arg back to 
                                                    break;
                                                case PrimitiveTypeKind.CefString:
                                                    stbuilder.AppendLine("args.SetArgAsNativeObject(" + i + ",&" + par.ParameterName + ");");
                                                    stbuilder.AppendLine("args.SetArgType(" + i + ", JSVALUE_TYPE_NATIVE_CEFSTRING);");
                                                    break;

                                                case PrimitiveTypeKind.Void:
                                                    //void*
                                                    stbuilder.AppendLine("args.SetArgAsNativeObject(" + i + "," + par.ParameterName);
                                                    break;
                                                //-----------------------
                                                case PrimitiveTypeKind.NotPrimitiveType:
                                                    stbuilder.AppendLine("args.SetArgAsNativeObject(" + i + "," + par.ParameterName + ");");
                                                    break;
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                }

                //-------
                //call the delegate
                stbuilder.AppendLine("mcallback_(CEF_MSG_" + met.Name + ",&args);");

                //get some var back  

                TypeSymbol returnType = met.ReturnType.ResolvedType;
                if (returnType == null)
                {
                    throw new NotSupportedException();
                }
                switch (returnType.TypeSymbolKind)
                {
                    default:
                        throw new NotSupportedException();
                    case TypeSymbolKind.ReferenceOrPointer:
                        {
                            ReferenceOrPointerTypeSymbol refOrPointer = (ReferenceOrPointerTypeSymbol)returnType;
                            TypeSymbol elemType = refOrPointer.ElementType;
                            switch (elemType.TypeSymbolKind)
                            {
                                default:
                                    break;
                                case TypeSymbolKind.Simple:
                                    {
                                        SimpleTypeSymbol ss = (SimpleTypeSymbol)elemType;
                                        if (ss.PrimitiveTypeKind == PrimitiveTypeKind.NotPrimitiveType)
                                        {
                                            //
                                            switch (ss.Name)
                                            {
                                                default:
                                                    break;
                                                case "CefResponseFilter":
                                                case "CefCookieManager":
                                                case "CefLoadHandler":
                                                case "CefAccessibilityHandler":
                                                case "CefPrintHandler":
                                                case "CefResourceHandler":
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                        }
                                    }
                                    break;
                            }


                        }
                        break;
                    case TypeSymbolKind.Simple:
                        {
                            SimpleTypeSymbol simpleType = (SimpleTypeSymbol)returnType;
                            switch (simpleType.PrimitiveTypeKind)
                            {
                                case PrimitiveTypeKind.NotPrimitiveType:
                                    {
                                        //data from user 
                                        //1. 
                                        switch (simpleType.Name)
                                        {
                                            default:
                                                break;
                                            case "CefSize":
                                                break;
                                        }
                                    }
                                    break;
                                case PrimitiveTypeKind.Void:
                                    {

                                    }
                                    break;
                                case PrimitiveTypeKind.Bool:
                                    stbuilder.AppendLine("return args.ReadOutputAsInt32(0) !=0");
                                    break;
                                case PrimitiveTypeKind.Int32:
                                    stbuilder.AppendLine("return args.ReadOutputAsInt32(0)");
                                    break;
                                case PrimitiveTypeKind.Int64:
                                    stbuilder.AppendLine("return args.ReadOutputAsInt64(0)");
                                    break;
                                case PrimitiveTypeKind.NaitveInt:
                                    stbuilder.AppendLine("return args.ReadOutputAsInt64(0)");
                                    break;
                                case PrimitiveTypeKind.size_t:
                                    stbuilder.AppendLine("return args.ReadOutputAsUInt32(0)");
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;

                }
                //clean up
                //-------
                stbuilder.AppendLine("}");
            }
        }


        void AddCppToCTypeInfoHint()
        {
            //this is cef3-speciific convention.
            string typename = this.typedecl.Name;
            if (typename.Contains("Callback"))
            {
                //callback:
                //1. it is create from .net side 
                //2. 
                //need to create c++ object 
                //that store func callback pointer to this .net side
                Generate_CppToC_CefCallback(tt3.Item1 as SimpleTypeSymbol);
            }
            else if (typename.Contains("Visitor"))
            {
                //visitor:
                //1. it is create from .net side 
                //2. we also create visitor for c++ side                      
                //   that store func callback pointer to this .net side
                Generate_CppToC_CefVisitor(tt3.Item1 as SimpleTypeSymbol);
            }
            else if (typename.Contains("Handler"))
            {
                //------------
                //handler:
                //.net side=> create interface that declare member of this handler
                //this will be called from native side
                //------------ 
                //create cpp code for handler                 
                Generate_CppToC_CefHandler(tt3.Item1 as SimpleTypeSymbol);
            }
            else if (typename.Contains("Base"))
            {

            }
            else
            {

            }
        }
        public TypePlan MakeTransformPlan(CodeTypeDeclaration typedecl)
        {
            this.typedecl = typedecl;
            //---------------------------------------------
            this.typeTxInfo = new TypePlan(typedecl);
            typedecl.TypePlan = this.typeTxInfo;
            //---------------------------------------------
            this.tt3 = null; //for hint

            if (typedecl.BaseTypes != null && typedecl.BaseTypes.Count == 1)
            {
                CodeTypeReference baseTypeReference = typedecl.BaseTypes[0];
                TypeSymbol baseTypeSymbol = baseTypeReference.ResolvedType;
                switch (baseTypeSymbol.TypeSymbolKind)
                {
                    default:
                        break;
                    case TypeSymbolKind.Simple:
                        {
                            SimpleTypeSymbol simpleTypeSymbol = (SimpleTypeSymbol)baseTypeSymbol;
                            switch (simpleTypeSymbol.Name)
                            {
                                default:
                                    break;
                                case "CefBaseRefCounted":
                                    typeTxInfo.CefTypeModel = CefTypeModel.CefBaseRefCounted;
                                    break;
                                case "CefBaseScoped":
                                    typeTxInfo.CefTypeModel = CefTypeModel.CefBaseScoped;
                                    break;
                            }
                        }
                        break;
                    case TypeSymbolKind.Template:
                        {
                            TemplateTypeSymbol templateTypeSymbol = (TemplateTypeSymbol)baseTypeSymbol;
                            switch (templateTypeSymbol.ItemCount)
                            {
                                default: throw new NotSupportedException();
                                case 1:
                                    {
                                        string templateName = templateTypeSymbol.Name;
                                        switch (templateName)
                                        {
                                            default:
                                                break;
                                            case "CefStructBase":
                                                typeTxInfo.CefTypeModel = CefTypeModel.CefStructBase;
                                                break;
                                        }
                                    }
                                    break;
                                case 3:
                                    {
                                        this.tt3 = (TemplateTypeSymbol3)templateTypeSymbol;
                                        string templateName = tt3.Name;
                                        switch (templateName)
                                        {
                                            default:
                                                break;
                                            case "CefCppToCRefCounted":
                                                typeTxInfo.CefTypeModel = CefTypeModel.CefCppToCRefCounted;
                                                typeTxInfo.PlainCTypeName = tt3.Item1.ToString();
                                                break;
                                            case "CefCppToCScoped":
                                                typeTxInfo.CefTypeModel = CefTypeModel.CefCppToCScoped;
                                                typeTxInfo.PlainCTypeName = tt3.Item1.ToString();
                                                break;
                                            case "CefCToCppScoped":
                                                typeTxInfo.CefTypeModel = CefTypeModel.CefCToCppScoped;
                                                typeTxInfo.PlainCTypeName = tt3.Item1.ToString();
                                                break;
                                            case "CefCToCppRefCounted":
                                                typeTxInfo.CefTypeModel = CefTypeModel.CefCToCppScoped;
                                                typeTxInfo.PlainCTypeName = tt3.Item2.ToString(); //for unwrap method

                                                break;
                                        }

                                    }
                                    break;
                            }

                        }
                        break;
                    case TypeSymbolKind.TypeDef:
                        {
                            CTypeDefTypeSymbol ctypedefSymbol = (CTypeDefTypeSymbol)baseTypeSymbol;
                        }
                        break;
                    case TypeSymbolKind.Vec:
                        {

                        }
                        break;
                    case TypeSymbolKind.ReferenceOrPointer:
                        {

                        }
                        break;
                }
            }
            else
            {

            }

            switch (typeTxInfo.CefTypeModel)
            {
                case CefTypeModel.CefCppToCRefCounted:
                case CefTypeModel.CefCppToCScoped:
                    {
                        AddCppToCTypeInfoHint();
                    }
                    break;
            }

            //-----------
            if (typedecl.Kind == TypeKind.Enum)
            {
                //enum
                foreach (CodeFieldDeclaration fieldDecl in typedecl.GetFieldIter())
                {
                    FieldPlan fieldTx = MakeFieldPlan(fieldDecl);
                    typeTxInfo.AddField(fieldTx);
                }
            }
            else
            {
               
                //---------------------------------------------------------------------
                foreach (CodeMethodDeclaration metDecl in typedecl.GetMethodIter())
                {

                    if (metDecl.MethodKind == MethodKind.Normal)
                    {
                        MethodPlan metTx = MakeMethodPlan(metDecl);
                        if (metTx == null)
                        {
                            throw new NotSupportedException();
                        }
                        typeTxInfo.AddMethod(metTx);
                    }
                    else
                    {

                    }
                }

            }

            return typeTxInfo;
        }
      
        FieldPlan MakeFieldPlan(CodeFieldDeclaration fieldDecl)
        {
            FieldPlan fieldTx = new FieldPlan(fieldDecl);
            return fieldTx;
        }
        MethodPlan MakeMethodPlan(CodeMethodDeclaration metDecl)
        {
            MethodPlan metTx = new MethodPlan(metDecl);
            //make return type plan 

            //1. return
            MethodParameter retTxInfo = new MethodParameter(null, metDecl.ReturnType.ResolvedType) { IsMethodReturnParameter = true };
            retTxInfo.Direction = ParameterDirection.Return;

            AddMethodParameterTypeTxInfo(retTxInfo, metDecl.ReturnType.ResolvedType);
            metTx.ReturnPlan = retTxInfo;

            //2. parameters
            int j = metDecl.Parameters.Count;
            for (int i = 0; i < j; ++i)
            {
                CodeMethodParameter metPar = metDecl.Parameters[i];

                MethodParameter parTxInfo = new MethodParameter(metPar.ParameterName, metPar.ParameterType.ResolvedType);
                parTxInfo.IsConst = metPar.IsConstPar;
                parTxInfo.Direction = ParameterDirection.In;
                //TODO: review Out,InOut direction 

                TypeSymbol parTypeSymbol = metPar.ParameterType.ResolvedType;
                AddMethodParameterTypeTxInfo(parTxInfo, parTypeSymbol);

                metTx.AddMethodParameterTx(parTxInfo);

            }
            return metTx;
        }

        void AddMethodParameterTypeTxInfo(MethodParameter parPlan, TypeSymbol resolvedParType)
        {
            TypeBridgeInfo bridgeInfo = resolvedParType.BridgeInfo;
            if (bridgeInfo == null)
            {
                switch (resolvedParType.TypeSymbolKind)
                {
                    default:
                        //other type should be resolved before,
                        //so, should not visit here
                        throw new NotSupportedException();
                    case TypeSymbolKind.ReferenceOrPointer:
                        {
                            bridgeInfo = CefTypeCollection.Planner.AssignTypeBridgeInfo((ReferenceOrPointerTypeSymbol)resolvedParType);
                        }
                        break;
                }
            }
            //-----------------------
            //assign dotnet side type
            if (bridgeInfo.CsMethodParType != null)
            {
                return;
            }
            switch (resolvedParType.TypeSymbolKind)
            {
                case TypeSymbolKind.ReferenceOrPointer:
                    {

                        ReferenceOrPointerTypeSymbol refOrPointer = (ReferenceOrPointerTypeSymbol)resolvedParType;
                        switch (bridgeInfo.WellKnownTypeName)
                        {
                            default:
                                {

                                }
                                break;
                            case WellKnownTypeName.PtrOf:
                                {
                                    TypeSymbol elemType = refOrPointer.ElementType;
                                    if (elemType.TypeSymbolKind == TypeSymbolKind.Simple)
                                    {
                                        SimpleTypeSymbol ss = (SimpleTypeSymbol)elemType;
                                        if (ss.IsEnum)
                                        {
                                            bridgeInfo.SetCsInterOpPrimitiveType("int");
                                            return;
                                        }

                                    }
                                    string vectorElementTypeName = elemType.ToString();
                                    switch (vectorElementTypeName)
                                    {
                                        default:
                                            //char* => .net byte[]
                                            {

                                            }
                                            break;
                                        case "bool":
                                            bridgeInfo.SetCsInterOp("bool*", "Cef3Bind.Set_unsafe_voidptr", "Cef3Bind.Get_unsafe_voidptr");
                                            return;
                                        case "char":
                                            //char*
                                            bridgeInfo.SetCsInterOpPrimitiveType("string");
                                            return;
                                        case "void": //void*
                                                     //for .net 
                                                     //1. unsafe void*
                                                     //2. IntPtr
                                            bridgeInfo.SetCsInterOp("void*", "Cef3Bind.Set_unsafe_voidptr", "Cef3Bind.Get_unsafe_voidptr");
                                            return;

                                    }
                                }
                                break;
                            case WellKnownTypeName.RefOfVec:
                                {

                                    VecTypeSymbol elem = (VecTypeSymbol)refOrPointer.ElementType;
                                    string vectorElementTypeName = elem.ElementType.ToString();
                                    switch (vectorElementTypeName)
                                    {
                                        default:
                                            break;
                                        case "refptr<CefX509Certificate>":
                                            bridgeInfo.SetCsListInterOp("List<CefX509Certificate>", "Set_ListOf_CefX509Certificate", "Get_ListOf_CefX509Certificate");
                                            return;
                                        case "CefRect":
                                            bridgeInfo.SetCsListInterOp("List<CefRect>", "Set_ListOf_CefRect", "Get_ListOf_CefRect");
                                            return;
                                        case "CefDraggableRegion":
                                            bridgeInfo.SetCsListInterOp("List<CefDraggableRegion>", "Set_ListOf_CefDraggableRegion", "Get_ListOf_CefDraggableRegion");
                                            return;
                                        case "int64":
                                            bridgeInfo.SetCsListInterOp("List<long>", "Set_ListOf_long", "Get_ListOf_long");
                                            return;
                                        case "CefString":
                                            bridgeInfo.SetCsListInterOp("List<string>", "Set_ListOf_string", "Get_ListOf_string");
                                            return;
                                        case "CefCompositionUnderline":
                                            bridgeInfo.SetCsListInterOp("List<CefCompositionUnderline>", "Set_ListOf_CefCompositionUnderline", "Get_ListOf_CefCompositionUnderline");
                                            return;

                                    }
                                }
                                break;
                            case WellKnownTypeName.RefOfCefString:
                                {
                                    AddMethodParameterTypeTxInfo(parPlan, refOrPointer.ElementType);
                                    bridgeInfo.SetCsInterOpPrimitiveType("string");
                                    return;
                                }
                            case WellKnownTypeName.RefOfPrimitive:
                                {
                                    //what is that primitive 
                                    AddMethodParameterTypeTxInfo(parPlan, refOrPointer.ElementType);
                                    //reference to primitive need 2 steps
                                    //1. copy user data to temp var
                                    //2. after call native method then copy result back to user data

                                    bridgeInfo.SetCsInterOp("ref " + refOrPointer.ElementType.BridgeInfo.CsMethodParType,
                                               refOrPointer.ElementType.BridgeInfo.CsAssignMethodName,
                                               refOrPointer.ElementType.BridgeInfo.CsGetValueMethodName
                                               );

                                    return;
                                }
                            case WellKnownTypeName.RefPtrOf:
                                {
                                    //our .net side  treat other kind of cpp native by ref                                     
                                    string name = refOrPointer.ElementType.ToString();
                                    switch (name)
                                    {
                                        case "CefBinaryValue":
                                            break;
                                        default:
                                            if (name.EndsWith("Callback"))
                                            {

                                            }
                                            else if (name.StartsWith("Cef"))
                                            {
                                            }
                                            else
                                            {

                                            }
                                            break;
                                    }

                                    bridgeInfo.SetCsNativeCppObjectInterOp(name);
                                    return;
                                }
                            case WellKnownTypeName.RefOf:
                                {
                                    string name = refOrPointer.ElementType.ToString();

                                    switch (name)
                                    {
                                        case "CefBinaryValue":
                                            break;
                                        default:
                                            if (name.EndsWith("Callback"))
                                            {

                                            }
                                            else
                                            {

                                            }
                                            break;
                                    }
                                    bridgeInfo.SetCsNativeCppObjectInterOp(name);
                                    return;
                                }
                        }
                    }
                    break;
            }

            switch (bridgeInfo.WellKnownTypeName)
            {
                default:

                    break;
                case WellKnownTypeName.OtherCppClass:
                    {
                        string typename = resolvedParType.ToString();
                        switch (typename)
                        {
                            default:
                                {
                                    if (typename.StartsWith("cef_"))
                                    {
                                        //native cef_

                                    }
                                    else
                                    {
                                    }
                                    bridgeInfo.SetCsNativeCppObjectInterOp(typename);
                                }
                                break;
                            case "CefProcessId":
                                bridgeInfo.SetCsNativeCppObjectInterOp(typename);
                                break;
                        }
                    }
                    break;
                case WellKnownTypeName.CefCNative:
                    {
                        string typename = resolvedParType.ToString();
                        switch (typename)
                        {
                            default:
                                {
                                    if (typename.StartsWith("cef_"))
                                    {
                                        //native cef_

                                    }
                                    else
                                    {
                                    }
                                    bridgeInfo.SetCsNativeCppObjectInterOp(typename);
                                }
                                break;
                            case "CefProcessId":
                                bridgeInfo.SetCsNativeCppObjectInterOp(typename);
                                break;
                        }
                    }
                    break;
                case WellKnownTypeName.TypeDefToAnother:
                    {
                        CTypeDefTypeSymbol ctypedefSymbol = (CTypeDefTypeSymbol)resolvedParType;
                        TypeSymbol referToType = ctypedefSymbol.ReferToTypeSymbol;
                        TypeBridgeInfo referToTypeBridge = referToType.BridgeInfo;
                        switch (referToTypeBridge.WellKnownTypeName)
                        {
                            case WellKnownTypeName.CefCNative:
                                {
                                    string typename = resolvedParType.ToString();
                                    bridgeInfo.SetCsNativeCppObjectInterOp(typename);
                                }
                                break;
                            case WellKnownTypeName.UInt32:
                                {
                                    bridgeInfo.SetCsInterOpPrimitiveType("uint");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            }

        }
    }

    enum CefCppSlotKind
    {

        //#define JSVALUE_TYPE_UNKNOWN_ERROR  -1
        //#define JSVALUE_TYPE_EMPTY			 0
        //#define JSVALUE_TYPE_NULL            1
        //#define JSVALUE_TYPE_BOOLEAN         2
        //#define JSVALUE_TYPE_INTEGER         3
        //#define JSVALUE_TYPE_NUMBER          4
        //#define JSVALUE_TYPE_STRING          5 //unicode string
        //#define JSVALUE_TYPE_DATE            6
        //#define JSVALUE_TYPE_INDEX           7
        //#define JSVALUE_TYPE_ARRAY          10
        //#define JSVALUE_TYPE_STRING_ERROR   11
        //#define JSVALUE_TYPE_MANAGED        12
        //#define JSVALUE_TYPE_MANAGED_ERROR  13
        //#define JSVALUE_TYPE_WRAPPED        14
        //#define JSVALUE_TYPE_DICT           15
        //#define JSVALUE_TYPE_ERROR          16
        //#define JSVALUE_TYPE_FUNCTION       17

        //#define JSVALUE_TYPE_JSTYPEDEF      18 //my extension
        //#define JSVALUE_TYPE_INTEGER64      19 //my extension
        //#define JSVALUE_TYPE_BUFFER         20 //my extension

        //#define JSVALUE_TYPE_NATIVE_CEFSTRING 30  //my extension
        //#define JSVALUE_TYPE_NATIVE_CEFHOLDER_STRING 31//my extension
        //#define JSVALUE_TYPE_MANAGED_CB 32
        //#define JSVALUE_TYPE_MEM_ERROR      50 //my extension


        JSVALUE_TYPE_UNKNOWN_ERROR = -1,
        JSVALUE_TYPE_EMPTY = 0,
        JSVALUE_TYPE_NULL = 1,
        JSVALUE_TYPE_BOOLEAN = 2,
        JSVALUE_TYPE_INTEGER = 3,
        JSVALUE_TYPE_NUMBER = 4,
        JSVALUE_TYPE_STRING = 5, //unicode string
        JSVALUE_TYPE_DATE = 6,
        JSVALUE_TYPE_INDEX = 7,
        JSVALUE_TYPE_ARRAY = 10,
        JSVALUE_TYPE_STRING_ERROR = 11,
        JSVALUE_TYPE_MANAGED = 12,
        JSVALUE_TYPE_MANAGED_ERROR = 13,
        JSVALUE_TYPE_WRAPPED = 14,
        JSVALUE_TYPE_DICT = 15,
        JSVALUE_TYPE_ERROR = 16,
        JSVALUE_TYPE_FUNCTION = 17,
        JSVALUE_TYPE_JSTYPEDEF = 18,//my extension
        JSVALUE_TYPE_INTEGER64 = 19, //my extension
        JSVALUE_TYPE_BUFFER = 20,//my extension 
        JSVALUE_TYPE_NATIVE_CEFSTRING = 30, //my extension
        JSVALUE_TYPE_NATIVE_CEFHOLDER_STRING = 31,//my extension
        JSVALUE_TYPE_MANAGED_CB = 32,
        JSVALUE_TYPE_MEM_ERROR = 50, //my extension
    }


    enum WellKnownTypeName
    {
        Unknown,
        RefOf,
        RefPtrOf,
        //
        ScopedPtrOf,
        PtrOf,
        //
        Void,
        //
        Vec,
        //
        Map,
        //
        CefString,
        OtherString,
        //
        RefOfCefString,
        RefOfVec,
        RefOfPrimitive,

        //
        MultiMap,
        //
        CefCppToCRefCounted,
        CefCppToCScoped,
        CefCToCppScoped,
        CefCToCppRefCounted,

        Bool,
        Int,
        Int32,
        UInt32,
        Int64,
        UInt64,
        IntPtr,
        CChar, //byte 
        Double,
        Float,

        size_t,
        NativeInt,
        //
        CefCNative,
        CefCpp,
        CefStructBase,
        //
        CefAbstract,
        OtherCppClass,
        TypeDefToAnother,
    }

    class TypeBridgeInfo
    {
        WellKnownTypeName wellknownTypeName;
        CefSlotName slotName;
        CefCppSlotKind cefTypeKind;

        TypeBridgeInfo _pointerBridge;
        TypeBridgeInfo _referenceBridge;
        TypeBridgeInfo _cefRefPtrBridge;
        TypeBridgeInfo _scopePtrBridge;

        TypeSymbol typeSymbol;
        TypeBridgeInfo elementTypeBridge;
        TypeBridgeInfo _bridgeToBase;
        TypeBridgeInfo _referToTypeBridge;
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif

        public TypeBridgeInfo(SimpleTypeSymbol t, WellKnownTypeName wellknownTypeName, CefCppSlotKind cefCppSlotKind)
        {
            this.typeSymbol = t;
            this.wellknownTypeName = wellknownTypeName;
            SetCefCppSlotKind(cefCppSlotKind);
        }
        public TypeBridgeInfo(SimpleTypeSymbol t, WellKnownTypeName wellknownTypeName, CefCppSlotKind cefCppSlotKind, TypeBridgeInfo bridgeToBase)
        {

            this.typeSymbol = t;
            this.wellknownTypeName = wellknownTypeName;
            this._bridgeToBase = bridgeToBase;
            SetCefCppSlotKind(cefCppSlotKind);
            //-------
            if (bridgeToBase != null)
            {
                TypeSymbol baseSymbol = bridgeToBase.typeSymbol;
                switch (baseSymbol.TypeSymbolKind)
                {
                    default:
                        {
                            switch (bridgeToBase.WellKnownTypeName)
                            {
                                default:
                                    break;
                                case WellKnownTypeName.CefAbstract:
                                    break;
                            }
                        }
                        break;
                    case TypeSymbolKind.Template:
                        {
                            TemplateTypeSymbol templateTypeSymbol = (TemplateTypeSymbol)baseSymbol;
                        }
                        break;
                }
            }


        }
        public TypeBridgeInfo(TemplateTypeSymbol t, WellKnownTypeName wellknownTypeName, CefCppSlotKind cefCppSlotKind)
        {
            this.typeSymbol = t;
            this.wellknownTypeName = wellknownTypeName;
            SetCefCppSlotKind(cefCppSlotKind);
        }
        public TypeBridgeInfo(VecTypeSymbol t, WellKnownTypeName wellknownTypeName, CefCppSlotKind cefCppSlotKind)
        {
            this.typeSymbol = t;
            this.wellknownTypeName = wellknownTypeName;
            SetCefCppSlotKind(cefCppSlotKind);
        }
        public TypeBridgeInfo(CTypeDefTypeSymbol t, WellKnownTypeName wellknownTypeName, CefCppSlotKind cefCppSlotKind, TypeBridgeInfo referToTypeBridge)
        {
            this.typeSymbol = t;
            this._referToTypeBridge = referToTypeBridge;
            this.wellknownTypeName = wellknownTypeName;
            SetCefCppSlotKind(cefCppSlotKind);
        }
        public TypeBridgeInfo(TemplateParameterTypeSymbol t, WellKnownTypeName wellknownTypeName, CefCppSlotKind cefCppSlotKind)
        {
            //TODO: review here again
            this.typeSymbol = t;
            this.wellknownTypeName = wellknownTypeName;
            SetCefCppSlotKind(cefCppSlotKind);
        }

        private TypeBridgeInfo(TypeBridgeInfo elementTypeBridge, WellKnownTypeName wellknownTypeName, CefCppSlotKind cefCppSlotKind)
        {
            this.elementTypeBridge = elementTypeBridge;
            this.wellknownTypeName = wellknownTypeName;
            SetCefCppSlotKind(cefCppSlotKind);
        }

        void SetCefCppSlotKind(CefCppSlotKind cefTypeKind)
        {

#if DEBUG

#endif
            this.cefTypeKind = cefTypeKind;
            switch (cefTypeKind)
            {
                default: throw new NotSupportedException();
                case CefCppSlotKind.JSVALUE_TYPE_ERROR:
                    slotName = CefSlotName.UNKNOWN;
                    break;
                case CefCppSlotKind.JSVALUE_TYPE_WRAPPED:
                    slotName = CefSlotName.ptr;
                    break;
                case CefCppSlotKind.JSVALUE_TYPE_INTEGER64:
                    slotName = CefSlotName.i64;
                    break;
                case CefCppSlotKind.JSVALUE_TYPE_EMPTY:
                    slotName = CefSlotName.UNKNOWN;
                    break;
                case CefCppSlotKind.JSVALUE_TYPE_BOOLEAN:
                case CefCppSlotKind.JSVALUE_TYPE_INTEGER: //TODO: review int32
                    slotName = CefSlotName.i32; //sign 32 bits
                    break;
                case CefCppSlotKind.JSVALUE_TYPE_NATIVE_CEFHOLDER_STRING:
                case CefCppSlotKind.JSVALUE_TYPE_NATIVE_CEFSTRING:
                    slotName = CefSlotName.ptr;
                    break;
                case CefCppSlotKind.JSVALUE_TYPE_NUMBER:
                    slotName = CefSlotName.num;
                    break;
            }
        }

        public CefSlotName CefCppSlotName
        {
            get { return slotName; }
        }
        public CefCppSlotKind CefCppSlotKind
        {
            get { return cefTypeKind; }
        }

        public WellKnownTypeName WellKnownTypeName
        {
            get { return wellknownTypeName; }
        }

        public override string ToString()
        {
            if (typeSymbol != null)
            {
                return "bridge : " + this.WellKnownTypeName + " " + typeSymbol.ToString();
            }
            else if (elementTypeBridge != null)
            {
                return "bridge : " + this.WellKnownTypeName + " " + elementTypeBridge.ToString();
            }
            else
            {
                return "bridge : " + this.WellKnownTypeName + " ???";
            }

        }

        public TypeBridgeInfo GetPointerBridge()
        {
            if (_pointerBridge == null)
            {
                //create new one  
                _pointerBridge = new TypeBridgeInfo(this, WellKnownTypeName.PtrOf, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
            }
            return _pointerBridge;
        }
        public TypeBridgeInfo GetReferenceBridge()
        {
            if (_referenceBridge == null)
            {
                //create new one

                switch (this.wellknownTypeName)
                {
                    default:
                        {
                            return _referenceBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefOf, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                        }
                    case WellKnownTypeName.CefString:
                        {
                            //CefString&
                            return _referenceBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefOfCefString, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                        }
                    case WellKnownTypeName.Vec:
                        {
                            return _referenceBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefOfVec, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                        }
                    case WellKnownTypeName.CefStructBase:
                        {
                            return _referenceBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefOf, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                        }
                    case WellKnownTypeName.CefCpp:
                        {
                            return _referenceBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefOf, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                        }
                    case WellKnownTypeName.Map:
                    case WellKnownTypeName.MultiMap:
                        {
                            return _referenceBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefOf, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                        }
                    case WellKnownTypeName.TypeDefToAnother:
                        {
                            return _referenceBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefOf, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                        }
                    case WellKnownTypeName.Float:
                    case WellKnownTypeName.Double:
                    case WellKnownTypeName.NativeInt:
                    case WellKnownTypeName.Bool:
                        {
                            return _referenceBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefOfPrimitive, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                        }
                    case WellKnownTypeName.CefCNative:
                        {
                            return _referenceBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefOf, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                        }
                }
            }
            return _referenceBridge;
        }
        public TypeBridgeInfo GetCefRefPtrBridge()
        {
            if (_cefRefPtrBridge == null)
            {
                //create new one
                _cefRefPtrBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefPtrOf, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
            }
            return _cefRefPtrBridge;
        }
        public TypeBridgeInfo GetCefRawPtrBridge()
        {
            if (_cefRefPtrBridge == null)
            {
                //create new one
                _cefRefPtrBridge = new TypeBridgeInfo(this, WellKnownTypeName.RefPtrOf, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
            }
            return _cefRefPtrBridge;
        }
        public TypeBridgeInfo GetScopePtrBridge()
        {
            if (_scopePtrBridge == null)
            {
                //create new one
                _scopePtrBridge = new TypeBridgeInfo(this, WellKnownTypeName.ScopedPtrOf, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
            }
            return _scopePtrBridge;
        }


        internal string CsMethodParType { get; private set; }
        internal string CsAssignMethodName { get; private set; }
        internal string CsGetValueMethodName { get; private set; }
        /// <summary>
        /// for cs
        /// </summary>
        /// <param name="dotnetTypeName"></param>
        /// <param name="dotnetAssignArgMethodName"></param>
        internal void SetCsNativeCppObjectInterOp(string dotnetTypeName)
        {
            this.CsMethodParType = dotnetTypeName;
            this.CsAssignMethodName = "Cef3Bind.Set_IntPtr";
            this.CsGetValueMethodName = "Cef3Bind.Get_IntPtr";
        }

        internal void SetCsListInterOp(string listOf, string setArgMethod, string getArgMethod)
        {
            this.CsMethodParType = listOf;
            this.CsAssignMethodName = "Cef3Bind.Set_IntPtr";
            this.CsGetValueMethodName = "Cef3Bind.Get_IntPtr";
        }
        /// <summary>
        /// for cs
        /// </summary>
        /// <param name="dotnetTypeName"></param>
        /// <param name="dotnetAssignArgMethodName"></param>
        internal void SetCsInterOpPrimitiveType(string dotnetTypeName)
        {
            this.CsMethodParType = dotnetTypeName;
            this.CsAssignMethodName = "Cef3Bind.Set_" + dotnetTypeName;
            this.CsGetValueMethodName = "Cef3Bind.Get_" + dotnetTypeName;
        }
        /// <summary>
        /// for cs
        /// </summary>
        /// <param name="dotnetTypeName"></param>
        /// <param name="dotnetAssignArgMethodName"></param>
        internal void SetCsInterOp(string dotnetTypeName, string setArgMethod, string getArgMethod)
        {
            this.CsMethodParType = dotnetTypeName;
            this.CsAssignMethodName = setArgMethod;
            this.CsGetValueMethodName = getArgMethod;
        }
        internal void SetCppInterOp(string cppTypeName, string cppReadDataFromDotNet)
        {

        }

    }
    enum CefSlotName
    {
        UNKNOWN,
        //
        type,
        i32,
        ptr,
        num,
        i64
    }

    class CefTypeBridgeTransformPlanner
    {

        //extern "C" { 
        //	struct jsvalue
        //        {
        //            int32_t type; //type and flags
        //                          //this for 32 bits values, also be used as string len, array len  and index to managed slot index
        //            int32_t i32;
        //            // native ptr (may point to native object, native array, native string)
        //            const void* ptr; //uint16_t* or jsvalue**   arr or 
        //                             //store float or double
        //            double num;
        //            //store 64 bits value
        //            int64_t i64;
        //        };
        //    }

        Dictionary<string, TypeSymbol> typeSymbols;
        TypeBridgeInfo SelectTypeBridgeForNonPrimitiveSimpleType(SimpleTypeSymbol simpleType)
        {
            //app-specific
            switch (simpleType.Name)
            {
                default:
                    {
                        if ((simpleType.Name.StartsWith("cef_") || simpleType.Name.StartsWith("_cef")) &&
                            CefResolvingContext.IsAllLowerLetter(simpleType.Name))
                        {
                            //this is native cef?
                            var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.CefCNative, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                            return typeBridge;
                        }
                        else if (simpleType.Name.StartsWith("Cef"))
                        {
                            TypeBridgeInfo bridgeToBase = null;
                            if (simpleType.BaseType != null)
                            {
                                bridgeToBase = SelectProperTypeBridge(simpleType.BaseType);
                            }
                            var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.CefCpp, CefCppSlotKind.JSVALUE_TYPE_WRAPPED, bridgeToBase);
                            typeBridge.SetCsNativeCppObjectInterOp(simpleType.Name);
                            return typeBridge;
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }

                case "CefBase":
                case "CefBaseRefCounted":
                case "CefBaseScoped":
                case "CefStructBase":
                    {
                        return new TypeBridgeInfo(simpleType, WellKnownTypeName.CefAbstract, CefCppSlotKind.JSVALUE_TYPE_ERROR);
                    }
                case "Handler":
                case "RECT":
                case "HINSTANCE":
                case "time_t":
                    {
                        return new TypeBridgeInfo(simpleType, WellKnownTypeName.OtherCppClass, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                    }

            }
        }


        TypeBridgeInfo SelectProperTypeBridge(TypeSymbol t)
        {
            if (t.BridgeInfo != null)
            {
                return t.BridgeInfo;
            }
            //
            //assign bridge info
            //for return, in and out
            switch (t.TypeSymbolKind)
            {
                default:
                    throw new NotSupportedException();
                case TypeSymbolKind.TemplateParameter:
                    {
                        var typeBridge = new TypeBridgeInfo((TemplateParameterTypeSymbol)t, WellKnownTypeName.Vec, CefCppSlotKind.JSVALUE_TYPE_NUMBER);
                        return typeBridge;
                    }
                case TypeSymbolKind.Vec:
                    {
                        var typeBridge = new TypeBridgeInfo((VecTypeSymbol)t, WellKnownTypeName.Vec, CefCppSlotKind.JSVALUE_TYPE_NUMBER);
                        return typeBridge;
                    }
                case TypeSymbolKind.ReferenceOrPointer:
                    {
                        return SelectProperTypeBridge((ReferenceOrPointerTypeSymbol)t);
                    }
                case TypeSymbolKind.Template:
                    {
                        TemplateTypeSymbol templateType = (TemplateTypeSymbol)t;
                        switch (templateType.ItemCount)
                        {
                            default: throw new NotSupportedException();
                            case 1:
                                return SelectProperTypeBridge((TemplateTypeSymbol1)templateType);
                            case 2:
                                return SelectProperTypeBridge((TemplateTypeSymbol2)templateType);
                            case 3:
                                return SelectProperTypeBridge((TemplateTypeSymbol3)templateType);
                        }
                    }
                case TypeSymbolKind.Simple:
                    {
                        SimpleTypeSymbol simpleType = (SimpleTypeSymbol)t;
                        switch (simpleType.PrimitiveTypeKind)
                        {
                            default:
                                throw new NotSupportedException();
                            case PrimitiveTypeKind.NotPrimitiveType:
                                return SelectTypeBridgeForNonPrimitiveSimpleType(simpleType);

                            case PrimitiveTypeKind.Float:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.Float, CefCppSlotKind.JSVALUE_TYPE_NUMBER);
                                    typeBridge.SetCsInterOpPrimitiveType("float");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.Double:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.Double, CefCppSlotKind.JSVALUE_TYPE_NUMBER);
                                    typeBridge.SetCsInterOpPrimitiveType("double");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.Bool:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.Bool, CefCppSlotKind.JSVALUE_TYPE_BOOLEAN);
                                    typeBridge.SetCsInterOpPrimitiveType("bool");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.Char:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.CChar, CefCppSlotKind.JSVALUE_TYPE_INTEGER);
                                    typeBridge.SetCsInterOpPrimitiveType("byte");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.Int32:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.Int32, CefCppSlotKind.JSVALUE_TYPE_INTEGER);
                                    typeBridge.SetCsInterOpPrimitiveType("int");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.NaitveInt:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.NativeInt, CefCppSlotKind.JSVALUE_TYPE_INTEGER);
                                    typeBridge.SetCsInterOpPrimitiveType("long");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.Int64:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.Int64, CefCppSlotKind.JSVALUE_TYPE_INTEGER64);
                                    typeBridge.SetCsInterOpPrimitiveType("long");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.UInt32:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.UInt32, CefCppSlotKind.JSVALUE_TYPE_INTEGER);
                                    typeBridge.SetCsInterOpPrimitiveType("uint");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.Void:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.Void, CefCppSlotKind.JSVALUE_TYPE_EMPTY);
                                    typeBridge.SetCsInterOp("void",
                                                    "!!!",
                                                     "!!!");//should not occour
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.IntPtr:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.IntPtr, CefCppSlotKind.JSVALUE_TYPE_INTEGER64);
                                    typeBridge.SetCsInterOpPrimitiveType("IntPtr");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.UInt64:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.UInt64, CefCppSlotKind.JSVALUE_TYPE_INTEGER64);
                                    typeBridge.SetCsInterOpPrimitiveType("ulong");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.size_t:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.size_t, CefCppSlotKind.JSVALUE_TYPE_INTEGER64);
                                    typeBridge.SetCsInterOpPrimitiveType("int");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.String:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.OtherString, CefCppSlotKind.JSVALUE_TYPE_NATIVE_CEFHOLDER_STRING);
                                    typeBridge.SetCsInterOpPrimitiveType("string");
                                    return typeBridge;
                                }
                            case PrimitiveTypeKind.CefString:
                                {
                                    var typeBridge = new TypeBridgeInfo(simpleType, WellKnownTypeName.CefString, CefCppSlotKind.JSVALUE_TYPE_NATIVE_CEFHOLDER_STRING);
                                    typeBridge.SetCsInterOpPrimitiveType("string");
                                    return typeBridge;
                                }
                        }
                    }
                case TypeSymbolKind.TypeDef:
                    {
                        CTypeDefTypeSymbol typedefSymbol = (CTypeDefTypeSymbol)t;
                        TypeBridgeInfo referToTypeBridge = AssignTypeBridgeInfo(typedefSymbol.ReferToTypeSymbol);
                        var typeBridge = new TypeBridgeInfo(typedefSymbol, WellKnownTypeName.TypeDefToAnother, CefCppSlotKind.JSVALUE_TYPE_WRAPPED, referToTypeBridge);

                        return typeBridge;

                    }
            }
        }

        TypeBridgeInfo SelectProperTypeBridge(ReferenceOrPointerTypeSymbol refOrPointer)
        {

            TypeSymbol elemType = refOrPointer.ElementType;
            //create pointer or reference bridge for existing bridge
            TypeBridgeInfo bridgeToElem = elemType.BridgeInfo;
            if (bridgeToElem == null)
            {
                //no bridge 
                elemType.BridgeInfo = bridgeToElem = SelectProperTypeBridge(elemType);
            }

            TypeBridgeInfo bridge = null;
            switch (refOrPointer.Kind)
            {
                default:
                    throw new NotSupportedException();

                case ContainerTypeKind.ByRef:
                    bridge = bridgeToElem.GetReferenceBridge();
                    break;
                case ContainerTypeKind.Pointer:
                    bridge = bridgeToElem.GetPointerBridge();
                    break;
                case ContainerTypeKind.scoped_ptr:
                    bridge = bridgeToElem.GetScopePtrBridge();
                    break;
                case ContainerTypeKind.CefRefPtr:
                    bridge = bridgeToElem.GetCefRefPtrBridge();
                    break;
                case ContainerTypeKind.CefRawPtr:
                    bridge = bridgeToElem.GetCefRefPtrBridge();
                    break;
            }

            return bridge;
        }
        TypeBridgeInfo SelectProperTypeBridge(TemplateTypeSymbol3 t3)
        {
            WellKnownTypeName wellKnownName = WellKnownTypeName.Unknown;
            switch (t3.Name)
            {
                default:
                    throw new NotSupportedException();
                case "CefCppToCRefCounted":
                    wellKnownName = WellKnownTypeName.CefCppToCRefCounted;
                    break;
                case "CefCppToCScoped":
                    wellKnownName = WellKnownTypeName.CefCppToCScoped;
                    break;
                case "CefCToCppScoped":
                    wellKnownName = WellKnownTypeName.CefCToCppScoped;
                    break;
                case "CefCToCppRefCounted":
                    wellKnownName = WellKnownTypeName.CefCToCppRefCounted;
                    break;
            }
            return new TypeBridgeInfo(t3, wellKnownName, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
        }
        TypeBridgeInfo SelectProperTypeBridge(TemplateTypeSymbol2 t2)
        {
            switch (t2.Name)
            {
                default:
                    throw new NotSupportedException();
                case "multimap":
                    return new TypeBridgeInfo(t2, WellKnownTypeName.MultiMap, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                case "map":
                    //as struct 
                    return new TypeBridgeInfo(t2, WellKnownTypeName.Map, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);

            }
        }
        TypeBridgeInfo SelectProperTypeBridge(TemplateTypeSymbol1 t1)
        {
            switch (t1.Name)
            {
                default:
                    throw new NotSupportedException();
                case "CefStructBase":
                    {
                        //as struct 
                        var typeBridge = new TypeBridgeInfo(t1, WellKnownTypeName.CefStructBase, CefCppSlotKind.JSVALUE_TYPE_WRAPPED);
                        return typeBridge;
                    }
            }
        }

        public void AssignTypeBridgeInfo(Dictionary<string, TypeSymbol> typeSymbols)
        {
            this.typeSymbols = typeSymbols;
            foreach (TypeSymbol t in typeSymbols.Values)
            {
                AssignTypeBridgeInfo(t);
            }
        }
        public TypeBridgeInfo AssignTypeBridgeInfo(TypeSymbol t)
        {
            if ((t is SimpleTypeSymbol) &&
                   ((SimpleTypeSymbol)t).IsGlobalCompilationUnitTypeDefinition)
            {
                return null;
            }
            if (t.BridgeInfo != null)
            {
                return t.BridgeInfo;
            }
            //
            TypeBridgeInfo bridgeInfo = SelectProperTypeBridge(t);
            if (bridgeInfo == null)
            {

            }
            t.BridgeInfo = bridgeInfo;
            return bridgeInfo;
        }


    }
}