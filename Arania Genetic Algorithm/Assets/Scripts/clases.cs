using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// PUEDEN SOBRAR MUCHOS GET Y SET PORQUE AL FINAL USO MUCHO PUBLIC

public class Genoma
{
    public float[,,] genes;
    public int genomaID;
    public int generationID;
    public float fitness;
    public float ratioMutation = 0f;
    
    
    public Genoma(int generation, int genoma)
    {
        genomaID = genoma;
        generationID = generation;
        int i, j, k;

        genes = new float[6, 4, 3];     // Pata, parámetro, articulación

        for(i = 0; i<6; i++)
        {
            for(j = 0; j<4; j++)
            {
                for(k=0; k<3; k++)
                {
                    genes[i, j, k] = NewGen(i, j, k);
                }
            }
        }
    }

    public Genoma(Genoma padre, Genoma madre, int generation, int genoma)
    {
        genomaID = genoma;
        generationID = generation;
        int i, j, k;

        genes = new float[6, 4, 3];     // Pata, parámetro, articulación

        for (i = 0; i < 6; i++)
        {
            for (j = 0; j < 4; j++)
            {
                for (k = 0; k < 3; k++)
                {
                    if (Random.value < ratioMutation)
                    {
                        genes[i, j, k] = NewGen(i, j, k);   // Mutar un gen
                    }
                    else
                    {
                        // Aquí está la función de CRUCE . Falta mucho por modificar
                        genes[i, j, k] = BlxAlpha(padre.genes[i, j, k], madre.genes[i, j, k]);

                        //Ahora "capo" los genes por si se van de rango:
                        float max = 0, min = 0;
                        switch (j)
                        {
                            case 0:                                         // frecuencia
                                {
                                    max = 1.5f;
                                    min = 0f;
                                    break;
                                }
                            case 1:                                         // desfase
                                {
                                    max = 2 * Mathf.PI;
                                    min = 0f;
                                    break;
                                }
                            case 2:                                         // amplitud
                                {
                                    max = 45f;
                                    min = 0f;
                                    break;
                                }
                            case 3:                                         // pos central
                                {
                                    switch (k)
                                    {
                                        case 0:
                                            {
                                                max = (60 - genes[i, 2, 0]) / 2f;
                                                min = (genes[i, 2, 0] - 60) / 2f;
                                                break;
                                            }
                                        case 1:
                                            {
                                                max = (90 - genes[i, 2, 1]) / 2f;
                                                min = (genes[i, 2, 0] - 90) / 2f;
                                                break;
                                            }
                                        case 2:
                                            {
                                                max = (65 - genes[i, 2, 2]) / 2f;
                                                min = (genes[i, 2, 0] - 65) / 2f;
                                                break;
                                            }

                                    }
                                    break;
                                }
                        }

                        if (genes[i, j, k] > max) genes[i, j, k] = max;
                        if (genes[i, j, k] < min) genes[i, j, k] = min;
                    }
                }
            }
        }
    }

    public Genoma(Genoma padre, Genoma madre, int nPuntosCruce, int generation, int genoma)
    {
        // Función que cruza 2 genomas cruzando los valores de los padres en n puntos
        genomaID = genoma;
        generationID = generation;
        int i, j, k, section = 0;
        List<int> puntos = new List<int>();

        genes = new float[6, 4, 3];     // Pata, parámetro, articulación

        for(i = 0; i<nPuntosCruce; i++)
        {
            puntos.Add(Random.Range(0, 72));
        }

        for (i = 0; i < 6; i++)
        {
            for (j = 0; j < 4; j++)
            {
                for (k = 0; k < 3; k++)
                {
                    if (Random.value < ratioMutation)
                    {
                        genes[i, j, k] = (NewGen(i, j, k) + genes[i, j, k]) / 2f;   // Mutar un gen
                        //Hago la media entre un valor aleatorio y el actual
                    }
                    else
                    {
                        // Aquí está la función de CRUCE . Falta mucho por modificar
                        if (section % 2 == 0) genes[i, j, k] = padre.genes[i, j, k];
                        else genes[i, j, k] = madre.genes[i, j, k];
                    }

                    if (puntos.Contains(6 * i + j * 4 + k * 3)) section++;
                }
            }
        }
    }

