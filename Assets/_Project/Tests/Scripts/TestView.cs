using Simon.Core.MVC;
using System;
using UnityEngine;

namespace Simon.Tests
{
    internal interface ITestView : IMvcView
    {
        event Action OnButtonClicked;
        void SetText(string text);
        Transform Transform { get; }
    }

    internal class TestView : MvcView, ITestView
    {
        [UnityEngine.SerializeField] private UnityEngine.UI.Text _displayText;
        [UnityEngine.SerializeField] private UnityEngine.UI.Button _actionButton;
        [UnityEngine.SerializeField] private Transform _transform;
        
        public Transform Transform => _transform;

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

    }
}
