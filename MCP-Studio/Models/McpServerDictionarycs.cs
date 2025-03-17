using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MCP_Studio.Models
{
    public class ServerConfig
    {
        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("args")]
        public string Args { get; set; }
    }

    public class MCPServerConfig
    {
        public string Name {  get; set; }
        public string Command { get; set; }
        public string Args { get; set; }
    }

    public class McpServerDictionary
    {
        [JsonPropertyName("mcpServers")]
        public Dictionary<string, ServerConfig> Servers { get; set; } = new Dictionary<string, ServerConfig>();
    }
}
