﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class AddOrganizationResponseModel
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public AddOrganizationResponseModel(string Title, string Message)
        {
            this.Title = Title;
            this.Message = Message;
        }
    }
}