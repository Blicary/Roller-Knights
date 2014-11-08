﻿using UnityEngine;
using System.Collections;

public enum SceneState { Arena, DeadScreen, GameOverScreen, InterArenaCorridor, VictoryRoom }

public class GameManager : MonoBehaviour 
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();

                if (_instance == null) Debug.LogError("Missing GameManger");
                else DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    // scene management
    private static string[] arena_sequence = { "test_scene", "test_scene" };
    private static string inter_arena_scene = "transition_scene";
    private static string game_over_scene = "game_over_scene";
    private static string victory_scene = "victory_scene";

    private static int current_arena = 0;
    public static SceneState Scenestate { get; private set; }



    public void Awake()
    {
        // if this is the first instance, make this the singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else
        {
            // destroy other instances that are not the already existing singleton
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }
    public void Start()
    {
        Scenestate = SceneState.Arena;
    }

    public void Update()
    {
        if (Scenestate == SceneState.DeadScreen && Input.GetKeyDown(KeyCode.Space))
        {
            NextCombatant();
            Scenestate = SceneState.Arena;
        }
            
    }

    public static void ClearArena()
    {
        if (Scenestate == SceneState.Arena)
        {
            current_arena += 1;
            if (current_arena < arena_sequence.Length)
            {
                Scenestate = SceneState.InterArenaCorridor;
                Application.LoadLevel(inter_arena_scene);
            }
            else
            {
                Application.LoadLevel(victory_scene);
            }
        }
        else if (Scenestate == SceneState.InterArenaCorridor)
        {
            Scenestate = SceneState.Arena;
            Application.LoadLevel(arena_sequence[current_arena]);
        }
    }
    public static void DeadScreen()
    {
        Debug.Log("dead screen");
        Scenestate = SceneState.DeadScreen;
        //Application.LoadLevel(dead_scene);
    }
    public static void GameOverScreen()
    {
        Scenestate = SceneState.GameOverScreen;
        Application.LoadLevel(game_over_scene);
    }
    public static void NextCombatant()
    {
        HouseManager.NextCombatant();

        //Application.LoadLevel(arena_sequence[current_arena]);
    }

    // PUBLIC ACCESSORS


}