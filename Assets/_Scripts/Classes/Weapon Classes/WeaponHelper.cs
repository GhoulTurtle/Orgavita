using System.Collections;
using UnityEngine;

public static class WeaponHelper{
    public static Vector3 CalculateBloom(float bloomAmount, Vector3 rayOrigin, Vector3 rayDirection){
        if(bloomAmount == 0) return rayDirection.normalized;

        float randomAngle = Random.Range(0f, bloomAmount);
        float randomRotation = Random.Range(0f, 360f);

        // Create a random direction within the cone
        Quaternion rotation = Quaternion.Euler(randomAngle * Mathf.Cos(randomRotation), randomAngle * Mathf.Sin(randomRotation), 0);
        Vector3 newDirection = rotation * rayDirection;

        return newDirection.normalized;
    }

    public static CoroutineContainer StartNewWeaponCoroutine(MonoBehaviour runner, float timeInSeconds){
        CoroutineContainer coroutineContainer = new(runner);
        IEnumerator weaponTimerCoroutine = WeaponTimerCoroutine(coroutineContainer, timeInSeconds);

        coroutineContainer.SetCoroutine(weaponTimerCoroutine);

        coroutineContainer.StartCoroutine();

        return coroutineContainer;
    }

    private static IEnumerator WeaponTimerCoroutine(CoroutineContainer container, float timeInSeconds){
        yield return new WaitForSeconds(timeInSeconds);

        container.Dispose();
    }
}