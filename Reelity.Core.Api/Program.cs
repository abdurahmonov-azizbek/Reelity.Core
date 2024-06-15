// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reelity.Core.Api.Brokers.DateTimes;
using Reelity.Core.Api.Brokers.Loggings;
using Reelity.Core.Api.Brokers.Storages;
using Reelity.Core.Api.Services.VideoMetadatas;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AddBrokers(builder.Services);
AddFoundationServices(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void AddBrokers(IServiceCollection services)
{
    services.AddTransient<IStorageBroker, StorageBroker>();
    services.AddTransient<ILoggingBroker, LoggingBroker>();
    services.AddTransient<IDateTimeBroker, DateTimeBroker>();
}

static void AddFoundationServices(IServiceCollection services)
{
    services.AddTransient<IVideoMetadataService, VideoMetadataService>();
}
