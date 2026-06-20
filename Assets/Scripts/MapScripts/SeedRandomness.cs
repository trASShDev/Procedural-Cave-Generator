using UnityEngine;
using System;
public class SeedRandomness : MonoBehaviour
{
    [SerializeField]
    private int seedToUse;
    [SerializeField]
    private int seed;

    public static UnityEngine.Random.State seedState;
    public static bool hasState = false;

    public static SeedRandomness Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        if (hasState) {
            UnityEngine.Random.state = seedState;
            Debug.Log(UnityEngine.Random.state);
            return;
        }
        if (seedToUse != 0) {
            UnityEngine.Random.InitState(seedToUse);
            seedState = UnityEngine.Random.state;
            Debug.Log(UnityEngine.Random.state);
            hasState = true;
            return;
        }
        seed = GenerateComplexSeed();
        UnityEngine.Random.InitState(seed);
        seedState = UnityEngine.Random.state;
        Debug.Log(UnityEngine.Random.state);
        hasState = true;
    }
    
    void OnDisable()
    {
        seedState = UnityEngine.Random.state;
    }

    int GenerateComplexSeed()
    {
        int seed = 0;

        seed ^= Environment.TickCount;                               // Time since system start
        seed ^= DateTime.Now.Millisecond << 11;                      // Current millisecond shifted
        seed ^= SystemInfo.graphicsDeviceID;                         // GPU dependent
        seed ^= SystemInfo.processorCount << 3;                      // CPU core count
        seed ^= SystemInfo.systemMemorySize << 2;                    // RAM
        seed ^= (int)(UnityEngine.Random.value * int.MaxValue);      // Unity's own random
        seed ^= (int)(Time.realtimeSinceStartup * 1000) << 5;        // Time since app launch

        return seed;
    }
}
