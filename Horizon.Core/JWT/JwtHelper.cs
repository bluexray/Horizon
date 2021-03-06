﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Horizon.Core.JWT
{
    public class JwtHelper
    {
        /// <summary>
        /// 颁发JWT字符串 /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public static dynamic BuildJwtToken(TokenModel tokenModel, AccessTokenType type=AccessTokenType.AccessToken)
        {
            var jwtConfig = new JwtAuthConfigModel();
            //过期时间（分钟）
            double exp = 0;

            switch (tokenModel.TokenType)
            {
                case (TokenType)0:
                    exp = jwtConfig.WebExp;
                    break;
                case (TokenType)1:
                    exp = jwtConfig.AppExp;
                    break;
                case (TokenType)2:
                    exp = jwtConfig.WxExp;
                    break;
                case (TokenType)3:
                    exp = jwtConfig.OtherExp;
                    break;
            }
            var dateTime = DateTime.UtcNow;
            var claims = new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid),
                    new Claim("UserName", tokenModel.UserName),//用户名
                    new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") , 
                    //这个就是过期时间，目前是过期100秒，可自定义，注意JWT有自己的缓冲过期时间
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(exp)).ToUnixTimeSeconds()}"),
                    new Claim(JwtRegisteredClaimNames.Iss,jwtConfig.Issuer),
                    new Claim(JwtRegisteredClaimNames.Aud,jwtConfig.Audience),
                    new Claim(ClaimTypes.Role,tokenModel.Role),
                    
               };

           // var expires = dateTime.Add(TimeSpan.FromMinutes(type.Equals(AccessTokenType.AccessToken) ? exp : jwtConfig.TyRefreshTokenExpiresMinutespe));
            //秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.JWTSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience:  jwtConfig.Audience ,//接收者，可以做变化 
                claims: claims,
                expires: dateTime.AddMinutes(exp),
                signingCredentials: creds);
            var jwtHandler = new JwtSecurityTokenHandler();

            var accessToken = jwtHandler.WriteToken(jwt);//生成token

            var refreshToken = "";

            if (type!=AccessTokenType.RefreshToken)
            {
                var refresh = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.RefreshTokenAudience,//接收者，可以做变化
                claims: claims,
                expires: dateTime.AddMinutes(jwtConfig.RefreshTokenExpiresMinutes),
                signingCredentials: creds);
                refreshToken = new JwtSecurityTokenHandler().WriteToken(refresh); 
            }



            var responseJson = new
            {
                success = true,
                access_token = accessToken,
                expires_in = dateTime.AddMinutes(exp),
                token_type = "Bearer",
                refres_token= refreshToken,
                expires_out=dateTime.AddMinutes(jwtConfig.RefreshTokenExpiresMinutes)
            };

            return responseJson;
        }


        public static dynamic RefreshToken(string token)
        {

            var tm = new TokenModel();
            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                object type = "";
                object userName = "";
                object role = "";
                object project = "";
                object tokentype = "";
                jwt.Payload.TryGetValue("GrandType", out type);
                jwt.Payload.TryGetValue("UserName", out userName);
                jwt.Payload.TryGetValue("Role", out role);
                jwt.Payload.TryGetValue("Project", out project);
                jwt.Payload.TryGetValue("TokenType", out tokentype);

                if (type.ToString().ToLower()=="refreshtoken")
                {
                    
                    tm.Uid = jwt.Id;
                    tm.GrandType = type.ToString();
                    tm.Role = role.ToString();
                    tm.Project = project.ToString();
                    tm.TokenType = (TokenType)Enum.Parse(typeof(TokenType),tokentype.ToString());//mark 


                  return  BuildJwtToken(tm, AccessTokenType.RefreshToken);
                }


            }
            catch (Exception)
            {

                throw;
            }

            return null;
            //验证refresh token
            //生成新的access token
            //return JwtHelper.BuildJwtToken(tm);
        }

        /// <summary>
        /// 解析token,未处理错误Token
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static TokenModel SerializeJWT(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            
            object role = new object();
            object userName = new object();
            try
            {
                jwtToken.Payload.TryGetValue("UserName", out userName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var tm = new TokenModel
            {
                Uid = jwtToken.Id,
                UserName = userName.ToString()
            };
            return tm;
        }

        public bool IsValidRefreshToken(string refreshToken)
        {

            try
            {
                var refresh = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken);
                object userName;
           
                    refresh.Payload.TryGetValue("UserName", out userName);
                
            }
            catch (Exception e)
            {

                throw;
            }
            
            return false;
        }
    }



    /// <summary>
    /// 令牌
    /// </summary>
    public class TokenModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 身份
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// 令牌类型
        /// </summary>

        public TokenType TokenType;

        /// <summary>
        /// AccessToken,RefreshToken
        /// </summary>
        public string GrandType { get; set; }
    }


    public enum TokenType
    {
        Web=0,
        App=1,
        WeChat=2,
        Other=3
    }

    public class ComplexToken
    {
        public TokenModel AccessToken { get; set; }
        public TokenModel RefreshToken { get; set; }
    }

    public enum AccessTokenType
    {
        AccessToken=0,
        RefreshToken=1
    }

}
