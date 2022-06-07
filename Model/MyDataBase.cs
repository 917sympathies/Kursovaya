using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Kursovaya
{
    class MyDataBase : DbContext
    {
        public MyDataBase()
        {
            Database.EnsureCreated();
        }
        public virtual DbSet<Student> students { get; set; }
        public virtual DbSet<Teacher> teachers { get; set; }
        public virtual DbSet<Subject> subjects { get; set; }
        public virtual DbSet<Mark> marks { get; set; }
        public virtual DbSet<LogAndPass> logsAndPass { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder modelBuilder)
        {
            modelBuilder.UseSqlite("Data Source=KursachDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.HasMany(w => w.Предметы).WithMany(c => c.Teachers).UsingEntity(w => w.ToTable("Enrollments"));
            });
        }
    }
}
