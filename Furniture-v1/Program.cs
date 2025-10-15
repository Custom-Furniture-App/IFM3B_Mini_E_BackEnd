var builder = WebApplication.CreateBuilder(args);

// CORS policy
const string FurnitureCorsPolicy = "_furnitureCorsPolicy";
builder.Services.AddCors(options =>
{
  options.AddPolicy(FurnitureCorsPolicy, policy =>
  {
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
  });
});

builder.Services.AddControllers();

// Register DatabaseHelper
builder.Services.AddScoped<DatabaseHelper>(sp =>
{
  var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
  return new DatabaseHelper(connectionString!);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Furniture API v1");
  c.RoutePrefix = "swagger";
});

// Use CORS
app.UseCors(FurnitureCorsPolicy);

app.UseAuthorization();
app.MapControllers();

app.Urls.Clear();                 
app.Urls.Add("http://0.0.0.0:5012");  

app.Run();
