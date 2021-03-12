using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Cockroach : MonoBehaviour
{
    public float speed;
    public float radius;
    public float areal;
    public float soundRandMin = 1;
    public float soundRandMax = 3;
    Vector3 startPos;
    Vector3 goal;
    float toNextSound;
    bool isAfraid;
    private void Awake()
    {
        toNextSound = Random.Range(soundRandMin, soundRandMax);
        startPos = transform.position;
        NewGoal();
        //await Task.Delay(1);
        //Scenario.currentScenario.SetPropActiveAndFade(gameObject.name, false, 0);
    }
    void Update()
    {
        if ((transform.position - goal).magnitude < areal / 10)
            NewGoal();
        var toGoal = goal - transform.position;
        var fromPlayer = transform.position - PlayerController.instance.transform.position;
        fromPlayer.y = 0;
        if (fromPlayer.magnitude <= radius && Lantern.IsLit)
        {
            toGoal = fromPlayer;
            isAfraid = true;
        }
        else
            isAfraid = false;
        var q = Quaternion.LookRotation(toGoal, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime * speed * (isAfraid ? 2 : 1));
        transform.position += transform.forward * speed * Time.deltaTime * (isAfraid ? 2 : 1);

        if (isAfraid)
            toNextSound -= Time.deltaTime / 2;
        else
            toNextSound -= Time.deltaTime;

        if (toNextSound <= 0)
        {
            toNextSound = Random.Range(soundRandMin, soundRandMax);
            if (isAfraid)
                SoundManager.PlaySoundAtEntity(gameObject.name + "_sound", "roach_scare", gameObject.name, 0);
            else
                SoundManager.PlaySoundAtEntity(gameObject.name + "_sound", "roach_idle", gameObject.name, 0);
        }
    }
    void NewGoal()
    {
        goal.y = 0;
        goal.x = Random.Range(-1.0f, 1.0f);
        goal.z = Random.Range(-1.0f, 1.0f);
        goal.Normalize();
        goal *= Random.Range(0, areal);
        goal += startPos;
    }
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(startPos, areal);
            Gizmos.DrawCube(goal, Vector3.one / 2);
        }
        else
            Gizmos.DrawWireSphere(transform.position, areal);
    }
}
