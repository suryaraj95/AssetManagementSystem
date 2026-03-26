using AssetManagement.API.Data;
using AssetManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AssetManagement.API.Services
{
    public interface IAssetIdGeneratorService
    {
        Task<string> GenerateAssetIdAsync(Guid branchId, Guid assetTypeId);
    }

    public class AssetIdGeneratorService : IAssetIdGeneratorService
    {
        private readonly AppDbContext _db;

        public AssetIdGeneratorService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<string> GenerateAssetIdAsync(Guid branchId, Guid assetTypeId)
        {
            var type = await _db.AssetTypes.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == assetTypeId);
            if (type == null) throw new Exception("Asset Type not found");
            
            var branch = await _db.Branches.FindAsync(branchId);
            if (branch == null) throw new Exception("Branch not found");

            string categoryCode = GetThreeLetterCode(type.Category!.Name);
            string typeCode = GetThreeLetterCode(type.Name);
            string branchCode = branch.Code.ToUpper();

            // Row-level locking logic in Postgres requires raw SQL if necessary, but we'll use a transaction for safety
            using var transaction = await _db.Database.BeginTransactionAsync();
            
            var config = await _db.AssetIdConfigs
                .FirstOrDefaultAsync(c => c.BranchCode == branchCode 
                    && c.CategoryCode == categoryCode 
                    && c.TypeCode == typeCode);
            
            if (config == null) {
                config = new AssetIdConfig { 
                    BranchCode = branchCode, 
                    CategoryCode = categoryCode, 
                    TypeCode = typeCode, 
                    LastSequence = 0 
                };
                _db.AssetIdConfigs.Add(config);
            }

            config.LastSequence++;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return $"{config.BranchCode}-{config.CategoryCode}-{config.TypeCode}-{config.LastSequence:D4}";
        }

        private string GetThreeLetterCode(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "XXX";
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                return (parts[0][0].ToString() + parts[1][0].ToString() + (parts.Length > 2 ? parts[2][0].ToString() : parts[1].Length > 1 ? parts[1][1].ToString() : "X")).ToUpper();
            }
            return name.Length >= 3 ? name.Substring(0, 3).ToUpper() : name.PadRight(3, 'X').ToUpper();
        }
    }
}
