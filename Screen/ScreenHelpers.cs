using System;
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
}
