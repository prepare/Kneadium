﻿//MIT, 2016-2017 ,WinterDev
using System;
using System.Collections.Generic;
namespace BridgeBuilder
{
    class CefTypeCollection
    {
        internal Dictionary<string, CodeTypeDeclaration> typeDics = new Dictionary<string, CodeTypeDeclaration>();
        internal Dictionary<string, TypeSymbol> subTypeDefs = new Dictionary<string, TypeSymbol>();

        internal List<CodeTypeDeclaration> cefBaseTypes = new List<CodeTypeDeclaration>();
        internal List<CodeTypeDeclaration> cefImplTypes = new List<CodeTypeDeclaration>();
        internal List<CodeTypeDeclaration> otherTypes = new List<CodeTypeDeclaration>();

        //semantic
        internal Dictionary<string, TypeSymbol> typeSymbols = new Dictionary<string, TypeSymbol>();



        //------------
        //classification (after all type symbols are created)
        internal List<CodeTypeDeclaration> callBackClasses = new List<CodeTypeDeclaration>();
        internal List<CodeTypeDeclaration> handlerClasses = new List<CodeTypeDeclaration>();
        internal List<CodeTypeDeclaration> cToCppClasses = new List<CodeTypeDeclaration>();
        internal List<CodeTypeDeclaration> cppToCClasses = new List<CodeTypeDeclaration>();
        internal List<CodeTypeDeclaration> otherClasses = new List<CodeTypeDeclaration>();
        //------------

        List<CodeCompilationUnit> compilationUnits;
        Dictionary<string, CodeCompilationUnit> _compilationUnitDics = new Dictionary<string, CodeCompilationUnit>();

        internal Dictionary<TypeSymbol, TypeHierarchyNode> hierarchy;



        void Reset()
        {
            typeDics.Clear();
            cefBaseTypes.Clear();
            cefImplTypes.Clear();
            otherTypes.Clear();
            typeSymbols.Clear();

            //--------------------------
            callBackClasses.Clear();
            handlerClasses.Clear();
            cToCppClasses.Clear();
            cppToCClasses.Clear();
            otherClasses.Clear();

            //--------------------------
            _compilationUnitDics.Clear();
            //--------------------------
            hierarchy = new Dictionary<TypeSymbol, TypeHierarchyNode>();

            //prebuild types & manual added types
            TypeSymbol[] prebuiltTypes = new TypeSymbol[]{

                //-----------------------------------------

                new SimpleTypeSymbol("void"){PrimitiveTypeKind = PrimitiveTypeKind.Void },
                new SimpleTypeSymbol("bool"){PrimitiveTypeKind = PrimitiveTypeKind.Bool },
                new SimpleTypeSymbol("char"){PrimitiveTypeKind = PrimitiveTypeKind.Char },
                new SimpleTypeSymbol("int"){PrimitiveTypeKind = PrimitiveTypeKind.Int32 },//TODO: review here
                new SimpleTypeSymbol("int32"){PrimitiveTypeKind = PrimitiveTypeKind.Int32 },
                new SimpleTypeSymbol("uint32"){PrimitiveTypeKind = PrimitiveTypeKind.UInt32 },
                new SimpleTypeSymbol("int64"){PrimitiveTypeKind = PrimitiveTypeKind.Int64 },
                new SimpleTypeSymbol("uint64"){PrimitiveTypeKind = PrimitiveTypeKind.UInt64 },
                new SimpleTypeSymbol("double"){PrimitiveTypeKind = PrimitiveTypeKind.Double },
                new SimpleTypeSymbol("float"){PrimitiveTypeKind = PrimitiveTypeKind.Float },
                new SimpleTypeSymbol("size_t"){PrimitiveTypeKind = PrimitiveTypeKind.size_t },

                new SimpleTypeSymbol("string"){PrimitiveTypeKind = PrimitiveTypeKind.String },
                new SimpleTypeSymbol("CefString"){PrimitiveTypeKind = PrimitiveTypeKind.CefString },
                new SimpleTypeSymbol("CefBase"),
                new SimpleTypeSymbol("CefBaseRefCounted"),
                new SimpleTypeSymbol("CefBaseScoped"),
                 
                //TODO: review             
                new SimpleTypeSymbol("Handler"),
                new CTypeDefTypeSymbol("CefWindowHandle",new CodeSimpleTypeReference("cef_window_handle_t")),
            };

            foreach (TypeSymbol typeSymbol in prebuiltTypes)
            {
                switch (typeSymbol.TypeSymbolKind)
                {
                    default: throw new NotSupportedException();
                    case TypeSymbolKind.Simple:
                        typeSymbols.Add(((SimpleTypeSymbol)typeSymbol).Name, typeSymbol);
                        break;
                    case TypeSymbolKind.TypeDef:
                        typeSymbols.Add(((CTypeDefTypeSymbol)typeSymbol).Name, typeSymbol);
                        break;
                }

            }
            //--------------------------
        }

