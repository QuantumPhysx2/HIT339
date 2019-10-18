using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tennis339.Models
{
    public partial class Coach
    {
        [Column("ID")]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Biography { get; set; }
        [Column("PhotoURL")]
        public string PhotoUrl { get; set; }
    }
}
