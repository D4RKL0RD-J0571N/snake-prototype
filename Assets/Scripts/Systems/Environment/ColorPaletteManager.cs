using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Level;

namespace SnakePrototype.Systems.Environment
{
    /// <summary>
    /// Manages the visual theme of the game by generating and distributing color palettes.
    /// </summary>
    [AddComponentMenu("SnakePrototype/Environment/Color Palette Manager")]
    public class ColorPaletteManager : IGameService
    {
        #region Fields
        public Palette CurrentPalette { get; private set; }
        private int _currentSeed;
        #endregion

        #region IGameService
        public void Initialize()
        {
            CurrentPalette = null;
            Debug.Log("ColorPaletteManager Initialized.");
        }

        public void Shutdown() { }
        #endregion

        #region Public Methods
        public void GeneratePalette(LevelConfig config, int seed = -1)
        {
            CurrentPalette = new Palette();
            
            if (seed != -1) Random.InitState(seed);

            switch (config.ColorTheme)
            {
                case ColorThemeMode.RandomHue:
                    GenerateRandomHuePalette();
                    break;
                case ColorThemeMode.Complementary:
                    GenerateComplementaryPalette();
                    break;
                case ColorThemeMode.HighContrast:
                    GenerateHighContrastPalette();
                    break;
                case ColorThemeMode.Preset:
                default:
                    CurrentPalette.CoreAccent = config.PrimaryColor;
                    CurrentPalette.GridAccent = config.GridColor;
                    CurrentPalette.Background = config.BackgroundColor;
                    CurrentPalette.UIAccent = config.PrimaryColor;
                    CurrentPalette.GuardAccent = config.AlertColor;
                    CurrentPalette.AlertAccent = config.AlertColor;
                    break;
            }

            BroadcastPalette();
        }

        public void BroadcastPalette()
        {
            GameEventManager.Publish(new PaletteChangedEvent(CurrentPalette));
        }
        #endregion

        #region Private Logic
        private void GenerateRandomHuePalette()
        {
            float hue = Random.value;
            CurrentPalette.CoreAccent = Color.HSVToRGB(hue, 0.7f, 0.9f);
            CurrentPalette.Background = Color.HSVToRGB(hue, 0.8f, 0.05f);
            CurrentPalette.GridAccent = Color.HSVToRGB(hue, 0.2f, 0.4f);
            CurrentPalette.UIAccent = CurrentPalette.CoreAccent;
            CurrentPalette.GuardAccent = Color.HSVToRGB((hue + 0.1f) % 1f, 0.9f, 0.8f);
            CurrentPalette.AlertAccent = Color.red;
        }

        private void GenerateComplementaryPalette()
        {
            float hue = Random.value;
            CurrentPalette.CoreAccent = Color.HSVToRGB(hue, 0.8f, 1.0f);
            
            float compHue = (hue + 0.5f) % 1.0f;
            CurrentPalette.Background = Color.HSVToRGB(compHue, 0.9f, 0.05f);
            CurrentPalette.GridAccent = Color.HSVToRGB(hue, 0.3f, 0.5f);
            CurrentPalette.UIAccent = CurrentPalette.CoreAccent;
            CurrentPalette.GuardAccent = Color.HSVToRGB(compHue, 0.7f, 0.8f);
            CurrentPalette.AlertAccent = Color.red;
        }

        private void GenerateHighContrastPalette()
        {
            float hue = Random.value;
            CurrentPalette.CoreAccent = Color.HSVToRGB(hue, 1.0f, 1.0f);
            CurrentPalette.Background = Color.black;
            CurrentPalette.GridAccent = Color.white;
            CurrentPalette.UIAccent = Color.white;
            CurrentPalette.GuardAccent = Color.red;
            CurrentPalette.AlertAccent = Color.red;
        }
        #endregion
    }
}
