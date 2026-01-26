using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GodotUtils.Framework.UI.Console;

public sealed class GameConsoleFramework
{
    #region Config
    private const int MaxTextFeed = 1000;
    #endregion

    #region Variables
    private readonly List<ConsoleCommandInfo> _commands = [];
    private readonly ConsoleHistory _history = new();
    private readonly Node _node;
    private readonly StringName _inputActionToggleConsole;
    private readonly StringName _inputActionUp;
    private readonly StringName _inputActionDown;
    private PanelContainer _mainContainer;
    private PopupPanel _settingsPopup;
    private CheckBox _settingsAutoScroll;
    private TextEdit _feed;
    private LineEdit _input;
    private Button _settingsBtn;
    private bool _autoScroll = true;
    #endregion

    public GameConsoleFramework(Node node, StringName inputActionToggleConsole, StringName inputActionUp, StringName inputActionDown)
    {
        _node = node;
        _inputActionToggleConsole = inputActionToggleConsole;
        _inputActionUp = inputActionUp;
        _inputActionDown = inputActionDown;
    }

    #region Godot Overrides
    public void Ready()
    {
        _feed          = _node.GetNode<TextEdit>("%Output");
        _input         = _node.GetNode<LineEdit>("%CmdsInput");
        _settingsBtn   = _node.GetNode<Button>("%Settings");
        _mainContainer = _node.GetNode<PanelContainer>("%MainContainer");
        _settingsPopup = _node.GetNode<PopupPanel>("%PopupPanel");

        _settingsAutoScroll = _node.GetNode<CheckBox>("%PopupAutoScroll");
        _settingsAutoScroll.ButtonPressed = _autoScroll;

        _input.TextSubmitted += OnConsoleInputEntered;
        _settingsBtn.Pressed += OnSettingsBtnPressed;
        _settingsAutoScroll.Toggled += OnAutoScrollToggeled;

        _mainContainer.Hide();
    }

    public void Process()
    {
        if (Input.IsActionJustPressed(_inputActionToggleConsole))
        {
            ToggleVisibility();
            return;
        }

        InputNavigateHistory();
    }

    public void ExitTree()
    {
        _input.TextSubmitted -= OnConsoleInputEntered;
        _settingsBtn.Pressed -= OnSettingsBtnPressed;
        _settingsAutoScroll.Toggled -= OnAutoScrollToggeled;
    }
    #endregion

    #region API
    public List<ConsoleCommandInfo> GetCommands()
    {
        return _commands;
    }

    public ConsoleCommandInfo RegisterCommand(string cmd, Action<string[]> code)
    {
        ConsoleCommandInfo info = new()
        {
            Name = cmd,
            Code = code
        };

        _commands.Add(info);

        return info;
    }

    public void AddMessage(object message)
    {
        double prevScroll = _feed.ScrollVertical;
        
        // Prevent text feed from becoming too large
        if (_feed.Text.Length > MaxTextFeed)
        {
            // If there are say 2353 characters then 2353 - 1000 = 1353 characters
            // which is how many characters we need to remove to get back down to
            // 1000 characters
            _feed.Text = _feed.Text[^MaxTextFeed..];
        }

        _feed.Text += $"\n{message}";

        // Removing text from the feed will mess up the scroll, this is why the
        // scroll value was stored previous, we set this to that value now to fix
        // this
        _feed.ScrollVertical = prevScroll;

        // Autoscroll if enabled
        _node.GetTree().ProcessFrame += ScrollDown;
    }

    public bool Visible => _mainContainer.Visible;

    public void ToggleVisibility()
    {
        _mainContainer.Visible = !_mainContainer.Visible;

        if (_mainContainer.Visible)
        {
            _input.GrabFocus();
            _node.GetTree().ProcessFrame += ScrollDown;
        }
        else
        {
            // Console was closed
            // TODO: GameFramework.FocusOutline.ClearFocus();
        }
    }
    #endregion

    #region Private Methods
    private void ScrollDown()
    {
        _node.GetTree().ProcessFrame -= ScrollDown;

        if (_autoScroll)
        {
            _feed.ScrollVertical = (int)_feed.GetVScrollBar().MaxValue;
        }
    }

    private bool ProcessCommand(string text)
    {
        string[] parts = text.ToLower().Split();
        string cmd = parts[0];

        ConsoleCommandInfo cmdInfo = TryGetCommand(cmd);

        if (cmdInfo == null)
        {
            GameFramework.Logger.Log($"The command '{cmd}' does not exist");
            return false;
        }

        string[] args = [.. parts.Skip(1)];

        cmdInfo.Code.Invoke(args);

        return true;
    }

    private ConsoleCommandInfo TryGetCommand(string text)
    {
        ConsoleCommandInfo cmd = _commands.Find(IsMatchingCommand);

        return cmd;

        bool IsMatchingCommand(ConsoleCommandInfo cmd)
        {
            if (string.Equals(cmd.Name, text, StringComparison.OrdinalIgnoreCase))
                return true;

            return cmd.Aliases != null && cmd.Aliases.Any(alias => string.Equals(alias, text, StringComparison.OrdinalIgnoreCase));
        }
    }

    private void InputNavigateHistory()
    {
        // If console is not visible or there is no history to navigate do nothing
        if (!_mainContainer.Visible || _history.NoHistory())
            return;

        if (Input.IsActionJustPressed(_inputActionUp))
        {
            string historyText = _history.MoveUpOne();

            _input.Text = historyText;

            // if deferred is not used then something else will override these settings
            SetCaretColumn(historyText.Length);
        }

        if (Input.IsActionJustPressed(_inputActionDown))
        {
            string historyText = _history.MoveDownOne();

            _input.Text = historyText;

            // if deferred is not used then something else will override these settings
            SetCaretColumn(historyText.Length);
        }
    }

    private void OnSettingsBtnPressed()
    {
        if (!_settingsPopup.Visible)
        {
            _settingsPopup.PopupCentered();
        }
    }

    private void OnAutoScrollToggeled(bool value)
    {
        _autoScroll = value;
    }

    private void OnConsoleInputEntered(string text)
    {
        // case sensitivity and trailing spaces should not factor in here
        string inputToLowerTrimmed = text.Trim().ToLower();
        string[] inputArr = inputToLowerTrimmed.Split(' ');

        // extract command from input
        string cmd = inputArr[0];

        // do not do anything if cmd is just whitespace
        if (string.IsNullOrWhiteSpace(cmd))
            return;

        // keep track of input history
        _history.Add(inputToLowerTrimmed);

        // process the command
        ProcessCommand(text);

        // clear the input after the command is executed
        _input.Clear();

        _node.GetTree().ProcessFrame += RefocusInput;
    }

    // Put focus back on the input and move caret to end so user can type immediately
    private void RefocusInput()
    {
        _node.GetTree().ProcessFrame -= RefocusInput;
        _input.Edit(); // MUST do this otherwise refocus on LineEdit will NOT work
        _input.GrabFocus();
        _input.CaretColumn = _input.Text.Length;
    }

    private void SetCaretColumn(int pos)
    {
        _input.CallDeferred(Control.MethodName.GrabFocus);
        _input.CallDeferred(GodotObject.MethodName.Set, LineEdit.PropertyName.CaretColumn, pos);
    }
    #endregion
}

#region Extensions
public static class ConsoleCommandInfoExtensions
{
    public static ConsoleCommandInfo WithAliases(this ConsoleCommandInfo cmdInfo, params string[] aliases)
    {
        cmdInfo.Aliases = aliases;
        return cmdInfo;
    }
}
#endregion
