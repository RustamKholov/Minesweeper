using Microsoft.JSInterop;

namespace Minesweeper.Web.Services
{
    public sealed class FullscreenService
    {
        private readonly IJSRuntime _js;

        public FullscreenService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<bool> IsSupportedAsync()
        {
            try
            {
                return await _js.InvokeAsync<bool>("eval", "!!(document.documentElement.requestFullscreen)");
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsFullscreenAsync()
        {
            try
            {
                return await _js.InvokeAsync<bool>("eval", "!!document.fullscreenElement");
            }
            catch
            {
                return false;
            }
        }

        public async Task EnterAsync()
        {
            try
            {
                await _js.InvokeVoidAsync("document.documentElement.requestFullscreen");
            }
            catch
            {
                // Some browsers (notably iOS Safari) don't support the Fullscreen API for
                // arbitrary elements - the PWA "add to home screen" path is the fallback there.
            }
        }

        public async Task ExitAsync()
        {
            try
            {
                await _js.InvokeVoidAsync("document.exitFullscreen");
            }
            catch
            {
            }
        }
    }
}
