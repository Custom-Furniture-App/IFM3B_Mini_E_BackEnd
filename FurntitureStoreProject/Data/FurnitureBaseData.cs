using FurntitureStoreProject.Model;
using Microsoft.Data.SqlClient;

namespace FurntitureStoreProject.Data
{
    public static class FurnitureBaseData
    {
        public static List<FurnitureBase> GetAllFurnitureByType(int FurntureTypeId)
        {
            string Query = "SELECT FurnitureBaseId, FurnitureBaseName, FurnitureBasePrice, FurnitureBaseDescription FROM FurnitureBase" +
                " INNER JOIN FurnitureType ON FurnitureBase.FurnitureTypeId = FurnitureType.FurnitureTypeId";
            List<FurnitureBase> FurnitureBaseList = new List<FurnitureBase>();
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
                                FurnitureBase Furnture = new FurnitureBase()
                                {
                                    FurnitureBaseId = Convert.ToInt32(Reader["FurnitureBaseId"]),
                                    FurnitureBaseName = Convert.ToString(Reader["FurnitureBaseName"]),
                                    FurnitureBasePrice = Convert.ToDouble(Reader["FurnitureBasePrice"]),
                                    FurnitureBaseDescription = Convert.ToString(Reader["FurnitureBaseDescription"])
                                };
                                FurnitureBaseList.Add(Furnture);
                            }
                        }
                    }
                }

            }
            return FurnitureBaseList;
        }
        public static FurnitureBase GetFurnitureItem(int BaseFurnitureId)
        {
            string Query = "SELECT FurnitureBaseId, FurnitureBaseName, FurnitureBasePrice, FurnitureBaseDescription FROM FurnitureBase WHERE FurnitureBaseId = \'" + BaseFurnitureId + "\'";
            FurnitureBase FurnitureBaseItem = new FurnitureBase();
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (Reader.Read())
                        {
                            FurnitureBaseItem = new FurnitureBase()
                            {
                                FurnitureBaseId = Convert.ToInt32(Reader["FurnitureBaseId"]),
                                FurnitureBaseName = Convert.ToString(Reader["FurnitureBaseName"]),
                                FurnitureBasePrice = Convert.ToDouble(Reader["FurnitureBasePrice"]),
                                FurnitureBaseDescription = Convert.ToString(Reader["FurnitureBaseDescription"])
                            };
                        }
                    }
                }
            }
            return FurnitureBaseItem;
        }
        public static bool AddFurnitureBaseItem(FurnitureBase FurnitureBaseItem, int FurnitureTypeId)
        {
            string Query = "INSERT INTO FurnitureBase(FurnitureBaseName, FurnitureTypeId, FurnitureBasePrice, FurnitureBaseDescription) " + 
                            "VALUES(\'"+FurnitureBaseItem.FurnitureBaseName+"\', \'"+ FurnitureTypeId+"\', \'"+ FurnitureBaseItem.FurnitureBasePrice+"\',\'"+FurnitureBaseItem.FurnitureBaseDescription+"\');";
            using(SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using(SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() == 1;
                }
            }
        }
        public static bool UpdateFurnitureBaseItem(FurnitureBase FurnitureBaseItem)
        {
            string Query = "UPDATE FurnitureBase SET FurnitureBaseName =\'"+FurnitureBaseItem.FurnitureBaseName+"\', FurnitureBasePrice = "+FurnitureBaseItem.FurnitureBasePrice+", " +
                           "FurnitureBaseDescription=\'"+FurnitureBaseItem.FurnitureBaseDescription+"\' WHERE FurnitureBaseId = "+ FurnitureBaseItem.FurnitureBaseId;
            using(SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using(SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() == 1;
                }
            }
        }
    }
}
