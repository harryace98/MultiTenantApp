using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("postgres-user", "postgres");
var password = builder.AddParameter("postgres-password", "StrongPass123");
// PostgreSQL database resource
var db = builder.AddPostgres("db")
    .WithUserName(username)
    .WithPassword(password)
    .WithDataVolume("my-postgres-data")
    .WithPgAdmin()
    .WithHostPort(5432);

var baseDatabase = db.AddDatabase("baseDatabase");

// API project with connection string from Aspire resource
builder.AddProject<Projects.MultiTenantApp_API>("multitenantapp-api")
    .WithReference(db);

builder.Build().Run();
