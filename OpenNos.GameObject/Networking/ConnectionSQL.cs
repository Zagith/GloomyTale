using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Networking
{
    public class ConnectionSQL
    {
        private static string connectionString = "Server=localhost;Database=gloomytale;User Id=sa;Password=strong_pass2018;";
        private SqlConnection connection;

        public ConnectionSQL()
        {
            connection = new SqlConnection(connectionString);
        }

        public Boolean CheckDupe(long charid)
        {
            string dupeCheck = "SELECT EquipmentSerialId, count(EquipmentSerialId) as Duplicates, CharacterId FROM ItemInstance GROUP BY EquipmentSerialId , CharacterId HAVING count(EquipmentSerialId) > 1; ";
            SqlCommand command = new SqlCommand(dupeCheck, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    /*Console.WriteLine(String.Format("{0}, {1}",
                    reader["tPatCulIntPatIDPk"], reader["tPatSFirstname"]));// etc*/
                    if ((long)reader["CharacterId"] == charid)
                        return true;
                }
            }
            finally
            {
                // Always call Close when done reading.
                reader.Close();
            }
            return false;
        }
    }
}
