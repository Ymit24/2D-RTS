using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Ymit
{
    public static class UI
    {
        private static GameObject CanvasGO;
        public static GameObject GetCanvas()
        {
            if (CanvasGO == null)
            {
                CreateCanvas();
            }
            return CanvasGO;
        }
        public static GameObject CreateCanvas()
        {
            CanvasGO = new GameObject("Canvas", typeof(RectTransform), typeof(CanvasScaler), typeof(GraphicRaycaster));
            CanvasGO.GetComponent<Canvas>().sortingLayerName = "CanvasLayer";
            CanvasGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            CanvasGO.GetComponent<RectTransform>().sizeDelta = Vector2.one;
            CanvasGO.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            return CanvasGO;
        }
        public static void ExpandToInclude(Vector2 position, Vector2 size)
        {
            if (CanvasGO == null) return;
            RectTransform CanvasRect = CanvasGO.GetComponent<RectTransform>();
            if (CanvasRect == null) return;
            if (Utils.AABB(
                CanvasRect.anchoredPosition.x,
                CanvasRect.anchoredPosition.y,
                CanvasRect.sizeDelta.x,
                CanvasRect.sizeDelta.y,

                position.x, position.y,
                size.x, size.y
                        ))
            {

            }
        }
        public static void ForceDebugUI()
        {
            if (CanvasGO != null) return;
            CreateCanvas();
        }
        public static void DebugFadeLabelMouse(string text)
        {
            ForceDebugUI();
            UIFadeupLabel.CreateMouse(new UILabelParameters() { Text = text }, 50, 1.5f);
        }
        public static Vector2 GetScreenSize() { return new Vector2(Screen.width, Screen.height); }
    }

    public class UILabelParameters
    {
        public UICoord CoordType;
        public Vector2 WorldPosition;
        public Color Color = Color.black;
        public string Text;
    }
    
    public class UIFadeupLabel : MonoBehaviour
    {
        private float Lifetime;
        private float Speed;
        private void Update()
        {
            Lifetime -= Time.deltaTime;
            GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition + Vector2.up * Speed * Time.deltaTime;
            if (Lifetime <= 0)
            {
                Destroy(gameObject);
            }
        }
        public static void Create(UILabelParameters Parameters, float Height, float Lifetime)
        {
            UILabel Label = UILabel.Create(Parameters);
            UIFadeupLabel FadeLabel = Label.GetGameObject().AddComponent<UIFadeupLabel>();
            FadeLabel.Lifetime = Lifetime;
            FadeLabel.Speed = Height / Lifetime;
        }
        public static void CreateMouse(UILabelParameters Parameters, float Height, float Lifetime)
        {
            Parameters.WorldPosition = Utils.WorldMouse();
            Parameters.CoordType = UICoord.WORLD;
            Create(Parameters, Height, Lifetime);
        }
    }
    public enum UICoord { SCREEN, WORLD }
    public abstract class UIWidget
    {
        protected GameObject UIGameObject;

        protected static void SetupRect(RectTransform Rect, GameObject CanvasGO, UICoord CoordType, Vector2 WorldPosition)
        {
            Rect.SetParent(CanvasGO.transform);
            if (CoordType == UICoord.SCREEN)
            {
                Rect.anchorMin = Vector2.zero;
                Rect.anchorMax = Vector2.zero;
                Rect.localPosition = Utils.ScreenToWorld(Vector2.zero) + WorldPosition;
            }
            else
            {
                Rect.localPosition = WorldPosition;
            }
            Rect.pivot = new Vector2(0.5f, 0.5f);
        }
        public void SetPosition(Vector2 WorldPosition)
        {
            if (UIGameObject == null) return;
            UIGameObject.GetComponent<RectTransform>().anchoredPosition = WorldPosition;
        }
        public GameObject GetGameObject()
        {
            return UIGameObject;
        }
    }
    public class UILabel : UIWidget
    {
        private UILabel(GameObject LabelGO) { this.UIGameObject = LabelGO; }

        public static UILabel Create(UILabelParameters Parameters)
        {
            GameObject CanvasGO = UI.GetCanvas();
            GameObject LabelGO = new GameObject("UILabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(ContentSizeFitter));
            UILabel Label = new UILabel(
                LabelGO
            );

            // CONSIDER USING THE ABSTRACT FUNCTION FOR HANDLING THIS
            //RectTransform LabelRect = LabelGO.GetComponent<RectTransform>();
            //LabelRect.SetParent(CanvasGO.transform);
            //if (Parameters.CoordType == UICoord.SCREEN)
            //{
            //    LabelRect.anchorMin = Vector2.zero;
            //    LabelRect.anchorMax = Vector2.zero;
            //    LabelRect.localPosition = Utils.ScreenToWorld(Vector2.zero) + Parameters.WorldPosition;
            //}
            //else
            //{
            //    LabelRect.localPosition = Parameters.WorldPosition;
            //}
            //LabelRect.pivot = new Vector2(0.5f, 0.5f);

            SetupRect(LabelGO.GetComponent<RectTransform>(), CanvasGO, Parameters.CoordType, Parameters.WorldPosition);

            Text LabelText = LabelGO.GetComponent<Text>();
            LabelText.text = Parameters.Text;
            LabelText.fontSize = 25;
            LabelText.color = Parameters.Color;
            LabelText.alignment = TextAnchor.MiddleCenter;
            LabelText.font = Font.CreateDynamicFontFromOSFont("Arial", 25);
            LabelGO.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            LabelGO.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return Label;
        }
        public static UILabel CreateMouse(UILabelParameters Parameters)
        {
            Parameters.WorldPosition = Utils.WorldMouse();
            Parameters.CoordType = UICoord.WORLD;
            return Create(Parameters);
        }

        public void SetText(string Text)
        {
            if (UIGameObject == null) return;
            Text LabelText = UIGameObject.GetComponent<Text>();
            if (LabelText == null) return;
            LabelText.text = Text;
        }
    }
    public class UIBox : UIWidget
    {
        private UIBox(GameObject UIGameObject) { this.UIGameObject = UIGameObject; }

        public static UIBox Create(Vector2 WorldPosition, UICoord CoordType, Vector2 Size)
        {
            GameObject CanvasGO = UI.GetCanvas();
            GameObject UIGameObject = new GameObject("UIBox", typeof(RectTransform), typeof(Image));
            UIBox Box = new UIBox(UIGameObject);

            SetupRect(UIGameObject.GetComponent<RectTransform>(), CanvasGO, CoordType, WorldPosition);

            Box.SetSize(Size);

            return Box;
        }
        public void SetSize(Vector2 Size)
        {
            if (UIGameObject == null) return;
            RectTransform Rect = UIGameObject.GetComponent<RectTransform>();
            if (Rect == null) return;
            Rect.sizeDelta = Size;
        }
    }
}
