using System.Text.Json;
using System.Text.Json.Serialization;
using Bcbcti.Configuration;
using Bcbcti.Exceptions;
using Bcbcti.Options;
using Bcbcti.Services;
using Bcbcti.Services.Storage;
using Bcbcti.Services.Storage.Json;

var builder = WebApplication.CreateBuilder(args);

// TODO add middleware that rejects sorting queries (see section 3.3): return "not implemented" response)
// TODO implement TAXII error handling (see section 3.6): error format very specific -> global server config?)

// Load TAXII server configuration
builder.Services.AddOptions<TaxiiOptions>().Bind(builder.Configuration.GetSection("TAXII"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Load BCBCTI configuration
builder.Services.AddOptions<BcbctiOptions>()
    .Bind(builder.Configuration.GetSection("BCBCTI"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

builder.Services.AddObjectStore(builder.Configuration.GetSection("Storage"));
builder.Services.AddJsonObjectStore(options =>
{
    // TODO JCS will probably be configured here when added
    options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSingleton<CollectionsManager>();

builder.Services
    .AddControllers(options => { options.ReturnHttpNotAcceptable = true; })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
    // app.UseDeveloperExceptionPage();
}

app.UseAuthorization();

app.MapControllers();

app.Run();