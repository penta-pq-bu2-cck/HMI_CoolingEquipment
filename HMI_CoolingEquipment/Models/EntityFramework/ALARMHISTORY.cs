using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace HMI_CoolingEquipment.Models
{
    public class ALARMHISTORY
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int No { get; set; } = 0;

        [Column(TypeName = "DateTime")]
        public DateTime AlarmDateTime { get; set; }
        
        [Column(TypeName = "DateTime")]
        public DateTime AlarmClearedDateTime { get; set; }

        [Required]
        [StringLength(50)]
        public AlarmType AlarmType { get; set; } = AlarmType.Warning;

        [Required]
        public int AlarmCode { get; set; } = 0;

        public string AlarmModule { get; set; } = "Spare";

        public string AlarmMessage { get; set; } = "Spare";

        public string AlarmAction { get; set; } = "Please contact administrator";
    }
}
