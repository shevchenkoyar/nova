using System.Reflection;
using Nava.Modules.Memory.Infrastructure;
using Nova.Common.Application;
using Nova.Common.Infrastructure;
using Nova.Common.Presentation.Endpoints;
using Nova.Modules.Clock.Infrastructure;
using Nova.Modules.Conversation.Infrastructure;
using Nova.Modules.HomeAssistant.Infrastructure;
using Nova.Modules.Reader.Infrastructure;
using Nova.Modules.Relationships.Infrastructure;
using Nova.Modules.Research.Application;
using Nova.Modules.Search.Infrastructure;
using Nova.WebAPI.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options => options.AddScalarTransformers());

builder.AddRedisClient("nova-redis");

Assembly[] moduleApplicationAssemblies = 
[
    Nova.Modules.Conversation.Application.AssemblyReference.Assembly,
];

builder.Services.AddApplication(moduleApplicationAssemblies, builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddConversationModule(builder.Configuration);

builder.Services.AddSearchModule(builder.Configuration);

builder.Services.AddRelationshipsModule(builder.Configuration);

builder.Services.AddReaderModule(builder.Configuration);

builder.Services.AddResearchModule(builder.Configuration);

builder.Services.AddMemoryModule();

builder.Services.AddHomeAssistantModule(builder.Configuration);

builder.Services.AddClockModule();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapEndpoints();

await app.RunAsync();