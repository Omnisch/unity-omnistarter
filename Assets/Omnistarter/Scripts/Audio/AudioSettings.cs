// author: Omnistudio
// version: 2026.03.15

using System;
using System.Collections.Generic;
using Omnis.Utils;
using UnityEngine;

namespace Omnis.Audio
{
    [CreateAssetMenu(menuName = "Omnis/Audio Settings")]
    public class AudioSettings : ScriptableObject
    {
        public List<SfxInfo> sfx;
        public List<MusicInfo> musics;
    }

    [Serializable]
    public class SfxInfo : AudioInfo
    {
        [SerializeField] [Min(0f)] private float minPlayInterval;
        public float MinPlayInterval => minPlayInterval;

        [HideInInspector] public float lastTimePlayed;
    }

    [Serializable]
    public class MusicInfo : AudioInfo
    {
        [SerializeField] private bool loop = true;
        public bool Loop => loop;
        
        [HideInInspector] public float lastTimePlayed;
    }

    [Serializable]
    public class AudioInfo
    {
        [SerializeField] private string id;
        public string Id => id;

        [SerializeField] [Min(0f)] public float volume = 1f;
        public float Volume => volume;

        [SerializeField] private AudioClip[] audios;
        public AudioClip Get(int index) => audios[index];
        public AudioClip PickRandom() => audios.PickRandom();
    }
}
