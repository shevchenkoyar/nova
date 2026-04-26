var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Nova_WebAPI>("nova-webapi");

builder.Build().Run();
