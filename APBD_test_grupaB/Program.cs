using APBD_test_grupaB.Components;
using APBD_test_grupaB.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();