using System;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.Extensions.Configuration;


namespace Horizon.Core.JWT
{
    public class JwtAuthConfigModel
    {
        public static IConfiguration Configuration { get; set; }

        public static JwtAuthConfigModel Model;

        public static JwtAuthConfigModel CreateInstance(IConfiguration configuration)
        {
            if (Model!=null)
            {
                return Model;
            }

            Configuration = configuration;

            return new JwtAuthConfigModel();
        }

        public JwtAuthConfigModel()
        {
            
            try
            {
                JWTSecretKey = Configuration["JwtAuth:SecurityKey"];
                WebExp = double.Parse(Configuration["JwtAuth:WebExp"]);
                AppExp = double.Parse(Configuration["JwtAuth:AppExp"]);
                WxExp = double.Parse(Configuration["JwtAuth:WxExp"]);
                OtherExp = double.Parse(Configuration["JwtAuth:OtherExp"]);
                Issuer = Configuration["JwtAuth:Issuer"];
                Audience = Configuration["JwtAuth:Audience"];
                RefreshTokenAudience = Configuration["JwtAuth:RefreshTokenAudience"];
                RefreshTokenExpiresMinutes = double.Parse(Configuration["JwtAuth:RefreshTokenExpiresMinutes"]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string JWTSecretKey = "lyDqoSIQmyFcUhmmN4KBRGWWzm1ELC7owHVtStOu1YD7wYz";
        /// <summary>
        /// 
        /// </summary>
        public double WebExp = 12;
        /// <summary>
        /// 
        /// </summary>
        public double AppExp = 12;
        /// <summary>
        /// 
        /// </summary>
        public double WxExp = 12;
        /// <summary>
        /// 
        /// </summary>
        public double OtherExp = 12;

        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer = "jwt";

        /// <summary>
        /// 接收者
        /// </summary>
        public string Audience = "jwt";


        public string RefreshTokenAudience = "RefreshTokenAudience";

        public double RefreshTokenExpiresMinutes = 12;
    }
}