extends Node


var Health :int
var MaxHealth: int
var BaseSpeed: int
var VerticalSpeed: int
var Coordinates: Vector2


# Called when the node enters the scene tree for the first time.
func _ready():
	MaxHealth = 100
	Health = MaxHealth
	
	BaseSpeed = 400
	VerticalSpeed = 600
	Coordinates = Vector2.ZERO
	#These parameters could be loaded from files during the Load Operation.


func LoadGame():
	print("Error: res://Code Logic/PlayerGlobals.gd/LoadGame Function is not implemented yet.")

func SaveGame():
	print("Error: res://Code Logic/PlayerGlobals.gd/SaveGame Function is not implemented yet.")
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
