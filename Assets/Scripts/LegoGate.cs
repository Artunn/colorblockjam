using UnityEngine;

public class LegoGate
{
    //TODO: We might need a data class
    public int Height;
    public bool ArrowEnabled;
    public (int x, int y) GatePosition;
    public bool isVertical;
    private LegoGateBlock legoGateBlockReference;
    private LegoGateBlock _blockInstance;
    public GateType Type;

    private LegoPiece.LegoColor gateColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public LegoGate(LegoGateBlock LegoGateBar, int x, int y, int height, LegoPiece.LegoColor color, bool verticality, bool arrowEnabled, GateType gateType)
    {
        gateColor = color;
        GatePosition.x = x;
        GatePosition.y = y;
        Height = height;
        isVertical = verticality;
        legoGateBlockReference = LegoGateBar;
        ArrowEnabled = arrowEnabled;
        Type = gateType;
    }

    public void GenerateWall()
    {
        var y = GatePosition.y;
        var x = GatePosition.x;
        var height = Height;
        
        float initialPosition = (isVertical ? y : x) + (height-1) / 2f;
        
        _blockInstance = GameObject.Instantiate(legoGateBlockReference);
        if (isVertical)
        {
            var isRight = x > 0;
            _blockInstance.transform.localPosition = new Vector3(GatePosition.x + (isRight? -0.25f: 0.25f), initialPosition, 0);
            _blockInstance.BlockContainer.transform.localScale = new Vector3(1, height, 1);
            _blockInstance.ArrowPointerContainer.localRotation = Quaternion.Euler(0, 0, isRight ? 0 : 180f);
        }
        else
        {
            var isUp = y > 0;
            _blockInstance.transform.localPosition = new Vector3(initialPosition, (GatePosition.y + (isUp ? -0.25f: 0.25f)), 0);
            _blockInstance.transform.localRotation = Quaternion.Euler(0, 0, 90);
            _blockInstance.BlockContainer.transform.localScale = new Vector3(1, height, 1);
            _blockInstance.ArrowPointerContainer.localRotation = Quaternion.Euler(0, 0, isUp ? 0 : 180);
        }
        
        _blockInstance.transform.position.Set(_blockInstance.transform.position.x, _blockInstance.transform.position.y, 0);

        _blockInstance.Renderer.material.color = LegoShapeData.ColorsDictionary.TryGetValue(gateColor,
            out var colorValue) ? colorValue : Color.white;;
        
        SetArrowEnabled(ArrowEnabled);
    }
    
    public bool CanShapeGoThrough(LegoShape shape)
    {
        if (shape.LegoData.LegoColor != gateColor) return false;

        Debug.Log($"Currently checking shape at position {shape.ShapeContainer.transform.position}, " +
                  $"on gate {GatePosition.x},{GatePosition.y} with height {Height} and isVertical {isVertical}");
        
        var anchorOfShape = shape.Anchor;
        var dimensionOfShape = shape.GetDimensions();
        if (isVertical)
        {
            if (anchorOfShape.x + dimensionOfShape.x == GatePosition.x||
                anchorOfShape.x - 1 == GatePosition.x) //Should be sticking to gate
            {
                if (anchorOfShape.y + dimensionOfShape.y - 1<= GatePosition.y + Height &&
                    anchorOfShape.y >= GatePosition.y )
                {
                    return true;
                }
            }
        }
        else
        {
            Debug.Log($"Anchor shape is {anchorOfShape.x},{anchorOfShape.y} with dimension {dimensionOfShape.x},{dimensionOfShape.y}");
            if (anchorOfShape.y + dimensionOfShape.y == GatePosition.y ||
                anchorOfShape.y - 1 == GatePosition.y) //Should be sticking to gate
            {
                if (anchorOfShape.x + dimensionOfShape.x - 1 <= GatePosition.x + Height &&
                    anchorOfShape.x >= GatePosition.x)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void SetArrowEnabled(bool arrowEnabled)
    {
        _blockInstance.ArrowPointerContainer.gameObject.SetActive(arrowEnabled);
    }

    public enum GateType
    {
        GATE,
        BLOCKER
    }
}
