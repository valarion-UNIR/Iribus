using UnityEngine;

public class StackerGameMenu : SubGamePlayerController
{
    public override SubGame SubGame => SubGame.Stacker;

    [SerializeField] private SubGame subGame;

    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject stackerManager;

    private bool gameStarted;

    protected override void Awake()
    {
        base.Awake();
        gameStarted = false;
    }

    private void Update()
    {
        if (Input.Stacker.PutBlock.triggered && !gameStarted)
        {
            menuCanvas.SetActive(false);
            if (stackerManager.TryGetComponent(out StackerSpawner ss))
            {
                ss.enabled = true;
            }
            gameStarted = true;
        }
    }

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
        if (stackerManager.TryGetComponent(out StackerSpawner ss))
        {
            ss.enabled = false;
        }
    }
}
