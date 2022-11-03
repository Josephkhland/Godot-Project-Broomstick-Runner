extends KinematicBody2D


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
var Direction = Vector2.LEFT
var Speed = 400;

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _physics_process(delta):
	move_and_slide(Direction*Speed);

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if (position.x <-50):
		queue_free()
