﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeasurementService
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ProjectDatabaseMeasureEntities : DbContext
    {
        public ProjectDatabaseMeasureEntities()
            : base("name=ProjectDatabaseMeasureEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LOCATION> LOCATIONS { get; set; }
        public virtual DbSet<MEASUREMENT> MEASUREMENTS { get; set; }
        public virtual DbSet<STATION> STATIONS { get; set; }
    }
}