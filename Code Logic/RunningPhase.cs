using Godot;
using System;

public class RunningPhase : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    LevelGenerator Generator = new LevelGenerator();
    int LevelProgress = -1;
    float CurrentStepTimer = 0;
    float StepTimer = 0.3F;

    //Packed Scenes
    PackedScene BaseObstacle = GD.Load<PackedScene>("res://Objects/Obstacle.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Utilities UtilitiesFunctions = GetNode<Utilities>("/root/Utilities");
        //Generator.AddLevelGenerationSeed(UtilitiesFunctions.GetCurrentLevelSeed());
        Generator.GenerateLanes();
    }

    private void GenerateObject(byte Item, int Lane)
    {
        switch (Item)
        {
            case (byte)Utilities.TileContent.Obstacle:
                Vector2 SpawnPoint = GetNode<Position2D>($"Spawners/Lane{Lane}Spawn").Position;
                KinematicBody2D instance = BaseObstacle.Instance() as KinematicBody2D;
                instance.Position = SpawnPoint;
                AddChild(instance);
                break;
            default:
                break;
        }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        CurrentStepTimer += delta;
        //GD.Print($"DELTA: {delta}");
        if (CurrentStepTimer>= StepTimer)
        {
            CurrentStepTimer = 0;
            LevelProgress += 1;
            GD.Print(LevelProgress);
            if (LevelProgress< Generator.Lanes[0].Length && LevelProgress >= 0)
            {
                byte ObjectForLane1 = Generator.Lanes[0][LevelProgress];
                byte ObjectForLane2 = Generator.Lanes[1][LevelProgress];
                byte ObjectForLane3 = Generator.Lanes[2][LevelProgress];
                byte ObjectForLane4 = Generator.Lanes[3][LevelProgress];
                GD.Print($"[{ObjectForLane1}][{ObjectForLane2}][{ObjectForLane3}][{ObjectForLane4}]");
                GenerateObject(ObjectForLane1, 1);
                GenerateObject(ObjectForLane2, 2);
                GenerateObject(ObjectForLane3, 3);
                GenerateObject(ObjectForLane4, 4);
            }
            
        }
    }
}
