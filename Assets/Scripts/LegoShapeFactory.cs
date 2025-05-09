using System.Collections.Generic;
using UnityEngine;

public class LegoShapeFactory
{
    private readonly LegoPiece _legoPiecePrefab;

    public LegoShapeFactory(LegoPiece legoPiecePrefab)
    {
        _legoPiecePrefab = legoPiecePrefab;
    }
    
    public LegoShape CreateLegoShape(LegoShapeData data)
    {
        var newLegoShape = new LegoShape();
        var legoPieceExistsMap = data.LegoPieceExistsMap;
        var legoColor = data.LegoColor;
        
        var boolContainerX = legoPieceExistsMap.GetLength(0);
        var boolContainerY = legoPieceExistsMap.GetLength(1);
        
        var legoPieces = new LegoPiece[boolContainerX, boolContainerY];
        var allLegoPieces = new List<LegoPiece>();
        var shapeContainer =  new GameObject("ShapeContainer").transform;
        
        for (int i = 0; i < boolContainerX; i++)
        {
            for (int j = 0; j < boolContainerY; j++)
            {
                if (legoPieceExistsMap[i, j])
                {
                   var newBlock = GameObject.Instantiate(_legoPiecePrefab, shapeContainer.transform);
                   legoPieces[i, j] = newBlock;
                   newBlock.transform.localPosition = new Vector3(i, j, 0);
                   newBlock.OwnerLegoShape = newLegoShape;
                   newBlock.SetLocalCoordinates(i,j);
                   
                   allLegoPieces.Add(newBlock);
                   
                   //TODO: Solved separated lego pieces look by adding legos in-between. This needs to be improved.
                   if ( i + 1 < legoPieceExistsMap.GetLength(0))
                   {
                       if (legoPieceExistsMap[i + 1, j])
                       {
                           var fillerBlock = GameObject.Instantiate(_legoPiecePrefab, shapeContainer.transform);
                           fillerBlock.transform.localPosition = new Vector3(i+0.5f, j, 0);
                           fillerBlock.OwnerLegoShape = newLegoShape; 
                           fillerBlock.transform.parent = newBlock.transform;
                           fillerBlock.transform.localRotation = Quaternion.Euler(180,0,0);
                           allLegoPieces.Add(fillerBlock);
                       }
                   }
                   
                   if (j + 1 < legoPieceExistsMap.GetLength(1))
                   {
                       if (legoPieceExistsMap[i, j+1])
                       {
                           var fillerBlock = GameObject.Instantiate(_legoPiecePrefab, shapeContainer.transform);
                           fillerBlock.transform.localPosition = new Vector3(i, j+0.5f, 0);
                           fillerBlock.OwnerLegoShape = newLegoShape; 
                           fillerBlock.transform.parent = newBlock.transform;
                           fillerBlock.transform.localRotation = Quaternion.Euler(180,0,0);
                           allLegoPieces.Add(fillerBlock);
                       }
                   }
                }
            }
        }

        Material newColorMaterial = new Material(allLegoPieces[0].Renderer.material);
        newColorMaterial.color = LegoShapeData.ColorsDictionary.TryGetValue(legoColor, out var colorValue)
            ? colorValue
            : Color.white;

        foreach (var block in allLegoPieces)
        {
            if (block == null) continue;
            block.Renderer.material = newColorMaterial;
        }

        newLegoShape.LegoData = data;
        newLegoShape.LegoPieces = legoPieces;
        newLegoShape.ShapeContainer = shapeContainer;
        newLegoShape.AllLegoPieces = allLegoPieces;
        
        return newLegoShape;
    }
}
