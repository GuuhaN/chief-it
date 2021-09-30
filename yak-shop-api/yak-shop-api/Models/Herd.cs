using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace yak_shop_api.Models
{
    public class Herd
    {
        [JsonIgnore]
        public long Id { get; set; }
        public List<Yak> Yaks { get; set; }
    }
}
