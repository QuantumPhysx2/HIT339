using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tennis339.Models
{
    public partial class ScheduleMembers
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("ScheduleID")]
        public int ScheduleId { get; set; }
        [DataType(DataType.EmailAddress)]
        public string MemberEmail { get; set; }

        [ForeignKey("ScheduleId")]
        [InverseProperty("ScheduleMembers")]
        public Schedule Schedule { get; set; }
    }
}
