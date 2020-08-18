using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horizon.DataAccess
{
    public class MappingColumn
    {
        public string PropertyName { get; set; }
        public string DbColumnName { get; set; }
        public string EntityName { get; set; }
    }
}
