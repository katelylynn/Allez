using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneInitializer : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    public GameObject fencerPrefab;
    public GameObject combatManagerPrefab;
    public GameObject environmentPrefab;
    public GameObject scoreUIPrefab;
    public GameObject countdownUIPrefab;

    private GameObject g;

    public FencerType fencer0Type;
    public FencerType fencer1Type;

    void Awake()
    {
        SpawnPrefabs();
        g.GetComponent<GameManager>().StartRound();
    }

    private void SpawnPrefabs()
    {
        /* FENCERS */
        GameObject f0 = Spawn(fencerPrefab);
        f0.GetComponent<Fencer>().Initialize(FencerId.Fencer0, fencer0Type);

        GameObject f1 = Spawn(fencerPrefab);
        f1.GetComponent<Fencer>().Initialize(FencerId.Fencer1, fencer1Type);

        // If the opponent is AI
        if (fencer1Type == FencerType.AI) {
            f1.GetComponent<AI>().enabled = true;
            f1.GetComponent<AI>().Initialize(f0);
        }

        // Set opponent's torso as the aim target for both players
        f0.GetComponent<Fencer>().SetAimTarget(f1.GetComponent<Fencer>().aimTarget);
        f1.GetComponent<Fencer>().SetAimTarget(f0.GetComponent<Fencer>().aimTarget);

        /* MANAGERS */
        g = Spawn(gameManagerPrefab);

        GameObject cm = Spawn(combatManagerPrefab);
        cm.GetComponent<CombatManager>().Initialize(f0.GetComponent<Fencer>(), f1.GetComponent<Fencer>());

        /* UI */
        GameObject scoreUI = Spawn(scoreUIPrefab);
        g.GetComponent<GameManager>().SetUIScore(scoreUI.GetComponent<Canvas>());

        GameObject countdownUI = Spawn(countdownUIPrefab);
        g.GetComponent<GameManager>().SetCountdownTimer(countdownUI.GetComponentInChildren<RoundStartCountDown>());

        // Setup both players UI managers
        UIScoreManager[] uiManagers = scoreUI.GetComponentsInChildren<UIScoreManager>(true);
        foreach (var ui in uiManagers)
        {
            ui.Initialize(g);
            ui.UpdateUI();
        }
      
        /* ENVIRONMENT */
        Spawn(environmentPrefab);
    }

    private GameObject Spawn(GameObject prefab)
    {
        GameObject go = Instantiate(prefab);
        go.name = prefab.name;
        return go;
    }
}
