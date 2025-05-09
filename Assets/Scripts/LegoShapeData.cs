using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LegoShapeData
{
    public readonly bool[,] LegoPieceExistsMap;
    public readonly LegoPiece.LegoColor LegoColor;

    public LegoShapeData(bool[,] legoPieceExistsMap, LegoPiece.LegoColor legoColor)
    {
        LegoPieceExistsMap = legoPieceExistsMap;
        LegoColor = legoColor;
    }
    
    public static readonly Dictionary<LegoPiece.LegoColor, Color> ColorsDictionary = new()
    {
        { LegoPiece.LegoColor.RED, Color.darkRed },
        { LegoPiece.LegoColor.BLUE, Color.deepSkyBlue },
        { LegoPiece.LegoColor.GREEN, new Color(0.16f,0.83f,0.16f)},
        { LegoPiece.LegoColor.YELLOW , new Color(1f,0.76f,0.16f)},
        { LegoPiece.LegoColor.ORANGE ,new Color(0.97f,0.5f,0.06f)},
        { LegoPiece.LegoColor.GRAY ,Color.darkGray},
    };
}
