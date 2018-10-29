﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
namespace DOSBookedResults
{
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class api
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("method", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public apiMethod[] method;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("error", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public apiError  error;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        [System.Xml.Serialization.XmlArrayItemAttribute("booked_events", typeof(apiDataBooked_eventsEvent[]), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [System.Xml.Serialization.XmlArrayItemAttribute("event", typeof(apiDataBooked_eventsEvent), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false, NestingLevel = 1)]
        public apiDataBooked_eventsEvent[][]  data;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ver;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string timestamp;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class apiMethod
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("parameter", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public apiMethodParameter[] parameter;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string details;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string total_items;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string previous_page;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string next_page;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class apiMethodParameter
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class apiError
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string message;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string status;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string internal_code;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class apiDataBooked_eventsEvent
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string client_event_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string booked_by;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string source;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string relation_status;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string start_date;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ft_only;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string coverage_type;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string scoutsfeed;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string status_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string status_name;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string status_type;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string sport_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string sport_name;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string day;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string clock_time;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string clock_status;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string winner_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string progress_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string bet_status;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string neutral_venue;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string item_status;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ut;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string old_event_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string slug;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string area_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string area_name;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string competition_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string competition_short_name;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string season_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string season_name;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string stage_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string stage_name;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string verified_result;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string round_id;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string round_name;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string inverted_participants;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class NewDataSet
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("api", Order = 0)]
        public api[] Items;
    }
}


////------------------------------------------------------------------------------
//// <auto-generated>
////     This code was generated by a tool.
////     Runtime Version:4.0.30319.42000
////
////     Changes to this file may cause incorrect behavior and will be lost if
////     the code is regenerated.
//// </auto-generated>
////------------------------------------------------------------------------------

//// 
//// This source code was auto-generated by xsd, Version=4.6.1055.0.
//// 
//namespace DOSBookedResults {
//    using System.Xml.Serialization;


//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
//    [System.SerializableAttribute()]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
//    public partial class api {

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("method", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
//        public apiMethod[] method;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("error", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
//        //public apiError[] error;
//        public apiError error;
//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string ver;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string timestamp;
//    }

//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
//    [System.SerializableAttribute()]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
//    public partial class apiMethod {

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("parameter", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
//        public apiMethodParameter[] parameter;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string name;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string details;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string total_items;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string previous_page;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string next_page;
//    }

//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
//    [System.SerializableAttribute()]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
//    public partial class apiMethodParameter {

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string name;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string value;
//    }

//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
//    [System.SerializableAttribute()]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
//    public partial class apiError {

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string message;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string status;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlAttributeAttribute()]
//        public string internal_code;
//    }

//    /// <remarks/>
//    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
//    [System.SerializableAttribute()]
//    [System.Diagnostics.DebuggerStepThroughAttribute()]
//    [System.ComponentModel.DesignerCategoryAttribute("code")]
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
//    public partial class NewDataSet {

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("api", Order=0)]
//        public api[] Items;
//    }
//}
