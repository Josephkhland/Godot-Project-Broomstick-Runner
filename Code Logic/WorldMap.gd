tool
extends Node2D


# Declare member variables here. Examples:
export var MapRectangle = Rect2(0,0,720,460) setget _set_rectangle

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _set_rectangle(value):
	MapRectangle = value
	$MapImage.rect_position = MapRectangle.position
	$MapImage.rect_size = MapRectangle.size
	
	$MapBorder.clear_points()
	var x = MapRectangle.position.x
	var y = MapRectangle.position.y
	var npoint = Vector2(x,y)
	
	$MapBorder.add_point(npoint)
	
	x += MapRectangle.size.x
	npoint = Vector2(x,y)
	$MapBorder.add_point(npoint)
	
	y += MapRectangle.size.y
	npoint = Vector2(x,y)
	$MapBorder.add_point(npoint)
	
	x -= MapRectangle.size.x
	npoint = Vector2(x,y)
	$MapBorder.add_point(npoint)
	
	y -= MapRectangle.size.y
	npoint = Vector2(x,y)
	$MapBorder.add_point(npoint)
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
