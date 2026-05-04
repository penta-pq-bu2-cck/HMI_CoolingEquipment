using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace HMI_CoolingEquipment.Models
{
    public class JOBDETAILS
    {
        [Required]
        [StringLength(50)]
        public string JobID { get; set; }

        [Required]
        [StringLength(50)]
        public string LotNo { get; set; }

        [Required]
        [StringLength(50)]
        public string CarrierID { get; set; }

        [StringLength(50)]
        public string PortLocation { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? LoadingTime { get; set; }

        [StringLength(50)]
        public string LoadBy { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? UnloadingTime { get; set; }

        [StringLength(50)]
        public string UnloadBy { get; set; }

        public JobStatus CarrierStatus { get; set; }
        
        [StringLength(50)]
        public string Type { get; set; }
    }
}
