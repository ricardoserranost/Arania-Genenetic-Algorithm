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
    public SerieParalelo dropDown;
    public int IndividuosPorTanda = 50;

    [Space(15)]
    [Header("Seguimiento")]

    public int GenerationActual = 0;
    public List<GameObject> aranias = new List<GameObject>();
    private float t;
    public Generation generation;

    void Start() {
        // Intento optimizar la simulación
        if (!Mostrar)
        {
            Renderer[] renderChildren = araniaOriginal.GetComponentsInChildren<Renderer>();
            foreach(Renderer r in renderChildren) r.enabled = false;
        }

        if (OptimizarRendimiento)
        {
            Time.timeScale = TimeScale;
        }

        int i;
        if (ExperimentoON)
        {
            generation = new Generation(size_generacion, 0);

            for (i = 0; i < size_generacion; i++)
            {
                GameObject nuevaArania = Instantiate(araniaOriginal, puntoSpawn.position, Quaternion.identity);
                nuevaArania.GetComponent<ScriptArania>().InitArania(generation.getGenoma(i));
                aranias.Add(nuevaArania);
            }
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        int i = 0;
        if (ExperimentoON)
        {
            if(Time.time > segundosGeneration + t)
            {
                float maxDistance = 0f;
                float minDistance = 0f;
                foreach(Genoma ind in generation.individuos)
                {
                    ind.SetFitness(aranias[i].GetComponent<ScriptArania>().GetFitness());
                    if (ind.GetFitness() > maxDistance) maxDistance = ind.GetFitness();
                    if (ind.GetFitness() < minDistance) minDistance = ind.GetFitness();
                    i++;
                }

                generation.maxFitness = maxDistance;
                generation.minFitness = minDistance;

                foreach (Genoma ind in generation.individuos)
                {
                    //NORMALIZO la fitness para ser sobre 1
                    ind.SetFitness((ind.GetFitness()-minDistance)/(maxDistance-minDistance));  // Hago esta cuenta por haber valores negativos
                }

                generation.Sort();

                print(Time.realtimeSinceStartup);

                generation.Save();

                generation = new Generation(generation);
                RestartAranias();

                t = Time.time;
            }

            GenerationActual = generation.generationID;
        }
	}

    public void RestartAranias()
    {
        int i = 0;
        foreach(GameObject a in aranias)
        {
            GameObject.Destroy(a);
        }
        aranias.Clear();

        if(generation.generationID < NGeneraciones)
        {
            for (i = 0; i < generation.GetNIndividuos(); i++)
            {
                GameObject nuevaArania = Instantiate(araniaOriginal, puntoSpawn.position, Quaternion.identity);
                nuevaArania.GetComponent<ScriptArania>().InitArania(generation.getGenoma(i));
                aranias.Add(nuevaArania);
            }
        }
        else ExperimentoON = false;

    }
}
