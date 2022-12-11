extends Area2D


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
var Direction = Vector2.LEFT
var Speed = PlayerGlobals.BaseSpeed;
var ObjectHit = false

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _physics_process(delta):
	position.x = position.x - Speed*delta
	#move_and_slide(Direction*Speed);

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if (position.x <-50):
		queue_free()



func _on_Obstacle_body_entered(body):
	if body.name == "RunnerPlayer":
		PlayerGlobals.Health -=10;
		var printstring = "Health: " + String(PlayerGlobals.Health)+ "/" + String(PlayerGlobals.MaxHealth)
		print(printstring)
		$Tween.interpolate_property(self,"modulate", modulate,Color.blue,0.01,Tween.TRANS_BOUNCE,Tween.EASE_OUT)
		$Tween.start()


func _on_Tween_tween_completed(object, key):
	queue_free()

