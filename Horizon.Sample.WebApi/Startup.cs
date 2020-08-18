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
            

            //����
            services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            //ע��jwt,���JWT Scheme
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
                    ValidateIssuer = true, //�Ƿ���֤Issuer
                    ValidateAudience = true, //�Ƿ���֤Audience
                    ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey
                    ValidateLifetime = true, //�Ƿ���֤��ʱ  ������exp��nbfʱ��Ч ͬʱ����ClockSkew 
                    ClockSkew = TimeSpan.FromSeconds(30), //ע�����ǻ������ʱ�䣬�ܵ���Чʱ��������ʱ�����jwt�Ĺ���ʱ�䣬��������ã�Ĭ����5����
                    ValidAudience = jwtConfig.Audience, //Audience
                    ValidIssuer = jwtConfig.Issuer, //Issuer���������ǰ��ǩ��jwt������һ��
                    RequireExpirationTime = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["JwtAuth:SecurityKey"])) //�õ�SecurityKey
                };
                o.Events = new JwtBearerEvents
                {
                    //��֤ʧ�ܺ�ֹͣ��Ӧ
                    OnChallenge = p =>
                    {
                        p.HandleResponse();


                        var payload = "{\"Success\":false,\"Msg\":\"�ܱ�Ǹ������Ȩ���ʸýӿ�\",\"StatusCode\":401}";
                        //�Զ��巵�ص���������
                        p.Response.ContentType = "application/json";
                        //�Զ��巵��״̬�룬Ĭ��Ϊ401 ������ĳ� 200
                        p.Response.StatusCode = 200;
                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //���Json���ݽ��
                        p.Response.WriteAsync(payload);
                        return Task.FromResult(0);
                    },
                    OnAuthenticationFailed = context =>
                    {
                        // ������ڣ����<�Ƿ����>��ӵ�������ͷ��Ϣ��
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

            //�����֤
            app.UseAuthentication();
            //��Ȩ
            app.UseAuthorization();


            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
