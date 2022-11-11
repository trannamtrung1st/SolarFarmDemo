using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SolarFarm;
using SolarFarm.Entities;
using SolarFarm.IotDataApi.Models;
using SolarFarm.IotDataApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s => s.CustomSchemaIds(t => t.FullName));

builder.Services.AddCors();

builder.Services.AddSingleton<IMessageBrokerService, KafkaMessageBrokerService>();

builder.Services.AddDbContext<IotDataContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("IotDataContext")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(b =>
{
    b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
});

app.UseHttpsRedirection();

app.MapGet("/api/panels", async ([FromServices] IotDataContext dbContext) =>
{
    var panels = await dbContext.SolarPanels.OrderBy(p => p.Name).ToArrayAsync();

    return panels;
})
.WithName("Get panels information");

app.MapPost("/api/iot-data", async (SolarPanelData data) =>
{
    data.Id = Guid.NewGuid();
    data.Time = DateTimeOffset.UtcNow;

    var result = await Producer.ProduceAsync(EventConstants.SolarPanelData, new Message<string, string>
    {
        Key = data.Id.ToString(),
        Value = JsonConvert.SerializeObject(data)
    });

    return Results.Accepted();
})
.WithName("Send IoT data");

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider provider = scope.ServiceProvider;

    await InitializeAsync(provider);

    await SetupMessageBrokerAsync(provider, app.Configuration);
}

app.MapGet("/api/dashboard/token", ([FromServices] IConfiguration configuration) =>
{
    var metabaseUrl = configuration.GetValue<string>("MetabaseUrl");
    var metabaseSecret = configuration.GetValue<string>("MetabaseDashboardSecretKey");
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(metabaseSecret));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    var exp = DateTime.UtcNow.AddDays(360);
    var payload = new JwtPayload()
    {
        ["resource"] = new Dictionary<string, int>
        {
            ["dashboard"] = 1
        },
        ["params"] = new Dictionary<string, string>(),
        ["exp"] = TimeSpan.FromTicks(exp.Ticks).TotalSeconds
    };

    var token = new JwtSecurityToken(new JwtHeader(credentials), payload);
    var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

    return new DashboardTokenModel
    {
        Token = tokenStr,
        BaseUrl = metabaseUrl
    };
})
.WithName("Get dashboard token");


using (Producer)
{
    app.Run();
}

static async Task InitializeAsync(IServiceProvider serviceProvider)
{
    var dbContext = serviceProvider.GetRequiredService<IotDataContext>();

    await dbContext.Database.MigrateAsync();
}

static async Task SetupMessageBrokerAsync(IServiceProvider provider, IConfiguration configuration)
{
    IMessageBrokerService messageBrokerService = provider.GetRequiredService<IMessageBrokerService>();

    await messageBrokerService.InitializeTopicsAsync();

    var producerConfig = new ProducerConfig();
    configuration.Bind("ProducerConfig", producerConfig);

    Producer = new ProducerBuilder<string, string>(producerConfig).Build();
}

static partial class Program
{
    static IProducer<string, string> Producer { get; set; }
}