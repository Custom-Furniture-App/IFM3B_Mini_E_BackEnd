using Furniture_v1.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Furniture_v1.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class NotificationsController : ControllerBase
  {
    private readonly DatabaseHelper _db;

    public NotificationsController(DatabaseHelper db)
    {
      _db = db;
    }

    public class NotificationDto
    {
      public int To { get; set; }               // UserId
      public string Title { get; set; } = "";
      public string Message { get; set; } = "";
      public string NotificationType { get; set; } = "info";
    }

    public class NotificationModel : NotificationDto
    {
      public int Id { get; set; }
      public DateTime CreatedDate { get; set; }
      public bool IsRead { get; set; }
    }


    // ✅ GET: api/notifications
    [HttpGet]
    public IActionResult GetAll()
    {
      var dt = _db.ExecuteQuery(@"
                SELECT Id, [To], Title, Message, NotificationType, CreatedDate, IsRead
                FROM Notifications
                ORDER BY CreatedDate DESC
            ");

      return Ok(dt.ToDictionaryList());
    }

    // ✅ GET: api/notifications/user/{userId}
    [HttpGet("user/{userId}")]
    public IActionResult GetByUser(int userId)
    {
      var dt = _db.ExecuteQuery(@"
                SELECT Id, [To], Title, Message, NotificationType, CreatedDate, IsRead
                FROM Notifications
                WHERE [To] = @userId
                ORDER BY CreatedDate DESC
            ", new SqlParameter("@userId", userId));

      return Ok(dt.ToDictionaryList());
    }

    // ✅ POST: api/notifications
    [HttpPost]
    public IActionResult Create([FromBody] NotificationDto dto)
    {
      if (dto.To <= 0 || string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Message))
      {
        return BadRequest(new { Message = "Invalid notification data." });
      }

      string sql = @"
                INSERT INTO Notifications ([To], Title, Message, NotificationType)
                VALUES (@To, @Title, @Message, @NotificationType);
                SELECT SCOPE_IDENTITY();
            ";

      int newId = Convert.ToInt32(_db.ExecuteScalar(sql,
          new SqlParameter("@To", dto.To),
          new SqlParameter("@Title", dto.Title),
          new SqlParameter("@Message", dto.Message),
          new SqlParameter("@NotificationType", dto.NotificationType)
      ));

      return Ok(new { Message = "Notification created successfully", Id = newId });
    }

    // ✅ Optional: Mark as read
    [HttpPut("mark-read/{id}")]
    public IActionResult MarkAsRead(int id)
    {
      int rows = _db.ExecuteNonQuery(
          "UPDATE Notifications SET IsRead = 1 WHERE Id = @id",
          new SqlParameter("@id", id)
      );

      if (rows > 0)
        return Ok(new { Message = "Notification marked as read" });

      return NotFound(new { Message = "Notification not found" });
    }
  }
}
