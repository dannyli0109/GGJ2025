using HFSM;
using UnityEngine;
using UnityHFSM;

public class PrintHFSM : MonoBehaviour
{
	public GameObject target;
	StateMachine fsm;
	public Animator animator;

	private void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if(fsm == null)
		{
			fsm = target.GetComponent<PlayerFSM>().lowerFsm;
			var ac = fsm.PrintToAnimator();
			//animator = gameObject.AddComponent<Animator>();
			animator.runtimeAnimatorController = ac;
		}
		string stateName = fsm.GetActiveNestedStateName();
		int id = Animator.StringToHash(stateName);
		if (animator.HasState(0, id))
		{
			//Debug.Log(stateName);
			animator.Play(stateName);
		}
	}


}
