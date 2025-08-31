#if DEBUG
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using static Godot.Control;
using System.Linq;

namespace GodotUtils.Debugging.Visualize;

/// <summary>
/// The main core class for the visualizer UI
/// </summary>
public static class VisualUI
{
    public const float VisualUiScaleFactor = 0.6f;

    private const string PathIcons = "res://Framework/GodotUtils/Visualize/Icons";
    private const double ReleaseFocusOnPressDelay = 0.1;
    private const float PanelScaleFactor = 0.9f;
    private const int MinScrollViewDistance = 350;
    private const int TitleFontSize = 20;
    private const int MemberFontSize = 18;
    private const int FontOutlineSize = 6;
    private const int MinButtonSize = 25;
    private const int MaxSecondsToWaitForInitialValues = 3;

    private static Texture2D _eyeOpen = GD.Load<Texture2D>($"{PathIcons}/EyeOpen.png");
    private static Texture2D _eyeClosed = GD.Load<Texture2D>($"{PathIcons}/EyeClosed.png");
    private static Texture2D _wrench = GD.Load<Texture2D>($"{PathIcons}/Wrench.png");
    private static Color Green = new(0.8f, 1, 0.8f);
    private static Color Pink = new(1.0f, 0.75f, 0.8f);

    /// <summary>
    /// Creates the visual panel for a specified visual node.
    /// </summary>
    public static (Control, List<Action>) CreateVisualPanel(VisualData visualData, string[] readonlyMembers)
    {
        Dictionary<Node, VBoxContainer> visualNodes = [];
        
        List<VisualSpinBox> spinBoxes = [];
        List<Action> updateControls = [];

        Node node = visualData.Node;

        PanelContainer panelContainer = CreatePanelContainer(node.Name);
        panelContainer.MouseFilter = MouseFilterEnum.Ignore;
        panelContainer.Name = "Main Panel";

        Vector2 currentCameraZoom = GetCurrentCameraZoom(node);
        panelContainer.Scale = new Vector2(1f / currentCameraZoom.X, 1f / currentCameraZoom.Y) * PanelScaleFactor;

        VBoxContainer mutableMembersVbox = CreateColoredVBox(Green);
        mutableMembersVbox.MouseFilter =  MouseFilterEnum.Ignore;
        mutableMembersVbox.Name = "Mutable Members";

        VBoxContainer readonlyMembersVbox = CreateColoredVBox(Pink);
        readonlyMembersVbox.MouseFilter = MouseFilterEnum.Ignore;
        readonlyMembersVbox.Name = "Readonly Members";

        // Readonly Members
        AddReadonlyControls(readonlyMembers, node, readonlyMembersVbox, updateControls, spinBoxes);

        // Mutable Members
        AddMutableControls(mutableMembersVbox, visualData.Properties, node, spinBoxes);
        AddMutableControls(mutableMembersVbox, visualData.Fields, node, spinBoxes);

        // Methods
        VisualMethods.AddMethodInfoElements(mutableMembersVbox, visualData.Methods, node, spinBoxes);

        VBoxContainer vboxLogs = new();
        vboxLogs.Name = "Logs";
        mutableMembersVbox.AddChild(vboxLogs);

        visualNodes.Add(node, vboxLogs);

        ScrollContainer scrollContainer = new();
        scrollContainer.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
        scrollContainer.VerticalScrollMode = ScrollContainer.ScrollMode.ShowNever;
        scrollContainer.CustomMinimumSize = new Vector2(0, MinScrollViewDistance);

        // Make them hidden by default
        //mutableMembersVbox.Hide();
        //readonlyMembersVbox.Hide();

        VBoxContainer titleBar = CreateTitleBar(node.Name, mutableMembersVbox, readonlyMembersVbox, visualData, readonlyMembers);
        titleBar.Name = "Main VBox";
        titleBar.MouseFilter = MouseFilterEnum.Ignore;
        titleBar.AddChild(readonlyMembersVbox);
        titleBar.AddChild(mutableMembersVbox);

        SetButtonsToReleaseFocusOnPress(titleBar);

        scrollContainer.AddChild(titleBar);
        panelContainer.AddChild(scrollContainer);
        
        // Add to canvas layer so UI is not affected by lighting in game world
        CanvasLayer canvasLayer = CreateCanvasLayer(node.Name, node.GetInstanceId());
        canvasLayer.AddChild(panelContainer);

        node.CallDeferred(Node.MethodName.AddChild, canvasLayer);

        // This is ugly but I don't know how else to do it
        VisualizeAutoload.Instance.VisualNodes = visualNodes;

        return (panelContainer, updateControls);
    }

    private static Vector2 GetCurrentCameraZoom(Node node)
    {
        Viewport viewport = node.GetViewport();

        Camera2D cam2D = viewport.GetCamera2D();

        if (cam2D != null)
        {
            return cam2D.Zoom;
        }

        return Vector2.One;
    }

