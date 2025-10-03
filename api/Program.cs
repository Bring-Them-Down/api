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

app.MapGet("/logs", async (Database db) =>
{
    //SQL
    var sql = "SELECT [KnownId], [IsAllied] FROM [dbo].[Known]";

    //Execute
    var rows = await db.ExecuteQueryAsync(sql);

    //Result
    return Results.Ok(rows);
});

app.MapPost("/log", async (Database db, Known known) =>
{
    //SQL
    var sql = "INSERT INTO [dbo].[Known] ([KnownId],[IsAllied]) VALUES (@KnownId, @IsAllied)";

    //Parameters
    var parameters = new List<SqlParameter>
    {
        new SqlParameter("@KnownId", SqlDbType.Int) { Value = known.KnownId },
        new SqlParameter("@IsAllied", SqlDbType.Int) { Value = known.IsAllied }
    };

    //Execute
    var affected = await db.ExecuteNonQueryAsync(sql, parameters);

    //Result
    return affected > 0 ? Results.Created() : Results.BadRequest("Insert Failed");

});

//----- Image

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

app.Run();