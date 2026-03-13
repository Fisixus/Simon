using Simon.Core.MVC;
using System;

namespace Simon.Tests
{
    internal interface ITestView : IMvcView
    {
        event Action OnButtonClicked;
        void SetText(string text);
        void Cleanup();
    }

    internal class TestView : MvcView, ITestView
    {
        [UnityEngine.SerializeField] private UnityEngine.UI.Text _displayText;
        [UnityEngine.SerializeField] private UnityEngine.UI.Button _actionButton;

        public event Action OnButtonClicked;

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
                UnityEngine.Debug.Log($"[TestView] {text}");
        }

        public void Cleanup()
        {
            if (_actionButton != null)
                _actionButton.onClick.RemoveAllListeners();
        }
        
    }
}
