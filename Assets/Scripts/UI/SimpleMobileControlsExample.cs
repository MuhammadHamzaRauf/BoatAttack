using UnityEngine;
using UnityEngine.UI;

namespace BoatAttack.UI
{
    /// <summary>
    /// Example script showing how to set up simple mobile controls
    /// </summary>
    public class SimpleMobileControlsExample : MonoBehaviour
    {
        [Header("Mobile Control Setup")]
        [SerializeField] private SimpleMobileControls mobileControls;
        
        [Header("Example UI Setup")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button forwardButton;
        [SerializeField] private Button reverseButton;
        
        void Start()
        {
            // Create mobile controls if not assigned
            if (mobileControls == null)
            {
                mobileControls = gameObject.AddComponent<SimpleMobileControls>();
            }
            
            // Set up example buttons
            SetupExampleButtons();
        }
        
        private void SetupExampleButtons()
        {
            // Create left button if not assigned
            if (leftButton == null)
            {
                leftButton = CreateButton("Left", new Vector2(100, 100));
                leftButton.onClick.AddListener(() => mobileControls?.OnLeftPressed());
            }
            
            // Create right button if not assigned
            if (rightButton == null)
            {
                rightButton = CreateButton("Right", new Vector2(300, 100));
                rightButton.onClick.AddListener(() => mobileControls?.OnRightPressed());
            }
            
            // Create forward button if not assigned
            if (forwardButton == null)
            {
                forwardButton = CreateButton("Forward", new Vector2(200, 200));
                forwardButton.onClick.AddListener(() => mobileControls?.OnForwardPressed());
            }
            
            // Create reverse button if not assigned
            if (reverseButton == null)
            {
                reverseButton = CreateButton("Reverse", new Vector2(200, 0));
                reverseButton.onClick.AddListener(() => mobileControls?.OnReversePressed());
            }
        }
        
        private Button CreateButton(string text, Vector2 position)
        {
            // Create button GameObject
            var buttonGO = new GameObject(text + "Button");
            buttonGO.transform.SetParent(transform);
            
            // Add required components
            var button = buttonGO.AddComponent<Button>();
            var image = buttonGO.AddComponent<Image>();
            var textComponent = buttonGO.AddComponent<Text>();
            
            // Set up text
            textComponent.text = text;
            textComponent.color = Color.black;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = 24;
            textComponent.alignment = TextAnchor.MiddleCenter;
            
            // Set up image
            image.color = Color.white;
            
            // Set up RectTransform
            var rectTransform = buttonGO.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(80, 80);
            
            // Set up button colors
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.gray;
            colors.pressedColor = new Color(0.3f, 0.3f, 0.3f); // Dark gray equivalent
            button.colors = colors;
            
            return button;
        }
        
        [ContextMenu("Setup Mobile Controls")]
        public void SetupMobileControls()
        {
            SetupExampleButtons();
        }
    }
}
