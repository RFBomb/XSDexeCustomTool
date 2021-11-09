﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

//The idea of the helper class is to preserve the output of XSD.exe for ease of (de)serialization

//NOTE: XSD.exe outputs class names in a camelCase format. 
//Outer-Most classes have start with a lower-case letter, nested classes start with Upper-Case.
//Example:
//outerclass    <-- root level class
//outerclassNestedclassone <-- class 1 nested inside root level class
//outerclassNestedclasstwo <-- class 2 nested inside root level class
//outerclassNestedclasstwoDoublenestedclass  <-- This class is nested within Nestedclasstwo

//Property Names should correlate to the element name of the file if the element is unique.
//Otherwise append 'Array' an to the proeprty name and make it an array.


namespace XSDCustomToolVSIX
{
    /// <summary> Interface that details the requirements for the file generation process that can be called from the SingleFileGenerator. </summary>
    interface IHelperClass
    {

        /// <inheritdoc cref="GenerateHelperClass_Base.FileOnDisk"/>
        FileInfo FileOnDisk { get; }

        /// <inheritdoc cref="GenerateHelperClass_Base.SupplementFileOnDisk"/>
        FileInfo SupplementFileOnDisk { get; }

        /// <inheritdoc cref="GenerateHelperClass_Base.ReadInClassFile"/>
        void ReadInClassFile();

        /// <summary>Parse the file generated by XSD.exe and generate a new file.</summary>
        void Generate();
        
        /// <summary>Generate an additional file to with  the PartialClasses implemented by XSD.exe</summary>
        void GenerateSupplement();
    }

    abstract class GenerateHelperClass_Base : IHelperClass
    {
        private GenerateHelperClass_Base() { }

        /// <summary> Set the base object settings from the xsdSettings Instance provided. </summary>
        protected GenerateHelperClass_Base(XSD_Instance xsdSettings)
        { this.xSD_Instance = xsdSettings; }

        /// <summary> Name of the output class when generating the helper file. </summary>
        public virtual string OutputClassName => xSD_Instance.InputFile.Name.Replace(".xsd", "_HelperClass");

        /// <summary>The character string used to let the compiler know this is the start of a comment line.</summary>
        protected abstract string CommentIndicator { get; }

        /// <summary> This is the First Node of the XML file that all other nodes are underneath. </summary>
        protected SourceClass TopLevelClass { get; set; }

        /// <summary> An array of all the classes found when parsing the output file of XSD.exe </summary>
        protected SourceClass[] DiscoveredClasses { get; set; }

        public HelperInnerClass[] InnerClasses { get; set; }

        /// <summary> File extension to use for the <see cref="FileOnDisk"/> and <see cref="SupplementFileOnDisk"/> properties </summary>
        public abstract string GeneratedFileExtension { get; }

        /// <summary> Location of the helper class on disk. </summary>
        public FileInfo FileOnDisk => new FileInfo(xSD_Instance.InputFile.FullName.Replace(".xsd", $"_HelperClass{GeneratedFileExtension}"));

        /// <summary> Location of the Supplement file on disk. </summary>
        public FileInfo SupplementFileOnDisk => new FileInfo(xSD_Instance.InputFile.FullName.Replace(".xsd", $"_AutoGenerated_Supplement{GeneratedFileExtension}"));

        /// <summary> This is the instance of XSD settings to work with. </summary>
        public XSD_Instance xSD_Instance { get; }

        public bool IsGeneratingClass => xSD_Instance.IsGeneratingClass;
        public bool IsGeneratingDataSet => xSD_Instance.IsGeneratingDataSet;

        ///<inheritdoc cref="XSD_Instance.OutputFile"/>
        protected FileInfo GeneratedFile => xSD_Instance.OutputFile;

        protected string TabLevel(int i) => VSTools.TabIndent(i);

        /// <summary>Generates the comment header of the file that says the file was automatically generated and when it will be overwritten.</summary>
        /// <returns>There are 3 Environment.NewLine keys inserted at the end of this comment.</returns>
        protected virtual string GetComment_AutoGen()
        {
            string FileText = "";
            FileText += $"{CommentIndicator}------------------------------------------------------------------------------\n";
            FileText += $"{CommentIndicator} <auto-generated>\n";
            FileText += $"{CommentIndicator}     This code was generated by XSDCustomTool VisualStudio Extension.\n";
            FileText += $"{CommentIndicator}     This file is only generated if it is missing, so it is safe to modify this file as needed.\n";
            FileText += $"{CommentIndicator}     If the file is renamed or deleted, then it will be regenerated the next time the custom tool is run.\n";
            FileText += $"{CommentIndicator}     The base file contains the Load(string), Save(string) methods, several constructors, \n";
            FileText += $"{CommentIndicator}     and several properties to work with the class file generated by XSD.exe.\n";
            FileText += $"{CommentIndicator} </auto-generated>\n";
            FileText += $"{CommentIndicator}------------------------------------------------------------------------------\n";
            FileText += "\n\n";
            return FileText;
        }

