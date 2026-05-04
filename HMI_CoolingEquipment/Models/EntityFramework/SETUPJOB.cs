using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMI_CoolingEquipment.Models
{
    public class SETUPJOB
    {
        [Required]
        [StringLength(50)]
        public string JobID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LotNo { get; set; }

        [Required]
        [StringLength(50)]
        public string CoolingTime { get; set; }

        [Required]
        [StringLength(50)]
        public string StagingTime { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? LD_START { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? LD_END { get; set; }
        
        [Column(TypeName = "DateTime")]
        public DateTime? LD_EXPECTED_END { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? CT_START { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? CT_END { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? ST_START { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? ST_END { get; set; }
        
        [Column(TypeName = "DateTime")]
        public DateTime? ST_EXPECTED_END { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? CreatedOn { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? CompletedOn { get; set; }

        public JobStatus? JobStatus { get; set; }

        public int NoOfCarrier { get; set; }

        [Required]
        public bool IsJobError { get; set; } = false;
    }
}
