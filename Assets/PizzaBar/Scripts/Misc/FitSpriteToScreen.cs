using NaughtyAttributes;
using ProjectCore.Misc;
using ProjectCore.ServiceLocator;
using UnityEngine;

namespace PizzaBar.Scripts.Misc
{
    public class FitSpriteToScreen : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Camera _mainCamera;
        
        private void Awake()
        {
            _mainCamera = ServiceLocator.Resolve<SceneComponents>().MainCamera;
            if (_spriteRenderer.IsNull())
                gameObject.AutoResolveComponent(out _spriteRenderer);
            
            FitToScreen();
        }

        [Button]
        public void FitToScreen()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                _mainCamera = Camera.main;
            
            if(_spriteRenderer == null)
                gameObject.AutoResolveComponent(out _spriteRenderer);
#endif
            
            ResizeSpriteToScreen(_spriteRenderer, _mainCamera, 1, 1);
        }

        private void ResizeSpriteToScreen(SpriteRenderer spriteRenderer, Camera mainCamera, int fitToScreenWidth, int fitToScreenHeight)
        {
            var sprite = spriteRenderer.sprite;
            var spriteTransform = spriteRenderer.transform;
            var newSpriteScale = Vector3.one;

            var width = sprite.bounds.size.x;
            var height = sprite.bounds.size.y;

            var worldScreenHeight = mainCamera.orthographicSize * 2f;
            var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            if (fitToScreenWidth != 0)
                newSpriteScale.x = worldScreenWidth / width / fitToScreenWidth;

            if (fitToScreenHeight != 0)
                newSpriteScale.y = worldScreenHeight / height / fitToScreenHeight;

            spriteTransform.localScale = newSpriteScale;
        }
    }
}
