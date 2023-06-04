using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class LaunchURL : MonoBehaviour
    {
        public void GoToURL(string URL)
        {
            Application.OpenURL(URL);
        }
    }
}