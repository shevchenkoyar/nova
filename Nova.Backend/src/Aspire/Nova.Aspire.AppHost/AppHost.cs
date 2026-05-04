using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> pgUser = builder.AddParameter("nova-pg-username", secret: true);
IResourceBuilder<ParameterResource> pgPass = builder.AddParameter("nova-pg-password", secret: true);
IResourceBuilder<ParameterResource> redisPass = builder.AddParameter("nova-redis-password", secret: true);
IResourceBuilder<ParameterResource> rabitmqUser = builder.AddParameter("nova-rabitmq-username", secret: true);
IResourceBuilder<ParameterResource> rabitmqPass = builder.AddParameter("nova-rabitmq-password", secret: true);

var postgres = builder
    .AddPostgres("nova-postgres", pgUser, pgPass)
    .WithDataVolume();

var coreDatabase = postgres.AddDatabase("nova-db");

IResourceBuilder<RedisResource> redis = builder.AddRedis("nova-redis", password: redisPass);

IResourceBuilder<RabbitMQServerResource> rabbitMq = builder.AddRabbitMQ(
    "nova-rabbitmq", rabitmqUser, rabitmqPass);

var migrator = builder
    .AddProject<Projects.Nova_Migrator>("nova-migrator")
    .WithReference(coreDatabase)
    .WaitFor(coreDatabase);

var coreWebApi = builder.AddProject<Projects.Nova_WebAPI>("nova-webapi")
    .WithReference(coreDatabase)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WaitFor(coreDatabase)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitForCompletion(migrator);

builder.AddScalarApiReference("nova-scalar", 50216 ,options =>
{
    options.WithTheme(ScalarTheme.Kepler); 
    options.EnableDarkMode(); 
    options.SortOperationsByMethod(); 
    options.HideDeveloperTools(); 
    options.HideClientButton(); 
    options.HideDarkModeToggle(); 
    options.HideModels();
}).WithApiReference(coreWebApi);

builder.Build().Run();
