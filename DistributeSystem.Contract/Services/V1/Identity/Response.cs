using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Contract.Services.V1.Identity
{
    public static class Response
    {
        public class Authenicated
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
            public DateTime RefreshTokenExpiryTime { get; set; }
        }
    }
}
