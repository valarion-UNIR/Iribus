
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("PANELS")]
    [SerializeField] private GameObject newGamePanelPlay;
    [SerializeField] private GameObject gamePanelSettings;
    [SerializeField] private GameObject controlsPanelSettings;
    [SerializeField] private GameObject videoPanelSettings;
    [SerializeField] private GameObject keyBindingPanelSettings;

    [Header("SETTINGS BUTTONS")]
    [SerializeField] private GameObject gameButton;
    [SerializeField] private GameObject controlsButton;
    [SerializeField] private GameObject videoButton;
    [SerializeField] private GameObject keyBindingButton;

    [Header("PLAY BUTTONS")]
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject yesNewButton;
    [SerializeField] private GameObject noNewButton;

    [Header("KEY BINDING BUTTONS")]
    [SerializeField] private GameObject saveKeyBindButton;
    [SerializeField] private GameObject defaultKeyBindButton;

    [Header("PLATFORM BUTTONS")]
    [SerializeField] private Color colorSelected;
    [SerializeField] private Color colorNotSelected;
    [SerializeField] private GameObject pcPlatformButton;
    [SerializeField] private GameObject xboxPlatformButton;
    [SerializeField] private GameObject ps4PlatformButton;

    [Header("PLATFORM SCROLL VIEW")]
    [SerializeField] private GameObject pcPlatformSV;
    [SerializeField] private GameObject xboxPlatformSV;
    [SerializeField] private GameObject ps4PlatformSV;

    [Header("SPRITES")]
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite spriteSelected;

    [Header("FLAGS")]
    [SerializeField] private bool exitsSaveFile;

    [Header("SAVE AND LOAD")]
    [SerializeField] private GameObject saveAndLoadObject;
    [SerializeField] private GameObject checkUnsavedObject;

    private Animator animator;
    private MenuWindow menuWindow;
    private bool HasUnsavedChanges;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = transform.GetComponent<Animator>();

        if (exitsSaveFile) 
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }

        DisableExceptMenu();
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

        if (saveAndLoadObject.GetComponent<RebindSaveLoad>().HasUnsavedChanges())
        {
            saveKeyBindButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            saveKeyBindButton.GetComponent<Button>().interactable = false;
        }
    }

    public void MenuToExit()
    {
        animator.SetTrigger("MenuExit");
        menuWindow = MenuWindow.Exit;
        EnableInteractableItems(exitScreen);
        DisableInteractableItems(mainScreen);
    }

    public void ExitToMenu()
    {
        animator.SetTrigger("ExitMenu");
        menuWindow = MenuWindow.Menu;
        DisableExceptMenu();
    }

    public void MenuToPlay()
    {
        animator.SetTrigger("MenuPlay");
        menuWindow = MenuWindow.Play;
        EnableInteractableItems(playScreen);
        DisableInteractableItems(mainScreen);
    }

    public void PlayToMenu()
    {
        animator.SetTrigger("PlayMenu");
        menuWindow = MenuWindow.Menu;
        DisableExceptMenu();
    }

    public void MenuToSettings()
    {
        animator.SetTrigger("MenuSettings");
        menuWindow = MenuWindow.Settings;
        EnableInteractableItems(settingsScreen);
        DisableInteractableItems(mainScreen);
    }

    public void SettingsToMenu()
    {
        if (saveAndLoadObject.GetComponent<RebindSaveLoad>().HasUnsavedChanges())
        {
            checkUnsavedObject.SetActive(true);
            return;
        }
        animator.SetTrigger("SettingsMenu");
        menuWindow = MenuWindow.Menu;
        DisableExceptMenu();
    }

    public void MenuToCredits()
    {
        animator.SetTrigger("MenuCredits");
        menuWindow = MenuWindow.Credits;
        EnableInteractableItems(creditsScreen);
        DisableInteractableItems(mainScreen);
    }

    public void CreditsToMenu()
    {
        animator.SetTrigger("CreditsMenu");
        menuWindow = MenuWindow.Menu;
        DisableExceptMenu();
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

    private void DisableExceptMenu()
    {
        EnableInteractableItems(mainScreen);
        DisableInteractableItems(playScreen);
        DisableInteractableItems(settingsScreen);
        DisableInteractableItems(creditsScreen);
        DisableInteractableItems(exitScreen);
    }

    public void SaveKeyBinding()
    {
        saveAndLoadObject.GetComponent<RebindSaveLoad>().SaveInputSystemRebindings();
    }

    public void YesUnsavedBinding()
    {
        saveAndLoadObject.GetComponent<RebindSaveLoad>().RevertChanges();
        checkUnsavedObject.SetActive(false);
    }

    public void NoUnsavedBinding()
    {
        checkUnsavedObject.SetActive(false);
    }

    private void DisableInteractableItems(GameObject root)
    {
        if (root == null) return;

        // Activar botón si existe en este GameObject
        Button button = root.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = false;
        }

        // Recorrer hijos recursivamente
        foreach (Transform child in root.transform)
        {
            DisableInteractableItems(child.gameObject);
        }
    }

    private void EnableInteractableItems(GameObject root)
    {
        if (root == null) return;

        // Activar botón si existe en este GameObject
        Button button = root.GetComponent<Button>();
        
        if (button != null && !root.GetComponent<Image>().sprite.Equals(spriteSelected))
        {
            button.interactable = true;
        }

        // Recorrer hijos recursivamente
        foreach (Transform child in root.transform)
        {
            EnableInteractableItems(child.gameObject);
        }
    }

    public void SettingGame()
    {
        if (saveAndLoadObject.GetComponent<RebindSaveLoad>().HasUnsavedChanges())
        {
            checkUnsavedObject.SetActive(true);
            return;
        }
        ActivateSettingsPanels(gamePanelSettings, gameButton);
    }

    public void SettingControls()
    {
        if (saveAndLoadObject.GetComponent<RebindSaveLoad>().HasUnsavedChanges())
        {
            checkUnsavedObject.SetActive(true);
            return;
        }
        ActivateSettingsPanels(controlsPanelSettings, controlsButton);
    }

    public void SettingVideo()
    {
        if (saveAndLoadObject.GetComponent<RebindSaveLoad>().HasUnsavedChanges())
        {
            checkUnsavedObject.SetActive(true);
            return;
        }
        ActivateSettingsPanels(videoPanelSettings, videoButton);
    }

    public void SettingKeyBinding()
    {
        if (saveAndLoadObject.GetComponent<RebindSaveLoad>().HasUnsavedChanges())
        {
            checkUnsavedObject.SetActive(true);
            return;
        }
        ActivateSettingsPanels(keyBindingPanelSettings, keyBindingButton);
    }

    public void PCButton()
    {
        ActivatePlatformPanels(pcPlatformSV, pcPlatformButton);
    }

    public void XboxButton()
    {
        ActivatePlatformPanels(xboxPlatformSV, xboxPlatformButton);
    }

    public void PS4Button()
    {
        ActivatePlatformPanels(ps4PlatformSV, ps4PlatformButton);
    }


    public void NewGame()
    {
        if (!exitsSaveFile)
        {
            //-----------CAMBIAR AQUI----------------
            //Reiniciar save file
            SceneManager.LoadSceneAsync(0);
        }
        else
        {
            ActivatePlayPanels(newGamePanelPlay, newGameButton);
            newGamePanelPlay.SetActive(true);
        }
    }

    public void ContinueGame()
    {
        ActivatePlayPanels(null, continueButton);
        //Cargar save file
        SceneManager.LoadSceneAsync(0);
    }

    private void ActivatePlatformPanels(GameObject panel, GameObject button)
    {
        //Scroll View
        pcPlatformSV.SetActive(false);
        xboxPlatformSV.SetActive(false);
        ps4PlatformSV.SetActive(false);

        //Buttons
        pcPlatformButton.GetComponent<Image>().color = colorNotSelected;
        pcPlatformButton.GetComponent<Button>().interactable = true;
        xboxPlatformButton.GetComponent<Image>().color = colorNotSelected;
        xboxPlatformButton.GetComponent<Button>().interactable = true;
        ps4PlatformButton.GetComponent<Image>().color = colorNotSelected;
        ps4PlatformButton.GetComponent<Button>().interactable = true;

        //Activate Corrects
        panel.SetActive(true);
        button.GetComponent<Image>().color = colorSelected;
        button.GetComponent<Button>().interactable = false;
    }

    private void ActivateSettingsPanels(GameObject panel, GameObject button)
    {
        //Panels
        gamePanelSettings.SetActive(false);
        controlsPanelSettings.SetActive(false);
        videoPanelSettings.SetActive(false);
        keyBindingPanelSettings.SetActive(false);

        //Buttons
        gameButton.GetComponent<Image>().sprite = sprite;
        gameButton.GetComponent<Image>().type = Image.Type.Sliced;
        gameButton.GetComponent<Button>().interactable = true;
        controlsButton.GetComponent<Image>().sprite = sprite;
        controlsButton.GetComponent<Button>().interactable = true;
        controlsButton.GetComponent<Image>().type = Image.Type.Sliced;
        videoButton.GetComponent<Image>().sprite = sprite;
        videoButton.GetComponent<Button>().interactable = true;
        videoButton.GetComponent<Image>().type = Image.Type.Sliced;
        keyBindingButton.GetComponent<Image>().sprite = sprite;
        keyBindingButton.GetComponent<Button>().interactable = true;
        keyBindingButton.GetComponent<Image>().type = Image.Type.Sliced;

        //Activate corrects
        panel.SetActive(true);
        button.GetComponent<Image>().sprite = spriteSelected;
        button.GetComponent<Image>().type = Image.Type.Simple;
        button.GetComponent<Button>().interactable = false;
    }

    private void ActivatePlayPanels(GameObject panel, GameObject button)
    {
        //Panels
        newGamePanelPlay.SetActive(false);

        //Buttons
        continueButton.GetComponent<Image>().sprite = sprite;
        continueButton.GetComponent<Image>().type = Image.Type.Sliced;
        continueButton.GetComponent<Button>().interactable = true;
        newGameButton.GetComponent<Image>().sprite = sprite;
        newGameButton.GetComponent<Button>().interactable = true;
        newGameButton.GetComponent<Image>().type = Image.Type.Sliced;

        //Activate corrects
        if (panel != null) panel.SetActive(true);
        button.GetComponent<Image>().sprite = spriteSelected;
        button.GetComponent<Image>().type = Image.Type.Simple;
        button.GetComponent<Button>().interactable = false;
    }
    // Update is called once per frame
}
