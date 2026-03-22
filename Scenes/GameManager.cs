using Godot;
using System;

public partial class GameManager : Node
{

	private static GameManager _instance;
	public static GameManager Instance { get { return _instance; } }
	public GameManager GDInstance { get { return _instance; } }


	public HandControl HCont;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//singleton
		if(Instance == null)
		{
			_instance = this;
			this.ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			GD.PrintErr($"Two Game Managers Created. Deleting {Name}");
			QueueFree();
			return;
		}
		GD.Print(CreatePassword("captcha", 50));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="passwordType">"captcha" provides a string of alphanumerical characters. "sequence" provides a string of compass directions. "pin" provides a string of numbers</param>
	/// <returns></returns>
	public virtual string CreatePassword(string passwordType = "captcha", int passwordLength = 6)
	{
		// Source - https://stackoverflow.com/a/1344258
		// Posted by Dan Rigby, modified by community. See post 'Timeline' for change history
		// Retrieved 2026-03-21, License - CC BY-SA 4.0
		string password = "default";
        Random random = new Random();


        switch (passwordType)
		{
			case "captcha":
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[{]},<.>/?`~|";
                char[] stringChars = new char[passwordLength +2];
                
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                password = new string(stringChars);
                break;
			case "sequence":
				string directions = "UDLR";
				char[] directionChars = new char[passwordLength];
                for (int i = 0; i < directionChars.Length; i++)
                {
                    directionChars[i] = directions[random.Next(directions.Length)];
                }
				password = new string(directionChars);
                break;
			case "pin":
				string nums = "1234567890";
                char[] numChars = new char[passwordLength];
                for (int i = 0; i < numChars.Length; i++)
                {
                    numChars[i] = nums[random.Next(nums.Length)];
                }
				password = new string(numChars);
                break;
			default:
				break;
		}
		return password;

    }
}
