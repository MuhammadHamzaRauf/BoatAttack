using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoatAttack.UI
{
    /// <summary>
    /// Simple joystick for mobile touch input
    /// </summary>
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Joystick Settings")]
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float maxRadius = 50f;
        [SerializeField] private bool snapToCenter = true;
        
        public System.Action<Vector2> OnValueChanged;
        
        private Vector2 _input;
        private bool _isDragging;
        private Vector2 _startPos;
        
        void Start()
        {
            if (background == null)
                background = transform as RectTransform;
            if (handle == null)
                handle = transform.Find("Handle") as RectTransform;
                
            ResetJoystick();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = true;
            _startPos = eventData.position;
            OnDrag(eventData);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            Vector2 delta = eventData.position - _startPos;
            _input = Vector2.ClampMagnitude(delta, maxRadius);
            
            // Update handle position
            if (handle != null)
            {
                handle.anchoredPosition = _input;
            }
            
            // Normalize input to -1 to 1 range
            Vector2 normalizedInput = _input / maxRadius;
            OnValueChanged?.Invoke(normalizedInput);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;
            ResetJoystick();
        }
        
        private void ResetJoystick()
        {
            _input = Vector2.zero;
            if (handle != null)
            {
                handle.anchoredPosition = Vector2.zero;
            }
            OnValueChanged?.Invoke(Vector2.zero);
        }
        
        // Public getters for current input values
        public float Horizontal => _input.x / maxRadius;
        public float Vertical => _input.y / maxRadius;
        public Vector2 Direction => _input.normalized;
        public bool IsPressed => _isDragging;
    }
}

