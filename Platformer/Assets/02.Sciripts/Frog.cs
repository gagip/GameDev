using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
    protected enum EnemyState
    {
        Idle,
        Move,
        OnDamaged
    }

    protected EnemyState frogState;
    
    protected override void Awake()
    {
        base.Awake();
        frogState = EnemyState.Idle;
    }

    private void Start()
    {
        StartCoroutine(StartState());
    }

    protected override IEnumerator StartState()
    {
        while (true)
        {
            isNewState = false;
            yield return StartCoroutine(frogState.ToString());
        }
    }

    void SetState(EnemyState newState)
    {
        isNewState = true;
        frogState = newState;
    }

    IEnumerator Idle()
    {
        anim.SetInteger("State", (int)frogState);

        yield return new WaitForSeconds(2.0f);

        SetState(EnemyState.Move);
    }

    protected override IEnumerator Move()
    {
        anim.SetInteger("State", (int)frogState);
        yield return null;

        while (!isNewState)
        {
            yield return null;

            
        }


    }

    protected override IEnumerator OnDamaged()
    {

        yield return null;

        while (!isNewState)
        {
            yield return null; 
        }
    }
    






}
