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
    
    public partial class CustomOrderVentilatorTest
    {
        public int ID { get; set; }
        public int CustomOrderVentilatorID { get; set; }
        public Nullable<int> MeasuredVentilatorHighRPM { get; set; }
        public Nullable<int> MeasuredVentilatorLowRPM { get; set; }
        public Nullable<int> MeasuredMotorHighRPM { get; set; }
        public Nullable<int> MeasuredMotorLowRPM { get; set; }
        public Nullable<int> MeasuredBladeAngle { get; set; }
        public Nullable<int> Cover { get; set; }
        public string MotorNumber { get; set; }
        public Nullable<int> Weight { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<decimal> I1High { get; set; }
        public Nullable<decimal> I1Low { get; set; }
        public Nullable<decimal> I2High { get; set; }
        public Nullable<decimal> I2Low { get; set; }
        public Nullable<decimal> I3High { get; set; }
        public Nullable<decimal> I3Low { get; set; }
    
        public virtual CustomOrderVentilator CustomOrderVentilator { get; set; }
        public virtual User User { get; set; }
    }
}
