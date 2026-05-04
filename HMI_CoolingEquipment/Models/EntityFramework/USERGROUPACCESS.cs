using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMI_CoolingEquipment.Models
{
    public class USERGROUPACCESS
    {
        [Required]
        [StringLength(50)]
        public string UserGroup { get; set; }

        [Required]
        public bool LotStatusPageAccess { get; set; }
        
        [Required]
        public bool LoadingPageAccess { get; set; }
        
        [Required]
        public bool UnloadingPageAccess { get; set; }

        [Required]
        public bool SecsGemPageAccess { get; set; }

        [Required]
        public bool SettingPageAccess { get; set; }
        
        [Required]
        public bool PTLPageAccess { get; set; }
        
        [Required]
        public bool EditUserGroupAccess { get; set; }
        
        [Required]
        public bool EditUserAccountList { get; set; }
    }
}
