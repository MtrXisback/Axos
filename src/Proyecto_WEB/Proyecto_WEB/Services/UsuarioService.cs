using Newtonsoft.Json;
using Proyecto_WEB.Models;
using System.Net.Http.Json;
using System.Text;

namespace Proyecto_WEB.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        public UsuarioService(IHttpClientFactory factory) => _httpClient = factory.CreateClient("MyAPI");

        // 💡 Reutilizamos la estructura ApiResponse para desempaquetar
        private async Task<T?> GetSingleData<T>(string url)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<T>>(url);
            return response != null ? response.Data : default;
        }

        // 1. REGISTRAR
        public async Task<bool> Registrar(Usuario obj) =>
            (await _httpClient.PostAsJsonAsync("api/Usuario/registrar", obj)).IsSuccessStatusCode;

        // 2. LOGIN
        public async Task<Usuario?> Login(string email, string password)
        {
            // 💡 IMPORTANTE: Apuntamos al nuevo AccesoController de la API
            var res = await _httpClient.PostAsJsonAsync("api/Acceso/login", new { email, password });

            if (res.IsSuccessStatusCode)
            {
                // Leemos el contenido como string
                var jsonResponse = await res.Content.ReadAsStringAsync();

                // Desempaquetamos la caja ApiResponse<Usuario>
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<Usuario>>(jsonResponse);

                // 🛡️ Solo devolvemos la Data si el Success de la API fue true
                if (apiResponse != null && apiResponse.Success)
                {
                    return apiResponse.Data;
                }
            }

            return null; // Si llegamos aquí, algo falló (clave mal, usuario inactivo, etc.)
        }

        // 3. OBTENER POR ID
        public async Task<Usuario?> ObtenerPorId(int id) =>
            await GetSingleData<Usuario>($"api/Usuario/{id}");

        // 4. ACTUALIZAR
        public async Task<bool> Actualizar(Usuario obj) =>
            (await _httpClient.PutAsJsonAsync("api/Usuario/actualizar", obj)).IsSuccessStatusCode;

        // 5. LISTAR TODOS
        public async Task<List<Usuario>> ListarTodos(int pagina = 1)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<Usuario>>>($"api/Usuario/listar?pagina={pagina}");
            return res?.Data ?? new List<Usuario>();
        }

        // 6. ELIMINAR (Toggle Lógico)
        public async Task<bool> Eliminar(int id) =>
            (await _httpClient.DeleteAsync($"api/Usuario/eliminar/{id}")).IsSuccessStatusCode;
    }
}