using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ProbarGenoma : MonoBehaviour
{
    public GameObject araniaModelo;
    private GameObject arania;
    public Transform puntoTest;
    public TextAsset genomaTexto;
    public TextAsset genomaCsv;
    public bool ProbarON;
    public Genoma genomaPrueba;
    public bool Txt0Csv1;

    // Start is called before the first frame update
    void Start()
    {
        if (ProbarON)
        {
            genomaPrueba = new Genoma(8, 8);
            arania = Instantiate(araniaModelo, puntoTest.position, Quaternion.identity);
            if (Txt0Csv1) LeerCsv();
            else LeerTxt();
            arania.GetComponent<ScriptArania>().InitArania(genomaPrueba);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!ProbarON)
        {
            //arania.GetComponent<ScriptArania>().printTrayectoria();
            //print(arania.GetComponent<ScriptArania>().GetFitness());
            //ProbarON = true;
        }
        
    }

    void LeerTxt()
    {
        string texto = genomaTexto.text;
        string[] datos = texto.Split(" "[0]);
        float[,,] genes = new float[6, 4, 3];
        int i;
        for(i = 0; i<72; i++)
        {
            genes[((i/3)/4)%6, (i/3)%4, i%3] = float.Parse(datos[i]);
            //Los índices son para pasar de un array a la hipermatriz
        }

        genomaPrueba.SetGenes(genes);
    }

    void LeerCsv()
    {
        string texto = genomaCsv.text;
        texto = texto.Replace('.', ',');
        string[] datos = texto.Split(";"[0]);
        float[,,] genes = new float[6, 4, 3];
        int i;
        for (i = 0; i < 72; i++)
        {
            genes[((i / 3) / 4) % 6, (i / 3) % 4, i % 3] = float.Parse(datos[i]);
            //Los índices son para pasar de un array a la hipermatriz
        }
        genomaPrueba.SetGenes(genes);
    }
}
