using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace yak_shop_api.Models
{
    public class Yak
    {
        [JsonIgnore]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public double Age { get; set; }
        public string Sex { get; set; }
        public double AgeLastShaved { get; set; }

        public Yak()
        {
            this.AgeLastShaved = this.Age;
        }
    }
}
