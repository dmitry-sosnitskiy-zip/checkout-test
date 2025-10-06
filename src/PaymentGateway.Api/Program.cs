using System.Text.Json.Serialization;
using FluentValidation;
using PaymentGateway.Application;
using PaymentGateway.Application.Behaviors;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Gateways;
using PaymentGateway.Infrastructure;
using PaymentGateway.Infrastructure.Gateways.Mocks;
using PaymentGateway.Infrastructure.Gateways.MountebankSimulator;

using HttpClient = PaymentGateway.Infrastructure.HttpClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IHttpClient, HttpClient>();
builder.Services.AddSingleton<IRepository, InMemoryRepository>();
builder.Services.AddSingleton<PaymentProcessingGatewayFactory>(x => 
    new PaymentProcessingGatewayFactory([
        new MountebankSimulatorGateway(x.GetRequiredService<IHttpClient>()),
        new SuccessMockGateway(),
        new FailMockGateway()
    ]));

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblyContaining<ApplicationLayer>();
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssemblyContaining<ApplicationLayer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
