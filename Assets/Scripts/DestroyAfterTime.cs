using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float Time;

    void Start()
    {
        Destroy(gameObject, Time);
    }
}
