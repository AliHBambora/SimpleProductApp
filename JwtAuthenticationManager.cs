using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace ProductApp
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly string key;
        private readonly string Username;
        private readonly string Password;



        private readonly IConfiguration Configuration;

        public JwtAuthenticationManager(string key, string Username,string Password)
        {
            this.key = key;
            this.Username = Username;
            this.Password = Password;

        }

        public JwtAuthenticationManager(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        

        public string[] Authenticate(string user, string password)
        {
            List<string> list = new List<string>();
            try {
                if(user==Username  && password == Password)
                {
                    var tokenhandler = new JwtSecurityTokenHandler();
                    var tokenkey = Encoding.ASCII.GetBytes(key);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                    new Claim(ClaimTypes.Name, user),
                    new Claim("Pass", password)
                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenhandler.CreateToken(tokenDescriptor);
                    list.Add("success");
                    list.Add(tokenhandler.WriteToken(token));
                    return list.ToArray();
                }
                else
                {
                    list.Add("fail");
                    list.Add("Authentication failed.Username and password are incorrect");
                    return list.ToArray();
                }
                
            }
            catch(Exception e)
            {
                list.Add("fail");
                list.Add(e.Message);
                return list.ToArray();
            }
        }
    }
}
