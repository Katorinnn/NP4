using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPG_Management_System.Models
{
    internal class AdminTable
    {
        [Key]
        public int adminId { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string PrivacyMessage { get; set; }  // To store the privacy message
        public bool IsPrivacyActive { get; set; }
    }
}
