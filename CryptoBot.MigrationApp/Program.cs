// See https://aka.ms/new-console-template for more information

using CryptoBot.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Migration tool is starting");

if (args.Length != 1)
    throw new Exception("Connection string doesn't exist.");

var connectionString = args[0];

if (string.IsNullOrEmpty(connectionString))
    throw new Exception("Connection string is incorrect.");
        
var host = Host.CreateDefaultBuilder().ConfigureServices((configuration, services) =>
{
    services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));
}).Build();

var context = host.Services.GetRequiredService<ApplicationContext>();
context.Database.Migrate();
        
Console.WriteLine("Migration tool is gone.");