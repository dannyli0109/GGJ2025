using UnityEngine;
//using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Data/Property/PlayerMovementData", fileName = "PlayerMovementData")]
public class PlayerMovementData : ScriptableObject
{
    //[FoldoutGroup("�ٶ�")]
    public float speed;
    //[FoldoutGroup("�ٶ�")]
    public float maxFallSpeed;
	//[FoldoutGroup("�ٶ�")]
	public float maxRiseSpeed;

	//[FoldoutGroup("���ٶ�")]
    public bool noAccelerateOnGround;
    //[HideIf("noAccelerateOnGround")]
    //[FoldoutGroup("���ٶ�")]
    public float acceleration;
    //[HideIf("noAccelerateOnGround")]
    //[FoldoutGroup("���ٶ�")]
    public float deceleration;
    //[FoldoutGroup("���ٶ�")]
    public float airAcceleration;
    //[FoldoutGroup("���ٶ�")]
    public float fallAcceleration;

	//[FoldoutGroup("���")]
    public float sprintSpeed;
    //[FoldoutGroup("���")]
    public float sprintTime;
    //[FoldoutGroup("���")]
    public float sprintCD;
	//[FoldoutGroup("���")]
	//public EasingLerps.EasingInOutType sprintInOutType;
	//[FoldoutGroup("���")]
	//public EasingLerps.EasingLerpsType sprintLerpType;

    //[FoldoutGroup("��Ծ")]
    public float landJumpHeight;
    //[FoldoutGroup("��Ծ")]
    public float airJumpHeight;
    //[FoldoutGroup("��Ծ")]
    public int jumpCount;

    //[FoldoutGroup("����ʱ��")]
    public float coyoteTime;
}
