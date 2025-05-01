using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class InGameBottomInputManager :  MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public RectTransform selectionBox;
    public List<GameObject> allSelectableItems;

    private Vector2 startPos;
    private List<GameObject> selectedItems = new List<GameObject>();

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
        selectionBox.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateSelectionBox(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        selectionBox.gameObject.SetActive(false);
        SelectInBox(eventData.position);
    }

    void UpdateSelectionBox(Vector2 currentPos)
    {
        Vector2 center = (startPos + currentPos) / 2f;
        selectionBox.position = center;

        Vector2 size = new Vector2(Mathf.Abs(startPos.x - currentPos.x), Mathf.Abs(startPos.y - currentPos.y));
        selectionBox.sizeDelta = size;
    }

    void SelectInBox(Vector2 endPos)
    {
        selectedItems.Clear();

        Vector2 min = Vector2.Min(startPos, endPos);
        Vector2 max = Vector2.Max(startPos, endPos);

        foreach (var item in allSelectableItems)
        {
            Debug.Log("item: " + item.name);
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, item.transform.position);
            if (screenPos.x >= min.x && screenPos.x <= max.x &&
                screenPos.y >= min.y && screenPos.y <= max.y)
            {
                selectedItems.Add(item);
                //item.OnSelect();
            }
            else
            {
                //item.OnDeselect();
            }
        }
    }
}
