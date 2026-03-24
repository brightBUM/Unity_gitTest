using UnityEngine;
using UnityEngine.AI;

public class NavMeshDist : MonoBehaviour
{
    public NavMeshAgent navmesh;
    public Transform player;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        navmesh.SetDestination(player.position);
    }
}
