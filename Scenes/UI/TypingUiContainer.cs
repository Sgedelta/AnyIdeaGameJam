using Godot;
using System;

public partial class TypingUiContainer : PanelContainer
{
	private bool _showing = false;

	private LineEdit _le_input;
	private Tween _showHideTween;

	[Export] float showHideSpeed = 10;
	[Export] Vector2 showPos = new Vector2(0, 0);
	[Export] Vector2 hidePos = new Vector2(0, 450);
	[Export] Vector2 showScale = new Vector2(600, 450);
	[Export] Vector2 hideScale = new Vector2(600, 100);


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_le_input = GetNode<LineEdit>("TypingMargins/LineEdit");

		_le_input.FocusEntered += ShowScreen;
		_le_input.FocusExited += HideScreen;
		//FocusEntered += ShowScreen;
		//FocusExited += HideScreen;

		FocusTyping(true);

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void FocusTyping(bool focus)
	{
		if(focus)
		{
            _le_input.GrabFocus();
        }
		else
		{
			_le_input.ReleaseFocus();
		}
		
	}

	public override void _Input(InputEvent @event)
	{
		if (_le_input == null)
		{
			GD.PushError($"Computer {Name} Text Edit is null! Input Error!");
			return;
		}

		if (!_le_input.HasFocus())
		{
			//discard all other input if not focused
			return;
		}

		//we don't have to handle typing input, text edit does that.
		//we do have to check for entering though!
		if (@event.IsAction("ui_text_submit") && (@event as InputEventKey).IsActionReleased("ui_text_submit"))
		{
			ProcessTextInput();
		}
	}

	private void ProcessTextInput()
	{
		string exactText = _le_input.Text;

		switch(exactText)
		{

		}

		_le_input.Text = "";
	}

	public void HideScreenINSTANT()
	{
		this.Size = hideScale;
		this.Position = hidePos;
	}

	public void ShowScreenINSTANT()
	{
		this.Size = showScale;
		this.Position = showPos;
	}

	public void ShowScreen()
	{
		if (_showing)
		{
			return;
		}
		_showing = true;

		if (_showHideTween != null && _showHideTween.IsRunning())
		{
			_showHideTween.Kill();
		}

		_showHideTween = CreateTween().SetParallel(true);

		_showHideTween.TweenProperty(this, "position", hidePos - new Vector2(0, hideScale.Y), showHideSpeed * .2);
		_showHideTween.TweenProperty(this, "size", showScale, showHideSpeed).SetDelay(showHideSpeed * .2);
		_showHideTween.TweenProperty(this, "position", showPos, showHideSpeed).SetDelay(showHideSpeed * .2);

	}

	public void HideScreen()
	{
		if (!_showing)
		{
			return;
		}
		//double check focus
		if (HasFocus() || _le_input.HasFocus())
		{
			return;
		}
		
		_showing = false;

        if (_showHideTween != null && _showHideTween.IsRunning())
        {
            _showHideTween.Kill();
        }

        _showHideTween = CreateTween();
        _showHideTween = CreateTween().SetParallel(true);

        
        _showHideTween.TweenProperty(this, "size", hideScale, showHideSpeed);
        _showHideTween.TweenProperty(this, "position", hidePos - new Vector2(0, hideScale.Y), showHideSpeed);
        _showHideTween.TweenProperty(this, "position", hidePos, showHideSpeed * .2).SetDelay(showHideSpeed);
    }
}
