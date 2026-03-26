using System;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.API.Models
{
    public class AssetIdConfig
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(10)]
        public string BranchCode { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string CategoryCode { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string TypeCode { get; set; } = string.Empty;

        public int LastSequence { get; set; } = 0;

        [MaxLength(100)]
        public string Pattern { get; set; } = "{BRANCH}-{CAT}-{TYPE}-{SEQ:4}";
    }
}
