namespace Horizon.DataAccess.Repository
{
    public interface IUnitOfWork
    {
        // 创建 sqlsugar client 实例
        ISqlSugarClient GetDbInstance();
        // 开始事务
        void BeginTran();
        // 提交事务
        void CommitTran();
        // 回滚事务
        void RollbackTran();
    }
}