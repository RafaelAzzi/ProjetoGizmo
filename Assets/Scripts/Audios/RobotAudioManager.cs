using System.Collections.Generic;
using UnityEngine;

// gerencia os loops de áudio dos robôs
public class RobotAudioManager : MonoBehaviour
{
    // singleton
    public static RobotAudioManager Instance;

    [Header("Robot Loop Clips")]

    // todos os loops disponíveis
    public List<AudioClip> robotLoopClips =
        new List<AudioClip>();

    // clips atualmente em uso
    private List<AudioClip> clipsInUse =
        new List<AudioClip>();

    void Awake()
    {
        // garante singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // reserva um clip disponível
    public AudioClip ReserveClip()
    {
        // lista temporária
        List<AudioClip> availableClips =
            new List<AudioClip>();

        // procura clips livres
        foreach (AudioClip clip in robotLoopClips)
        {
            if (!clipsInUse.Contains(clip))
            {
                availableClips.Add(clip);
            }
        }

        // nenhum disponível
        if (availableClips.Count <= 0)
        {
            Debug.LogWarning(
                "Nenhum loop de robô disponível!"
            );

            return null;
        }

        // escolhe aleatório
        AudioClip selectedClip =
            availableClips[
                Random.Range(0, availableClips.Count)
            ];

        // marca como usado
        clipsInUse.Add(selectedClip);

        return selectedClip;
    }

    // libera clip quando robô sai
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