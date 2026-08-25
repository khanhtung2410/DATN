using System;

namespace Cuahangchamsocthucung.Notifications.Dto
{
    public class ThongBaoDto
    {
        public Guid Id { get; set; }
        public string NotificationName { get; set; }
        public string Message { get; set; }
        public int State { get; set; }
        public DateTime CreationTime { get; set; }
    }
}