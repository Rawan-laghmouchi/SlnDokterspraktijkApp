using Dokterspraktijk.Tests.Acceptance.UI.Shared.Pages.Abstract;
using Microsoft.Playwright;

namespace Dokterspraktijk.Tests.Acceptance.UI.Shared.Pages
{
    public class LoginPagina : PageObject
    {
        public LoginPagina(IPage page) : base(page)
        {
        }

        public async Task OpenAsync(string basisUrl)
        {
            await Page.GotoAsync($"{basisUrl}/Identity/Account/Login");
        }

        public async Task LoginAsync(string email, string wachtwoord)
        {
            await Page.FillAsync("input[name='Input.Email']", email);
            await Page.FillAsync("input[name='Input.Password']", wachtwoord);

            await Page.ClickAsync("button[type='submit']");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }
}