        public string RootFolder { get; set; }
        public void SetTypeSystem(List<CodeCompilationUnit> compilationUnits)
        {

            Reset();
            //-----------------------
            this.compilationUnits = compilationUnits;
            //-----------------------
            //resolve cu's file path

            foreach (CodeCompilationUnit cu in compilationUnits)
            {
                //check absolute path for include file                  
                foreach (IncludeFileDirective includeDirective in cu._includeFiles)
                {
                    //remove " from begin and end of the original IncludeFile 
                    if (includeDirective.SystemFolder)
                    {
                        continue;
                    }
                    // 
                    string include_file = includeDirective.IncludeFile.Substring(1, includeDirective.IncludeFile.Length - 2);
                    includeDirective.ResolvedAbsoluteFilePath = RootFolder + "\\" + include_file;
                    //check 
                    if (!System.IO.File.Exists(includeDirective.ResolvedAbsoluteFilePath))
                    {
                        //file not found
                        throw new NotSupportedException();
                    }
                }
                //
                _compilationUnitDics.Add(cu.Filename, cu);
            }


            //-----------------------
            //1. collect
            foreach (CodeCompilationUnit cu in compilationUnits)
            {
                //
                RegisterTypeDeclaration(cu.GlobalTypeDecl);
                //extract type from global typedecl
                foreach (CodeMemberDeclaration subType in cu.GlobalTypeDecl.GetSubTypeIter())
                {
                    if (subType.MemberKind == CodeMemberKind.TypeDef)
                    {
                        CodeCTypeDef ctypeDef = (CodeCTypeDef)subType;
                        // 
                        CTypeDefTypeSymbol ctypedefTypeSymbol = new CTypeDefTypeSymbol(ctypeDef.Name, ctypeDef.From);
                        ctypedefTypeSymbol.CreatedTypeCTypeDef = ctypeDef;
                        //---

                        TypeSymbol existing;
                        if (typeSymbols.TryGetValue(ctypeDef.Name, out existing))
                        {
                            throw new NotSupportedException();
                        }

                        typeSymbols.Add(ctypeDef.Name, ctypedefTypeSymbol);
                    }
                }

                int typeCount = cu.TypeCount;
                for (int i = 0; i < typeCount; ++i)
                {
                    RegisterTypeDeclaration(cu.GetTypeDeclaration(i));
                }
            }
            ResolveBaseTypes();
            ResolveTypeMembers();

            //-----------------------
            //do class classification 
            foreach (CodeTypeDeclaration t in typeDics.Values)
            {
                string name = t.Name;
                if (name.EndsWith("Callback"))
                {
                    callBackClasses.Add(t);
                }
                else if (name.EndsWith("Handler"))
                {
                    handlerClasses.Add(t);
                }
                else if (name.EndsWith("CToCpp"))
                {
                    cToCppClasses.Add(t);
                }
                else if (name.EndsWith("CppToC"))
                {
                    cppToCClasses.Add(t);
                }
                else
                {
                    otherClasses.Add(t);
                }
            }
            //-----------------------
            //for analysis

            foreach (CodeTypeDeclaration t in typeDics.Values)
            {
                TypeSymbol resolvedType = t.ResolvedType;
                if (t.BaseTypes.Count == 0)
                {
                    //
                }
                else
                {
                    TypeSymbol baseType = t.BaseTypes[0].ResolvedType;
                    TypeHierarchyNode found;
                    if (!hierarchy.TryGetValue(baseType, out found))
                    {
                        found = new TypeHierarchyNode(baseType);
                        hierarchy.Add(baseType, found);
                    }

                    if (found.Type != resolvedType)
                    {
                        found.AddTypeSymbol(resolvedType);
                    }
                }
            }
            //----------------------- 
        }


