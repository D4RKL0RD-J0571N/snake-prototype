using UnityEngine;
using System.Collections.Generic;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Level;

namespace SnakePrototype.Systems.Environment
{
    public class AudioManager : IGameService
    {
        #region Fields
        private AudioSource _musicBaseSource;
        private AudioSource _musicTensionSource;
        private AudioSource _musicAlertSource;
        private AudioSource _sfxSource;
        private AudioSource _ambientSource;
        
        private AudioLowPassFilter _baseLowPass;
        private AudioLowPassFilter _tensionLowPass;
        private AudioLowPassFilter _alertLowPass;
        
        private float _currentDetection;
        private float _targetBaseVolume = 0.5f;
        private float _masterVolume = 0.6f;
        
        private LevelConfig _currentConfig;
        #endregion

        #region Initialization
        public AudioManager(AudioSource musicBase, AudioSource musicTension, AudioSource musicAlert, AudioSource sfx, AudioSource ambient)
        {
            _musicBaseSource = musicBase;
            _musicTensionSource = musicTension;
            _musicAlertSource = musicAlert;
            _sfxSource = sfx;
            _ambientSource = ambient;

            // Try to get or add low pass filters
            _baseLowPass = SetupLowPass(_musicBaseSource);
            _tensionLowPass = SetupLowPass(_musicTensionSource);
            _alertLowPass = SetupLowPass(_musicAlertSource);
        }

        private AudioLowPassFilter SetupLowPass(AudioSource source)
        {
            if (source == null) return null;
            var filter = source.GetComponent<AudioLowPassFilter>();
            if (filter == null) filter = source.gameObject.AddComponent<AudioLowPassFilter>();
            filter.cutoffFrequency = 22000;
            return filter;
        }

        public void Initialize()
        {
            Debug.Log("AudioManager Initialized with Layers");
            GameEventManager.AddListener<DetectionLevelChangedEvent>(OnDetectionChanged);
            GameEventManager.AddListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.AddListener<SnakeDiedEvent>(OnSnakeDied);
            GameEventManager.AddListener<RespawnEvent>(OnRespawn);
            GameEventManager.AddListener<GameStateChangedEvent>(OnGameStateChanged);
            GameEventManager.AddListener<LevelCompleteEvent>(OnLevelComplete);
            GameEventManager.AddListener<LevelStartedEvent>(OnLevelStarted);
            GameEventManager.AddListener<LevelIntroConfirmedEvent>(OnIntroConfirmed);
            
            if (_ambientSource != null && !_ambientSource.isPlaying)
            {
                _ambientSource.loop = true;
                _ambientSource.Play();
            }
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<DetectionLevelChangedEvent>(OnDetectionChanged);
            GameEventManager.RemoveListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.RemoveListener<SnakeDiedEvent>(OnSnakeDied);
            GameEventManager.RemoveListener<RespawnEvent>(OnRespawn);
            GameEventManager.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
            GameEventManager.RemoveListener<LevelCompleteEvent>(OnLevelComplete);
            GameEventManager.RemoveListener<LevelStartedEvent>(OnLevelStarted);
            GameEventManager.RemoveListener<LevelIntroConfirmedEvent>(OnIntroConfirmed);
        }
        #endregion

