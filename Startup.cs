using AutoMapper;
using System.Net;
using System.Text;
using Web.Commons.Config;
using Microsoft.OpenApi.Models;
using MayMayShop.API.Repos;
using Microsoft.AspNetCore.Http;
using MayMayShop.API.Context;
using MayMayShop.API.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MayMayShop.API.Interfaces.Repos;
using MayMayShop.API.Const;
using MayMayShop.API.Facade;
using System.Collections.Generic;
using Newtonsoft.Json;
using MayMayShop.API.Interfaces.Services;
using MayMayShop.API.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;
using System;

namespace MayMayShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // try
            // {
                var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(logRepository, new FileInfo(Configuration.GetSection("appSettings:log4netFile").Value));

                AppConfig appConfig = new AppConfig(MayMayShopConst.PARAM_APPLICATION);
                appConfig.Load(Configuration.GetSection("appSettings:appHome").Value
                    , Configuration.GetSection("appSettings:appSettingFile").Value);
                ConfigFacade.ApplicationConfig = appConfig;
                MayMayShopConst.loadConfigData();

                services.AddDbContext<MayMayShopContext>(x => x.UseSqlServer
                    (MayMayShopConst.DB_CONNECTION));

                services.AddControllers().AddNewtonsoftJson(opt => {
                    opt.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MayMayShop API", Version = "v1",Description="Swagger for MayMayShop system authorized by Myanmar High Society" });
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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

                services.AddCors();
        
                CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
                context.LoadUnmanagedLibrary(Directory.GetCurrentDirectory() + "\\Library\\libwkhtmltox.dll");
                services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));            

                services.AddAutoMapper(typeof(Startup));
                services.AddScoped<IMiscellaneousRepository, MiscellaneousRepository>();
                services.AddScoped<IProductRepository, ProductRepository>();
                services.AddScoped<IOrderRepository, OrderRepository>();
                services.AddScoped<IMayMayShopServices, MayMayShopServices>();
                services.AddScoped<IPaymentGatewayServices, PaymentGateWayServices>();
                services.AddScoped<IUserServices, UserServices>();
                services.AddScoped<IDeliveryService, DeliveryService>();
                services.AddScoped<IMemberPointServices, MemberPointServices>();
                services.AddScoped<IMemberPointRepository, MemberPointRepository>();
                services.AddScoped<IReportRepository, ReportRepository>();
                
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateLifetime =MayMayShopConst.TOKEN_VALIDATELIFETIME,
                            ValidateIssuerSigningKey =MayMayShopConst.TOKEN_VALIDATEISSUERSIGNINGKEY,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                                .GetBytes(MayMayShopConst.TOKEN_SECRET)),
                            ValidateIssuer =MayMayShopConst.TOKEN_VALIDATEISSUER,
                            ValidateAudience =MayMayShopConst.TOKEN_VALIDATEAUDIENCE,
                            ValidIssuer =MayMayShopConst.TOKEN_ISSUER,
                        };
                    });

                services.AddScoped<ActionActivity>();
                services.AddScoped<ActionActivityLog>();

                services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

                services.AddControllers();
                services.AddHttpContextAccessor();
                
            // } catch (Exception e)
            // {
            //     log.Error(e.Message);
            // }
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {           
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "API");
                c.RoutePrefix = "swagger";                
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            app.UseRouting();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
