using apiRestEscaladores.MVVM.Modelo;
using System.Text;
using System.Text.Json;

namespace apiRestEscaladores.MVVM.ModelView
{

    // de aqu� vamos a conectar el servicio con los get/update... etc
    public class EscaladorServicio
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "https://67ab6a475853dfff53d75da8.mockapi.io/api/v1/escaladores";

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        // GET - Obtener todos los escaladores
        public async Task<List<Escalador>> GetEscaladoresAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Escalador>>(json, _jsonOptions) ?? new List<Escalador>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener escaladores: {ex.Message}");
            }
            return new List<Escalador>();
        }

        // POST - Agregar un nuevo escalador
        public async Task<bool> AddEscaladorAsync(Escalador escalador)
        {
            try
            {
                string json = JsonSerializer.Serialize(escalador, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar escalador: {ex.Message}");
                return false;
            }
        }

        // PUT - Editar un escalador existente
        public async Task<bool> UpdateEscaladorAsync(Escalador escalador)
        {
            try
            {
                string json = JsonSerializer.Serialize(escalador, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync($"{BaseUrl}/{escalador.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar escalador: {ex.Message}");
                return false;
            }
        }

        // DELETE - Eliminar un escalador
        public async Task<bool> DeleteEscaladorAsync(string id)  // Cambiado a string
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar escalador: {ex.Message}");
                return false;
            }
        }
    }
}
