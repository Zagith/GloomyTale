using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GloomyTale.DAL.EF.Context
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<OpenNosContext>
    {
        public OpenNosContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<OpenNosContext> optionsBuilder = new DbContextOptionsBuilder<OpenNosContext>();

            optionsBuilder.UseSqlServer("Server=82.165.19.227;Database=gloomytaleNewSource;User ID=GloomytaleSa;Password=strong_pass2018;");
            return new OpenNosContext(optionsBuilder.Options);
        }
    }
}
