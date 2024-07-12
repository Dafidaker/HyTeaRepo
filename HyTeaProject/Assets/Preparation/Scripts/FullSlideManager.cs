using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FullSlideManager : MonoBehaviour
{
   [SerializeField] private Section CorrespondingSection;
   [SerializeField] private TextMeshProUGUI Title;
   [SerializeField] private GameObject OptionTextSlot;
   [SerializeField] private GameObject OptionTextMenu;
   [SerializeField] private List<string> Options;
   [SerializeField] private string SelectedOption;

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

   public string GetSelectedOption()
   {
      return SelectedOption;
   }

   public void SetSelectedOption(string option)
   {
      SelectedOption = option;
   }

   public List<string> GetOptions()
   {
      return Options;
   }

   public void SetOptions(List<string> options)
   {
      Options = options;
   }

   private void OnEnable()
   {
      Options = new List<string>();
   }

   public void OpenTextMenu()
   {
      OptionTextMenu.SetActive(true);
   }

   public void CloseTextMenu()
   {
      OptionTextMenu.SetActive(false);
   }
}

