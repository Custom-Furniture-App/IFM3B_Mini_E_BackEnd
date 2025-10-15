using FurntitureStoreProject.Data;
using FurntitureStoreProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurntitureStoreProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FurnitureTypeController : ControllerBase
    {
        [HttpGet("GetAllFurnitureTypes")]
        public ActionResult<FurnitureType> GetAllFurnitureTypes()
        {
            return Ok(FurnitureTypeData.GetAllFurnitureTypes());
        }
    }
}
