using System.Collections;
using UnityEngine;

public class PachinkoBall : MonoBehaviour
{
    private PachinkoManager pachinkoManager;
    private float score;

    private void Awake()
    {
        pachinkoManager = transform.parent.GetComponent<PachinkoManager>();
    }

    private void Update()
    {
        score += Time.deltaTime;
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
}
