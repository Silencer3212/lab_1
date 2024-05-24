using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProtectedWeb.Models;

namespace ProtectedWeb;

public partial class SneakersContext : DbContext
{
    public SneakersContext()
    {
    }

    public SneakersContext(DbContextOptions<SneakersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }
    public virtual DbSet<Sneaker> Sneakers { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=sneakers;Username=postgres;Password=12345");

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
