using API.Data;
using API.Extensions;
using API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle



// p' mi metodo de extension ApplicationServiceExtensions
builder.Services.AddAplicationServices(builder.Configuration);

// p' mi metodo de extension AddIdentityServices
builder.Services.AddIdentityServices(builder.Configuration);



builder.Services.AddEndpointsApiExplorer(); // *
builder.Services.AddSwaggerGen(); // *

var app = builder.Build();

// Configure the HTTP request pipeline.
// DEBE IR EN LA PARTE DE MAS ARRIBA DEL pipeline
// este es para ocupar mi middleware de excepciones y no tener que poner try-catch por todos lados
app.UseMiddleware<ExceptionMiddleware>();





if (app.Environment.IsDevelopment()) // *
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); // *

//app.UseAuthorization();  // *



// debe ir entre UseRouting y Endpoint, y antes de Authorization y UseAuthentication
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));





// antes de MapControllers y despues de UseCors
app.UseAuthentication(); // checa q el token sea valido
app.UseAuthorization(); // checa lo q tengo permitido hacer de acuerdo a lo q diga mi token




app.MapControllers();




// para el seeding de users, va despues de MapControllers y antes de .Run
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
// try-catch p' errores durante el seeding
try
{
    var context = services.GetRequiredService<DataContext>();
    //var userManager = services.GetRequiredService<UserManager<AppUser>>();
    //var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    //await Seed.SeedUsers(context);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}




app.Run();
