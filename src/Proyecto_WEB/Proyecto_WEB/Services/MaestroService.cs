using Newtonsoft.Json;
using Proyecto_WEB.Models;
using System.Net.Http.Json;
using System.Text;

namespace Proyecto_WEB.Services
{
    public class MaestroService
    {
        private readonly HttpClient _httpClient;
        public MaestroService(IHttpClientFactory factory) => _httpClient = factory.CreateClient("MyAPI");

        // Helper para deserializar la respuesta estándar de nuestra API
        private async Task<List<T>> GetListData<T>(string url)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<T>>>(url);
            return response?.Data ?? new List<T>();
        }

        private async Task<T?> GetSingleData<T>(string url)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<T>>(url);
            return response != null ? response.Data : default;
        }

        // ==========================================
        // 1. ESPECIALIDADES
        // ==========================================
        public async Task<List<Especialidad>> ListarEspecialidades() =>
            await GetListData<Especialidad>("api/Maestro/especialidades");

        public async Task<Especialidad?> ObtenerEspecialidad(int id) =>
            await GetSingleData<Especialidad>($"api/Maestro/especialidades/{id}");

        public async Task<bool> GuardarEspecialidad(string nombre)
        {
            var nuevaEspecialidad = new Especialidad { Nombre = nombre };

            var response = await _httpClient.PostAsJsonAsync("api/Maestro/especialidades", nuevaEspecialidad);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarEspecialidad(Especialidad obj) =>
            (await _httpClient.PutAsJsonAsync("api/Maestro/especialidades", obj)).IsSuccessStatusCode;

        public async Task<bool> EliminarEspecialidad(int id) =>
            (await _httpClient.DeleteAsync($"api/Maestro/especialidades/{id}")).IsSuccessStatusCode;


        // ==========================================
        // 2. UBICACIONES
        // ==========================================
        public async Task<List<Ubicacion>> ListarUbicaciones() =>
            await GetListData<Ubicacion>("api/Maestro/ubicaciones");

        public async Task<Ubicacion?> ObtenerUbicacion(int id) =>
            await GetSingleData<Ubicacion>($"api/Maestro/ubicaciones/{id}");

        public async Task<bool> GuardarUbicacion(string ciudad, string pais)
        {
            // Usamos los nombres de propiedad que la API espera (normalmente en Mayúsculas)
            var nuevaUbicacion = new { Ciudad = ciudad, Pais = pais };
            var response = await _httpClient.PostAsJsonAsync("api/Maestro/ubicaciones", nuevaUbicacion);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarUbicacion(Ubicacion obj) =>
            (await _httpClient.PutAsJsonAsync("api/Maestro/ubicaciones", obj)).IsSuccessStatusCode;

        public async Task<bool> EliminarUbicacion(int id) =>
            (await _httpClient.DeleteAsync($"api/Maestro/ubicaciones/{id}")).IsSuccessStatusCode;


        // ==========================================
        // 3. HABILIDADES
        // ==========================================
        public async Task<List<Habilidad>> ListarHabilidades() =>
            await GetListData<Habilidad>("api/Maestro/habilidades");

        public async Task<Habilidad?> ObtenerHabilidad(int id) =>
            await GetSingleData<Habilidad>($"api/Maestro/habilidades/{id}");

        public async Task<bool> GuardarHabilidad(string nombre)
        {
            // Enviamos el objeto con "Nombre" en mayúscula para que el [FromBody] haga match
            var nuevaHabilidad = new { Nombre = nombre };
            var response = await _httpClient.PostAsJsonAsync("api/Maestro/habilidades", nuevaHabilidad);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarHabilidad(Habilidad obj) =>
            (await _httpClient.PutAsJsonAsync("api/Maestro/habilidades", obj)).IsSuccessStatusCode;

        public async Task<bool> EliminarHabilidad(int id) =>
            (await _httpClient.DeleteAsync($"api/Maestro/habilidades/{id}")).IsSuccessStatusCode;
    }

    // 💡 Clase auxiliar para mapear la respuesta Pro de la API
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int Count { get; set; }
    }
}