using System;
using System.Collections.Generic;
using System.Text;
using LongJohnSilver.Database;

namespace LongJohnSilver.Statics
{
    public static class Factory
    {
        /// <summary>
        /// Factory to return the current main database
        /// </summary>
        /// <returns></returns>
        public static IDatabase GetDatabase()
        {
            return new MainDataDb();
        }
    }
}
