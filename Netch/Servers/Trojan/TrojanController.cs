﻿using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Netch.Controllers;
using Netch.Interfaces;
using Netch.Models;
using Netch.Servers.Trojan.Models;

namespace Netch.Servers.Trojan
{
    public class TrojanController : Guard, IServerController
    {
        public override string MainFile { get; protected set; } = "Trojan.exe";

        protected override IEnumerable<string> StartedKeywords { get; set; } = new[] { "started" };

        protected override IEnumerable<string> StoppedKeywords { get; set; } = new[] { "exiting" };

        public override string Name { get; } = "Trojan";

        public ushort? Socks5LocalPort { get; set; }

        public string? LocalAddress { get; set; }

        public void Start(in Server s, in Mode mode)
        {
            var server = (Trojan)s;
            var trojanConfig = new TrojanConfig
            {
                local_addr = this.LocalAddress(),
                local_port = this.Socks5LocalPort(),
                remote_addr = server.AutoResolveHostname(),
                remote_port = server.Port,
                password = new List<string>
                {
                    server.Password
                }
            };


            if (!string.IsNullOrWhiteSpace(server.Host))
                trojanConfig.ssl.sni = server.Host;
            else if (Global.Settings.ResolveServerHostname)
                trojanConfig.ssl.sni = server.Hostname;

            File.WriteAllBytes(Constants.TempConfig, JsonSerializer.SerializeToUtf8Bytes(trojanConfig, Constants.DefaultJsonSerializerOptions));

            StartInstanceAuto("-c ..\\data\\last.json");
        }

        public override void Stop()
        {
            StopInstance();
        }
    }
}