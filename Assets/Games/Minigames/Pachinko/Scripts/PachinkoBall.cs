using System.Collections;
using UnityEngine;

public class PachinkoBall : MonoBehaviour
{
    private PachinkoManager pachinkoManager;
    private float score;
    private Vector3 lastPositionBall;
    private float timeWithoutMoving;
    private float force;

    private void Awake()
    {
        pachinkoManager = transform.parent.GetComponent<PachinkoManager>();
        timeWithoutMoving = 0;
        force = 0.1f;
        lastPositionBall = transform.position;
    }

    private void Update()
    {
        score += Time.deltaTime;
        AddLittleForceIfStacks(3f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TagsIribus.PCKStar) || other.gameObject.CompareTag(TagsIribus.PCKCorrect) || other.gameObject.CompareTag(TagsIribus.PCKIncorrect))
        {
            if (other.gameObject.CompareTag(TagsIribus.PCKStar))
            {
                pachinkoManager.numberOfScore += ((int)score * 2);
                pachinkoManager.numberOfPrize += 2;
            }
            else if (other.gameObject.CompareTag(TagsIribus.PCKCorrect))
            {
                pachinkoManager.numberOfScore += ((int)score);
                pachinkoManager.numberOfPrize += 1;
            }
            else if (other.gameObject.CompareTag(TagsIribus.PCKIncorrect))
            {
                if (pachinkoManager.numberOfPrize > 0)
                {
                    pachinkoManager.numberOfScore += ((int)score/4);
                    pachinkoManager.numberOfPrize -= 1;
                }
            }
            pachinkoManager.NextBallToCreate();
            Destroy(this);
        }
    }

    private bool AddLittleForceIfStacks(float timeThreshold)
    {
        if (transform.position == lastPositionBall)
        {
            timeWithoutMoving += Time.deltaTime;

            if (timeWithoutMoving >= timeThreshold)
            {
                int randomDirection = Random.value < 0.5f ? 1 : -1;
                GetComponent<Rigidbody>().AddForce(new Vector3(randomDirection*force, 0 , 0), ForceMode.Impulse);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            timeWithoutMoving = 0;
            lastPositionBall = transform.position;
            return false;
        }
    }
}
