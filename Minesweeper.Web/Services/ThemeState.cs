namespace Minesweeper.Web.Services
{
    public sealed class ThemeState
    {
        private static readonly Dictionary<string, string> AccentHexByKey = new()
        {
            ["orange"] = "#d9922c",
            ["blue"] = "#3b6fd6",
            ["green"] = "#1f9d55",
            ["red"] = "#c2453b",
        };

        public string CurrentAccentHex { get; private set; } = AccentHexByKey["orange"];

        public event Action? Changed;

        public void SetAccent(string key)
        {
            if (!AccentHexByKey.TryGetValue(key, out var hex)) return;
            CurrentAccentHex = hex;
            Changed?.Invoke();
        }
    }
}
