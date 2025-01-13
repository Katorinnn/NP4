﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPG_Management_System.Models
{
    public class ReportsTable
    {
        [Key]
        public int TransactionID { get; set; }
        public DateTime Date { get; set; }
        public string ProductName { get; set; }
        public int TankID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ChangeGiven { get; set; }
        public TransactionStatus  Status{ get; set; }
    }
    public enum TransactionStatus
    {
        Pending,
        Completed,
        Canceled
    }
    

}
