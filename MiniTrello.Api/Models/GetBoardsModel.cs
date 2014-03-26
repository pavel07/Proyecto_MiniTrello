﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class GetBoardsModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsArchived { get; set; }
    }
}