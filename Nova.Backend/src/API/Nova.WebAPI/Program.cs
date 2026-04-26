using System.Reflection;
using Nova.Common.Application;
using Nova.Common.Infrastructure;
using Nova.Common.Presentation.Endpoints;
using Nova.Modules.Conversation.Infrastructure;
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

builder.Services.AddApplication(moduleApplicationAssemblies);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddConversationModule(builder.Configuration);

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