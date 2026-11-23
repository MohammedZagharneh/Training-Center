using Business.Registrations;
using APIs.ExceptionMiddleware;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddBusinessServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("TrainingCenterAPIsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7041","http://localhost:5206").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("TrainingCenterAPIsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
