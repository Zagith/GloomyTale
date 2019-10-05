// WingsEmu
// 
// Developed by NosWings Team

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OpenNos.DAL.EF.DB
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<OpenNosContext>
    {
        public OpenNosContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<OpenNosContext> optionsBuilder = new DbContextOptionsBuilder<OpenNosContext>();

            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=gloomytale;Username=postgres;Password=strong_pass2018;");
            return new OpenNosContext(optionsBuilder.Options);
        }
    }
}