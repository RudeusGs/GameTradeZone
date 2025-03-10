using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class PostInfo : EntityBase
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public string Caption { get; set; }

        [Required]
        public string Content { get; set; }
     
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }

        public int LikesCount { get; set; } = 0;

        public int CommentsCount { get; set; } = 0;

        public virtual User User { get; set; }
    }
}
