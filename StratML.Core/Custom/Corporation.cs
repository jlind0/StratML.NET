using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core.Three;

namespace StratML.Core.Custom
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas:AIIM:org:03201408:Extension")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:schemas:AIIM:org:03201408:Extension", IsNullable = false)]
    public class Corporation : Organization
    {
        //[System.Xml.Serialization.XmlArray("Organizations")]
        [System.Xml.Serialization.XmlElementAttribute("Organization", Namespace = "urn:schemas:AIIM:org:03201408:AdditionalElements")]
        public Organization[] Organizations { get; set; }
        [System.Xml.Serialization.XmlArray("People")]
        public Person[] People { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("PerformancePlanOrReport", Namespace = "urn:schemas:AIIM:org:03201408:AdditionalElements")]
        public PerformancePlanOrReport OverallStrategy { get; set; }
        [System.Xml.Serialization.XmlArray("Strategies")]
        [System.Xml.Serialization.XmlArrayItem("PerformancePlanOrReport", Namespace = "urn:schemas:AIIM:org:03201408:AdditionalElements")]
        public PerformancePlanOrReport[] Strategies { get; set; }
        
    }
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas:AIIM:org:03201408:AdditionalElements")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:schemas:AIIM:org:03201408:Extension", IsNullable = false)]
    public class Person
    {
        [System.Xml.Serialization.XmlElementAttribute("NameDescription", Namespace = "urn:schemas:AIIM:org:03201408:AdditionalElements")]
        public NameDescriptionType NameDescription { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("PointOfContact", Namespace = "urn:schemas:AIIM:org:03201408:AdditionalElements")]
        public ContactInformationType PointOfContact { get; set; }
    }
}
