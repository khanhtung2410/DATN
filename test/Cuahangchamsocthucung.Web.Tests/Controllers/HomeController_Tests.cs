using Cuahangchamsocthucung.Models.TokenAuth;
using Cuahangchamsocthucung.Web.Controllers;
using Cuahangchamsocthucung.Models.TokenAuth;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Cuahangchamsocthucung.Web.Tests.Controllers
{
    public class HomeController_Tests : CuahangchamsocthucungWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}