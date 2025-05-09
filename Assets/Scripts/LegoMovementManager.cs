using DG.Tweening;
using UnityEngine;

public class LegoMovementManager :MonoBehaviour
{
    private InputManager _inputManager;
    private bool _holdStateCache;

    private Grid _grid;
    private Vector3 _lastTargetDestination;
    private Tweener _movementTween;
    private GridItem _targetGridItem;
    private LegoShape _currentSelectedLego;
    private Vector3 _cachedDiffVector;
    
    private float _xPositionRightLimit;
    private float _xPositionLeftLimit;
    private float _yPositionUpLimit;
    private float _yPositionDownLimit;
    
    private const float Offset = 0.1f;

    public void Initialize(Grid grid, InputManager inputManager)
    {
        _grid = grid;
        _inputManager = inputManager;
        
        InputManager.Instance.OnLegoHold += HoldLego;
        InputManager.Instance.OnLegoReleased += ReleaseLego;
    }
    
    private Vector3 _speedCache;
    private GridItem _lastCheckTargetGridItem;

    void Update()
    {
        if(!_holdStateCache) return;

        _currentSelectedLego = _inputManager.GetSelectedLegoPiece().OwnerLegoShape;
        _currentSelectedLego.SelectLego();

        if (!_inputManager.HoldingLego || !_holdStateCache || _currentSelectedLego == null ||
            !_grid.TryGetGridItem(Mathf.RoundToInt(_currentSelectedLego.ShapeContainer.transform.position.x),
                Mathf.RoundToInt(_currentSelectedLego.ShapeContainer.transform.position.y),
                out _targetGridItem)) return;
        
        if (_targetGridItem == _lastCheckTargetGridItem)
        {
            MoveLegoToTarget();
            return;
        }
        
        _lastCheckTargetGridItem = _targetGridItem;
        CalculateNewMovementLimits();
        MoveLegoToTarget();
    }

    private void CalculateNewMovementLimits()
    {
        _xPositionRightLimit = !_grid.CanMoveShapeAnchorToPosition(_currentSelectedLego,_targetGridItem.x + 1, _targetGridItem.y) ? _targetGridItem.x + Offset: float.PositiveInfinity;
        _xPositionLeftLimit = !_grid.CanMoveShapeAnchorToPosition(_currentSelectedLego,_targetGridItem.x - 1, _targetGridItem.y) ? _targetGridItem.x - Offset: float.NegativeInfinity;
        _yPositionUpLimit = !_grid.CanMoveShapeAnchorToPosition(_currentSelectedLego,_targetGridItem.x, _targetGridItem.y+1) ? _targetGridItem.y + Offset: float.PositiveInfinity;
        _yPositionDownLimit = !_grid.CanMoveShapeAnchorToPosition(_currentSelectedLego,_targetGridItem.x, _targetGridItem.y-1) ? _targetGridItem.y - Offset: float.NegativeInfinity;
    }

    private void MoveLegoToTarget()
    {
        var target = _inputManager.GetActivePlaneTouchPosition() + _cachedDiffVector;
        Vector3 targetPosition = new Vector2(
            Mathf.Clamp(target.x, _xPositionLeftLimit, _xPositionRightLimit),
            Mathf.Clamp(target.y, _yPositionDownLimit, _yPositionUpLimit)
        );

        _currentSelectedLego.ShapeContainer.transform.position = Vector3.SmoothDamp(_currentSelectedLego.ShapeContainer.transform.position, targetPosition,ref _speedCache, 0.1f);
    }

    private void HoldLego(LegoPiece selectedLegoPiece, Vector3 planeTouchPosition)
    {
        _cachedDiffVector = selectedLegoPiece.OwnerLegoShape.ShapeContainer.position - planeTouchPosition;
        _holdStateCache = true;
    }
    
    private void ReleaseLego(LegoPiece releasedLegoPiece)
    {
        _holdStateCache = false;
        
        _grid.MoveShapeDataToNewPosition(_currentSelectedLego, _targetGridItem.x, _targetGridItem.y);
        
        _movementTween?.Kill();
        
        //TODO: Should this be cached for closure? Check if needed
        var cachedActiveLego = _currentSelectedLego;
        _movementTween = _currentSelectedLego.ShapeContainer.transform.DOLocalMove(new Vector3(_targetGridItem.x,_targetGridItem.y),
            0.1f).OnComplete(() =>
        {
            GameManager.Instance.CheckGates(cachedActiveLego);
        });
        
        _currentSelectedLego?.DeselectLego();
        _currentSelectedLego = null;
    }
}
