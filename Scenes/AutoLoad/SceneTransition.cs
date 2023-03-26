using Godot;
using System;

public class SceneTransition : CanvasLayer
{
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
    }

    public async void ChangeScene(string target, bool transition = false)
    {
        if (transition)
        {
            _animationPlayer.Play("Dissolve");
            await ToSignal(_animationPlayer, "animation_finished");
            GetTree().ChangeScene(target);
            _animationPlayer.PlayBackwards("Dissolve");
        }
        else
        {
            GetTree().ChangeScene(target);

        }
    }
}
