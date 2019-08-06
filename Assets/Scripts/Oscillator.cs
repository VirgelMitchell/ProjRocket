using UnityEngine;

namespace PG.Control
{
    [DisallowMultipleComponent]
    public class Oscillator : MonoBehaviour
    {
        [SerializeField] bool isSlider = false;
        [SerializeField] bool isRotater = false;
        [SerializeField] Vector3 moveVector = new Vector3(0f, 100f, 0f);
        [SerializeField] float period = Mathf.Epsilon;
        
        const float tau = Mathf.PI * 2f;
            
        Vector3 startPosition;
        
        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            if          (isSlider)      { SliderOscillation(); }
            else if     (isRotater)     { RotaterOscillation(); }

        }

        private void SliderOscillation()
        {
            if (period <= Mathf.Epsilon) { period = .001f; }
            float cycles = Time.time / period;
            float amplitude = Mathf.Sin(cycles * tau);
            float movementFactor = (amplitude / 2) + .5f;
            Vector3 movement = moveVector * movementFactor;
            transform.position = startPosition + movement;
        }

        private void RotaterOscillation()
        {
            
        }
    }
}
