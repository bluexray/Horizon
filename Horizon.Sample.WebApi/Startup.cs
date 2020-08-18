using Horizon.Core.JWT;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;
using Horizon.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using SkyApm.Utilities.DependencyInjection;

namespace Horizon.Sample.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtAuthConfigModel.CreateInstance(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSkyApmExtensions();//add track


            services.AddHorizonORM(Configuration);
            

            //跨域
            services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            //注入jwt,添加JWT Scheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                var jwtConfig = new JwtAuthConfigModel();
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, //是否验证Issuer
                    ValidateAudience = true, //是否验证Audience
                    ValidateIssuerSigningKey = true, //是否验证SecurityKey
                    ValidateLifetime = true, //是否验证超时  当设置exp和nbf时有效 同时启用ClockSkew 
                    ClockSkew = TimeSpan.FromSeconds(30), //注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
                    ValidAudience = jwtConfig.Audience, //Audience
                    ValidIssuer = jwtConfig.Issuer, //Issuer，这两项和前面签发jwt的设置一致
                    RequireExpirationTime = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["JwtAuth:SecurityKey"])) //拿到SecurityKey
                };
                o.Events = new JwtBearerEvents
                {
                    //验证失败后停止响应
                    OnChallenge = p =>
                    {
                        p.HandleResponse();


                        var payload = "{\"Success\":false,\"Msg\":\"很抱歉，您无权访问该接口\",\"StatusCode\":401}";
                        //自定义返回的数据类型
                        p.Response.ContentType = "application/json";
                        //自定义返回状态码，默认为401 我这里改成 200
                        p.Response.StatusCode = 200;
                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //输出Json数据结果
                        p.Response.WriteAsync(payload);
                        return Task.FromResult(0);
                    },
                    OnAuthenticationFailed = context =>
                    {
                        // 如果过期，则把<是否过期>添加到，返回头信息中
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            app.UseRouting();

            //身份验证
            app.UseAuthentication();
            //授权
            app.UseAuthorization();


            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
