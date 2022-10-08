using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedHeartsFirst
{
    internal class ArrowButtons : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public HeartMenu menuInstance;
        public Image img;
        public bool isLeft;

        public Sprite Normal;

        public void OnPointerDown(PointerEventData eventData)
        {
            ArrowClick();
        }

        public void ArrowClick()
        {
            int index = (int)SaveFile.SaveData;
            int max = Enum.GetNames(typeof(HeartOrder)).Length;
            index = isLeft ? index -= 1 : index += 1;
            if (index >= max) index = 0;
            if (index < 0) index = max - 1;

            SaveFile.SaveData = (HeartOrder)index;
            menuInstance.SetHeartIcons();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ChangeColor(isRed: true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ChangeColor(isRed: false);
        }

        void ChangeColor(bool isRed)
        {
            Color temp = isRed ? new Color(1f, 0, 0, 1f) : new Color(1f, 1f, 1f, 1f);
            img.color = temp;
        }
    }
}
