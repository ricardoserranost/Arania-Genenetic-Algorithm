using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class VariosExps : MonoBehaviour
{
    public ScriptSpawn PuntoSpawn;
    public TextAsset Experimentos;
    public bool  VariosExperimentosON = false;
    public int NumExperimentos;
    
    private List<int> sizes;    // Size de generación
    private List<int> numGener;  // Núm de individuos
    private List<int> secs;     // Segundos por generación
    private List<float> rM;     // Ratio de mutación
    private List<float> rE;     // Ratio de élite
    private List<int> pC;       // Puntos de cruce

    private int ExpsTerm = 0;   // Experimentos que han terminado


    // Start is called before the first frame update
    void Start()
    {
        if (VariosExperimentosON)
        {
            LeerExpsCsv();
            PuntoSpawn.TERMINADO = true;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (VariosExperimentosON)
        {
            if (PuntoSpawn.TERMINADO)
            {
                // Ponerle el nombre correcto a la carpeta del experimento:
                if (Directory.Exists("Assets/GENOMAS/EXPERIMENTO/"))
                {
                    string pathNuevo = "Assets/GENOMAS/EXP" + ExpsTerm.ToString() + "/";
                    if (Directory.Exists(pathNuevo))
                    {
                        Directory.Delete(pathNuevo, true);
                    }
                    System.IO.Directory.Move("Assets/GENOMAS/EXPERIMENTO/", pathNuevo);
                }
                //------------

                if (ExpsTerm < NumExperimentos)
                {
                    PuntoSpawn.Restart(sizes[ExpsTerm], numGener[ExpsTerm], secs[ExpsTerm], rM[ExpsTerm], rE[ExpsTerm], pC[ExpsTerm]);
                    ExpsTerm++;
                }
                else
                {
                    print("TODOS LOS EXPERIMENTOS TERMINADOS");
                    VariosExperimentosON = false;
                    PuntoSpawn.ExperimentoON = false;
                }
            }
        }
    }

    void LeerExpsCsv()
    {
        // Forma de los parámetros:
        // Size_gen NGen SegsGen Mutation Elite PuntosCruce

        string texto = Experimentos.text;
        texto = texto.Replace('.', ',');
        string[] datos = texto.Split(";"[0]);

        NumExperimentos = datos.Length/6;
        //sizes = new List<int>(NumExperimentos);
        sizes = new List<int>();
        numGener = new List<int>();
        secs = new List<int>();
        rM = new List<float>();
        rE = new List<float>();
        pC = new List<int>();

        int i;
        for (i = 0; i < NumExperimentos; i++)
        {
            sizes.Add(int.Parse(datos[i * 6]));
            numGener.Add(int.Parse(datos[i * 6 + 1]));
            secs.Add(int.Parse(datos[i * 6 + 2]));
            rM.Add(float.Parse(datos[i * 6 + 3]));
            rE.Add(float.Parse(datos[i * 6 + 4]));
            pC.Add(int.Parse(datos[i * 6 + 5]));
        }
    }
}
