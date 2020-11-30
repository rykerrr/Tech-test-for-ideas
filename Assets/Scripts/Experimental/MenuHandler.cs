using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class MenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject cursorOptionsMenu;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            cursorOptionsMenu.SetActive(!cursorOptionsMenu.activeSelf);

            //if(cursorOptionsMenu.activeSelf)
            //{
            //    Cursor.lockState = CursorLockMode.None;
            //}
            //else
            //{
            //    Cursor.lockState = CursorLockMode.Locked;
            //}
        }
    }

}
#pragma warning restore 0649