using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ymit
{
    public class SpriteAnimator : MonoBehaviour
    {
        private Sprite[] Frames;

        private int CurrentFrame = 0;
        private float FrameRate = 0.2f;
        private float Timer;

        private SpriteRenderer Renderer;

        public void PlayAnimation(Sprite[] frames, float framerate)
        {
            this.Frames = frames;
            this.FrameRate = framerate;
            SetFirstFrame();
        }

        public void SetRenderer(SpriteRenderer Renderer)
        {
            this.Renderer = Renderer;
        }

        private void SetFirstFrame()
        {
            this.CurrentFrame = 0;
            this.Timer = 0;
            SetSprite(0);
        }

        private void SetSprite(int FrameNumber)
        {
            if (Renderer == null) return;
            if (Frames == null) return;
            Renderer.sprite = Frames[FrameNumber];
        }

        private void Update()
        {
            if (Frames == null) return;
            Timer += Time.deltaTime;
            if (Timer >= FrameRate)
            {
                Timer -= FrameRate;
                CurrentFrame = (CurrentFrame + 1) % Frames.Length;
                SetSprite(CurrentFrame);
            }
        }
    }
}