    private float NewGen(int i, int j, int k)
    {
        float max = 0, min = 0;
        switch (j)
        {
            case 0:                                         // frecuencia (estaba a 1.5)
                {
                    max = 0.75f;
                    min = 0f;
                    break;
                }
            case 1:                                         // desfase
                {
                    max = 2 * Mathf.PI;
                    min = 0f;
                    break;
                }
            case 2:                                         // amplitud
                {
                    max = 45f;
                    min = 0f;
                    break;
                }
            case 3:                                         // pos central
                {
                    switch (k)
                    {
                        case 0:
                            {
                                max = (60 - genes[i, 2, 0]) / 2f;
                                min = (genes[i, 2, 0] - 60) / 2f;
                                break;
                            }
                        case 1:
                            {
                                max = (80 - genes[i, 2, 1]) / 2f;
                                min = (genes[i, 2, 0] - 80) / 2f;
                                break;
                            }
                        case 2:
                            {
                                max = (55 - genes[i, 2, 2]) / 2f;
                                min = (genes[i, 2, 0] - 65) / 2f;
                                break;
                            }
                            //Divido entre 2 los valores para no llegar a posiciones "extrañas"
                    }
                    break;
                }
        }

        return Random.Range(min, max);
    }

    private float BlxAlpha(float a, float b)
    {
        //Una forma de cruzar genomas
        float alpha = Random.value;
        float I = Mathf.Abs(a - b);
        return Random.Range(Mathf.Min(a, b) - I*alpha, Mathf.Max(a, b) + I * alpha);
    }

    public void SaveTxt()
    {
        int i, j, k;
        if (!Directory.Exists("Assets/GENOMAS/GENERATION" + generationID.ToString() + "/"))
        {
            Directory.CreateDirectory("Assets/GENOMAS/GENERATION" + generationID.ToString() + "/");
        }
        string path = "Assets/GENOMAS/GENERATION" + generationID.ToString() + "/" + "g" + generationID.ToString() + "_" + genomaID.ToString() + ".txt";
        File.Delete(path);
        StreamWriter writer = new StreamWriter(path, true);

        for(i = 0; i<6; i++)
        {
            for(j = 0; j<4; j++)
            {
                for(k = 0; k<3; k++)
                {
                    writer.Write(genes[i, j, k].ToString() + " ");
                }
                writer.WriteLine("");
            }
            writer.WriteLine("");
        }
        writer.Close();
    }

    public void SaveCsv()
    {
        int i, j, k;
        if (!Directory.Exists("Assets/GENOMAS/EXPERIMENTO/GENERATION" + generationID.ToString() + "/"))
        {
            Directory.CreateDirectory("Assets/GENOMAS/EXPERIMENTO/GENERATION" + generationID.ToString() + "/");
        }
        string path = "Assets/GENOMAS/EXPERIMENTO/GENERATION" + generationID.ToString() + "/" + "g" + generationID.ToString() + "_" + genomaID.ToString() + ".csv";
        File.Delete(path);
        StreamWriter writer = new StreamWriter(path, true);

        for (i = 0; i < 6; i++)
        {
            for (j = 0; j < 4; j++)
            {
                for (k = 0; k < 3; k++)
                {
                    writer.Write(genes[i, j, k].ToString() + ";");
                }
                writer.WriteLine("");
            }
        }
        writer.Close();
    }

    public void SetGenes(float[,,] g)
    {
        genes = g;
    }

    public float[] GetFreq(int coxa)
    {
        float[] freq = new float[3];
        int i;
        for(i = 0; i<3; i++)
        {
            freq[i] = genes[coxa, 0, i];
        }
           
        return freq;
    }

    public float[] GetDesfase(int coxa)
    {
        float[] desfase = new float[3];
        int i;
        for (i = 0; i < 3; i++)
        {
            desfase[i] = genes[coxa, 1, i];
        }

        return desfase;
    }

    public float[] GetAmpl(int coxa)
    {
        float[] amp = new float[3];
        int i;
        for (i = 0; i < 3; i++)
        {
            amp[i] = genes[coxa, 2, i];
        }

        return amp;
    }

    public float[] GetPosCentral(int coxa)
    {
        float[] pos = new float[3];
        int i;
        for (i = 0; i < 3; i++)
        {
            pos[i] = genes[coxa, 3, i];
        }

        return pos;
    }

    public void SetFitness(float f)
    {
        fitness = f;
    }

    public float GetFitness()
    {
        return fitness;
    }

}


public class Generation
{
    public List<Genoma> individuos;
    int nIndividuos;
    public int generationID;
    public float ratioElite = 0.08f;
    public float ratioMutation = 0.01f;
    public int puntosCruce = 2;

    public float maxFitness, minFitness;

    public Generation(int N, int ID)
    {
        int i;
        nIndividuos = N;
        individuos = new List<Genoma>();

        for(i = 0; i<N; i++)
        {
            individuos.Add(new Genoma(ID, i));
        }
    }

