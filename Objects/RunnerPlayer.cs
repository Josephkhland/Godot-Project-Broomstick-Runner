using Godot;
using System;

public class RunnerPlayer : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    Position2D[] AvailablePositions = new Position2D[4] { null, null, null, null };
    Node ParentNode = null;
    int position_index = 0;
    bool InMotion = false;
    Vector2 CurrentPosition;
    Vector2 NextPosition;
    [Export]
    int Speed = 300;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (GetParent() != null)
        {
            ParentNode = GetParent();
            for (int i=1; i<5; i++)
            {
                string path = $"PlayerPositions/Lane{i}Spot";
                Node PositionNode = ParentNode.GetNode(path);
                if (PositionNode != null)
                {
                    AvailablePositions[i - 1] = PositionNode as Position2D;
                }
                else
                {
                    GD.Print($"'{path}' Not Found.");
                }
            }
            if (AvailablePositions[position_index] != null)
            {
                Position = AvailablePositions[position_index].Position;
            }
        }
        else
        {
            GD.Print("Parent Not Found.");
        }
    }

    private void GetInput()
    {
        if (InMotion) return;
        if (Input.IsActionPressed("up"))
        {
            if (position_index == 0) return;
            else
            {
                //Move up
                GD.Print("UP");
                InMotion = true;
                CurrentPosition = AvailablePositions[position_index].Position;
                NextPosition = AvailablePositions[position_index - 1].Position;
                position_index -= 1;
            }
        }
        else if (Input.IsActionPressed("down"))
        {
            if (position_index == 3) return;
            else
            {
                //Move Down
                GD.Print("DOWN");
                InMotion = true;
                CurrentPosition = AvailablePositions[position_index].Position;
                NextPosition = AvailablePositions[position_index + 1].Position;
                position_index += 1;
                
            }
        }
    }

    private void Move()
    {
        if (InMotion)
        {
            Vector2 Direction = CurrentPosition.DirectionTo(NextPosition);
            Vector2 Velocity = Direction * Speed;

            if (Position == NextPosition)
            {
                InMotion = false;
                return;
            }

            Vector2 ActualDirection = Position.DirectionTo(NextPosition);
            if (ActualDirection.AngleTo(Direction)> Mathf.Deg2Rad(90) || ActualDirection.AngleTo(Direction) < Mathf.Deg2Rad(-90))
            {
                //We have just crossed over the point. So let's just snap.
                Position = NextPosition;
                InMotion = false;
            }
            else 
                MoveAndSlide(Velocity);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        Move();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
   public override void _Process(float delta)
   {
        GetInput();
   }
}
