﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace yak_shop_api.Models
{
    public class CustomerOrder
    {
        public long Id { get; set; }
        public string Customer { get; set; }
        public OrderItem Order { get; set; }
    }
}
