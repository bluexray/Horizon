using System;
using System.Collections.Generic;
using System.Text;

namespace Horizon.DataAccess.Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public UnitOfWork(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        public void BeginTran()
        {
            GetDbInstance().Ado.BeginTran();
        }

        public void CommitTran()
        {
            try
            {
                GetDbInstance().Ado.CommitTran();
            }
            catch (Exception ex)
            {
                GetDbInstance().Ado.RollbackTran();
                throw ex;
            }
        }

        public ISqlSugarClient GetDbInstance()
        {
            return _sqlSugarClient as SqlSugarClient;
        }

        public void RollbackTran()
        {
            GetDbInstance().Ado.RollbackTran();
        }
    }
}
