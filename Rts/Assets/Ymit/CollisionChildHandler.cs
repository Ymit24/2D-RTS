using UnityEngine;

namespace Ymit
{
    public enum CollisionType { ENTER, STAY, EXIT }
    public interface ICollisionParentHandler
    {
        void OnCollision(Collider2D Other, CollisionType Type);
    }

    public class CollisionChildHandler : MonoBehaviour
    {
        private ICollisionParentHandler ParentHandler;
        public void SetHandler(ICollisionParentHandler ParentHandler)
        {
            this.ParentHandler = ParentHandler;
        }
        private void OnTriggerEnter2D(Collider2D Other)
        {
            ParentHandler.OnCollision(Other, CollisionType.ENTER);
        }
        private void OnTriggerExit2D(Collider2D Other)
        {
            ParentHandler.OnCollision(Other, CollisionType.EXIT);
        }
        private void OnTriggerStay2D(Collider2D Other)
        {
            ParentHandler.OnCollision(Other, CollisionType.STAY);
        }
    }
}

