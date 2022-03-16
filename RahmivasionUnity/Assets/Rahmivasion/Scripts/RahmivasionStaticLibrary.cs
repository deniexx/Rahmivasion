using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RahmivasionStaticLibrary : MonoBehaviour
{
    public static HealthComponent GetHealthComponent(GameObject gameObject)
    {
        return gameObject.GetComponent<HealthComponent>();
    }

    public static bool IsGameObjectAlive(GameObject gameObject)
    {
        HealthComponent healthComp = gameObject.GetComponent<HealthComponent>();

        if (healthComp != null)
        {
            return healthComp.IsAlive();
        }

        return false;
    }

    public static void KillGameObject(GameObject gameObject)
    {
        HealthComponent healthComp = gameObject.GetComponent<HealthComponent>();

        if (healthComp != null)
        {
            healthComp.Kill();
        }
    }
}
