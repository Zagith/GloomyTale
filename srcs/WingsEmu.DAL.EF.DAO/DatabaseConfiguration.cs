// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace WingsEmu.DAL.EF.DAO
{
    public class DatabaseConfiguration
    {
        public DatabaseConfiguration()
        {
            Ip = Environment.GetEnvironmentVariable("DATABASE_IP") ?? "localhost";
            Username = Environment.GetEnvironmentVariable("DATABASE_USER") ?? "postgres";
            Password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "strong_pass2018";
            Database = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "gloomytale";
            if (!ushort.TryParse(Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432", out ushort port))
            {
                port = 1433;
            }

            Port = port;
        }

        public string Ip { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public ushort Port { get; set; }

        public override string ToString() => $"Host={Ip};Port={Port};Database={Database};Username={Username};Password={Password};";
    }
}