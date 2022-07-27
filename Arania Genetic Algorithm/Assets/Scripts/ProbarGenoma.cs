using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbarGenoma : MonoBehaviour
{
    public GameObject araniaModelo;
    private GameObject arania;
    public Transform puntoTest;
    public TextAsset genomaTexto;
    public bool ProbarON;
    public Genoma genomaPrueba;

    // Start is called before the first frame update
    void Start()
    {
        if (ProbarON)
        {
            genomaPrueba = new Genoma(8, 8);
            arania = Instantiate(araniaModelo, puntoTest.position, Quaternion.identity);
            Leer();
            arania.GetComponent<ScriptArania>().InitArania(genomaPrueba);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Leer()
    {
        string texto = genomaTexto.text;
        string[] datos = texto.Split(" "[0]);
        float[,,] genes = new float[6, 4, 3];
        int i;
        for(i = 0; i<72; i++)
        {
            genes[((i/3)/4)%6, (i/3)%4, i%3] = float.Parse(datos[i]);
            //Los �ndices son para pasar de un array a la hipermatriz
        }

        genomaPrueba.SetGenes(genes);
    }
}