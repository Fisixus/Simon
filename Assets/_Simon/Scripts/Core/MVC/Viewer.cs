using UnityEngine;

namespace Simon.Core.MVC
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        public Transform Transform => _transform;
        
    }
}
