using McpDotNet.Client;
using McpDotNet.Protocol.Transport;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCP_Studio.Models;
using McpDotNet.Configuration;
using System.Text.Json;
using System.IO;
using McpDotNet.Extensions.AI;
using Microsoft.Extensions.AI;

namespace MCP_Studio.Service
{
    public static class MCPService
    {      
        public static async Task<List<AITool>?> GetToolsAsync()
        {
            var serverList = new List<MCPServerConfig>();
            try
            {
                string jsonString = File.ReadAllText("mcp_settings.json");
                var mcpServerDictionary = JsonSerializer.Deserialize<McpServerDictionary>(jsonString);

                if (mcpServerDictionary?.Servers != null)
                {
                    foreach (var server in mcpServerDictionary.Servers)
                    {
                        MCPServerConfig serverConfig = new MCPServerConfig();
                        serverConfig.Name = server.Key;
                        serverConfig.Command = server.Value.Command;
                        serverConfig.Args = server.Value.Args;
                        serverList.Add(serverConfig);                    
                    }
                }

                McpClientOptions options = new()
                {
                    ClientInfo = new() { Name = "MCP-Studio", Version = "1.0.0" }
                };

                List<McpServerConfig> mcpServerConfigs = new List<McpServerConfig>();

                foreach (var server in serverList)
                {
                    McpServerConfig config = new()
                    {
                        Id = server.Name,
                        Name = server.Name,
                        TransportType = TransportTypes.StdIo,
                        TransportOptions = new()
                        {
                            ["command"] = server.Command,
                            ["arguments"] = server.Args
                        }
                    };
                    mcpServerConfigs.Add(config);
                }

                var factory = new McpClientFactory(
                       mcpServerConfigs,
                       options,
                       NullLoggerFactory.Instance
                   );
                List<AITool> mappedTools = new List<AITool>();
                foreach (var server in serverList)
                {
                    var client = await factory.GetClientAsync(server.Name);
                    var listToolsResult = await client.ListToolsAsync();
                    server.Tools = listToolsResult.Tools;
                    mappedTools.AddRange(listToolsResult.Tools.Select(t => t.ToAITool(client)).ToList());
                }
                return mappedTools;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing settings: {ex.Message}");
                return null;
            }          
        }
    }
}
