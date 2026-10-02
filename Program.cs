// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Configuration;
using Compellio.Bcbcti.Exceptions;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services;
using Compellio.Bcbcti.Services.Ingestion;
using Compellio.Bcbcti.Services.RegistryApi;
using Compellio.Bcbcti.Services.Serialization;
using Compellio.Bcbcti.Services.Storage;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Json.Canonical;
using Compellio.Bcbcti.Services.Taxii;
using Compellio.Bcbcti.Services.Taxii.Filters;

var builder = WebApplication.CreateBuilder(args);

// TODO implement TAXII error handling (see section 3.6): error format very specific -> global server config?)

// Load BCBCTI configuration
builder.Services.AddSingleton<CollectionsManager>();
builder.Services.AddOptions<CollectionsOptions>()
    .Bind(builder.Configuration.GetSection("BCBCTI"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<JournalRepository>();
builder.Services.AddSingleton<StixObjectRepository>();
builder.Services.AddSingleton<RegistrationReceiptsRepository>();
builder.Services.AddSingleton<RegistryOperationRepository>();
builder.Services.AddSingleton<ObjectRegistrationRepository>();
builder.Services.AddSingleton<ManifestRepository>();

builder.Services.AddRegistryApi(builder.Configuration.GetSection("RegistryApi")); // TODO options, etc.

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
// TODO merge and move to new AddStorage in Services.Storage
builder.Services.AddObjectStore(builder.Configuration.GetSection("BCBCTI:Storage"));
builder.Services
    .AddJsonObjectStore(JsonSerializerConfigurations.Storage)
    .AddCanonicalJsonObjectStore("canonical");

builder.Services.AddIngestion(builder.Configuration.GetSection("BCBCTI:Ingestion"));

builder.Services.AddTaxiiServices(builder.Configuration.GetSection("BCBCTI:TAXII"));
builder.Services.AddExceptionHandler<TaxiiExceptionHandler>(); // TODO-REVIEW move to Services.Taxii

builder.Services
    .AddControllers(options =>
    {
        options.ReturnHttpNotAcceptable = true;
        options.Filters.Add<TaxiiCustomHeadersFilter>(); // TODO-REVIEW review placement in .AddTaxiiServices()?
    })
    .AddJsonOptions(options => JsonSerializerConfigurations.Taxii(options.JsonSerializerOptions));

builder.Services.ConfigureOptions<ConfigureTaxiiMediaTypes>();

builder.WebHost.ConfigureKestrel(options => { options.AddServerHeader = false; });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage(); // TODO ignore custom taxii exceptions in dev
}

app.UseAuthorization();

app.MapControllers();

app.Run();
