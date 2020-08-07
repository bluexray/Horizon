namespace Horizon.GRPC.Interceptors
{
    public class SecurityOption
    {
        /// <summary>
        /// 白名单
        /// </summary>
        public string Whitelist { get; set; } = "*";

        /// <summary>
        /// 黑名单
        /// </summary>
        public string BlackList { get; set; }
    }
}