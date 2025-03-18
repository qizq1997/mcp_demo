using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCP_Studio.Models;
using System.Text.Json;
using System;
using System.IO;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using OpenAI;
using System.ClientModel;
using System.Collections.Generic;
using Tmds.DBus.Protocol;
using System.Threading.Tasks;
using MCP_Studio.Service;
using System.Linq;
using System.Collections.ObjectModel;
using McpDotNet.Protocol.Types;

namespace MCP_Studio.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    public ChatModelConfig? ChatModelConfig { get; set; }
    public ObservableCollection<Microsoft.Extensions.AI.ChatMessage>? Messages { get; set; }
    public IChatClient? ChatClient {  get; set; }
    public List<AITool>? Tools { get; set; }
    public ChatViewModel()
    {
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
            string jsonString = File.ReadAllText("ChatModelSettings.json");
            ChatModelConfig = JsonSerializer.Deserialize<ChatModelConfig>(jsonString);

            if (ChatModelConfig == null)
            {
                throw new InvalidOperationException("ChatModelConfig is null after deserialization.");
            }

            ApiKeyCredential apiKeyCredential = new ApiKeyCredential(ChatModelConfig.ApiKey);

            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(ChatModelConfig.BaseURL);

            IChatClient openaiClient = new OpenAIClient(apiKeyCredential, openAIClientOptions)
                .AsChatClient(ChatModelConfig.ModelID);

            // Note: To use the ChatClientBuilder you need to install the Microsoft.Extensions.AI package
            ChatClient = new ChatClientBuilder(openaiClient)
               .UseFunctionInvocation()
               .Build(); 

            Messages =
               [
               // Add a system message
               new(ChatRole.System, "You are a helpful assistant, helping us test MCP server functionality."),
                ];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing settings: {ex.Message}");
        }
    }

    private async Task LoadAvailableTools()
    {
        Tools = await MCPService.GetToolsAsync();
    }

    [RelayCommand]
    private async Task SendMessage(string message)
    {
        Messages.Add(new(ChatRole.User, message));
    
        var response = await ChatClient.GetResponseAsync(
               Messages,
               new() { Tools = Tools });

        Messages.AddMessages(response);
    }

    [RelayCommand]
    private async Task Test()
    {
        try
        {
            string jsonString = File.ReadAllText("ChatModelSettings.json");
            var chatModelConfig = JsonSerializer.Deserialize<ChatModelConfig>(jsonString);

            ApiKeyCredential apiKeyCredential = new ApiKeyCredential(chatModelConfig.ApiKey);

            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(chatModelConfig.BaseURL);

            IChatClient openaiClient = new OpenAIClient(apiKeyCredential, openAIClientOptions)
                .AsChatClient(chatModelConfig.ModelID);

            // Note: To use the ChatClientBuilder you need to install the Microsoft.Extensions.AI package
            var chatClient = new ChatClientBuilder(openaiClient)
                .UseFunctionInvocation()
                .Build();
            List<Microsoft.Extensions.AI.ChatMessage> messages = new();
            messages =
            [
               // Add a system message
               new(ChatRole.System, "You are a helpful assistant, helping us test MCP server functionality."),
            ];

            string query = "获取https://github.com/Ming-jiayou/mcp_demo的文本内容";
            // Add a user message
            messages.Add(new(ChatRole.User, query));

            var tools = await MCPService.GetToolsAsync();
            var response = await chatClient.GetResponseAsync(
                   messages,
                   new() { Tools = tools });

            messages.AddMessages(response);
            var toolUseMessage = response.Messages.Where(m => m.Role == ChatRole.Tool);
            if (response.Messages[0].Contents.Count > 1)
            {
                //var functionMessage = response.Messages.Where(m => (m.Role == ChatRole.Assistant && m.Text == "")).Last();
                var functionCall = (FunctionCallContent)response.Messages[0].Contents[1];
                Console.ForegroundColor = ConsoleColor.Green;
                string arguments = "";
                if (functionCall.Arguments != null)
                {
                    foreach (var arg in functionCall.Arguments)
                    {
                        arguments += $"{arg.Key}:{arg.Value};";
                    }
                    Console.WriteLine($"调用函数名:{functionCall.Name};参数信息：{arguments}");
                    foreach (var message in toolUseMessage)
                    {
                        var functionResultContent = (FunctionResultContent)message.Contents[0];
                        Console.WriteLine($"调用工具结果：{functionResultContent.Result}");
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("调用工具参数缺失");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("本次没有调用工具");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing settings: {ex.Message}");
        }

    }
} 