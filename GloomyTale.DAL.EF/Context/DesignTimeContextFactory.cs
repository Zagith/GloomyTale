using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GloomyTale.DAL.EF.Context
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<OpenNosContext>
    {
        public OpenNosContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<OpenNosContext> optionsBuilder = new DbContextOptionsBuilder<OpenNosContext>();

            optionsBuilder.UseSqlServer("Server=localhost;Database=gloomytaleNewSource;User ID=sa;Password=strong_pass2018;");
            return new OpenNosContext(optionsBuilder.Options);
        }
    }
}
