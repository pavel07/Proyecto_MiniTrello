using System;

namespace MiniTrello.Api.Models
{
    public class AuthenticationModel
    {
        public string Token { get; set; }
        public int AvailableTime { get; set; }
    }
}