using Godot;
using System;
using System.Collections.Generic;

public class LevelGenerator : Node
{
	// Declare member variables here. Examples:

	

	enum MovementOptions
	{
		Up,
		Down,
		Forward
	}

	
	private List<Utilities.RegionsOrderStruct> RegionsOrder= new List<Utilities.RegionsOrderStruct>();
	private int LevelLength = 48000; //Number of Bytes per Level Lane. This Size is calculated by dividing the Vector Distance of the Line by 8 and multiplying it by 16. (Or just multiplying it by two and flooring it).

	[Export]
	public int MaxLevelSize = 48000; 
	private int MaxArraySize = 96000; //96 kb -> Reached for Vector Distance 48000.

	public Dictionary<int, byte[]> Lanes = new Dictionary<int, byte[]>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MaxArraySize = MaxLevelSize * 2;
	}

	//Regions Order
	public void AddLevelGenerationSeed(Utilities.LevelGeneratorData level_seed)
	{
		LevelLength = level_seed.LevelLength;
		RegionsOrder = level_seed.RegionsOrder;
	}

	//Generate Lanes
	public void GenerateLanes()
	{
		InitializeLanes();
		GeneratePath();
		GenerateHazardsAndTreasures();
	}

	#region Lane Generation Functions

	/// <summary>
	/// Function used to initiliaze each Lane in the Lanes Dictionary, with an Empty Array of LevelLength Size. 
	/// </summary>
	private void InitializeLanes()
	{
		for (int laneID = 0; laneID < 4; laneID++)
		{
			byte[] NewLane = new byte[LevelLength];
			Lanes.Add(laneID, NewLane);
		}
	}

	/// <summary>
	/// Function used to Generate a Valid Path between the Start of the Level and the End of the Level, to ensure that the Player can potentially complete the level
	/// without crashing on obstacles.
	/// Additionally it generates the Required Mana Bands upon the path.
	/// </summary>
	private void GeneratePath()
	{
		//Generate A Path between the start of the level and the end. 
		Random random = new Random();
		int PreviousLane = -1;
		int PreviousTile = -1;
		int CurrentLane = random.Next(0, 4);
		int CurrentTile = 0;

		//Mana Shards Placement Parameters
		int MinimumManaBand = 3;
		int MaximumManaBand = 10;
		int MinimumManaGap = 1;
		int MaximumManaGap = 10;
		bool ManaPlacementOn = false;
		int ManaPlacementIndex = 0;
		int CurrentManaBandLength = random.Next(MinimumManaGap, MaximumManaGap);

		while (CurrentTile < LevelLength - 1)
		{
			//Set the CurrentTile,Lane as Empty
			Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.Empty;
			
			//Depending on the Mana Placement Parameters set a Mana Shard on the current Tile or leave it empty.
			if (ManaPlacementOn)
			{
				Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.ManaShard;
			}

			//Mana is placed in Bands, each Band of Mana Shards has a Gap between it and the previous Band of Mana Shards.
			ManaPlacementIndex++;
			if (ManaPlacementIndex >= CurrentManaBandLength)
			{
				ManaPlacementOn = !ManaPlacementOn; //Toggle ManaPlacementOn.
				if (ManaPlacementOn)
					CurrentManaBandLength = random.Next(MinimumManaBand, MaximumManaBand);
				else
					CurrentManaBandLength = random.Next(MaximumManaGap, MaximumManaGap);
				ManaPlacementIndex = 0;
			}

			List<MovementOptions> AvailableMoves = new List<MovementOptions>();
			AvailableMoves.Add(MovementOptions.Forward);
			switch (CurrentLane)
			{
				case 0:
					if (PreviousLane != 1 && PreviousTile != CurrentTile)
					{
						AvailableMoves.Add(MovementOptions.Down);
					}
					break;
				case 1:
					if (PreviousLane != 0 && PreviousTile != CurrentTile)
					{
						AvailableMoves.Add(MovementOptions.Up);
					}
					if (PreviousLane != 2 && PreviousTile != CurrentTile)
					{
						AvailableMoves.Add(MovementOptions.Down);
					}
					break;
				case 2:
					if (PreviousLane != 1 && PreviousTile != CurrentTile)
					{
						AvailableMoves.Add(MovementOptions.Up);
					}
					if (PreviousLane != 3 && PreviousTile != CurrentTile)
					{
						AvailableMoves.Add(MovementOptions.Down);
					}
					break;
				case 3:
					if (PreviousLane != 2 && PreviousTile != CurrentTile)
					{
						AvailableMoves.Add(MovementOptions.Up);
					}
					break;
			}
			int NextMoveIndex = random.Next(AvailableMoves.Count);
			MovementOptions NextMove = AvailableMoves[NextMoveIndex];

			PreviousTile = CurrentTile;
			PreviousLane = CurrentLane;
			switch (NextMove)
			{
				case MovementOptions.Up:
					CurrentLane -= 1;
					break;
				case MovementOptions.Down:
					CurrentLane += 1;
					break;
				case MovementOptions.Forward:
					CurrentTile += 1;
					break;
			}
		}
	}
	
	/// <summary>
	/// Function used to place obstacles, enemies, orbs and treasure upon the lanes.
	/// </summary>
	private void GenerateHazardsAndTreasures()
	{
		Random random = new Random();
		int EmptyOrObstacleChances = 9690; // Out of 10000
		int OrbsChances = 200; // Out of 10000
		int EnemiesChances = 100; // out of 10000
		int CollectiblesChance = 10;

		//In Case we need to Apply something different in the generation based on the Region (Meaning different Ratios of Obstacles, Orbs, Enemies etc). 
		int CurrentRegionIndex = 0;
		
		for (int CurrentTile = 0; CurrentTile < LevelLength; CurrentTile++)
		{
			if (CurrentRegionIndex != RegionsOrder.Count - 1 && RegionsOrder.Count >0)
			{
				if (CurrentTile == RegionsOrder[CurrentRegionIndex + 1].StartingIndex)
					CurrentRegionIndex++;
			}
			for (int CurrentLane = 0; CurrentLane < 4; CurrentLane++)
			{
				int d10000Result = random.Next(10000);
				if (Lanes[CurrentLane][CurrentTile] == (byte)Utilities.TileContent.Unitialized)
				{
					//Anything can be assigned here.
					//Empty tiles and Obstacles should have the highest chance. Followed by a small chance of Mana Shards or Orbs.
					//Then a smaller chance of enemies and an even smaller chance of collectibles.
					if (d10000Result < EmptyOrObstacleChances)
					{
						//Place Obstacles or leave Empty (with a chance to have mana shards in empty tiles).
						int d10Result = random.Next(10);
						if (d10Result < 3) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.Empty;
						else if (d10Result < 5) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.ManaShard;
						else Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.Obstacle;
					}
					else if( d10000Result >= EmptyOrObstacleChances && d10000Result < EmptyOrObstacleChances + OrbsChances)
					{
						//Place Orbs.
						int d10Result = random.Next(10);
						if (d10Result < 5) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.HealthOrb;
						else Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.SpeedOrb;
					}
					else if (d10000Result >= EmptyOrObstacleChances + OrbsChances && d10000Result < EmptyOrObstacleChances + OrbsChances + EnemiesChances)
					{
						int d100Result = random.Next(100);
						if (d100Result < 40) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.CommonEnemy1;
						else if (d100Result < 80) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.CommonEnemy2;
						else if (d100Result < 95) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.UncommonEnemy;
						else if (d100Result < 99) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.RareEnemy;
						else Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.BossEnemy;
					}
					else //What's left is for the Collectibles. 
					{
						int d100Result = random.Next(100);
						if (d100Result < 33) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.UncommonCollectible1;
						else if (d100Result < 66) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.UncommonCollectible2;
						else if (d100Result < 99) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.UncommonCollectible3;
						else
						{
							int RarerResult = random.Next(100);
							if (RarerResult < 45) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.RareCollectible1;
							else if (RarerResult < 90) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.RareCollectible2;
							else Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.VeryRareCollectible;
						}
					}
				}
				else //Empty or Mana Shard (Part of the Path Generated in the Previous Step). 
				{
					//In this case there should be a Minor Chance for the Tile Content to change to an Orb and an Even harder case for it to change to a Collectible.
					//Enemies or Obstacles can't be assigned here.
					if (d10000Result > 9800)
					{
						int d10Result = random.Next(10);
						if (d10Result < 6) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.HealthOrb;
						else if (d10Result < 9) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.SpeedOrb;
						else
						{
							int d100Result = random.Next(100);
							if (d100Result < 33) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.UncommonCollectible1;
							else if (d100Result < 66) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.UncommonCollectible2;
							else if (d100Result < 99) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.UncommonCollectible3;
							else
							{
								int RarerResult = random.Next(100);
								if (RarerResult < 45) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.RareCollectible1;
								else if (RarerResult < 90) Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.RareCollectible2;
								else Lanes[CurrentLane][CurrentTile] = (byte)Utilities.TileContent.VeryRareCollectible;
							}
						}
					}
				}
			}
		}
		
	}
	
	#endregion
	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
