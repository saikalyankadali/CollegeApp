using CollegeApp.Configurations;
using CollegeApp.Data;
using CollegeApp.Data.Repository;
using CollegeApp.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var corsPolicyName = "CollegeAppPolicy";
var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecret"));
builder.Services.AddDbContext<CollegeDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CollegeAppConnectionString"));
});
#region serilog settings
//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Information()
//    .WriteTo.File("F:\\PracticeDotNet\\CollegeApp\\Logs\\Log.txt", rollingInterval : RollingInterval.Minute)
//    .CreateLogger();

//// Use this serilog to overide the built-in loggers
//builder.Services.AddSerilog();
#endregion

#region log4net settings
builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();
#endregion

//// Use this serilog along with built-in loggers
//builder.Logging.AddSerilog();

// Add services to the container.
builder.Services.AddControllers(options => options.ReturnHttpNotAcceptable = true)
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

// ✅ Add Swagger/OpenAPI services
// Replace this line:
// builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

// With this line:
builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperConfig));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CollegeApp API", Version = "v1" });

    // Force Swagger UI to use JSON by default
    c.SupportNonNullableReferenceTypes();
});

builder.Services.AddScoped<IMyLogger, LogToFile>();
builder.Services.AddTransient<IStudentRepository, StudentRepository>();
builder.Services.AddTransient(typeof(ICollegeRepository<>), typeof(CollegeRepository<>));

builder.Services.AddCors(options => options.AddPolicy(corsPolicyName, policy =>
{
    //Allow all origins
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // ✅ Enable Swagger middleware
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();
