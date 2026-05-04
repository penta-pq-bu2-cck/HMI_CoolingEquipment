using System.ComponentModel.DataAnnotations;

namespace HMI_CoolingEquipment.Models
{
    public class SETTINGS
    {
        [Required]
        [StringLength(50)]
        public string Items { get; set; }

        [Required]
        [StringLength(50)]
        public string Value { get; set; }

        [Required]
        public SettingsType Type { get; set; }
        
        [Required]
        public HMIPage Page { get; set; }
    }
}