        #region Core Logic
        public void Tick(float deltaTime)
        {
            float stress = _currentDetection / 100f;
            
            // Volume Layering
            // Base is always on but muffled by stress? Or base is foundation.
            // Tension ramps up from 20% to 80%
            // Alert ramps up from 60% to 100%
            
            float tensionVol = Mathf.Clamp01((_currentDetection - 20f) / 60f);
            float alertVol = Mathf.Clamp01((_currentDetection - 60f) / 40f);
            
            UpdateLayer(_musicBaseSource, _targetBaseVolume * _masterVolume);
            UpdateLayer(_musicTensionSource, tensionVol * _masterVolume);
            UpdateLayer(_musicAlertSource, alertVol * _masterVolume);

            // Filter Modulation: Lower cutoff as things get intense (or vice versa? Usually intense = brighter)
            // Actually, let's muffle the base slightly when tension is high to give it "space".
            if (_baseLowPass != null)
                _baseLowPass.cutoffFrequency = Mathf.Lerp(22000, 5000, tensionVol * 0.5f);
            
            if (_tensionLowPass != null)
                _tensionLowPass.cutoffFrequency = Mathf.Lerp(1000, 22000, tensionVol);

            if (_alertLowPass != null)
                _alertLowPass.cutoffFrequency = Mathf.Lerp(500, 22000, alertVol);
            
            // Pitch modulation
            float pitchMult = 1f + (stress * 0.05f);
            if (_musicBaseSource != null) _musicBaseSource.pitch = pitchMult;
            if (_musicTensionSource != null) _musicTensionSource.pitch = pitchMult;
            if (_musicAlertSource != null) _musicAlertSource.pitch = pitchMult;
        }

        private void UpdateLayer(AudioSource source, float targetVolume)
        {
            if (source == null) return;
            if (!source.isPlaying && targetVolume > 0 && source.clip != null) source.Play();
            source.volume = Mathf.MoveTowards(source.volume, targetVolume, Time.unscaledDeltaTime * 0.5f);
            if (source.volume <= 0 && source.isPlaying) source.Stop();
        }

        public void SetPalette(LevelConfig config)
        {
            if (config == null) return;
            _currentConfig = config;
            
            SetupSource(_musicBaseSource, config.MusicBase);
            SetupSource(_musicTensionSource, config.MusicTension);
            SetupSource(_musicAlertSource, config.MusicAlert);
            
            if (_ambientSource != null && config.AmbientLoop != null)
            {
                _ambientSource.clip = config.AmbientLoop;
                _ambientSource.Play();
            }
        }

        private void SetupSource(AudioSource source, AudioClip clip)
        {
            if (source == null) return;
            source.clip = clip;
            source.loop = true;
            source.volume = 0;
            if (clip != null) source.Play();
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (_sfxSource != null && clip != null)
            {
                _sfxSource.PlayOneShot(clip, volume);
            }
        }
        #endregion

        #region Event Handlers
        private void OnDetectionChanged(DetectionLevelChangedEvent e)
        {
            _currentDetection = e.DetectionLevel;
            
            if (_currentDetection >= 80 && _currentConfig != null && _currentConfig.AlertSFX != null)
            {
                 if (!_sfxSource.isPlaying) PlaySFX(_currentConfig.AlertSFX, 0.4f);
            }
        }

        private void OnEnergyCollected(EnergyCollectedEvent e)
        {
            if (_currentConfig != null && _currentConfig.CollectionSFX != null)
            {
                PlaySFX(_currentConfig.CollectionSFX, 0.6f);
            }
        }

        private void OnSnakeDied(SnakeDiedEvent e)
        {
            if (_currentConfig != null && _currentConfig.DeathSFX != null)
            {
                PlaySFX(_currentConfig.DeathSFX, 0.8f);
            }
            _targetBaseVolume = 0f;
        }

        private void OnRespawn(RespawnEvent e)
        {
            _currentDetection = 0;
            _targetBaseVolume = 0.5f;
        }

        private void OnLevelComplete(LevelCompleteEvent e)
        {
            _targetBaseVolume = 0.1f;
        }

        private void OnLevelStarted(LevelStartedEvent e)
        {
            _targetBaseVolume = 0.5f;
            if (_currentConfig != null && _currentConfig.LevelStartSFX != null)
            {
                PlaySFX(_currentConfig.LevelStartSFX, 0.5f);
            }
        }

        private void OnIntroConfirmed(LevelIntroConfirmedEvent e)
        {
             _targetBaseVolume = 0.5f;
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            if (e.NewState == GameState.GameOver)
            {
                _targetBaseVolume = 0f;
            }
        }
        #endregion
    }
}
