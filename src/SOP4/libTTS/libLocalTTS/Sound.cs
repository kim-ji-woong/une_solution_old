using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Threading;

namespace libTTS
{
	public sealed class SoundUtils
	{
	
		/// <summary>
		/// WAV파일 수행
		/// </summary>
		/// <param name="filename">WAV파일명 또는 경로+파일명</param>
		/// <param name="loop">true:무한제생, false:1번재생</param>
		public static void PlaySound(string filename, bool loop)
		{
			if (File.Exists(filename))
			{
				// Wav file
				//IWavePlayer wavePlayer = new WaveOutEvent();  // Method 1
				IWavePlayer wavePlayer = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Exclusive, false, 100);
				AudioFileReader audioFile = new AudioFileReader(filename);
				audioFile.Volume = (float)1.0;
				//wavePlayer.PlaybackStopped += new EventHandler(wavePlayer_PlaybackStopped);
				wavePlayer.Init(audioFile);

				//wavePlayer.Stop();
				wavePlayer.Play();
				while (wavePlayer.PlaybackState == PlaybackState.Playing)
				{
					Thread.Sleep(100);
				}
				//wavePl
				wavePlayer.Dispose();
			}
		}
	}
}
