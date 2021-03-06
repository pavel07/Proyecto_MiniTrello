﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class GetLanesModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public List<CardModel> Cards { get; set; }
        public bool IsArchived { get; set; }
    }
}