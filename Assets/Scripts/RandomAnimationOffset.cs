using UnityEngine;

public class RandomAnimationOffset : StateMachineBehaviour
{
    [Range(0f, 1f)]
    public float minOffset = 0f;

    [Range(0f, 1f)]
    public float maxOffset = 1f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float randomTime = Random.Range(minOffset, maxOffset);
        animator.Update(randomTime * stateInfo.length);
    }
}