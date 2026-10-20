

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClinicApiCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7176",
                "http://localhost:5202"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



// 1. ADD SWAGGER SERVICES
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // <--- Make sure this is here!



var app = builder.Build();



// 2. ENABLE SWAGGER MIDDLEWARE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Generates the JSON spec
    app.UseSwaggerUI(); // Serves the HTML webpage at /swagger
}
app.UseHttpsRedirection();
app.UseCors("ClinicApiCorsPolicy");   // <-- MUST be before MapControllers / UseAuthorization


app.UseAuthorization();
app.MapControllers();

app.Run();
