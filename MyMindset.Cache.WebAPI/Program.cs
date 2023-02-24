using MyMindset.Cache.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using MyMindset.Cache.WebAPI.Infrastracture;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddDbContext<KeyAndValueContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabase")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var keyAndValueContext = scope.ServiceProvider.GetRequiredService<KeyAndValueContext>();
    await keyAndValueContext.Database.EnsureDeletedAsync();
    await keyAndValueContext.Database.EnsureCreatedAsync();
    await keyAndValueContext.SeedAsync();
}
var slidingExpiration = new DistributedCacheEntryOptions
{
    SlidingExpiration = TimeSpan.FromSeconds(5)
};

app.MapGet("/{key}", async (
    string key,
    IDistributedCache distributedCache,
    KeyAndValueContext keyAndValueContext
) =>
{
    string? value = distributedCache.GetString(key);
    if (value is not null)
    {
        return $"Key:{key}, Value:{value}. Source:Redis";
    }

    var keyAndValue = await keyAndValueContext.FindAsync<KeyValueModel>(key);

    if (keyAndValue is null)
    {
        return $"{key} not found";
    }

    await distributedCache.SetStringAsync(key, keyAndValue.Value, slidingExpiration);
    return $"Key:{key}, Value:{keyAndValue.Value}. Source:MSSQL";
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
