﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    public class MenuEndGame : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            SceneManager.UnloadSceneAsync("dejarik");
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnSelected(GameObject gameObject)
        {
            SceneManager.LoadScene("startup");
        }
    }
}
