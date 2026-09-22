using System.Text.Json;
using System.Text.Json.Serialization;
using Compellio.Bcbcti.Configuration;
using Compellio.Bcbcti.Exceptions;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services;
using Compellio.Bcbcti.Services.Ingestion;
using Compellio.Bcbcti.Services.RegistryApi;
using Compellio.Bcbcti.Services.Storage;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Json.Canonical;
using Compellio.Bcbcti.Services.Taxii;

var builder = WebApplication.CreateBuilder(args);

// TODO add middleware that rejects sorting queries (see section 3.3): return "not implemented" response)
// TODO implement TAXII error handling (see section 3.6): error format very specific -> global server config?)

// Load TAXII server configuration
builder.Services.AddOptions<TaxiiOptions>()
    .Bind(builder.Configuration.GetSection("TAXII"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Load BCBCTI configuration
builder.Services.AddOptions<BcbctiOptions>()
    .Bind(builder.Configuration.GetSection("BCBCTI"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddRegistryApi(); // TODO options, etc.

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

builder.Services.AddObjectStore(builder.Configuration.GetSection("Storage"));

builder.Services
    .AddJsonObjectStore(options =>
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .AddCanonicalJsonObjectStore("canonical");

builder.Services.AddSingleton<CollectionsManager>();

builder.Services.AddSingleton<JournalRepository>();
builder.Services.AddSingleton<StixObjectRepository>();
builder.Services.AddSingleton<RegistrationReceiptsRepository>();
builder.Services.AddSingleton<RegistryOperationRepository>();
builder.Services.AddSingleton<ObjectRegistrationRepository>();
builder.Services.AddSingleton<ManifestRepository>();

builder.Services.AddSingleton<StixIngestionService>();
builder.Services.AddSingleton<StixReconciliationService>();
builder.Services.AddHostedService<ReconciliationHostedService>();

builder.Services.AddTaxiiServices();

builder.Services
    .AddControllers(options => { options.ReturnHttpNotAcceptable = true; })
    .AddJsonOptions(options =>
    {
        // TODO add TAXII-specific DateTime converter (5-point ms precision)
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.ConfigureOptions<ConfigureTaxiiMediaTypes>();

builder.Services.AddExceptionHandler<TaxiiExceptionHandler>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseAuthorization();

app.MapControllers();

app.Run();