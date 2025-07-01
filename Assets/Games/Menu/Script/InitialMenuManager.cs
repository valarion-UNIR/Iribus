using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public enum MenuWindow{ Menu, Play, Settings, Credits, Exit }
public class InitialMenuManager : MonoBehaviour
{
    [Header("MENUS")]
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject playScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject exitScreen;

    [Header("LINES")]
    [SerializeField] private GameObject playLine;
    [SerializeField] private GameObject settingsLine;
    [SerializeField] private GameObject creditsLine;
    [SerializeField] private GameObject exitLine;

    private Animator animator;
    private MenuWindow menuWindow;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    void Update()
    {
        switch (menuWindow)
        {
            case MenuWindow.Play:
                playLine.SetActive(true);
                break;
            case MenuWindow.Settings:
                settingsLine.SetActive(true);
                break;
            case MenuWindow.Exit:
                exitLine.SetActive(true);
                break;
            case MenuWindow.Credits:
                creditsLine.SetActive(true);
                break;
            case MenuWindow.Menu:
                break;
        }
    }

    public void MenuToExit()
    {
        animator.SetTrigger("MenuExit");
        menuWindow = MenuWindow.Exit;
    }

    public void ExitToMenu()
    {
        animator.SetTrigger("ExitMenu");
        menuWindow = MenuWindow.Menu;
    }

    public void MenuToPlay()
    {
        animator.SetTrigger("MenuPlay");
        menuWindow = MenuWindow.Play;
    }

    public void PlayToMenu()
    {
        animator.SetTrigger("PlayMenu");
        menuWindow = MenuWindow.Menu;
    }

    public void MenuToSettings()
    {
        animator.SetTrigger("MenuSettings");
        menuWindow = MenuWindow.Settings;
    }

    public void SettingsToMenu()
    {
        animator.SetTrigger("SettingsMenu");
        menuWindow = MenuWindow.Menu;
    }

    public void MenuToCredits()
    {
        animator.SetTrigger("MenuCredits");
        menuWindow = MenuWindow.Credits;
    }

    public void CreditsToMenu()
    {
        animator.SetTrigger("CreditsMenu");
        menuWindow = MenuWindow.Menu;
    }

    public void ExitTheGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
			Application.Quit();
        #endif
    }

    public void LinePlayHover(bool x)
    {
        playLine.SetActive(x);
        creditsLine.SetActive(false);
        settingsLine.SetActive(false);
        exitLine.SetActive(false);
    }

    public void LineSettingsHover(bool x)
    {
        playLine.SetActive(false);
        creditsLine.SetActive(false);
        settingsLine.SetActive(x);
        exitLine.SetActive(false);
    }

    public void LineCreditsHover(bool x)
    {
        playLine.SetActive(false);
        creditsLine.SetActive(x);
        settingsLine.SetActive(false);
        exitLine.SetActive(false);
    }

    public void LineExitHover(bool x)
    {
        playLine.SetActive(false);
        creditsLine.SetActive(false);
        settingsLine.SetActive(false);
        exitLine.SetActive(x);
    }

    // Update is called once per frame
}
