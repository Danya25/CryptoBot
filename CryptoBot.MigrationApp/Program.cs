// See https://aka.ms/new-console-template for more information

using CryptoBot.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Applying migrations");

var iHost = Host.CreateDefaultBuilder()
    .Build();
using (var context = iHost.Services.GetRequiredService<ApplicationContext>())
{
    context.Database.Migrate();
}
Console.WriteLine("Done");