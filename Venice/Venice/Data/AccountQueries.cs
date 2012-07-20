using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Venice.ConnectionFactory;

namespace Venice.Data
{
    public static class AccountQueries
    {
        #region Create/Update/Delete

        public static int CreateUser(string userName, string email, string password)
        {
            var id = 0;
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var createCommand = "INSERT INTO Users (Username,Password,Email,CreatedOn,CreatedBy,UpdatedOn, UpdatedBy) VALUES ('"
                                    + userName + "', '"
                                    + password + "', '"
                                    + email + "', '"
                                    + DateTime.Now + "', '"
                                    + userName + "', '"
                                    + DateTime.Now + "', '"
                                    + userName + "'); SELECT SCOPE_IDENTITY()";

                using (var cmd = new SqlCommand(createCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    id = Convert.ToInt32(cmd.ExecuteScalar());
                }
                conn.Close();
            }
            return id;
        }

        public static bool IsValidCredential(string username, string password)
        {
             var exists = 0;
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var selectCommand = "IF EXISTS (SELECT * FROM USERS WHERE Username = '"+ username + "' AND Password ='" + password +"') select 1 else select 0";

                using (var cmd = new SqlCommand(selectCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    exists = Convert.ToInt32(cmd.ExecuteScalar());
                }
                conn.Close();
            }
            return exists > 0;
            
        }
        #endregion
    }
}