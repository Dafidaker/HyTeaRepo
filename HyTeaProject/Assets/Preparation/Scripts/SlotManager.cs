using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotManager : MonoBehaviour, IDropHandler
{
   private GameObject _placedItem;
   
   public void OnDrop(PointerEventData eventData)
   {
      Debug.Log("Dropped");

      if (eventData.pointerDrag != null)
      {
         UpdatePlacedItem();
         eventData.pointerDrag.GetComponent<Transform>().position = GetComponent<Transform>().position;
         eventData.pointerDrag.GetComponent<DragAndDropOptionManager>().SetAsPlaced(true);
         _placedItem = eventData.pointerDrag;
         
      }
   }

   private void UpdatePlacedItem()
   {
      if (_placedItem == null) return;
         
      _placedItem.GetComponent<DragAndDropOptionManager>().ResetPosition();
   }
}
