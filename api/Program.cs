//------------------- Settings ----------------------

using api;
using Microsoft.Data.SqlClient;
using System.Data;
using DotNetEnv;
using api.DTO;

var builder = WebApplication.CreateBuilder(args);

Env.Load(); // Load .env file

var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_SERVER")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                       $"TrustServerCertificate=True;";

builder.Services.AddSingleton(new Database(connectionString));

var app = builder.Build();

//----------------- API Endpoints -------------------

//----- Logs

app.MapGet("/", () => "Take Them Down API is Running");

app.MapGet("/log", async (Database db) =>
{
    //SQL
    var sql = "SELECT [LogId], [Timestamp], [CaptureId], [DeviceId], [KnownId] FROM [dbo].[Log]";

    //Execute
    var rows = await db.ExecuteQueryAsync(sql);

    //Result
    return Results.Ok(rows);
});

app.MapPost("/log", async (Database db, Log log) =>
{
    //SQL
    var sql = "INSERT INTO [dbo].[Log] ([LogId],[Timestamp],[CaptureId],[DeviceId],[KnownId]) VALUES (@LogId, @Timestamp, @CaptureId, @DeviceId, @KnownId)";

    //Parameters
    var parameters = new List<SqlParameter>
    {
        new SqlParameter("@LogId", SqlDbType.Int) { Value = log.LogId },
        new SqlParameter("@Timestamp", SqlDbType.Int) { Value = log.Timestamp },
        new SqlParameter("@CaptureId", SqlDbType.Int) { Value = log.CaptureId },
        new SqlParameter("@DeviceId", SqlDbType.Int) { Value = log.DeviceId },
        new SqlParameter("@KnownId", SqlDbType.Int) { Value = log.KnownId }
    };

    //Execute
    var affected = await db.ExecuteNonQueryAsync(sql, parameters);

    //Result
    return affected > 0 ? Results.Created() : Results.BadRequest("Insert Failed");

});

//----- Image

app.MapGet("/img", async (Database db) =>
{
    //SQL
    var sql = "SELECT * FROM [dbo].[Snapshot]";

    //Execute
    var rows = await db.ExecuteQueryAsync(sql);

    //Result
    return Results.Ok(rows);
});

app.MapPost("/image", async (Database db) =>
{
    // SQL
    var sql = "";

    // Execute
    var rows = await db.ExecuteQueryAsync(sql);

    var captureId = Convert.ToInt32(rows[0][""]);
    // Result
    return Results.Ok(new { CaptureId = captureId });
});

app.MapDelete("/image/{id}", async (int id, Database db) =>
{
    // SQL
    var sql = "DELETE FROM [dbo].[Snapshot] WHERE [Id] = @Id";

    // Parameters
    var parameters = new List<SqlParameter>
    {
        new SqlParameter("@Id", SqlDbType.Int) { Value = id }
    };

    // Execute
    var affected = await db.ExecuteNonQueryAsync(sql, parameters);

    // Result
    return affected > 0 ? Results.Ok() : Results.NotFound();
});

app.Run();