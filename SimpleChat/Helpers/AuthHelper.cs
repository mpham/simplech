using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using SimpleChat.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SimpleChat.Helpers
{
    public class AuthHelper
    {
        public static string CreateToken(int id)
        {
            // set expiration to 6 hours
            IDateTimeProvider provider = new UtcDateTimeProvider();
            DateTime now = provider.GetNow();
            now = now.AddHours(6);
            // convert to seconds since 1/1/1970 UTC
            var expirationInSecondsSinceEpoch = Math.Round((now - JwtValidator.UnixEpoch).TotalSeconds);

            var payload = new Dictionary<string, object>
            {
                { "id", id.ToString() },
                { "exp", expirationInSecondsSinceEpoch }
            };

            var secret = ConfigurationManager.AppSettings["jwt.secret"];

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, secret);
            return token;
        }

        public static object DecodeToken(string token)
        {
            var secret = ConfigurationManager.AppSettings["jwt.secret"];

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                IDictionary<string, object> payload = decoder.DecodeToObject(token, secret, true);
                return payload;
            }
            catch(TokenExpiredException)
            {
                return "Token expired";
            }
            catch(SignatureVerificationException)
            {
                return "Invalid signature";
            }
        }
    }
}