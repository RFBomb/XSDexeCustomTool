﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XSDCustomToolVSIX
{
    class GenerateHelperClass_CSharp : GenerateHelperClass_Base
    {

        public GenerateHelperClass_CSharp(XSD_Instance xsdSettings) : base(xsdSettings) { }

        public override string OutputClassName => xSD_Instance.InputFile.Name.Replace(".xsd", "_HelperClass");
        public string NameSpace => xSD_Instance.XSDexeOptions.NameSpace;

        public override string GeneratedFileExtension => ".cs";

        protected override string CommentIndicator => "//";

        #region < Parse AutogeneratedFile >

        protected override SourceClass[] ParseLoop(string[] txt, int StartIndex)
        {
            
            int? OpenBracketIndex = null; int OpenBracketCount = 0; int ClosedBracketCount = 0; int i = StartIndex;
            string className = null;
            List<string> classText = new List<string> { };
            List<string> DiscoverClassNames = new List<string>();
            List<SourceClass_CSharp> DiscoveredClasses = new List<SourceClass_CSharp> { };
            List<ClassProperty> ClassProperties = new List<ClassProperty>();
            SourceClass_CSharp[] nestedClasses = new SourceClass_CSharp[0] { };

            while ( i <= txt.Length )
            {
                string ln = txt[i];

                if (ln.Contains("partial class"))
                {
                    if (className is null)
                    {
                        //Setup the ClassName and reset the bracket counters
                        className = ExtractClassNameFromLine(ln);
                        OpenBracketIndex = i;
                        OpenBracketCount = 0;
                        ClosedBracketCount = 0;
                    }
                    else
                        nestedClasses = (SourceClass_CSharp[])ParseLoop(txt, i); // Found an inner class -> begin parsing for that class.
                }
                else if (className != null && ln.Contains("public") && !( ln.Contains("void") | ln.Contains("event") ))
                {
                    //This is going to be an auto-generated Property
                    string[] arr = ln.Split(' ');
                    int expectedlocation = arr.ToList().IndexOf("public");
                    string PropType = arr[expectedlocation +1];
                    string PropName = arr[expectedlocation + 2];
                    bool IsSerializableProperty = !classText.Last().Contains("XmlIgnoreAttribute");
                    bool IsGeneratedClassProperty = PropType.Contains(className) | DiscoverClassNames.Contains(PropType); //Indicates this is a class property of another auto-generated class.
                    if (IsSerializableProperty)
                        ClassProperties.Add(new ClassProperty(PropName, PropType, IsGeneratedClassProperty, IsSerializableProperty));
                }

                if (className != null) classText.Add(ln);
                OpenBracketCount += ln.Count( c => c == '{' ); // the first instance of should be on the start of the class.
                ClosedBracketCount += ln.Count( c => c == '}' );
                if (OpenBracketCount == ClosedBracketCount & className != null)
                {
                    // this should be the end of the class in question
                    SourceClass_CSharp @class = new SourceClass_CSharp(className, classText.ToArray());
                    @class.InnerClasses.AddRange(nestedClasses?.ToList());
                    @class.ClassProperties.AddRange(ClassProperties);
                    DiscoveredClasses.Add(@class);
                    DiscoverClassNames.Add(@class.ClassName);
                    className = null;
                    ClassProperties = new List<ClassProperty>();
                }
                else if (OpenBracketCount < ClosedBracketCount) // hit more than closed brackets than open brackets -> end of file.
                    return DiscoveredClasses.ToArray();
                i++;
            }
            return DiscoveredClasses.ToArray();

        }

        private string ExtractClassNameFromLine(string LineText)
        {
            string[] arr = LineText.Split(' ');
            int expectedlocation = arr.ToList().IndexOf("class");
            if (expectedlocation > 0) return arr[expectedlocation +1 ]; //public partial class [classname]
            throw new Exception("Unexpected! - Output did not conform to expected format.");
        }

        private static string RegionWrap(string inputTxt, string RegionName, int BaseIndentLevel)
        {
            string txt = $"{VSTools.TabIndent(BaseIndentLevel)}#region < {RegionName} >\n\n";
            txt += inputTxt;
            txt += $"{VSTools.TabIndent(BaseIndentLevel)}#endregion </ {RegionName} >\n\n";
            return txt;
        }

        #endregion </ Parse AutogeneratedFile >

        #region < Generate Helper Class >

        public override void Generate()
        {
            string txt = "";
            txt += "using System;\n";
            txt += "using System.IO;\n";
            txt += "using System.Xml.Serialization;\n\n";
            txt += base.GetComment_AutoGen();
            txt += $"namespace {this.NameSpace} {{\n\n"; // NameSpace
            txt += $"{TabLevel(1)}/// <summary>Helper class to ease working with {this.xSD_Instance.InputFile.Name} autogenerated {(IsGeneratingClass? "class" : "dataset")}</summary>\n";
            txt += $"{TabLevel(1)}public partial class {this.OutputClassName} {{\n\n";
            txt += RegionWrap(GenerateConstructors(2), "Constructors", 2);
            txt += RegionWrap(GenerateProperties(2), "Properties", 2);
            txt += RegionWrap(GetClassLoaderMethod(2) + GetClassSaverMethod(2), "Saving & Loading XML Files", 2);
            txt += $"{TabLevel(1)}}}\n"; // Close out the class 
            txt += $"}}"; // Close out the namespace
            //Write to file
            base.Save(txt);
        }

        /// <summary></summary>
        /// <param name="BaseIndentLevel"></param>
        /// <returns></returns>
        protected override string GenerateConstructors(int BaseIndentLevel)
        {
            string NoArgs = String.Concat(
                $"{VSTools.TabIndent(BaseIndentLevel)}/// <summary> Construct a new instance of the {OutputClassName} object. </summary>\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}public {OutputClassName}()\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}{{\n",
                $"{VSTools.TabIndent(BaseIndentLevel + 1)} // TO DO: assign values for all the properties\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}}}\n\n"
                );

            string FilePathArg = String.Concat(
                $"{VSTools.TabIndent(BaseIndentLevel)}/// <summary> Construct a new instance of the {OutputClassName} object by Deserializing an XML file. </summary>\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}/// <param name=\"FilePath\"> This XML file to read into the class object </param>\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}public {OutputClassName}(string FilePath)\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}{{\n",
                $"{VSTools.TabIndent(BaseIndentLevel + 1)} {TopLevelClass.HelperClass_PropertyName} = Load(FilePath);\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}}}\n\n"
                );

            string DeserializedXML = String.Concat(
                $"{VSTools.TabIndent(BaseIndentLevel)}/// <summary> Construct a new instance of the {OutputClassName} object from an existing <typeparamref name=\"{TopLevelClass.ClassName}\"/> object. </summary>\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}/// <param name=\"{TopLevelClass.HelperClass_PropertyName.ToLower()}\"> A pre-existing <typeparamref name=\"{TopLevelClass.ClassName}\"/> object.</param>\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}public {OutputClassName}({TopLevelClass.ClassName} {TopLevelClass.HelperClass_PropertyName.ToLower()})\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}{{\n",
                $"{VSTools.TabIndent(BaseIndentLevel + 1)} {TopLevelClass.HelperClass_PropertyName} = {TopLevelClass.HelperClass_PropertyName.ToLower()};\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}}}\n\n"
                );

            return String.Concat(NoArgs, FilePathArg, DeserializedXML);
        }

        /// <summary></summary>
        /// <param name="BaseIndentLevel"></param>
        /// <returns></returns>
        protected string GenerateProperties(int BaseIndentLevel)
        {
            List<string> PropertyList = new List<string>();
            PropertyList.Add(TopLevelClass.GetPropertyString(BaseIndentLevel));
            foreach (SourceClass cl in this.DiscoveredClasses)
                if (cl != TopLevelClass)
                    if (!cl.ClassName.Contains(TopLevelClass.ClassName))
                        PropertyList.Add(cl.GetPropertyString(BaseIndentLevel));

            string ret = "";
            foreach (string s in PropertyList)
                ret += s + "\n\n";
            
            return ret;

        }

        /// <summary> Generate a Load(string) method to deserialize an XML file into this helper class. </summary>
        /// <returns></returns>
        protected override string GetClassLoaderMethod(int BaseIndentLevel)
        {

            string Comments = GetComment_LoadMethod(BaseIndentLevel);

            string Method = String.Concat(
                $"{VSTools.TabIndent(BaseIndentLevel)}public static {TopLevelClass.ClassName} Load(string FilePath) {{\n",
                    $"{VSTools.TabIndent(BaseIndentLevel + 1)}{TopLevelClass.ClassName} retObj = null;\n",
                    $"{VSTools.TabIndent(BaseIndentLevel + 1)}try {{\n");
            if (IsGeneratingClass)
            {
                Method = String.Concat(Method,
                        $"{VSTools.TabIndent(BaseIndentLevel + 2)}using (Stream stream = File.Open(FilePath, FileMode.Open)) {{\n",
                            $"{VSTools.TabIndent(BaseIndentLevel + 3)}XmlSerializer serializer = new XmlSerializer(typeof({TopLevelClass.ClassName}));\n",
                            $"{VSTools.TabIndent(BaseIndentLevel + 3)}retObj = ({TopLevelClass.ClassName})serializer.Deserialize(stream);\n",
                        $"{VSTools.TabIndent(BaseIndentLevel + 2)}}}\n");
            } else if (IsGeneratingDataSet)
            {
                // Must use DataSet.ReadXML to load into the class.
                throw new NotImplementedException("Testing with DataSets not done yet!");
                Method = String.Concat(Method,
                        $"{VSTools.TabIndent(BaseIndentLevel + 2)}using (Stream stream = File.Open(FilePath, FileMode.Open)) {{\n",
                            $"{VSTools.TabIndent(BaseIndentLevel + 3)}XmlSerializer serializer = new XmlSerializer(typeof({TopLevelClass.ClassName}));\n",
                            $"{VSTools.TabIndent(BaseIndentLevel + 3)}retObj = ({TopLevelClass.ClassName})serializer.Deserialize(stream);\n",
                        $"{VSTools.TabIndent(BaseIndentLevel + 2)}}}\n");
            }
            Method = String.Concat(Method,
                $"{VSTools.TabIndent(BaseIndentLevel + 1)}}} catch (Exception E) {{\n",
                        $"{VSTools.TabIndent(BaseIndentLevel + 2)}throw new NotImplementedException(\"Catch Statement Not Implemented. See Inner Error.\", E);\n",
                    $"{VSTools.TabIndent(BaseIndentLevel + 1)}}}\n",
                    $"{VSTools.TabIndent(BaseIndentLevel + 1)}return retObj;\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}}}\n\n"
                );
            return String.Concat(Comments, Method);
        }

        /// <summary> Generate a Save(string) method to serialize an XML file from this class. </summary>
        /// <returns></returns>
        protected override string GetClassSaverMethod(int BaseIndentLevel)
        {
            string Comments = base.GetComment_SaveMethod(BaseIndentLevel);

            string Method = String.Concat(
                $"{VSTools.TabIndent(BaseIndentLevel)}public void SaveXMLFile(string FilePath) {{\n",
                    $"{VSTools.TabIndent(BaseIndentLevel + 1)}try {{\n",
                        $"{VSTools.TabIndent(BaseIndentLevel + 2)}Directory.CreateDirectory(new FileInfo(FilePath).DirectoryName);\n");
            if (IsGeneratingClass)
            {
                Method = String.Concat(Method,
                $"{VSTools.TabIndent(BaseIndentLevel + 2)}using (Stream stream = File.Open(FilePath, FileMode.Create)) {{\n",
                            $"{VSTools.TabIndent(BaseIndentLevel + 3)}XmlSerializer serializer = new XmlSerializer(typeof({TopLevelClass.ClassName}));\n",
                            $"{VSTools.TabIndent(BaseIndentLevel + 3)}serializer.Serialize(stream, this.{TopLevelClass.HelperClass_PropertyName});\n",
                            $"{VSTools.TabIndent(BaseIndentLevel + 3)}stream.Flush();\n",
                        $"{VSTools.TabIndent(BaseIndentLevel + 2)}}}\n");
            } else if (IsGeneratingDataSet)
            {
                // Save DataSet to xml
                // Use WriteXml to write the document.
                //OriginalDataSet.WriteXml(xmlFilename);
            }
            Method = String.Concat(Method,
                $"{VSTools.TabIndent(BaseIndentLevel + 1)}}} catch (Exception E) {{\n",
                        $"{VSTools.TabIndent(BaseIndentLevel + 2)}throw new NotImplementedException(\"Catch Statement Not Implemented. See Inner Error.\", E);\n",
                    $"{VSTools.TabIndent(BaseIndentLevel + 1)}}}\n",
                $"{VSTools.TabIndent(BaseIndentLevel)}}}\n\n"
                );
            return String.Concat(Comments, Method);
        }

        /// <summary>Generate the Class Tree of nested helper classes within this helper class</summary>
        /// <param name="BaseIndentLevel"></param>
        /// <returns></returns>
        protected override string GenerateClassTree(int BaseIndentLevel) => TopLevelClass.BuildClassTree(BaseIndentLevel);

        #endregion </ Generate >

        #region < Supplement File >

        public override void GenerateSupplement()
        {
            string txt = "";
            string region;
            txt += "using System;\n";
            txt += "using System.IO;\n";
            txt += "using System.Xml.Serialization;\n\n";
            txt += base.GetComment_Supplement();
            txt += $"namespace {this.NameSpace} {{\n\n"; // NameSpace Start
            foreach (SourceClass_CSharp cls in DiscoveredClasses)
            {
                //Process Inner Classes / Properties
                string Ser = "";
                string classProps = "";
                foreach (ClassProperty prop in cls.ClassProperties)
                {
                    //ShouldSerialize -> Classes rely on their properties to determine serialization, so only process non-class properties.
                    if (prop.IsSerializable)
                    {
                        if (Ser != "") Ser += "\n";
                        Ser += ShouldSerializeProperty(prop.PropertyName, 2);
                    }
                    if (!cls.AutoGeneratedConstructorExists)
                    {
                        //Property Constructors
                        string CST;
                        switch (prop.PropertyType.ToLower())
                        {
                            case "string": CST = "String.Empty;"; break;
                            case "bool": CST = "false;"; break;
                            default:
                                if (prop.PropertyType.Contains("[]"))
                                    CST = $"new {prop.PropertyType}{{}};"; //Array
                                else
                                    CST = $"new {prop.PropertyType}();"; //Assume Class
                                break;

                        }
                        classProps += $"{TabLevel(3)}{prop.PropertyName} = {CST}\n";
                    }
                }

                //Start the class
                region = String.Concat(
                $"{TabLevel(1)}/// <summary>\n",
                $"{TabLevel(1)}/// Partial Class {cls.ClassName} generated by XSD.exe\n",
                $"{TabLevel(1)}/// </summary>\n",
                $"{TabLevel(1)}public partial class {cls.ClassName}\n", // Class Start
                $"{TabLevel(1)}{{\n");
                if (!cls.AutoGeneratedConstructorExists)
                {
                    region = String.Concat(region,
                        $"{TabLevel(2)}public {cls.ClassName}()\n",   //Constructor Start
                        $"{TabLevel(2)}{{\n",
                        $"{TabLevel(3)}// TO DO: assign values for all the properties\n",
                        $"{classProps}",
                        $"{TabLevel(2)}}}\n");//Constructor End
                } else
                    region += $"{TabLevel(2)}//AutoGenerated File already has parameterless constructor.\n";
                
                region += $"\n";
                string SerComment = $"{TabLevel(2)}/* \n" +
                    $"{TabLevel(2)}ShouldSerialize is run  by the XML Serializer against properties to determine whether to write them to disk. \n" +
                    $"{TabLevel(2)}The Default functionality (without this method) is to only serialize if the value is changed from the default. \n" +
                    $"{TabLevel(2)}These methods override that functionality, allowing the programmer to decide when they are serialized.\n" +
                    $"{TabLevel(2)}To restore original functionality (allowing Serializer to decide for each parameter), comment out these methods.\n" +
                    $"{TabLevel(2)}*/\n\n";
                Ser = String.IsNullOrWhiteSpace(Ser) ? String.Empty : RegionWrap(SerComment + Ser + "\n", "ShouldSerializeProperty", 2);
                region += Ser;
                region += $"{TabLevel(1)}}}\n\n";//Class End Bracket
                txt += RegionWrap(region, "Partial Class "+ cls.ClassName, 1);
            }
            txt += "}"; //NameSpace End
            base.SaveSupplement(txt);
        }

        private string ShouldSerializeProperty(string PropName, int IndentLevel)
            => String.Concat(
                $"{TabLevel(IndentLevel)}/// <summary>Determine when the <see cref=\"{PropName}\"/> property is written to disk during XML Serialization</summary>\n",
                $"{TabLevel(IndentLevel)}private bool ShouldSerialize{PropName}() => true;\n");

        #endregion </ Supplement >

    }

    class SourceClass_CSharp : SourceClass
    {
        /// <inheritdoc cref="SourceClass.SourceClass(string)"/>
        public SourceClass_CSharp(string className, string[] classText) : base(className, classText) { }

        /// <inheritdoc cref="SourceClass.SourceClass(string, bool)"/>
        public SourceClass_CSharp(string className, string[] classText, bool isTopLevelNode) : base(className, classText, isTopLevelNode) { }

        public override Enums.SupportedLanguages ClassLanguage => Enums.SupportedLanguages.CSharp;

        /// <returns>"{Public/Private} {ClassName} {PropertyName} {get;set;}"</returns>
        /// <inheritdoc cref="GetPropertyString(int, bool)"/>
        public override string GetPropertyString(int IndentLevel, bool IsPublic = true)
            => $"{VSTools.TabIndent(IndentLevel)}/// <summary>  </summary>\n" +
            $"{VSTools.TabIndent(IndentLevel)}{(IsPublic ? "public" : "private")} {ClassName} {HelperClass_PropertyName} {{ get; {(IsPublic ? "private " : "")}set; }}";

        /// <inheritdoc cref="GetConstructors(int)"/>
        public override string GetConstructors(int IndentLevel) => $"{VSTools.TabIndent(IndentLevel)}public {this.ClassName}() {{}}";

        /// <inheritdoc cref="BuildClassTree(int)"/>
        public override string BuildClassTree(int IndentLevel)
        {
            string properties = String.Empty;
            string nested = String.Empty;

            foreach (SourceClass IC in InnerClasses)
                properties = String.Concat(properties, IC.GetPropertyString(IndentLevel + 1), Environment.NewLine);

            foreach (SourceClass IC in InnerClasses)
                nested = String.Concat(nested, IC.BuildClassTree(IndentLevel + 1), Environment.NewLine);

            string thisclass = String.Concat(
                $"{VSTools.TabIndent(IndentLevel)}#region < {this.ClassName} >\n",
                this.GetConstructors(IndentLevel + 1), Environment.NewLine,

                $"{VSTools.TabIndent(IndentLevel)}#region < Nested Class Objects Properties >\n",
                properties, Environment.NewLine,
                $"{VSTools.TabIndent(IndentLevel)}#endregion </ Nested Class Objects Properties >\n", Environment.NewLine,

                $"{VSTools.TabIndent(IndentLevel)}#region < Nested Classes >\n",
                nested, Environment.NewLine,
                $"{VSTools.TabIndent(IndentLevel)}#endregion </ Nested Classes >\n",

                $"{VSTools.TabIndent(IndentLevel)}#endregion </ {this.ClassName} >"
                );

            return thisclass;
        }

    }
}
