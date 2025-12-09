using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager dM;
    private const string filePath = "/playerRecords.dat";
    private const string KEY_AI_DIFFICULTY = "AIDifficulty"; // 0=Easy,1=Normal,2=Hard

    public GameData gameData;
    public string p1;
    public string p2;

    // Selected AI difficulty index (0 = Easy, 1 = Normal, 2 = Hard)
    public int aiDifficultyIndex = 1; // default Normal

    public void Awake()
    {
        if (dM != null && dM != this)
        {
            Destroy(gameObject);
            return;
        }

        dM = this;
        DontDestroyOnLoad(gameObject);

        LoadData();
        if (gameData == null || gameData.data == null || gameData.data.Count == 0)
        {
            CreateFakeData();
            PrintAllPlayerData();
        }

        // Load persisted AI difficulty index (default Normal = 1)
        aiDifficultyIndex = PlayerPrefs.GetInt(KEY_AI_DIFFICULTY, 1);
        aiDifficultyIndex = Mathf.Clamp(aiDifficultyIndex, 0, 2);
    }

    public static PlayerDataManager GetInstance()
    {
        if (dM == null)
        {
            dM = FindFirstObjectByType<PlayerDataManager>();
            if (dM == null)
            {
                GameObject pDM = new GameObject(typeof(PlayerDataManager).Name);
                dM = pDM.AddComponent<PlayerDataManager>();
            }
            DontDestroyOnLoad(dM.gameObject);
        }
        return dM;
    }
    [Serializable]
    public class GameData
    {
        public Dictionary<string, PlayerData> data;
    }
    [Serializable]
    public class PlayerData
    {
        public string name;
        public int gamesWon;
        public int roundsWon;
        public int gamesPlayed;
        public int roundsPlayed;
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + filePath, FileMode.Open, FileAccess.Read);
            GameData data = (GameData)bf.Deserialize(fs);
            fs.Close();
            dM.gameData = data;
            //Debug.Log("LoadData done");
            //PrintAllPlayerData();
        }
        else
        {
            Debug.Log($"{filePath} file not found");
        }
    }

    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open(Application.persistentDataPath + filePath, FileMode.OpenOrCreate);
        GameData gd = new GameData();
        gd.data = new Dictionary<string, PlayerData>();
        foreach (KeyValuePair<string, PlayerData> pair in gameData.data)
        {
            gd.data.Add(pair.Key, pair.Value);
        }
        //data = gameData;
        bf.Serialize(fs, gd);
        fs.Close();
    }

    public List<string> GetAllPlayerNames()
    {
        List<string> playerNames = new List<string>();
        foreach (KeyValuePair<string, PlayerData> pair in gameData.data)
        {
            playerNames.Add(pair.Key);
        }
        return playerNames;
    }
    public void AddNewPlayer(string name)
    {
        if (gameData.data == null)
        {
            Debug.Log("Gamedata is null for some reason, aborting");
            return;
        }
        if (!gameData.data.ContainsKey(name))
        {
            PlayerData newPlayerData = new PlayerData();
            newPlayerData.name = name;
            gameData.data.Add(name, newPlayerData);
            SaveData();
        }
        else
        {
            Debug.Log($"{name} is already in the player list! It will not be added again");
        }
    }

    public void UpdatePlayerDataAfterGame(string name, bool isWinner, int roundsWon, int roundsPlayed)
    {
        if (isWinner) gameData.data[name].gamesWon++;
        gameData.data[name].gamesPlayed++;
        gameData.data[name].roundsWon += roundsWon;
        gameData.data[name].roundsPlayed += roundsPlayed;
    }

    private void ClearPlayerData()
    {
        gameData.data.Clear();
        SaveData();
    }

    private void CreateFakeData()
    {
        if (gameData == null)
        {
            gameData = new GameData();
        }
        if (gameData.data == null)
        {
            gameData.data = new Dictionary<string, PlayerData>();
        }
        ClearPlayerData();
        PlayerData data1 = new PlayerData();
        string[] fakeNames = new string[]
        {
        "FencerFinn", "BladeRider", "ThrustMaster",
        "ParryQueen", "RapierRogue", "LungeLegend",
        "GuardDog", "PointBreaker", "ZorroClone", "FoilFiend"
        };

        System.Random rand = new System.Random();

        foreach (string name in fakeNames)
        {
            PlayerData fakePlayer = new PlayerData();
            fakePlayer.name = name;

            fakePlayer.gamesPlayed = rand.Next(5, 50);
            fakePlayer.gamesWon = rand.Next(0, fakePlayer.gamesPlayed);
            fakePlayer.roundsPlayed = fakePlayer.gamesPlayed * rand.Next(3, 5);
            fakePlayer.roundsWon = (int)(fakePlayer.roundsPlayed * UnityEngine.Random.Range(0.3f, 0.9f));

            gameData.data.Add(name, fakePlayer);
        }

        SaveData();

        Debug.Log($"Created {fakeNames.Length} fake player records and saved to {Application.persistentDataPath + filePath}");
    }

    public void PrintAllPlayerData()
    {
        Debug.Log("Printing list of player names");
        foreach (string name in gameData.data.Keys)
        {
            Debug.Log($"{name} \nGames: {gameData.data[name].gamesPlayed} Games Won: {gameData.data[name].gamesWon} \nRounds: {gameData.data[name].roundsPlayed} Rounds Won: {gameData.data[name].roundsWon}");
        }
    }

    public void IncrementGames(string playerName)
    {
        gameData.data[playerName].gamesPlayed++;
    }

    public void IncrementGamesWon(string playerName)
    {
        gameData.data[playerName].gamesWon++;
    }

    public void IncrementRounds(string playerName)
    {
        gameData.data[playerName].roundsPlayed++;
    }

    public void IncrementRoundsWon(string playerName)
    {
        gameData.data[playerName].roundsWon++;
    }

    public void ClearSelectedPlayers()
    {
        p1 = null;
        p2 = null;
    }
}
