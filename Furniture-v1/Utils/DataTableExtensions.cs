namespace Furniture_v1.Utils
{
  using System;
  using System.Collections.Generic;
  using System.Data;

  public static class DataTableExtensions
  {
    // Convert a single DataRow to Dictionary<string, object>
    public static Dictionary<string, object> ToDictionary(this DataRow row)
    {
      var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

      // Use row.Table.Columns so we get the column names in correct order
      foreach (DataColumn col in row.Table.Columns)
      {
        var val = row[col];
        dict[col.ColumnName] = val == DBNull.Value ? null : val;
      }

      return dict;
    }

    // Convert a whole DataTable to List<Dictionary<string,object>>
    public static List<Dictionary<string, object>> ToDictionaryList(this DataTable dt)
    {
      var list = new List<Dictionary<string, object>>();

      foreach (DataRow row in dt.Rows)
      {
        list.Add(row.ToDictionary());
      }

      return list;
    }
  }
}
