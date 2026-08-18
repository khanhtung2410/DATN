using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Net.Sms
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string phoneNumber, string message);
    }
}