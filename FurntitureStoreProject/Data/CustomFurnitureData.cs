using FurntitureStoreProject.Model;
using Microsoft.Data.SqlClient;
using System.ComponentModel;

namespace FurntitureStoreProject.Data
{
    public static class CustomFurnitureData
    {
        public static List<CustomFurniture> GetCustomFurnituresWithCartId(int CartId)
        {
            string Query = "SELECT CustomFurnitureId, CustomFurnitureQuantity, CustomFurnitureTotalCost, FurnitureColourBridgeId FROM CustomFurniture WHERE CartId = " + CartId;
            List<CustomFurniture> CustomFurnitureList = new List<CustomFurniture>();
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                    
                            while(Reader.Read())
                            {
                                CustomFurniture CustomFurnitureItem = new CustomFurniture()
                                {
                                    CustomFurnitureId = Convert.ToInt32(Reader["CustomFurnitureId"]),
                                    CartId = CartId,
                                    CustomFurniturePrice = Convert.ToDouble(Reader["CustomFurnitureTotalCost"]),
                                    CustomFurnitureQuantity = Convert.ToInt32(Reader["CustomFurnitureQuantity"]),
                                    MaterialColourId = Convert.ToInt32(Reader["ColourMaterialBridgeId"])
                                };
                                CustomFurnitureList.Add(CustomFurnitureItem);
                            }
                    }

                }
            }
            return CustomFurnitureList;


        }
        public static bool AddCustomFurniture(CustomFurniture CustomFurnitureItem)
        {
            string Query = "INSERT INTO CustomFurniture(CustomFurnitureQuantitu, CustomFurnitureTotalCost, CartId, ColourMaterialBridgeId, FurnitureBaseId) VALUES(" + CustomFurnitureItem.CustomFurnitureQuantity + "," + CustomFurnitureItem.CustomFurniturePrice + ","+CustomFurnitureItem.MaterialColourId+","+CustomFurnitureItem.FurnitureBaseId+")";
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() == 1;
                }
            }
        }
        public static bool UpdateCustomFurnitureQuantity(int CustomFurnitureId, int CustomFurnitureQuantity)
        {
            string Query = "UPDATE CustomFurniture SET CustomFurnitureQuantity = " +CustomFurnitureQuantity+" WHERE CustomFurnitureId = " + CustomFurnitureId;
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using(SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() == 1;

                }
            }
        }
        public static bool DeleteCustomFurniture(int CustomFurnitureId)
        {
            string Query = "DELETE FROM CustomFurniture WHERE CustomFurnitureId = " + CustomFurnitureId;
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() == 1;
                }
            }
        }
        public static bool DeleteCustomFurnitureDesign(int CustomFurnitureDesignId)
        {
            string Query = "DELETE CustomFurnitureDesign WHERE CustomFurnitureBridgeId = " + CustomFurnitureDesignId;
            using(SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using(SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery()==1;
                }
            }
        }
        public static bool DeleteCustomFurnitureDesignCustomId(int CustomFurnitureId)
        {
            string Query = "DELETE CustomFurnitureDesign WHERE CustomFurnitureId = " + CustomFurnitureId;
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() > 0;
                }
            }
        }
        public static bool AddCustomFurnitureComponent(int ComponentId, int CustomFurnitureId)
        {
            string Query = "INSERT INTO CustomFurnitureDesign(ComponentId, CustomFurnitureId) VALUES("+ComponentId +","+ CustomFurnitureId+")";
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                using (SqlCommand Command = new SqlCommand(Query,Connection))
                {
                    return Command.ExecuteNonQuery() == 1;
                }
            }
        }
        public static double GetCustomFurnitureComponentTotalPrice(int CustomFurnitureId)
        {
            string Query = "SELECT SUM(Component.ComponentPrice) AS Total_Sum FROM CustomFurnitureDesign " +
                           "INNER JOIN Component ON Component.ComponentId = CustomerFurnitureDesign.ComponentId WHERE CustomFurnitureDesign.CustomFurnitureDesignId = " + CustomFurnitureId;
            double TotalCost = 0;
            using(SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using(SqlCommand Command = new SqlCommand(Query,Connection))
                {
                    using(SqlDataReader Reader = Command.ExecuteReader())
                    {
                        if(Reader.Read())
                        {
                            TotalCost = Convert.ToDouble(Reader["Total_Sum"]);
                        }
                    }
                }
            }
            return TotalCost;
        }
    }
}
