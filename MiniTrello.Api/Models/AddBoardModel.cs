﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class AddBoardModel
    {
        public long organizationId { get; set; }
        public string Title { get; set; }
    }
}