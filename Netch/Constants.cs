﻿using System.Text.Encodings.Web;
using System.Text.Json;

namespace Netch
{
    public static class Constants
    {
        public const string TempConfig = "data\\last.json";
        public const string TempRouteFile = "data\\route.txt";
        public const string EOF = "\r\n";
        public const string OutputTemplate = @"[{Timestamp:yyyy-MM-dd HH:mm:ss}][{Level}] {Message:lj}{NewLine}{Exception}";
        public const string LogFile = "logging\\application.log";

        public static class Parameter
        {
            public const string Show = "-show";
            public const string ForceUpdate = "-forceUpdate";
        }

        public static JsonSerializerOptions DefaultJsonSerializerOptions => new()
        {
            WriteIndented = true,
            IgnoreNullValues = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}