using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LongJohnSilver.Database
{
    public class KPlayer
    {
        public string PlayerId { get; set; }
        public int TurnsLeft { get; set; }
        public int LastPlayed { get; set; }
    }
}
