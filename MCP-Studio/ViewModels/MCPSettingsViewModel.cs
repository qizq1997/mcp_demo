using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using MCP_Studio.Models;

namespace MCP_Studio.ViewModels;

public partial class MCPSettingsViewModel : ViewModelBase
{
    [RelayCommand]
    private void Test()
    {
        try
        {
            string jsonString = File.ReadAllText("mcp_settings.json");
            McpServerDictionary root = JsonSerializer.Deserialize<McpServerDictionary>(jsonString);          
        }
        catch (Exception ex)
        {
            // 处理异常
            Console.WriteLine($"Error parsing settings: {ex.Message}");
        }
    }
}
