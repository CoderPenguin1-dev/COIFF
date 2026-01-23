using System;
using System.Runtime.InteropServices.Swift;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CinderEngine;

namespace Flame;

public partial class MainWindow : Window
{
    private Game _game;
    internal string GameFilePath { get; set; } = string.Empty;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode) return; // Ignore everything else if it's the designer.
        
        // Still none? Kill the whole damn app. Useless without one!
        if (GameFilePath != string.Empty)
        {
            _game = new(GameFilePath);
            OpenGame();
        }
        
    }
    
    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (OptionListBox.SelectedIndex != -1)
        {
            _game.ParseSelectedOption(OptionListBox.SelectedIndex);
            RefreshGame();
        }
    }

    private void RefreshGame()
    {
        Title = $"{_game.GameTitle} ({_game.GameVersion}) by {_game.Author}";  
        StoryText.Text = _game.GetNodeText();
        ScrollViewer.ScrollToHome();
        OptionListBox.Items.Clear();
        string[] options = _game.GetNodeOptions();
        foreach (string option in options)
            OptionListBox.Items.Add(option);
    }

    private void OpenGame()
    {
        _game = new(GameFilePath);
        RefreshGame();
    }
    
    private void QuitButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void OpenGameButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var filePicker = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Choose Story File",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Story File") {Patterns = ["*.cstory"]}, FilePickerFileTypes.All]
        });

        if (filePicker.Count > 0)
        {
            GameFilePath = filePicker[0].TryGetLocalPath();
            OpenGame();
        }
    }
}