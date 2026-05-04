using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.DataModels
{
    public class UserKeepItem
    {
        [Key]
        public int UserKeepItemID { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Required]
        public int ProductID { get; set; }

        [ForeignKey(nameof(ProductID))]
        public Product Product { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ItemValue { get; set; }
    }
}