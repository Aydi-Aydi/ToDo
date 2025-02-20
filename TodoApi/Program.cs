using TodoApi;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using AuthServer.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);

//דאגה למשתנה הסביבה

var connectionString = Environment.GetEnvironmentVariable("connection_string") 
    ?? builder.Configuration.GetConnectionString("ToDo");


builder.Services.AddDbContext<ToDoDbContext>(option => option.UseMySql(connectionString,
        Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql")));

builder.Services.AddEndpointsApiExplorer();
var _configuration = builder.Configuration;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
        Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

//כנ"ל מברוק כתבה, צריך לבדוק שתקין:
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
    policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);

    app.UseSwagger();
    app.UseSwaggerUI();

//גם ממברוק
app.UseAuthentication();
app.UseAuthorization();




object CreateJWT(User user)
{
    var claims = new List<Claim>()
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("name", user.UserName),
                };
#pragma warning disable CS8604 // Possible null reference argument.
    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Key")));
#pragma warning restore CS8604 // Possible null reference argument.
    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    var tokeOptions = new JwtSecurityToken(
        issuer: _configuration.GetValue<string>("JWT:Issuer"),
        audience: _configuration.GetValue<string>("JWT:Audience"),
        claims: claims,
        expires: DateTime.Now.AddDays(30),
        signingCredentials: signinCredentials
    );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
    return new { Token = tokenString };
}
//לוגין - קוד מברוק
//חסר לנו: קריאט ג'ט
app.MapPost("/login", (ToDoDbContext dbContext, [FromBody] LoginModel loginModel) =>
{
    
    var user = dbContext.Users?.FirstOrDefault(u => u.UserName == loginModel.userName && u.Password == loginModel.Password);
    if (user is not null)
    {
        var jwt = CreateJWT(user);
        return Results.Ok(jwt);
    }
    return Results.Unauthorized();
});

//הרשמה - רגיסטר - 
app.MapPost("/register", (ToDoDbContext dbContext, [FromBody] LoginModel loginModel) =>
{
    var name = loginModel.userName;
    var lastId = dbContext.Users?.Max(u => u.Id) ?? 0;
    var newUser = new User { Id = lastId + 1, UserName = name, Password = loginModel.Password };
    dbContext.Users?.Add(newUser);
    dbContext.SaveChanges();
    var jwt = CreateJWT(newUser);
    return Results.Ok(jwt);
});
app.MapGet("/", () => "Helloההההההההההה!");
app.MapGet("/hy", () => "איידי, זה פשוט רץ לי... ה' תודה");
app.MapGet("/tasks", [Authorize] async(ToDoDbContext dbContext) => dbContext.Items);

app.MapPost("/tasks", [Authorize] async (ToDoDbContext dbContext, [FromBody] CreateItemRequest request) =>
{
    if (string.IsNullOrEmpty(request.Name))
    {
        return Results.BadRequest("Name is required.");
    }

    Item item = new Item
    {
        Name = request.Name,
        IsComplete = false
    };

    dbContext.Items.Add(item);
    await dbContext.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapDelete("/tasks/{ID}", [Authorize] async (ToDoDbContext dbContext, int ID) =>
{
    var item = await dbContext.Items.FindAsync(ID);
    if (item != null)
    {
        dbContext.Items.Remove(item);
        await dbContext.SaveChangesAsync();
        return Results.Ok();
    }
    return Results.NotFound();
});

app.MapPut("/tasks/{ID}", [Authorize] async (ToDoDbContext dbContext, int ID, [FromBody] bool IsComplete) =>
{
    var item = await dbContext.Items.FindAsync(ID);
    if (item != null)
    {
        item.IsComplete = IsComplete; // עדכן את ה-IsComplete ישירות
        await dbContext.SaveChangesAsync();
        return Results.Ok(item);
    }
    return Results.NotFound();
});

app.Run();

