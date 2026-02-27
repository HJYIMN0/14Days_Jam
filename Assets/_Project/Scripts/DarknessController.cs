using UnityEngine;

public class DarknessController : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;
    [SerializeField] private float zPos = 0f;

    private void Start()
    {
        if (playerObj == null)
        {
            Debug.LogWarning("Player reference missing from Darkness canva!");
            Debug.Log($" {gameObject.name} is looking for the player, but it is better if you serialize it correctly");
            playerObj = GameObject.FindGameObjectWithTag("Player");
        }
    }
    private void FixedUpdate()
    {
        Vector3 targetpos = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y, zPos);
        if (transform.position != targetpos)
        {
            transform.position = targetpos;
        }
    }
}