        void RegisterTypeDeclaration(CodeTypeDeclaration typeDecl)
        {
            //1. collect


            if (typeDecl.IsGlobalCompilationUnitType)
            {
                //this is global type                        
                if (typeDecl.MemberCount == 0)
                {
                    //skip this global type
                    return;
                }
            }

            if (!typeDecl.IsForwardDecl && typeDecl.Name != null)
            {
                if (typeDics.ContainsKey(typeDecl.Name))
                {
                    throw new Exception("duplicated key " + typeDecl.Name);
                }
                typeDics.Add(typeDecl.Name, typeDecl);
                //-----------------------

                SimpleTypeSymbol typeSymbol = new SimpleTypeSymbol(typeDecl.Name);
                typeSymbol.CreatedByTypeDeclaration = typeDecl;
                typeDecl.ResolvedType = typeSymbol;
                //

                TypeSymbol existingTypeSymbol;
                if (typeSymbols.TryGetValue(typeSymbol.Name, out existingTypeSymbol))
                {
                    //have existing value
                    throw new NotSupportedException();
                }
                else
                {
                    typeSymbols.Add(typeSymbol.Name, typeSymbol);
                }
                //
                //and sub types
                if (!typeDecl.IsGlobalCompilationUnitType)
                {
                    foreach (CodeMemberDeclaration subType in typeDecl.GetSubTypeIter())
                    {
                        if (subType.MemberKind == CodeMemberKind.TypeDef)
                        {

                            CodeCTypeDef ctypeDef = (CodeCTypeDef)subType;
                            //

                            CTypeDefTypeSymbol ctypedefTypeSymbol = new CTypeDefTypeSymbol(ctypeDef.Name, ctypeDef.From);
                            ctypedefTypeSymbol.CreatedTypeCTypeDef = ctypeDef;
                            ctypedefTypeSymbol.ParentType = typeSymbol;

                            //---
                            typeSymbols.Add(typeSymbol.Name + "." + ctypeDef.Name, ctypedefTypeSymbol);

                            List<TypeSymbol> nestedTypes = typeSymbol.NestedTypeSymbols;
                            if (nestedTypes == null)
                            {
                                typeSymbol.NestedTypeSymbols = nestedTypes = new List<TypeSymbol>();
                            }
                            nestedTypes.Add(ctypedefTypeSymbol);
                        }
                    }

                }

            }

        }


        CodeTypeDeclaration _currentResolvingType = null;



        void ResolveBaseTypes()
        {

            //-----------------------
            //2. resolve allbase type
            foreach (CodeTypeDeclaration typedecl in typeDics.Values)
            {
                _currentResolvingType = typedecl;
                //resolve base type
                List<CodeTypeReference> baseTypes = typedecl.BaseTypes;
                if (baseTypes.Count == 0)
                {
                    //eg. struct

                }
                else
                {
                    foreach (CodeTypeReference baseType in baseTypes)
                    {

                        //if (baseType.Name != "CefBaseRefCounted" &&
                        //    baseType.Name != "struct_type" &&
                        //    baseType.Name != "CefStructBase")
                        //{

                        //}
                        baseType.ResolvedType = ResolveType(baseType);
                    }
                }
            }
            //----------------------- 
            _currentResolvingType = null; //reset
        }
        void ResolveTypeMembers()
        {
            foreach (CodeTypeDeclaration typedecl in typeDics.Values)
            {
                _currentResolvingType = typedecl;

                foreach (CodeMethodDeclaration metDecl in typedecl.GetMethodIter())
                {
                    if (metDecl.MethodKind == MethodKind.Normal)
                    {
                        //resolve return type and type parameter
                        metDecl.ReturnType.ResolvedType = ResolveType(metDecl.ReturnType);
                        foreach (CodeMethodParameter p in metDecl.Parameters)
                        {
                            p.ParameterType.ResolvedType = ResolveType(p.ParameterType);
                        }
                    }
                    else
                    {
                        //this version, we skip other method kind
                    }
                    break;
                }
            }
            _currentResolvingType = null;
        }
        TypeSymbol RegisterBaseCToCppTypeSymbol(CodeTypeReference cToCppTypeReference)
        {

#if DEBUG
            if (!IsAllLowerLetter(cToCppTypeReference.Name))
            {
                //cef-name convention
                throw new NotSupportedException();
            }
#endif
            TypeSymbol found;
            if (!typeSymbols.TryGetValue(cToCppTypeReference.Name, out found))
            {
                //if not found then create the new simple type
                found = new SimpleTypeSymbol(cToCppTypeReference.Name);
                typeSymbols.Add(cToCppTypeReference.Name, found);
            }
            return cToCppTypeReference.ResolvedType = found;
        }

