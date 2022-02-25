using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QuasarFireOperation_MELI.Models
{
    public class Satellite
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public string[] Message { get; set; }

        public float? Distance { get; set; }

        public Coordinate Location { get; set; }
    }
}
