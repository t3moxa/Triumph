using UnityEngine;

public abstract class Selectable : MonoBehaviour
{
    protected bool IsSelected;
    public KingdomData Owner;
    public abstract (GameObject, Selectable) OnSelect();
    public abstract void OnDeselect();
    public abstract void OnOrder();
}
