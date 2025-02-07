using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Domain.Entities
{
    public class SepayWebHooksReceiver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Gateway { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [MaxLength(100)]
        public string? AccountNumber { get; set; }

        [MaxLength(250)]
        public string? SubAccount { get; set; }

        [Column(TypeName = "decimal(20,2)")]
        public decimal AmountIn { get; set; } = 0.00m;

        [Column(TypeName = "decimal(20,2)")]
        public decimal AmountOut { get; set; } = 0.00m;

        [Column(TypeName = "decimal(20,2)")]
        public decimal Accumulated { get; set; } = 0.00m;

        [MaxLength(250)]
        public string? Code { get; set; }

        public string? TransactionContent { get; set; }

        [MaxLength(255)]
        public string? ReferenceNumber { get; set; }

        public string? Body { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
