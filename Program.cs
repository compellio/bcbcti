using System.Text.Json;
using System.Text.Json.Serialization;
using Bcbcti.Configuration;
using Bcbcti.Exceptions;
using Bcbcti.Options;
using Bcbcti.Services;

var builder = WebApplication.CreateBuilder(args);

// TODO add middleware that rejects sorting queries (see section 3.3): return "not implemented" response)
// TODO implement TAXII error handling (see section 3.6): error format very specific -> global server config?)

// Load TAXII server configuration
builder.Services.AddOptions<TaxiiOptions>().Bind(builder.Configuration.GetSection("TAXII")).ValidateOnStart();

// Load BCBCTI configuration
builder.Services.AddOptions<BcbctiOptions>()
    .Bind(builder.Configuration.GetSection("BCBCTI"))
    .Validate(options => options.Collections.Length > 0, "At least one collection is required")
    .ValidateOnStart();

builder.Services.AddSingleton<CollectionsCatalog>();

builder.Services
    .AddControllers(options =>
    {
        options.ReturnHttpNotAcceptable = true;
    })
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