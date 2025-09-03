using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class CheckRadius : MonoBehaviour
{
    EnemyMove enemyMove;

    void Start()
    {
        enemyMove = GetComponentInParent<EnemyMove>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyMove.setShouldFollow(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyMove.setShouldFollow(false);
        }
    }
}
