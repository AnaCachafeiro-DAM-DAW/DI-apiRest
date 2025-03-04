using apiRestEscaladores.MVVM.Modelo;
using System.Text;
using System.Text.Json;

namespace apiRestEscaladores.MVVM.ModelView
{

    // de aquí vamos a conectar el servicio con los get/update... etc
    public class EscaladorServicio
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "https://67ab6a475853dfff53d75da8.mockapi.io/api/v1/escaladores";

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true, // Ignorar mayúsculas/minúsculas
            WriteIndented = true               // Formatear JSON con indentación
        };

        // GET - Obtener todos los escaladores
        public async Task<List<Escalador>> GetEscaladoresAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(BaseUrl);
                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Respuesta JSON: {json}"); // Depuración

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<List<Escalador>>(json, _jsonOptions) ?? new List<Escalador>();
                }
                else
                {
                    Console.WriteLine($"Error en la respuesta GET: {response.StatusCode}");
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

        // Realizamos el POST para agregar el escalador
        HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl, content);

        // Verificamos la respuesta
        if (response.IsSuccessStatusCode)
        {
            // Imprimir la respuesta para depuración y verificar que MockAPI devuelve el objeto correctamente
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Respuesta del POST: {jsonResponse}");

            return true;
        }
        else
        {
            Console.WriteLine($"Error al agregar escalador: {response.StatusCode}");
            return false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al agregar escalador: {ex.Message}");
        return false;
    }
}



        // DELETE - Eliminar un escalador
        public async Task<bool> DeleteEscaladorAsync(string id)
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


        // PUT - Editar un escalador existente
        public async Task<bool> UpdateEscaladorAsync(Escalador escalador)
        {
            try
            {
                // Serializar el objeto escalador a JSON
                string json = JsonSerializer.Serialize(escalador, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Realizamos el PUT para actualizar el escalador
                HttpResponseMessage response = await _httpClient.PutAsync($"{BaseUrl}/{escalador.Id}", content);

                // Verificamos si la respuesta fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Leer la respuesta JSON de la API
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserializamos el objeto retornado para asegurarnos de que el ID ha sido asignado
                    var escaladorEditado = JsonSerializer.Deserialize<Escalador>(jsonResponse, _jsonOptions);

                    if (escaladorEditado != null && escaladorEditado.Id != null)
                    {
                        Console.WriteLine($"Escalador editado con éxito. ID: {escaladorEditado.Id}");
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine($"Error al editar escalador: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar escalador: {ex.Message}");
            }
            return false;
        }

    }
}