using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMI_CoolingEquipment.Models
{
    public class EVENTLOG
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int No { get; set; } = 0;

        [Column(TypeName = "DateTime")]
        public DateTime DateTime { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string EventPage { get; set; } = string.Empty;

        public string JobID { get; set; } = string.Empty;

        public string LotID { get; set; } = string.Empty;

        public string CarrierID { get; set; } = string.Empty;

        public string LoadPortID { get; set; } = string.Empty;

        public string UserID { get; set; } = string.Empty;

        public string EventMessage { get; set; } = string.Empty;
    }
}
