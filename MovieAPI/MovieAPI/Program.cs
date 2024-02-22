using MovieAPI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.ConfigureDbContext();
builder.ConfigureIdentity();
builder.ConfigureAuthentication();
builder.ConfigureServices();





WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(options =>
            options.WithOrigins("*").
            AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
