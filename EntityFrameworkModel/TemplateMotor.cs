//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntityFrameworkModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class TemplateMotor
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public Nullable<int> IEC { get; set; }
        public Nullable<int> IP { get; set; }
        public string BuildingType { get; set; }
        public string ISO { get; set; }
        public string Power { get; set; }
        public string RPM { get; set; }
        public string Amperage { get; set; }
        public Nullable<int> StartupAmperage { get; set; }
        public Nullable<int> VoltageTypeID { get; set; }
        public Nullable<int> Frequency { get; set; }
        public Nullable<int> PowerFactor { get; set; }
    
        public virtual VoltageType VoltageType { get; set; }
    }
}
