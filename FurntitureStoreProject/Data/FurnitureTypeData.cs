using FurntitureStoreProject.Model;
using Microsoft.Data.SqlClient;

namespace FurntitureStoreProject.Data
{
    public static class FurnitureTypeData
    {
        public static List<FurnitureType> GetAllFurnitureTypes()
        {
            string Query = "SELECT * FROM FurnitureType";
            List<FurnitureType> FurnitureTypeList = new List<FurnitureType>();
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        while (Reader.Read()) {
                            FurnitureType FurnitureTypeItem = new FurnitureType()
                            {
                                FurnitureTypeId = Convert.ToInt32(Reader["FurnitureTypeId"]),
                                FurnitureTypeName = Convert.ToString(Reader["FurnitureTypeName"])
                            };
                            FurnitureTypeList.Add(FurnitureTypeItem);
                        }
                    }

                }
            }
            return FurnitureTypeList;
        }
    }
}
