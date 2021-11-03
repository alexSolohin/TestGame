using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Action<string> GameOver;

    [SerializeField] private TextMeshProUGUI textMesh;

    private void Start()
    {
        GameOver += RestartGame;
        textMesh.text = "";
    }

    private void RestartGame(string message)
    {
        textMesh.text = message;
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
    }

    public void OnDestroy()
    {
        GameOver -= RestartGame;
    }
}
