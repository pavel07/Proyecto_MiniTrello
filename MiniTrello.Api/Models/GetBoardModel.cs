﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class GetBoardModel
    {
        public long BoardId { get; set; }
        public string NewBoardTitle { get; set; }
    }
}