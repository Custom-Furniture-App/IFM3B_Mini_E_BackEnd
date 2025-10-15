using FurntitureStoreProject.Models;
using Microsoft.Data.SqlClient;

namespace FurntitureStoreProject.Data
{
    public static class CartData
    {
        public static bool CreateCart(int UserId)
        {
            string Query = "INSERT INTO Cart(CartCreationDate, CartStatus, UserId)"+
                            "VALUES("+DateTime.Now+",\'act\',"+UserId+")";
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() == 1;
                }
            }
        }    
        public static Cart GetActiveCart(int UserId)
        {
            string Query = "SELECT * FROM Cart WHERE UserId = " + UserId + " AND CartStatus = \'act\'";
            Cart CartItem = new Cart();
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using(SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            CartItem = new Cart()
                            {
                                CartId = Convert.ToInt32(Reader["CartId"]),
                                CartCreationDate = Convert.ToDateTime(Reader["CartCreationDate"]),
                                CartStatus = Convert.ToString(Reader["CartStatus"]),
                                UserId = UserId
                            };
                            return CartItem;
                        }
                    }
                }
            }
            return CartItem;
        }
        public static Cart GetCart(int CartId)
        {
            string Query = "SELECT * FROM Cart WHERE CartId = " + CartId;
            Cart CartItem = new Cart();
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            CartItem = new Cart()
                            {
                                CartId = Convert.ToInt32(Reader["CartId"]),
                                CartCreationDate = Convert.ToDateTime(Reader["CartCreationDate"]),
                                CartStatus = Convert.ToString(Reader["CartStatus"]),
                                UserId = Convert.ToInt32(Reader["CartId"])
                            };
                            return CartItem;
                        }
                    }
                }
            }
            return CartItem;
        }
        public static List<Cart> GetAllCarts(int UserId)
        {
            string Query = "SELECT * FROM Cart WHERE UserId = " + UserId;
            List<Cart> CartList = new List<Cart>();
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                Cart CartItem = new Cart()
                                {
                                    CartId = Convert.ToInt32(Reader["CartId"]),
                                    CartCreationDate = Convert.ToDateTime(Reader["CartCreationDate"]),
                                    CartStatus = Convert.ToString(Reader["CartStatus"]),
                                    UserId = Convert.ToInt32(Reader["CartId"])
                                };
                                CartList.Add(CartItem);
                            }

                        }
                    }
                }
            }
            return CartList;
        }

    }
}
