using UnityEngine;

public class GroundDetector : MonoBehaviour
{
	public enum DetectType
	{
		Box,
		Circle
	}
	public LayerMask groundLayerMask;
	[HideInInspector] public Collider2D groundCollider;
	public float radius;

	public Collider2D Detect()
	{
		groundCollider = CircleDetect();
		return groundCollider;
	}

	public Collider2D CircleDetect()
	{
		return Physics2D.OverlapCircle(transform.position, radius, groundLayerMask);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, radius);

	}
}
