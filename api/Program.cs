//------------------- Settings ----------------------

using api;
using Microsoft.Data.SqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "";

builder.Services.AddSingleton(new Database(connectionString));

var app = builder.Build();

//----------------- API Endpoints -------------------

app.MapGet("/", () => "Take Them Down API is Running");

app.MapGet("/logs", async (Database db) =>
{
    //SQL
    var sql = "";

    //Execute
    var rows = await db.ExecuteQueryAsync(sql);

    //Result
    return Results.Ok(rows);
});

app.MapPost("/log", async (Database db) =>
{
    //SQL
    var sql = "";

    //Parameters
    var parameters = new List<SqlParameter>
    {
        new SqlParameter("@Name", SqlDbType.NVarChar) { Value = "Alice" },
        new SqlParameter("@Email", SqlDbType.NVarChar) { Value = "alice@example.com" }
    };

    //Execute
    var affected = await db.ExecuteNonQueryAsync(sql, parameters);

    //Result
    return affected > 0 ? Results.Created() : Results.BadRequest("Insert Failed");

});

app.Run();
