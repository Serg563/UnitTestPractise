﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using OrderApi.Controllers;

namespace OrderApi.SignalR.Services.Data
{
    public partial class Connections
    {
        public string UserId { get; set; }
        public string ConnectionId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}