    /// <summary>
    /// Ensures buttons release focus on press within a VBoxContainer.
    /// </summary>
    private static void SetButtonsToReleaseFocusOnPress(VBoxContainer vboxParent)
    {
        foreach (BaseButton baseButton in vboxParent.GetChildren<BaseButton>())
        {
            baseButton.Pressed += () =>
            {
                _ = new GodotTween(baseButton)
                    .Delay(ReleaseFocusOnPressDelay)
                    .Callback(() => baseButton.ReleaseFocus());
            };
        }
    }

    private static VBoxContainer CreateTitleBar(string name, Control mutableMembersVbox, Control readonlyMembersVbox, VisualData visualData, string[] readonlyMembers)
    {
        VBoxContainer vboxParent = new();

        HBoxContainer hbox = new()
        {
            Name = "Title Bar",
            Alignment = BoxContainer.AlignmentMode.Center,
            MouseFilter = MouseFilterEnum.Ignore
        };

        Label title = new()
        {
            Name = "Title",
            Text = name,
            Visible = true,
            LabelSettings = new LabelSettings
            {
                FontSize = TitleFontSize,
                FontColor = Colors.LightSkyBlue,
                OutlineColor = Colors.Black,
                OutlineSize = FontOutlineSize,
            }
        };

        hbox.AddChild(title);

        Button readonlyBtn = null;
        Button mutableBtn = null;

        if (readonlyMembers != null)
        {
            readonlyBtn = CreateVisibilityButton(_eyeOpen, Colors.Pink);
            readonlyBtn.ButtonPressed = true;
            hbox.AddChild(readonlyBtn);
        }

        if (visualData.Properties.Any() || visualData.Fields.Any())
        {
            mutableBtn = CreateVisibilityButton(_wrench, Colors.Gray);
            mutableBtn.ButtonPressed = true;
            hbox.AddChild(mutableBtn);
        }

        if (readonlyBtn != null)
        {
            readonlyBtn.Pressed += () =>
            {
                readonlyBtn.Icon = readonlyBtn.ButtonPressed ? _eyeOpen : _eyeClosed;
                readonlyMembersVbox.Visible = readonlyBtn.ButtonPressed;
                title.Visible = readonlyBtn.ButtonPressed || (mutableBtn != null && mutableBtn.ButtonPressed);
            };
        }

        if (mutableBtn != null)
        {
            mutableBtn.Pressed += () =>
            {
                mutableMembersVbox.Visible = mutableBtn.ButtonPressed;
                title.Visible = mutableBtn.ButtonPressed || (readonlyBtn != null && readonlyBtn.ButtonPressed);
            };
        }

        vboxParent.AddChild(hbox);

        return vboxParent;
    }

    private static Button CreateVisibilityButton(Texture2D icon, Color color)
    {
        Button btn = new()
        {
            Name = "Toggle Visibility",
            ToggleMode = true,
            Icon = icon,
            Flat = true,
            ExpandIcon = true,
            SelfModulate = color,
            CustomMinimumSize = Vector2.One * MinButtonSize,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest
        };

        btn.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());

