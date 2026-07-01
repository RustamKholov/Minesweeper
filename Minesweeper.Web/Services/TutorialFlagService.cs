using Microsoft.JSInterop;

namespace Minesweeper.Web.Services
{
    public sealed class TutorialFlagService
    {
        private const string StorageKey = "msw_seen_tutorial_v1";
        private readonly IJSRuntime _js;

        public TutorialFlagService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<bool> HasSeenTutorialAsync()
            => await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey) is not null;

        public async Task MarkSeenAsync()
            => await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, "1");
    }
}
