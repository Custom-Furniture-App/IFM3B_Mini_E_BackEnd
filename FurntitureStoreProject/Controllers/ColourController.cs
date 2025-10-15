using FurntitureStoreProject.Data;
using FurntitureStoreProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurntitureStoreProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColourController : ControllerBase
    {

        [HttpGet("GetAllColours/{MaterialId}")]
        public ActionResult<List<Colour>> GetAllColour(int MaterialId)
        {
            return Ok(ColourData.GetAllColours(MaterialId));
        }

        [HttpGet("GetColour/{ColourId}")]
        public ActionResult<Colour> GetColour(int ColourId)
        {
            return Ok(ColourData.GetColour(ColourId));  
        }
    }
}