        return btn;
    }

    /// <summary>
    /// Creates a colored VBoxContainer with specified RGB values.
    /// </summary>
    private static VBoxContainer CreateColoredVBox(Color color)
    {
        return new VBoxContainer
        {
            Modulate = color
        };
    }
    
    /// <summary>
    /// Creates a panel container with a specified name.
    /// </summary>
    private static PanelContainer CreatePanelContainer(string name)
    {
        PanelContainer panelContainer = new()
        {
            // Ensure this info is rendered above all game elements
            Name = name,
            Scale = Vector2.One * VisualUiScaleFactor,
            ZIndex = (int)RenderingServer.CanvasItemZMax
        };

        panelContainer.AddThemeStyleboxOverride("panel", new StyleBoxEmpty());

        return panelContainer;
    }

    /// <summary>
    /// Attempts to get member information for a Internal.
    /// </summary>
    private static void TryGetMemberInfo(Node node, string visualMember, out PropertyInfo property, out FieldInfo field, out object initialValue)
    {
        BindingFlags memberTypes = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        property = node.GetType().GetProperty(visualMember, memberTypes);
        field = null;
        initialValue = null;

        if (property != null)
        {
            initialValue = property.GetValue(property.GetGetMethod(true).IsStatic ? null : node);
        }
        else
        {
            field = node.GetType().GetField(visualMember, memberTypes);

            if (field != null)
            {
                initialValue = field.GetValue(field.IsStatic ? null : node);
            }
        }
    }

    /// <summary>
    /// Adds visual controls for specified members of a Internal.
    /// </summary>
    private static void AddReadonlyControls(string[] visualizeMembers, Node node, Control readonlyMembers, List<Action> updateControls, List<VisualSpinBox> spinBoxes)
    {
        if (visualizeMembers == null)
        {
            return;
        }
        
        foreach (string visualMember in visualizeMembers)
        {
            TryGetMemberInfo(node, visualMember, out PropertyInfo property, out FieldInfo field, out object initialValue);

            // If neither property nor field is found, skip this member
            if (property == null && field == null)
            {
                continue;
            }

            if (initialValue != null)
            {
                AddReadonlyControl(visualMember, readonlyMembers, node, field, property, initialValue, updateControls, spinBoxes);
            }
            else
            {
                _ = TryAddReadonlyControlAsync(visualMember, readonlyMembers, node, field, property, updateControls, spinBoxes);
            }
        }
    }

    /// <summary>
    /// Creates a canvas layer for a Internal with a specified name and instance ID.
    /// </summary>
    private static CanvasLayer CreateCanvasLayer(string name, ulong instanceId)
    {
        CanvasLayer canvasLayer = new();
        canvasLayer.FollowViewportEnabled = true;
        canvasLayer.Name = $"Visualizing {name} {instanceId}";
        return canvasLayer;
    }

    /// <summary>
    /// Asynchronously tries to add a visual control for a Internal member.
    /// </summary>
    private static async Task TryAddReadonlyControlAsync(string visualMember, Control readonlyMembers, Node node, FieldInfo field, PropertyInfo property, List<Action> updateControls, List<VisualSpinBox> spinBoxes)
    {
        CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;

        int elapsedSeconds = 0;

        while (!token.IsCancellationRequested)
        {
            object value = null;

            if (field != null)
            {
                value = field.GetValue(node);
            }
            else if (property != null)
            {
                value = property.GetValue(node);
            }

            if (value != null)
            {
                AddReadonlyControl(visualMember, readonlyMembers, node, field, property, value, updateControls, spinBoxes);
                break;
            }

            try
            {
                const int OneSecondInMs = 1000;
                await Task.Delay(OneSecondInMs, token);
                elapsedSeconds++;

                if (elapsedSeconds == MaxSecondsToWaitForInitialValues)
                {
                    string memberName = string.Empty;

                    if (field != null)
                    {
                        memberName = field.Name;
                    }
                    else if (property != null)
                    {
                        memberName = property.Name;
                    }

                    GD.PrintRich($"[color=orange][Visualize] Tracking '{node.Name}' to see if '{memberName}' value changes[/color]");
                }
            }
            catch (TaskCanceledException)
            {
                // Task was cancelled, exit the loop
                break;
            }
        }
    }

    /// <summary>
    /// Adds a visual control to the UI for a Internal member.
    /// </summary>
    private static void AddReadonlyControl(string visualMember, Control readonlyMembers, Node node, FieldInfo field, PropertyInfo property, object initialValue, List<Action> updateControls, List<VisualSpinBox> spinBoxes)
    {
        Type memberType = property != null ? property.PropertyType : field.FieldType;

        MemberInfo member = property != null ? property : field;

        VisualControlContext context = new(spinBoxes, initialValue, _ =>
        {
            // Do nothing
        });

        VisualControlInfo visualControlInfo = VisualControlTypes.CreateControlForType(memberType, member, context);

        visualControlInfo.VisualControl.SetEditable(false);

        updateControls.Add(() =>
        {
            object newValue = property != null
                ? property.GetValue(property.GetGetMethod(true).IsStatic ? null : node)
                : field.GetValue(field.IsStatic ? null : node);

            visualControlInfo.VisualControl.SetValue(newValue);
        });

        HBoxContainer hbox = new();
        hbox.Name = visualMember;

        hbox.AddChild(new Label { Text = visualMember });
        hbox.AddChild(visualControlInfo.VisualControl.Control);

        readonlyMembers.AddChild(hbox);
    }

    /// <summary>
    /// Adds member information elements to a VBoxContainer.
    /// </summary>
    private static void AddMutableControls(Control vbox, IEnumerable<MemberInfo> members, Node node, List<VisualSpinBox> spinBoxes)
    {
        foreach (MemberInfo member in members)
        {
            Control element = CreateMemberInfoElement(member, node, spinBoxes);

            if (element != null)
                vbox.AddChild(element);
        }
    }

    /// <summary>
    /// Creates a member information element for a specified Internal member.
    /// </summary>
    private static Control CreateMemberInfoElement(MemberInfo member, Node node, List<VisualSpinBox> spinBoxes)
    {
        Type type = VisualHandler.GetMemberType(member);

        object initialValue = VisualHandler.GetMemberValue(member, node);

        if (initialValue == null)
        {
            PrintUtils.Warning($"[Visualize] '{member.Name}' value in '{node.Name}' is null");
            return null;
        }

        VisualControlInfo element = VisualControlTypes.CreateControlForType(type, member, new VisualControlContext(spinBoxes, initialValue, v => 
        {
            VisualHandler.SetMemberValue(member, node, v);
        }));

        Control container;
        Label label = new();

        if (element.VisualControl is ClassControl)
        {
            container = new VBoxContainer();
            label.LabelSettings = new LabelSettings
            {
                FontSize = MemberFontSize,
                OutlineSize = FontOutlineSize,
                OutlineColor = Colors.Black,
            };
        }
        else
        {
            container = new HBoxContainer();
        }

        label.Text = member.Name.ToPascalCase().AddSpaceBeforeEachCapital();
        label.HorizontalAlignment = HorizontalAlignment.Center;
        container.Name = member.Name;

        if (element.VisualControl == null)
            return container;

        container.AddChild(label);
        container.AddChild(element.VisualControl.Control);

        return container;
    }
}
#endif
