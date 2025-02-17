using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Models
{
    public class LoginModel
    {
        public string userName { get; set; }

        public string Password { get; set; }
    }
}
