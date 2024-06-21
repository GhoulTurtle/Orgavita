using UnityEngine;

public static class GunCalculation{
    public static Vector3 CalculateBloom(float bloomAmount, Vector3 rayOrigin, Vector3 rayDirection){
        if(bloomAmount == 0) return rayDirection.normalized;

        float randomAngle = Random.Range(0f, bloomAmount);
        float randomRotation = Random.Range(0f, 360f);

        // Create a random direction within the cone
        Quaternion rotation = Quaternion.Euler(randomAngle * Mathf.Cos(randomRotation), randomAngle * Mathf.Sin(randomRotation), 0);
        Vector3 newDirection = rotation * rayDirection;

        return newDirection.normalized;
    }

}
