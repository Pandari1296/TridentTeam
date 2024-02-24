using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BaseApplication.Models;

namespace BaseApplication.Entity;

public partial class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Coordinator> Coordinators { get; set; }

    public virtual DbSet<RegistrationEmail> RegistrationEmails { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TridentClient> TridentClients { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coordinator>(entity =>
        {
            entity.ToTable("Coordinator");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<RegistrationEmail>(entity =>
        {
            entity.ToTable("RegistrationEmail");

            entity.Property(e => e.Email).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.RegistrationEmails)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegistrationEmail_Roles");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TridentClient>(entity =>
        {
            entity.ToTable("TridentClient");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(d => d.Coordinator).WithMany(p => p.TridentClients)
                .HasForeignKey(d => d.CoordinatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TridentClient_Coordinator");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.UserEmail).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<BaseApplication.Models.ClientModel> ClientModel { get; set; } = default!;
}
