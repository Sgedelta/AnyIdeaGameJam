using Godot;
using Godot.Collections;
using System;

public partial class CameraController : Camera3D
{

	[Export] private Dictionary<string, Vector3> _cameraLocs = new Dictionary<string, Vector3>();
	private Tween _controlTween;
	[Export] private float _cameraSpeed = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Tween tester = CreateTween();

		tester.TweenCallback(Callable.From(() => { TweenCameraToLoc("fucked"); }));
        tester.TweenCallback(Callable.From(() => { TweenCameraToLoc("right"); })).SetDelay(_cameraSpeed * 2);

        tester.TweenCallback(Callable.From(() => { TweenCameraToLoc("backward"); })).SetDelay(_cameraSpeed * 2);

        tester.TweenCallback(Callable.From(() => { TweenCameraToLoc("fucked"); })).SetDelay(_cameraSpeed * 2);
        tester.TweenCallback(Callable.From(() => { TweenCameraToLoc("backward"); })).SetDelay(_cameraSpeed * 2);
        tester.TweenCallback(Callable.From(() => { TweenCameraToLoc("forward"); })).SetDelay(_cameraSpeed * 2);
        tester.TweenCallback(Callable.From(() => { TweenCameraToLoc("backward"); })).SetDelay(_cameraSpeed * 2);
        tester.TweenCallback(Callable.From(() => { TweenCameraToLoc("forward"); })).SetDelay(_cameraSpeed * 2);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}


	public void TweenCameraToLoc(string locName)
	{
		if (!_cameraLocs.ContainsKey(locName))
		{
			GD.PrintErr($"provided loc name {locName} was not in location dictionary!");
			throw new ArgumentException($"provided loc name {locName} was not in location dictionary!");
		}

		if(_controlTween != null && _controlTween.IsRunning())
		{
			_controlTween.Kill();
		}

		//do a bit of stuff here to make sure we are taking a shortest path
		Vector3 locToGoTo = _cameraLocs[locName];
			//get abs distance, between 0 and 360 (more than that is pointless)
		Vector3 diff = new Vector3((locToGoTo.X - RotationDegrees.X)%360, (locToGoTo.Y - RotationDegrees.Y) %360, (locToGoTo.Z - RotationDegrees.Z) %360);
			//now, if its more than 180, subtract from 360 (negative to keep direction)
		diff = new Vector3(
			Mathf.Abs(diff.X) > 180 ? -360 + diff.X : diff.X,
            Mathf.Abs(diff.Y) > 180 ? -360 + diff.Y : diff.Y,
            Mathf.Abs(diff.Z) > 180 ? -360 + diff.Z : diff.Z
            );
			//get change and apply to current position so we "snap" to the right position no matter where we are
		locToGoTo = RotationDegrees + diff;

		_controlTween = CreateTween();

		_controlTween.TweenProperty(this, "rotation", new Vector3(Mathf.DegToRad(locToGoTo.X), Mathf.DegToRad(locToGoTo.Y), Mathf.DegToRad(locToGoTo.Z)), _cameraSpeed);
	}

}
