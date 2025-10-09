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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");


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
        new SqlParameter("@Timestamp", SqlDbType.DateTime) { Value = log.Timestamp },
        new SqlParameter("@CaptureId", SqlDbType.Int) { Value = log.CaptureId != null ? log.CaptureId : DBNull.Value },
        new SqlParameter("@DeviceId", SqlDbType.Int) { Value = log.DeviceId },
        new SqlParameter("@KnownId", SqlDbType.Int) { Value = log.KnownId }
    };

    //Execute
    var affected = await db.ExecuteNonQueryAsync(sql, parameters);

    //Result
    return affected > 0 ? Results.Created() : Results.BadRequest("Insert Failed");

});

//----- Image

app.MapGet("/image", async (Database db) =>
{
    //SQL
    var sql = "SELECT * FROM [dbo].[Snapshot]";

    //Execute
    var rows = await db.ExecuteQueryAsync(sql);

    //Result
    return Results.Ok(rows);
});

app.MapPost("/image", async (Database db, Snapshot snap) =>
{
    // SQL
    var sql = "INSERT INTO [dbo].[Snapshot] ([FileName],[ContentType],[Data]) VALUES (@FileName, @ContentType, @Data) ";

    //Parameters
    var parameters = new List<SqlParameter>
    {
        new SqlParameter("@FileName", SqlDbType.NVarChar) { Value = snap.FileName },
        new SqlParameter("@ContentType", SqlDbType.NVarChar) { Value = snap.ContentType },
        new SqlParameter("@Data", SqlDbType.VarBinary) { Value = snap.Data },
    };

    // Execute
    var affected = await db.ExecuteNonQueryAsync(sql, parameters);

    // Result
    return affected > 0 ? Results.Created() : Results.BadRequest("Insert Failed");
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