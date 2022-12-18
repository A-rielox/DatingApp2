using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

app.Run();
