using UnityEngine;
using UnityEngine.UI;

public class PlayerListEntryInGame : MonoBehaviour
    {
        public Text PlayerNameText;
        public Text ZombieKillCountText;

        public void Initialize(string name, string killCount)
        {
            PlayerNameText.text = name;
            ZombieKillCountText.text = killCount;
        }
    }