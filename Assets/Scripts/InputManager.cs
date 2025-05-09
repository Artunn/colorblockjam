using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    
    public bool HoldingLego => _activeLegoPiece != null;
    private bool _inputLocked;

    private LegoPiece _activeLegoPiece;
    private GridItem _activeHoveredGridItem;
    private Vector3 _activePlaneTouchPosition;

    public Action<LegoPiece, Vector3> OnLegoHold;
    public Action<LegoPiece> OnLegoReleased;
    private bool _shouldTriggerHoldStartedEvent;

    private void Awake()
    {
        //TODO: Singleton should be written better at some point by considering life cycle.
        Instance = this;
    }

    void Update()
    {
        if(_inputLocked) return;
        
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            //TODO: Collider comparison can be made with better operation than compare tag?
            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("LegoBlock"))
                    {
                        if (_activeLegoPiece != null) continue;
                        
                        _activeLegoPiece = hit.collider.gameObject.GetComponent<LegoPiece>();
                        _shouldTriggerHoldStartedEvent = true;
                    }
                    
                    if (hit.collider.CompareTag("Ground"))
                    {
                        _activeHoveredGridItem = hit.collider.gameObject.GetComponent<GroundTile>().MasterGridItem;
                    }
                    
                    if (hit.collider.CompareTag("Plane"))
                    {
                        _activePlaneTouchPosition = hit.point;
                    }
                } 
            }

            if (_shouldTriggerHoldStartedEvent)
            {
                OnLegoHold?.Invoke(_activeLegoPiece, _activePlaneTouchPosition);
                _shouldTriggerHoldStartedEvent = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _activeLegoPiece = null;
            _activeHoveredGridItem = null;
            OnLegoReleased?.Invoke(_activeLegoPiece);
        }
    }

    public GridItem GetActiveHoveredGridItem()
    {
        return _activeHoveredGridItem;
    }
    
    public LegoPiece GetSelectedLegoPiece()
    {
        return _activeLegoPiece;
    }    
    
    public Vector3 GetActivePlaneTouchPosition()
    {
        return _activePlaneTouchPosition;
    }

    public void LockInput()
    {
        _inputLocked = true;
    }
    
    public void UnlockInput()
    {
        _inputLocked = false;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blueViolet;
        Gizmos.DrawSphere(_activePlaneTouchPosition, 0.2f); // Draws at a world position
    }
}
