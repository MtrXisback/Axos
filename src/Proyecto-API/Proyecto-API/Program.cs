using Proyecto_API.DAO;
using Proyecto_API.Helpers;
using Proyecto_API.Services; // ¡Importante para los Services!

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// 1. CONFIGURACIÓN DE SERVICIOS (Contenedor de Dependencias)
// =========================================================

// Configuración de CORS: Permite que el Front-end se comunique con el API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Agregar controladores al sistema
builder.Services.AddControllers();

// Configuración de Swagger para pruebas de Endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------------------------------------------------
// INYECCIÓN DE DEPENDENCIAS
// ---------------------------------------------------------

// Helper de Conexión (Singleton: Una sola instancia para toda la App)
builder.Services.AddSingleton<Conexion>();

// Registro de DAOs (Transient: Una instancia por cada petición HTTP)
builder.Services.AddTransient<UsuarioDAO>();
builder.Services.AddTransient<EmpresaDAO>();
builder.Services.AddTransient<OfertaTrabajoDAO>();
builder.Services.AddTransient<PostulacionDAO>();
builder.Services.AddTransient<MaestroDAO>();

// Registro de Services (Transient: Aquí es donde vive la lógica de negocio)
builder.Services.AddTransient<UsuarioService>();
builder.Services.AddTransient<EmpresaService>();
builder.Services.AddTransient<OfertaTrabajoService>();
builder.Services.AddTransient<PostulacionService>();
builder.Services.AddTransient<MaestroService>();


// =========================================================
// 2. CONFIGURACIÓN DEL PIPELINE (Middleware)
// =========================================================

var app = builder.Build();

// Configuración para el entorno de desarrollo (Swagger)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activar la política de CORS definida arriba
app.UseCors("AllowAll");

// Middleware de Autorización (se usará más adelante con JWT)
app.UseAuthorization();

// Mapeo de las rutas de los controladores
app.MapControllers();

// ¡Arrancamos el motor!
app.Run();