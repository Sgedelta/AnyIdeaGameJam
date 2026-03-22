extends CollisionShape3D


@export var Follow : Node3D;


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:

	rotation = Follow.rotation;
	pass
