using Microsoft.Playwright;

namespace Dokterspraktijk.Tests.Acceptance.UI.Shared.Pages.Abstract
{
    public abstract class PageObject
    {
        protected IPage Page { get; private set; }

        protected PageObject(IPage page)
        {
            Page = page;
        }

        public TPage As<TPage>() where TPage : PageObject
        {
            return (TPage)this;
        }

        public async Task RefreshAsync()
        {
            await Page.ReloadAsync();
        }

        public async Task<bool> WaitForConditionAsync(
            Func<Task<bool>> condition,
            bool waitForValue = true,
            int checkDelayMs = 100,
            int numberOfChecks = 300)
        {
            bool waarde = !waitForValue;

            for (int teller = 0; teller < numberOfChecks; teller++)
            {
                waarde = await condition();

                if (waarde == waitForValue)
                {
                    break;
                }

                await Task.Delay(checkDelayMs);
            }

            return waarde;
        }
    }
}