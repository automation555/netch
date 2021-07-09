﻿using System.Collections.Generic;

namespace Netch.Models.Server.ShadowsocksR
{
    public static class Global
    {
        public static readonly List<string> Methods = new List<string>()
        {
            "rc4",
            "bf-cfb",
            "des-cfb",
            "rc2-cfb",
            "rc4-md5",
            "idea-cfb",
            "seed-cfb",
            "cast5-cfb",
            "aes-128-ctr",
            "aes-192-ctr",
            "aes-256-ctr",
            "aes-128-cfb",
            "aes-192-cfb",
            "aes-256-cfb",
            "camellia-128-cfb",
            "camellia-192-cfb",
            "camellia-256-cfb",
            "chacha20",
            "chacha20-ietf"
        };

        public static readonly List<string> Prots = new List<string>()
        {
            "origin",
            "auth_sha1_v4",
            "auth_aes128_md5",
            "auth_aes128_sha1",
            "auth_chain_a",
        };

        public static readonly List<string> OBFSs = new List<string>()
        {
            "plain",
            "http_post",
            "http_simple",
            "tls1.2_ticket_auth"
        };
    }
}