        /// <summary>Generates the comment header of the supplement file that says the file was automatically generated and when it will be overwritten.</summary>
        /// <returns>There are 3 Environment.NewLine keys inserted at the end of this comment.</returns>
        protected virtual string GetComment_Supplement()
        {
            string FileText = "";
            FileText += $"{CommentIndicator}------------------------------------------------------------------------------\n";
            FileText += $"{CommentIndicator} <auto-generated>\n";
            FileText += $"{CommentIndicator}     This code was generated by XSDCustomTool VisualStudio Extension.\n";
            FileText += $"{CommentIndicator}     This file is only generated if it is missing, so it is safe to modify this file as needed.\n";
            FileText += $"{CommentIndicator}     If the file is renamed or deleted, then it will be regenerated the next time the custom tool is run.\n\n";
            FileText += $"{CommentIndicator}     This file is meant to act as a supplement to the file generated by XSD.exe, \n";
            FileText += $"{CommentIndicator}     allowing setting up constructors, methods, and defaults for the partial classes that XSD.exe generated.\n";
            FileText += $"{CommentIndicator} </auto-generated>\n";
            FileText += $"{CommentIndicator}------------------------------------------------------------------------------\n";
            FileText += "\n\n";
            return FileText;
        }

        /// <summary>Get the comment for the Save(string) method.</summary>
        /// <returns></returns>
        protected virtual string GetComment_SaveMethod(int BaseIndentLevel)
        {
            return String.Concat(
                $"{VSTools.TabIndent(BaseIndentLevel)}{CommentIndicator} < summary>\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}{CommentIndicator} This method will take the {TopLevelClass.ClassName} object, create an XML serializer for it, and write the XML to the <paramref name = \"FilePath\" />\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}{CommentIndicator} </summary>\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}{CommentIndicator} <param name=\"FilePath\"> Destination file path to save the file into. </param>\n"
                );
        }

        protected virtual string GetComment_LoadMethod(int BaseIndentLevel)
        {
            return String.Concat(
                $"{VSTools.TabIndent(BaseIndentLevel)}{CommentIndicator} <summary>\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}{CommentIndicator} Load a file path and produce a Deserialized <typeparamref name=\"{TopLevelClass.ClassName}\"/> Object\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}{CommentIndicator} </summary>\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}{CommentIndicator} <param name=\"FilePath\"> This XML file to read into the class object </param>\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}{CommentIndicator} <returns> A new <typeparamref name=\"{TopLevelClass.ClassName}\"/> object </returns>\n"
                );
        }

        protected abstract string GenerateConstructors(int BaseIndentLevel);

        /// <summary> Generate a Load(string) method to deserialize an XML file into this helper class. </summary>
        /// <returns></returns>
        protected abstract string GetClassLoaderMethod(int BaseIndentLevel);

        /// <summary> Generate a Save(string) method to serialize an XML file from this class. </summary>
        /// <returns></returns>
        protected abstract string GetClassSaverMethod(int BaseIndentLevel);

        /// <summary> Generate a Save(string) method to serialize an XML file from this class. </summary>
        /// <returns></returns>
        protected abstract string GenerateClassTree(int BaseIndentLevel); 

        public static GenerateHelperClass_Base HelperClassFactory(XSD_Instance xsdSettings)
        {
            switch (xsdSettings.XSDexeOptions.Language)
            {
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.CS : return new GenerateHelperClass_CSharp(xsdSettings);
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VB: return new GenerateHelperClass_VB(xsdSettings);
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.JS : return new GenerateHelperClass_JavaScript(xsdSettings);
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VJS: return new GenerateHelperClass_JSharp(xsdSettings);
                default: throw new NotImplementedException("Unknown Output Language");
            }
        }

        /// <summary>
        /// Parse the file generated by XSD.exe and generate a new file. <br/>
        /// This should start by calling base.ReadInClassFile(), then generate the required code, and finally end with base.Save();
        /// </summary>
        public abstract void Generate();

        /// <summary>
        /// After reading in the file generated by XSD.exe using the <see cref="Generate"/> method, 
        /// run this method to create a supplement file for the parial classes XSD.exe generated.
        /// </summary>
        public  abstract void GenerateSupplement();

        /// <summary>
        /// Read in the class file. The ParseLoop method is called from here.
        /// This will then store the DiscoveredClasses output by the parse loop into to the DiscoveredClasses and TopLevelClass properties.
        /// </summary>
        public virtual void ReadInClassFile()
        {
            List<string> FileText = new List<string> { };
            string ln;
            //Read the file into memory
            using (StreamReader rdr = GeneratedFile.OpenText())
            {
                do
                {
                    ln = rdr.ReadLine();
                    FileText.Add(ln);
                } while (ln != null);
            }

            //Begin Parsing the File
            SourceClass[] discoveredClasses = ParseLoop(FileText.ToArray(), 0);
            if (discoveredClasses.Length > 0) this.TopLevelClass = discoveredClasses[0]; //Assume the first found class is the top level class
            this.DiscoveredClasses = discoveredClasses;
        }

