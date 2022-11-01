using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Utilities : Node
{
    // Declare member variables here. Examples:
    float MaxSeparationDistance = 16;
    #region Structs

    public struct Polyline
    {
        public Vector2[] Points { get; set; }
        public float Length 
        { 
            get
            {
                if (Length == -1)
                {
                    Length = CalculatePolylineLength();
                }
                return Length;
            }
            set{ }
        }

        //Public Functions
        public void AddPoint(Vector2 Point)
        {
            Vector2 PreviousLastPoint = new Vector2(Points[Points.Length-1]);
            float AdditionalDistance = PreviousLastPoint.DistanceTo(Point);

            List<Vector2> AsList = new List<Vector2>(Points);
            AsList.Add(Point);
            Points = AsList.ToArray();
            Length += AdditionalDistance;
        }

        //Private Functions
        private float CalculatePolylineLength()
        {
            if (Points.Length < 2) return 0;
            float SumLength = 0;
            for (int point_i = 0; point_i < Points.Length - 1; point_i++)
            {
                Vector2 PointA = new Vector2(Points[point_i]);
                Vector2 PointB = new Vector2(Points[point_i + 1]);
                SumLength += PointA.DistanceTo(PointB);
            }
            return SumLength;
        }
        public Polyline(Vector2[] Points)
        {
            this.Points = Points;
            Length = -1;
        }
    }

    public struct MapRegion
    {
        public int RegionID { get; set; } 
        public string Name { get; set; }
        public Polygon2D polygon { get; set; }
        public int[] CollectiblesUncommon { get; set; }
        public int[] CollectiblesRare { get; set; }
        public int CollectibleVeryRare { get; set; }
        public int[] EnemiesCommon { get; set; }
        public int EnemyUncommon { get; set; }
        public int EnemyRare { get; set; }
        public int Boss { get; set; }

        public MapRegion(int RegionID, string Name, Polygon2D polygon) 
        {
            this.RegionID = RegionID;
            this.Name = Name;
            this.polygon = polygon;
            CollectiblesUncommon = new int[3] { -1, -1, -1 };
            CollectiblesRare = new int[2] { -1, -1 };
            CollectibleVeryRare = -1;
            EnemiesCommon = new int[2] {-1,-1};
            EnemyUncommon = -1;
            EnemyRare = -1;
            Boss = -1;
        }
    }

    public struct RegionsOrderStruct
    {
        public int RegionID;
        public int StartingIndex;
    }

    public struct LevelGeneratorData
    {
        public int LevelLength { get; set; }
        public List<RegionsOrderStruct> RegionsOrder { get; set; }

        public LevelGeneratorData(Polyline polyline, List<RegionsOrderStruct> RegionsOrder)
        {
            LevelLength = (int) Math.Floor(polyline.Length*2);
            this.RegionsOrder = new List<RegionsOrderStruct>(RegionsOrder);
        }
    }

    #endregion

    #region Dictionaries
    Dictionary<int, MapRegion> AllRegions = new Dictionary<int, MapRegion>();

    #endregion

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    /// <summary>
    /// Given a Polyline, a point is found whose distance from the beginning of the Polyline when travelling over the Polyline is equal to the given distance.
    /// </summary>
    /// <param name="polyline">The Line that is traversed</param>
    /// <param name="DistanceAlongLine">The Distance of the found point from the beginning of the Polyline</param>
    /// <param name="Extension"> Whether to search for points in the extensions of the Polyline</param>
    /// <returns>Vector2 Point</returns>
    public Vector2 MovePointAlongLine(Polyline polyline, float DistanceAlongLine, bool Extension = false)
    {
        float SumLength = 0;
        Vector2 PointResult = new Vector2();
        if (DistanceAlongLine < 0 & polyline.Points.Length >2)
        {
            if (Extension)
            {
                Vector2 PointA = new Vector2(polyline.Points[0]);
                Vector2 PointB = new Vector2(polyline.Points[1]);
                PointResult = PointB.DirectionTo(PointA) * Math.Abs(DistanceAlongLine);
                PointResult = PointA + PointResult;
                return PointResult;
            }
            else
            {
                return new Vector2(polyline.Points[0]);
            }
            
        }
        if (DistanceAlongLine > polyline.Length & polyline.Points.Length > 2)
        {
            if (Extension)
            {
                Vector2 PointA = new Vector2(polyline.Points[polyline.Points.Length-2]);
                Vector2 PointB = new Vector2(polyline.Points[polyline.Points.Length-1]);
                PointResult = PointA.DirectionTo(PointB) * Math.Abs(DistanceAlongLine);
                PointResult = PointA + PointResult;
                return PointResult;
            }
            else
            {
                return new Vector2(polyline.Points[polyline.Points.Length - 1]);
            }
        }
        for (int point_i=0; point_i<polyline.Points.Length-1; point_i++)
        {
            Vector2 PointA = new Vector2(polyline.Points[point_i]);
            Vector2 PointB = new Vector2(polyline.Points[point_i + 1]);
            float DistanceFromPrevious = PointA.DistanceTo(PointB);
            if (SumLength + DistanceFromPrevious > DistanceAlongLine)
            {
                //The Point we seek is between PointA and PointB.
                PointResult = PointA.DirectionTo(PointB) * (DistanceAlongLine - DistanceFromPrevious);
                PointResult = PointResult + PointA;
                break;
            }
            else if (SumLength + DistanceFromPrevious == DistanceAlongLine)
            {
                //The Point we seek is after Point B
                PointResult = new Vector2(PointB);
                break;
            }
            else
            {
                SumLength += DistanceFromPrevious;
            }
        }
        return PointResult;
    }

    /// <summary>
    /// Given a Point, find the Closest Region to that Point.
    /// </summary>
    /// <param name="Point"></param>
    /// <returns>Returns the int id of the found Region, -1 if it's outside of every region.</returns>
    public int FindClosestRegion(Vector2 Point)
    {
        List<int> FoundRegions = AllRegions.Keys.Where(reg => Geometry.IsPointInPolygon(Point, AllRegions[reg].polygon.Polygon)).ToList();
        if (FoundRegions.Count > 0)
        {
            return FoundRegions[0];
        }
        else
        {
            List<int> OrderedRegions = AllRegions.Keys.OrderBy(reg => GetDistanceBetweenPointAndPolygon(Point, AllRegions[reg].polygon)).ToList();
            if (GetDistanceBetweenPointAndPolygon(Point, AllRegions[OrderedRegions[0]].polygon) > MaxSeparationDistance) return -1;
            else return OrderedRegions[0];
        }
       
    }
    
    /// <summary>
    /// Calculates the distance between a Point and a Polygon.
    /// </summary>
    /// <param name="Point">A Vector2 Point, expecting Global Coordinates.</param>
    /// <param name="polygon">The target Polygon2D</param>
    /// <returns>float Distance of Point to its Closest point upon the Polygon</returns>
    public float GetDistanceBetweenPointAndPolygon(Vector2 Point, Polygon2D polygon)
    {
        Vector2[] PolygonPoints = polygon.Polygon;
        float distance = float.MaxValue;
        for(int point_i = 0; point_i < PolygonPoints.Length-1; point_i++)
        {
            Vector2 SegmentVertice1 = polygon.ToGlobal(PolygonPoints[point_i]);
            Vector2 SegmentVertice2 = polygon.ToGlobal(PolygonPoints[point_i + 1]);
            Vector2 ClosestPoint = Geometry.GetClosestPointToSegment2d(Point, SegmentVertice1, SegmentVertice2);
            float distanceToPoint = Point.DistanceTo(ClosestPoint);
            if (distanceToPoint < distance)
            {
                distance = distanceToPoint;
            }
        }
        return distance;
    }

    /// <summary>
    /// Given a Polygon2D and a Name it creates a MapRegion and adds it to the AllRegions Dictionary
    /// </summary>
    /// <param name="Name">The name of the MapRegion, also used as the Key to the AllRegions Dictionary</param>
    /// <param name="polygon">The Polygon2D data of the Region</param>
    /// <returns>RegionID of the Region if it was added, -1 if a Region with that Name already Existed</returns>
    public int AddRegion(string Name, Polygon2D polygon)
    {
        if (AllRegions.Values.Any(reg => reg.Name == Name)) return -1;
        int newRegionID = AllRegions.Keys.Max() + 1;
        MapRegion newMapRegion = new MapRegion(newRegionID, Name, polygon);
        AllRegions.Add(newRegionID, newMapRegion);
        return newRegionID;
    }

    public LevelGeneratorData FindLevelGeneratorData(Line2D route)
    {
        List<Vector2> TransformedPoints = new List<Vector2>();
        foreach (Vector2 Point in route.Points)
        {
            Vector2 GPoint = route.ToGlobal(Point);
            TransformedPoints.Add(GPoint);
        }
        Vector2[] tPointsArray = TransformedPoints.ToArray();
        Polyline polyline = new Polyline(tPointsArray);

        //Get the Travel steps for every 8 units
        List<Vector2> TravelSteps = new List<Vector2>();
        for (int t_dist = 0; t_dist < Math.Floor(polyline.Length); t_dist += 8)
        {
            TravelSteps.Add(MovePointAlongLine(polyline, t_dist));
        }

        List<RegionsOrderStruct> OutputList = new List<RegionsOrderStruct>();
        int StepsCounter = 0;
        int PreviousRegion = -1;
        foreach (Vector2 step in TravelSteps)
        {
            int FoundRegion = FindClosestRegion(step);
            if (PreviousRegion != FoundRegion)
            {
                RegionsOrderStruct orderStruct = new RegionsOrderStruct();
                orderStruct.RegionID = FoundRegion;
                orderStruct.StartingIndex = StepsCounter * 16;
                OutputList.Add(orderStruct);
            }
            PreviousRegion = FoundRegion;
            StepsCounter++;
        }
        LevelGeneratorData level_seed = new LevelGeneratorData(polyline, OutputList);
        return level_seed;
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
