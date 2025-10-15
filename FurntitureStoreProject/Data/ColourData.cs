using FurntitureStoreProject.Model;
using Microsoft.Data.SqlClient;

namespace FurntitureStoreProject.Data
{
    public static class ColourData
    {
        public static List<Colour> GetAllColours(int MaterialId)
        {
            string Query = "SELECT Colour.ColourId, ColourName, ColourRGB FROM Colour INNER JOIN ColourMaterialBridge ON ColourMaterialBridge.ColourId = Colour.ColourId WHERE ColourMaterialBridge.MaterialId = " + MaterialId;
            List<Colour> ColourList = new List<Colour>();
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using(SqlCommand Command = new SqlCommand(Query,Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                            while (Reader.Read())
                            {
                                Colour ColourItem = new Colour()
                                {
                                    ColourId = Convert.ToInt32(Reader["ColourId"]),
                                    ColourName = Convert.ToString(Reader["ColourName"]),
                                    ColourRGB = Convert.ToString(Reader["ColourRGB"])
                                };
                                ColourList.Add(ColourItem);
                            }
                    }

                }
            }
            return ColourList;
        }
        public static Colour GetColour(int ColourId)
        {
            string Query = "SELECT * FROM Colour WHERE ColourId = " + ColourId;
            Colour ColourItem = new Colour();
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (Reader.Read())
                        {

                            ColourItem = new Colour()
                            {
                                ColourId = Convert.ToInt32(Reader["ColourId"]),
                                ColourName = Convert.ToString(Reader["ColourName"]),
                                ColourRGB = Convert.ToString(Reader["ColourRGB"])
                            };
                        }
                    }
                }

            }
            return ColourItem;
        }
    }
}
