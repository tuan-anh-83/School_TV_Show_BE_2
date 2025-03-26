using BOs.Data;
using BOs.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Net.payOS;
using Repos;
using Services;
using Services.CloudFlareService;
using Services.Email;
using Services.HostedServices;
using Services.MiddleWare;
using Services.SwaggerConfig;
using Services.Token;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 🛠 Cấu hình Swagger với hỗ trợ Bearer Token
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "School_TV_Show API",
        Version = "v1",
        Description = "API for School TV Show project with JWT Authentication"
    });
    c.OperationFilter<FileUploadOperationFilter>();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer YOUR_TOKEN_HERE')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

var payOSConfig = builder.Configuration.GetSection("Environment");
var clientId = payOSConfig["PAYOS_CLIENT_ID"];
var apiKey = payOSConfig["PAYOS_API_KEY"];
var checksumKey = payOSConfig["PAYOS_CHECKSUM_KEY"];

builder.Services.AddSingleton(new PayOS(clientId, apiKey, checksumKey));
// Repositories
builder.Services.AddScoped<IScheduleRepo, ScheduleRepo>();
builder.Services.AddScoped<IProgramRepo, ProgramRepo>();
builder.Services.AddScoped<IReportRepo, ReportRepo>();
builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<IVideoRepo, VideoRepo>();
builder.Services.AddScoped<IVideoViewRepo, VideoViewRepo>();
builder.Services.AddScoped<IVideoLikeRepo, VideoLikeRepo>();
builder.Services.AddScoped<IShareRepo, ShareRepo>();
builder.Services.AddScoped<IPackageRepo, PackageRepo>();
builder.Services.AddScoped<ICommentRepo, CommentRepo>();
builder.Services.AddScoped<ISchoolChannelRepo, SchoolChannelRepo>();
builder.Services.AddScoped<IPaymentRepo>(provider =>
    new PaymentRepo(clientId, apiKey, checksumKey));
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderDetailRepo, OrderDetailRepo>();
builder.Services.AddScoped<INewsRepo, NewsRepo>();
builder.Services.AddScoped<ISchoolChannelFollowRepo, SchoolChannelFollowRepo>();
builder.Services.AddScoped<IPaymentHistoryRepo, PaymentHistoryRepo>();

// Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISchoolChannelService, SchoolChannelService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<IVideoViewService, VideoViewService>();
builder.Services.AddScoped<IVideoLikeService, VideoLikeService>();
builder.Services.AddScoped<IShareService, ShareService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
//builder.Services.AddHttpClient<ILiveStreamService, LiveStreamService>();
builder.Services.AddScoped<ISchoolChannelFollowService, SchoolChannelFollowService>();
builder.Services.AddScoped<IPaymentHistoryService, PaymentHistoryService>();
builder.Services.AddHostedService<PendingAccountReminderService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
//builder.Services.AddSingleton<OrderTrackingService>();
//builder.Services.AddHostedService<LiveStreamScheduler>();


builder.Services.AddDistributedMemoryCache();

// Cloudflare configuration
builder.Services.Configure<CloudflareSettings>(builder.Configuration.GetSection("Cloudflare"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Register IConfiguration
IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

// Configure DbContext
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin() // Cho phép tất cả các nguồn
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
})
.AddCookie("ExternalCookie", options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;
})
.AddGoogle(options =>
{
    options.SignInScheme = "ExternalCookie";
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/api/accounts/google-response";
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAllOrigins"); // Apply CORS policy
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers();
app.Run();
