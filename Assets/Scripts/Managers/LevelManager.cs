using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }

    public Player Player { get; private set; }
    public CameraController Camera { get; private set; }

    public Checkpoint currentCheckPoint { get; set; }
    public Checkpoint DebugSpawn;

    // Stores dead enemies that will revive if the player dies. 
    // Triggering a checkpoint will empty this list.
    private List<GameObject> deadEnemies;

    public void Awake() {
        // Establishes the singleton here to ensure it is assigned before the Start method
        Instance = this;
    }
    
    public void Start() {
        currentCheckPoint = GameObject.FindGameObjectWithTag("StartingPoint").GetComponent<Checkpoint>();
        Player = FindObjectOfType<Player>();
        Camera = FindObjectOfType<CameraController>();
        deadEnemies = new List<GameObject>();
        
#if UNITY_EDITOR
        // If DebugSpawn is set, have it override the starting spawn point
        if (DebugSpawn != null)
            DebugSpawn.SpawnPlayer(Player);
        else if (currentCheckPoint != null)
            currentCheckPoint.SpawnPlayer(Player);
#else
        if (currentCheckPoint != null)
            currentCheckPoint.SpawnPlayer(Player);
#endif
    }

    // TODO change checkpoint handling to fit my game. 
    // This is based on x position. Does not work for vertical levels!
    public void Update() {
        
    }

    public void KillPlayer() {
        StartCoroutine(KillPlayerCo());
    }

    private IEnumerator KillPlayerCo() {
        Player.Kill();
        Camera.IsFollowing = false;
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(ScreenFader.instance.FadeToBlack());

        Camera.IsFollowing = true;
        // Respawn recently killed enemies.
        foreach (var enemy in deadEnemies) {
            var canRespawn = (IRespawnable)enemy.GetComponent(typeof(IRespawnable));
            if (canRespawn != null) {
                enemy.SetActive(true);
                canRespawn.RespawnMe();
            }
        }
        // After enemies are respawned, empty the list.
        ClearDeadEnemies();

        

        if (currentCheckPoint != null)
            currentCheckPoint.SpawnPlayer(Player);

        yield return StartCoroutine(ScreenFader.instance.FadeToClear());
    }

    public void AddDeadEnemy(GameObject enemy) {
        deadEnemies.Add(enemy);
    }

    public void ClearDeadEnemies() {
        deadEnemies.Clear();
    }
}
