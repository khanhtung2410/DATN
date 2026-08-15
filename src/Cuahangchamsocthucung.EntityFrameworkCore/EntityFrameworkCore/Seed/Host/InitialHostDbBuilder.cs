using Cuahangchamsocthucung.Authorization.Roles;
using System.Linq;

namespace Cuahangchamsocthucung.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly CuahangchamsocthucungDbContext _context;

        public InitialHostDbBuilder(CuahangchamsocthucungDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    
    }
}
