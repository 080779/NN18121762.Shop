using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace IMS.Common
{
    public static class JwtHelper
    {
        private static string secret = System.Configuration.ConfigurationManager.AppSettings["TokenSecret"];
        /// <summary>
        /// jwt加密
        /// </summary>
        /// <returns></returns>
        public static string JwtEncrypt(Dictionary<string, object> payload)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            string token = encoder.Encode(payload, secret);
            return token;
        }
        /// <summary>
        /// jwt加密
        /// </summary>
        /// <returns></returns>
        public static string JwtEncrypt<T>(T obj)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            Type type = obj.GetType();
            PropertyInfo[] infos = type.GetProperties();
            foreach (var info in infos)
            {
                payload.Add(info.Name, info.GetValue(obj));
            }
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            string token = encoder.Encode(payload, secret);
            return token;
        }
        /// <summary>
        /// jwt解密
        /// </summary>
        /// <returns></returns>
        public static bool JwtDecrypt(string token, out string res)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
                var json = decoder.Decode(token, secret, verify: true);
                res = json;
                return true;
            }
            catch (TokenExpiredException)
            {
                res = "Token已经过期";
                return false;
            }
            catch (SignatureVerificationException)
            {
                res = "无效的签名token";
                return false;
            }
        }

        /// <summary>
        /// jwt解密
        /// </summary>
        /// <returns></returns>
        public static T JwtDecrypt<T>(HttpControllerContext context)
        {
            IEnumerable<string> values;
            context.Request.Headers.TryGetValues("token", out values);
            string token = values.First();
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
                var json = decoder.Decode(token, secret, verify: true);
                return serializer.Deserialize<T>(json);
            }
            catch (TokenExpiredException)
            {
                return default(T);
            }
            catch (SignatureVerificationException)
            {
                return default(T);
            }
        }
    }
}
