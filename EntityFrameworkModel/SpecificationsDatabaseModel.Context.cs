﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ATEXType> ATEXTypes { get; set; }
        public virtual DbSet<CatType> CatTypes { get; set; }
        public virtual DbSet<CustomOrderMotor> CustomOrderMotors { get; set; }
        public virtual DbSet<CustomOrder> CustomOrders { get; set; }
        public virtual DbSet<CustomOrderVentilator> CustomOrderVentilators { get; set; }
        public virtual DbSet<CustomOrderVentilatorTest> CustomOrderVentilatorTests { get; set; }
        public virtual DbSet<GroupType> GroupTypes { get; set; }
        public virtual DbSet<SoundLevelType> SoundLevelTypes { get; set; }
        public virtual DbSet<TemperatureClass> TemperatureClasses { get; set; }
        public virtual DbSet<TemplateMotor> TemplateMotors { get; set; }
        public virtual DbSet<TemplateVentilator> TemplateVentilators { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<VentilatorType> VentilatorTypes { get; set; }
    }
}
