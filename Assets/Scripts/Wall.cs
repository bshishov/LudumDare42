using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Wall : MonoBehaviour
    {
        private Transform _cameraTransform;
        private MeshRenderer _meshRenderer;

        private bool _isVisible;

        void Start ()
        {
            _cameraTransform = Camera.main.transform;
            _meshRenderer = GetComponent<MeshRenderer>();

            StartCoroutine(LateStart());
        }

        IEnumerator LateStart()
        {
            yield return new WaitForEndOfFrame();
            Check();
        }
        
        void Update ()
        {
            Check();
        }

        void Check()
        {
            var isFacingCamera = Vector3.Dot(transform.forward, _cameraTransform.position - _cameraTransform.forward) > 0f;

            if (isFacingCamera && !_isVisible)
            {
                _meshRenderer.enabled = true;
                _isVisible = true;
            }

            if (!isFacingCamera && _isVisible)
            {
                _meshRenderer.enabled = false;
                _isVisible = false;
            }
        }
    }
}
