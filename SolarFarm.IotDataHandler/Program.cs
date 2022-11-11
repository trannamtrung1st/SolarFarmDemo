using Microsoft.EntityFrameworkCore;
using SolarFarm.Entities;
using SolarFarm.IotDataHandler;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();

        services.AddDbContext<IotDataContext>(opt =>
            opt.UseSqlServer(context.Configuration.GetConnectionString("IotDataContext")));
    })
    .Build();

await host.RunAsync();