using System.ComponentModel.DataAnnotations;

namespace HMI_CoolingEquipment.Models
{
    public class ALARMDEFINITION
    {
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
