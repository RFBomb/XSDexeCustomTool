﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System.Xml.Serialization

'
'This source code was auto-generated by xsd, Version=4.8.3928.0.
'
Namespace XSDCustomToolVSIX.Example_Files
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true),  _
     System.Xml.Serialization.XmlRootAttribute([Namespace]:="", IsNullable:=false)>  _
    Partial Public Class XSDCustomTool_Parameters
        
        Private xSDexeOptionsField As XSDCustomTool_ParametersXSDexeOptions
        
        Private elementsToGenerateCodeForField() As String
        
        '''<remarks/>
        Public Property XSDexeOptions() As XSDCustomTool_ParametersXSDexeOptions
            Get
                Return Me.xSDexeOptionsField
            End Get
            Set
                Me.xSDexeOptionsField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlArrayItemAttribute("Element", IsNullable:=false)>  _
        Public Property ElementsToGenerateCodeFor() As String()
            Get
                Return Me.elementsToGenerateCodeForField
            End Get
            Set
                Me.elementsToGenerateCodeForField = value
            End Set
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true)>  _
    Partial Public Class XSDCustomTool_ParametersXSDexeOptions
        
        Private nameSpaceField As String
        
        Private languageField As XSDCustomTool_ParametersXSDexeOptionsLanguage
        
        Private languageFieldSpecified As Boolean
        
        Private noLogoField As Boolean
        
        Private noLogoFieldSpecified As Boolean
        
        Private generateClassField As Boolean
        
        Private classOptionsField As XSDCustomTool_ParametersXSDexeOptionsClassOptions
        
        Private dataSetOptionsField As XSDCustomTool_ParametersXSDexeOptionsDataSetOptions
        
        '''<remarks/>
        Public Property [NameSpace]() As String
            Get
                Return Me.nameSpaceField
            End Get
            Set
                Me.nameSpaceField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property Language() As XSDCustomTool_ParametersXSDexeOptionsLanguage
            Get
                Return Me.languageField
            End Get
            Set
                Me.languageField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property LanguageSpecified() As Boolean
            Get
                Return Me.languageFieldSpecified
            End Get
            Set
                Me.languageFieldSpecified = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property NoLogo() As Boolean
            Get
                Return Me.noLogoField
            End Get
            Set
                Me.noLogoField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property NoLogoSpecified() As Boolean
            Get
                Return Me.noLogoFieldSpecified
            End Get
            Set
                Me.noLogoFieldSpecified = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property GenerateClass() As Boolean
            Get
                Return Me.generateClassField
            End Get
            Set
                Me.generateClassField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property ClassOptions() As XSDCustomTool_ParametersXSDexeOptionsClassOptions
            Get
                Return Me.classOptionsField
            End Get
            Set
                Me.classOptionsField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property DataSetOptions() As XSDCustomTool_ParametersXSDexeOptionsDataSetOptions
            Get
                Return Me.dataSetOptionsField
            End Get
            Set
                Me.dataSetOptionsField = value
            End Set
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true)>  _
    Public Enum XSDCustomTool_ParametersXSDexeOptionsLanguage
        
        '''<remarks/>
        CS
        
        '''<remarks/>
        VB
        
        '''<remarks/>
        JS
        
        '''<remarks/>
        VJS
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true)>  _
    Partial Public Class XSDCustomTool_ParametersXSDexeOptionsClassOptions
        
        Private propertiesInsteadOfFieldsField As Boolean
        
        Private propertiesInsteadOfFieldsFieldSpecified As Boolean
        
        Private orderField As Boolean
        
        Private orderFieldSpecified As Boolean
        
        Private enableDataBindingField As Boolean
        
        Private enableDataBindingFieldSpecified As Boolean
        
        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Qualified)>  _
        Public Property PropertiesInsteadOfFields() As Boolean
            Get
                Return Me.propertiesInsteadOfFieldsField
            End Get
            Set
                Me.propertiesInsteadOfFieldsField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property PropertiesInsteadOfFieldsSpecified() As Boolean
            Get
                Return Me.propertiesInsteadOfFieldsFieldSpecified
            End Get
            Set
                Me.propertiesInsteadOfFieldsFieldSpecified = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Qualified)>  _
        Public Property Order() As Boolean
            Get
                Return Me.orderField
            End Get
            Set
                Me.orderField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property OrderSpecified() As Boolean
            Get
                Return Me.orderFieldSpecified
            End Get
            Set
                Me.orderFieldSpecified = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Qualified)>  _
        Public Property EnableDataBinding() As Boolean
            Get
                Return Me.enableDataBindingField
            End Get
            Set
                Me.enableDataBindingField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property EnableDataBindingSpecified() As Boolean
            Get
                Return Me.enableDataBindingFieldSpecified
            End Get
            Set
                Me.enableDataBindingFieldSpecified = value
            End Set
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true)>  _
    Partial Public Class XSDCustomTool_ParametersXSDexeOptionsDataSetOptions
        
        Private enableLinqDataSetField As Boolean
        
        Private enableLinqDataSetFieldSpecified As Boolean
        
        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Qualified)>  _
        Public Property EnableLinqDataSet() As Boolean
            Get
                Return Me.enableLinqDataSetField
            End Get
            Set
                Me.enableLinqDataSetField = value
            End Set
        End Property
        
        '''<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public Property EnableLinqDataSetSpecified() As Boolean
            Get
                Return Me.enableLinqDataSetFieldSpecified
            End Get
            Set
                Me.enableLinqDataSetFieldSpecified = value
            End Set
        End Property
    End Class
End Namespace
