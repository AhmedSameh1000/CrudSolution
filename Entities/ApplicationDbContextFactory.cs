using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationDbContextFactory :
        IDesignTimeDbContextFactory<CrudDbContext>
    {
        public CrudDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CrudDbContext>();
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-45F6T46\\SQLEXPRESS;Initial Catalog=CrudSolution;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

            return new CrudDbContext(optionsBuilder.Options);
        }
    }
}