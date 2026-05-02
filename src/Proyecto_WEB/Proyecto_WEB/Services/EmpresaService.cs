using Proyecto_WEB.Models;
using System.Net.Http.Json;

namespace Proyecto_WEB.Services
{
    public class EmpresaService
    {
        private readonly HttpClient _httpClient;
        public EmpresaService(IHttpClientFactory factory) => _httpClient = factory.CreateClient("MyAPI");

        public async Task<bool> RegistrarNuevoPartner(RegistroEmpresaVM modelo)
        {
            // Enviamos el ViewModel completo a un endpoint especial en la API
            var response = await _httpClient.PostAsJsonAsync("api/Empresa/registrar-completo", modelo);
            return response.IsSuccessStatusCode;
        }

        // 💡 El método Registrar antiguo lo dejamos por si acaso, 
        // pero el de arriba es el que usaremos en la vista.
        public async Task<bool> Registrar(Usuario empresa) =>
            (await _httpClient.PostAsJsonAsync("api/Empresa/registrar", empresa)).IsSuccessStatusCode;

        // 2. BUSCAR POR ID (Ajustado para abrir la caja ApiResponse)
        public async Task<Empresa?> BuscarPorId(int id)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<Empresa>>($"api/Empresa/perfil/{id}");
            return res?.Data;
        }

        // 3. OBTENER PERFIL (Por UsuarioID)
        public async Task<Empresa?> ObtenerPerfil(int usuarioId)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<Empresa>>($"api/Empresa/perfil/{usuarioId}");
            return res?.Data;
        }

        // 4. OBTENER POR EMPRESA ID
        public async Task<Empresa?> ObtenerPorEmpresa(int id)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<Empresa>>($"api/Empresa/{id}");
            return res?.Data;
        }

        // 5. LISTAR EMPRESAS (💡 El que causaba el JsonException)
        public async Task<List<Empresa>> ListarEmpresas()
        {
            // Leemos ApiResponse<List<Empresa>> en lugar de List<Empresa> directamente
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<Empresa>>>("api/Empresa/listar");
            return res?.Data ?? new List<Empresa>();
        }

        // 6. ACTUALIZAR
        public async Task<bool> Actualizar(Empresa empresa) =>
            (await _httpClient.PutAsJsonAsync("api/Empresa/actualizar", empresa)).IsSuccessStatusCode;

        // 7. ELIMINAR (Toggle Lógico)
        public async Task<bool> Eliminar(int id) =>
            (await _httpClient.DeleteAsync($"api/Empresa/eliminar/{id}")).IsSuccessStatusCode;
    }
}