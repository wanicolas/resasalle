using Microsoft.EntityFrameworkCore;
using Roomie.Backend.Data;
using Roomie.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration CORS pour Nuxt.js (Front)
var corsPolicyName = "AllowNuxt";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Modifie selon ton URL front
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Ajouter la connexion à la base de données SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=roomie.db"));

var app = builder.Build();

// Vérifier la base de données et ajouter des données de test si nécessaire
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    // Vérifier si la base de données est déjà créée
    context.Database.EnsureCreated();

    // Ajouter des données de test si la table Rooms est vide
    if (!context.Rooms.Any())
    {
        context.Rooms.AddRange(new List<Room>
        {
            new Room { Name = "Salle A", Capacity = 20, AccessiblePMR = true },
            new Room { Name = "Salle B", Capacity = 10, AccessiblePMR = false },
            new Room { Name = "Salle C", Capacity = 50, AccessiblePMR = true }
        });
        context.SaveChanges();
    }
}

// Activer Swagger uniquement en mode développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activer CORS avant les contrôleurs
app.UseCors(corsPolicyName);
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
