using Furniture_v1.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Furniture_v1.Controllers
{
  // DTOs
  public class OrderItemDto
  {
    public int? ProductId { get; set; }
    public int? ComponentId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ItemType { get; set; } = "component";
  }

  public class OrderCreateDto
  {
    public int CustomerId { get; set; }
    public string? Status { get; set; } = "pending";
    public string? FulfillmentType { get; set; } = "collection";
    public List<OrderItemDto> Items { get; set; } = new();
  }

  [ApiController]
  [Route("api/[controller]")]
  public class OrdersController : ControllerBase
  {
    private readonly DatabaseHelper _db;

    public OrdersController(DatabaseHelper db)
    {
      _db = db;
    }

    // 🛠️ FIX APPLIED: Removed the trailing semicolon (;) from the SQL string.
    private const string OrderItemsSelectSql = @"
SELECT
    oi.OrderId,
    oi.Id AS OrderItemId,
    oi.ItemId,
    oi.Quantity,
    oi.UnitPrice,
    oi.ItemType,
    (oi.Quantity * oi.UnitPrice) AS Subtotal,

    -- Item Name Subquery
    CASE oi.ItemType
        WHEN 'product' THEN (SELECT p.ProductName FROM Products p WHERE p.Id = oi.ItemId)
        WHEN 'component' THEN (SELECT c.Name FROM Components c WHERE c.Id = oi.ItemId)
        ELSE NULL
    END AS ItemName,

    -- Item Category Subquery
    CASE oi.ItemType
        WHEN 'product' THEN (SELECT p.Category FROM Products p WHERE p.Id = oi.ItemId)
        WHEN 'component' THEN (SELECT c.Category FROM Components c WHERE c.Id = oi.ItemId)
        ELSE NULL
    END AS ItemCategory,

    -- Image URL Subquery
    CASE oi.ItemType
        WHEN 'product' THEN (SELECT p.ImageUrl FROM Products p WHERE p.Id = oi.ItemId)
        WHEN 'component' THEN (SELECT c.ImageUrl FROM Components c WHERE c.Id = oi.ItemId)
        ELSE NULL
    END AS ImageUrl

FROM
    OrderItems oi
"; // <-- The semicolon has been removed here.

    // ✅ GET: api/orders
    [HttpGet]
    public IActionResult GetAll()
    {
      var ordersDt = _db.ExecuteQuery(@"
                SELECT 
                    o.Id, o.OrderNumber, o.CustomerId,
                    u.FullName AS CustomerName, u.Email, u.Phone, u.Address,
                    o.Status, o.TotalAmount, o.CreatedAt, o.UpdatedAt, o.CompletedAt, o.FulfillmentType
                FROM Orders o
                JOIN Users u ON o.CustomerId = u.Id
                ORDER BY o.Id DESC
            ");

      if (ordersDt.Rows.Count == 0)
        return Ok(new List<object>());

      var orders = ordersDt.ToDictionaryList();
      var itemsDt = _db.ExecuteQuery(OrderItemsSelectSql);

      // Group items by OrderId
      var itemsByOrder = new Dictionary<int, List<Dictionary<string, object>>>();
      foreach (DataRow row in itemsDt.Rows)
      {
        var itemDict = row.ToDictionary();
        int orderId = Convert.ToInt32(itemDict["OrderId"]);

        if (!itemsByOrder.TryGetValue(orderId, out var list))
        {
          list = new List<Dictionary<string, object>>();
          itemsByOrder[orderId] = list;
        }
        list.Add(itemDict);
      }

      foreach (var order in orders)
      {
        int orderId = Convert.ToInt32(order["Id"]);
        order["Items"] = itemsByOrder.ContainsKey(orderId)
            ? itemsByOrder[orderId]
            : new List<object>();
      }

      return Ok(orders);
    }

    // ✅ GET: api/orders/user/{userId}
    [HttpGet("user/{userId}")]
    public IActionResult GetOrdersByUser(int userId)
    {
      var ordersDt = _db.ExecuteQuery(@"
                SELECT 
                    o.Id, o.OrderNumber, o.CustomerId,
                    u.FullName AS CustomerName, u.Email, u.Phone, u.Address,
                    o.Status, o.TotalAmount,
                    o.CreatedAt, o.UpdatedAt, o.CompletedAt, o.FulfillmentType
                FROM Orders o
                JOIN Users u ON o.CustomerId = u.Id
                WHERE o.CustomerId = @id
                ORDER BY o.Id DESC",
          new SqlParameter("@id", userId)
      );

      if (ordersDt.Rows.Count == 0)
        return NotFound(new { Message = "No orders found for this user." });

      var orders = ordersDt.ToDictionaryList();
      var orderIds = string.Join(",", orders.Select(o => o["Id"]));

      if (string.IsNullOrWhiteSpace(orderIds))
        return Ok(orders);

      var itemsDt = _db.ExecuteQuery($@"
                {OrderItemsSelectSql}
                WHERE oi.OrderId IN ({orderIds})
            ");

      var itemsByOrder = new Dictionary<int, List<Dictionary<string, object>>>();
      foreach (DataRow row in itemsDt.Rows)
      {
        var itemDict = row.ToDictionary();
        int orderId = Convert.ToInt32(itemDict["OrderId"]);

        if (!itemsByOrder.TryGetValue(orderId, out var list))
        {
          list = new List<Dictionary<string, object>>();
          itemsByOrder[orderId] = list;
        }
        list.Add(itemDict);
      }

      foreach (var order in orders)
      {
        int orderId = Convert.ToInt32(order["Id"]);
        order["Items"] = itemsByOrder.ContainsKey(orderId)
            ? itemsByOrder[orderId]
            : new List<Dictionary<string, object>>();
      }

      return Ok(orders);
    }

    // ✅ GET: api/orders/{id}
    [HttpGet("{id}")]
    public IActionResult GetSingle(int id)
    {
      var orderDt = _db.ExecuteQuery(@"
                SELECT 
                    o.Id, o.OrderNumber, o.CustomerId,
                    u.FullName AS CustomerName, u.Email, u.Phone, u.Address,
                    o.Status, o.TotalAmount,
                    o.CreatedAt, o.UpdatedAt, o.CompletedAt, o.FulfillmentType
                FROM Orders o
                JOIN Users u ON o.CustomerId = u.Id
                WHERE o.Id = @id",
          new SqlParameter("@id", id)
      );

      if (orderDt.Rows.Count == 0)
        return NotFound(new { Message = "Order not found." });

      var order = orderDt.Rows[0].ToDictionary();

      var itemsDt = _db.ExecuteQuery($@"
                {OrderItemsSelectSql}
                WHERE oi.OrderId = @orderId",
          new SqlParameter("@orderId", id)
      );

      order["Items"] = itemsDt.ToDictionaryList();
      return Ok(order);
    }

    // ✅ POST: api/orders
    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderCreateDto order)
    {
      if (order == null || order.Items.Count == 0)
        return BadRequest(new { Message = "Order must have at least one item." });

      var validTypes = new[] { "component", "product" };

      // Check for valid ItemType and ensure at least one ID is provided
      foreach (var item in order.Items)
      {
        if (!validTypes.Contains(item.ItemType.ToLower().Trim()))
        {
          return BadRequest(new { Message = $"Invalid ItemType '{item.ItemType}'. Must be 'component' or 'product'." });
        }

        // Validate that the correct ID field is present based on ItemType
        bool isProduct = item.ItemType.ToLower() == "product";
        bool isComponent = item.ItemType.ToLower() == "component";

        if ((isProduct && item.ProductId == null) || (isComponent && item.ComponentId == null))
        {
          return BadRequest(new { Message = $"Missing required ID for ItemType '{item.ItemType}'." });
        }
      }

      decimal totalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

      // ✅ Add R50 delivery fee if fulfillment type is "delivery"
      if (!string.IsNullOrEmpty(order.FulfillmentType) &&
          order.FulfillmentType.Equals("delivery", StringComparison.OrdinalIgnoreCase))
      {
        totalAmount += 50;
      }

      string orderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

      string insertOrderSql = @"
    INSERT INTO Orders (OrderNumber, CustomerId, Status, TotalAmount, FulfillmentType)
    OUTPUT INSERTED.Id
    VALUES (@OrderNumber, @CustomerId, @Status, @TotalAmount, @FulfillmentType)";

      int orderId = Convert.ToInt32(_db.ExecuteScalar(
          insertOrderSql,
          new SqlParameter("@OrderNumber", orderNumber),
          new SqlParameter("@CustomerId", order.CustomerId),
          new SqlParameter("@Status", order.Status ?? "pending"),
          new SqlParameter("@TotalAmount", totalAmount),
          new SqlParameter("@FulfillmentType", order.FulfillmentType ?? "collection")
      ));


      string insertItemSql = @"
                INSERT INTO OrderItems (OrderId, ItemId, Quantity, UnitPrice, ItemType)
                VALUES (@OrderId, @ItemId, @Quantity, @UnitPrice, @ItemType)";

      foreach (var item in order.Items)
      {
        // Determine the actual ItemId based on the ItemType property
        int itemIdToInsert = 0;
        if (item.ItemType.ToLower() == "product" && item.ProductId.HasValue)
        {
          itemIdToInsert = item.ProductId.Value;
        }
        else if (item.ItemType.ToLower() == "component" && item.ComponentId.HasValue)
        {
          itemIdToInsert = item.ComponentId.Value;
        }

        _db.ExecuteNonQuery(insertItemSql,
            new SqlParameter("@OrderId", orderId),
            new SqlParameter("@ItemId", itemIdToInsert),
            new SqlParameter("@Quantity", item.Quantity),
            new SqlParameter("@UnitPrice", item.UnitPrice),
            new SqlParameter("@ItemType", item.ItemType.ToLower().Trim())
        );
      }

      return Ok(new
      {
        Message = $"Order created successfully. Total items: {order.Items.Count}",
        OrderId = orderId,
        OrderNumber = orderNumber,
        TotalAmount = totalAmount
      });
    }

    // ✅ PUT: api/orders/{id}/status
    [HttpPut("{id}/status")]
    public IActionResult UpdateStatus(int id, [FromBody] string newStatus)
    {
      if (string.IsNullOrWhiteSpace(newStatus))
        return BadRequest(new { Message = "Status cannot be empty." });

      var rows = _db.ExecuteNonQuery(
          "UPDATE Orders SET Status = @status WHERE Id = @id",
          new SqlParameter("@status", newStatus),
          new SqlParameter("@id", id)
      );

      if (rows > 0)
        return Ok(new { Message = $"Order status updated to '{newStatus}'." });

      return NotFound(new { Message = "Order not found." });
    }

    // ✅ DELETE: api/orders/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(int id)
    {
      // Note: If the database is not configured with ON DELETE CASCADE, 
      // you must delete OrderItems first. Assuming it is, but if not, 
      // the Delete All endpoint shows the safe way to do it.
      var rows = _db.ExecuteNonQuery(
          "DELETE FROM Orders WHERE Id = @id",
          new SqlParameter("@id", id)
      );

      if (rows > 0)
        return Ok(new { Message = "Order deleted successfully." });

      return NotFound(new { Message = "Order not found." });
    }

    // 🛑 NEW: DELETE ALL ORDERS ENDPOINT
    [HttpDelete("all")]
    public IActionResult DeleteAllOrders()
    {
      try
      {
        // Step 1: Delete all associated OrderItems first due to Foreign Key constraint
        int itemsDeleted = _db.ExecuteNonQuery("DELETE FROM OrderItems");

        // Step 2: Delete all Orders
        int ordersDeleted = _db.ExecuteNonQuery("DELETE FROM Orders");

        return Ok(new
        {
          Message = "All orders deleted successfully.",
          OrdersDeleted = ordersDeleted,
          ItemsDeleted = itemsDeleted
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new
        {
          Message = "An error occurred while deleting all orders. Check database constraints.",
          Error = ex.Message
        });
      }
    }

    // ✅ PUT: api/orders/change-status/{orderId}
    private readonly string[] AllowedStatuses = new[]
    {
                "assembling", "done-assembling", "ready-for-delivery",
                "courier-on-the-way", "ready-for-collection", "completed"
        };

    [HttpPut("change-status/{orderId}")]
    public IActionResult ChangeStatus(int orderId, [FromBody] string newStatus)
    {
      if (string.IsNullOrWhiteSpace(newStatus))
        return BadRequest(new { Message = "Status is required." });

      newStatus = newStatus.ToLower().Trim();

      if (!AllowedStatuses.Contains(newStatus))
        return BadRequest(new { Message = $"Invalid status. Allowed: {string.Join(", ", AllowedStatuses)}" });

      var dtOrder = _db.ExecuteQuery(
          "SELECT CustomerId FROM Orders WHERE Id = @OrderId",
          new SqlParameter("@OrderId", orderId)
      );

      if (dtOrder.Rows.Count == 0)
        return NotFound(new { Message = "Order not found." });

      int customerId = Convert.ToInt32(dtOrder.Rows[0]["CustomerId"]);

      int rows = _db.ExecuteNonQuery(@"
                UPDATE Orders
                SET Status = @Status,
                    UpdatedAt = GETDATE(),
                    CompletedAt = CASE WHEN @Status = 'completed' THEN GETDATE() ELSE CompletedAt END
                WHERE Id = @OrderId",
          new SqlParameter("@Status", newStatus),
          new SqlParameter("@OrderId", orderId)
      );

      if (rows == 0)
        return BadRequest(new { Message = "Failed to update order status." });

      string title = "Order Status Updated";
      string message = newStatus switch
      {
        "assembling" => "Your order is now being assembled.",
        "done-assembling" => "Assembly of your order is completed.",
        "ready-for-delivery" => "Your order is ready for delivery.",
        "courier-on-the-way" => "Your order is on the way via courier.",
        "ready-for-collection" => "Your order is ready for collection.",
        "completed" => "Your order has been completed. Thank you!",
        _ => $"Your order status is now: {newStatus}"
      };

      _db.ExecuteNonQuery(@"
                INSERT INTO Notifications ([To], Title, Message, NotificationType)
                VALUES (@To, @Title, @Message, @Type)",
          new SqlParameter("@To", customerId),
          new SqlParameter("@Title", title),
          new SqlParameter("@Message", message),
          new SqlParameter("@Type", "order")
      );

      return Ok(new { Message = "Order status updated and notification sent.", NewStatus = newStatus });
    }
  }
}