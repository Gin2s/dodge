using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RetryButton : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.StartGame());
    }
}
