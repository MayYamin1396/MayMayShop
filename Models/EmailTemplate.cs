﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public class EmailTemplate
    {
        public int Id { get; set; }

        public string ActionName { get; set; }
      
        public string Subject { get; set; }
        public string Body { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
