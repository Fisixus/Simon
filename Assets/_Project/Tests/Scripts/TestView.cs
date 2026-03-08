using Simon.Core.MVC;
using UnityEngine;
using UnityEngine.UI;

namespace Simon.Tests
{
    internal class TestView : MvcView
    {
        [SerializeField] private Text _displayText;
        [SerializeField] private Button _actionButton;

        public System.Action OnButtonClicked;

        public override void Initialize()
        {
            if (_actionButton != null)
                _actionButton.onClick.AddListener(() => OnButtonClicked?.Invoke());
        }

        public void SetText(string text)
        {
            if (_displayText != null)
                _displayText.text = text;
            else
                Debug.Log($"[TestView] {text}");
        }

        public void Cleanup()
        {
            if (_actionButton != null)
                _actionButton.onClick.RemoveAllListeners();
        }
    }
}
