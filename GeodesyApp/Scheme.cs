using System;
using System.Collections.Generic;
using System.Text;

namespace GeodesyApp
{
    public class Scheme
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string View { get; set; } = "";
        public decimal Volume { get; set; }
        public string Status { get; set; } = "";
    }
}
