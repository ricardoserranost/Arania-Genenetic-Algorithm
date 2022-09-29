using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    generation.maxFitness = generation.individuos[0].fitness;
                    generation.minFitness = generation.individuos[generation.individuos.Count - 1].fitness;
                    generation.NormalizarFitness();


                    print(Time.realtimeSinceStartup);

                    generation.Save();

                    generation = new Generation(generation);
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
        else ExperimentoON = false;
    }
}
