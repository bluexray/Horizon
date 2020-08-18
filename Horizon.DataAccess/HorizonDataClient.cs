using System;
using System.Collections.Generic;
using System.Text;
using Horizon.Core.Data;

namespace Horizon.DataAccess
{
    public  class HorizonDataClient {
        public HorizonDataClient(MutiDbOperate config)
        {

        }
        //public HorizonDataClient(MutiDbOperate config):base(config) { DbName = config.DbName;}
        public string DbName { set; get; }
    }
}
