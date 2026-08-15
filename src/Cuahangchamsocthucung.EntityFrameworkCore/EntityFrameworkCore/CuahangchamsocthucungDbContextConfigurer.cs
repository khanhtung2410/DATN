using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Cuahangchamsocthucung.EntityFrameworkCore
{
    public static class CuahangchamsocthucungDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<CuahangchamsocthucungDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<CuahangchamsocthucungDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
