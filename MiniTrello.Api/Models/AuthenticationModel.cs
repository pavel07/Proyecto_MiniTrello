using System;

namespace MiniTrello.Api.Models
{
    public class AuthenticationModel
    {
        public int Status { get; set; }
        public string Token { get; set; }
        public TimeSpan AvailableTime { get; set; }
    }
}