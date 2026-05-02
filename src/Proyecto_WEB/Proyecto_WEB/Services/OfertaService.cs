using Newtonsoft.Json;
using Proyecto_WEB.Models;
using System.Text;

namespace Proyecto_WEB.Services
{
    public class OfertaService
    {
        private readonly HttpClient _httpClient;
        public OfertaService(IHttpClientFactory factory) => _httpClient = factory.CreateClient("MyAPI");

        // --- GESTIÓN DE OFERTAS ---
        public async Task<List<OfertaTrabajo>> ListarFiltrado(int? espId, string? mod, decimal? sal, int pag = 1)
        {
            var url = $"api/OfertaTrabajo/buscar?especialidadId={espId}&modalidad={mod}&salarioMin={sal}&pagina={pag}";
            var response = await _httpClient.GetAsync(url);
            return response.IsSuccessStatusCode ?
                JsonConvert.DeserializeObject<List<OfertaTrabajo>>(await response.Content.ReadAsStringAsync())! : new();
        }

        public async Task<OfertaTrabajo?> ObtenerPorID(int id) =>
            await _httpClient.GetFromJsonAsync<OfertaTrabajo>($"api/OfertaTrabajo/{id}");

        public async Task<bool> Registrar(OfertaTrabajo obj)
        {
            var res = await _httpClient.PostAsJsonAsync("api/OfertaTrabajo/publicar", obj);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> Actualizar(OfertaTrabajo obj) =>
            (await _httpClient.PutAsJsonAsync("api/OfertaTrabajo/actualizar", obj)).IsSuccessStatusCode;

        // Modificamos para recibir el nuevo estado ('Activa' o 'Inactiva')
        public async Task<bool> Eliminar(int id, string nuevoEstado) =>
            (await _httpClient.DeleteAsync($"api/OfertaTrabajo/{id}?estado={nuevoEstado}")).IsSuccessStatusCode;

        // --- HABILIDADES POR OFERTA ---
        public async Task<List<Habilidad>> ListarHabilidades(int ofertaId) =>
            await _httpClient.GetFromJsonAsync<List<Habilidad>>($"api/OfertaTrabajo/habilidades/{ofertaId}") ?? new();

        public async Task<bool> AsignarHabilidades(int ofertaId, List<int> habilidadesIds)
        {
            
            var req = new
            {
                ofertaId = ofertaId,
                habilidadesIds = habilidadesIds
            };

            var response = await _httpClient.PostAsJsonAsync("api/OfertaTrabajo/habilidades/asignar", req);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarHabilidad(int ofertaId, int habilidadId) =>
            (await _httpClient.DeleteAsync($"api/OfertaTrabajo/habilidades/eliminar/{ofertaId}/{habilidadId}")).IsSuccessStatusCode;

        // Método para limpiar todas las habilidades de una oferta (Reset)
        public async Task<bool> ResetearHabilidades(int ofertaId) =>
            (await _httpClient.DeleteAsync($"api/OfertaTrabajo/habilidades/reset/{ofertaId}")).IsSuccessStatusCode;

        public async Task<List<OfertaTrabajo>> ListarPorEmpresa(int id) 
        {            
            var url = $"api/OfertaTrabajo/empresa/{id}";
            return await _httpClient.GetFromJsonAsync<List<OfertaTrabajo>>(url) ?? new();
        }

    }
}