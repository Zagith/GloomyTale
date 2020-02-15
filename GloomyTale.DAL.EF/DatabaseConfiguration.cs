using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.DAL.EF
{
    public class DatabaseConfiguration
    {
        public DatabaseConfiguration()
        {
            Ip = Environment.GetEnvironmentVariable("DATABASE_IP") ?? "82.165.19.227";
            Username = Environment.GetEnvironmentVariable("DATABASE_USER") ?? "GloomytaleSa";
            Password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "strong_pass2018";
            Database = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "gloomytaleNewSource";
            if (!ushort.TryParse(Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "1433", out ushort port))
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

        public override string ToString() => $"Server={Ip},{Port};User id={Username};Password={Password};Initial Catalog={Database};";
    }
}
