using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Entity;

public partial class TridentdemoContext : DbContext
{
    public TridentdemoContext()
    {
    }

    public TridentdemoContext(DbContextOptions<TridentdemoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RegistrationEmail> RegistrationEmails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:tridentwebappdemo.database.windows.net,1433;Initial Catalog=Tridentdemo;Persist Security Info=False;User ID=dbadmin;Password=Trident@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegistrationEmail>(entity =>
        {
            entity.ToTable("RegistrationEmail");

            entity.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.UserEmail).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
