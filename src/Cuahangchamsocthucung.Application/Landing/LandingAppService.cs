using Abp.Application.Services;
using Abp.Domain.Repositories;
using Cuahangchamsocthucung.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Landing
{
    public class LandingAppService : ApplicationService
    {
        private readonly IRepository<Entities.DichVu, int> _dichVuRepository;
        private readonly IRepository<BangGia, int> _bangGiaRepository;

        public LandingAppService(
            IRepository<Entities.DichVu, int> dichVuRepository,
            IRepository<BangGia, int> bangGiaRepository)
        {
            _dichVuRepository = dichVuRepository;
            _bangGiaRepository = bangGiaRepository;
        }

        public async Task<List<BangGia>> GetBangGia(
            string tenDichVu)
        {
            var dichVu = await _dichVuRepository
                .GetAll()
                .FirstOrDefaultAsync(x =>
                    x.TenDichVu == tenDichVu);

            if (dichVu == null)
            {
                return new List<BangGia>();
            }

            return await _bangGiaRepository
                .GetAll()
                .Where(x => x.DichVuId == dichVu.Id)
                .OrderBy(x => x.Loaithucung)
                .ThenBy(x => x.Cannangtu)
                .ToListAsync();
        }
    }
}