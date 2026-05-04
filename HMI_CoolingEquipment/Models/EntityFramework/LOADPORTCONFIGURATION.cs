using System.ComponentModel.DataAnnotations;

namespace HMI_CoolingEquipment.Models
{
    public class LOADPORTCONFIGURATION
    {
        [Required]
        [StringLength(50)]
        public string LoadPortStatusKey { get; set; }

        public LoadPortState LoadPortStatusState { get; set; }

        public LEDColour LoadPortLEDColour { get; set; }

        public LEDState LoadPortLEDState { get; set; }
    }
}