    public Generation(Generation anterior)
    {
        nIndividuos = anterior.nIndividuos; //Cambiar esto si hago variable el número de individuos
        generationID = anterior.generationID + 1;
        individuos = new List<Genoma>();

        //SELECCIÓN DE PADRES Y REPRODUCCIÓN ENTRE ELLOS

        //Los pertenecientes a la élite pasan a la siguiente generación, por ahora
        int i = 0;
        while(individuos.Count < nIndividuos * ratioElite)
        {
            anterior.individuos[i].generationID = generationID;
            anterior.individuos[i].genomaID = i;                // Esto se puede cambiar para mantener su identidad
            individuos.Add(anterior.individuos[i]);
            i++;
        }

        while (individuos.Count < nIndividuos)
        {
            foreach (Genoma g in anterior.individuos)
            {
                float chance = Random.value;
                Genoma randomGenoma = anterior.individuos[Random.Range(0, anterior.individuos.Count)];
                if(randomGenoma.GetHashCode() != g.GetHashCode())
                {
                    if (chance < ((g.fitness + randomGenoma.fitness) / 2f))
                    {
                        // Crea un hijo con la probabilidad media entre el padre actual y uno aleatorio de la lista
                        // Un poco "chapucero" pero se puede cambiar. Puede dar emparejamiento con sigo mismo


                        Genoma hijo = new Genoma(g, randomGenoma, puntosCruce, generationID, individuos.Count);
                        hijo.ratioMutation = ratioMutation;
                        individuos.Add(hijo);
                    }
                }
                
                if (individuos.Count >= nIndividuos) break;
            }
        }
        /*
        while (individuos.Count < nIndividuos)
        {
            foreach(Genoma g in anterior.individuos)
            {
                float chance = Random.value;
                if (chance < g.fitness)
                {
                    g.generationID = generationID;
                    individuos.Add(g);
                    anterior.individuos.Remove(g);
                }

                if (individuos.Count >= nIndividuos) break;
            }
        }
        */ //PEGARLE UNA PLANTEADA
    }

    public int GetNIndividuos()
    {
        return nIndividuos;
    }

    public Genoma getGenoma(int N)
    {
        return individuos[N];
    }
    
    public void SetFitness(int n, float fitness)
    {
        individuos[n].SetFitness(fitness);
    }

    public void Save()
    {
        // GUARDAR DATOS DE LA GENERACIÓN, INDIVIDUOS Y SU FITNESS
        if (!Directory.Exists("Assets/GENOMAS/EXPERIMENTO/DataGeneraciones/"))
        {
            Directory.CreateDirectory("Assets/GENOMAS/EXPERIMENTO/DataGeneraciones/");
        }

        string path = "Assets/GENOMAS/EXPERIMENTO/DataGeneraciones/DataGeneration" + generationID.ToString()+ ".txt";
        File.Delete(path);
        StreamWriter writer = new StreamWriter(path, true);

        // GUARDAR CADA GENOMA
        foreach (Genoma individuo in individuos)
        {
            // No guardo todos los individuos, por aligerar:
            if (individuo.generationID % 10 == 9) individuo.SaveCsv();

            writer.WriteLine(individuo.genomaID.ToString() + " :  " + individuo.fitness);
        }

        writer.WriteLine("\nMAX FITNESS: " + maxFitness.ToString() + "\nMIN FITNESS: " + minFitness.ToString());

       writer.Close();
    }

    public void Sort()
    {
        individuos.Sort(CompararFitness);

        maxFitness = individuos[0].fitness;
        minFitness = individuos[individuos.Count - 1].fitness;
    }

    public void NormalizarFitness()
    {
        foreach (Genoma ind in individuos)
        {
            //NORMALIZO la fitness para ser sobre 1
            ind.SetFitness((ind.GetFitness() - minFitness) / (maxFitness - minFitness));
        }
    }

    public void ProbabilidadSumaTotal()
    {
        // En la fitness de cada individuo mete fitness(i)/sumatorio(fitness)
        float suma = 0;
        foreach(Genoma ind in individuos)
        {
            suma += ind.GetFitness();
        }
        foreach(Genoma ind in individuos)
        {
            ind.SetFitness(ind.GetFitness() / suma);
        }
    }

    private static int CompararFitness(Genoma x, Genoma y)
    {
        // Esta función sirve para cuando llamo a la función Sort
        if (x.fitness > y.fitness) return -1;
        else return 1;
    }

}


