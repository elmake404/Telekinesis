using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public UIDrawLine uiDrawLine;
    public GameObject winBlock;
    public GameObject failBlock;
    public Text levelFailText;
    public Text levelWinText;
    public Button restartButton;
    public Button restartButtonInFail;
    public Button nextLevelButton;


    private string keyCurrentLevel = "currentLevel";
    private string keyCountCompletedLevel = "countLevel";
    private int currentIndexLevel;
    private string textTemplate = "Level";


    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        currentIndexLevel = SceneManager.GetActiveScene().buildIndex;
        OnClickEventRestartScene();
        OnClickEventWinScene();
    }

    private void OnClickEventRestartScene()
    {
        restartButton.onClick.AddListener(RestartScene);
        restartButtonInFail.onClick.AddListener(RestartScene);
    }

    private void OnClickEventWinScene()
    {
        nextLevelButton.onClick.AddListener(LoadNextLevel);
    }

    public void LoadNextLevel()
    {
        int maxSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
        Debug.Log(maxSceneIndex);
        int nextSceneIndex = currentIndexLevel + 1;

        if (nextSceneIndex > maxSceneIndex)
        {
            SaveIncreasedCompletedCountLevel();
            SceneManager.LoadScene(0);
        }
        else
        {
            SaveIncreasedCompletedCountLevel();
            SceneManager.LoadScene(nextSceneIndex);
        }

        
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(currentIndexLevel);
    }

    public void SaveIncreasedCompletedCountLevel()
    {
        int currentNum =  PlayerPrefs.GetInt(keyCountCompletedLevel);
        if (currentNum == 0)
        {
            PlayerPrefs.SetInt(keyCountCompletedLevel, 1);
        }
        else
        {
            PlayerPrefs.SetInt(keyCountCompletedLevel, currentNum + 1);
        }

        
    }

    public int GetCompletedLevelCount()
    {
        int num = PlayerPrefs.GetInt(keyCountCompletedLevel);
        if (num == 0) { num = 1; }
        else { num += 1; }
        return num;
    }

    public void SaveLevelId(int numOfLevel)
    {
        PlayerPrefs.SetInt(keyCurrentLevel, numOfLevel);
    }

    public int GetSavedLevelId()
    {
        return PlayerPrefs.GetInt(keyCurrentLevel);
    }

    public void DelayActivateFailBlock()
    {
        levelFailText.text = textTemplate + " " + GetCompletedLevelCount();
        StartCoroutine(RunDelayActivateFailBlock());
    }

    public void DelayActivateWinBlock()
    {
        levelWinText.text = textTemplate + " " + GetCompletedLevelCount();
        StartCoroutine(RunDelayActivateWinBlock());
    }

    private IEnumerator RunDelayActivateFailBlock()
    {
        yield return new WaitForSeconds(2f);
        failBlock.SetActive(true);
        yield return null;
    }

    private IEnumerator RunDelayActivateWinBlock()
    {
        yield return new WaitForSeconds(2f);
        winBlock.SetActive(true);
        yield return null;
    }
}
