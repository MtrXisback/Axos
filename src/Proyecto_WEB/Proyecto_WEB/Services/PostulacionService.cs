using Proyecto_WEB.Models;
using System.Net.Http.Json;
using Newtonsoft.Json; // Asegúrate de tener instalado Newtonsoft.Json o usa JsonSerializer

namespace Proyecto_WEB.Services
{
    public class PostulacionService
    {
        private readonly HttpClient _httpClient;

        public PostulacionService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("MyAPI");
        }

        // 1. APLICAR A UNA OFERTA
        public async Task<bool> Aplicar(Postulacion obj)
        {
            var res = await _httpClient.PostAsJsonAsync("api/Postulacion/aplicar", obj);
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return result?.Success ?? false;
            }
            return false;
        }

        // 2. LISTAR POR OFERTA (Lo usa la Empresa para ver quién postuló)
        public async Task<List<Postulacion>> ListarPorOferta(int id)
        {
            try
            {
                var res = await _httpClient.GetAsync($"api/Postulacion/oferta/{id}");
                if (res.IsSuccessStatusCode)
                {
                    var result = await res.Content.ReadFromJsonAsync<ApiResponse<List<Postulacion>>>();
                    return result?.Data ?? new List<Postulacion>();
                }
            }
            catch (Exception)
            {
                // Loguear error si es necesario
            }
            return new List<Postulacion>();
        }

        // 3. LISTAR POR USUARIO (Lo usa el Candidato para ver sus postulaciones)
        public async Task<List<Postulacion>> ListarPorUsuario(int id)
        {
            try
            {
                var res = await _httpClient.GetAsync($"api/Postulacion/usuario/{id}");
                if (res.IsSuccessStatusCode)
                {
                    // 💡 Aquí es donde desempaquetamos la "Data" de la caja ApiResponse
                    var result = await res.Content.ReadFromJsonAsync<ApiResponse<List<Postulacion>>>();
                    return result?.Data ?? new List<Postulacion>();
                }
            }
            catch (Exception)
            {
                // Error de pulso detectado, devolvemos lista limpia
            }
            return new List<Postulacion>();
        }

        // 4. CAMBIAR ESTADO (Aceptar/Rechazar)
        public async Task<bool> CambiarEstado(int id, string estado)
        {
            var payload = new
            {
                postulacionId = id,
                nuevoEstado = estado
            };

            // 💡 Enviamos el Patch
            var res = await _httpClient.PatchAsJsonAsync("api/Postulacion/cambiar-estado", payload);

            return res.IsSuccessStatusCode;
        }

        // 5. CANCELAR POSTULACIÓN (Lógica Delete)
        public async Task<bool> CancelarPostulacion(int id)
        {
            var res = await _httpClient.DeleteAsync($"api/Postulacion/cancelar/{id}");

            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return result?.Success ?? false;
            }
            return false;
        }
    }
}