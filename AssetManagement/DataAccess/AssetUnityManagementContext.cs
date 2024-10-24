using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.DataAccess;

public partial class AssetUnityManagementContext : DbContext
{
    public AssetUnityManagementContext()
    {
    }

    public AssetUnityManagementContext(DbContextOptions<AssetUnityManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asset> Assets { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server =(local); database = AssetUnityManagement;uid=sa;pwd=12345678;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.ToTable("Asset");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.ToTable("File");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AssetId).HasColumnName("assetId");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Path)
                .HasColumnType("text")
                .HasColumnName("path");
            entity.Property(e => e.TagId).HasColumnName("tagId");
            entity.Property(e => e.UploadDate)
                .HasColumnType("datetime")
                .HasColumnName("uploadDate");

            entity.HasOne(d => d.Asset).WithMany(p => p.Files)
                .HasForeignKey(d => d.AssetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_File_Asset");

            entity.HasOne(d => d.Tag).WithMany(p => p.Files)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_File_Tag");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
