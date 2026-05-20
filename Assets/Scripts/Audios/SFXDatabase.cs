using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SFXDatabase",
    menuName = "Audio/SFX Database"
)]
public class SFXDatabase : ScriptableObject
{
    [Serializable]
    public class SFXEntry
    {
        // tipo do efeito sonoro
        public SFXType sfxType;

        // clip do áudio
        public AudioClip clip;
    }

    [Header("Lista de efeitos sonoros")]
    public List<SFXEntry> sfxList =
        new List<SFXEntry>();

    // busca clip pelo tipo
    public AudioClip GetClip(SFXType type)
    {
        foreach (var entry in sfxList)
        {
            if (entry.sfxType == type)
            {
                return entry.clip;
            }
        }

        Debug.LogWarning(
            $"SFX não encontrado: {type}"
        );

        return null;
    }
}