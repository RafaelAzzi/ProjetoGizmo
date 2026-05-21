using System.Collections.Generic;
using UnityEngine;

// gerencia os loops de áudio dos robôs
public class RobotAudioManager : MonoBehaviour
{
    // singleton
    public static RobotAudioManager Instance;

    [Header("Robot Loop Clips")]

    // todos os clips disponíveis
    public List<AudioClip> robotLoopClips =
        new List<AudioClip>();

    // clips atualmente em uso
    private List<AudioClip> clipsInUse =
        new List<AudioClip>();

    // fila rotativa dos clips
    private Queue<AudioClip> clipQueue =
        new Queue<AudioClip>();

    void Awake()
    {
        // garante singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // cria fila inicial
        SetupQueue();
    }

    // monta fila inicial
    void SetupQueue()
    {
        clipQueue.Clear();

        foreach (AudioClip clip in robotLoopClips)
        {
            // ignora nulos
            if (clip == null)
                continue;

            clipQueue.Enqueue(clip);
        }
    }

    // reserva próximo clip disponível
    public AudioClip ReserveClip()
    {
        // segurança
        if (clipQueue.Count <= 0)
        {
            Debug.LogWarning(
                "Nenhum clip configurado no RobotAudioManager!"
            );

            return null;
        }

        int attempts = clipQueue.Count;

        // tenta encontrar clip livre
        for (int i = 0; i < attempts; i++)
        {
            // pega primeiro da fila
            AudioClip clip = clipQueue.Dequeue();

            // joga para o final
            clipQueue.Enqueue(clip);

            // verifica se está livre
            if (!clipsInUse.Contains(clip))
            {
                // marca como usado
                clipsInUse.Add(clip);

                return clip;
            }
        }

        // nenhum disponível
        Debug.LogWarning(
            "Todos os loops de robô estão em uso!"
        );

        return null;
    }

    // libera clip
    public void ReleaseClip(AudioClip clip)
    {
        // segurança
        if (clip == null)
            return;

        // remove da lista
        if (clipsInUse.Contains(clip))
        {
            clipsInUse.Remove(clip);
        }
    }
}