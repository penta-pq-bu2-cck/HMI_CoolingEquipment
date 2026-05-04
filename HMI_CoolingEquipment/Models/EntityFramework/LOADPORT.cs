using System.ComponentModel.DataAnnotations;

namespace HMI_CoolingEquipment.Models
{
    public class LOADPORT
    {
        [Required]
        [StringLength(50)]
        public string LoadPortID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LoadPortRack { get; set; }

        [Required]
        [StringLength(50)]
        public string LoadPortColumn { get; set; }

        [Required]
        [StringLength(50)]
        public string LoadPortRow { get; set; }

        [Required]
        [StringLength(50)]
        public string LoadPortPriority { get; set; }

        [Required]
        [StringLength(50)]
        public string LoadPortType { get; set; }

        [StringLength(50)]
        public string? LoadPortCarrierLoad { get; set; } = null;
        
        [StringLength(50)]
        public string? LoadPortIPAddress { get; set; }
        
        public int? LoadPortNodeAddress { get; set; }

        public bool LoadPortNodeConnectionStatus { get; set; } = false;

        public bool LoadPortError { get; set; } = false;

        public LoadPortState LoadPortState { get; set; } = LoadPortState.Empty;

        [StringLength(50)]
        public string? PreAssignLoad { get; set; } = null;
        
        public bool LoadPortIsEnable { get; set; } = true;
    }
}
