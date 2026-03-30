using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.DataModels
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        [Required]
        public string CustomerID { get; set; }

        [ForeignKey(nameof(CustomerID))]
        public ApplicationUser Customer { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
