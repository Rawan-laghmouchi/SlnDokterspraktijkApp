using Dokterspraktijk.Application.Repositories;
using Dokterspraktijk.Application.Services.Implementation;
using Dokterspraktijk.Application.Services.Interfaces;
using Dokterspraktijk.Infrastructure.Data;
using Dokterspraktijk.Infrastructure.Identity;
using Dokterspraktijk.Infrastructure.Repositories;
using Dokterspraktijk.WebUI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

string connString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connString));

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("IsAdmin", "true"));

    options.AddPolicy("DokterOnly", policy =>
        policy.RequireClaim("IsDokter", "true"));

    options.AddPolicy("PatientOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.Identity != null &&
            context.User.Identity.IsAuthenticated &&
            !context.User.HasClaim("IsAdmin", "true") &&
            !context.User.HasClaim("IsDokter", "true")));
});

// Tijdelijke fake repositories voor de experimentele testfase van de bachelorproef.
// testdata tijdens het draaien v/d applicatie blijft behouden.
// builder.Services.AddSingleton<FakeDokterspraktijkDatastore>();

// tijdelijke fake repo's
// Later worden deze registraties vervangen door EF Core repositories.
//builder.Services.AddScoped<IDokterRepository, FakeDokterRepository>();
//builder.Services.AddScoped<IPatientRepository, FakePatientRepository>();
//builder.Services.AddScoped<ITijdslotRepository, FakeTijdslotRepository>();
//builder.Services.AddScoped<IAfspraakRepository, FakeAfspraakRepository>();
//builder.Services.AddScoped<IDoktersattestRepository, FakeDoktersattestRepository>();

builder.Services.AddScoped<IDokterRepository, DokterRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<ITijdslotRepository, TijdslotRepository>();
builder.Services.AddScoped<IAfspraakRepository, AfspraakRepository>();
builder.Services.AddScoped<IDoktersattestRepository, DoktersattestRepository>();
builder.Services.AddScoped<IAfspraakCategorieRepository, AfspraakCategorieRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// application services
builder.Services.AddScoped<ITijdslotService, TijdslotService>();
builder.Services.AddScoped<IAfspraakService, AfspraakService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoktersattestService, DoktersattestService>();

// builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // ��n instantie per request

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    UserManager<ApplicationUser> userManager =
        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    SeedData.Initialize(userManager);
}

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
