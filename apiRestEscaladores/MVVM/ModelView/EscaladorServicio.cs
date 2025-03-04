using apiRestEscaladores.MVVM.Modelo; // Importamos el modelo Escalador
using System.Text;                   // Para codificar el contenido en JSON
using System.Text.Json;              // Para serializar y deserializar JSON

namespace apiRestEscaladores.MVVM.ModelView
{
    // Estos son los métodos que voy a probar en POSTMAN
    // Esta clase se encarga de realizar las operaciones CRUD con la API
    public class EscaladorServicio
    {
        // Cliente HTTP para realizar las peticiones
        private readonly HttpClient _httpClient = new HttpClient();

        // URL base de la API REST (en este caso, MockAPI)
        private const string BaseUrl = "https://67ab6a475853dfff53d75da8.mockapi.io/api/v1/escaladores";

        // Opciones de configuración para la serialización JSON
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true, // Ignorar mayúsculas/minúsculas en los nombres de las propiedades
            WriteIndented = true               // Formatear el JSON con indentación (para mejor legibilidad)
        };

        // ------------------------ MÉTODO GET ------------------------
        // Método para obtener la lista de todos los escaladores
        public async Task<List<Escalador>> GetEscaladoresAsync()
        {
            try
            {
                // Realiza una petición GET a la API
                HttpResponseMessage response = await _httpClient.GetAsync(BaseUrl);

                // Lee el contenido de la respuesta como una cadena JSON
                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Respuesta JSON: {json}"); // Para depuración

                // Verifica si la respuesta es exitosa (código 200-299)
                if (response.IsSuccessStatusCode)
                {
                    // Deserializa el JSON a una lista de objetos Escalador
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

            // En caso de error, retorna una lista vacía
            return new List<Escalador>();
        }

        // ------------------------ MÉTODO POST ------------------------
        // Método para agregar un nuevo escalador
        public async Task<bool> AddEscaladorAsync(Escalador escalador)
        {
            try
            {
                // Serializa el objeto escalador a formato JSON
                string json = JsonSerializer.Serialize(escalador, _jsonOptions);

                // Crea el contenido HTTP con el JSON serializado
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Realiza una petición POST a la API para agregar el escalador
                HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl, content);

                // Si la respuesta es exitosa (código 200-299)
                if (response.IsSuccessStatusCode)
                {
                    // Lee la respuesta como JSON (útil para depuración)
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Respuesta del POST: {jsonResponse}");
                    return true; // Indica éxito
                }
                else
                {
                    Console.WriteLine($"Error al agregar escalador: {response.StatusCode}");
                    return false; // Indica fallo
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar escalador: {ex.Message}");
                return false; // Maneja errores de excepción
            }
        }

        // ------------------------ MÉTODO DELETE ------------------------
        // Método para eliminar un escalador por su ID
        public async Task<bool> DeleteEscaladorAsync(string id)
        {
            try
            {
                // Realiza una petición DELETE a la API con el ID del escalador
                HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

                // Devuelve true si la respuesta fue exitosa (código 200-299)
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar escalador: {ex.Message}");
                return false; // Maneja errores
            }
        }

        // ------------------------ MÉTODO PUT ------------------------
        // Método para actualizar (editar) un escalador existente
        public async Task<bool> UpdateEscaladorAsync(Escalador escalador)
        {
            try
            {
                // Serializa el objeto escalador a JSON
                string json = JsonSerializer.Serialize(escalador, _jsonOptions);

                // Crea el contenido HTTP con el JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Realiza una petición PUT a la API usando el ID del escalador
                HttpResponseMessage response = await _httpClient.PutAsync($"{BaseUrl}/{escalador.Id}", content);

                // Verifica si la respuesta es exitosa (código 200-299)
                if (response.IsSuccessStatusCode)
                {
                    // Aquí lee la respuesta JSON de la API (para validar el objeto actualizado)
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserializa el JSON para confirmar que el escalador se actualizó correctamente
                    var escaladorEditado = JsonSerializer.Deserialize<Escalador>(jsonResponse, _jsonOptions);

                    // Si el objeto deserializado es válido, confirma el éxito
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
