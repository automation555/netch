namespace Netch.Models.Config
{
    public class Ports1
    {
        /// <summary>
        ///     Socks 端口
        /// </summary>
        [Newtonsoft.Json.JsonProperty("socks")]
        public int Socks = 2081;

        /// <summary>
        ///     Mixed 端口
        /// </summary>
        [Newtonsoft.Json.JsonProperty("mixed")]
        public int Mixed = 2082;

        /// <summary>
        ///     Redir 端口
        /// </summary>
        [Newtonsoft.Json.JsonProperty("redir")]
        public int Redir = 2083;
        
        int x=0;
    }
}
