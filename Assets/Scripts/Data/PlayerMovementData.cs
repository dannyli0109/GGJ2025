using UnityEngine;
//using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Data/Property/PlayerMovementData", fileName = "PlayerMovementData")]
public class PlayerMovementData : ScriptableObject
{
    //[FoldoutGroup("速度")]
    public float speed;
    //[FoldoutGroup("速度")]
    public float maxFallSpeed;
	//[FoldoutGroup("速度")]
	public float maxRiseSpeed;

	//[FoldoutGroup("加速度")]
    public bool noAccelerateOnGround;
    //[HideIf("noAccelerateOnGround")]
    //[FoldoutGroup("加速度")]
    public float acceleration;
    //[HideIf("noAccelerateOnGround")]
    //[FoldoutGroup("加速度")]
    public float deceleration;
    //[FoldoutGroup("加速度")]
    public float airAcceleration;
    //[FoldoutGroup("加速度")]
    public float fallAcceleration;

	//[FoldoutGroup("冲刺")]
    public float sprintSpeed;
    //[FoldoutGroup("冲刺")]
    public float sprintTime;
    //[FoldoutGroup("冲刺")]
    public float sprintCD;
	//[FoldoutGroup("冲刺")]
	//public EasingLerps.EasingInOutType sprintInOutType;
	//[FoldoutGroup("冲刺")]
	//public EasingLerps.EasingLerpsType sprintLerpType;

    //[FoldoutGroup("跳跃")]
    public float landJumpHeight;
    //[FoldoutGroup("跳跃")]
    public float airJumpHeight;
    //[FoldoutGroup("跳跃")]
    public int jumpCount;

    //[FoldoutGroup("土狼时间")]
    public float coyoteTime;
}
