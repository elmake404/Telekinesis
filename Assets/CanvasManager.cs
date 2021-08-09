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


    private string _keyCurrentLevel = "currentLevel";
    private string _keyCountCompletedLevel = "countLevel";
    private int _currentIndexLevel;
    private string _textTemplate = "Level";


    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        _currentIndexLevel = SceneManager.GetActiveScene().buildIndex;
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
        int nextSceneIndex = _currentIndexLevel + 1;

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
        //SceneManager.LoadScene(_currentIndexLevel);
        SceneManager.LoadScene(0);
    }

    public void SaveIncreasedCompletedCountLevel()
    {
        int currentNum =  PlayerPrefs.GetInt(_keyCountCompletedLevel);
        if (currentNum == 0)
        {
            PlayerPrefs.SetInt(_keyCountCompletedLevel, 1);
        }
        else
        {
            PlayerPrefs.SetInt(_keyCountCompletedLevel, currentNum + 1);
        }

        
    }

    public int GetCompletedLevelCount()
    {
        int num = PlayerPrefs.GetInt(_keyCountCompletedLevel);
        if (num == 0) { num = 1; }
        else { num += 1; }
        return num;
    }

    public void SaveLevelId(int numOfLevel)
    {
        PlayerPrefs.SetInt(_keyCurrentLevel, numOfLevel);
    }

    public int GetSavedLevelId()
    {
        return PlayerPrefs.GetInt(_keyCurrentLevel);
    }

    public void DelayActivateFailBlock()
    {
        levelFailText.text = _textTemplate + " " + GetCompletedLevelCount();
        StartCoroutine(RunDelayActivateFailBlock());
    }

    public void DelayActivateWinBlock()
    {
        levelWinText.text = _textTemplate + " " + GetCompletedLevelCount();
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
        yield return new WaitForSeconds(1f);
        winBlock.SetActive(true);
        yield return null;
    }
}
