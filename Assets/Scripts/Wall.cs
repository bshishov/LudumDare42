using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Wall : MonoBehaviour
    {
        private Transform _cameraTransform;
        private MeshRenderer _meshRenderer;

        private bool _isVisible = true;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
            _meshRenderer = GetComponent<MeshRenderer>();

            //StartCoroutine(LateStart());
            Check();
        }

        IEnumerator LateStart()
        {
            //yield return new WaitForSeconds(2f);
            yield return new WaitForEndOfFrame();
            //yield return null;
            Debug.Log("CHECK!");
            Check();
        }
        
        void Update()
        {
            Check();
        }

        void Check()
        {
            Debug.DrawRay(_cameraTransform.position, _cameraTransform.forward, Color.green);
            Debug.DrawRay(transform.position, transform.forward, Color.yellow);
            var xyT = new Vector2(transform.forward.x, transform.forward.z);
            var xyC = new Vector2(_cameraTransform.forward.x, _cameraTransform.forward.z);
            var isFacingCamera = Vector2.Dot(xyT, xyC) < 0;

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
