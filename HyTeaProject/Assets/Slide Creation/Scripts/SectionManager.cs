using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SectionManager : MonoBehaviour
{
   [SerializeField] private Section CorrespondingSection;
   public Vector2 ButtonPos;
   private int _numOfSections;
   
   public void AddToList()
   {
      EventManager.AddSectionToOrderEvent.Invoke(CorrespondingSection, ButtonPos);
   }

   private void UpdateNumOfSections(int num)
   {
      _numOfSections = num;
      //Debug.Log(_numOfSections);
   }

   public void SetSection(Section section)
   {
      CorrespondingSection = section;
   }

   public Section GetSection()
   {
      return CorrespondingSection;
   }

   private void OnEnable()
   {
      EventManager.SetNumOfSectionsAdded.AddListener(UpdateNumOfSections);
   }

   private void OnDisable()
   {
      EventManager.SetNumOfSectionsAdded.RemoveListener(UpdateNumOfSections);
   }
}
