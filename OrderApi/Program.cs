using System.Reflection;
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
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;
using OrderApi.SignalR.Services;
using OrderApi.SignalR.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using OrderApi.Controllers;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Hosting;
using OrderApi.CQRS.Data;
using MediatR;
using OrderApi.CQRS.Models;
using OrderApi.CQRS.Resources.Commands;
using OrderApi.CQRS.Resources.Queries;
using OrderApi.Filters;

public class Program
{
    public const string CustomTokenScheme = nameof(CustomTokenScheme);
    public const string CustomCookieScheme = nameof(CustomCookieScheme);
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
       

        builder.Services.AddSwaggerGen(c =>
        {
            // Other Swagger configuration...

            c.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "Duration"
            });
        });
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
       
        builder.Services.AddAuthorization(c =>
        {
            c.AddPolicy("Cookie", pb => pb
                .AddAuthenticationSchemes("Cookie")
                .RequireAuthenticatedUser());

            c.AddPolicy("Token", pb => pb
                // schema get's ignored in signalr
                .AddAuthenticationSchemes(CustomTokenScheme)
                .RequireClaim("token")
                .RequireAuthenticatedUser());
        });
        builder.Services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        ;
        builder.Services.AddDbContext<AppOrderContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
        });
        //builder.Services.AddDbContext<SignalRContext>(options =>
        //{
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("SignalRConnection"));
        //});
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddAutoMapper(typeof(Program));
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        //builder.Services.AddSignalR();
        //builder.Services.AddHostedService<MyBackgroundService>();

        //builder.Services.AddSingleton<ChatRegistry>();
        //builder.Services.AddScoped<OrderApi.SignalR.Services.NotificationRegistry>();
        //builder.Services.AddScoped(typeof(ConnectionMapping));
        //builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
        //builder.Services.AddSingleton<ShareDb>();
        //builder.Services.AddCors();
        //builder.Services.AddSingleton<INotificationSink, NotificationService>();
        //builder.Services.AddHostedService(sp => (NotificationService)sp.GetService<INotificationSink>());
        //builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<SignalRContext>();
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
        });
        builder.Services.AddScoped<ApiResponse>();
        //builder.Services.AddScoped<ChatService>();

        //_____________________________________SQRS______________________________________

        //builder.Services.AddSingleton<ProductContext>();
        //builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        builder.Services.AddScoped<ResourceFilter>();
        builder.Services.AddScoped<CustomActionFilter>();

        var app = builder.Build();


        //app.MapControllers();
        app.UseCors(options => options
            .WithOrigins("http://localhost:3000") // You can also specify a specific origin
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseHttpsRedirection();
        //app.Use((ctx, next) =>
        //{
        //    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
        //    var protector = idp.CreateProtector("auth-cookie");
        //    var dataFromHeader =
        //        ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith(".AspNetCore.Cookie="));
        //    Console.WriteLine("DataFromHeader_________________"+dataFromHeader);
        //    var splitted = dataFromHeader?.Split("=").Last();
        //    var claims = ctx.User.Claims;
        //    var unprotected = splitted != null ? protector?.Unprotect(splitted) : null;
        //    Console.WriteLine("___________________________User Claims:");
        //    Console.WriteLine(unprotected);
        //    return next();
        //});
       
        app.UseAuthentication();
        app.UseAuthorization();

        //app.MapHub<ChatHub>("/chat");
        //app.MapHub<ProtectedHub>("/protected");
        //app.MapHub<NotificationHub>("/notificationHub");
        
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
                new Claim(ClaimTypes.NameIdentifier, "bob"),
                new Claim("name", "bob"),
                new Claim("hobbie", "football"),
            };
            var identity = new ClaimsIdentity(claims,"Cookie");
            var user = new ClaimsPrincipal(identity);
            await ctx.SignInAsync("Cookie", user);

        });
        app.MapGet("/", ctx =>
        {
            var claims = ctx.User.Claims;

            Console.WriteLine("User Claims: | " +
                              ctx.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            foreach (var claim in claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }

            return Task.FromResult(1);
        });
        //.RequireAuthorization("Cookie");
        //app.Map("/cookie", ctx =>
        //{
        //    ctx.Response.StatusCode = 200;
        //    return ctx.Response.WriteAsync(ctx.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value);
        //}).RequireAuthorization("Cookie");


        //_____________________________________CQRS______________________________________

        //app.MapGet("/test", c =>
        //{
        //    Console.WriteLine("Test");
        //    return Task.FromResult(0);
        //});
        //app.MapGet("/product/get-by-id/{id:int}", async (IMediator _mediator, int id) =>
        //{
        //    try
        //    {
        //        var command = new GetProductByIdQuery() { Id = id };
        //        var response = await _mediator.Send(command);
        //        return response is not null ? Results.Ok(response) : Results.NotFound();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Results.BadRequest(ex.Message);
        //    }
        //});

        //app.MapPost("product/create", async (IMediator _mediator, Product product) =>
        //{
        //    try
        //    {
        //        var command = new CreateProductCommand()
        //        {
        //            Name = product.Name,
        //            Description = product.Description,
        //            Category = product.Category,
        //            Price = product.Price,
        //            Active = product.Active,
        //        };
        //        var response = await _mediator.Send(command);
        //        return response is not null ? Results.Ok(response) : Results.NotFound();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Results.BadRequest(ex.Message);
        //    }
        //});


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
