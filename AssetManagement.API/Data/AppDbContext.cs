using AssetManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AssetManagement.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<SpecDefinition> SpecDefinitions { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetSpecValue> AssetSpecValues { get; set; }
        public DbSet<AssetHistory> AssetHistories { get; set; }
        public DbSet<AssetRequest> AssetRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AssetIdConfig> AssetIdConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.EmployeeId).IsUnique();
            modelBuilder.Entity<Branch>().HasIndex(b => b.Code).IsUnique();
            modelBuilder.Entity<Asset>().HasIndex(a => a.AssetId).IsUnique();
            modelBuilder.Entity<AssetRequest>().HasIndex(r => r.RequestNumber).IsUnique();
            
            modelBuilder.Entity<SpecDefinition>()
                .HasIndex(s => new { s.AssetTypeId, s.SpecName })
                .IsUnique();
                
            modelBuilder.Entity<AssetSpecValue>()
                .HasIndex(v => new { v.AssetId, v.SpecDefinitionId })
                .IsUnique();

            // Relationships and delete behavior
            modelBuilder.Entity<AssetSpecValue>()
                .HasOne(v => v.Asset)
                .WithMany(a => a.SpecValues)
                .HasForeignKey(v => v.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssetHistory>()
                .HasOne(h => h.Asset)
                .WithMany(a => a.History)
                .HasForeignKey(h => h.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssetRequest>()
                .HasOne(r => r.RequestedBy)
                .WithMany()
                .HasForeignKey(r => r.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetRequest>()
                .HasOne(r => r.ApprovedByHr)
                .WithMany()
                .HasForeignKey(r => r.ApprovedByHrId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetRequest>()
                .HasOne(r => r.ApprovedByAdmin)
                .WithMany()
                .HasForeignKey(r => r.ApprovedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetHistory>()
                .HasOne(h => h.PerformedBy)
                .WithMany()
                .HasForeignKey(h => h.PerformedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetHistory>()
                .HasOne(h => h.FromEmployee)
                .WithMany()
                .HasForeignKey(h => h.FromEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetHistory>()
                .HasOne(h => h.ToEmployee)
                .WithMany()
                .HasForeignKey(h => h.ToEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Data
            var branch1Id = Guid.Parse("943dad51-08de-42b3-b184-de1f272743b6");
            var branch2Id = Guid.Parse("f0bd8ca8-d641-40d1-8562-ef748bfa6ec7");
            modelBuilder.Entity<Branch>().HasData(
                new Branch { Id = branch1Id, Name = "Coimbatore", Code = "CBE", Address = "Coimbatore, TN", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
                new Branch { Id = branch2Id, Name = "Perundurai", Code = "PDR", Address = "Perundurai, TN", IsActive = true, CreatedAt = DateTimeOffset.UtcNow }
            );

            var adminId = Guid.Parse("ba9ed1ab-bba9-4bf9-b527-ac21fa42f8c1");
            var hrId = Guid.Parse("22222222-2222-4222-b222-222222222222");
            var empId = Guid.Parse("33333333-3333-4333-b333-333333333333");
            var emp2Id = Guid.Parse("44444444-4444-4444-b444-444444444444");
            var emp3Id = Guid.Parse("55555555-5555-4555-b555-555555555555");
            var emp4Id = Guid.Parse("66666666-6666-4666-b666-666666666666");
            var emp5Id = Guid.Parse("77777777-7777-4777-b777-777777777777");

            modelBuilder.Entity<User>().HasData(
                new User { Id = adminId, Email = "admin@assetorg.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), FullName = "System Admin", EmployeeId = "EMP-001", Role = "Admin", BranchId = branch1Id, Status = "Active", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = hrId, Email = "hr@assetorg.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("HR@123"), FullName = "HR Manager", EmployeeId = "EMP-002", Role = "HR", BranchId = branch1Id, Status = "Active", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = empId, Email = "employee@assetorg.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"), FullName = "Test Employee", EmployeeId = "EMP-003", Role = "Employee", BranchId = branch2Id, Status = "Active", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = emp2Id, Email = "johndoe@assetorg.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"), FullName = "John Doe", EmployeeId = "EMP-004", Role = "Employee", BranchId = branch1Id, Status = "Active", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = emp3Id, Email = "janesmith@assetorg.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"), FullName = "Jane Smith", EmployeeId = "EMP-005", Role = "Employee", BranchId = branch2Id, Status = "Active", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = emp4Id, Email = "mikej@assetorg.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"), FullName = "Mike Johnson", EmployeeId = "EMP-006", Role = "Employee", BranchId = branch1Id, Status = "Active", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = emp5Id, Email = "emilyd@assetorg.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"), FullName = "Emily Davis", EmployeeId = "EMP-007", Role = "Employee", BranchId = branch2Id, Status = "Active", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow }
            );

            var catEgId = Guid.Parse("e8a88ae2-f263-45cf-8086-91ff3a270431");
            var catFurnId = Guid.Parse("01c7667e-c8c3-4437-9251-7f72cadd5593");
            var catAccId = Guid.Parse("7807dfa2-4e33-4c82-8736-815f5a4d8f11");
            modelBuilder.Entity<AssetCategory>().HasData(
                new AssetCategory { Id = catEgId, Name = "Electronic Gadgets", Description = "Laptops, Mobiles, etc.", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
                new AssetCategory { Id = catFurnId, Name = "Furniture", Description = "Tables, Chairs, etc.", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
                new AssetCategory { Id = catAccId, Name = "Accessories", Description = "Keyboards, Mice, etc.", IsActive = true, CreatedAt = DateTimeOffset.UtcNow }
            );

            var laptopTypeId = Guid.Parse("5cb47334-4c76-4601-a08c-46e312ed59a7");
            modelBuilder.Entity<AssetType>().HasData(
                new AssetType { Id = laptopTypeId, CategoryId = catEgId, Name = "Laptops", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
                new AssetType { Id = Guid.Parse("5e3c0b99-efda-4d23-a60c-6868d78794d2"), CategoryId = catEgId, Name = "Chargers", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
                new AssetType { Id = Guid.Parse("3c16c20c-3dd8-428e-b3c3-46d65d0464b9"), CategoryId = catAccId, Name = "Keyboard", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
                new AssetType { Id = Guid.Parse("12f30718-65e0-4426-acb9-b28e980de613"), CategoryId = catAccId, Name = "Mouse", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
                new AssetType { Id = Guid.Parse("a1684a71-0609-49a6-b637-251de31a32c2"), CategoryId = catEgId, Name = "Monitor", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
                new AssetType { Id = Guid.Parse("a6b45145-1844-4c52-9188-a6891a34fee2"), CategoryId = catFurnId, Name = "Table", IsActive = true, CreatedAt = DateTimeOffset.UtcNow },
                new AssetType { Id = Guid.Parse("60891474-041a-436c-9e2e-94bb162d454d"), CategoryId = catFurnId, Name = "Chair", IsActive = true, CreatedAt = DateTimeOffset.UtcNow }
            );

            modelBuilder.Entity<SpecDefinition>().HasData(
                new SpecDefinition { Id = Guid.Parse("555d1ab0-cfb4-424a-bc9e-4660d8af8c78"), AssetTypeId = laptopTypeId, SpecName = "RAM", SpecDataType = "text", IsRequired = true, DisplayOrder = 1, CreatedAt = DateTimeOffset.UtcNow },
                new SpecDefinition { Id = Guid.Parse("283067c6-7e80-4107-9b42-dab7df858814"), AssetTypeId = laptopTypeId, SpecName = "Processor", SpecDataType = "text", IsRequired = true, DisplayOrder = 2, CreatedAt = DateTimeOffset.UtcNow },
                new SpecDefinition { Id = Guid.Parse("934bb7e0-7176-4929-8ae6-25b91f415084"), AssetTypeId = laptopTypeId, SpecName = "Storage", SpecDataType = "text", IsRequired = true, DisplayOrder = 3, CreatedAt = DateTimeOffset.UtcNow },
                new SpecDefinition { Id = Guid.Parse("9a40815c-e5f2-4365-b870-509d514c6c70"), AssetTypeId = laptopTypeId, SpecName = "Screen Size", SpecDataType = "text", IsRequired = false, DisplayOrder = 4, CreatedAt = DateTimeOffset.UtcNow },
                new SpecDefinition { Id = Guid.Parse("fc2e60b5-a17d-4cff-b5a3-dc8f4fe0e85d"), AssetTypeId = laptopTypeId, SpecName = "OS", SpecDataType = "text", IsRequired = true, DisplayOrder = 5, CreatedAt = DateTimeOffset.UtcNow }
            );
        }
    }
}
