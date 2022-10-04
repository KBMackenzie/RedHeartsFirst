using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedHeartsFirst
{
    internal class BoxHelper : MonoBehaviour, IDragHandler
    {
        public HeartMenu menuInstance;
        public Image img;
        public RectTransform dragRectTransform;
        public Canvas canvas;

        void Start()
        {
            dragRectTransform = GetComponent<RectTransform>();
            // maybe pass heartmenu transform here instead?
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    internal class HeartsIcon : MonoBehaviour
    {
        public HeartMenu menuInstance;
        public Image img;
        public int id;
    }
    
    internal class ArrowButtons : MonoBehaviour, IPointerDownHandler
    {
        public HeartMenu menuInstance;
        public bool isLeft;

        bool isSelected;

        // Getters
        int currentIndex => (int)SaveFile.SaveData;
        int heartStateCount => Enum.GetNames(typeof(HeartState)).Length;

        void Start()
        {

        }

        void OnMouseEnter()
        {
            isSelected = true;
        }

        void OnMouseExit()
        {
            isSelected = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            int index = currentIndex;
            index = isLeft ? index -= 1 : index += 1;
            if (index >= heartStateCount) index = 0;
            if (index < 0) index = heartStateCount - 1;

            FileLog.Log("Pre-save: " + index.ToString());

            SaveFile.SaveData = (HeartState)index;
            menuInstance.SetHeartIcons();

            FileLog.Log("Post save: " + index.ToString());
        }
    }
}
