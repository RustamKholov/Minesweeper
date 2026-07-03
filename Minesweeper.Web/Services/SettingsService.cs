using System.Text.Json;
using Microsoft.JSInterop;

namespace Minesweeper.Web.Services
{
    public enum AnimSpeed { Off, Fast, Normal }
    public enum FlagGestureMode { Flick, LongPress }

    // User preferences, persisted to localStorage. A single Changed event lets every consumer
    // (board sizing, control behaviour, theme, animation timing) react live.
    public sealed class SettingsService
    {
        private const string StorageKey = "msw_settings_v1";
        private readonly IJSRuntime _js;

        public SettingsService(IJSRuntime js) => _js = js;

        // ---- Appearance ----
        private static readonly Dictionary<string, string> AccentHex = new()
        {
            ["orange"] = "#d9922c",
            ["blue"] = "#3b6fd6",
            ["green"] = "#1f9d55",
            ["red"] = "#c2453b",
            ["purple"] = "#7a5cd0",
            ["slate"] = "#5b6472",
        };
        public IReadOnlyDictionary<string, string> Accents => AccentHex;
        public string AccentKey { get; private set; } = "orange";
        public string AccentHexValue => AccentHex.TryGetValue(AccentKey, out var h) ? h : AccentHex["orange"];

        public AnimSpeed Animations { get; private set; } = AnimSpeed.Normal;
        public string AnimClass => Animations switch
        {
            AnimSpeed.Off => "anim-off",
            AnimSpeed.Fast => "anim-fast",
            _ => "anim-normal",
        };

        // ---- Board ----
        public const int MinTile = 22;
        public const int MaxTile = 64;
        public int TileSize { get; private set; } = 36;

        // ---- Controls ----
        public bool ChordEnabled { get; private set; } = true;
        public FlagGestureMode Flag { get; private set; } = FlagGestureMode.Flick;
        public bool PanInertia { get; private set; } = true;

        // ---- Custom level bounds (kept modest so no-guess generation stays fast) ----
        public const int MinDim = 5;
        public const int MaxDim = 30;

        public event Action? Changed;

        public async Task LoadAsync()
        {
            try
            {
                var json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
                if (!string.IsNullOrEmpty(json))
                {
                    var dto = JsonSerializer.Deserialize<Dto>(json);
                    if (dto is not null) Apply(dto);
                }
            }
            catch { /* corrupt/absent settings just fall back to defaults */ }
            Changed?.Invoke();
        }

        private void Apply(Dto d)
        {
            if (d.Accent is not null && AccentHex.ContainsKey(d.Accent)) AccentKey = d.Accent;
            if (d.Anim is int a && Enum.IsDefined(typeof(AnimSpeed), a)) Animations = (AnimSpeed)a;
            if (d.Tile is int t) TileSize = Math.Clamp(t, MinTile, MaxTile);
            if (d.Chord is bool c) ChordEnabled = c;
            if (d.Flag is int f && Enum.IsDefined(typeof(FlagGestureMode), f)) Flag = (FlagGestureMode)f;
            if (d.Inertia is bool i) PanInertia = i;
        }

        public void SetAccent(string key) { if (AccentHex.ContainsKey(key)) { AccentKey = key; Save(); } }
        public void SetAnimations(AnimSpeed s) { Animations = s; Save(); }
        public void SetTileSize(int px) { TileSize = Math.Clamp(px, MinTile, MaxTile); Save(); }
        public void SetChord(bool v) { ChordEnabled = v; Save(); }
        public void SetFlag(FlagGestureMode g) { Flag = g; Save(); }
        public void SetInertia(bool v) { PanInertia = v; Save(); }

        public void ResetDefaults()
        {
            AccentKey = "orange";
            Animations = AnimSpeed.Normal;
            TileSize = 36;
            ChordEnabled = true;
            Flag = FlagGestureMode.Flick;
            PanInertia = true;
            Save();
        }

        private void Save()
        {
            Changed?.Invoke();
            _ = PersistAsync();
        }

        private async Task PersistAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(new Dto
                {
                    Accent = AccentKey,
                    Anim = (int)Animations,
                    Tile = TileSize,
                    Chord = ChordEnabled,
                    Flag = (int)Flag,
                    Inertia = PanInertia,
                });
                await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
            }
            catch { /* private mode / storage full - preferences just won't persist */ }
        }

        private sealed class Dto
        {
            public string? Accent { get; set; }
            public int? Anim { get; set; }
            public int? Tile { get; set; }
            public bool? Chord { get; set; }
            public int? Flag { get; set; }
            public bool? Inertia { get; set; }
        }
    }
}
