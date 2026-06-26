using Microsoft.EntityFrameworkCore;
using SportEquipment.Mvc.Data;
using SportEquipment.Mvc.Options;
using SportEquipment.Mvc.Repositories;
using SportEquipment.Mvc.Services;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Cấu hình Serilog tự động ghi ra file txt mỗi ngày
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/SportHub-Log-.txt", rollingInterval: RollingInterval.Day));

// Đăng ký ProblemDetails cho API
builder.Services.AddProblemDetails(options => {
    options.CustomizeProblemDetails = context => {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;
    };
});

// Đăng ký Health Check (Kiểm tra app và database)
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Application is running."), tags: new[] { "live" })
    .AddDbContextCheck<SportEquipment.Mvc.Data.AppDbContext>("database", tags: new[] { "ready" });

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký cấu hình Options Pattern
builder.Services.Configure<SportHubSettings>(builder.Configuration.GetSection("SportHubSettings"));

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // Đổi chữ UseSqlServer thành UseSqlite

// Đăng ký Repository và Service dưới dạng Scoped (vòng đời theo từng Request)
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Kích hoạt đường dẫn /health/live và /health/ready
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions {
    Predicate = check => check.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions {
    Predicate = check => check.Tags.Contains("ready")
});

// Tạo API test lỗi ProblemDetails
app.MapGet("/api/equipments/{id:int}", async (int id, SportEquipment.Mvc.Data.AppDbContext db, HttpContext http) => {
    var equipment = await db.Equipments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    if (equipment == null) {
        return Results.Problem(
            type: "https://example.com/problems/equipment-not-found",
            title: "Equipment not found",
            detail: $"Dụng cụ với ID {id} không tồn tại.",
            statusCode: StatusCodes.Status404NotFound,
            instance: http.Request.Path);
    }
    return Results.Ok(equipment);
});

app.Run();
