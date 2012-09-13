using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Project_Entertainment_Game
{
    public class Audio
    {

        //Variables
        public Song song;
        public SoundEffect sound;
        public SoundEffectInstance soundInstance;
        //private int currentTime = 0; // Uncommented b/c it keeps producing those annoying warnings.
        public bool musicPlaying;

        //Constructor
        public Audio()
        {
            MediaPlayer.IsRepeating = true;
        }

        //Load the music file into the song variable
        public void LoadMusic(String audio)
        {
            song = Game1.INSTANCE.Content.Load<Song>("Music/" + audio);
        }

        //Play the music
        public void PlayMusic()
        {
            if (MediaPlayer.State == MediaState.Stopped || MediaPlayer.State == MediaState.Paused)
            {
   
                MediaPlayer.Play(song);
            }
        }

        //Stop the music
        public void StopMusic()
        {
            MediaPlayer.Stop();
        }

        //Play an sound
        public void PlaySound(String audio, float volume)
        {
        
            sound = Game1.INSTANCE.Content.Load<SoundEffect>("Sounds/" + audio);
            sound.Play(volume, 0.0f, 0.0f);
        }

        public void setMusicVolume(float v)
        {
            MediaPlayer.Volume = v;
        }
    }
}