        /// <summary>
        /// This method is called by the <see cref="ReadInClassFile"/> method.
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="StartIndex"></param>
        /// <returns>An array of the discovered classes found when parsing the file output by XSD.exe. </returns>
        protected abstract SourceClass[] ParseLoop(string[] txt, int StartIndex);

        /// <summary>
        /// Writes the file text to the FileOnDisk location, then adds it to the project.
        /// </summary>
        /// <param name="fileText"></param>
        protected void Save(string fileText)
        {
            File.WriteAllText(this.FileOnDisk.FullName, fileText);
            AddToProject(FileOnDisk);
        }

        /// <summary>
        /// Writes the file text to the FileOnDisk location, then adds it to the project.
        /// </summary>
        /// <param name="fileText"></param>
        protected void  SaveSupplement(string fileText)
        {
            File.WriteAllText(this.SupplementFileOnDisk.FullName, fileText);
            AddToProject(SupplementFileOnDisk);
        }

        /// <summary>
        /// Adds the file to the project.
        /// </summary>
        protected virtual void AddToProject(FileInfo  file)
        {
            if (file.Exists)
                VSTools.AddFileToProject(xSD_Instance.InputFile, file);
        }

        public class HelperInnerClass
        {

        }
    }

    /// <summary> This represents a class that was discovered when evaluting the output file from XSD.exe </summary>
    public abstract class SourceClass
    {
        /// <summary> Setup the object with a className. This constructor makes an object assumed to be a child element. </summary>
        /// <param name="className"></param>
        public SourceClass(string className, string[] classText)
        {
            IsTopLevelNode = false;
            ClassText = classText;
            Init(className);
        }

        /// <summary>
        /// If an autogenerated constructor was found during parsing,  set this to  true.
        /// </summary>
        /// <remarks>BaseFunctionality searches the <see cref="ClassText"/> for "public  {ClassName}()  "</remarks>
        public virtual bool AutoGeneratedConstructorExists => ClassText.Any((string s) => s.Contains($"public {ClassName}()"));

        /// <summary> Setup the object with a className.</summary>
        /// <param name="className"></param>
        /// <param name="isTopLevelNode">Set TRUE if the class represents the very first element in the xml file.</param>
        public SourceClass(string className, string[] classText, bool isTopLevelNode)
        {
            IsTopLevelNode = IsTopLevelNode;
            ClassText = classText;
            Init(className);
        }

        protected virtual void Init(string className)
        {
            ClassName = className;
            HelperClass_PropertyName = className + "_Property";
        }

        public abstract Enums.SupportedLanguages ClassLanguage { get; }

        public bool IsTopLevelNode { get; }

        /// <summary> This is the name of the class (the 'type' of the class.)</summary>
        public string ClassName { get; private set; }

        /// <summary> This is the raw text of the class </summary>
        public string[] ClassText { get; }

        /// <summary> This is the name of the property within the parent class. </summary>
        public string HelperClass_PropertyName { get; set; }

        /// <summary> The class may have multiple inner classes. These are defined in a list here. </summary>
        public List<SourceClass> InnerClasses { get; } = new List<SourceClass>();

        public List<ClassProperty> ClassProperties { get; } = new List<ClassProperty>();

        /// <summary> Runs the appropriate method based on the selected output language. </summary>
        /// <param name="BaseIndentLevel"></param>
        /// <returns>
        /// A string that represents a tree of nested classes, starting with the class object that called this method <br/>
        /// Example:   <br/>class Caller{   <br/>
        /// _     NestedClass_1 PropertyName {get;} <br/>
        /// _     NestedClass_2 PropertyName {get;} <br/>
        /// _     class NestedClass_1{}  <br/>
        /// _     class NestedClass_2{}  <br/> 
        /// }
        /// </returns>
        public abstract string BuildClassTree(int BaseIndentLevel);

        /// <summary>Get the string that represents this class as a property of another class.</summary>
        /// <param name="IndentLevel"></param>
        /// <param name="IsPublic">set TRUE to return a Public Property, set FALSE to return a Private property</param>
        public abstract string GetPropertyString(int IndentLevel, bool IsPublic = true);

        /// <summary>Get the string that builds a simple constructor for this class.</summary>
        /// <param name="BaseIndentLevel"></param>
        /// <returns> Class() {} </returns>
        public abstract string GetConstructors(int BaseIndentLevel);

    }

    /// <summary>
    /// Represents the discovered Property of a class
    /// </summary>
    public class ClassProperty
    {
        private ClassProperty() { }
        
        public ClassProperty(string Name, string Type, bool isGeneratedClassType, bool isSerializable)
        {
            this.PropertyName = Name;
            this.PropertyType = Type;
            this.IsGeneratedClassType = isGeneratedClassType;
            this.IsSerializable = isSerializable;
        }

        public string PropertyName { get; }
        public string PropertyType { get; }
        public bool IsGeneratedClassType { get; }
        public bool IsSerializable { get; }
    }
}
