using DataAccessLayer.DataModels;
namespace KeepCollectors.Models
{
    public class KeepViewModel
    {
        public List<OrderItem> PurchasedItems { get; set; } = new();
        public List<WishlistItem> SavedItems { get; set; } = new();

        public decimal CollectionWorth { get; set; }
        public decimal SavedItemsWorth { get; set; }
        public decimal TotalKeepWorth => CollectionWorth + SavedItemsWorth;

    }
}
