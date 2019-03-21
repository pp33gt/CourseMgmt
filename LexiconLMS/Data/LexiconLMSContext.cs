﻿using LexiconLMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Data
{
    public class LexiconLMSContext : IdentityDbContext<User>
    {
        public LexiconLMSContext(DbContextOptions<LexiconLMSContext> options) : base(options)
        {
        }

        /*** Properties of DbSets for entities that you want to interact with directly ***/

        public DbSet<Course> Courses { get; set; }

        public DbSet<Module> Modules { get; set; }

        public DbSet<Activityy> Activities { get; set; }

        public DbSet<ActivityType> ActivityType { get; set; }

        public DbSet<CourseDocument> CourseDocument { get; set; }

        public DbSet<ModuleDocument> ModuleDocument { get; set; }

        public DbSet<ActivityDocument> ActivityDocument { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<Course>()
            //    .HasOne<User>(c => c.Teacher) //Needed for creating a foreign key to the Teacher.
            //    .WithOne(d => d.Course)
            //    .HasForeignKey<User>(e => e.CourseId);

            builder.Entity<Module>();

            builder.Entity<Course>()
                .HasMany<User>(u => u.Users)
                .WithOne(c => c.Course)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<User>()
                .HasOne<Course>(c => c.Course);

            builder.Entity<Module>();

            builder.Entity<Activityy>()
                .HasMany<ActivityDocument>(d => d.Documents)
                .WithOne(c => c.Activityy)
                .HasForeignKey(e => e.ActivityId);

            builder.Entity<ActivityType>();

            builder.Entity<CourseDocument>();
            
            builder.Entity<ModuleDocument>();

            builder.Entity<ActivityDocument>()
                .HasOne<Activityy>(a => a.Activityy)
                .WithMany(b => b.Documents)
                .HasForeignKey(e => e.ActivityId);
            
            /*** Seed Data for Activity Types ***/

            builder.Entity<ActivityType>()
                .HasData(
                    new ActivityType() { Id = 1, Type = "E-Learning" },
                    new ActivityType() { Id = 2, Type = "Lectures" },
                    new ActivityType() { Id = 3, Type = "Exercise" }
                );

            /*** Seed Data for course ***/

            SeedData.SeedCourseData(builder);

        }     

        //public DbSet<LexiconLMS.Models.GenericDocument> GenericDocument { get; set; }
    }
}
