using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataModels
{
    public class Wishlist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WishlistID { get; set; }

        [Required]
        public String CustomerID { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(CustomerID))]
        public ApplicationUser? Customer { get; set; }

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

    }
}
