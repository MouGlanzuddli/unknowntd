using UnityEngine;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemyText;

    void Update()
    {
        //enemyText.text = "Enemies: " + WaveSpawner.enemiesAlive.ToString();
    }
}