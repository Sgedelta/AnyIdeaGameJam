using Godot;
using System;

public partial class TypingUiContainer : PanelContainer
{
	[Signal] delegate void PasswordCorrectEventHandler();
	[Export] bool _enterCapVersion = false;

	private bool _showing = false;

	private LineEdit _le_input;
	private RichTextLabel _rich_output; 
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
		_rich_output = GetNode<RichTextLabel>("DisplayMargins/Display");

		_le_input.FocusEntered += ShowScreen;
		_le_input.FocusExited += HideScreen;
		//FocusEntered += ShowScreen;
		//FocusExited += HideScreen;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void FocusTyping(bool focus)
	{
		if (focus)
		{
			_le_input.GrabFocus();
			if (_enterCapVersion)
			{
				_rich_output.AppendText("[code]Enter Captcha...[/code]");
			}
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
		_rich_output.Clear();
        _le_input.Text = "";

		if(_enterCapVersion)
		{
            if (exactText == GameManager.Instance.CaptchaAnswer)
            {
				EmitSignal(SignalName.PasswordCorrect);
                _rich_output.AppendText("[code]Correct! Valve Unlocked![/code]");
            }
			else
			{
                _rich_output.AppendText("[code]Invalid Input! Captcha Regenerated![/code]");
            }
            GameManager.Instance.CreatePassword("captcha", 10);

            return;
		}

        //no one told you it was okay to program like this.
        switch (exactText)
		{
			case "engine_on":
				GameManager.Instance.CaptchaAnswer = GameManager.Instance.CreatePassword("captcha", 10);
				//ToDo GENERATE CAPTCHA AND PASS TO GM
				_rich_output.AppendText($"[code]Please prove you are not human.[br][/code][font=res://Assets/Fonts/crooked/Crooked.ttf][s]{GameManager.Instance.CaptchaAnswer}[/s][/font]");
				break;

			case "starboard_pass":
				GameManager.Instance.DirKeypadAnswer = GameManager.Instance.CreatePassword("sequence", 6);
				//ToDo GENERATE PASS AND PASS TO GM
				_rich_output.AppendText($"[code]Starboard Engine Password is: {GameManager.Instance.DirKeypadAnswer}[/code]");
				break;

			case "port_pass":
				GameManager.Instance.NumKeypadAnswer = GameManager.Instance.CreatePassword("pin", 6);
				//ToDo GENERATE PASS AND PASS TO GM
				_rich_output.AppendText($"[code]Port Engine Password is: {GameManager.Instance.NumKeypadAnswer}[/code]");
				break;

			case "HELP!":
				_rich_output.AppendText("[code]Valid Inputs:[br]HELP! - Get Help![br]port_pass - see port engine password[br]starboard_pass - see starboard engine password[br]engine_on - begin engine engage sequence[code]");
				break;
			default:
				_rich_output.AppendText("[code]Invalid Input! use \"HELP!\"![/code]");
				break;

		}

		
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

		_rich_output.Clear();
		_le_input.Text = "";

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

		_rich_output.Clear();
		_le_input.Text = "";

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
