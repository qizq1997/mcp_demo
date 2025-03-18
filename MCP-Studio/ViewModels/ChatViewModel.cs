using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCP_Studio.Models;
using System.Text.Json;
using System;
using System.IO;

namespace MCP_Studio.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    public ChatViewModel()
    {
        
    }

    [RelayCommand]
    private void Test()
    {
        try
        {
            string jsonString = File.ReadAllText("ChatModelSettings.json");
            var chatModelConfig = JsonSerializer.Deserialize<ChatModelConfig>(jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing settings: {ex.Message}");
        }

    }
} 