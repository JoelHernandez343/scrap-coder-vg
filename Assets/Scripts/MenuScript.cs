using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Sprite[] sprite, menu, text;
    [SerializeField] private Image[] images;
    private Vector2[] rangos;
    private int menuCount;
    [SerializeField] private Image[] Bar;
    private int[] volume;
    [SerializeField] private int[] selectCount = new int[3];
    void Start()
    {
        rangos = new Vector2[6];
        rangos[0] = new Vector2(0, 4);
        rangos[1] = new Vector2(5, 7);
        rangos[2] = new Vector2(8, 15);
        rangos[3] = new Vector2(5, 7);
        rangos[5] = new Vector2(17, 17);
        //images[i] = this.gameObject.transform.GetChild(i).GetComponent<Image>();
        selectCount[0] = 0;
        selectCount[1] = 5;
        selectCount[2] = 8;
        selectCount[3] = 6;
        selectCount[5] = 17;
        volume = new int[3];
        for (int i = 0; i < 3; i++)
            volume[i] = 10;


        menuCount = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        images[0].sprite = menu[menuCount];
        //images[1].sprite = menu[3];
        images[2].sprite = text[menuCount];
        if ((int)rangos[menuCount].x != (int)rangos[menuCount].y)
        {
            if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0) // Arriba
            {
                if (selectCount[menuCount] == rangos[menuCount].x)
                {
                    selectCount[menuCount] = (int)rangos[menuCount].y;
                }
                else
                {
                    selectCount[menuCount]--;
                }
            }
            else if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0) // Abajo
            {
                if (selectCount[menuCount] == rangos[menuCount].y)
                {
                    selectCount[menuCount] = (int)rangos[menuCount].x;
                }
                else
                {
                    selectCount[menuCount]++;
                }
            }
        }
        switch (menuCount)
        {
            case 0:
                MainMenu();
                break;
            case 1:
                ConfigMenu();
                break;
            case 2:
                ConfigTeclas();
                break;
            case 3:
                ConfigMenu();
                break;
            case 4:
                ConfigSonido();
                break;
            case 5:
                Sobre();
                break;
            default:
                break;
        }

        if(this.gameObject.transform.GetChild(4).gameObject.activeInHierarchy == true)
        {
            Salir();
        }
        if (menuCount==1 && selectCount[1] == 6)
        {
            menuCount = 3;
            selectCount[1] = 5;
            this.gameObject.transform.GetChild(3).gameObject.SetActive(true);
            //images[1].gameObject.SetActive(true);
        }
        else if(menuCount == 3 && selectCount[3] == 5)
        {
            menuCount = 1;
            selectCount[3] = 6;
            this.gameObject.transform.GetChild(3).gameObject.SetActive(false);
        }
        images[1].sprite = sprite[selectCount[menuCount]];
    }

    private void MainMenu()
    {
        if (Input.GetButtonDown("Interact"))
        {
            switch (selectCount[0])
            {
                case 0:     /*        Nueva Partida      */
                    SceneManager.LoadScene("SampleScene");
                    /*if (SceneManager.sceneLoaded("SampleScene"))
                    {
                        SceneManager.LoadScene("SampleScene");
                    }*/
                    break;
                case 1:     /*       Cargar Partida      */
                    SceneManager.LoadScene("NodesScene");
                    break;
                case 2:     /*          Opciones         */
                    menuCount = 1;
                    break;
                case 3:     /*      Salir del Juego      */
                    menuCount = 0;
                    break;
                default:    /*           Sobre           */
                    menuCount = 5;
                    break;
            }
            if (selectCount[0] == 3)
            {
                this.gameObject.transform.GetChild(4).gameObject.SetActive(true);
                Salir();
            }
        }
        else if (Input.GetButtonDown("Return"))
        {

            menuCount = 0;
            this.gameObject.transform.GetChild(4).gameObject.SetActive(true);
            Salir();
            
        }
        this.gameObject.transform.GetChild(3).gameObject.SetActive(false);
    }

    private void ConfigMenu()
    {
        if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") > 0 || Input.GetButtonDown("Interact"))
        {
            switch (selectCount[menuCount])
            {
                case 5:     /*    Asignación de Teclas   */
                    menuCount = 2;
                    break;
                case 6:     /*          Sonido           */
                    menuCount = 4;
                    break;
                case 7:
                    menuCount = 0;
                    selectCount[1] = 5;
                    selectCount[3] = 6;
                    break;
            }
        }
        else if (Input.GetButtonDown("Return"))
        {
            menuCount = 0;
            selectCount[1] = 5;
        }

    }

    public void ConfigTeclas()
    {
        if(Input.GetButtonDown("Interact"))
        {
            switch (selectCount[2])
            {
                case 0:     /*        Derecha        */
                    break;
                case 1:     /*       Izquierda       */
                    break;
                case 2:     /*        Arriba         */
                    break;
                case 3:     /*         Abajo         */
                    break;
                case 4:     /*      Interactuar      */
                    break;
                case 5:     /*       Habilidad       */
                    break;
                case 6:     /*         Pausa         */
                    break;
                case 7:     /*       Restaurar       */
                    break;
            }
        }
        else if (Input.GetButtonDown("Return") || Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
        {
            menuCount = 1;
            selectCount[2] = 8;
        }
    }

    private void ConfigSonido()
    {
        
        if (Input.GetButtonDown("Interact"))
        {
            switch (selectCount[menuCount])
            {
                case 0:     /*        Maestro        */
                    break;
                case 1:     /*        Musica          */
                    break;
                case 2:     /*        Efectos         */
                    break;
            }
        }
        else if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") != 0)
        {
            switch (Input.GetAxisRaw("Horizontal"))
            {
                case -1:
                    break;
                case 1:
                    break;
            }
            menuCount = 1;
            selectCount[2] = 8;
        } else if (Input.GetButtonDown("Return"))
        {
            menuCount = 3;
            selectCount[1] = 6;
            selectCount[3] = 5;
        }
    }
    
    private void Salir()
    {
        this.gameObject.transform.GetChild(4).gameObject.SetActive(true);
        if (Input.GetButtonDown("Interact"))
        {
            Application.Quit();
        }
        else if (Input.GetButtonDown("Return"))
        {
            menuCount = 0;
            this.gameObject.transform.GetChild(4).gameObject.SetActive(false);
        }
    }

    private void Sobre()
    {
        if (Input.GetButtonDown("Interact") || Input.GetButtonDown("Return"))
        {
            menuCount = 0;
        }
    }
}

