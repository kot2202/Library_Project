﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Library_Project.data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class library_projectEntities : DbContext
    {
        public library_projectEntities()
            : base("name=library_projectEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Czytelnicy> Czytelnicy { get; set; }
        public virtual DbSet<Ksiazki> Ksiazki { get; set; }
        public virtual DbSet<Wypozyczenia> Wypozyczenia { get; set; }
    }
}