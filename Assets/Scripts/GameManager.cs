using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Grid Grid;
    public InputManager InputManager;
    public LegoMovementManager MovementManager;

    public GroundTile GroundTile;
    public List<LegoGate> Gates;
    public LegoPiece LegoPiecePrefab;
    public LegoGateBlock GateBlock;
    
    public static GameManager Instance;

    private GroundTile _lastHoveredGroundTile;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var levelWidthX = 5;
        var levelWidthY = 7;
        
        Grid = new Grid(levelWidthX, levelWidthY);
        MovementManager.Initialize(Grid, InputManager);
        
        for (int i = 0; i < levelWidthX; i++)
        {
            for (int j = 0; j < levelWidthY; j++)
            {
                var newBlockItem = new GridItem();
                Grid.GridItems[i, j] = newBlockItem;
                newBlockItem.x = i;
                newBlockItem.y = j;
                
                var tile = Instantiate(GroundTile);

                tile.transform.position = new Vector3(i, j, 0.2f);
                tile.MasterGridItem = newBlockItem;
                newBlockItem.GroundPiece = tile;
            }
        }
        
        bool[,] legoShapeCorner = {
            { false, true }, 
            { true, true }
        };
           
        bool[,] legoShapeL = {
            { true, true,true },
            { false, false,true }
        };   
        bool[,] legoShapeI = {
            { true, true }
        };

        LegoShapeData data1 = new LegoShapeData(legoShapeL, LegoPiece.LegoColor.ORANGE);
        LegoShapeData data2 = new LegoShapeData(legoShapeI, LegoPiece.LegoColor.YELLOW);
        LegoShapeData data3 = new LegoShapeData(legoShapeI, LegoPiece.LegoColor.ORANGE);

        LegoShapeFactory shapeFactory = new LegoShapeFactory(LegoPiecePrefab);
        LegoShape shape1 = shapeFactory.CreateLegoShape(data1);
        LegoShape shape2 = shapeFactory.CreateLegoShape(data2);
        LegoShape shape3 = shapeFactory.CreateLegoShape(data3);
        
        Grid.PlaceShapeToGrid(1,3,shape1);
        Grid.PlaceShapeToGrid(0,5,shape2);
        Grid.PlaceShapeToGrid(2,1,shape3);
        
        Gates = new List<LegoGate>();

        var builder = new LegoGateBuilder(Grid, GateBlock);
        builder.GenerateLegoWalls( 2,-1,1, LegoPiece.LegoColor.ORANGE, false);
        builder.GenerateLegoWalls( 3,-1,2, LegoPiece.LegoColor.YELLOW, false);
        builder.GenerateBlockingWallCoordinates();
        var gates = builder.GetAllGates();
        
        foreach (var wallCoordinate in gates)
        {
            wallCoordinate.GenerateWall();
            Gates.Add(wallCoordinate);
        }
    }

    public void CheckGates(LegoShape shape)
    {
        foreach (var legoGate in Gates)
        {
            if (legoGate.Type == LegoGate.GateType.BLOCKER || !legoGate.CanShapeGoThrough(shape)) continue;
            
            InputManager.LockInput();
            Grid.RemoveFromGrid(shape);
            //shape.DestroyLegoAnimation(()=>InputManager.UnlockInput());
        }
    }
}
