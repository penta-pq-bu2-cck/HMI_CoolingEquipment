using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMI_CoolingEquipment.Models
{
    public class USERACCOUNT
    {

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string UserGroup { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? LastLoginTime { get; set; }
        
        [Column(TypeName = "DateTime")]
        public DateTime? CreatedOn { get; set; }

    }
}
