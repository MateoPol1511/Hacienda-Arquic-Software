using Bib_Hacienda.Aplicacion;
using Bib_Hacienda.Dominio;
using Bib_Hacienda.Dominio.Eventos;
using Bib_Hacienda.Dominio.Validacion;
using Bib_Hacienda.Infraestructura;

namespace p_mvcHacienda
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // --- Configuración de Autenticación por Cookies ---
            builder.Services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.Cookie.Name = "HaciendaSoft.Auth";
                    options.LoginPath = "/Account/Login"; // Página de login
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Duración de la sesión
                });

            // Agregar HttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            // ==================================================================
            // COMPOSITION ROOT (3C-2B)
            // Arquitectura respetada: Controller -> Servicio de Aplicación ->
            // Interfaz -> Infraestructura (ver UML TO-BE). Todo se registra
            // como Scoped (una instancia por request HTTP):
            // - Los repositorios *Texto leen/escriben archivo en cada llamada,
            //   sin estado en memoria que deba sobrevivir entre requests (a
            //   diferencia del AS-IS, que cargaba todo en un "Hacienda"
            //   Singleton al arrancar).
            // - Los Publisher* (PublisherPesoMin, PublisherVacunaVencida, etc.)
            //   acumulan suscriptores en cada llamada a sus métodos
            //   (evt_x += ...). Si se registraran como Singleton, esas
            //   suscripciones se acumularían entre requests y duplicarían
            //   mensajes; Scoped evita ese problema.
            // ==================================================================

            // --- Infraestructura: serializadores de texto ---
            // ISerializador<T> se mantiene como ÚNICA interfaz genérica
            // (no se crean cinco interfaces distintas).
            builder.Services.AddScoped<ISerializador<Potrero>, SerializadorPotrero>();
            builder.Services.AddScoped<ISerializador<Res>, SerializadorRes>();
            builder.Services.AddScoped<ISerializador<Usuario>, SerializadorUsuario>();
            builder.Services.AddScoped<ISerializador<Vacuna>, SerializadorVacuna>();
            builder.Services.AddScoped<ISerializador<Venta>, SerializadorVenta>();
            // SC-3: serializador de eventos de historia clínica (HistoriaClinica.txt).
            builder.Services.AddScoped<ISerializador<EventoClinico>, SerializadorEventoClinico>();

            // --- Dominio: validadores (ISP, uno por entidad) ---
            builder.Services.AddScoped<IValidadorPotrero, ValidadorPotrero>();
            builder.Services.AddScoped<IValidadorRes, ValidadorRes>();
            builder.Services.AddScoped<IValidadorVacuna, ValidadorVacuna>();
            builder.Services.AddScoped<IValidadorVenta, ValidadorVenta>();
            // SC-3: validador de eventos de historia clínica.
            builder.Services.AddScoped<IValidadorEventoClinico, ValidadorEventoClinico>();

            // --- Infraestructura: repositorios (uno por entidad, DIP) ---
            builder.Services.AddScoped<IRepositorioPotreros, RepositorioPotrerosTexto>();
            builder.Services.AddScoped<IRepositorioUsuarios, RepositorioUsuariosTexto>();
            builder.Services.AddScoped<IRepositorioVacunas, RepositorioVacunasTexto>();
            builder.Services.AddScoped<IRepositorioVentas, RepositorioVentasTexto>();
            // SC-3: repositorio de historia clínica (uno por res, ver EventoClinico).
            builder.Services.AddScoped<IRepositorioHistoriaClinica, RepositorioHistoriaClinicaTexto>();

            // --- Aplicacion: fábricas de vacunas ---
            builder.Services.AddScoped<IFabricaVacunaBacteriana, FabricaVacunaBacteriana>();
            builder.Services.AddScoped<IFabricaVacunaViva, FabricaVacunaViva>();

            // --- Aplicacion: registro de fábricas de Res, indexado por Tipo_potrero ---
            // Claves = mismos valores de texto que el enum l_tipos_potreros del
            // AS-IS (Bib_Hacienda.Clases.Potrero.l_tipos_potreros: ternero,
            // cebon, novillo), que en el TO-BE Potrero.Tipo_potrero pasó de enum
            // a string (ver Potrero.cs y RegistroFabricasRes.cs). No se inventan
            // claves nuevas: son las mismas 3 que ya usaban las Views migradas
            // (Views/Potrero/Create.cshtml).
            builder.Services.AddScoped<IRegistroFabricasRes>(sp =>
            {
                var fabricas = new Dictionary<string, IFabricaRes>
                {
                    { "ternero", new FabricaTernero() },
                    { "cebon", new FabricaCebon() },
                    { "novillo", new FabricaNovillo() }
                };
                return new RegistroFabricasRes(fabricas);
            });

            // --- Aplicacion: autorización ---
            // IServicioHash no tenía implementación concreta (ver Bloque
            // 3C-2A); se agrega ServicioHashSha256 (Infraestructura), la
            // mínima necesaria para que ServicioAutenticacion pueda
            // hashear/verificar contraseñas.
            builder.Services.AddScoped<IServicioHash, ServicioHashSha256>();
            builder.Services.AddScoped<IProveedorPermisos, ProveedorPermisosPorRol>();

            // --- Dominio: publishers de eventos ---
            // PublisherPotreroMitad y PublisherPotreroLleno NO se registran
            // aquí: el UML no le da a ServicioPotreros esa dependencia por
            // constructor, así que CrearPotrero los sigue instanciando con
            // "new" directamente (ver nota en ServicioPotreros.cs).
            builder.Services.AddScoped<PublisherPesoMin>();
            builder.Services.AddScoped<PublisherPesoVenta>();
            builder.Services.AddScoped<PublisherVacunaVencida>();
            builder.Services.AddScoped<PublisherVacunacionCompletada>();

            // --- Aplicacion: servicios de aplicación (los que consumen los Controllers) ---
            builder.Services.AddScoped<ServicioAutenticacion>();
            builder.Services.AddScoped<ServicioPotreros>();
            builder.Services.AddScoped<ServicioAlimentacion>();
            builder.Services.AddScoped<ServicioVentas>();
            builder.Services.AddScoped<ServicioInventarioVacunas>();
            builder.Services.AddScoped<ServicioVacunacion>();
            // SC-3: servicio de aplicación para registrar/consultar la historia clínica de una Res.
            builder.Services.AddScoped<ServicioHistoriaClinica>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // --- Habilitar Autenticación y Autorización ---
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
