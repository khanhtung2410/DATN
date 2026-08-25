using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.UI;
using Cuahangchamsocthucung.Notifications.Dto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.ThongBao
{
    [AbpAuthorize]
    public class ThongBaoAppService : ApplicationService
    {
        private readonly IRepository<UserNotificationInfo, Guid> _userNotificationRepository;
        private readonly IRepository<TenantNotificationInfo, Guid> _tenantNotificationRepository;

        public ThongBaoAppService(
            IRepository<UserNotificationInfo, Guid> userNotificationRepository,
            IRepository<TenantNotificationInfo, Guid> tenantNotificationRepository)
        {
            _userNotificationRepository = userNotificationRepository;
            _tenantNotificationRepository = tenantNotificationRepository;
        }

        public async Task<List<ThongBaoDto>> GetCuaToi()
        {
            if (!AbpSession.UserId.HasValue)
                throw new UserFriendlyException("Vui lòng đăng nhập.");

            var userId = AbpSession.UserId.Value;

            var data = await (
                from userNotification in _userNotificationRepository.GetAll()
                join tenantNotification in _tenantNotificationRepository.GetAll()
                    on userNotification.TenantNotificationId equals tenantNotification.Id
                where userNotification.UserId == userId
                orderby tenantNotification.CreationTime descending
                select new
                {
                    userNotification.Id,
                    userNotification.State,
                    tenantNotification.NotificationName,
                    tenantNotification.Data,
                    tenantNotification.CreationTime
                }).ToListAsync();

            return data.Select(x => new ThongBaoDto
            {
                Id = x.Id,
                NotificationName = x.NotificationName,
                Message = LayMessage(x.Data),
                State = (int)x.State,
                CreationTime = x.CreationTime
            }).ToList();
        }

        public async Task DanhDauDaDoc(Guid id)
        {
            if (!AbpSession.UserId.HasValue)
                throw new UserFriendlyException("Vui lòng đăng nhập.");

            var notification = await _userNotificationRepository.FirstOrDefaultAsync(id);

            if (notification == null || notification.UserId != AbpSession.UserId.Value)
                throw new UserFriendlyException("Không tìm thấy thông báo.");

            notification.State = UserNotificationState.Read;
            await _userNotificationRepository.UpdateAsync(notification);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task DanhDauTatCaDaDoc()
        {
            if (!AbpSession.UserId.HasValue)
                throw new UserFriendlyException("Vui lòng đăng nhập.");

            var userId = AbpSession.UserId.Value;

            var notifications = await _userNotificationRepository.GetAllListAsync(
                x => x.UserId == userId && x.State == UserNotificationState.Unread);

            foreach (var notification in notifications)
                notification.State = UserNotificationState.Read;

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<int> DemChuaDoc()
        {
            if (!AbpSession.UserId.HasValue)
                return 0;

            return await _userNotificationRepository.CountAsync(
                x => x.UserId == AbpSession.UserId.Value &&
                     x.State == UserNotificationState.Unread);
        }

        private string LayMessage(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return "Bạn có một thông báo mới.";

            try
            {
                var json = JObject.Parse(data);
                return json["Message"]?.ToString()
                    ?? json["Properties"]?["Message"]?.ToString()
                    ?? "Bạn có một thông báo mới.";
            }
            catch
            {
                return data;
            }
        }
    }
}