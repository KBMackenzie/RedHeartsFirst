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
    
    internal class ArrowButtons : MonoBehaviour, IPointerDownHandler, ISelectHandler, IDeselectHandler
    {
        public HeartMenu menuInstance;
        public bool isLeft;

        // Getters
        int currentIndex => (int)SaveFile.SaveData;
        int heartStateCount => Enum.GetNames(typeof(HeartState)).Length;

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

        public void OnSelect(BaseEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
