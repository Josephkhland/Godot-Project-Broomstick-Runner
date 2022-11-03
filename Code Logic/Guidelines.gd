tool
extends Node2D


# Declare member variables here. Examples:
export onready var OffsetFromTop = 243 setget _setOffsetFromTop
export onready var GapBetweenGuidelines = 64 setget _setGapBetweenGuidelines
export onready var SpawnersXPosition = 1400 setget _setSpawnersXPosition

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _setGapBetweenGuidelines(value):
	GapBetweenGuidelines = value
	RepositionLines()
	
func _setOffsetFromTop(value):
	OffsetFromTop = value
	RepositionLines()

func _setSpawnersXPosition(value):
	SpawnersXPosition = value
	
	get_parent().get_node("Spawners/Lane1Spawn").position.x = SpawnersXPosition
	get_parent().get_node("Spawners/Lane2Spawn").position.x = SpawnersXPosition
	get_parent().get_node("Spawners/Lane3Spawn").position.x = SpawnersXPosition
	get_parent().get_node("Spawners/Lane4Spawn").position.x = SpawnersXPosition

func RepositionLines():
	$GuidelineOne.points[0].y = OffsetFromTop
	$GuidelineOne.points[1].y = OffsetFromTop
	get_parent().get_node("Spawners/Lane1Spawn").position.y = $GuidelineOne.points[1].y + (GapBetweenGuidelines/2)
	get_parent().get_node("PLayerPositions/Lane1Spot").position.y = $GuidelineOne.points[1].y + (GapBetweenGuidelines/2)
	
	$GuidelineTwo.points[0].y = OffsetFromTop+GapBetweenGuidelines
	$GuidelineTwo.points[1].y = OffsetFromTop+GapBetweenGuidelines
	get_parent().get_node("Spawners/Lane2Spawn").position.y = $GuidelineTwo.points[1].y + (GapBetweenGuidelines/2)
	get_parent().get_node("PLayerPositions/Lane2Spot").position.y = $GuidelineOne.points[1].y + (GapBetweenGuidelines/2)
	
	$GuidelineThree.points[0].y = OffsetFromTop+2*GapBetweenGuidelines
	$GuidelineThree.points[1].y = OffsetFromTop+2*GapBetweenGuidelines
	get_parent().get_node("Spawners/Lane3Spawn").position.y = $GuidelineThree.points[1].y + (GapBetweenGuidelines/2)
	get_parent().get_node("PLayerPositions/Lane3Spot").position.y = $GuidelineOne.points[1].y + (GapBetweenGuidelines/2)
	
	
	$GuidelineFour.points[0].y = OffsetFromTop+3*GapBetweenGuidelines
	$GuidelineFour.points[1].y = OffsetFromTop+3*GapBetweenGuidelines
	get_parent().get_node("Spawners/Lane4Spawn").position.y = $GuidelineFour.points[1].y + (GapBetweenGuidelines/2)
	get_parent().get_node("PLayerPositions/Lane4Spot").position.y = $GuidelineOne.points[1].y + (GapBetweenGuidelines/2)
	
	$GuidelineFive.points[0].y = OffsetFromTop+4*GapBetweenGuidelines
	$GuidelineFive.points[1].y = OffsetFromTop+4*GapBetweenGuidelines
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
