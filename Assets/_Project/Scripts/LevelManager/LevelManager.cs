using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool needsAllEnemiesDefeatedToExit = true;
    [SerializeField] private GameObject winCanvaGo;

    public List<GameObject> Enemies { get; private set; }   
    
    public bool AreAllEnemiesDefeated() => Enemies.Count == 0;


    private void Start()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Enemies = new List<GameObject>();
        foreach (GameObject enemy in enemies) 
        {
            Debug.Log("Adding " + enemy.name);
            Enemies.Add(enemy);
        }

        winCanvaGo.SetActive(false);
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        Debug.Log("Function Remove Enemy called");
        if (Enemies.Count > 0)
        {
            foreach (GameObject enemyInList in Enemies)
            {
                if (enemy == enemyInList)
                {
                    Enemies.Remove(enemy);
                    Debug.Log(enemy.name + " removed!");
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player found!");
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
        winCanvaGo.SetActive(true);
    }

}
