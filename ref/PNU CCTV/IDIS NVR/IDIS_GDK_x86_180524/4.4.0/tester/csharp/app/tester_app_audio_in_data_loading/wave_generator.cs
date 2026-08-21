using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GDK;

namespace GDK_tester
{
    public class WaveHeader
    {
        public string ChunkID;     // Contains the letters "RIFF" in ASCII form
        public uint ChunkSize;   // This is the size of the rest of the chunk following this number (Total File Size - 8)
        public string Format;    // Contains the letters "WAVE" in ASCII form

        public WaveHeader()
        {
            ChunkSize = 0;
            ChunkID = "RIFF";
            Format = "WAVE";
        }
    }
    public class WaveFormatChuck
    {
        public string ChunkID;     // Contains the letters "fmt " in ASCII form
        public uint ChunkSize;    // 16 for PCM. This is the size of the rest of the Subchunk which follows this number.
        public ushort AudioFormat;   // PCM = 1
        public ushort NumChannels;    // Mono = 1, Stereo = 2, etc.
        public uint SampleRate;    
        public uint AvgByteRate;   // SampleRate * NumChannels * BitsPerSample/8
        public ushort BlockAlign;      // NumChannels * BitsPerSample/8
        public ushort BitPerSample; 

        public WaveFormatChuck(G2AUDIO_CODEC_INFO aci)
        {
            ChunkID = "fmt ";
            ChunkSize = 16;
            AudioFormat = 1;
            NumChannels = aci.channel;
            SampleRate = Convert.ToUInt32(aci.sampling);
            BitPerSample = aci.bits_per_sample;
            BlockAlign = Convert.ToUInt16(NumChannels * (BitPerSample >> 3));
            AvgByteRate = SampleRate * BlockAlign;
        }

    }
    public class WaveDataChunk
    {
        public string ChunkID;     // Contains the letters "data" in ASCII form
        public uint ChunkSize;    // Data Size

        public WaveDataChunk()
        {
            ChunkID = "data";
        }
    }

    class wave_generator
    {
        public wave_generator(G2AUDIO_CODEC_INFO aci)
        {
            header  = new WaveHeader();
            format  = new WaveFormatChuck(aci);
            data    = new WaveDataChunk();
            _decompressed_data_size = 0;
            _is_data_saving       = false;
            _temp_path = Path.GetTempPath() + "temp.dat";
        }
        public void Save(string filePath)
        {
            lock (tempLock)
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(fileStream);
                BinaryReader reader = new BinaryReader(File.Open(@_temp_path, FileMode.Open));
                writer.Write(header.ChunkID.ToCharArray());
                writer.Write(header.ChunkSize);
                writer.Write(header.Format.ToCharArray());

                writer.Write(format.ChunkID.ToCharArray());
                writer.Write(format.ChunkSize);
                writer.Write(format.AudioFormat);
                writer.Write(format.NumChannels);
                writer.Write(format.SampleRate);
                writer.Write(format.AvgByteRate);
                writer.Write(format.BlockAlign);
                writer.Write(format.BitPerSample);

                writer.Write(data.ChunkID.ToCharArray());
                byte[] byte_data = reader.ReadBytes(_decompressed_data_size);
                writer.Write(byte_data.Length);
                foreach (byte dat in byte_data)
                {
                    writer.Write(dat);
                }
                writer.Seek(4, SeekOrigin.Begin);
                uint filesize = (uint)writer.BaseStream.Length;
                writer.Write(filesize - 8);
                reader.Close();
                writer.Close();
                fileStream.Close();
                File.Delete(@_temp_path);
                _is_data_saving = false;
            }
            
        }
        public void SaveDecompressedData(byte[] dec_data)
        {
            // decompressed data save to temp file
            lock (tempLock)
            {
                FileStream fileStream = new FileStream(@_temp_path, FileMode.Append);
                BinaryWriter writer = new BinaryWriter(fileStream);

                writer.Write(dec_data);
                _decompressed_data_size = Convert.ToInt32(writer.BaseStream.Length);
                writer.Close();
                fileStream.Close();
            }
            
        }
        WaveHeader      header;
        WaveFormatChuck format;
        WaveDataChunk   data;
        int             _decompressed_data_size;
        string          _temp_path;
        public bool     _is_data_saving;
        private object tempLock = new object();
    }
}
