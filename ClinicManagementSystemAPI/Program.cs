var builder = WebApplication.CreateBuilder(args);

// 1. ADD SWAGGER SERVICES
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // <--- Make sure this is here!

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();



// 2. ENABLE SWAGGER MIDDLEWARE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Generates the JSON spec
    app.UseSwaggerUI(); // Serves the HTML webpage at /swagger
}
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");   // <-- MUST be before MapControllers / UseAuthorization


app.UseAuthorization();
app.MapControllers();

app.Run();
