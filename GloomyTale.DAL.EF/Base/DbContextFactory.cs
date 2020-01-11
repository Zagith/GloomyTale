using Microsoft.EntityFrameworkCore;
using GloomyTale.DAL.EF.Helpers;

namespace GloomyTale.DAL.EF.Base
{
    public class DbContextFactory : IOpenNosContextFactory
    {
        private readonly DatabaseConfiguration _conf;

        public DbContextFactory(DatabaseConfiguration conf) => _conf = conf;

        public OpenNosContext CreateContext()
        {
            DbContextOptionsBuilder<OpenNosContext> optionsBuilder = new DbContextOptionsBuilder<OpenNosContext>();
            optionsBuilder.UseSqlServer(_conf.ToString());
            return new OpenNosContext(optionsBuilder.Options);
        }
    }
}
