using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataModels
{
    public class WishlistItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WishlistItemID { get; set; }

        [Required]
        public int WishlistID { get; set; }

        [ForeignKey(nameof(WishlistID))]
        public Wishlist? Wishlist { get; set; }

        [Required]
        public int ProductID { get; set; }

        [ForeignKey(nameof(ProductID))]
        public Product? Product { get; set; }   // ✅ this is what fixes: wi.Product

        [Required]
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
