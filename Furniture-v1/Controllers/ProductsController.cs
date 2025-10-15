using Furniture_v1.Dtos;
using Furniture_v1.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Furniture_v1.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProductsController : ControllerBase
  {
    private readonly DatabaseHelper _db;

    public ProductsController(DatabaseHelper db)
    {
      _db = db;
    }

    // POST: api/products
    [HttpPost]
    public IActionResult Create([FromBody] ProductDto dto)
    {
      var rows = _db.ExecuteNonQuery(
          @"INSERT INTO Products 
            (ProductName, Description, Category, Price, ImageUrl, Stock, CreatedDate, IsActive) 
          VALUES 
            (@name, @desc, @cat, @price, @img, @stock, GETDATE(), 1)",
          new SqlParameter("@name", dto.ProductName),
          new SqlParameter("@desc", dto.Description),
          new SqlParameter("@cat", dto.Category),
          new SqlParameter("@price", dto.Price),
          new SqlParameter("@img", dto.ImageUrl),
          new SqlParameter("@stock", dto.Stock)
      );

      if (rows > 0)
        return Ok(new { Message = "Product created successfully" });

      return BadRequest(new { Message = "Failed to create product" });
    }


    // GET: api/products
    [HttpGet]
    public IActionResult GetAll()
    {
      var dt = _db.ExecuteQuery("SELECT * FROM Products WHERE IsActive = 1");
      return Ok(dt.ToDictionaryList());
    }

    // GET: api/products/category/{category}
    [HttpGet("category/{category}")]
    public IActionResult GetByCategory(string category)
    {
      var dt = _db.ExecuteQuery(
          "SELECT * FROM Products WHERE Category = @category AND IsActive = 1",
          new SqlParameter("@category", category)
      );
      return Ok(dt.ToDictionaryList());
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public IActionResult GetSingle(int id)
    {
      var dt = _db.ExecuteQuery(
          "SELECT * FROM Products WHERE Id = @id AND IsActive = 1",
          new SqlParameter("@id", id)
      );

      if (dt.Rows.Count == 0)
        return NotFound(new { Message = "Product not found" });

      return Ok(dt.ToDictionaryList()[0]);
    }

    // PUT: api/products/{id}
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] ProductDto dto)
    {
      var rows = _db.ExecuteNonQuery(
          @"UPDATE Products SET 
                ProductName = @name, 
                Description = @desc, 
                Category = @cat, 
                Price = @price, 
                ImageUrl = @img, 
                Stock = @stock, 
                UpdatedDate = GETDATE() 
              WHERE Id = @id",
          new SqlParameter("@name", dto.ProductName),
          new SqlParameter("@desc", dto.Description),
          new SqlParameter("@cat", dto.Category),
          new SqlParameter("@price", dto.Price),
          new SqlParameter("@img", dto.ImageUrl),
          new SqlParameter("@stock", dto.Stock),
          new SqlParameter("@id", id)
      );

      if (rows > 0)
        return Ok(new { Message = "Product updated successfully" });

      return BadRequest(new { Message = "Update failed" });
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      // Soft delete
      var rows = _db.ExecuteNonQuery(
          "UPDATE Products SET IsActive = 0 WHERE Id = @id",
          new SqlParameter("@id", id)
      );

      if (rows > 0)
        return Ok(new { Message = "Product deleted successfully" });

      return NotFound(new { Message = "Product not found" });
    }
  }
}
