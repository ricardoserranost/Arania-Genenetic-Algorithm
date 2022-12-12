using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptArania : MonoBehaviour {
    public GameObject coxa1, coxa2, coxa3, coxa4, coxa5, coxa6;
    public List<MoverSenoidal> coxas = new List<MoverSenoidal>();
    // Para guardar los puntos por los que va pasando:
    private float tiempoInicial, ultimoTiempo;
    private float rotationAnterior = 0, rotationAbsoluta = 0;
    private List<Vector3> trayectoriaPosiciones = new List<Vector3>();
    private List<float> trayectoriaRotation = new List<float>();

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
	void FixedUpdate () {
        // Se guarda 1 dato de forma periódica
		if((Time.time - ultimoTiempo) >= 0.5)
        {
            // x es el frente de la araña
            // y es el "lateral" hacia la izquierda
            // z es hacia arriba
            // Hago estas operaciones para que la referencia de cada eje coincida
            float x = -transform.Find("thorax").position[0] + transform.position[0];    
            float y = -transform.Find("thorax").position[2] + transform.position[2];
            float z = transform.Find("thorax").position[1] - transform.position[1];
            trayectoriaPosiciones.Add(new Vector3(x, y, z));

            // Guardo la rotación en el eje vertical
            // La rotación absoluta permite saber el ángulo girado desde el instante inicial
            float rotation = (Mathf.Atan2(transform.Find("thorax").right.z, transform.Find("thorax").right.x)) * 360 / (2 * Mathf.PI);
            if (Mathf.Abs(rotation-rotationAnterior) < 170)
            {
                rotationAbsoluta += (rotation-rotationAnterior);
            }
            else
            {
                // Este es el caso en el que la rotacións se ha "pasado" de rango (p. ej. de 179 a -179)
                if (rotation < 0) rotationAbsoluta += (180 + rotation + 180 - rotationAnterior);
                else rotationAbsoluta -= (180 - rotation + 180 + rotationAnterior);
            }
            trayectoriaRotation.Add(rotationAbsoluta);
            ultimoTiempo = Time.time;
            rotationAnterior = rotation;
        }
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
        // Para resetear la trayectoria:
        trayectoriaPosiciones.Clear();
        tiempoInicial = Time.time;
        ultimoTiempo = tiempoInicial;
        trayectoriaPosiciones.Add(new Vector3(0, 0, 0)); // Se añade la posición inicial
        trayectoriaRotation.Add(0);
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
        // -----------------------------------------
        // Aquí se implementa la FUNCIÓN DE FITNESS
        // -----------------------------------------

        // Para la regresión lineal (ir hacia "delante"):
        //return fitnessRegreLineal();

        // Para la rotación sobre el eje vertical
        return fitnessGirar();

        // Solo teniendo en cuenta la pos inicial y final (OBSOLETA):
        //return ((-transform.Find("thorax").position[0] + transform.position[0]) - 0.4f*Mathf.Abs(transform.Find("thorax").position[2] - transform.position[2]));

    }

    private float FitnessRCuadrado()
    {
        return 0;
    }

    public void printTrayectoria()
    {
        foreach(Vector3 punto in trayectoriaPosiciones)
        {
            print(punto);
        }
        foreach(float rot in trayectoriaRotation)
        {
            print(rot);
        }
    }

    public float fitnessRegreLineal()
    {
        float sumx = 0, sumy = 0, sumxy = 0, sumx2 = 0;
        int i = 0;
        float m, y0, fitness;
        int n = trayectoriaPosiciones.Count;
        float SCE = 0, SCT = 0;
        float r2;

        for(i=0; i<n; i++)
        {
            //En este caso se obtiene y = my + y0
            sumx += trayectoriaPosiciones[i].x;
            sumy += trayectoriaPosiciones[i].y;
            sumxy += trayectoriaPosiciones[i].x * trayectoriaPosiciones[i].y;
            sumx2 += trayectoriaPosiciones[i].x * trayectoriaPosiciones[i].x;
        }

        m = (sumxy - (sumx * sumy) / n) / (sumx2 - (sumx * sumx) / n);
        y0 = sumy / n - m * (sumx / n);


        // Calcular el error
        for (i=0; i<n; i++)
        {
            float yteorica = y0 + m * trayectoriaPosiciones[i].x;
            SCE += ((trayectoriaPosiciones[i].y - yteorica)* (trayectoriaPosiciones[i].y - yteorica));
            SCT += (trayectoriaPosiciones[i].y - sumy / n) * (trayectoriaPosiciones[i].y - sumy / n);
        }

        // R-cuadrado: 0 si no se parece a una recta, 1 si sí
        r2 = 1 - SCE / SCT;

        // AQUÍ entra el fitness, donde se pondera cada parámetro
        // Es donde se pueden probar varios
        float alpha = Mathf.Abs(Mathf.Atan2(m, 1))/(Mathf.PI/2);    // Ángulo que forma con la horizontal normalizado a 1
        fitness = (trayectoriaPosiciones[n-1].x + sumx/n) * (1 + 0.5f*r2 - 2.5f*alpha) - Mathf.Abs(y0);
        //Tengo en cuenta la posición final, la posición media(como si contase la "integral", lo recta que es la tray. y la y0)

        // print(trayectoriaPosiciones[n - 1].x.ToString() + " m: " + m.ToString() + " y0: " + y0.ToString() +  " alpha: " + alpha.ToString() + " r2: " + r2.ToString() + " n: " + n.ToString() + " fitness: " + fitness.ToString());
        //print("SCE: " + SCE.ToString() + "SCT: " + SCT.ToString());
        return fitness;
    }

    public float fitnessGirar()
    {
        float fitness = 0, rotMedia = 0, distMedia = 0;
        int n = trayectoriaRotation.Count;
        // Calcular el ángulo acumulado
        // El ángulo instantáneo va de -180 a 180

        foreach(float r in trayectoriaRotation)
        {
            rotMedia += r;
        }
        rotMedia = rotMedia / n;

        foreach(Vector3 pos in trayectoriaPosiciones)
        {
            distMedia += Mathf.Sqrt(Mathf.Pow(pos.x, 2) + Mathf.Pow(pos.y, 2));
        }
        distMedia = distMedia / n;

        fitness = rotMedia / (1 + 10 * distMedia * distMedia);  // El 50 es para ajustar las unidades de distancia

        return fitness;
    }

    public void saveTrayectoria()
    {

    }
}
