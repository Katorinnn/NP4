using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPG_Management_System.Models
{
    internal class CompanyTable
    {
        [Key]
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }
        public string CompanyEmail { get; set; }

    }
}
