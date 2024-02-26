using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderApi;
using OrderApi.Data;
using OrderApi.Repositories;
using OrderApi.Services;
using OrderApi.SignalR;
using System.Security.Claims;
using System.Text.Encodings.Web;
using OrderApi.SignalR.Services;
using OrderApi.SignalR.Services.Data;

public class Program
{
    public const string CustomTokenScheme = nameof(CustomTokenScheme);
    public const string CustomCookieScheme = nameof(CustomCookieScheme);
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthentication("Cookie")
             .AddCookie("Cookie")
             //.AddScheme<AuthenticationSchemeOptions, CustomCookie>(CustomCookieScheme, _ =>
             //{
             //})
            .AddJwtBearer(CustomTokenScheme, o =>
            {
                o.Events = new()
                {
                    OnMessageReceived = (context) =>
                    {
                        var path = context.HttpContext.Request.Path;
                        if (path.StartsWithSegments("/protected")
                            || path.StartsWithSegments("/token"))
                        {
                            var accessToken = context.Request.Query["access_token"];

                            if (!string.IsNullOrWhiteSpace(accessToken))
                            {
                                // context.Token = accessToken;

                                var claims = new Claim[]
                                {
                                    new("user_id", accessToken),
                                    new("token", "token_claim"),
                                };
                                var identity = new ClaimsIdentity(claims, CustomTokenScheme);
                                context.Principal = new(identity);
                                context.Success();
                            }
                        }

                        return Task.CompletedTask;
                    },
                };
            }); 
        //builder.Services.AddAuthorization(c =>
        //{
        //    c.AddPolicy("Cookie", pb => pb
        //        .AddAuthenticationSchemes(CustomCookieScheme)
        //        .RequireAuthenticatedUser());

        //    c.AddPolicy("Token", pb => pb
        //        // schema get's ignored in signalr
        //        .AddAuthenticationSchemes(CustomTokenScheme)
        //        .RequireClaim("token")
        //        .RequireAuthenticatedUser());
        //});
        builder.Services.AddControllers();
        builder.Services.AddDbContext<AppOrderContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
        });
        builder.Services.AddDbContext<SignalRContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("SignalRConnection"));
        });
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddAutoMapper(typeof(Program));
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSignalR();
        //builder.Services.AddHostedService<MyBackgroundService>();

        builder.Services.AddSingleton<ChatRegistry>();
        builder.Services.AddScoped<OrderApi.SignalR.Services.NotificationRegistry>();
        builder.Services.AddScoped(typeof(ConnectionMapping));
        builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
        builder.Services.AddSingleton<ShareDb>();
        builder.Services.AddCors();
        builder.Services.AddSingleton<INotificationSink, NotificationService>();
        builder.Services.AddHostedService(sp => (NotificationService)sp.GetService<INotificationSink>());


        var app = builder.Build();

        app.UseCors(options => options
            .WithOrigins("http://localhost:5173") // You can also specify a specific origin
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
       
       
        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseSwagger();
        //    app.UseSwaggerUI();
        //}
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<ChatHub>("/chat");
        app.MapHub<ProtectedHub>("/protected");
        app.MapHub<NotificationHub>("/notificationHub");
       
        app.Map("/token", ctx =>
        {
            ctx.Response.StatusCode = 200;
            return ctx.Response.WriteAsync(ctx.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value);
        }).RequireAuthorization("Token");
        app.MapGet("/get-cookie",async  ctx =>
        {
            //ctx.Response.StatusCode = 200;
            //ctx.Response.Cookies.Append("signalr-auth-cookie", Guid.NewGuid().ToString(), new()
            //{
            //    Expires = DateTimeOffset.UtcNow.AddSeconds(6000)
            //});
            string userName = ctx.Request.Query["userName"];
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userName),
            };
            var identity = new ClaimsIdentity(claims,"Cookie");
            var user = new ClaimsPrincipal(identity);
            await ctx.SignInAsync("Cookie", user);

        });
        //app.Map("/cookie", ctx =>
        //{
        //    ctx.Response.StatusCode = 200;
        //    return ctx.Response.WriteAsync(ctx.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value);
        //}).RequireAuthorization("Cookie");

        app.MapControllers();

        app.Run();
    }
    public class CustomCookie : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomCookie(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Context.Request.Cookies.TryGetValue("signalr-auth-cookie", out var cookie))
            {
                var claims = new Claim[]
                {
                    new("user_id", cookie),
                    new("cookie", "cookie_claim"),
                };
                var identity = new ClaimsIdentity(claims, CustomCookieScheme);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, new(), CustomCookieScheme);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("signalr-auth-cookie not found"));
        }
    }
}
