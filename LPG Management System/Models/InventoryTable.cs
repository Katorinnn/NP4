using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPG_Management_System.Models
{
    internal class InventoryTable
    {
        [Key]
        public int TankID { get; set; }
        public string BrandName { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public byte[] ProductImage { get; set; }
    }
}
