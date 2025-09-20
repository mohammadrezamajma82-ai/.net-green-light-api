using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class GreenlightController : ControllerBase
{
    private readonly string sshHost = "2.179.167.151";
    private readonly string sshUser = "root";
    private readonly string sshPath = "./sshkey";

    private readonly string dbHost = "127.0.0.1";
    private readonly int dbPort = 5432;
    private readonly string dbName = "greenlight-v3-production";
    private readonly string dbUser = "php_ext_access_user";
    private readonly string dbPass = "r1u;+&:Y39hN";

    [HttpGet("rooms")]
    public IActionResult GetRooms()
    {
        var helper = new SshPostgresHelper();
        var rooms = new List<object>(); 

        using var conn = helper.Connect(sshHost,sshUser,sshPath,dbHost, dbPort, dbName, dbUser, dbPass);

        using var cmd = new NpgsqlCommand("SELECT id, name, owner_id FROM rooms;", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            rooms.Add(new { id = reader["id"], name = reader["name"], owner_id = reader["owner_id"] });
        }

        helper.Disconnect();
        return Ok(rooms);
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var helper = new SshPostgresHelper();
        var users = new List<object>();

        using var conn = helper.Connect(sshHost, sshUser,sshPath, dbHost, dbPort, dbName, dbUser, dbPass);

        using var cmd = new NpgsqlCommand("SELECT id, name, email FROM users;", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new { id = reader["id"], name = reader["name"], email = reader["email"] });
        }

        helper.Disconnect();
        return Ok(users);
    }
}
