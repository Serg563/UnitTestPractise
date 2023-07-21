﻿using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.Entities
{
    public class Shipper
    {
        [Column("ShipperID")]
        public int Id { get; private set; }

        public string? CompanyName { get; init; } = default!;

        public string? Phone { get; set; }
    }
}
