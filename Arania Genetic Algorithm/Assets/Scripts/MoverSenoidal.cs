using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/* ---------------IMPORTANTE---------------
 * Voy comentando cosas para cambiar, hará falta limpiar el script en algún momento
   ----------------------------------------*/

public class MoverSenoidal : MonoBehaviour {

    public GameObject coxa;     //gira en z
    public GameObject femur;    //gira en x
    public GameObject tibia;    //gira en z

    private float amplitud_coxa, freq_coxa, pos_central_coxa, desfase_coxa, deseada_coxa;
    private float amplitud_femur, freq_femur, pos_central_femur, desfase_femur, deseada_femur;
    private float amplitud_tibia, freq_tibia, pos_central_tibia, desfase_tibia, deseada_tibia;

    private Quaternion inicial_coxa, inicial_femur, inicial_tibia;
    private ArticulationDrive drive_coxa, drive_femur, drive_tibia;



    void Start()
    {
        // VALORES DE ARTICULATION BODY
        drive_coxa = coxa.GetComponent<ArticulationBody>().xDrive;
        drive_coxa.forceLimit = 5f;   // Estimado como 10 kgf*cm, la pongo mayor por haber estimado masas tmbn
        drive_coxa.stiffness = 100;     // Valor alto para que siga la trayectoria fielmente
        drive_coxa.damping = 15;         // Valor experimental, conviene probar y cambiarlo
        coxa.GetComponent<ArticulationBody>().xDrive = drive_coxa;
        coxa.GetComponent<ArticulationBody>().mass = 0.13f;     // 130 g (2 motores (55 g), piezas, tornillos)

        drive_femur = femur.GetComponent<ArticulationBody>().xDrive;
        drive_femur.forceLimit = 5f;
        drive_femur.stiffness = 100;
        drive_femur.damping = 15;
        femur.GetComponent<ArticulationBody>().xDrive = drive_femur;
        femur.GetComponent<ArticulationBody>().mass = 0.02f;    // 20 g (2 placas, tornillos)

        drive_tibia = tibia.GetComponent<ArticulationBody>().xDrive;
        drive_tibia.forceLimit = 5f;
        drive_tibia.stiffness = 100;
        drive_tibia.damping = 15;
        tibia.GetComponent<ArticulationBody>().xDrive = drive_tibia;
        coxa.GetComponent<ArticulationBody>().mass = 0.08f;     // 80 g (motor, pata)
    }
	

	void FixedUpdate () {
        float tiempo = Time.time;
        
        deseada_coxa = pos_central_coxa + (Mathf.Sin((2 * Mathf.PI * tiempo * freq_coxa) + desfase_coxa)) * amplitud_coxa;
        deseada_femur = pos_central_femur + (Mathf.Sin(2 * Mathf.PI * tiempo * freq_femur + desfase_femur)) * amplitud_femur;
        deseada_tibia = pos_central_tibia + (Mathf.Sin(2 * Mathf.PI * tiempo * freq_tibia + desfase_tibia)) * amplitud_tibia;


        // Se manda la pos deseada de la coxa:
        drive_coxa = coxa.GetComponent<ArticulationBody>().xDrive;
        drive_coxa.target = deseada_coxa;
        coxa.GetComponent<ArticulationBody>().xDrive = drive_coxa;

        // Se manda la pos deseada de la femur:
        drive_femur = femur.GetComponent<ArticulationBody>().xDrive;
        drive_femur.target = deseada_femur;
        femur.GetComponent<ArticulationBody>().xDrive = drive_femur;

        // Se manda la pos deseada de la tibia:
        drive_tibia = tibia.GetComponent<ArticulationBody>().xDrive;
        drive_tibia.target = deseada_tibia;
        tibia.GetComponent<ArticulationBody>().xDrive = drive_tibia;

        //Se podría setear con setDriveTargets, pero no lo he conseguido
        //coxa.GetComponent<ArticulationBody>().GetDriveTargets(targets);
        //coxa.GetComponent<ArticulationBody>().SetDriveTargets(targets);

        //Display();
    }

    void Display()
    {
        
    }

    public void SetValoresRandom()
    {
        //Habrá que variar el rango de amplitudes en función de los límites de la articulación
        amplitud_coxa = Random.Range(0f, 45f);
        amplitud_femur = Random.Range(0f, 45f);
        amplitud_tibia = Random.Range(0f, 45f);

        freq_coxa = Random.Range(0f, 1.5f);
        freq_femur = Random.Range(0f, 1.5f);
        freq_tibia = Random.Range(0f, 1.5f);


        // Pongo los límites actuales, es mejor coger los límites desde el articulation body        
        pos_central_coxa = (Random.Range(amplitud_coxa - 60, 60 - amplitud_coxa))/2f;
        pos_central_femur = (Random.Range(amplitud_femur - 90, 90 - amplitud_femur))/ 2f;
        pos_central_tibia = (Random.Range(amplitud_tibia - 65, 55 - amplitud_tibia))/ 2f;

        desfase_coxa = Random.Range(0f, 2 * Mathf.PI);
        desfase_femur = Random.Range(0f, 2 * Mathf.PI);
        desfase_tibia = Random.Range(0f, 2 * Mathf.PI);
    }


    public void SetFreq(float[] freq)
    {
        freq_coxa = freq[0];
        freq_femur = freq[1];
        freq_tibia = freq[2];
    }

    public void SetDesfase(float[] desfase)
    {
        desfase_coxa = desfase[0];
        desfase_femur = desfase[1];
        desfase_tibia = desfase[2];
    }

    public void SetPosCentral(float[] pos)
    {
        pos_central_coxa = pos[0];
        pos_central_femur = pos[1];
        pos_central_tibia = pos[2];
    }
    
    public void SetAmplitudes(float[] amp)
    {
        amplitud_coxa = amp[0];
        amplitud_femur = amp[1];
        amplitud_tibia = amp[2];
    }
}
