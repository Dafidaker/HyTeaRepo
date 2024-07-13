using System;
using System.Collections;
using System.Collections.Generic;
using Mediapipe;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Image = Microsoft.Unity.VisualStudio.Editor.Image;

public class FullSlideManager : MonoBehaviour
{
   [SerializeField] private Section CorrespondingSection;
   [SerializeField] private TextMeshProUGUI Title;
   [SerializeField] private GameObject OptionTextSlot;
   [SerializeField] private GameObject OptionTextMenu;
   [SerializeField] private List<string> TextOptions;
   [SerializeField] private GameObject MiscOption;
   [SerializeField] private List<GameObject> TextDisplayBoxes;
   [SerializeField] private GameObject MiscDisplayBox;
   [SerializeField] private GameObject SelectedTextOption;
   [SerializeField] private GameObject SelectedImageOption;

   public bool HasMiscOption;

   public void SetTitle(string title)
   {
      Title.text = title;
   }

   public Section GetSection()
   {
      return CorrespondingSection;
   }

   public void SetSection(Section section)
   {
      CorrespondingSection = section;
   }

   public GameObject GetSelectedOption()
   {
      return SelectedTextOption;
   }

   public void SetSelectedOption(GameObject option)
   {
      SelectedTextOption = option;
   }

   public List<string> GetOptions()
   {
      return TextOptions;
   }

   public void SetOptions(List<string> options)
   {
      TextOptions = options;
   }

   public void SetMiscOption(GameObject misc)
   {
      MiscOption = misc;
   }

   public GameObject GetMiscOption()
   {
      return MiscOption;
   }

   private void OnEnable()
   {
      TextOptions ??= new List<string>();
   }

   public void OpenTextMenu()
   {
      OptionTextMenu.SetActive(true);
      UpdateOptions();
   }

   public void CloseTextMenu(GameObject option)
   {
      OptionTextMenu.SetActive(false);
      UpdateSelectedOption(option);
      transform.Find("OptionTextSlot").GetComponent<UnityEngine.UI.Image>().enabled = false;
   }

   private void UpdateOptions()
   {
      for (int i = 0; i < TextDisplayBoxes.Count; i++)
      {
         TextDisplayBoxes[i].GetComponent<TextMeshProUGUI>().text = TextOptions[i];
      }

      if (HasMiscOption)
      {
         
      }
   }

   private void UpdateSelectedOption(GameObject option)
   {
      if(option.name != "MiscOption") SelectedTextOption.GetComponent<TextMeshProUGUI>().text = option.GetComponent<TextMeshProUGUI>().text;
      else Debug.Log("Misc Option");
   }
}

