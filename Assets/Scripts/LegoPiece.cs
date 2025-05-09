using UnityEngine;

public class LegoPiece : MonoBehaviour
{
    public LegoShape OwnerLegoShape;
    public Renderer Renderer;
    public GameObject LegoBlock;
    public GridItem GridItem;

    public int x => GridItem?.x ?? -2;
    public int y =>  GridItem?.y ?? -2;
    
    public int localX;
    public int localY;
    
    public void SetOverlayActive(bool isEnabled)
    {
        LegoBlock.layer = isEnabled ? 6 : 0;
    }

    public enum LegoColor
    {
        BLUE,
        GREEN,
        VIOLET,
        ORANGE,
        RED,
        YELLOW,
        GRAY
    }

    public (int x, int y) GetPosition()
    {
        return (x, y);
    }

    public void SetGridItem(GridItem gridItem)
    {
        GridItem = gridItem;
    }

    public void SetLocalCoordinates(int i, int j)
    {
        localX = i;
        localY = j;
    }
}