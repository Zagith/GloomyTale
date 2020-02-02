using System.Data.Entity.Migrations;
using OpenNos.DAL.EF.Context;

namespace OpenNos.DAL.EF.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<OpenNosContext>
    {
        #region Instantiation

        public Configuration() => AutomaticMigrationsEnabled = true;

        #endregion
    }
}