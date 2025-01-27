using System.ComponentModel.DataAnnotations;

namespace LPG_Management_System.Models
{
    internal class InventoryTable
    {
        [Key]
        public int StocksID { get; set; } // Unique identifier for the inventory item

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } // Name of the product (e.g., brand)

        [Required]
        [MaxLength(50)]
        public string Size { get; set; } // Size of the tank (e.g., "11 kg")

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive.")]
        public decimal Price { get; set; } // Price of the product

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative.")]
        public int Stocks { get; set; } // Price of the product

        public byte[] ProductImage { get; set; } // Image of the product (stored as byte array)

        public DateTime Date { get; set; }
    }
}
