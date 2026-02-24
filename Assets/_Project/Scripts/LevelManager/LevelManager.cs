using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool needsAllEnemiesDefeatedToExit = true;

    public List<GameObject> Enemies { get; private set; }   
    
    public bool AreAllEnemiesDefeated() => Enemies.Count == 0;


    private void Start()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemies.Add(enemy);
        }
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        if (Enemies.Count > 0)
        {
            foreach (GameObject enemyInList in Enemies)
            {
                if (enemy == enemyInList)
                {
                    Enemies.Remove(enemy);
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!needsAllEnemiesDefeatedToExit)
            {
                Win();
                return;
            }

            if (AreAllEnemiesDefeated())
            {
                Win();
            }
        }
    }

    public void Win()
    {
        Debug.Log("Player has reached the exit! Level complete!");
    }

}
