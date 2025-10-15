using FurntitureStoreProject.Model;
using Microsoft.Data.SqlClient;

namespace FurntitureStoreProject.Data
{
    public static class ComponentData
    {
        public static List<Component> GetAllCompatibleComponents(int FurnitureBaseId)
        {
            string Query = "SELECT Component.ComponentId, ComponentName, ComponentDescription FROM Component " +
                "INNER JOIN FurnitureComponentBridge ON Component.ComponentId = FurnitureComponentBridge.ComponentId WHERE FurnitureComponentBridge.FurnitureBaseId = " + FurnitureBaseId;
            List<Component> ComponentList = new List<Component>();
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
                                Component ComponentItem = new Component()
                                {
                                    ComponentId = Convert.ToInt32(Reader["ComponentId"]),
                                    ComponentName = Convert.ToString(Reader["ComponentName"]),
                                    ComponentDescription = Convert.ToString(Reader["ComponentDescription"])
                                };
                                ComponentList.Add(ComponentItem);
                            }

                        }

                    }
                }
            }
            return ComponentList;
        }

        public static Component GetComponentItem(int ComponentId)
        {
            string Query = "SELECT * FROM Component WHERE ComponentId = " + ComponentId;
            Component ComponentItem = new Component();
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (Reader.Read())
                        {
                            ComponentItem = new Component()
                            {
                                ComponentId = ComponentId,
                                ComponentName = Convert.ToString(Reader["ComponentName"]),
                                ComponentDescription = Convert.ToString(Reader["ComponentDescription"]),
                                ComponentPrice = Convert.ToDouble(Reader["ComponentPrice"])
                            };
                        }

                    }
                }
            }
            return ComponentItem;
        }

        public static bool AddComponent(Component ComponentItem)
        {
            string Query = "INSERT INTO Component(ComponentName, ComponentDescription, ComponentPrice) VALUES (\'" + ComponentItem.ComponentName + "\',\'" + ComponentItem.ComponentDescription + "\',"+ComponentItem.ComponentPrice+")";
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() == 1;
                }
            }

        }

        public static bool UpdateComponent(Component ComponentItem)
        {
            string Query = "UPDATE Component SET ComponentName = \'" + ComponentItem.ComponentName +
                "\', ComponentDescription = \'" + ComponentItem.ComponentDescription + "\', ComponentPrice = "+ ComponentItem.ComponentPrice+ " WHERE ComponentId = " + ComponentItem.ComponentId;
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    return Command.ExecuteNonQuery() == 1;
                }
            }
        }

        public static bool AddComponentToFurnitureBase(int FurnitureBaseId, int ComponentId)
        {
            string Query = "INSERT INTO FurnitureComponentBridge(FurnitureBaseId, ComponentId) VALUES ("+FurnitureBaseId+","+ComponentId+")";
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
