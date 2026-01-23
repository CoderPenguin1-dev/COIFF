using System.Runtime.InteropServices.Swift;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CinderEngine;

namespace Flame;

public partial class MainWindow : Window
{
    private readonly Game _game;
    internal string GameFilePath { get; set; } = "game.cyoa";
    public MainWindow()
    {
        InitializeComponent();
        _game = new(GameFilePath);
        Title = $"{_game.GameTitle} ({_game.GameVersion}) by {_game.Author}";  
        RefreshGame();
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
        StoryText.Text = _game.GetNodeText();
        ScrollViewer.ScrollToHome();
        OptionListBox.Items.Clear();
        string[] options = _game.GetNodeOptions();
        foreach (string option in options)
            OptionListBox.Items.Add(option);
    }

    private void QuitButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}