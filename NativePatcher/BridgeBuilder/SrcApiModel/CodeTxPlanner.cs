﻿//2016, MIT, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace BridgeBuilder
{
    //about: code transformation 
    abstract class DotNetResolvedTypeBase
    {

    }
    class DotNetResolvedSimpleType : DotNetResolvedTypeBase
    {

        public DotNetResolvedSimpleType(SimpleType simpleType)
        {
            this.SimpleType = simpleType;
        }
        public SimpleType SimpleType { get; set; }
        public override string ToString()
        {
            return SimpleType.ToString();
        }
    }
    class DotNetResolvedArrayType : DotNetResolvedTypeBase
    {
        public DotNetResolvedArrayType(DotNetResolvedTypeBase elementType)
        {
            ElementType = elementType;
        }
        public DotNetResolvedTypeBase ElementType { get; set; }
        public override string ToString()
        {
            return ElementType + "[]";
        }

    }


    class TypeTxInfo
    {

        public List<MethodTxInfo> methods = new List<MethodTxInfo>();
        public TypeTxInfo(CodeTypeDeclaration typedecl)
        {
            this.TypeDecl = typedecl;
        }
        public CodeTypeDeclaration TypeDecl
        {
            get;
            set;
        }
        public void AddMethod(MethodTxInfo met)
        {
            methods.Add(met);
        }
    }

    class MethodTxInfo
    {
        CodeMethodDeclaration metDecl;
        public List<MethodParameterTxInfo> pars = new List<MethodParameterTxInfo>();
        public MethodTxInfo(CodeMethodDeclaration metDecl)
        {
            this.metDecl = metDecl;
            this.Name = metDecl.Name;
        }
        public string Name { get; set; }
        public void AddMethodParameterTx(MethodParameterTxInfo par)
        {
            pars.Add(par);
        }
        public MethodReturnParameterTxInfo ReturnPlan
        {
            get;
            set;
        }

    }
    class MethodReturnParameterTxInfo
    {

        DotNetResolvedTypeBase resolvedBaseSimpleType;
        public MethodReturnParameterTxInfo(CodeTypeReference retType)
        {
            this.SrcReturnTypeReference = retType;
        }
        public CodeTypeReference SrcReturnTypeReference { get; set; }
        public bool IsVoid { get; set; }

        public DotNetResolvedTypeBase ResolvedBaseSimpleType { get; set; }
        public int MarkAsReferenceCount { get; set; }

        public override string ToString()
        {
            return ResolvedBaseSimpleType.ToString();
        }
    }
    class MethodParameterTxInfo
    {
        public MethodParameterTxInfo(CodeMethodParameter p)
        {
            this.Parameter = p;
            this.Name = p.ParameterName;
        }
        public string Name { get; set; }
        public CodeMethodParameter Parameter { get; set; }
        public bool IsVoid { get; set; }

        public DotNetResolvedTypeBase DotnetResolvedType { get; set; }
        public int MarkAsReferenceCount { get; set; }
        public override string ToString()
        {
            return DotnetResolvedType.ToString();
        }
    }

    class TypeTranformPlanner
    {
        CefTypeCollection typeCollection;
        public TypeTranformPlanner(CefTypeCollection typeCollection)
        {
            this.typeCollection = typeCollection;
        }
        public TypeTxInfo MakeTransformPlan(CodeTypeDeclaration typedecl)
        {
            TypeTxInfo typeTxInfo = new TypeTxInfo(typedecl);
            foreach (CodeMemberDeclaration mb in typedecl.Members)
            {
                if (mb.MemberKind == CodeMemberKind.Method)
                {
                    CodeMethodDeclaration metDecl = (CodeMethodDeclaration)mb;
                    if (metDecl.MethodKind == MethodKind.Normal)
                    {
                        MethodTxInfo metTx = MakeMethodPlan(metDecl);
                        if (metTx == null)
                        {
                            throw new NotSupportedException();
                        }
                        typeTxInfo.AddMethod(metTx);
                    }
                }
            }
            return typeTxInfo;
        }

        MethodTxInfo MakeMethodPlan(CodeMethodDeclaration metDecl)
        {
            MethodTxInfo metTx = new MethodTxInfo(metDecl);
            //make return type plan

            //1. return
            MethodReturnParameterTxInfo retTxInfo = new MethodReturnParameterTxInfo(metDecl.ReturnType);
            AddMethodReturnTypeTxInfo(retTxInfo, metDecl.ReturnType.ResolvedType);
            metTx.ReturnPlan = retTxInfo;

            //2. parameters
            int j = metDecl.Parameters.Count;
            for (int i = 0; i < j; ++i)
            {
                CodeMethodParameter metPar = metDecl.Parameters[i];
                MethodParameterTxInfo parTxInfo = new MethodParameterTxInfo(metPar);
                AddMethodParameterTypeTxInfo(parTxInfo, metPar.ParameterType.ResolvedType);
                metTx.AddMethodParameterTx(parTxInfo);
                if (!metPar.IsConstPar)
                {
                    //TODO: review this
                    //if not, this should gen out or ref parameter
                }
            }

            return metTx;
        }
        void AddMethodReturnTypeTxInfo(MethodReturnParameterTxInfo retPlan, TypeSymbol retType)
        {

            //2. type to return in cs side 
            switch (retType.TypeSymbolKind)
            {
                case TypeSymbolKind.Simple:
                    {
                        SimpleType simpleType = (SimpleType)retType;
                        if (IsPrimitiveType(simpleType.Name))
                        {
                            if (simpleType.Name == "void")
                            {
                                retPlan.IsVoid = true;
                            }
                            else
                            {
                                //
                            }
                            retPlan.ResolvedBaseSimpleType = new DotNetResolvedSimpleType(simpleType);
                        }
                        else
                        {
                            //eg ret by reference 
                            //create wrapper for that type
                            retPlan.ResolvedBaseSimpleType = new DotNetResolvedSimpleType(simpleType);
                        }
                    } break;
                case TypeSymbolKind.Vec:
                    {
                        throw new NotSupportedException();
                    }
                    break;
                case TypeSymbolKind.ReferenceOrPointer:
                    {
                        var refOrPointer = (ReferenceOrPointerTypeSymbol)retType;
                        //check element inside
                        switch (refOrPointer.Kind)
                        {
                            case ContainerTypeKind.ByRef:
                                throw new NotSupportedException();
                                break;
                            case ContainerTypeKind.CefRefPtr:
                                {
                                    AddMethodReturnTypeTxInfo(retPlan, refOrPointer.ElementType);
                                    if (IsPrimitiveType(retPlan.ResolvedBaseSimpleType.ToString()))
                                    {
                                        throw new NotSupportedException();
                                    }
                                    else
                                    {
                                        //return reference of unmanaged heap object                                        
                                        //then just pass 
                                        if (retPlan.MarkAsReferenceCount == 0)
                                        {
                                            retPlan.MarkAsReferenceCount++;
                                        }
                                        else
                                        {
                                            throw new NotSupportedException();
                                        }
                                    }

                                } break;
                            case ContainerTypeKind.Pointer:
                                throw new NotSupportedException();
                                break;
                            case ContainerTypeKind.ScopePtr:
                                throw new NotSupportedException();
                                break;
                            default:
                                throw new NotSupportedException();
                                break;

                        }

                    }
                    break;
                default:
                    throw new NotSupportedException();
            }


        }

        void AddMethodParameterTypeTxInfo(MethodParameterTxInfo parPlan, TypeSymbol resolvedParType)
        {


            switch (resolvedParType.TypeSymbolKind)
            {
                case TypeSymbolKind.Simple:
                    {
                        SimpleType simpleType = (SimpleType)resolvedParType;
                        if (IsPrimitiveType(simpleType))
                        {
                            if (simpleType.Name == "void")
                            {
                                parPlan.IsVoid = true;
                            }
                            else
                            {
                                //
                            }
                            parPlan.DotnetResolvedType = new DotNetResolvedSimpleType(simpleType);
                        }
                        else
                        {
                            //eg ret by reference 
                            //create wrapper for that type
                            parPlan.DotnetResolvedType = new DotNetResolvedSimpleType(simpleType);
                        }
                    } break;
                case TypeSymbolKind.Vec:
                    {
                        var vec = (VecTypeSymbol)resolvedParType;
                        AddMethodParameterTypeTxInfo(parPlan, vec.ElementType);
                        //make it array
                        parPlan.DotnetResolvedType = new DotNetResolvedArrayType(
                            parPlan.DotnetResolvedType);

                    }
                    break;
                case TypeSymbolKind.ReferenceOrPointer:
                    {
                        var refOrPointer = (ReferenceOrPointerTypeSymbol)resolvedParType;
                        //check element inside
                        switch (refOrPointer.Kind)
                        {
                            case ContainerTypeKind.ByRef:
                                {
                                    AddMethodParameterTypeTxInfo(parPlan, refOrPointer.ElementType);
                                    if (IsPrimitiveType(parPlan.DotnetResolvedType))
                                    {
                                        throw new NotSupportedException();
                                    }
                                    else
                                    {
                                        //return reference of unmanaged heap object                                        
                                        //then just pass 
                                        if (parPlan.MarkAsReferenceCount == 0)
                                        {
                                            parPlan.MarkAsReferenceCount++;
                                        }
                                        else
                                        {
                                            throw new NotSupportedException();
                                        }
                                    }

                                } break;
                            case ContainerTypeKind.CefRefPtr:
                                {
                                    AddMethodParameterTypeTxInfo(parPlan, refOrPointer.ElementType);

                                    if (IsPrimitiveType(parPlan.DotnetResolvedType))
                                    {
                                        throw new NotSupportedException();
                                    }
                                    else
                                    {
                                        //return reference of unmanaged heap object                                        
                                        //then just pass 
                                        if (parPlan.MarkAsReferenceCount == 0)
                                        {
                                            parPlan.MarkAsReferenceCount++;
                                        }
                                        else
                                        {
                                            throw new NotSupportedException();
                                        }
                                    }

                                } break;
                            case ContainerTypeKind.Pointer:
                                throw new NotSupportedException();
                                break;
                            case ContainerTypeKind.ScopePtr:
                                throw new NotSupportedException();
                                break;
                            default:
                                throw new NotSupportedException();
                                break;
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        static bool IsPrimitiveType(string typename)
        {
            switch (typename)
            {
                case "void":
                case "bool":
                case "char":
                case "int":
                case "int32":
                case "uint32":
                case "int64":
                case "uint64":
                    return true;

            }
            return false;
        }

        static bool IsPrimitiveType(SimpleType ss)
        {
            return IsPrimitiveType(ss.Name);
        }
        static bool IsPrimitiveType(DotNetResolvedTypeBase dnResolvedType)
        {
            var asSimpleType = dnResolvedType as DotNetResolvedSimpleType;
            if (asSimpleType != null)
            {
                return IsPrimitiveType(asSimpleType.SimpleType);
            }
            return false;
        }
    }
}