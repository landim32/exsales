﻿using System;
using System.Collections.Generic;

namespace DB.Infra.Context;

public partial class Product
{
    public long ProductId { get; set; }

    public long NetworkId { get; set; }

    public string Name { get; set; }

    public double Price { get; set; }

    public int Frequency { get; set; }

    public int Limit { get; set; }

    public int Status { get; set; }

    public string Slug { get; set; }

    public virtual Network Network { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
