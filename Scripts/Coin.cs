using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Coin : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<Coin> Collided;

    public int value = 0;
    Vector3 jumpVelocity = new Vector3();

    [SerializeField]
    private AudioSource landSound;
    [SerializeField]
    private AudioSource collectSound;
    [SerializeField]
    private Gradient lerpGradient;

    private const float jumpForce = 0.005f;

    public void Setup(Vector3 startPosition)
    {
        jumpVelocity = new Vector3(Random.Range(-jumpForce, jumpForce), 0.05f, Random.Range(-jumpForce, jumpForce));
        transform.position = startPosition;
        GetComponent<Rigidbody>().AddForce(jumpVelocity, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collided?.Invoke(this);
        landSound.Play();
    }

    public IEnumerator Lerp(Vector3 targetPosition, float speed, Vector3 pos1 = new Vector3(), Vector3 pos2 = new Vector3())
    {
        Destroy(GetComponent<Rigidbody>());
        GetComponentInChildren<TrailRenderer>().time = 0.05f;
        GetComponentInChildren<TrailRenderer>().colorGradient = lerpGradient;

        Vector3 startPosition = transform.position + new Vector3(0, 0.25f, 0);
        float duration = Vector3.Distance(startPosition, targetPosition) / (speed * Time.deltaTime);
        float activeTime = 0;

        while (activeTime < duration)
        {
            activeTime += Time.deltaTime;
            if (pos1 == new Vector3() || pos2 == new Vector3())
            {
                transform.position = cubeBezier3(startPosition, targetPosition, activeTime / duration);
            }
            else
            {
                transform.position = cubeBezier3(startPosition, pos1, pos2, targetPosition, activeTime / duration);
            }
            yield return new WaitForEndOfFrame();
        }

        collectSound.Play();

        GetComponent<MeshRenderer>().enabled = false;
        Destroy(this.gameObject, 5);
        Destroy(this);
    }

    private Vector3 cubeBezier3(Vector3 p0, Vector3 p3, float t)
    {
        Vector3 p1 = (p3 - p0 / 2) * 0.25f + p0 + new Vector3(jumpVelocity.x * 100, 1, 0);
        Vector3 p2 = (p3 - p0 / 2) * 0.75f + p0 + new Vector3(jumpVelocity.x * 100, 1, 0);

        float r = 1f - t;
        float f0 = r * r * r;
        float f1 = r * r * t * 3;
        float f2 = r * t * t * 3;
        float f3 = t * t * t;
        return f0 * p0 + f1 * p1 + f2 * p2 + f3 * p3;
    }

    private Vector3 cubeBezier3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float r = 1f - t;
        float f0 = r * r * r;
        float f1 = r * r * t * 3;
        float f2 = r * t * t * 3;
        float f3 = t * t * t;
        return f0 * p0 + f1 * p1 + f2 * p2 + f3 * p3;
    }
}
