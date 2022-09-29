using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptArania : MonoBehaviour {
    public GameObject coxa1, coxa2, coxa3, coxa4, coxa5, coxa6;
    public List<MoverSenoidal> coxas = new List<MoverSenoidal>();

	// Use this for initialization
	void Awake () {
        if (coxas.Count < 6)
        {
            coxas.Add(coxa1.GetComponent<MoverSenoidal>());    //añade a la lista el componente script de  la coxa
            coxas.Add(coxa2.GetComponent<MoverSenoidal>());
            coxas.Add(coxa3.GetComponent<MoverSenoidal>());
            coxas.Add(coxa4.GetComponent<MoverSenoidal>());
            coxas.Add(coxa5.GetComponent<MoverSenoidal>());
            coxas.Add(coxa6.GetComponent<MoverSenoidal>());
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitArania(Genoma genoma)
    {
        int i;
        for(i = 0; i<coxas.Count; i++)
        {
            coxas[i].SetFreq(genoma.GetFreq(i));
            coxas[i].SetDesfase(genoma.GetDesfase(i));
            coxas[i].SetAmplitudes(genoma.GetAmpl(i));
            coxas[i].SetPosCentral(genoma.GetPosCentral(i));
        }
    }

    public void InitArania()
    {
        foreach(MoverSenoidal coxa in coxas)
        {
            coxa.SetValoresRandom();
        }
    }

    public float GetFitness()
    {
        // Aquí se implementa la FUNCIÓN DE FITNESS

        return ((-transform.Find("thorax").position[0] + transform.position[0]) - 0.4f*Mathf.Abs(transform.Find("thorax").position[2] - transform.position[2]));
        //El sentido de las x es en negativo porque el front de la araña está hacia -x
    }
}
