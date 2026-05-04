using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMI_CoolingEquipment.Models
{
    public class JOBDETAILSHISTORY
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int No { get; set; } = 0;

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

        [StringLength(50)]
        public string Type { get; set; }

    }
}
