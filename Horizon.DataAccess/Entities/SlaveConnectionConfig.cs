using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horizon.DataAccess
{
    public class SlaveConnectionConfig
    {
        /// <summary>
        ///Default value is 1
        ///If value is 0 means permanent non execution
        /// </summary>
        public int HitRate { get; set; }

        public string ConnectionString { get; set; }
    }
}
