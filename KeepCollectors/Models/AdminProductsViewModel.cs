using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace KeepCollectors.Models
{
    public class AdminProductsEditViewModel
    {
        public int ProductID { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int QuantityAvailable { get; set; }

        // Either paste a URL...
        [Display(Name = "Image URL (optional)")]
        public string? ImageUrl { get; set; }

        // ...or upload a file
        [Display(Name = "Upload Image (optional)")]
        public IFormFile? ImageFile { get; set; }

        // For Edit page preview
        public string? ExistingImagePath { get; set; }
    }
}