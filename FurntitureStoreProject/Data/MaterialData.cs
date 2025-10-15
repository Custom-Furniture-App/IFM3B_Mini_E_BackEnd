using FurntitureStoreProject.Model;
using Microsoft.Data.SqlClient;

namespace FurntitureStoreProject.Data
{
    public class MaterialData
    {
        public static List<Material> GetAllCompatibleMaterials(int FurnitureBaseId)
        {
            string Query = "SELECT MaterialId, MaterialName, MaterialPrice FROM Material " +
                "INNER JOIN FurnitureMaterialBridge ON Material.MaterialId = FurnitureMaterialBridge.MaterialId WHERE FurnitureMaterialBridge.FurnitureBaseId = " + FurnitureBaseId;
            List<Material> MaterialList = new List<Material>();
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
                                Material MaterialItem = new Material()
                                {
                                    MaterialId = Convert.ToInt32(Reader["MaterialId"]),
                                    MaterialName = Convert.ToString(Reader["MaterialName"]),
                                    MaterialPrice = Convert.ToDouble(Reader["MaterialPrice"])
                                };
                                MaterialList.Add(MaterialItem);
                            }

                        }

                    }
                }
            }
            return MaterialList;
        }
        public static Material GetMaterialItem(int MaterialId)
        {
            string Query = "SELECT * FROM Material WHERE MaterialId = " + MaterialId;
            Material MaterialItem = new Material();
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        MaterialItem = new Material()
                        {
                            MaterialId = MaterialId,
                            MaterialName = Convert.ToString(Reader["MaterialName"]),
                            MaterialPrice = Convert.ToDouble(Reader["MaterialPrice"])
                        };
                    }
                }
            }
            return MaterialItem;
        }
        public static bool AddMaterials(Material MaterialItem)
        {
            string Query = "INSERT INTO Material(MaterialName, MaterialPrice) VALUES (\'"+ MaterialItem.MaterialName +"\',"+MaterialItem.MaterialPrice+")";
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() == 1;
                }
            }

        }
        public static bool UpdateMaterial(Material MaterialItem)
        {
            string Query = "UPDATE Material SET MaterialName = \'" + MaterialItem.MaterialName +
                "\', MaterialPrice = " + MaterialItem.MaterialPrice + "WHERE MaterialId = " + MaterialItem.MaterialId;
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {

                    return Command.ExecuteNonQuery() == 1;
                }
            }
        }
        public static int GetColourMaterialId(int MaterialId, int ColourId)
        {
            string Query = "SELECT ColourMaterialBridgeId FROM ColourMaterialBridgeId WHERE MaterialId = "+ MaterialId+" AND ColourId = "+ ColourId;
            int ColourMaterialId = 0;
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using(SqlDataReader Reader = Command.ExecuteReader())
                    {
                        if(Reader.Read())
                        {
                            ColourMaterialId = Convert.ToInt32(Reader["ColourMaterialBridgeId"]);
                        }
                    }
                }
            }
            return ColourMaterialId;
        }
        public static bool AddColourMaterial(int ColourId, int MaterialId)
        {
            string Query = "INSERT INTO ColourMaterialBridge(MaterialId, ColourId) VALUES ("+MaterialId+","+ColourId+")";
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() == 1;
                }
            }
        }
    }
}
