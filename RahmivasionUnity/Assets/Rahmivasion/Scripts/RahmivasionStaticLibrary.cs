using UnityEngine;

public class RahmivasionStaticLibrary : MonoBehaviour
{
    /// <summary>
    /// Returns the health component of a selected game object
    /// </summary>
    /// <param name="gameObject">The gameObject to take attempt to get the health component from</param>
    /// <returns>The gameObject's healthComponent</returns>
    public static HealthComponent GetHealthComponent(GameObject gameObject)
    {
        return gameObject.GetComponent<HealthComponent>();
    }
    
    /// <summary>
    /// Checks if the gameObject is alive
    /// </summary>
    /// <param name="gameObject">The gameObject that should be checked if it is alive</param>
    /// <returns>Returns whether the game object is alive, if it does not have HealthComponent, return value will be false</returns>
    public static bool IsGameObjectAlive(GameObject gameObject)
    {
        HealthComponent healthComp = gameObject.GetComponent<HealthComponent>();

        if (healthComp != null)
        {
            return healthComp.IsAlive();
        }

        return false;
    }

    /// <summary>
    /// Kills the selected GameObject
    /// </summary>
    /// <param name="gameObject">GameObject to kill</param>
    public static void KillGameObject(GameObject gameObject)
    {
        HealthComponent healthComp = gameObject.GetComponent<HealthComponent>();

        if (healthComp != null)
        {
            healthComp.Kill();
        }
    }

    /// <summary>
    /// Applies health change to a game object
    /// </summary>
    /// <param name="instigator">The gameObject that is calling this event</param>
    /// <param name="gameObjectToDamge">The gameObject that should be damaged</param>
    /// <param name="delta">The health delta to be applied, must be negative to apply damage</param>
    /// <returns>Return true if health change has been successfully applied, ie health has been changed</returns>
    public static bool ApplyGameObjectHealthChange(GameObject instigator, GameObject gameObjectToDamge, float delta)
    {
        HealthComponent healthComp = gameObjectToDamge.GetComponent<HealthComponent>();

        if (healthComp != null)
        {
            return healthComp.ApplyHealthChange(instigator, delta);
        }

        return false;
    }
}
