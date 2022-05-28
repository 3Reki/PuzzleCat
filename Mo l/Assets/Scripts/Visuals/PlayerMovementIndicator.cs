using System;
using System.Collections;
using UnityEngine;

namespace PuzzleCat.Visuals
{
    public class PlayerMovementIndicator : MonoBehaviour
    {
        [SerializeField] private Transform indicatorTransform;
        [SerializeField] private Material indicatorMaterial;
        [SerializeField] private float animationSpeed = 1f;
        
        private static readonly int _offsetProperty = Shader.PropertyToID("_Offset");
        private IEnumerator _currentCoroutine;
        private bool _isPlaying;
        private float _timerStart;

        public void Play(Vector3 position, Quaternion rotation)
        {
            indicatorTransform.gameObject.SetActive(true);
            indicatorTransform.position = position;
            indicatorTransform.rotation = rotation;
            _timerStart = Time.time;
            indicatorMaterial.SetFloat(_offsetProperty, 0);
            if (_isPlaying)
            {
                StopCoroutine(_currentCoroutine);
            }

            StartCoroutine(_currentCoroutine = ShaderMovementCoroutine());
        }

        private IEnumerator ShaderMovementCoroutine()
        {
            _isPlaying = true;
            
            while ((Time.time - _timerStart) * animationSpeed < 1)
            {
                yield return null;
                indicatorMaterial.SetFloat(_offsetProperty, (Time.time - _timerStart) * animationSpeed);
            }
            
            indicatorTransform.gameObject.SetActive(false);
            _isPlaying = false;
        }

        private void OnDestroy()
        {
            indicatorMaterial.SetFloat(_offsetProperty, 0);
        }
    }
}