        TypeSymbol ResolveType(CodeTypeReference typeRef)
        {
            //recursive
            switch (typeRef.Kind)
            {
                case CodeTypeReferenceKind.Simple:
                    {
                        var simpleBase = (CodeSimpleTypeReference)typeRef;
                        return ResolveType(simpleBase.Name);
                    }
                case CodeTypeReferenceKind.QualifiedName:
                    {
                        var qnameType = (CodeQualifiedNameType)typeRef;
                        switch (qnameType.LeftPart)
                        {
                            //resolve wellknown type template   

                            case "std":
                                return ResolveType(qnameType.RightPart);
                            default:
                                {
                                    if (_currentResolvingType != null &&
                                        _currentResolvingType.TemplateNotation != null)
                                    {
                                        //search ns from template notation
                                        if (qnameType.LeftPart ==
                                            _currentResolvingType.TemplateNotation.templatePar.ParameterName)
                                        {
                                            //TODO: resolve template type parameter
                                            TemplateParameterTypeSymbol templatePar = new TemplateParameterTypeSymbol(_currentResolvingType.TemplateNotation.templatePar);
                                            return templatePar;
                                        }
                                    }
                                    throw new NotSupportedException();
                                }

                        }
                    }
                case CodeTypeReferenceKind.TypeTemplate:
                    {
                        //resolve wellknown type template   
                        var typeTemplate = (CodeTypeTemplateTypeReference)typeRef;
                        string templateName = typeTemplate.Name;
                        switch (typeTemplate.Name)
                        {
                            default:
                                throw new NotSupportedException();
                            case "CefStructBase":
                                {
                                    TemplateTypeSymbol1 t1 = new TemplateTypeSymbol1(typeTemplate.Name);
                                    t1.Item0 = ResolveType(typeTemplate.Items[0]);
                                    return t1;
                                }
                            case "CefCppToCScoped":
                            case "CefCppToCRefCounted":
                                {
                                    //cpp to c
                                    if (typeTemplate.Items.Count == 3)
                                    {
                                        //auto add native c/c++ type 
                                        TemplateTypeSymbol3 t3 = new TemplateTypeSymbol3(typeTemplate.Name);
                                        t3.Item1 = ResolveType(typeTemplate.Items[1]);
                                        t3.Item2 = RegisterBaseCToCppTypeSymbol(typeTemplate.Items[2]);
                                        return t3;
                                    }
                                    else
                                    {
                                        throw new NotSupportedException();
                                    }
                                }
                            case "CefCToCppScoped":
                            case "CefCToCppRefCounted":
                            case "CefCToCpp":
                                {
                                    //c to cpp
                                    if (typeTemplate.Items.Count == 3)
                                    {
                                        //auto add native c/c++ type
                                        TemplateTypeSymbol3 t3 = new TemplateTypeSymbol3(typeTemplate.Name);
                                        t3.Item1 = ResolveType(typeTemplate.Items[1]);
                                        t3.Item2 = RegisterBaseCToCppTypeSymbol(typeTemplate.Items[2]);
                                        return t3;

                                    }
                                    else
                                    {
                                        throw new NotSupportedException();
                                    }
                                }
                            case "RefCountedThreadSafe":
                                {
                                    switch (typeTemplate.Items.Count)
                                    {
                                        case 1:
                                            return ResolveType(typeTemplate.Items[0]);
                                        case 2:
                                            // from cef c api , 
                                            //template <class T, typename Traits = DefaultRefCountedThreadSafeTraits<T> >
                                            return ResolveType(typeTemplate.Items[0]);

                                        default:
                                            throw new NotSupportedException();
                                    }
                                }
                            case "CefRefPtr":
                                {
                                    if (typeTemplate.Items.Count == 1)
                                    {
                                        return new ReferenceOrPointerTypeSymbol(ResolveType(typeTemplate.Items[0]), ContainerTypeKind.CefRefPtr);
                                    }
                                    throw new NotSupportedException();
                                }
                            case "CefRawPtr":
                                {
                                    if (typeTemplate.Items.Count == 1)
                                    {
                                        return new ReferenceOrPointerTypeSymbol(ResolveType(typeTemplate.Items[0]), ContainerTypeKind.CefRefPtr);
                                    }
                                    throw new NotSupportedException();
                                }
                            case "scoped_ptr":
                                {
                                    if (typeTemplate.Items.Count == 1)
                                    {
                                        return new ReferenceOrPointerTypeSymbol(ResolveType(typeTemplate.Items[0]), ContainerTypeKind.ScopePtr);
                                    }
                                    else
                                    {
                                        throw new NotSupportedException();
                                    }
                                }
                            case "vector":
                                {
                                    if (typeTemplate.Items.Count == 1)
                                    {
                                        return new VecTypeSymbol(ResolveType(typeTemplate.Items[0]));
                                    }
                                    else
                                    {
                                        throw new NotSupportedException();
                                    }
                                }

                        }

                    }
                case CodeTypeReferenceKind.Pointer:
                    {
                        var pointerType = (CodePointerTypeReference)typeRef;
                        TypeSymbol elementType = ResolveType(pointerType.ElementType);
                        return new ReferenceOrPointerTypeSymbol(elementType, ContainerTypeKind.Pointer);
                    }
                case CodeTypeReferenceKind.ByRef:
                    {
                        var byRefType = (CodeByRefTypeReference)typeRef;
                        TypeSymbol elementType = ResolveType(byRefType.ElementType);
                        return new ReferenceOrPointerTypeSymbol(elementType, ContainerTypeKind.ByRef);
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }

        TypeSymbol ResolveType(string typename)
        {
            //-------
            if (_currentResolvingType != null)
            {
                //1. 
                if (_currentResolvingType.IsTemplateTypeDefinition)
                {
                    //** this version, our template notation has only 1 type parameter
                    //TODO: review template notation that has more than 1 type parameters.
                    //
                    //check if this is the template type parameter
                    if (typename == _currentResolvingType.TemplateNotation.templatePar.ReAssignToTypeName)
                    {//found
                        return new TemplateParameterTypeSymbol(_currentResolvingType.TemplateNotation.templatePar);
                    }
                }
                //2.
                //search nest type
                //TODO: review here -> use field                
                if (_currentResolvingType.HasSubType)
                {
                    List<CodeMemberDeclaration> tempResults = new List<CodeMemberDeclaration>();
                    int foundCount;
                    if ((foundCount = _currentResolvingType.FindSubType(typename, tempResults)) > 0)
                    {
                        for (int i = 0; i < foundCount; ++i)
                        {
                            CodeMemberDeclaration subtype = tempResults[i];
                            switch (subtype.MemberKind)
                            {
                                default: throw new NotSupportedException();
                                case CodeMemberKind.TypeDef:
                                    {
                                        CodeCTypeDef ctypedef = (CodeCTypeDef)subtype;
                                        TypeSymbol resolveFromType = ResolveType(ctypedef.From);
                                        if (resolveFromType != null)
                                        {
                                            //found
                                            return resolveFromType;
                                        }
                                    }
                                    break;
                            }

                        }
                    }
                }
                ////3. search global type of current compilation unit( cu)
                //CodeCompilationUnit cu = _currentResolvingType.OriginalCompilationUnit;
                //if (cu.GlobalTypeDecl.MemberCount > 0)
                //{
                //    //
                //}
                //else
                //{

                //}
            }
            //-------


            //-------
            TypeSymbol foundSymbol;
            if (typeSymbols.TryGetValue(typename, out foundSymbol))
            {
                return foundSymbol;
            }

            //this is convention 
            if (typename.StartsWith("cef_") && IsAllLowerLetter(typename))
            {
                //assume this is base c/cpp type
                foundSymbol = new SimpleTypeSymbol(typename);
                typeSymbols.Add(
                    typename,
                    foundSymbol);
                return foundSymbol;
            }

            //not found
            return foundSymbol;
        }
        public bool TryGetTypeDeclaration(string typeName, out CodeTypeDeclaration found)
        {
            return typeDics.TryGetValue(typeName, out found);
        }
        static bool IsAllLowerLetter(string name)
        {

            for (int i = name.Length - 1; i >= 0; --i)
            {
                char c = name[i];
                if (!((c >= 'a' && c <= 'z') || c == '_' || char.IsNumber(c)))
                {
                    return false;
                }
            }
            return true;
        }

    }
    class TypeHierarchyNode
    {
        List<TypeSymbol> children = new List<TypeSymbol>();
        public TypeHierarchyNode(TypeSymbol type)
        {
            this.Type = type;
        }
        public TypeSymbol Type { get; private set; }
        public void AddTypeSymbol(TypeSymbol c)
        {
            if (c == Type)
            {
                throw new Exception("cyclic!");
            }
            children.Add(c);
        }
    }

}