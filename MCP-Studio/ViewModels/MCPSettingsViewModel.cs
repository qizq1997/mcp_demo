using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using MCP_Studio.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Styling;
using McpDotNet.Configuration;
using McpDotNet.Protocol.Transport;
using McpDotNet.Protocol.Types;
using McpDotNet.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using McpDotNet.Extensions.AI;
using Microsoft.Extensions.AI;

namespace MCP_Studio.ViewModels;

public partial class MCPSettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<MCPServerConfig> serverList;
    

    public  MCPSettingsViewModel()
    {
        ServerList = new ObservableCollection<MCPServerConfig>();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        LoadSettings();
        await LoadAvailableTools();
    }


    private void LoadSettings()
    {
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
                    ServerList.Add(serverConfig);
                }              
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing settings: {ex.Message}");
        }
    }

    private async Task LoadAvailableTools()
    {
        McpClientOptions options = new()
        {
            ClientInfo = new() { Name = "MCP-Studio", Version = "1.0.0" }
        };

        List<McpServerConfig> mcpServerConfigs = new List<McpServerConfig>();

        foreach (var server in ServerList)
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

        foreach (var server in ServerList)
        {
            var client = await factory.GetClientAsync(server.Name);
            var listToolsResult = await client.ListToolsAsync();
            server.Tools = listToolsResult.Tools;
        }  
    }

    [RelayCommand]
    private async Task Test()
    {       
        McpClientOptions options = new()
        {
            ClientInfo = new() { Name = "MCP-Studio", Version = "1.0.0" }
        };

        List<McpServerConfig> mcpServerConfigs = new List<McpServerConfig>();

        foreach(var server in ServerList)
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

       List<IMcpClient> clients = new List<IMcpClient>();
       foreach(var server in ServerList)
        {
            var client = await factory.GetClientAsync(server.Name);
            clients.Add(client);
        }

       List<ListToolsResult> listToolsResults = new List<ListToolsResult>();
       List<AITool> tools = new List<AITool>();
       foreach(var client in clients)
       {
            var listToolsResult = await client.ListToolsAsync();
            var mappedTools = listToolsResult.Tools.Select(t => t.ToAITool(client)).ToList();

            listToolsResults.Add(listToolsResult);
            tools.AddRange(mappedTools);
        }       
    }


}
