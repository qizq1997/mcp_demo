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

namespace MCP_Studio.ViewModels;

public partial class MCPSettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<MCPServerConfig> serverList;
    

    public MCPSettingsViewModel()
    {
        ServerList = new ObservableCollection<MCPServerConfig>();
        LoadSettings();
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

    [RelayCommand]
    private void Test()
    {
        LoadSettings();
    }
}
