using UnityEngine;

namespace Ymit
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets _instance;
        public static GameAssets i
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameAssets>();
                    if (_instance == null)
                    {
                        Debug.LogWarning("Game Assets is null!");
                    }
                }
                return _instance;
            }
        }

        public Sprite WhitePixel;
    }
}