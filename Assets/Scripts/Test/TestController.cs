using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestController : MonoBehaviour
{

    private NavMeshAgent m_Agent;

    private float m_LoopTime;

    private float m_NextMoveTime;
    
	void Start ()
	{
	    m_Agent = gameObject.GetComponent<NavMeshAgent>();
	    if (m_Agent == null)
	        m_Agent = gameObject.AddComponent<NavMeshAgent>();
	    RandomMove();

	}
	
	void Update ()
	{
	    m_LoopTime += Time.deltaTime;
	    if (m_LoopTime > m_NextMoveTime)
	    {
	        m_LoopTime = 0;
	        RandomMove();
	    }
	}

    void RandomMove()
    {
        float x = Random.Range(-11f, 16f);
        float z = Random.Range(-11f, 16f);
        float y = 3.61f;
        m_Agent.SetDestination(new Vector3(x, y, z));

        m_NextMoveTime = Random.Range(4f, 10f);
    }
}
