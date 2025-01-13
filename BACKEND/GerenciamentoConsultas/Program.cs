using GerenciamentoConsultas.Application.Services;
using GerenciamentoConsultas.Domain.Interfaces;
using GerenciamentoConsultas.Domain.Services;
using GerenciamentoConsultas.Infraestrutura.Repositories;
using GerenciamentoConsultas.Infrastructure.Data;
using GerenciamentoConsultas.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar o DbContext com o banco de dados PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar a conexão IDbConnection para o Dapper
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new NpgsqlConnection(connectionString);
});

// Registrar serviços da aplicação
builder.Services.AddScoped<ConsultaAppService>();
builder.Services.AddScoped<MedicoAppService>();
builder.Services.AddScoped<PacienteAppService>();
builder.Services.AddScoped<UsuarioAppService>();
builder.Services.AddScoped<JWTService>();

// Registrar serviços de domínio
builder.Services.AddScoped<ConsultaService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<PacienteService>();

// Registrar repositórios
builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Configurar autenticação com JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(jwtSettings["Issuer"]) || string.IsNullOrEmpty(jwtSettings["Audience"]))
{
    Console.WriteLine("Erro: Configuração do JWT incompleta. Verifique o arquivo appsettings.json.");
    throw new InvalidOperationException("Configuração do JWT incompleta.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var authorizationHeader = context.Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                {
                    // Remove o prefixo "Bearer " antes de passar o token para validação
                    context.Token = authorizationHeader.Substring(7);
                    Console.WriteLine($"Token processado: {context.Token}");
                }
                else
                {
                    Console.WriteLine("Cabeçalho Authorization não contém o prefixo 'Bearer '.");
                }
            }
            else
            {
                Console.WriteLine("Nenhum cabeçalho Authorization foi encontrado.");
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[Autenticação] Token inválido: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("[Autenticação] Token válido.");
            return Task.CompletedTask;
        }
    };
});

// Configurar políticas de autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PacientePolicy", policy =>
    {
        policy.RequireClaim("role", "Paciente");
    });
    options.AddPolicy("MedicoPolicy", policy =>
    {
        policy.RequireClaim("role", "Medico");
    });
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:5173") // Domínio do frontend
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// Adicionar controladores e documentação via Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração de ambiente para desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gerenciamento Consultas API V1");
        options.RoutePrefix = string.Empty; // Deixa o Swagger acessível na raiz
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Usar autenticação e autorização na ordem correta
app.UseRouting();

// Usar CORS
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
