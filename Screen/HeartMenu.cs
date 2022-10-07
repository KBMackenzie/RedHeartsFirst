using HarmonyLib;
using Lamb.UI.PauseMenu;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

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

        public readonly Dictionary<string, Sprite> ArrowSprites = new Dictionary<string, Sprite>()
        {
            { "Left",     TexLoader.MakeSprite(Properties.Resources.ArrowL) },
            { "Right",    TexLoader.MakeSprite(Properties.Resources.ArrowR) }
        };

        Sprite BoxSprite = TexLoader.MakeSprite(Properties.Resources.HeartBox3);
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
            ChangeOpacity(ref img, 0.8f);

            BoxHelper helper = box.AddComponent<BoxHelper>();
            helper.menuInstance = this;
            helper.img = img;
            helper.canvas = canvas;

            box.transform.localScale = new Vector3(0.5f, 0.5f, 0);

            return box;
        }

        void ChangeOpacity(ref Image image, float opacity)
        {
            Color color = image.color;
            color.a = opacity;
            image.color = color;
        }

        Vector3 UIImageSize(Image img)
        {
            float w = img.sprite.rect.width;
            float h = img.sprite.rect.height;
            return new Vector3(w, h, 0);
        }

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

            TextMeshProUGUI textMesh = textBox.AddComponent<TextMeshProUGUI>();
            textMesh.font = UIText.font;
            textMesh.fontSize = UIText.fontSize;
            textMesh.text = "Heart Order";

            textBox.transform.localPosition = new Vector3(0, 63f, 0);
            textMesh.alignment = TextAlignmentOptions.Center;
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
            img.sprite = ArrowSprites["Left"];
            img.SetNativeSize();

            ArrowButtons arrL = leftArrow.AddComponent<ArrowButtons>();
            arrL.menuInstance = this;
            arrL.img = img;
            arrL.isLeft = true;
            arrL.Normal = ArrowSprites["Left"];

            Arrows.Add("Left", arrL);


            // Right arrow
            GameObject rightArrow = new GameObject();
            rightArrow.name = "RightArrow";
            rightArrow.layer = Layer;
            rightArrow.transform.SetParent(parent);
            rightArrow.transform.localPosition = new Vector3(360, -60f, 0);
            rightArrow.transform.localScale = new Vector3(1.2f, 1.2f, 0);

            Image img2 = rightArrow.AddComponent<Image>();
            img2.sprite = ArrowSprites["Right"];
            img2.SetNativeSize();

            ArrowButtons arrR = rightArrow.AddComponent<ArrowButtons>();
            arrR.menuInstance = this;
            arrR.img = img2;
            arrR.isLeft = false;
            arrR.Normal = ArrowSprites["Right"];

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
            switch (SaveFile.SaveData)
            {
                case (HeartOrder.Off):
                    return (HeartSprites["Red"], HeartSprites["Blue"], HeartSprites["Black"]);

                case (HeartOrder.BlackRedBlue):
                    return (HeartSprites["Black"], HeartSprites["Red"], HeartSprites["Blue"]);

                case (HeartOrder.RedBlackBlue):
                    return (HeartSprites["Red"], HeartSprites["Black"], HeartSprites["Blue"]);

                case (HeartOrder.BlueRedBlack):
                    return (HeartSprites["Blue"], HeartSprites["Red"], HeartSprites["Black"]);

                default: /* Yes, I know having 'default' here is redundant, but still. */
                    return (HeartSprites["Red"], HeartSprites["Blue"], HeartSprites["Black"]);
            }
        }

        // Handle keyboard presses!
        void Update()
        {
            // Simulate right arrow press
            if (Input.GetKeyDown("h"))
            {
                Arrows["Right"].ArrowClick();
            }
        }
    }
}
