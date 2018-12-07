using UnityEngine;

namespace Ymit
{
    public class BarBorder
    {
        public float Thickness;
        public Color Color;

        public BarBorder(float Thickness, Color Color)
        {
            this.Thickness = Thickness;
            this.Color = Color;
        }
    }

    public class ProgressBar : MonoBehaviour
    {
        private GameObject HealthbarGO;
        private GameObject BarGO;
        public static ProgressBar Create(float MaxValue, Vector2 Position, Vector2 Size, Color BarColor, Color BackgroundColor, BarBorder Border = null)
        {
            GameObject HealthbarGO = new GameObject("Healthbar", typeof(ProgressBar));
            HealthbarGO.transform.position = Position;
            ProgressBar pb = HealthbarGO.GetComponent<ProgressBar>();
            pb.HealthbarGO = HealthbarGO;

            GameObject BackgroundGO = new GameObject("Background", typeof(SpriteRenderer));
            BackgroundGO.transform.SetParent(HealthbarGO.transform);
            BackgroundGO.transform.localPosition = Vector2.zero;
            BackgroundGO.transform.localScale = Size;
            SpriteRenderer BackgroundRenderer = BackgroundGO.GetComponent<SpriteRenderer>();
            BackgroundRenderer.sprite = GameAssets.i.WhitePixel;
            BackgroundRenderer.color = BackgroundColor;
            BackgroundRenderer.sortingOrder = -1;

            pb.BarGO = new GameObject("Bar");
            pb.BarGO.transform.SetParent(HealthbarGO.transform);
            pb.BarGO.transform.localPosition = new Vector2(-Size.x / 2, 0);

            GameObject BarSpriteGO = new GameObject("BarSprite", typeof(SpriteRenderer));
            BarSpriteGO.transform.SetParent(pb.BarGO.transform);
            BarSpriteGO.transform.localPosition = new Vector2(Size.x / 2, 0);
            BarSpriteGO.transform.localScale = Size;
            SpriteRenderer BarSpriteRenderer = BarSpriteGO.GetComponent<SpriteRenderer>();
            BarSpriteRenderer.sprite = GameAssets.i.WhitePixel;
            BarSpriteRenderer.color = BarColor;

            if (Border != null)
            {
                GameObject BorderGO = new GameObject("Border", typeof(SpriteRenderer));
                BorderGO.transform.SetParent(HealthbarGO.transform);
                BorderGO.transform.localPosition = Vector2.zero;
                BorderGO.transform.localScale = new Vector2(Size.x + Border.Thickness, Size.y + Border.Thickness);
                SpriteRenderer BorderSpriteRenderer = BorderGO.GetComponent<SpriteRenderer>();
                BorderSpriteRenderer.sprite = GameAssets.i.WhitePixel;
                BorderSpriteRenderer.color = Border.Color;
                BorderSpriteRenderer.sortingOrder = -2;
            }

            return pb;
        }

        public void SetProgress(float normalizedSize)
        {
            if (BarGO != null)
            {
                BarGO.transform.localScale = new Vector2(normalizedSize, 1);
            }
        }

        public void LockToTransform(Transform locker)
        {
            if (HealthbarGO != null)
            {
                HealthbarGO.transform.SetParent(locker);
            }
        }
    }
}