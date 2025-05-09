using UnityEngine;

public class Grid
{
    public readonly GridItem[,] GridItems;
    
    public Grid(int width, int height)
    {
        GridItems = new GridItem[width, height];
    }
    
    public void AddShapeToGrid(int x, int y, LegoShape shape)
    {
        var (shapeDimensionX,shapeDimensionY) = shape.GetDimensions();
        for (var i = 0; i < shapeDimensionY; i++)
        {
            for (var j = 0; j < shapeDimensionX; j++)
            {
                if (i == 0 && j == 0) shape.Anchor = GridItems[x, y];

                var currentLegoPiece = shape.LegoPieces[j, i];
                if (currentLegoPiece == null) continue;
                
                var gridItemAtIndex = GridItems[x + j, y + i];
                gridItemAtIndex.LegoPiece = currentLegoPiece;
                currentLegoPiece.SetGridItem(gridItemAtIndex);
            }
        }
    }
    
    public void PlaceShapeToGrid(int x, int y, LegoShape shape)
    {
        MoveShapeDataToNewPosition(shape,x,y);
        shape.ShapeContainer.transform.localPosition = new Vector3(x, y, 0);
    }
  
    public void MoveShapeDataToNewPosition(LegoShape shape, int newX, int newY)
    {
        RemoveFromGrid(shape);
        AddShapeToGrid(newX, newY, shape);
    }

    public bool CanMoveShapeAnchorToPosition(LegoShape shape, int newX, int newY)
    {
        int blocksXLength = GridItems.GetLength(0);
        int blocksYLength = GridItems.GetLength(1);
        
        //New coordinates are in safe zone
        if (newX < 0 || newY < 0 || newX >= blocksXLength|| newY >= blocksYLength) return false;
        
        //Moving shape's all coordinates are in the safe zone
        var dimension = shape.GetDimensions();
        if(dimension.x + newX > blocksXLength ||
           dimension.y + newY > blocksYLength) return false;
        
        //Moving shape's blocks dont collide with existing blocks
        for (int i = 0; i < dimension.x; i++)
        {
            for (int y = 0; y < dimension.y; y++)
            {
                if(!shape.HasBlockAtRelativeIndex(i,y)) continue;

                var blockAtCurrentIndex = GridItems[newX + i, newY + y].LegoPiece;
                if (blockAtCurrentIndex == null) continue;
                if(blockAtCurrentIndex.OwnerLegoShape != shape) return false;
            }
        }

        return true;
    }

    public GridItem GetGridItemAtIndex(int x, int y)
    {
        return GridItems[x,y];
    }

    public (int x, int y) GetNewPositionOnGridByDirection(Direction direction, int x, int y)
    {
        var xNewPositionDiff = direction switch
        {
            Direction.RIGHT => 1,
            Direction.LEFT => -1,
            _ => 0
        };
        
        var yNewPositionDiff = direction switch
        {
            Direction.UP => 1,
            Direction.DOWN => -1,
            _ => 0
        };
        
        return (x +xNewPositionDiff, y + yNewPositionDiff);
    }

    public void RemoveFromGrid(LegoShape shape)
    {
        foreach (var shapeLegoPiece in shape.LegoPieces)
        {
            if(shapeLegoPiece == null || shapeLegoPiece.GridItem == null) continue;

            GridItems[shapeLegoPiece.x,shapeLegoPiece.y].LegoPiece = null;
            shapeLegoPiece.SetGridItem(null);
        }
    }
    
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }

    public bool TryGetGridItem(int x, int y, out GridItem item)
    {
        item = null;
        if (x < GridItems.GetLength(0) &&
            y < GridItems.GetLength(1) && x >= 0 && y >= 0)
        {
            item = GridItems[x, y];
            return true;
        }

        item = null;
        return false;
    }

    public bool IsTileDisabled(int x, int y)
    {
        if (x < GridItems.GetLength(0) &&
            y < GridItems.GetLength(1) && x >= 0 && y >= 0)
        {
            return true;
        }

        return false;
    }
}
