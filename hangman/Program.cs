using hangman.Components;
using hangman.Data;
using hangman.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;
using MudBlazor.Services;
using hangman.Models;
using System.Diagnostics.Tracing;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<UserService>();
var validator = new WordValidator("words.txt");
var roomservice = new RoomService(validator);//go
builder.Services.AddSingleton(roomservice);
builder.Services.AddScoped<Client>(); // client hozzaadva scoped kinet elerheto az adott user frontendjén
builder.Services.AddMudServices();
//release verzihoz kell hogy 80 as porton broadcastolja a webszervert
#if RELEASE
    builder.WebHost.UseUrls("http://*:80"); 
#endif
AppDbContext.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine(AppDbContext.ConnectionString);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var context = new AppDbContext())
{
    context.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
