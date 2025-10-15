using FurntitureStoreProject.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using System.Text;
namespace FurntitureStoreProject.Data
{
    public static class UserData
    {
        public static User Login(string UserEmail, string UserPassword)
        {
            User user = new User();
            string Query = "SELECT * FROM UserStore WHERE UserEmail = \'" + UserEmail + " \' AND UserPassword = \'" + UserPassword+"\';";
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query,Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (Reader.Read())
                        {
                            user = new User()
                            {
                                UserId = Convert.ToInt32(Reader["UserId"]),
                                UserEmail = UserEmail,
                                UserFirstName = Convert.ToString(Reader["UserFirstName"]),
                                UserLastName = Convert.ToString(Reader["UserSurname"]),
                                UserPhone = Convert.ToString(Reader["UserPhoneNumber"])
                            };
                        }
                    }
                }
            }
            return user;
        }
        public static bool IsAdmin(int UserId)
        {
            string Query = "SELECT IsAdmin FROM UserStore WHERE UserId = " + UserId;
            using(SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            return Convert.ToBoolean(Reader["IsAdmin"]);
                        }
                    }
                }
            }
            return false;
        }
        public static bool EditUser(User EditUser, string Password)
        {
            string Query = "Update UserStore SET UserPassword =\'" + Password + "\', UserFirstName = \'" + EditUser.UserFirstName + "\', UserSurname = \'" + EditUser.UserLastName +
                            "\' ,UserEmail = \'" + EditUser.UserEmail + "\', UserPhoneNumber = \'" + EditUser.UserPhone + "\' WHERE UserId = " + EditUser.UserId;
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
               
                        return Command.ExecuteNonQuery()==1;
                }
            }
            return false;
        }
        public static bool Register(User NewUser, string Password)
        {
            if (!IsUser(NewUser.UserEmail))
            {
                string Query = "INSERT INTO UserStore(UserFirstName, UserSurname, UserEmail, UserPassword, UserPhoneNumber, IsAdmin)" +
                                "VALUES(\'" + NewUser.UserFirstName + "\',\'" + NewUser.UserLastName + "\',\'" + NewUser.UserEmail + "\',\'" + Password + "\',\'" + NewUser.UserPhone + "\'," +NewUser.Admin+")";
                using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
                {
                    Connection.Open();
                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        if (Command.ExecuteNonQuery() == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private static bool IsUser(string UserEmail)
        {
            string Query = "SELECT * FROM UserStore WHERE UserEmail = \'"+ UserEmail+"\' ;";
            using (SqlConnection Connection = new SqlConnection(DBConnectionString.ConnectionString))
            {
                Connection.Open();
                using(SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        return Reader.HasRows;
                    }
                }
            }
        }

    }
}
