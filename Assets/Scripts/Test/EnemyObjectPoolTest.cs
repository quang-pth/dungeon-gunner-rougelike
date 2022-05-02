using UnityEngine;

public class EnemyObjectPoolTest : MonoBehaviour
{
    [SerializeField] private EnemyAnimationDetails[] enemyAnimationDetailsArray;
    [SerializeField] private GameObject enemyExamplePrefab;
    private float timer = 1f;

    [System.Serializable]
    public struct EnemyAnimationDetails {
        public RuntimeAnimatorController runtimeAnimatorController;
        public Color spriteColor;
    }

    private void FixedUpdate() {
        timer -= Time.deltaTime;

        if (timer <= 0f) {
            GetEnemyExample();
            timer = 1f;
        }
    }

    private void GetEnemyExample() {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Vector3 spawnPos = new Vector3(Random.Range(currentRoom.lowerBounds.x, currentRoom.upperBounds.x),
                    Random.Range(currentRoom.lowerBounds.y, currentRoom.upperBounds.y), 0f);

        EnemyAnimation enemyAnimation = (EnemyAnimation) PoolManager.Instance.ReuseComponent(enemyExamplePrefab, 
                    HelperUtilities.GetPositionNearestToPlayer(spawnPos), Quaternion.identity);
        
        if (enemyAnimation != null) {
            int randomIdx = Random.Range(0, enemyAnimationDetailsArray.Length);
            enemyAnimation.gameObject.SetActive(true);
            enemyAnimation.SetAnimation(enemyAnimationDetailsArray[randomIdx].runtimeAnimatorController, enemyAnimationDetailsArray[randomIdx].spriteColor);
        }
    }
}
