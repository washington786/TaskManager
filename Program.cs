using TaskManager.Utils;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();