using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.Auth
{
    public class LogoutRequest
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}
