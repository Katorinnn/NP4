using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPG_Management_System.Models
{
    internal class CustomersTable
    {
        [Key]
        public int CustomerID { get; set; }
        public int TankID { get; set; }
        public string CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
    }
}
