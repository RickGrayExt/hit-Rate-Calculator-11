
using Contracts;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>using Contracts;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<HitRateCalculatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", h => { });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapGet("/health", () => "ok");

// Store results in memory for this demo (in production, use a database)
var results = new Dictionary<Guid, double>();

app.MapGet("/result/{runId:guid}", (Guid runId) =>
{
    if (results.TryGetValue(runId, out var hitRate))
    {
        return Results.Json(new { runId, hitRate, status = "completed" });
    }
    return Results.Json(new { runId, status = "processing" });
});

app.Run("http://0.0.0.0:8080");

public class HitRateCalculatedConsumer : IConsumer<HitRateCalculated>
{
    private readonly ILogger<HitRateCalculatedConsumer> _logger;

    public HitRateCalculatedConsumer(ILogger<HitRateCalculatedConsumer> logger)
    {
        _logger = loggerusing Contracts;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<HitRateCalculatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", h => { });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapGet("/health", () => "ok");

// Store results in memory for this demo (in production, use a database)
var results = new Dictionary<Guid, double>();

app.MapGet("/result/{runId:guid}", (Guid runId) =>
{
    if (Program.TryGetResult(runId, out var hitRate))
    {
        return Results.Json(new { runId, hitRate, status = "completed" });
    }
    return Results.Json(new { runId, status = "processing" });
});

app.Run("http://0.0.0.0:8080");

public class HitRateCalculatedConsumer : IConsumer<HitRateCalculated>
{
    private readonly ILogger<HitRateCalculatedConsumer> _logger;

    public HitRateCalculatedConsumer(ILogger<HitRateCalculatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<HitRateCalculated> context)
    {
        _logger.LogInformation("Received hit rate calculation result: RunId={RunId}, HitRate={HitRate}", 
            context.Message.RunId, context.Message.HitRate);

        // In a real application, you would store this in a database
        // For this demo, we'll use a static collection accessible from the main program
        Program.StoreResult(context.Message.RunId, context.Message.HitRate);

        return Task.CompletedTask;
    }
}

public static partial class Program
{
    private static readonly Dictionary<Guid, double> _results = new();

    public static void StoreResult(Guid runId, double hitRate)
    {
        _results[runId] = hitRate;
    }

    public static bool TryGetResult(Guid runId, out double hitRate)
    {
        return _results.TryGetValue(runId, out hitRate);
    }
};
    }

    public Task Consume(ConsumeContext<HitRateCalculated> context)
    {
        _logger.LogInformation("Received hit rate calculation result: RunId={RunId}, HitRate={HitRate}", 
            context.Message.RunId, context.Message.HitRate);

        // In a real application, you would store this in a database
        // For this demo, we'll use a static collection accessible from the main program
        Program.StoreResult(context.Message.RunId, context.Message.HitRate);

        return Task.CompletedTask;
    }
}

public static partial class Program
{
    private static readonly Dictionary<Guid, double> _results = new();

    public static void StoreResult(Guid runId, double hitRate)
    {
        _results[runId] = hitRate;
    }

    public static bool TryGetResult(Guid runId, out double hitRate)
    {
        return _results.TryGetValue(runId, out hitRate);
    }
}
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", h => { });
    });
});

var app = builder.Build();
app.MapGet("/health", () => "ok");

app.MapPost("/run", async (StartRunCommand req, IBus bus) =>
{
    var dataset = string.IsNullOrWhiteSpace(req.DatasetPath) ? "/app/data/DataSetClean.csv" : req.DatasetPath;
    var runId = req.RunId == Guid.Empty ? Guid.NewGuid() : req.RunId;
    var cmd = req with { RunId = runId, DatasetPath = dataset };
    await bus.Publish(cmd);

    // ✅ Always return JSON instead of Accepted
    return Results.Json(new { runId });
});

app.Run("http://0.0.0.0:8080");
