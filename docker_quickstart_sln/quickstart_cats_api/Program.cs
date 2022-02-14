using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*
 * There are some tricky little things to known when connecting to a service in another container
 * for local development you need to set the domain of the service, ex: localhost:6379
 * 
 * Through docker compose set the connection string to the named service instance in the docker compose file
 * ex: cache:6379
 */
var redisConfig = new ConfigurationOptions
{
    Password = Environment.GetEnvironmentVariable("REDIS_PASSWORD")
};
redisConfig.EndPoints.Add($"{Environment.GetEnvironmentVariable("REDIS_HOST")}:{Environment.GetEnvironmentVariable("REDIS_PORT")}");
var redis = ConnectionMultiplexer.Connect(redisConfig);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(builder =>
    builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .WithMethods(new string[] { "GET", "POST", "PUT" })
);

app.UseAuthorization();

app.MapControllers();

app.Run();
