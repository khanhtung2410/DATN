using System.ComponentModel.DataAnnotations;

namespace Cuahangchamsocthucung.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}