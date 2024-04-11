﻿using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Input;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings;
using AssistUser.Lib.Base.Models;
using AssistUser.Lib.V2.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExCSS;
using Serilog;

namespace Assist.ViewModels.Assist;

public partial class AssistAccountLoginFormViewModel : ViewModelBase
{
    [ObservableProperty] private ICommand? _backToRegisterCommand;
    [ObservableProperty] private ICommand? _accountCompleteCommand;
    [ObservableProperty] private string _passwordText = "";
    [ObservableProperty] private string _usernameText = "";
    [ObservableProperty] private bool _isProcessing = false;
    [ObservableProperty] private string _isProcessingText = "Processing...";
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private bool _errorMessageVisible = false;
    [ObservableProperty] private bool _requestedCancel = false;

    private const string _discordAuthUrl =
        "https://discord.com/oauth2/authorize?client_id=984912187837526038&response_type=code&redirect_uri=https%3A%2F%2Flive.assistval.com%2Fapi%2Foauth%2Fdiscord%2Fredirect&scope=guilds.join+email+connections+identify&state={0}";
        
    [RelayCommand]
    public async Task DiscordOAuthCommand()
    {
        Log.Information("User Requested to Authenticate to Assist with Discord");
        Log.Information("Setting Processing Flag");

        IsProcessing = true;
        
        Log.Information("Creating Custom State");
        var bytes = new byte[new Random().NextInt64(16,64)];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(bytes);
        }
        
        string hash1 = BitConverter.ToString(bytes);

        var randHash = hash1.Replace("-", "");
        
        Log.Information("Opening Discord Window");
        Log.Information("Session Code: " + randHash);
        OpenDiscordOAuth(randHash);

        await CheckForClientUpdate(randHash);
    }
    
    [RelayCommand]
    public void CancelOAuth()
    {
        RequestedCancel = true;
    }
    
    [RelayCommand]
    public async Task TwitchOAuthCommand()
    {
        Log.Information("User Requested to Authenticate to Assist with Twitch");
        Log.Information("Setting Processing Flag");

        //IsProcessing = true;
        
        Log.Information("Requesting Authentication from Assist Server");
        

    }

    [RelayCommand]
    public async Task LoginButtonCommand()
    {
        Log.Information("User Requested to Login a User/Pass Account to Assist");

#if DEBUG
        Log.Information($"User Username: {UsernameText}");
        Log.Information($"User Password: {PasswordText}");
#endif
        
        
        IsProcessing = true;
        Log.Information("Sending Login Request");
        
        //TODO: Send Register Request Here.
        try
        {
            await AssistApplication.AssistUser.Authentication.AuthenticateWithLogin(new LoginAccountModel()
            {
                Password = PasswordText,
                AccountIdentifier = UsernameText
            });
        }
        catch (Exception e)
        {
            ChangeErrorMessage(e.Message);
            IsProcessing = false;
            return;
        }
        
        Log.Information("Account Login Successful");
        Log.Information("Saving Account Code...");
        AssistSettings.Default.SaveAssistUserCode(AssistApplication.AssistUser.userTokens.RefreshToken);
        
        AccountCompleteCommand?.Execute(null); // Signals that account is completed.
    }

    private async Task CheckForClientUpdate(string stateCode)
    {
        AssistTokens? tokens = null; 
        for (int i = 0; i < 20; i++)
        {
            await Task.Delay(5000);
            if (RequestedCancel)
            {
                RequestedCancel = false;
                IsProcessing = false;
                break;
            }
            
            try
            {
               tokens = await AssistApplication.AssistUser.Authentication.AuthenticateWithClient(stateCode);
               break;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to get Client on try {i} : {e.Message}");
            }
            
        }

        if (tokens is null)
        {
            ChangeErrorMessage("Failed to get login from Discord.");
            IsProcessing = false;
            return;
        }
        
        Log.Information("Information from Client Received");
        
        AssistSettings.Default.SaveAssistUserCode(AssistApplication.AssistUser.userTokens.RefreshToken);
        
        AccountCompleteCommand?.Execute(null);
    }
    
    private void ChangeErrorMessage(string message)
    {
        ErrorMessageVisible = true;
        ErrorMessage = message;
    }

    private void OpenDiscordOAuth(string state)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = string.Format(_discordAuthUrl, state),
            UseShellExecute = true
        });
    }
}