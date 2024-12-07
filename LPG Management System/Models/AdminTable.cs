using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LPG_Management_System.Models;


namespace LPG_Management_System.Models
    {
        public class AdminTable
        {
            [Key]
            public int adminId { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }
    }
