using HarmonyLib;
using Lamb.UI.PauseMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;

namespace RedHeartsFirst
{
    internal class HeartMenu : MonoBehaviour
    {
        public GameObject Parent;
        LayerMask Layer = LayerMask.NameToLayer("UI");
        static LoadTexture TexLoader = new LoadTexture(FilterMode.Bilinear);

        public readonly Dictionary<string, Sprite> HeartSprites = new Dictionary<string, Sprite>()
        {
            { "Black",  TexLoader.MakeSprite(Properties.Resources.BlackHeart) },
            { "Red",    TexLoader.MakeSprite(Properties.Resources.RedHeart)   },
            { "Blue",   TexLoader.MakeSprite(Properties.Resources.BlueHeart)  },
        };

        Sprite BoxSprite = TexLoader.MakeSprite(Properties.Resources.HeartBox3, 0.8f);
        int HeartCount = 3;

        public TextMeshProUGUI UIText;
        public Canvas canvas;

        // Heart list
        List<HeartsIcon> HeartIcons = new List<HeartsIcon>();
        // Arrows
        Dictionary<string, ArrowButtons> Arrows = new Dictionary<string, ArrowButtons>();

        void Start() // Initialize
        {
            CreateMenu();
        }

        void CreateMenu()
        {
            GameObject HeartBox = CreateBox();
            Transform BoxParent = HeartBox.transform;

            for (int i = 0; i < HeartCount; i++)
            {
                CreateHeart(BoxParent, i);
            }

            SetHeartIcons();
            CreateText(BoxParent);
            CreateArrows(BoxParent);
        }

        GameObject CreateBox()
        {
            GameObject box = new GameObject();
            box.name = "HeartBox";
            box.layer = Layer;
            box.transform.SetParent(Parent.transform);
            box.transform.localPosition = new Vector3(1f, 1f, 0);

            Image img = box.AddComponent<Image>();
            img.sprite = BoxSprite;
            img.SetNativeSize();
            img.preserveAspect = true;

            BoxCollider2D col = box.AddComponent<BoxCollider2D>();
            col.size = UIImageSize(img);

            BoxHelper helper = box.AddComponent<BoxHelper>();
            helper.menuInstance = this;
            helper.img = img;
            helper.canvas = canvas;

            box.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            return box;
        }

        Vector3 UIImageSize(Image img)
        {
            float w = img.sprite.rect.width;
            float h = img.sprite.rect.height;
            return new Vector3(w, h, 0);
        }

        /*
        Vector3 ResizeBox (Transform transform, int div = 2)
        {
            // 'div' is implicitly converted to float here! c:
            Vector3 resize = new Vector3(
                transform.localScale.x / div,
                transform.localScale.y / div,
                transform.localScale.z / div
            );

            return resize;
        }*/

        GameObject CreateHeart(Transform parent, int i)
        {
            float x_offset = -160 + 160 * i;

            GameObject heart = new GameObject();
            heart.name = "Heart" + i.ToString();
            heart.layer = Layer;
            heart.transform.SetParent(parent);
            heart.transform.localPosition = new Vector3(x_offset, -60f, 0f);
            heart.transform.localScale = new Vector3(1.2f, 1.2f, 0);

            Image img = heart.AddComponent<Image>();
            img.sprite = HeartSprites["Red"];
            img.SetNativeSize();

            /*
            BoxCollider2D col = heart.AddComponent<BoxCollider2D>();
            col.size = UIImageSize(img);*/

            HeartsIcon heartScript = heart.AddComponent<HeartsIcon>();
            heartScript.menuInstance = this;
            heartScript.img = img;
            heartScript.id = i;
            HeartIcons.Add(heartScript);

            return heart;
        }

        void CreateText(Transform parent)
        {
            GameObject textBox = new GameObject();
            textBox.name = "UIText_Heart";
            textBox.layer = Layer;
            textBox.transform.SetParent(parent);
            textBox.transform.localPosition = new Vector3(70f, 43f, 0);
            TextMeshProUGUI textMesh = textBox.AddComponent<TextMeshProUGUI>();
            textMesh.font = UIText.font;
            textMesh.fontSize = UIText.fontSize;
            textMesh.text = "Heart Order";
        }

        void CreateArrows(Transform parent)
        {
            // Left arrow
            GameObject leftArrow = new GameObject();
            leftArrow.name = "LeftArrow";
            leftArrow.layer = Layer;
            leftArrow.transform.SetParent(parent);
            leftArrow.transform.localPosition = new Vector3(-360f, -60f, 0);
            leftArrow.transform.localScale = new Vector3(1.2f, 1.2f, 0);

            Image img = leftArrow.AddComponent<Image>();
            img.sprite = HeartSprites["Red"]; // Add proper sprite
            img.SetNativeSize();

            /*
            BoxCollider2D col = leftArrow.AddComponent<BoxCollider2D>();
            col.size = UIImageSize(img);*/

            ArrowButtons arrL = leftArrow.AddComponent<ArrowButtons>();
            arrL.menuInstance = this;
            arrL.isLeft = true;

            Arrows.Add("Left", arrL);


            // Right arrow
            GameObject rightArrow = new GameObject();
            rightArrow.name = "RightArrow";
            rightArrow.layer = Layer;
            rightArrow.transform.SetParent(parent);
            rightArrow.transform.localPosition = new Vector3(360, -60f, 0);
            rightArrow.transform.localScale = new Vector3(1.2f, 1.2f, 0);

            Image img2 = rightArrow.AddComponent<Image>();
            img2.sprite = HeartSprites["Red"]; // Add proper sprite
            img2.SetNativeSize();

            /*
            BoxCollider2D col2 = rightArrow.AddComponent<BoxCollider2D>();
            col2.size = UIImageSize(img2);*/

            ArrowButtons arrR = rightArrow.AddComponent<ArrowButtons>();
            arrR.menuInstance = this;
            arrR.isLeft = false;

            Arrows.Add("Right", arrR);
        }

        public void SetHeartIcons()
        {
            (Sprite, Sprite, Sprite) HeartIconsTuple = GetHeartStateIcons();

            HeartIcons[0].img.sprite = HeartIconsTuple.Item1;
            HeartIcons[1].img.sprite = HeartIconsTuple.Item2;
            HeartIcons[2].img.sprite = HeartIconsTuple.Item3;
        }

        (Sprite, Sprite, Sprite) GetHeartStateIcons ()
        {
            switch (HeartPatches.Hearts)
            {
                case (HeartState.Off):
                    return (HeartSprites["Red"], HeartSprites["Blue"], HeartSprites["Black"]);

                case (HeartState.BlackRedBlue):
                    return (HeartSprites["Black"], HeartSprites["Red"], HeartSprites["Blue"]);

                case (HeartState.RedBlackBlue):
                    return (HeartSprites["Red"], HeartSprites["Black"], HeartSprites["Blue"]);

                case (HeartState.BlueRedBlack):
                    return (HeartSprites["Blue"], HeartSprites["Red"], HeartSprites["Black"]);

                default: /* Yes, I know having 'default' here is redundant, but still. */
                    return (HeartSprites["Red"], HeartSprites["Blue"], HeartSprites["Black"]);
            }
        }

    }
    
}
