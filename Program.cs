using InmobiliariaCC2.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registramos los repositorios para poder utilizarlos
// posteriormente mediante inyección de dependencias.
builder.Services.AddScoped<RepositorioPropietario>();
builder.Services.AddScoped<RepositorioReserva>();
builder.Services.AddScoped<RepositorioTipoInmueble>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();