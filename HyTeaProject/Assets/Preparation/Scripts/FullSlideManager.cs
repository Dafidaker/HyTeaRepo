using System;
using System.Collections;
using System.Collections.Generic;
using Mediapipe;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = Microsoft.Unity.VisualStudio.Editor.Image;

public class FullSlideManager : MonoBehaviour
{
   [SerializeField] private Section CorrespondingSection;
   [SerializeField] private TextMeshProUGUI Title;
   [SerializeField] private GameObject OptionTextSlot;
   [SerializeField] private GameObject OptionImageSlot;
   
   [SerializeField] private GameObject OptionTextMenu;
   [SerializeField] private GameObject OptionImageMenu;
   [SerializeField] private List<string> TextOptions;
   [SerializeField] private List<GameObject> ImageOptions;
   
   [SerializeField] private GameObject MiscOption;
   [SerializeField] private Sprite MiscOptionSpirte;
   [SerializeField] private List<GameObject> TextDisplayBoxes;
   [SerializeField] private GameObject SelectedTextOption;
   [SerializeField] private Sprite DefaultBorderSprite;

   [SerializeField] private Transform ImageStart;
   [SerializeField] private GameObject TextDesc;
   [SerializeField] private GameObject ImageDesc;
   
   public bool HasMiscOption;
   public bool HasTextOptionSelected;
   
   

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

   public void SetImages(List<GameObject> images)
   {
      ImageOptions = images;
   }

   public void SetMiscOption(Sprite misc)
   {
      MiscOptionSpirte = misc;
   }

   public Sprite GetMiscOption()
   {
      return MiscOptionSpirte;
   }

   private void OnEnable()
   {
      TextOptions ??= new List<string>();
      ImageOptions ??= new List<GameObject>();
   }

   public void OpenTextMenu()
   {
      OptionTextMenu.SetActive(true);
      OptionTextSlot.transform.SetSiblingIndex(2);
      UpdateTextOptions();
   }

   public void OpenImageMenu()
   {
      OptionImageMenu.SetActive(true);
      OptionImageSlot.transform.SetSiblingIndex(2);
      UpdateImageOptions();
   }

   public void CloseTextMenu(GameObject option)
   {
      OptionTextMenu.SetActive(false);
      UpdateSelectedTextOption(option);
   }

   public void CloseImageMenu(GameObject image)
   {
      UpdateSelectedImage(image);
      
      OptionImageMenu.SetActive(false);
   }

   public void RemoveImage()
   {
      OptionImageSlot.GetComponent<UnityEngine.UI.Image>().sprite = DefaultBorderSprite;
      OptionImageMenu.SetActive(false);
      ImageDesc.SetActive(true);
   }

   private void UpdateTextOptions()
   {
      for (int i = 0; i < TextDisplayBoxes.Count; i++)
      {
         TextDisplayBoxes[i].GetComponent<TextMeshProUGUI>().text = TextOptions[i];
      }

      if (HasMiscOption)
      {
         MiscOption.GetComponent<UnityEngine.UI.Image>().sprite = MiscOptionSpirte;
      }
   }

   private void UpdateImageOptions()
   {
      if (ImageStart.childCount == 0)
      {
         float horizontalSpacing = ImageOptions[0].GetComponent<RectTransform>().rect.width + 150 + Screen.width / 100;
         int count = ImageOptions.Count;
         float totalWidth = count * horizontalSpacing;
     
         Vector3 startPosition = ImageStart.position;
         startPosition.x -= (totalWidth - horizontalSpacing) / 2;

         for (int i = 0; i < count; i++)
         {
            var go = Instantiate(ImageOptions[i], ImageStart);
            go.GetComponent<Button>().onClick.AddListener(() => CloseImageMenu(go));

            Vector3 newPosition = startPosition + new Vector3(i * horizontalSpacing, 0, 0);
            go.transform.position = newPosition;
         }
      }
      else
      {
         Debug.Log("Images already created");
      }
   }

   private void UpdateSelectedTextOption(GameObject option)
   {
      if (option.name != "MiscOption")
      {
         SelectedTextOption.GetComponent<TextMeshProUGUI>().text = option.GetComponent<TextMeshProUGUI>().text;
         transform.Find("OptionTextSlot").GetComponent<UnityEngine.UI.Image>().enabled = false;
      }
      else
      {
         Debug.Log("Misc Option");
         transform.Find("OptionTextSlot").GetComponent<UnityEngine.UI.Image>().enabled = true;
         transform.Find("OptionTextSlot").GetComponent<UnityEngine.UI.Image>().sprite = MiscOptionSpirte;
         SelectedTextOption.GetComponent<TextMeshProUGUI>().text = "";
      }
      
      TextDesc.SetActive(false);
      HasTextOptionSelected = true;
   }

   private void UpdateSelectedImage(GameObject image)
   {
      OptionImageSlot.GetComponent<UnityEngine.UI.Image>().sprite = image.GetComponent<UnityEngine.UI.Image>().sprite;
      ImageDesc.SetActive(false);
   }
}

