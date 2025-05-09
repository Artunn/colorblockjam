using System.Collections.Generic;
using UnityEngine;

public class LegoShape
{
    public LegoShapeData LegoData;
    public LegoPiece[,] LegoPieces;
    public List<LegoPiece> AllLegoPieces;
    public Transform ShapeContainer;
    public GridItem Anchor; //?

    public (int x, int y) GetDimensions()
    {
      return (LegoData.LegoPieceExistsMap.GetLength(0),
          LegoData.LegoPieceExistsMap.GetLength(1));
    }

    public bool HasBlockAtRelativeIndex(int i, int j)
    {
       return LegoData.LegoPieceExistsMap[i, j];
    }

    public void SetOverlayActive(bool active)
    {
        foreach (var legoPiece in AllLegoPieces)
        {
            legoPiece.SetOverlayActive(active);
        }
    }

    public void SelectLego()
    {
        SetOverlayActive(true);
    }

    public void DeselectLego()
    {
        SetOverlayActive(false);
    }
}
