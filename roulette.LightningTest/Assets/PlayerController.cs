using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform visor;

    [SerializeField] CharacterController controller;
    [SerializeField]
    [Range(0.01f, 10f)] float xSens = 0.1f;
    [SerializeField]
    [Range(0.01f, 10f)] float ySens = 0.1f;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask interactable;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject tableCamera;
    //[SerializeField] private Animator chipAnim;
    //[SerializeField] private GameObject chip;
    [SerializeField] public List<GameObject> chipsList;
    //PlayerInput pI;

   // private Rigidbody rb;
    private Vector2 moveInput;
    //private bool grounded;

    [SerializeField] float gravity = -9.81f;                //сила прит€жени€
    Vector3 direction = Vector3.zero;                       //направление перемещени€ персонажа
    Vector3 velocity = Vector3.zero;                        //скорость перемещени€ игрока
    bool grounded = true;                                   //флаг, определ€ющий, стоит ли персонаж на поверхности

    Quaternion center;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        center = visor.localRotation;
        //  rb = GetComponent<Rigidbody>();


}

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnLook(InputValue lookValue)
    {
        if (mainCamera.activeSelf == true)

        {
            Vector2 rotation = lookValue.Get<Vector2>();
            float mouseY = rotation.y * ySens;

            Quaternion yRotation = visor.localRotation * Quaternion.AngleAxis(mouseY, -Vector3.right);

            if (Quaternion.Angle(center, yRotation) < 90)
            {
                visor.localRotation = yRotation;
            }

            float mouseX = rotation.x * xSens;
            Quaternion xRotation = player.localRotation * Quaternion.AngleAxis(mouseX, Vector3.up);

            player.localRotation = xRotation;
            //Debug.Log(player.localRotation);
        }
    }

    private void OnMove(InputValue movementValue)    //метод, вызываемый системой ввода при нажатии клавиш перемещени€
    {
        if (mainCamera.activeSelf == true)
        {
            Vector2 movementVector = movementValue.Get<Vector2>();  //можно посмотреть значени€ вектора при помощи Debug.Log
                                                                    //расчЄт направлени€ перемещени€
            direction = new Vector3(movementVector.x, 0, movementVector.y).normalized;
        }
    }

    //—сылок: 0
    private void OnJump()    //метод, вызываемый системой ввода при нажатии клавиши прыжка
    {
        foreach (GameObject chiper in chipsList)
        {
            Debug.Log("Scoring");
            chiper.GetComponent<Animator>().SetTrigger("start");
            chiper.GetComponent<ChipScript>().ChipEffect().Play();
        }
    }

    //—ообщение Unity | —сылок: 0
    private void LateUpdate()
    {
        grounded = controller.isGrounded;    //получение информации о контакте игрока с поверхностью
        controller.Move(transform.TransformDirection(direction) * (speed * Time.deltaTime));    //перемещение в плоскости XZ

        if (grounded && velocity.y < 0) velocity.y = 0; //предотвращение прохождени€ персонажа сквозь поверхность снизу
        velocity.y += gravity * Time.deltaTime;          //применение силы т€жести

        controller.Move(velocity * Time.deltaTime);      //перемещение персонажа
    }

    private void CheckForRaycasts()
    {
        if (mainCamera.activeSelf == true)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 100.0f, interactable))

            {
                //mainCamera.SetActive(false);
                mainCamera.SetActive(false);
                tableCamera.SetActive(true); 

            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }

    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        CheckForRaycasts();
        Debug.Log("Mouse Clicked at: " + Mouse.current.position.ReadValue());
    }

    private void OnAttack()
    {
        Debug.Log("Clicked");
        CheckForRaycasts();
    }
    private void OnCrouch()
    {
        Debug.Log("E");
        LeaveTable();
    }
    private void LeaveTable()
    {
        if (mainCamera.activeSelf == false)
        {
            tableCamera.SetActive(false);
            mainCamera.SetActive(true);
        }
    }
    private void OnScoringStart()
    {
        Debug.Log("Scoring");
        //chipAnim.SetTrigger("start");
        //chip.GetComponent<ChipScript>().ChipEffect().Play();
    }

}

