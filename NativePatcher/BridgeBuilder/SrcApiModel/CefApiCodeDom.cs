﻿//2016, MIT, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace BridgeBuilder
{
    //still very dirty parser!
    //this version is designed for cef3 only

    class CodeCTypeDef : CodeMemberDeclaration
    {
        public CodeCTypeDef(CodeTypeReference from, string name)
        {
            this.From = from;
            this.Name = name;
        }
        public CodeTypeReference From { get; set; }
        public string Name { get; set; }
        public override CodeMemberKind MemberKind { get { return CodeMemberKind.TypeDef; } }
        public override string ToString()
        {
            return "typedef " + From + " " + this.Name + ";";
        }

    }
    enum TypeKind
    {
        Class,
        Struct
    }
    class CodeTypeDeclaration : CodeMemberDeclaration
    {
        public CodeTypeDeclaration()
        {
            this.BaseTypes = new List<CodeTypeReference>();
            this.Members = new List<CodeMemberDeclaration>();
            this.SpecialImplMacroMembers = new List<CodeMemberDeclaration>();
        }
        public TypeKind Kind { get; set; }
        public string Name { get; set; }
        public bool BaseIsPublic { get; set; }
        public bool BaseIsVirtual { get; set; }
        public bool IsGlobalCompilationUnitType { get; set; }
        public List<CodeTypeReference> BaseTypes { get; set; }
        public List<CodeMemberDeclaration> Members { get; set; }
        public override CodeMemberKind MemberKind { get { return CodeMemberKind.Type; } }
        public bool IsForwardDecl { get; set; }
        public override string ToString()
        {
            if (IsGlobalCompilationUnitType)
            {
                return "!global";
            }

            StringBuilder stbuilder = new StringBuilder();
            switch (this.Kind)
            {
                case TypeKind.Class:
                    stbuilder.Append("class ");
                    break;
                case TypeKind.Struct:
                    stbuilder.Append("struct ");
                    break;
                default:
                    throw new NotSupportedException();
            }
            stbuilder.Append(Name);
            if (this.IsForwardDecl)
            {
                stbuilder.Append(';');
            }
            return stbuilder.ToString();
        }
        public List<CodeMemberDeclaration> SpecialImplMacroMembers { get; set; }

        //-------------
        //semantic 
        public TypeSymbol ResolvedType { get; set; }
    }
    class CodeCompilationUnit
    {
        public CodeCompilationUnit()
        {
            this.Members = new List<CodeTypeDeclaration>();
        }
        public string Filename { get; set; }
        public List<CodeTypeDeclaration> Members { get; set; }
        public override string ToString()
        {
            return this.Filename;
        }
    }

    enum CodeTypeReferenceKind
    {
        Simple,
        FuncPointer,
        QualifiedName,
        TypeTemplate,
        Pointer,
        ByRef,

    }
    abstract class CodeTypeReference
    {
        public CodeTypeReference() { }
        public CodeTypeReference(string typename)
        {
            this.Name = typename;
        }

        public string Name
        {
            get;
            set;
        }
        public override string ToString()
        {
            return this.Name;
        }
        public abstract CodeTypeReferenceKind Kind { get; }
        //-------------
        //semantic 
        public TypeSymbol ResolvedType { get; set; }
    }
    class CodeSimpleTypeReference : CodeTypeReference
    {
        public CodeSimpleTypeReference(string typename)
            : base(typename)
        {
        }
        public override CodeTypeReferenceKind Kind { get { return CodeTypeReferenceKind.Simple; } }
    }

    class CodeFunctionPointerTypeRefernce : CodeTypeReference
    {
        public CodeFunctionPointerTypeRefernce()
        {
            this.Parameters = new List<CodeMethodParameter>();
        }
        public CodeTypeReference ReturnType { get; set; }
        public List<CodeMethodParameter> Parameters { get; set; }
        public override CodeTypeReferenceKind Kind { get { return CodeTypeReferenceKind.FuncPointer; } }
        public override string ToString()
        {

            var stbuilder = new StringBuilder();
            stbuilder.Append(ReturnType.ToString());
            int j = Parameters.Count;
            stbuilder.Append('(');
            for (int i = 0; i < j; ++i)
            {
                if (i > 0)
                {
                    stbuilder.Append(',');
                }
                stbuilder.Append(Parameters[i]);
            }
            stbuilder.Append(')');

            return stbuilder.ToString();
        }
    }
    class CodeQualifiedNameType : CodeTypeReference
    {

        public CodeQualifiedNameType(string leftPartName, CodeTypeReference rightPart)
            : base(leftPartName)
        {
            this.RightPart = rightPart;
        }
        public CodeTypeReference RightPart { get; set; }
        public override CodeTypeReferenceKind Kind { get { return CodeTypeReferenceKind.QualifiedName; } }
        public override string ToString()
        {
            return base.Name + "::" + RightPart.ToString();
        }
    }
    class CodeTypeTemplateTypeReference : CodeTypeReference
    {
        //similar to C# generic
        List<CodeTypeReference> templateItems = new List<CodeTypeReference>();
        public CodeTypeTemplateTypeReference(string typename)
            : base(typename)
        {

        }
        public List<CodeTypeReference> Items { get { return templateItems; } }
        public void AddTemplateItem(CodeTypeReference item)
        {
            templateItems.Add(item);
        }
        public override CodeTypeReferenceKind Kind { get { return CodeTypeReferenceKind.TypeTemplate; } }
        public override string ToString()
        {
            StringBuilder stbuilder = new StringBuilder();
            stbuilder.Append(base.Name);
            stbuilder.Append('<');
            int j = templateItems.Count;
            for (int i = 0; i < j; ++i)
            {
                if (i > 0)
                {
                    stbuilder.Append(',');
                }
                stbuilder.Append(templateItems[i].ToString());
            }
            stbuilder.Append('>');
            return stbuilder.ToString();
        }

    }
    class CodePointerTypeReference : CodeTypeReference
    {
        CodeTypeReference elementType;
        public CodePointerTypeReference(CodeTypeReference elementType)
        {
            this.elementType = elementType;
        }
        public CodeTypeReference ElementType { get { return this.elementType; } }
        public override CodeTypeReferenceKind Kind { get { return CodeTypeReferenceKind.Pointer; } }
        public override string ToString()
        {
            return elementType.ToString() + "*";
        }
    }
    class CodeByRefTypeReference : CodeTypeReference
    {
        CodeTypeReference elementType;
        public CodeByRefTypeReference(CodeTypeReference elementType)
        {
            this.elementType = elementType;
        }
        public CodeTypeReference ElementType { get { return this.elementType; } }
        public override CodeTypeReferenceKind Kind { get { return CodeTypeReferenceKind.ByRef; } }
        public override string ToString()
        {
            return elementType.ToString() + "&";
        }
    }
    abstract class CodeMemberDeclaration
    {
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public abstract CodeMemberKind MemberKind { get; }
    }

    enum CodeMemberKind
    {
        Type,
        TypeDef,
        Field,
        Method
    }
    enum MethodKind
    {
        Normal,
        Ctor,
        Dtor
    }
    class CodeFieldDeclaration : CodeMemberDeclaration
    {
        public CodeFieldDeclaration()
        {
        }
        public string Name { get; set; }
        public bool IsStatic { get; set; }
        public bool IsConst { get; set; }
        public override CodeMemberKind MemberKind { get { return CodeMemberKind.Field; } }
        public CodeTypeReference FieldType { get; set; }
        public override string ToString()
        {
            return FieldType.ToString() + " " + Name + ";";
        }
    }

    class CodeMethodDeclaration : CodeMemberDeclaration
    {
        public CodeMethodDeclaration()
        {
            this.Parameters = new List<CodeMethodParameter>();
        }
        public string Name { get; set; }
        public List<CodeMethodParameter> Parameters { get; set; }
        public CodeTypeReference ReturnType { get; set; }
        public bool IsOverrided { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsStatic { get; set; }
        public bool IsConst { get; set; }
        public MethodKind MethodKind { get; set; }

        public override CodeMemberKind MemberKind { get { return CodeMemberKind.Method; } }
        public override string ToString()
        {
            StringBuilder stbuilder = new StringBuilder();
            switch (MethodKind)
            {
                case BridgeBuilder.MethodKind.Ctor:
                    //do nothing
                    break;
                case BridgeBuilder.MethodKind.Dtor:
                    stbuilder.Append('~');
                    break;
                default:
                    stbuilder.Append(ReturnType.ToString());
                    stbuilder.Append(' ');
                    break;
            }

            stbuilder.Append(Name);
            stbuilder.Append('(');

            int parCount = Parameters.Count;
            for (int i = 0; i < parCount; ++i)
            {
                if (i > 0)
                {
                    stbuilder.Append(',');
                }
                stbuilder.Append(Parameters[i].ToString());
            }

            stbuilder.Append(')');
            return stbuilder.ToString();
        }


    }

    class CodeMethodParameter
    {
        public string ParameterName
        {
            get;
            set;
        }
        public CodeTypeReference ParameterType
        {
            get;
            set;
        }
        /// <summary>
        /// const modifier
        /// </summary>
        public bool IsConstPar
        {
            get;
            set;
        }
        public bool IsConstParName
        {
            get;
            set;
        }
        public bool IsConstPointerParName
        {
            get;
            set;
        }
        public override string ToString()
        {
            StringBuilder stbuild = new StringBuilder();
            if (IsConstPar)
            {
                stbuild.Append("const ");
            }
            stbuild.Append(this.ParameterType.ToString());
            if (this.IsConstParName)
            {
                stbuild.Append(" const");
                if (IsConstPointerParName)
                {
                    stbuild.Append("*");
                }
            }
            if (ParameterName != null)
            {
                stbuild.Append(" ");
                stbuild.Append(this.ParameterName);
            }
            return stbuild.ToString();

        }
    }

}