using UnityEngine;
using System.Collections;

public class SimpleRotor : MonoBehaviour
{
    [SerializeField]
    private float _degreesPerSecond;
    [SerializeField]
    private float _discreteAngle;

    private Coroutine _rotateCoroutine;
    
    void OnEnable()
    {
        _rotateCoroutine = StartCoroutine(Rotate());
    }

    void OnDisable()
    {
        StopCoroutine(_rotateCoroutine);
    }

    private IEnumerator Rotate()
    {
        if (_discreteAngle != 0)
        {
            float period = Mathf.Abs(_discreteAngle) / _degreesPerSecond;
            var waiter = new WaitForSeconds(period);
            while (true)
            {
                yield return waiter;
                transform.Rotate(0.0f, 0.0f, _discreteAngle);
            }
        }
        else
        {
            while (true)
            {
                yield return null;
                transform.Rotate(0.0f, 0.0f, _degreesPerSecond * Time.deltaTime);
            }
        }
    } 
}
