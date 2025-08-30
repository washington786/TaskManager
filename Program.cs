using Microsoft.AspNetCore.Identity;
using TaskManager.Services;
using TaskManager.Utils;

var builder = WebApplication.CreateBuilder(args);

// map service
builder.Services.AddScoped<TokenService>();
// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var connections = new Helper(builder.Services, builder.Configuration);

connections.SwaggerConfig();

connections.DbConnectionCtx();
connections.IdentityAdding();
connections.AddCors();
connections.AddAuthentication();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI();
    app.UseSwagger();

    app.UseCors("dev");
}
else
{
    app.UseCors("prod");
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// seeding
using (var scope = app.Services.CreateAsyncScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = ["Admin", "User", "TaskManager"];

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}


app.Run();