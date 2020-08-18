using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horizon.DataAccess
{
    public partial interface IDMLBuilder
    {
        string SqlTemplate { get; }
        List<SugarParameter> Parameters { get; set; }
        SqlSugarProvider  Context { get; set; }
        StringBuilder sql { get; set; }
        string ToSqlString();
        void Clear();
    }
}
