using System;
using System.Collections.Generic;
using System.Text;

namespace LongJohnSilver.FlintTimeToolkit
{
    public static class SecondsTill
    {
        public static int HourIsUp => ((60 - (DateTime.Now.Minute)) * 60) - DateTime.Now.Second;
    }
}
