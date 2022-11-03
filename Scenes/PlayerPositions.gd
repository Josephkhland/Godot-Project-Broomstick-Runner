tool
extends Node2D


# Declare member variables here. Examples:
export onready var DefaultXPosition = 480 setget _setDefaultXPosition
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _setDefaultXPosition(value):
	DefaultXPosition = value
	$Lane1Spot.position.x = DefaultXPosition
	$Lane2Spot.position.x = DefaultXPosition
	$Lane3Spot.position.x = DefaultXPosition
	$Lane4Spot.position.x = DefaultXPosition

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
