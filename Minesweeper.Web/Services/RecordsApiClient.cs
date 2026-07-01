using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Minesweeper.Web.Models;

namespace Minesweeper.Web.Services
{
    public sealed class RecordsApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly HttpClient _http;

        public RecordsApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<GameRecordDto?> SubmitAsync(SubmitRecordRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/records", request, JsonOptions);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<GameRecordDto>(JsonOptions);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to submit game record: {ex.Message}");
                return null;
            }
        }

        public async Task<List<GameRecordDto>> GetAllAsync(Difficulty? difficulty = null)
        {
            try
            {
                var url = difficulty is null ? "api/records" : $"api/records?difficulty={difficulty}";
                return await _http.GetFromJsonAsync<List<GameRecordDto>>(url, JsonOptions) ?? new();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to load game records: {ex.Message}");
                return new();
            }
        }
    }
}
