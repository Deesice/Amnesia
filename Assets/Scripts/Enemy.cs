using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : ReflectableMonoBehaviour, ISave
{
    List<Tuple<Vector3, float>> pathNodes = new List<Tuple<Vector3, float>>();
    Animator animator;
    NavMeshAgent navMeshAgent;
    public bool hallucination;
    public float hallucinationEndDist;
    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void OnEnable()
    {
        if (pathNodes.Count > 0)
            SmartInvoke.Invoke(() => navMeshAgent.SetDestination(pathNodes[0].Item1), 0);
    }
    public void AddEnemyPatrolNode(Vector3 pos, float time)
    {
        pathNodes.Add(new Tuple<Vector3, float>(pos, time));
        if (enabled && gameObject.activeSelf && pathNodes.Count == 1)
            navMeshAgent.SetDestination(pos);
    }
    private void Update()
    {
        animator.SetFloat("speed", navMeshAgent.velocity.magnitude);
        if (hallucination && (PlayerController.instance.playerBody.transform.position - transform.position).magnitude <= hallucinationEndDist)
        {
            Destroy(this);
            Destroy(animator);
            Destroy(navMeshAgent);
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());
            Scenario.currentScenario.SetPropActiveAndFade(gameObject.name, false, 1);
        }
    }

    public void OnLoad(Data data)
    {
        int count;
        data.IntKeys.TryGetValue(this.GetHierarchyPath(), out count, 0);
        for (int i = 0; i < count; i++)
        {
            Vector3 v;
            float f;
            data.VectorKeys.TryGetValue(this.GetHierarchyPath() + "_" + i, out v, Vector3.zero);
            data.FloatKeys.TryGetValue(this.GetHierarchyPath() + "_" + i, out f, 0);
            AddEnemyPatrolNode(v, f);
        }
    }

    public void OnSave(Data data)
    {
        data.IntKeys.SetValueSafety(this.GetHierarchyPath(), pathNodes.Count);
        for (int i = 0; i < pathNodes.Count; i++)
        {
            data.VectorKeys.SetValueSafety(this.GetHierarchyPath() + "_" + i, pathNodes[i].Item1);
            data.FloatKeys.SetValueSafety(this.GetHierarchyPath() + "_" + i, pathNodes[i].Item2);
        }
    }
}
