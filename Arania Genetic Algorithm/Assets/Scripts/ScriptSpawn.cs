using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Uso una lista de arañas, a las cuales le voy asociando los valores de cada generación
// Este uso hace el código algo engorroso, pero la información se queda definida en las clases de Genoma y Generation

public class ScriptSpawn : MonoBehaviour
{
    public GameObject araniaOriginal;
    public Transform puntoSpawn;
    [Space(15)]
    [Header("Variables a controlar del algoritmo genético")]

    public int size_generacion = 5;
    public int NGeneraciones = 10;
    public float segundosGeneration = 5;
    public float RatioMutation = 0.0f;
    public float RatioElite = 0.08f;
    public int PuntosCruce = 2;

    [Space(15)]
    [Header("Variables de la simulación")]

    public bool ExperimentoON = true;
    public bool Mostrar = true;
    public bool OptimizarRendimiento = false;
    public float TimeScale = 10;
    public enum SerieParalelo { Serie, Paralelo };
    public SerieParalelo DropDownSerieParalelo;
    public int IndividuosPorTanda = 50;

    [Space(15)]
    [Header("Seguimiento")]

    public int GenerationActual = 0;
    public bool TERMINADO = false;
    public List<GameObject> aranias = new List<GameObject>();

    
    private float t;
    public Generation generation;
    private int individuosSimulados = 0;

    void Start() {
        // Intento optimizar la simulación
        if (!Mostrar)
        {
            Renderer[] renderChildren = araniaOriginal.GetComponentsInChildren<Renderer>();
            foreach(Renderer r in renderChildren) r.enabled = false;
        }


        int i;
        if (ExperimentoON)
        {
            generation = new Generation(size_generacion, 0);
            generation.ratioElite = RatioElite;
            generation.ratioMutation = RatioMutation;
            generation.puntosCruce = PuntosCruce;


            if (DropDownSerieParalelo == SerieParalelo.Paralelo)
            {
                IndividuosPorTanda = size_generacion;
            }

            for (i = 0; i < IndividuosPorTanda && i < size_generacion; i++)
            {
                GameObject nuevaArania = Instantiate(araniaOriginal, puntoSpawn.position, Quaternion.identity);
                nuevaArania.GetComponent<ScriptArania>().InitArania(generation.getGenoma(i));
                aranias.Add(nuevaArania);
            }
            individuosSimulados = 0;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (OptimizarRendimiento)
        {
            Time.timeScale = TimeScale; // Se puede poner en Start si no quiero modificarlo mientras está running
        }
        else Time.timeScale = 1f;

        if (ExperimentoON)
        {
            if(Time.time > segundosGeneration + t)
            {
                foreach(GameObject a in aranias)
                {
                    generation.individuos[individuosSimulados].SetFitness(a.GetComponent<ScriptArania>().GetFitness());
                    individuosSimulados++;  // individuosSimulados representa de cuántos de la generación se ha obtenido fitness
                }

                if (individuosSimulados >= generation.GetNIndividuos()) // si es la última serie
                {
                    generation.Sort();
                    SaveData(generation);
                    generation.NormalizarFitness();
                    //generation.ProbabilidadSumaTotal(); // Esto cambia las probabilidades!!! No parece ayudar y ralentiza

                    print(Time.realtimeSinceStartup);

                    generation = new Generation(generation);    //Se crea la nueva generación
                    generation.ratioElite = RatioElite;
                    generation.ratioMutation = RatioMutation;
                    generation.puntosCruce = PuntosCruce;
                    individuosSimulados = 0;
                }

                RespawnAranias();

                t = Time.time;
            }

            GenerationActual = generation.generationID;
        }
	}

    public void RespawnAranias()
    {
        int i = 0;
        foreach(GameObject a in aranias)
        {
            GameObject.Destroy(a);
        }
        aranias.Clear();

        if(generation.generationID < NGeneraciones)
        {
            for (i = 0; (i < (generation.GetNIndividuos() - individuosSimulados)) && (i < IndividuosPorTanda); i++)
            {
                GameObject nuevaArania = Instantiate(araniaOriginal, puntoSpawn.position, Quaternion.identity);
                nuevaArania.GetComponent<ScriptArania>().InitArania(generation.getGenoma(i + individuosSimulados));
                aranias.Add(nuevaArania);
            }
        }
        else
        {
            ExperimentoON = false;
            TERMINADO = true;
        } 
    }

    public void SaveData(Generation gn)
    {
        gn.Save();  // Guardar datos de la generación y todos los genomas

        // Añadir mejor y peor fitness a archivo de datos:
        if (!Directory.Exists("Assets/GENOMAS/EXPERIMENTO/DataGlobal/"))
        {
            Directory.CreateDirectory("Assets/GENOMAS/EXPERIMENTO/DataGlobal/");
        }
        string path = "Assets/GENOMAS/EXPERIMENTO/DataGlobal/FitnessValues.csv";
        if(gn.generationID == 0)File.Delete(path);
        StreamWriter writer = new StreamWriter(path, true);

        //Esto solo escribe la mejor y la peor:
        //writer.WriteLine(gn.maxFitness.ToString() + ";" + gn.minFitness + ";");
        //Mejor guardo todas:
        foreach(Genoma g in gn.individuos)
        {
            writer.Write(g.fitness.ToString() + ";");
        }
        writer.WriteLine("");
        writer.Close();
    }

    public void Restart(int size, int NumGen, int secs, float rM, float rE, int pC)
    {
        print("NUEVO EXPERIMENTO");

        size_generacion = size;
        NGeneraciones = NumGen;
        segundosGeneration = secs;
        RatioMutation = rM;
        RatioElite = rE;
        PuntosCruce = pC;

        TERMINADO = false;
        ExperimentoON = true;

        if (!Mostrar)
        {
            Renderer[] renderChildren = araniaOriginal.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderChildren) r.enabled = false;
        }

        generation = new Generation(size_generacion, 0);
        generation.ratioElite = RatioElite;
        generation.ratioMutation = RatioMutation;
        generation.puntosCruce = PuntosCruce;


        if (DropDownSerieParalelo == SerieParalelo.Paralelo)
        {
            IndividuosPorTanda = size_generacion;
        }

        RespawnAranias();

        individuosSimulados = 0;
    }
}