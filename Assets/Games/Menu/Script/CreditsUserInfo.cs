using UnityEngine;

public class CreditsUserInfo : MonoBehaviour
{
    [SerializeField] private string url;
    
    public void GoToURL()
    {
        Application.OpenURL(url);
    }
}
