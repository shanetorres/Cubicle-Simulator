// - AIController.cs
// Functions for the manager AI, such as walking around the office, stopping at certain places, and talking to player and npcs (in progress).
// Code written by Shane Torres.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [Header("Navmesh Settings")]
    [SerializeField] private NavMeshAgent m_NavMeshAgent;
    [SerializeField] private GameObject m_Target;
    [SerializeField] private float m_rotationSpeed = 5f;

    [Header("Animation Settings")]
    [SerializeField] private Animator m_Animator;
    [SerializeField] private CurrentState m_CurrentState;

    private GameObject d_leftfront;
    private GameObject d_rightfront;
    private GameObject d_userdesk;
    private GameObject d_screen;
    private GameObject d_secretary;
    private GameObject managerScreen;
    private GameObject secretaryKeyboard;
    private float timestamp;
    private string m_currentDestination;
    
    // Start is called before the first frame update
    void Start()
    {
        d_leftfront = GameObject.FindGameObjectWithTag("d_leftfront");
        d_rightfront = GameObject.FindGameObjectWithTag("d_rightfront");
        d_userdesk = GameObject.FindGameObjectWithTag("d_userdesk");
        d_screen = GameObject.FindGameObjectWithTag("d_screen");
        d_secretary = GameObject.FindGameObjectWithTag("d_secretary");
        managerScreen = GameObject.FindGameObjectWithTag("manager_screen");
        secretaryKeyboard = GameObject.FindGameObjectWithTag("keyboard_secretary");
    }

    // Update is called once per frame
    void Update()
    {
        float velocity = m_NavMeshAgent.velocity.magnitude;
        AnimationChecker();
        // If the time since the last move is past 5 seconds and the manager is standing still, move.
        if (Time.time - timestamp >= 5.0 && velocity == 0)
        {
            // Choose a random destination for the manager to walk to.
            int random = Random.Range(0, 5);
            switch (random)
            {
                case 0:
                    m_Target = d_leftfront;
                    m_NavMeshAgent.SetDestination(m_Target.transform.position);
                    m_currentDestination = "d_leftfront";
                    break;
                case 1:
                    m_Target = d_rightfront;
                    m_NavMeshAgent.SetDestination(m_Target.transform.position);
                    m_currentDestination = "d_rightfront";
                    break;
                case 2:
                    m_Target = d_userdesk;
                    m_NavMeshAgent.SetDestination(m_Target.transform.position);
                    m_currentDestination = "d_userdesk";
                    break;
                case 3:
                    m_Target = d_screen;
                    m_NavMeshAgent.SetDestination(m_Target.transform.position);
                    m_currentDestination = "d_screen";
                    break;
                case 4:
                    m_Target = d_secretary;
                    m_NavMeshAgent.SetDestination(m_Target.transform.position);
                    m_currentDestination = "d_secretary";
                    break;
            }
        }
    }


    private void AnimationChecker()
    {
        float velocity = m_NavMeshAgent.velocity.magnitude;
        // MANAGER IS WALKING.
        if (velocity >= .5)
        {
            m_CurrentState = CurrentState.Walking;
            m_Animator.SetBool("isWalking", true);
            // The time between the last move should only increase when the manager has reached a destination.
            timestamp = Time.time;
        }
        // MANAGER IS STANDING.
        if (velocity < .5)
        {
            m_CurrentState = CurrentState.Idle;
            m_Animator.SetBool("isWalking", false);
            
        }
        if (velocity < 1.5)
        {
            RotateTowardsDestination();
        }
    }

    private void RotateTowardsDestination()
    {
        switch (m_currentDestination)
        {
            case "d_leftfront":
                Vector3 dlf_direction = (d_leftfront.transform.position - m_NavMeshAgent.transform.position);
                Quaternion dlf_lookRotation = Quaternion.LookRotation(new Vector3(dlf_direction.x, 0, dlf_direction.z));
                m_NavMeshAgent.transform.rotation = Quaternion.Slerp(m_NavMeshAgent.transform.rotation, dlf_lookRotation, Time.deltaTime * m_rotationSpeed);
                break;
            case "d_rightfront":
                Vector3 drf_direction = (d_rightfront.transform.position - m_NavMeshAgent.transform.position);
                Quaternion drf_lookRotation = Quaternion.LookRotation(new Vector3(drf_direction.x, 0, drf_direction.z));
                m_NavMeshAgent.transform.rotation = Quaternion.Slerp(m_NavMeshAgent.transform.rotation, drf_lookRotation, Time.deltaTime * m_rotationSpeed);
                break;
            case "d_userdesk":
                Vector3 dud_direction = (d_userdesk.transform.position - m_NavMeshAgent.transform.position);
                Quaternion dud_lookRotation = Quaternion.LookRotation(new Vector3(dud_direction.x, 0, dud_direction.z));
                m_NavMeshAgent.transform.rotation = Quaternion.Slerp(m_NavMeshAgent.transform.rotation, dud_lookRotation, Time.deltaTime * m_rotationSpeed);
                break;
            case "d_screen":
                Vector3 ds_direction = (managerScreen.transform.position - m_NavMeshAgent.transform.position);
                Quaternion ds_lookRotation = Quaternion.LookRotation(new Vector3(ds_direction.x, 0, ds_direction.z));
                m_NavMeshAgent.transform.rotation = Quaternion.Slerp(m_NavMeshAgent.transform.rotation, ds_lookRotation, Time.deltaTime * m_rotationSpeed);
                break;
            case "d_secretary":
                Vector3 dsec_direction = (secretaryKeyboard.transform.position - m_NavMeshAgent.transform.position);
                Quaternion dsec_lookRotation = Quaternion.LookRotation(new Vector3(dsec_direction.x, 0, dsec_direction.z));
                m_NavMeshAgent.transform.rotation = Quaternion.Slerp(m_NavMeshAgent.transform.rotation, dsec_lookRotation, Time.deltaTime * m_rotationSpeed);
                break;
        }
    }

    enum CurrentState
    {
        Idle,
        Walking
    }
}
