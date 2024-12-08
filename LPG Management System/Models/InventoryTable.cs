using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LPG_Management_System.Models;

namespace LPG_Management_System.Models
{
    public class InventoryTable
    {
        [Key]
        public int TankId { get; set; }
        public string productName { get; set; }
        public string size { get; set; }
        public string price { get; set; }

        public byte[] Image { get; set; }

    }
}
