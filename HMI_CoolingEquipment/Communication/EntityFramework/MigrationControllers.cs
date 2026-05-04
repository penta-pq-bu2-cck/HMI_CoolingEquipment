using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace HMI_CoolingEquipment.Communication
{
    [Route("api/migration")]
    [EnableCors("MyPolicy")]
    [ApiController]

    public class MigrationControllers
    {
        private readonly DB_Context _context;

        public MigrationControllers(DB_Context context)
        {
            _context = context;
        }
    }
}
