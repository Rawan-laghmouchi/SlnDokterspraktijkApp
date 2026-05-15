using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Implementation;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Infrastructure.Data;
using Dokterspraktijk.Infrastructure.Fakes;
using Dokterspraktijk.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connString));

// Tijdelijke fake repositories voor de experimentele testfase van de bachelorproef.
// testdata tijdens het draaien v/d applicatie blijft behouden.
builder.Services.AddSingleton<FakeDokterspraktijkDatastore>();

// tijdelijke fake repo's
// Later worden deze registraties vervangen door EF Core repositories.
builder.Services.AddScoped<IDokterRepository, FakeDokterRepository>();
builder.Services.AddScoped<IPatientRepository, FakePatientRepository>();
builder.Services.AddScoped<ITijdslotRepository, FakeTijdslotRepository>();
builder.Services.AddScoped<IAfspraakRepository, FakeAfspraakRepository>();
builder.Services.AddScoped<IDoktersattestRepository, FakeDoktersattestRepository>();

// application services
builder.Services.AddScoped<ITijdslotService, TijdslotService>();
builder.Services.AddScoped<IAfspraakService, AfspraakService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoktersattestService, DoktersattestService>(); ;

// builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // één instantie per request

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
