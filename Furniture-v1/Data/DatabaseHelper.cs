using Microsoft.Data.SqlClient;
using System.Data;

public class DatabaseHelper
{
  private readonly string _connectionString;

  public DatabaseHelper(string connectionString)
  {
    _connectionString = connectionString;
  }

  /// <summary>
  /// Execute a SELECT query and return results as DataTable
  /// </summary>
  public DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
  {
    using SqlConnection conn = new SqlConnection(_connectionString);
    using SqlCommand cmd = new SqlCommand(sql, conn);

    if (parameters != null && parameters.Length > 0)
      cmd.Parameters.AddRange(parameters);

    using SqlDataAdapter adapter = new SqlDataAdapter(cmd);
    DataTable dt = new DataTable();
    adapter.Fill(dt);

    return dt;
  }

  /// <summary>
  /// Execute INSERT, UPDATE, DELETE and return number of affected rows
  /// </summary>
  public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
  {
    using SqlConnection conn = new SqlConnection(_connectionString);
    using SqlCommand cmd = new SqlCommand(sql, conn);

    if (parameters != null && parameters.Length > 0)
      cmd.Parameters.AddRange(parameters);

    conn.Open();
    return cmd.ExecuteNonQuery();
  }

  /// <summary>
  /// Execute a scalar query (e.g., COUNT, SUM) and return the result
  /// </summary>
  public object ExecuteScalar(string sql, params SqlParameter[] parameters)
  {
    using SqlConnection conn = new SqlConnection(_connectionString);
    using SqlCommand cmd = new SqlCommand(sql, conn);

    if (parameters != null && parameters.Length > 0)
      cmd.Parameters.AddRange(parameters);

    conn.Open();
    return cmd.ExecuteScalar()!;
  }
}
