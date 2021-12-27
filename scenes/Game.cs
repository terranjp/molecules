using Godot;
using System;

public class Game : Node2D
{

    private Control _mainMenu;
    private Node2D _molecules;
    private Label _messageLabel;
    private AudioStreamPlayer _music;

    float totalMoleculeMass = 0;


    // Vector2 screenSize = new Vector2((float)ProjectSettings.GetSetting("display/window/size/width"), (float)ProjectSettings.GetSetting("display/window/size/height"));
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetTree().Paused = true;
        GD.Randomize();

        _mainMenu = GetNode<Control>("MainMenu");
        _molecules = GetNode<Node2D>("Molecules");
        _messageLabel = GetNode<Label>("Message/Label");
        _music = GetNode<AudioStreamPlayer>("Music");

        _mainMenu.Connect("request_new_level", this, "generateMolecules");
        _mainMenu.Connect("request_music", this, "_on_music_request");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (_messageLabel.Text.Length > 20)
        {
            if (Input.IsActionJustPressed("dismiss"))
            {
                _messageLabel.Text = "";
                _mainMenu.Hide();
                GetTree().Paused = false;
            }

        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
